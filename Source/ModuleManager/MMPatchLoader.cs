/*
	This file is part of Module Manager /L
		© 2018-2023 LisiasT
		© 2013-2018 Sarbian; Blowfish
		© 2013 ialdabaoth

	Module Manager /L is licensed as follows:
		* GPL 3.0 : https://www.gnu.org/licenses/gpl-3.0.txt

	Module Manager /L is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.

	You should have received a copy of the GNU General Public License 3.0
	along with Module Manager /L. If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

using ModuleManager.Logging;
using ModuleManager.Extensions;
using ModuleManager.Tags;
using ModuleManager.Patches;
using NodeStack = ModuleManager.Collections.ImmutableStack<ConfigNode>;

using static ModuleManager.FilePathRepository;
using ModuleManager.Utils;

namespace ModuleManager
{
    public class MMPatchLoader : LoadingSystem
    {
        private const string PHYSICS_NODE_NAME = "PHYSICSGLOBALS";

        public string status = "";

        public string errors = "";

        public static bool keepPartDB = false;

        private string configSha;
        private int totalConfigFilesSize;
        private readonly Dictionary<string, string> filesShaMap = new Dictionary<string, string>();
        private readonly Dictionary<string, int> filesSizeMap = new Dictionary<string, int>();

        private const int STATUS_UPDATE_INVERVAL_MS = 33;

        private readonly IEnumerable<ModListGenerator.ModAddedByAssembly> modsAddedByAssemblies;
        private readonly IBasicLogger logger;
        private readonly Progress.ProgressCounter counter;
        private readonly Progress.Timings timings;

        public static void AddPostPatchCallback(ModuleManagerPostPatchCallback callback)
        {
            PostPatchLoader.AddPostPatchCallback(callback);
        }

        public MMPatchLoader(IEnumerable<ModListGenerator.ModAddedByAssembly> modsAddedByAssemblies, IBasicLogger logger, Progress.ProgressCounter counter, Progress.Timings timings)
        {
            this.modsAddedByAssemblies = modsAddedByAssemblies ?? throw new ArgumentNullException(nameof(modsAddedByAssemblies));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.counter = counter;
            this.timings = timings;
        }

        ~MMPatchLoader()
        {   // Being paranoid on memory cleaning...
            this.CleanUpCaches();
            ConfigNodeEditUtils.Instance.Destroy();
        }

        public IEnumerable<IProtoUrlConfig> Run()
        {
            this.timings.Patching.Start();

            status = "Checking Cache";
            logger.Info(status);

#pragma warning disable CS0618 // Type or member is obsolete
			ModuleManager.IsLoadedFromCache = false;
			try
            {
                ModuleManager.IsLoadedFromCache = !ModuleManager.IgnoreCache && IsCacheUpToDate();
            }
            catch (Exception ex)
            {
                logger.Exception(ex, "Exception in IsCacheUpToDate");
            }

            IEnumerable<IProtoUrlConfig> databaseConfigs = null;

            if (!ModuleManager.IsLoadedFromCache)
#pragma warning restore CS0618 // Type or member is obsolete
            {
                IBasicLogger patchLogger = new PatchLogger(FilePathRepository.PATCH_LOG_FILENAME);

                Progress.IPatchProgress progress = new Progress.PatchProgress(patchLogger, this.counter);
                status = "Pre patch init";
                patchLogger.Info(status);
                IEnumerable<string> mods = ModListGenerator.GenerateModList(modsAddedByAssemblies, progress, patchLogger);

                // If we don't use the cache then it is best to clean the PartDatabase.cfg
                if (!keepPartDB && PART_DATABASE.IsLoadable)
                    File.Delete(PART_DATABASE.Path);

                LoadPhysicsConfig();

                #region Sorting Patches

                status = "Extracting patches";
                patchLogger.Info(status);

                UrlDir gameData = GameDatabase.Instance.root.children.First(dir => dir.type == UrlDir.DirectoryType.GameData && dir.name == "");
                INeedsChecker needsChecker = new NeedsChecker(mods, gameData, progress, patchLogger);
                ITagListParser tagListParser = new TagListParser(progress);
                IProtoPatchBuilder protoPatchBuilder = new ProtoPatchBuilder(progress);
                IPatchCompiler patchCompiler = new PatchCompiler(patchLogger);
                PatchExtractor extractor = new PatchExtractor(progress, patchLogger, needsChecker, tagListParser, protoPatchBuilder, patchCompiler);

                // Have to convert to an array because we will be removing patches
                IEnumerable<IPatch> extractedPatches =
                    GameDatabase.Instance.root.AllConfigs.Select(urlConfig => extractor.ExtractPatch(urlConfig)).Where(patch => patch != null);
                PatchList patchList = new PatchList(mods, extractedPatches, progress);

                #endregion

                #region Applying patches

                status = "Applying patches";
                patchLogger.Info(status);

                IPass currentPass = null;

                progress.OnPassStarted.Add(delegate (IPass pass)
                {
                    currentPass = pass;
                    StatusUpdate(progress, currentPass.Name);
                });

                System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                stopwatch.Start();

                progress.OnPatchApplied.Add(delegate
                {
                    long timeRemaining = STATUS_UPDATE_INVERVAL_MS - stopwatch.ElapsedMilliseconds;
                    if (timeRemaining < 0)
                    {
                        StatusUpdate(progress, currentPass.Name);
                        stopwatch.Reset();
                        stopwatch.Start();
                    }
                });

                PatchApplier applier = new PatchApplier(progress, patchLogger);
                databaseConfigs = applier.ApplyPatches(patchList);

                stopwatch.Stop();
                StatusUpdate(progress);

                patchLogger.Info("Done patching");

                #endregion Applying patches

                #region Saving Cache

                foreach (KeyValuePair<string, int> item in this.counter.warningFiles)
                {
                    patchLogger.Warning(item.Value + " warning" + (item.Value > 1 ? "s" : "") + " related to GameData/" + item.Key);
                }

                if (this.counter.errors > 0 || this.counter.exceptions > 0)
                {
                    foreach (KeyValuePair<string, int> item in this.counter.errorFiles)
                    {
                        errors += item.Value + " error" + (item.Value > 1 ? "s" : "") + " related to GameData/" + item.Key
                                  + "\n";
                    }

                    patchLogger.Warning("Errors in patch prevents the creation of the cache");
                    try
                    {
                        CACHE_CONFIG.Destroy();
                        SHA_CONFIG.Destroy();
                    }
                    catch (Exception e)
                    {
                        patchLogger.Exception(e, "Exception while deleting stale cache ");
                    }
                }
                else
                {
                    status = "Saving Cache";
                    CreateCache(databaseConfigs, this.counter.patchedNodes);
                    patchLogger.Info("Cache saved.");
                }

                patchLogger.Finish();
                StatusUpdate(progress);

                #endregion Saving Cache

                SaveModdedTechTree(databaseConfigs);
                SaveModdedPhysics(databaseConfigs);
            }
            else
            {
                status = "Loading from Cache";
                logger.Info(status);
                databaseConfigs = LoadCache();
            }

            // Using an dedicated external log is nice. Dumping it into KSP.log breaking the known formats is not.
            // But...
            // Now I see the reason this was done this way. Asking the user to send KSP.log is tricky by itself, and
            // sending more than one log is yet more troublesome.
            // What I can do is to only dump this on KSP.log when the the KSPe Globals for ModuleManager has Debug set to true.
            // (the default KSPe's Global for Module Manager as from KSPe 2.2.3)
            string patchLogPath = KSPe.IO.File<Startup>.Data.Solve(FilePathRepository.PATCH_LOG_FILENAME + ".log");
            if (File.Exists(patchLogPath))
            {
#pragma warning disable CS0618 // Type or member is obsolete
                if (ModuleManager.IsLoadedFromCache && ModLogger.DebugMode)
#pragma warning restore CS0618 // Type or member is obsolete
                {
                    logger.Info("Dumping patch log");
                    logger.Info("\n#### BEGIN PATCH LOG ####\n\n\n" + File.ReadAllText(patchLogPath) + "\n\n\n#### END PATCH LOG ####");
                }
                else
                    logger.Info("The Patch log can be found on " + patchLogPath);
            }
            else
            {
                logger.Error("Patch log does not exist: " + patchLogPath);
            }

#if !KSP12
            if (KSP.Localization.Localizer.Instance != null)
                KSP.Localization.Localizer.SwitchToLanguage(KSP.Localization.Localizer.CurrentLanguage);
#endif

            logger.Info(status + "\n" + errors);

            // Cleaning some memory
            ConfigNodeEditUtils.Instance.Destroy();
            this.CleanUpCaches();

            this.timings.Patching.Stop();
            logger.Info("Ran in " + this.timings.Patching);

            return databaseConfigs;
        }

        private void LoadPhysicsConfig()
        {
            logger.Info("Loading Physics.cfg");
            UrlDir gameDataDir = GameDatabase.Instance.root.AllDirectories.First(d => d.path.EndsWith("GameData") && d.name == "" && d.url == "");
            // need to use a file with a cfg extension to get the right fileType or you can't AddConfig on it
            UrlDir.UrlFile physicsUrlFile = new UrlDir.UrlFile(gameDataDir, new FileInfo(PHYSICS_DEFAULT.Path));
            // Since it loaded the default config badly (sub node only) we clear it first
            physicsUrlFile.configs.Clear();
            // And reload it properly
            ConfigNode physicsContent = ConfigNode.Load(PHYSICS_DEFAULT.Path); 
            physicsContent.name = PHYSICS_NODE_NAME;
            physicsUrlFile.AddConfig(physicsContent);
            gameDataDir.files.Add(physicsUrlFile);
        }

        private void SaveModdedPhysics(IEnumerable<IProtoUrlConfig> databaseConfigs)
        {
            IEnumerable<IProtoUrlConfig> configs = databaseConfigs.Where(config => config.NodeType == PHYSICS_NODE_NAME);
            int count = configs.Count();

            if (count == 0)
            {
                logger.Info($"No {PHYSICS_NODE_NAME} node found. No custom Physics config will be saved");
                return;
            }

            if (count > 1)
            {
                logger.Info($"{count} {PHYSICS_NODE_NAME} nodes found. A patch may be wrong. Using the first one");
            }

            PHYSICS_CONFIG.Save(configs.First().Node);
        }

        private void CleanUpCaches()
        {
            this.filesShaMap.Clear();
            this.filesSizeMap.Clear();
        }

        private bool IsCacheUpToDate()
        {
            this.timings.ShaCalc.Start();

            System.Security.Cryptography.SHA256 sha = System.Security.Cryptography.SHA256.Create();
            System.Security.Cryptography.SHA256 filesha = System.Security.Cryptography.SHA256.Create();
            UrlDir.UrlFile[] files = GameDatabase.Instance.root.AllConfigFiles.ToArray();
            this.totalConfigFilesSize = 0;

            this.CleanUpCaches();

            for (int i = 0; i < files.Length; i++)
            {
                string url = files[i].GetUrlWithExtension();
                // Hash the file path so the checksum change if files are moved
                byte[] pathBytes = Encoding.UTF8.GetBytes(url);
                sha.TransformBlock(pathBytes, 0, pathBytes.Length, pathBytes, 0);
                this.totalConfigFilesSize += pathBytes.Length;

                // hash the file content
                byte[] contentBytes = File.ReadAllBytes(files[i].fullPath);
                sha.TransformBlock(contentBytes, 0, contentBytes.Length, contentBytes, 0);

                filesha.ComputeHash(contentBytes);
                if (!(this.filesShaMap.ContainsKey(url) || this.filesSizeMap.ContainsKey(url)))
                {
                    this.filesShaMap.Add(url, BitConverter.ToString(filesha.Hash));
                    this.filesSizeMap.Add(url, contentBytes.Length);
                }
                else
                {
                    logger.Warning("Duplicate filesShaMap/filesSizeMap key. This should not append. The key is {0}" + url);
                }
            }

            // Hash the mods dll path so the checksum change if dlls are moved or removed (impact NEEDS)
            foreach (AssemblyLoader.LoadedAssembly dll in AssemblyLoader.loadedAssemblies)
            {
                string path = dll.url + "/" + dll.name;
                byte[] pathBytes = Encoding.UTF8.GetBytes(path);
                sha.TransformBlock(pathBytes, 0, pathBytes.Length, pathBytes, 0);
            }

            // Hash the symbols added by Assemblies, as they impact the patching process.
            {
                IEnumerable<string> symbols = from s in this.modsAddedByAssemblies
                                orderby s.modName
                                select s.modName
                            ;
                foreach (string symbol in symbols)
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(symbol);
                    sha.TransformBlock(bytes, 0, bytes.Length, bytes, 0);
                }
            }

            byte[] godsFinalMessageToHisCreation = Encoding.UTF8.GetBytes("Fork by Lisias.");
            sha.TransformFinalBlock(godsFinalMessageToHisCreation, 0, godsFinalMessageToHisCreation.Length);

            configSha = BitConverter.ToString(sha.Hash);
            sha.Clear();
            filesha.Clear();

            this.timings.ShaCalc.Stop();

            logger.Info("SHA generated in " + this.timings.ShaCalc);
            logger.Info("      SHA = " + configSha);
            logger.Info("     SIZE = " + this.totalConfigFilesSize);

            bool useCache = false;
            if (SHA_CONFIG.IsLoadable)
            {
                SHA_CONFIG.Load();
                logger.Info("ConfigSHA loaded");
                if (null != SHA_CONFIG.Node) try
                {
                    KSPe.ConfigNodeWithSteroids cs = KSPe.ConfigNodeWithSteroids.from(SHA_CONFIG.Node);
                    string storedSHA = cs.GetValue("SHA","");
                    int storedTotalSize = cs.GetValue<int>("SIZE", -1);
                    string version = cs.GetValue("version","");
                    string kspVersion = cs.GetValue("KSPVersion","");
                    ConfigNode filesShaNode = cs.GetNode("FilesSHA");
                    useCache = version.Equals(Assembly.GetExecutingAssembly().GetName().Version.ToString());
                    useCache = useCache && kspVersion.Equals(Versioning.version_major + "." + Versioning.version_minor + "." + Versioning.Revision + "." + Versioning.BuildID);
                    useCache = useCache && storedSHA.Equals(configSha);
                    useCache = useCache && storedTotalSize == this.totalConfigFilesSize;
                    useCache = useCache && CACHE_CONFIG.IsLoadable;
                    useCache = useCache && PHYSICS_CONFIG.IsLoadable;
                    useCache = useCache && TECHTREE_CONFIG.IsLoadable;
                    useCache = useCache && CheckFilesChange(files, filesShaNode);
                    logger.Info("Cache SHA, SIZE = " + storedSHA + ", " + storedTotalSize);
                    logger.Info("useCache = " + useCache);
                }
                catch (Exception e)
                {
                    logger.Error("Error while reading SHA values from cache: " + e.ToString());
                    useCache = false;
                }
            }
            return useCache;
        }

        private bool CheckFilesChange(UrlDir.UrlFile[] files, ConfigNode shaConfigNode)
        {
            bool noChange = true;

            for (int i = 0; i < files.Length; i++)
            {
                string url = files[i].GetUrlWithExtension();
                ConfigNode fileNode = GetFileNode(shaConfigNode, url);
                if (fileNode == null) continue;

                KSPe.ConfigNodeWithSteroids cs = KSPe.ConfigNodeWithSteroids.from(fileNode);
                string fileSha = cs.GetValue("SHA", "");
                int fileSize = cs.GetValue<int>("SIZE", -1);

                if (-1 == fileSize || String.IsNullOrEmpty(fileSha) || filesShaMap[url] != fileSha)
                {
                    logger.Info("Changed : " + fileNode.GetValue("filename") + ".cfg");
                    noChange = false;
                }
            }
            for (int i = 0; i < files.Length; i++)
            {
                string url = files[i].GetUrlWithExtension();
                ConfigNode fileNode = GetFileNode(shaConfigNode, url);

                if (fileNode == null)
                {
                    logger.Info("Added   : " + files[i].url + ".cfg");
                    noChange = false;
                }
                shaConfigNode.RemoveNode(fileNode);
            }
            
            foreach (ConfigNode fileNode in shaConfigNode.GetNodes())
            {
                logger.Info("Deleted : " + fileNode.GetValue("filename") + ".cfg");
                noChange = false;
            }
            
            return noChange;
        }

        private ConfigNode GetFileNode(ConfigNode shaConfigNode, string filename)
        {
            for (int i = 0; i < shaConfigNode.nodes.Count; i++)
            {
                ConfigNode file = shaConfigNode.nodes[i];
                if (file.name == "FILE" && file.GetValue("filename") == filename)
                    return file;
            }
            return null;
        }


        private void CreateCache(IEnumerable<IProtoUrlConfig> databaseConfigs, int patchedNodeCount)
        {
            SHA_CONFIG.Clear();
            SHA_CONFIG.Node.AddValue("SHA", configSha);
            SHA_CONFIG.Node.AddValue("SIZE", this.totalConfigFilesSize);
            SHA_CONFIG.Node.AddValue("version", Assembly.GetExecutingAssembly().GetName().Version.ToString());
            SHA_CONFIG.Node.AddValue("KSPVersion", Versioning.version_major + "." + Versioning.version_minor + "." + Versioning.Revision + "." + Versioning.BuildID);
            ConfigNode filesSHANode = SHA_CONFIG.Node.AddNode("FilesSHA");

            CACHE_CONFIG.Clear();
            CACHE_CONFIG.Node.AddValue("patchedNodeCount", patchedNodeCount.ToString());

            foreach (IProtoUrlConfig urlConfig in databaseConfigs)
            {
                ConfigNode node = CACHE_CONFIG.Node.AddNode("UrlConfig");
                node.AddValue("parentUrl", urlConfig.UrlFile.GetUrlWithExtension());

                ConfigNode urlNode = urlConfig.Node.DeepCopy();
                urlNode.EscapeValuesRecursive();

                node.AddNode(urlNode);
            }

            foreach (UrlDir.UrlFile file in GameDatabase.Instance.root.AllConfigFiles)
            {
                string url = file.GetUrlWithExtension();
                // "/Physics" is the node we created manually to loads the PHYSIC config
                if (file.url != "/Physics" && filesShaMap.ContainsKey(url))
                {
                    ConfigNode shaNode = filesSHANode.AddNode("FILE");
                    shaNode.AddValue("filename", url);
                    shaNode.AddValue("SHA", filesShaMap[url]);
                    shaNode.AddValue("SIZE", this.filesSizeMap[url]);
                    filesShaMap.Remove(url);
                    filesSizeMap.Remove(url);
                }
            }

            logger.Info("Saving cache");

            try
            {
                SHA_CONFIG.Save();
            }
            catch (Exception e)
            {
                logger.Exception(e, "Exception while saving the sha");
            }
            try
            {
                CACHE_CONFIG.Save();
                return;
            }
            catch (NullReferenceException e)
            {
                logger.Exception(e, "NullReferenceException while saving the cache");
            }
            catch (Exception e)
            {
                logger.Exception(e, "Exception while saving the cache");
            }

            try
            {
                logger.Error("An error occured while creating the cache. Deleting the cache files to avoid keeping a bad cache");
                CACHE_CONFIG.Destroy();
                SHA_CONFIG.Destroy();
            }
            catch (Exception e)
            {
                logger.Exception(e, "Exception while deleting the cache");
            }
        }

        private void SaveModdedTechTree(IEnumerable<IProtoUrlConfig> databaseConfigs)
        {
            IEnumerable<IProtoUrlConfig> configs = databaseConfigs.Where(config => config.NodeType == TECHTREE_CONFIG.Node.name);
            int count = configs.Count(); // FIXME: Why didn't he used .Any()?

            if (count == 0)
            {
                logger.Info($"No {TECHTREE_CONFIG.Node.name} node found. No custom {TECHTREE_CONFIG.Node.name} will be saved");
                return;
            }

            if (count > 1)
            {
                logger.Info($"{count} {TECHTREE_CONFIG.Node.name} nodes found. A patch may be wrong. Using the first one");
            }

            TECHTREE_CONFIG.Clear();
            TECHTREE_CONFIG.Node.AddData(configs.First().Node);
            TECHTREE_CONFIG.Save();
        }

        private IEnumerable<IProtoUrlConfig> LoadCache()
        {
            ConfigNode cache = CACHE_CONFIG.Load().Node;

            if (cache.HasValue("patchedNodeCount") && int.TryParse(cache.GetValue("patchedNodeCount"), out int patchedNodeCount))
            { 
                status = "ModuleManager: " + patchedNodeCount + " patch" + (patchedNodeCount != 1 ? "es" : "") +  " loaded from cache";
                this.counter.patchedNodes.Set(patchedNodeCount);
            }

            // Create the fake file where we load the physic config cache
            UrlDir gameDataDir = GameDatabase.Instance.root.AllDirectories.First(d => d.path.EndsWith("GameData") && d.name == "" && d.url == "");
            // need to use a file with a cfg extension to get the right fileType or you can't AddConfig on it
            UrlDir.UrlFile physicsUrlFile = new UrlDir.UrlFile(gameDataDir, new FileInfo(PHYSICS_CONFIG.KspPath));
            gameDataDir.files.Add(physicsUrlFile);

            List<IProtoUrlConfig> databaseConfigs = new List<IProtoUrlConfig>(cache.nodes.Count);

            foreach (ConfigNode node in cache.nodes)
            {
                string parentUrl = node.GetValue("parentUrl");

                UrlDir.UrlFile parent = gameDataDir.Find(parentUrl);
                if (parent != null)
                {
                    node.nodes[0].UnescapeValuesRecursive();
                    databaseConfigs.Add(new ProtoUrlConfig(parent, node.nodes[0]));
                }
                else
                {
                    logger.Warning("Parent null for " + parentUrl);
                }
            }
            logger.Info("Cache Loaded");

            return databaseConfigs;
        }

        private void StatusUpdate(Progress.IPatchProgress progress, string activity = null)
        {
            status = "ModuleManager: " + this.counter.patchedNodes + " patch" + (this.counter.patchedNodes != 1 ? "es" : "") + " applied";
            if (progress.ProgressFraction < 1f - float.Epsilon)
                status += " (" + progress.ProgressFraction.ToString("P0") + ")";

            if (activity != null)
                status += "\n" + activity;

            if (this.counter.warnings > 0)
                status += ", found <color=yellow>" + this.counter.warnings + " warning" + (this.counter.warnings != 1 ? "s" : "") + "</color>";

            if (this.counter.errors > 0)
                status += ", found <color=orange>" + this.counter.errors + " error" + (this.counter.errors != 1 ? "s" : "") + "</color>";

            if (this.counter.exceptions > 0)
                status += ", encountered <color=red>" + this.counter.exceptions + " exception" + (this.counter.exceptions != 1 ? "s" : "") + "</color>";
        }

        #region Applying Patches

        // Name is group 1, index is group 2, vector related filed is group 3, vector separator is group 4, operator is group 5
        private static readonly Regex parseValue = new Regex(@"([\w\&\-\.\?\*\#+/^!\(\) ]+(?:,[^*\d][\w\&\-\.\?\*\(\) ]*)*)(?:,(-?[0-9\*]+))?(?:\[((?:[0-9\*]+)+)(?:,(.))?\])?");

        // ModifyNode applies the ConfigNode mod as a 'patch' to ConfigNode original, then returns the patched ConfigNode.
        // it uses FindConfigNodeIn(src, nodeType, nodeName, nodeTag) to recurse.
        public static ConfigNode ModifyNode(IBasicLogger log, NodeStack original, ConfigNode mod, PatchContext context)
        {
            ConfigNode newNode = original.value.DeepCopy();
            NodeStack nodeStack = original.ReplaceValue(newNode);

            #region Values

            string vals = "modding values";
            foreach (ConfigNode.Value modVal in mod.values)
            {
                vals += "\n   " + modVal.name + "= " + modVal.value;

                Command cmd = CommandParser.Parse(modVal.name, out string valName);

                Operator op;
                if (valName.Length > 2 && valName[valName.Length - 2] == ',')
                    op = Operator.Assign;
                else
                    op = OperatorParser.Parse(valName, out valName);

                if (cmd == Command.Special)
                {
                    ConfigNode.Value val = RecurseVariableSearch(log, valName, nodeStack.Push(mod), context);

                    if (val == null)
                    {
                        context.progress.Error(context.patchUrl, "Error - Cannot find value assigning command: " + valName);
                        continue;
                    }

                    if (op != Operator.Assign)
                    {
                        if (double.TryParse(modVal.value, NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out double s)
                            && double.TryParse(val.value, NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out double os))
                        {
                            switch (op)
                            {
                                case Operator.Multiply:
                                    val.value = (os * s).ToString(CultureInfo.InvariantCulture);
                                    break;

                                case Operator.Divide:
                                    val.value = (os / s).ToString(CultureInfo.InvariantCulture);
                                    break;

                                case Operator.Add:
                                    val.value = (os + s).ToString(CultureInfo.InvariantCulture);
                                    break;

                                case Operator.Subtract:
                                    val.value = (os - s).ToString(CultureInfo.InvariantCulture);
                                    break;

                                case Operator.Exponentiate:
                                    val.value = Math.Pow(os, s).ToString(CultureInfo.InvariantCulture);
                                    break;
                            }
                        }
                    }
                    else
                    {
                        val.value = modVal.value;
                    }
                    continue;
                }

                Match match = parseValue.Match(valName);
                if (!match.Success)
                {
                    context.progress.Error(context.patchUrl, "Error - Cannot parse value modifying command: " + valName);
                    continue;
                }

                // Get the bits and pieces from the regexp
                valName = match.Groups[1].Value;

                // Get a position for editing a vector
                int position = 0;
                bool isPosStar = false;
                if (match.Groups[3].Success)
                {
                    if (match.Groups[3].Value == "*")
                        isPosStar = true;
                    else if (!int.TryParse(match.Groups[3].Value, out position))
                    {
                        context.progress.Error(context.patchUrl, "Error - Unable to parse number as number. Very odd.");
                        continue;
                    }
                }
                char seperator = ',';
                if (match.Groups[4].Success)
                {
                    seperator = match.Groups[4].Value[0];
                }

                // In this case insert the value at position index (with the same node names)
                int index = 0;
                bool isStar = false;
                if (match.Groups[2].Success)
                {
                    if (match.Groups[2].Value == "*")
                        isStar = true;
                    // can have "node,n *" (for *= ect)
                    else if (!int.TryParse(match.Groups[2].Value, out index))
                    {
                        context.progress.Error(context.patchUrl, "Error - Unable to parse number as number. Very odd.");
                        continue;
                    }
                }

                int valCount = 0;
                for (int i=0; i<newNode.CountValues; i++)
                    if (newNode.values[i].name == valName)
                        valCount++;

                string varValue;
                switch (cmd)
                {
                    case Command.Insert:
                        if (match.Groups[5].Success)
                        {
                            context.progress.Error(context.patchUrl, "Error - Cannot use operators with insert value: " + mod.name);
                        }
                        else
                        {
                            // Insert at the end by default
                            varValue = ProcessVariableSearch(log, modVal.value, nodeStack, context);
                            if (varValue != null)
                                InsertValue(newNode, match.Groups[2].Success ? index : int.MaxValue, valName, varValue);
                            else
                                context.progress.Error(context.patchUrl, "Error - Cannot parse variable search when inserting new key " + valName + " = " +
                                    modVal.value);
                        }
                        break;

                    case Command.Replace:
                        if (match.Groups[2].Success || match.Groups[5].Success || valName.Contains('*')
                            || valName.Contains('?'))
                        {
                            if (match.Groups[2].Success)
                                context.progress.Error(context.patchUrl, "Error - Cannot use index with replace (%) value: " + mod.name);
                            if (match.Groups[5].Success)
                                context.progress.Error(context.patchUrl, "Error - Cannot use operators with replace (%) value: " + mod.name);
                            if (valName.Contains('*') || valName.Contains('?'))
                                context.progress.Error(context.patchUrl, "Error - Cannot use wildcards (* or ?) with replace (%) value: " + mod.name);
                        }
                        else
                        {
                            varValue = ProcessVariableSearch(log, modVal.value, nodeStack, context);
                            if (varValue != null)
                            {
                                newNode.RemoveValues(valName);
                                newNode.AddValueSafe(valName, varValue);
                            }
                            else
                            {
                                context.progress.Error(context.patchUrl, "Error - Cannot parse variable search when replacing (%) key " + valName + " = " +
                                    modVal.value);
                            }
                        }
                        break;

                    case Command.Edit:
                    case Command.Copy:

                        // Format is @key = value or @key *= value or @key += value or @key -= value
                        // or @key,index = value or @key,index *= value or @key,index += value or @key,index -= value

                        while (index < valCount)
                        {
                            varValue = ProcessVariableSearch(log, modVal.value, nodeStack, context);

                            if (varValue != null)
                            {
                                string value = ConfigNodeEditUtils.Instance.FindAndReplaceValue(
                                    mod,
                                    ref valName,
                                    varValue, newNode,
                                    op,
                                    index,
                                    out ConfigNode.Value origVal,
                                    context,
                                    match.Groups[3].Success,
                                    position,
                                    isPosStar,
                                    seperator
                                );

                                if (value != null)
                                {
                                    if (origVal.value != value)
                                        vals += ": " + origVal.value + " -> " + value;

                                    if (cmd != Command.Copy)
                                        origVal.value = value;
                                    else
                                        newNode.AddValueSafe(valName, value);
                                }
                            }
                            else
                            {
                                context.progress.Error(context.patchUrl, "Error - Cannot parse variable search when editing key " + valName + " = " + modVal.value);
                            }

                            if (isStar) index++;
                            else break;
                        }
                        break;

                    case Command.Delete:
                        if (match.Groups[5].Success)
                        {
                            context.progress.Error(context.patchUrl, "Error - Cannot use operators with delete (- or !) value: " + mod.name);
                        }
                        else if (match.Groups[2].Success)
                        {
                            while (index < valCount)
                            {
                                // If there is an index, use it.
                                ConfigNode.Value v = ConfigNodeEditUtils.Instance.FindValueIn(newNode, valName, index);
                                if (v != null)
                                    newNode.values.Remove(v);
                                if (isStar) index++;
                                else break;
                            }
                        }
                        else if (valName.Contains('*') || valName.Contains('?'))
                        {
                            // Delete all matching wildcard
                            ConfigNode.Value last = null;
                            while (true)
                            {
                                ConfigNode.Value v = ConfigNodeEditUtils.Instance.FindValueIn(newNode, valName, index++);
                                if (v == last)
                                    break;
                                last = v;
                                newNode.values.Remove(v);
                            }
                        }
                        else
                        {
                            // Default is to delete ALL values that match. (backwards compatibility)
                            newNode.RemoveValues(valName);
                        }
                        break;

                    case Command.Rename:
                        if (nodeStack.IsRoot)
                        {
                            context.progress.Error(context.patchUrl, "Error - Renaming nodes does not work on top nodes");
                            break;
                        }
                        newNode.name = modVal.value;
                        break;

                    case Command.Create:
                        if (match.Groups[2].Success || match.Groups[5].Success || valName.Contains('*')
                            || valName.Contains('?'))
                        {
                            if (match.Groups[2].Success)
                                context.progress.Error(context.patchUrl, "Error - Cannot use index with create (&) value: " + mod.name);
                            if (match.Groups[5].Success)
                                context.progress.Error(context.patchUrl, "Error - Cannot use operators with create (&) value: " + mod.name);
                            if (valName.Contains('*') || valName.Contains('?'))
                                context.progress.Error(context.patchUrl, "Error - Cannot use wildcards (* or ?) with create (&) value: " + mod.name);
                        }
                        else
                        {
                            varValue = ProcessVariableSearch(log, modVal.value, nodeStack, context);
                            if (varValue != null)
                            {
                                if (!newNode.HasValue(valName))
                                    newNode.AddValueSafe(valName, varValue);
                            }
                            else
                            {
                                context.progress.Error(context.patchUrl, "Error - Cannot parse variable search when replacing (&) key " + valName + " = " +
                                    modVal.value);
                            }
                        }
                        break;
                }
            }
            log.Trace(vals);

            #endregion Values

            #region Nodes

            foreach (ConfigNode subMod in mod.nodes)
            {
                subMod.name = subMod.name.RemoveWS();

                if (!subMod.name.IsBracketBalanced())
                {
                    context.progress.Error(context.patchUrl,
                        "Error - Skipping a patch subnode with unbalanced square brackets or a space (replace them with a '?') in "
                        + mod.name + " : \n" + subMod.name + "\n");
                    continue;
                }

                string subName = subMod.name;
                Command command = CommandParser.Parse(subName, out string tmp);

                if (command == Command.Insert)
                {
                    ConfigNode newSubMod = new ConfigNode(subMod.name);
                    newSubMod = ModifyNode(log, nodeStack.Push(newSubMod), subMod, context);
                    subName = newSubMod.name;
                    if (subName.Contains(",") && int.TryParse(subName.Split(',')[1], out int index))
                    {
                        // In this case insert the node at position index (with the same node names)
                        newSubMod.name = subName.Split(',')[0];
                        InsertNode(newNode, newSubMod, index);
                    }
                    else
                    {
                        newNode.AddNode(newSubMod);
                    }
                }
                else if (command == Command.Paste)
                {
                    //int start = subName.IndexOf('[');
                    //int end = subName.LastIndexOf(']');
                    //if (start == -1 || end == -1 || end - start < 1)
                    //{
                    //    log("Pasting a node require a [path] to the node to paste" + mod.name + " : \n" + subMod.name + "\n");
                    //    errorCount++;
                    //    continue;
                    //}

                    //string newName = subName.Substring(0, start);
                    //string path = subName.Substring(start + 1, end - start - 1);

                    ConfigNode toPaste = RecurseNodeSearch(log, subName.Substring(1), nodeStack, context);

                    if (toPaste == null)
                    {
                        context.progress.Error(context.patchUrl, "Error - Can not find the node to paste in " + mod.name + " : " + subMod.name + "\n");
                        continue;
                    }

                    ConfigNode newSubMod = new ConfigNode(toPaste.name);
                    newSubMod = ModifyNode(log, nodeStack.Push(newSubMod), toPaste, context);
                    if (subName.LastIndexOf(',') > 0 && int.TryParse(subName.Substring(subName.LastIndexOf(',') + 1), out int index))
                    {
                        // In this case insert the node at position index
                        InsertNode(newNode, newSubMod, index);
                    }
                    else
                        newNode.AddNode(newSubMod);
                }
                else
                {
                    string constraints = "";
                    string tag = "";
                    string nodeType, nodeName;
                    int index = 0;
                    string logspam_msg = "";
                    List<ConfigNode> subNodes = new List<ConfigNode>();

                    // three ways to specify:
                    // NODE,n will match the nth node (NODE is the same as NODE,0)
                    // NODE,* will match ALL nodes
                    // NODE:HAS[condition] will match ALL nodes with condition
                    if (subName.Contains(":HAS[", out int hasStart))
                    {
                        constraints = subName.Substring(hasStart + 5, subName.LastIndexOf(']') - hasStart - 5);
                        subName = subName.Substring(0, hasStart);
                    }

                    if (subName.Contains(","))
                    {
                        tag = subName.Split(',')[1];
                        subName = subName.Split(',')[0];
                        int.TryParse(tag, out index);
                    }

                    if (subName.Contains("["))
                    {
                        // format @NODETYPE[Name] {...}
                        // or @NODETYPE[Name, index] {...}
                        nodeType = subName.Substring(1).Split('[')[0];
                        nodeName = subName.Split('[')[1].Replace("]", "");
                    }
                    else
                    {
                        // format @NODETYPE {...} or ! instead of @
                        nodeType = subName.Substring(1);
                        nodeName = null;
                    }

                    if (tag == "*" || constraints.Length > 0)
                    {
                        // get ALL nodes
                        if (command != Command.Replace)
                        {
                            ConfigNode n, last = null;
                            while (true)
                            {
                                n = FindConfigNodeIn(newNode, nodeType, nodeName, index++);
                                if (n == last || n == null)
                                    break;
                                if (CheckConstraints(log, n, constraints))
                                    subNodes.Add(n);
                                last = n;
                            }
                        }
                        else
                            logspam_msg += "  cannot wildcard a % node: " + subMod.name + "\n";
                    }
                    else
                    {
                        // just get one node
                        ConfigNode n = FindConfigNodeIn(newNode, nodeType, nodeName, index);
                        if (n != null)
                            subNodes.Add(n);
                    }

                    if (command == Command.Replace)
                    {
                        // if the original exists modify it
                        if (subNodes.Count > 0)
                        {
                            logspam_msg += "  Applying subnode " + subMod.name + "\n";
                            ConfigNode newSubNode = ModifyNode(log, nodeStack.Push(subNodes[0]), subMod, context);
                            subNodes[0].ShallowCopyFrom(newSubNode);
                            subNodes[0].name = newSubNode.name;
                        }
                        else
                        {
                            // if not add the mod node without the % in its name
                            logspam_msg += "  Adding subnode " + subMod.name + "\n";

                            ConfigNode copy = new ConfigNode(nodeType);

                            if (nodeName != null)
                                copy.AddValueSafe("name", nodeName);

                            ConfigNode newSubNode = ModifyNode(log, nodeStack.Push(copy), subMod, context);
                            newNode.nodes.Add(newSubNode);
                        }
                    }
                    else if (command == Command.Create)
                    {
                        if (subNodes.Count == 0)
                        {
                            logspam_msg += "  Adding subnode " + subMod.name + "\n";

                            ConfigNode copy = new ConfigNode(nodeType);

                            if (nodeName != null)
                                copy.AddValueSafe("name", nodeName);

                            ConfigNode newSubNode = ModifyNode(log, nodeStack.Push(copy), subMod, context);
                            newNode.nodes.Add(newSubNode);
                        }
                    }
                    else
                    {
                        // find each original subnode to modify, modify it and add the modified.
                        if (subNodes.Count == 0) // no nodes to modify!
                            logspam_msg += "  Could not find node(s) to modify: " + subMod.name + "\n";

                        foreach (ConfigNode subNode in subNodes)
                        {
                            logspam_msg += "  Applying subnode " + subMod.name + "\n";
                            ConfigNode newSubNode;
                            switch (command)
                            {
                                case Command.Edit:

                                    // Edit in place
                                    newSubNode = ModifyNode(log, nodeStack.Push(subNode), subMod, context);
                                    subNode.ShallowCopyFrom(newSubNode);
                                    subNode.name = newSubNode.name;
                                    break;

                                case Command.Delete:

                                    // Delete the node
                                    newNode.nodes.Remove(subNode);
                                    break;

                                case Command.Copy:

                                    // Copy the node
                                    newSubNode = ModifyNode(log, nodeStack.Push(subNode), subMod, context);
                                    newNode.nodes.Add(newSubNode);
                                    break;
                            }
                        }
                    }
                    log.Trace(logspam_msg);
                }
            }

            #endregion Nodes

            return newNode;
        }


        // Search for a ConfigNode by a path alike string
        private static ConfigNode RecurseNodeSearch(IBasicLogger log, string path, NodeStack nodeStack, PatchContext context)
        {
            //log("Path : \"" + path + "\"");

            if (path[0] == '/')
            {
                return RecurseNodeSearch(log, path.Substring(1), nodeStack.Root, context);
            }

            int nextSep = path.IndexOf('/');

            bool root = (path[0] == '@');
            int shift = root ? 1 : 0;
            string subName = (nextSep != -1) ? path.Substring(shift, nextSep - shift) : path.Substring(shift);
            string nodeType, nodeName;
            string constraint = "";

            int index = 0;
            if (subName.Contains(":HAS[", out int hasStart))
            {
                constraint = subName.Substring(hasStart + 5, subName.LastIndexOf(']') - hasStart - 5);
                subName = subName.Substring(0, hasStart);
            }
            else if (subName.Contains(","))
            {
                string tag = subName.Split(',')[1];
                subName = subName.Split(',')[0];
                int.TryParse(tag, out index);
            }

            if (subName.Contains("["))
            {
                // NODETYPE[Name]
                nodeType = subName.Split('[')[0];
                nodeName = subName.Split('[')[1].Replace("]", "");
            }
            else
            {
                // NODETYPE
                nodeType = subName;
                nodeName = null;
            }

            // ../XXXXX
            if (path.StartsWith("../"))
            {
                if (nodeStack.IsRoot)
                    return null;

                return RecurseNodeSearch(log, path.Substring(3), nodeStack.Pop(), context);
            }

            log.Trace("nextSep : \"{0}\" root : \"{1}\" nodeType : \"{2}\" nodeName : \"{3}\"", nextSep, root, nodeType, nodeName);

            // @XXXXX
            if (root)
            {
                bool foundNodeType = false;
                foreach (IProtoUrlConfig urlConfig in context.databaseConfigs)
                {
                    ConfigNode node = urlConfig.Node;

                    if (node.name != nodeType) continue;

                    foundNodeType = true;

                    if (nodeName == null || (node.GetValue("name") is string testNodeName && ConfigNodeEditUtils.Instance.WildcardMatch(testNodeName, nodeName)))
                    {
                        nodeStack = new NodeStack(node);
                        break;
                    }
                }

                if (!foundNodeType) log.Warning("Can't find nodeType:" + nodeType);
                if (nodeStack == null) return null;
            }
            else
            {
                if (constraint.Length > 0)
                {
                    // get the first one matching
                    ConfigNode last = null;
                    while (true)
                    {
                        ConfigNode n = FindConfigNodeIn(nodeStack.value, nodeType, nodeName, index++);
                        if (n == last || n == null)
                        {
                            nodeStack = null;
                            break;
                        }
                        if (CheckConstraints(log, n, constraint))
                        {
                            nodeStack = nodeStack.Push(n);
                            break;
                        }
                        last = n;
                    }
                }
                else
                {
                    // just get one node
                    nodeStack = nodeStack.Push(FindConfigNodeIn(nodeStack.value, nodeType, nodeName, index));
                }
            }

            // XXXXXX/
            if (nextSep > 0 && nodeStack != null)
            {
                path = path.Substring(nextSep + 1);
                //log("NewPath : \"" + path + "\"");
                return RecurseNodeSearch(log, path, nodeStack, context);
            }

            return nodeStack.value;
        }

        // KeyName is group 1, index is group 2, value index is group 3, value separator is group 4
        private static readonly Regex parseVarKey = new Regex(@"([\w\&\-\.]+)(?:,((?:[0-9]+)+))?(?:\[((?:[0-9]+)+)(?:,(.))?\])?");

        // Search for a value by a path alike string
        private static ConfigNode.Value RecurseVariableSearch(IBasicLogger log, string path, NodeStack nodeStack, PatchContext context)
        {
            //log("path:" + path);
            if (path[0] == '/')
                return RecurseVariableSearch(log, path.Substring(1), nodeStack.Root, context);
            int nextSep = path.IndexOf('/');

            // make sure we don't stop on a ",/" which would be a value separator
            // it's a hack that should be replaced with a proper regex for the whole node search
            while (nextSep > 0 && path[nextSep - 1] == ',')
                nextSep = path.IndexOf('/', nextSep + 1);

            if (path[0] == '@')
            {
                if (nextSep < 2)
                    return null;

                string subName = path.Substring(1, nextSep - 1);
                string nodeType, nodeName;

                if (subName.Contains("["))
                {
                    // @NODETYPE[Name]/
                    nodeType = subName.Split('[')[0];
                    nodeName = subName.Split('[')[1].Replace("]", "");
                }
                else
                {
                    // @NODETYPE/
                    nodeType = subName;
                    nodeName = null;
                }

                bool foundNodeType = false;
                foreach (IProtoUrlConfig urlConfig in context.databaseConfigs)
                {
                    ConfigNode node = urlConfig.Node;

                    if (node.name != nodeType) continue;

                    foundNodeType = true;

                    if (nodeName == null || (node.GetValue("name") is string testNodeName && ConfigNodeEditUtils.Instance.WildcardMatch(testNodeName, nodeName)))
                    {
                        return RecurseVariableSearch(log, path.Substring(nextSep + 1), new NodeStack(node), context);
                    }
                }

                if (!foundNodeType) log.Warning("Can't find nodeType:" + nodeType);

                return null;
            }
            if (path.StartsWith("../"))
            {
                if (nodeStack.IsRoot)
                    return null;

                return RecurseVariableSearch(log, path.Substring(3), nodeStack.Pop(), context);
            }

            // Node search
            if (nextSep > 0 && path[nextSep - 1] != ',')
            {
                // Big case of code duplication here ...
                // TODO : replace with a regex

                string subName = path.Substring(0, nextSep);
                string constraint = "";
                string nodeType, nodeName;
                int index = 0;
                if (subName.Contains(":HAS[", out int hasStart))
                {
                    constraint = subName.Substring(hasStart + 5, subName.LastIndexOf(']') - hasStart - 5);
                    subName = subName.Substring(0, hasStart);
                }
                else if (subName.Contains(','))
                {
                    string tag = subName.Split(',')[1];
                    subName = subName.Split(',')[0];
                    int.TryParse(tag, out index);
                }

                if (subName.Contains("["))
                {
                    // format NODETYPE[Name] {...}
                    // or NODETYPE[Name, index] {...}
                    nodeType = subName.Split('[')[0];
                    nodeName = subName.Split('[')[1].Replace("]", "");
                }
                else
                {
                    // format NODETYPE {...}
                    nodeType = subName;
                    nodeName = null;
                }

                if (constraint.Length > 0)
                {
                    // get the first one matching
                    ConfigNode last = null;
                    while (true)
                    {
                        ConfigNode n = FindConfigNodeIn(nodeStack.value, nodeType, nodeName, index++);
                        if (n == last || n == null)
                            break;
                        if (CheckConstraints(log, n, constraint))
                            return RecurseVariableSearch(log, path.Substring(nextSep + 1), nodeStack.Push(n), context);
                        last = n;
                    }
                    return null;
                }
                else
                {
                    // just get one node
                    ConfigNode n = FindConfigNodeIn(nodeStack.value, nodeType, nodeName, index);
                    if (n != null)
                        return RecurseVariableSearch(log, path.Substring(nextSep + 1), nodeStack.Push(n), context);
                    return null;
                }
            }

            // Value search

            Match match = parseVarKey.Match(path);
            if (!match.Success)
            {
                log.Warning("Cannot parse variable search command: " + path);
                return null;
            }

            string valName = match.Groups[1].Value;

            int idx = 0;
            if (match.Groups[2].Success)
                int.TryParse(match.Groups[2].Value, out idx);

            ConfigNode.Value cVal = ConfigNodeEditUtils.Instance.FindValueIn(nodeStack.value, valName, idx);
            if (cVal == null)
            {
                log.Warning("Cannot find key " + valName + " in " + nodeStack.value.name);
                return null;
            }

            if (match.Groups[3].Success)
            {
                ConfigNode.Value newVal = new ConfigNode.Value(cVal.name, cVal.value);
                int.TryParse(match.Groups[3].Value, out int splitIdx);

                char sep = ',';
                if (match.Groups[4].Success)
                    sep = match.Groups[4].Value[0];
                string[] split = newVal.value.Split(sep);
                if (splitIdx < split.Length)
                    newVal.value = split[splitIdx];
                else
                    newVal.value = "";
                return newVal;
            }
            return cVal;
        }

        private static string ProcessVariableSearch(IBasicLogger log, string value, NodeStack nodeStack, PatchContext context)
        {
            // value = #xxxx$yyyyy$zzzzz$aaaa$bbbb
            // There is 2 or more '$'
            if (value.Length > 0 && value[0] == '#' && value.IndexOf('$') != -1 && value.IndexOf('$') != value.LastIndexOf('$'))
            {
                //log("variable search input : =\"" + value + "\"");
                string[] split = value.Split('$');

                if (split.Length % 2 != 1)
                    return null;

                StringBuilder builder = new StringBuilder();
                builder.Append(split[0].Substring(1));

                for (int i = 1; i < split.Length - 1; i += 2)
                {
                    ConfigNode.Value result = RecurseVariableSearch(log, split[i], nodeStack, context);
                    if (result == null || result.value == null)
                        return null;
                    builder.Append(result.value);
                    builder.Append(split[i + 1]);
                }
                value = builder.ToString();
                log.Info("variable search output : =\"{0}\"", value);
            }
            return value;
        }

        #endregion Applying Patches

        #region Condition checking

        // Split condiction while not getting lost in embeded brackets
        public static List<string> SplitConstraints(string condition)
        {
            condition = condition.RemoveWS() + ",";
            List<string> conditions = new List<string>();
            int start = 0;
            int level = 0;
            for (int end = 0; end < condition.Length; end++)
            {
                if ((condition[end] == ',' || condition[end] == '&') && level == 0)
                {
                    conditions.Add(condition.Substring(start, end - start));
                    start = end + 1;
                }
                else if (condition[end] == '[')
                    level++;
                else if (condition[end] == ']')
                    level--;
            }
            return conditions;
        }

        static readonly char[] contraintSeparators = { '[', ']' };

        public static bool CheckConstraints(IBasicLogger log, ConfigNode node, string constraints)
        {
            constraints = constraints.RemoveWS();

            if (constraints.Length == 0)
                return true;

            List<string> constraintList = SplitConstraints(constraints);

            if (constraintList.Count == 1)
            {
                constraints = constraintList[0];

                string remainingConstraints = "";
                if (constraints.Contains(":HAS[", out int hasStart))
                {
                    hasStart += 5;
                    remainingConstraints = constraints.Substring(hasStart, constraintList[0].LastIndexOf(']') - hasStart);
                    constraints = constraints.Substring(0, hasStart - 5);
                }

                string[] splits = constraints.Split(contraintSeparators, 3);
                string type = splits[0].Substring(1);
                string name = splits.Length > 1 ? splits[1] : null;

                switch (constraints[0])
                {
                    case '@':
                    case '!':

                        // @MODULE[ModuleAlternator] or !MODULE[ModuleAlternator]
                        bool not = (constraints[0] == '!');

                        bool any = false;
                        int index = 0;
                        ConfigNode last = null;
                        while (true)
                        {
                            ConfigNode subNode = FindConfigNodeIn(node, type, name, index++);
                            if (subNode == last || subNode == null)
                                break;
                            any = any || CheckConstraints(log, subNode, remainingConstraints);
                            last = subNode;
                        }
                        if (last != null)
                        {
                            log.Trace("CheckConstraints: {0} {1}", constraints, (not ^ any));
                            return not ^ any;
                        }
                        log.Trace("CheckConstraints: {0} {1}", constraints, (not ^ false));
                        return not ^ false;

                    case '#':

                        // #module[Winglet]
                        if (node.HasValue(type) && WildcardMatchValues(node, type, name))
                        {
                            bool ret2 = CheckConstraints(log, node, remainingConstraints);
                            log.Trace("CheckConstraints: {0} {1}", constraints, ret2);
                            return ret2;
                        }
                        log.Trace("CheckConstraints: {0} false", constraints);
                        return false;

                    case '~':

                        // ~breakingForce[]  breakingForce is not present
                        // or: ~breakingForce[100]  will be true if it's present but not 100, too.
                        if (name == "" && node.HasValue(type))
                        {
                            log.Trace("CheckConstraints: {0} false", constraints);
                            return false;
                        }
                        if (name != "" && WildcardMatchValues(node, type, name))
                        {
                            log.Trace("CheckConstraints: {0} false", constraints);
                            return false;
                        }
                        bool ret = CheckConstraints(log, node, remainingConstraints);
                        log.Trace("CheckConstraints: {0} {1}", constraints, ret);
                        return ret;

                    default:
                        log.Trace("CheckConstraints: {0} false", constraints);
                        return false;
                }
            }

            bool ret3 = true;
            foreach (string constraint in constraintList)
            {
                ret3 = ret3 && CheckConstraints(log, node, constraint);
            }
            log.Trace("CheckConstraints: {0} {1}", constraints, ret3);
            return ret3;
        }

        public static bool WildcardMatchValues(ConfigNode node, string type, string value)
        {
            double val = 0;
            bool compare = value != null && value.Length > 1 && (value[0] == '<' || value[0] == '>');
            compare = compare && double.TryParse(value.Substring(1), NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out val);

            string[] values = node.GetValues(type);
            for (int i = 0; i < values.Length; i++)
            {
                if (!compare && ConfigNodeEditUtils.Instance.WildcardMatch(values[i], value))
                    return true;

                if (compare && double.TryParse(values[i], NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out double val2)
                    && ((value[0] == '<' && val2 < val) || (value[0] == '>' && val2 > val)))
                {
                    return true;
                }
            }
            return false;
        }

        #endregion Condition checking

        #region Config Node Utilities

        private static void InsertNode(ConfigNode newNode, ConfigNode subMod, int index)
        {
            string modName = subMod.name;

            ConfigNode[] oldValues = newNode.GetNodes(modName);
            if (index < oldValues.Length)
            {
                newNode.RemoveNodes(modName);
                int i = 0;
                for (; i < index; ++i)
                    newNode.AddNode(oldValues[i]);
                newNode.AddNode(subMod);
                for (; i < oldValues.Length; ++i)
                    newNode.AddNode(oldValues[i]);
            }
            else
                newNode.AddNode(subMod);
        }

        private static void InsertValue(ConfigNode newNode, int index, string name, string value)
        {
            string[] oldValues = newNode.GetValues(name);
            if (index < oldValues.Length)
            {
                newNode.RemoveValues(name);
                int i = 0;
                for (; i < index; ++i)
                    newNode.AddValueSafe(name, oldValues[i]);
                newNode.AddValueSafe(name, value);
                for (; i < oldValues.Length; ++i)
                    newNode.AddValueSafe(name, oldValues[i]);
                return;
            }
            newNode.AddValueSafe(name, value);
        }

        //FindConfigNodeIn finds and returns a ConfigNode in src of type nodeType.
        //If nodeName is not null, it will only find a node of type nodeType with the value name=nodeName.
        //If nodeTag is not null, it will only find a node of type nodeType with the value name=nodeName and tag=nodeTag.
        public static ConfigNode FindConfigNodeIn(
            ConfigNode src,
            string nodeType,
            string nodeName = null,
            int index = 0)
        {
            List<ConfigNode> nodes = new List<ConfigNode>();
            int c = src.nodes.Count;
            for(int i = 0; i < c; ++i)
            {
                if (ConfigNodeEditUtils.Instance.WildcardMatch(src.nodes[i].name, nodeType))
                    nodes.Add(src.nodes[i]);
            }
            int nodeCount = nodes.Count;
            if (nodeCount == 0)
                return null;
            if (nodeName == null)
            {
                if (index >= 0)
                    return nodes[Math.Min(index, nodeCount - 1)];
                return nodes[Math.Max(0, nodeCount + index)];
            }
            ConfigNode last = null;
            if (index >= 0)
            {
                for (int i = 0; i < nodeCount; ++i)
                {
                    if (nodes[i].HasValue("name") && ConfigNodeEditUtils.Instance.WildcardMatch(nodes[i].GetValue("name"), nodeName))
                    {
                        last = nodes[i];
                        if (--index < 0)
                            return last;
                    }
                }
                return last;
            }
            for (int i = nodeCount - 1; i >= 0; --i)
            {
                if (nodes[i].HasValue("name") && ConfigNodeEditUtils.Instance.WildcardMatch(nodes[i].GetValue("name"), nodeName))
                {
                    last = nodes[i];
                    if (++index >= 0)
                        return last;
                }
            }
            return last;
        }

        #endregion Config Node Utilities
    }
}
