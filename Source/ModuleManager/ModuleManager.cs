/*
	This file is part of Module Manager /L
		© 2018-2024 LisiasT
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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
#if CATS
using ModuleManager.Cats;
#endif
using ModuleManager.Extensions;
using ModuleManager.Logging;
using ModuleManager.UnityLogHandle;
using ModuleManager.Utils;

namespace ModuleManager
{
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    public class ModuleManager : MonoBehaviour
    {
        #region state

        private bool inRnDCenter;

        public bool showUI = false;
        private float textPos = 0;

        //private Texture2D tex;
        //private Texture2D tex2;

        private bool nyan = false;
        private bool nCats = false;
        public static bool dumpPostPatch = false;
        public static bool DontCopyLogs { get; private set; } = false;

        [Obsolete("This attribute is not Standard MM. Do not use it on things to be published on Forum")]
        public static bool IsExperimentalActive { get; private set; } = false;

        [Obsolete("This attribute is not Standard MM. Do not use it on things to be published on Forum")]
        public static bool IsLoadedFromCache { get; internal set; }

        internal static bool IgnoreCache { get; private set; }
        #if DEBUG
            = true;
        #else
            = false;
        #endif

        private GUI.Menu menu;

        internal MMPatchRunner patchRunner;
        private readonly Progress.ProgressCounter counter;
        private readonly Progress.Timings timings;

        private InterceptLogHandler interceptLogHandler;

        #endregion state

        private static bool loadedInScene;

        private ModuleManager()
        {
            this.counter = new Progress.ProgressCounter();
            this.timings = new Progress.Timings();
        }

        internal void OnRnDCenterSpawn()
        {
            inRnDCenter = true;
        }

        internal void OnRnDCenterDeSpawn()
        {
            inRnDCenter = false;
        }

        internal void Awake()
        {
            Detail("Awake");
            if (LoadingScreen.Instance == null)
            {
                Destroy(gameObject);
                return;
            }

            // Ensure that only one copy of the service is run per scene change.
            if (loadedInScene || !ElectionAndCheck())
            {
                Assembly currentAssembly = Assembly.GetExecutingAssembly();
                Log("Multiple copies of current version. Using the first copy. Version: {0}", currentAssembly.GetName().Version);
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
            this.menu = new GUI.Menu(this);
            PerformanceMetrics.Instance.Start();

            interceptLogHandler = new InterceptLogHandler();

            // Allow loading the background in the loading screen
            Application.runInBackground = true;

            // More cool loading screen. Less 4 stoke logo.
            for (int i = 0; i < LoadingScreen.Instance.Screens.Count; i++)
            {
				LoadingScreen.LoadingScreenState state = LoadingScreen.Instance.Screens[i];
                state.fadeInTime = i < 3 ? 0.1f : 1;
                state.displayTime = i < 3 ? 1 : 3;
                state.fadeOutTime = i < 3 ? 0.1f : 1;
            }

            TextMeshProUGUI[] texts = LoadingScreen.Instance.gameObject.GetComponentsInChildren<TextMeshProUGUI>();
            foreach (TextMeshProUGUI text in texts)
            {
                textPos = Mathf.Min(textPos, text.rectTransform.localPosition.y);
            }

            // Subscribe to the RnD center spawn/deSpawn events
            GameEvents.onGUIRnDComplexSpawn.Add(OnRnDCenterSpawn);
            GameEvents.onGUIRnDComplexDespawn.Add(OnRnDCenterDeSpawn);


            LoadingScreen screen = FindObjectOfType<LoadingScreen>();
            if (screen == null)
            {
                Log("Can't find LoadingScreen type. Aborting ModuleManager execution");
                return;
            }
            List<LoadingSystem> list = LoadingScreen.Instance.loaders;

            if (list != null)
            {
                // So you can insert a LoadingSystem object in this list at any point.
                // GameDatabase is first in the list, and PartLoader is second
                // We could insert ModuleManager after GameDatabase to get it to run there
                // and SaveGameFixer after PartLoader.

                int gameDatabaseIndex = list.FindIndex(s => s is GameDatabase);

                GameObject aGameObject = new GameObject("ModuleManager");
                DontDestroyOnLoad(aGameObject);

                Log("Adding post patch to the loading screen {0}", list.Count);
                list.Insert(gameDatabaseIndex + 1, aGameObject.AddComponent<PostPatchLoader>());
                PostPatchLoader.Instance.Set(this.timings);

                this.patchRunner = new MMPatchRunner(ModLogger.Instance, this.counter, this.timings);
                StartCoroutine(patchRunner.Run());

                // Workaround for 1.6.0 Editor bug after a PartDatabase rebuild.
                if (Versioning.version_major == 1 && Versioning.version_minor == 6 && Versioning.Revision == 0)
                {
                    Fix16 fix16 = aGameObject.AddComponent<Fix16>();
                    list.Add(fix16);
                }
            }

            {
                bool foolsDay = (DateTime.Now.Month == 4 && DateTime.Now.Day == 1);
                bool catDay = (DateTime.Now.Month == 2 && DateTime.Now.Day == 22);
                nyan = foolsDay || Environment.GetCommandLineArgs().Contains("-nyan-nyan");
                nCats = catDay || Environment.GetCommandLineArgs().Contains("-ncats");
            }
            dumpPostPatch = Environment.GetCommandLineArgs().Contains("-mm-dump");
            DontCopyLogs = Environment.GetCommandLineArgs().Contains("-mm-dont-copy-logs");
            IgnoreCache = Environment.GetCommandLineArgs().Contains("-ignore-cache");

#pragma warning disable CS0618 // Type or member is obsolete
            // To supress features non Stock.
            IsExperimentalActive = Environment.GetCommandLineArgs().Contains("-mm-experimental");
#pragma warning restore CS0618 // Type or member is obsolete

            loadedInScene = true;
        }

        private TextMeshProUGUI status;
        private TextMeshProUGUI errors;
        private TextMeshProUGUI warning;

        [SuppressMessage("Code Quality", "IDE0051", Justification = "Called by Unity")]
        private void Start()
        {
            Detail("Start");
#if CATS
            if (nCats)
                CatManager.LaunchCats();
            else if (nyan)
                CatManager.LaunchCat();
#endif
            Canvas canvas = LoadingScreen.Instance.GetComponentInChildren<Canvas>();

            status = CreateTextObject(canvas, "MMStatus");
            errors = CreateTextObject(canvas, "MMErrors");
            warning = CreateTextObject(canvas, "MMWarning");
            warning.text = "";

            //if (Versioning.version_major == 1 && Versioning.version_minor == 0 && Versioning.Revision == 5 && Versioning.BuildID == 1024)
            //{
            //    warning.text = "Your KSP 1.0.5 is running on build 1024. You should upgrade to build 1028 to avoid problems with addons.";
            //    //if (GUI.Button(new Rect(Screen.width / 2f - 100, offsetY, 200, 20), "Click to open the Forum thread"))
            //    //    Application.OpenURL("http://forum.kerbalspaceprogram.com/index.php?/topic/124998-silent-patch-for-ksp-105-published/");
            //}
        }

        private TextMeshProUGUI CreateTextObject(Canvas canvas, string name)
        {
            GameObject statusGameObject = new GameObject(name);
            TextMeshProUGUI text = statusGameObject.AddComponent<TextMeshProUGUI>();
            text.text = name;
            text.fontSize = 18;
            text.autoSizeTextContainer = true;
            text.font = Resources.Load("Fonts/Calibri SDF", typeof(TMP_FontAsset)) as TMP_FontAsset;
            text.alignment = TextAlignmentOptions.Center;
            text.enableWordWrapping = false;
            text.isOverlay = true;
            text.rectTransform.anchorMin = new Vector2(0.5f, 0);
            text.rectTransform.anchorMax = new Vector2(0.5f, 0);
            text.rectTransform.anchoredPosition = Vector2.zero;
            statusGameObject.transform.SetParent(canvas.transform);

            return text;
        }

        // Unsubscribe from events when the behavior dies
        internal void OnDestroy()
        {
            Detail("OnDestroy");
            GameEvents.onGUIRnDComplexSpawn.Remove(OnRnDCenterSpawn);
            GameEvents.onGUIRnDComplexDespawn.Remove(OnRnDCenterDeSpawn);
        }

        internal void Update()
        {
            this.menu.OnUpdate(this.inRnDCenter);

            if (PerformanceMetrics.Instance.IsRunning && HighLogic.LoadedScene == GameScenes.MAINMENU)
            {
                PerformanceMetrics.Instance.Stop();
                if (0 == this.counter.totalPatches.Value)
                {
                    Log("Loaded {0} patch(es) from cache."
                            , this.counter.patchedNodes.Value
                        );
                    Log("Total loading Time = {0:0.000}s."
                            , PerformanceMetrics.Instance.ElapsedTimeInSecs
                        );
                }
                else
                {
                    Log("Found {0} and applied {1} patch(es), with {2} warning(s) and {3} error(s). {4} node(s) patched."
                            , this.counter.totalPatches.Value
                            , this.counter.appliedPatches.Value
                            , this.counter.warnings.Value
                            , this.counter.errors.Value
                            , this.counter.patchedNodes.Value
                        );
                    Log("Total loading Time = {0:0.000}s, with {1} on patching and {2} on post patching (both parallel to loading)."
                            , PerformanceMetrics.Instance.ElapsedTimeInSecs
                            , (string)this.timings.Patching
                            , (string)this.timings.PostPatching
                        );
                }
                PerformanceMetrics.Instance.Destroy();
            }

            float offsetY = textPos;
            float h;

            if (patchRunner != null)
            {
                if (warning)
                {
                    warning.text = InterceptLogHandler.Warnings;
                    h = warning.text.Length > 0 ? warning.textBounds.size.y : 0;
                    offsetY += h;
                    warning.rectTransform.localPosition = new Vector3(0, offsetY);
                }

                if (status)
                {
                    status.text = patchRunner.Status;
                    h = status.text.Length > 0 ? status.textBounds.size.y : 0;
                    offsetY += h;
                    status.transform.localPosition = new Vector3(0, offsetY);
                }

                if (errors)
                {
                    errors.text = patchRunner.Errors;
                    h = errors.text.Length > 0 ? errors.textBounds.size.y : 0;
                    offsetY += h;
                    errors.transform.localPosition = new Vector3(0, offsetY);
                }
            }
        }

        #region GUI stuff.

        internal static IntPtr intPtr = new IntPtr(long.MaxValue);
        /* Not required anymore. At least
        public static bool IsABadIdea()
        {
            return (intPtr.ToInt64() == long.MaxValue) && (Environment.OSVersion.Platform == PlatformID.Win32NT);
        }
        */

		internal IEnumerator DataBaseReloadWithMM(bool keepPartDB)
		{
			MMPatchLoader.keepPartDB = keepPartDB;

			GUI.ReloadingDatabaseDialog reloadingDialog = GUI.ReloadingDatabaseDialog.Show(this);
			try
			{
				patchRunner = new MMPatchRunner(ModLogger.Instance, this.counter, this.timings);

				yield return null;

				GameDatabase.Instance.Recompile = true;
				GameDatabase.Instance.StartLoad();

				yield return null;
				StartCoroutine(patchRunner.Run());

				// wait for it to finish
				while (!GameDatabase.Instance.IsReady())
					yield return null;

				PostPatchLoader.Instance.Set(this.timings);

				while (!PostPatchLoader.Instance.IsReady())
					yield return null;

				PartLoader.Instance.StartLoad();

				while (!PartLoader.Instance.IsReady())
					yield return null;

				// Needs more work.
				//ConfigNode game = HighLogic.CurrentGame.config.GetNode("GAME");

				//if (game != null && ResearchAndDevelopment.Instance != null)
				//{
				//	ScreenMessages.PostScreenMessage("GAME found");
				//	ConfigNode scenario = game.GetNodes("SCENARIO").FirstOrDefault((ConfigNode n) => n.name == "ResearchAndDevelopment");
				//	if (scenario != null)
				//	{
				//		ScreenMessages.PostScreenMessage("SCENARIO found");
				//		ResearchAndDevelopment.Instance.OnLoad(scenario);
				//	}
				//}
			}
			finally
			{
				reloadingDialog = reloadingDialog.Dismiss();
			}
		}

		internal IEnumerator DumpDataBaseToFiles()
		{
			ModLogger.LOG.info("Dumping DataBase to files.");
			OutputAllConfigs();
			ModLogger.LOG.info("DataBase dump finished.");
			yield return null;
		}

        public static void OutputAllConfigs()
        {
            try
            {
                Directory.CreateDirectory(FilePathRepository.MMCfgOutputPath);
                foreach (string file in Directory.GetFiles(FilePathRepository.MMCfgOutputPath))
                {
                    File.Delete(file);
                }
                foreach (string dir in Directory.GetDirectories(FilePathRepository.MMCfgOutputPath))
                {
                    Directory.Delete(dir, true);
                }
            }
            catch (Exception e) when (e is IOException || e is UnauthorizedAccessException)
            {
                Log("Exception {0}, while cleaning the export dir!", e);
            }

            void WriteDirectoryRecursive(UrlDir currentDir, string dirPath)
            {
                if (currentDir.files.Count > 0) Directory.CreateDirectory(dirPath);

                foreach (UrlDir.UrlFile urlFile in currentDir.files)
                {
                    if (urlFile.fileType != UrlDir.FileType.Config) continue;

                    Log("Exporting " + urlFile.GetUrlWithExtension());
                    string filePath = Path.Combine(dirPath, urlFile.GetNameWithExtension());

                    bool first = true;

                    using (FileStream stream = new FileStream(filePath, FileMode.Create))
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        foreach (UrlDir.UrlConfig urlConfig in urlFile.configs)
                        {
                            try
                            {
                                if (first) first = false;
                                else writer.Write("\n");

                                ConfigNode copy = urlConfig.config.DeepCopy();
                                copy.EscapeValuesRecursive();
                                writer.Write(copy.ToString());
                            }
                            catch (Exception e)
                            {
                                Log("Exception while trying to write the file " + filePath + "\n" + e);
                            }
                        }
                    }
                }

                foreach (UrlDir urlDir in currentDir.children)
                {
                    WriteDirectoryRecursive(urlDir, Path.Combine(dirPath, urlDir.name));
                }
            }

            try
            {
                WriteDirectoryRecursive(GameDatabase.Instance.root, FilePathRepository.MMCfgOutputPath);
            }
            catch (DirectoryNotFoundException directoryNotFoundException)
            {
                Log("Exception while exporting the cfg\n" + directoryNotFoundException);
            }
            catch (IOException ioException)
            {
                Log("Exception while exporting the cfg\n" + ioException);
            }
            catch (UnauthorizedAccessException unauthorizedAccessException)
            {
                Log("Exception while exporting the cfg\n" + unauthorizedAccessException);
            }
        }

        #endregion GUI stuff.

        public bool ElectionAndCheck()
        {
            #region Type election

            // TODO : Move the old version check in a process that call Update.

            // Check for old version and MMSarbianExt
            IEnumerable<AssemblyLoader.LoadedAssembly> oldMM =
                AssemblyLoader.loadedAssemblies.Where(
                    a => a.assembly.GetName().Name == Assembly.GetExecutingAssembly().GetName().Name)
                    .Where(a => a.assembly.GetName().Version.CompareTo(new System.Version(1, 5, 0)) == -1);
            IEnumerable<AssemblyLoader.LoadedAssembly> oldAssemblies =
                oldMM.Concat(AssemblyLoader.loadedAssemblies.Where(a => a.assembly.GetName().Name == "MMSarbianExt"));
            if (oldAssemblies.Any())
            {
                IEnumerable<string> badPaths =
                    oldAssemblies.Select(a => a.path)
                        .Select(
                            p =>
                                Uri.UnescapeDataString(
                                    new Uri(Path.GetFullPath(KSPUtil.ApplicationRootPath)).MakeRelativeUri(new Uri(p))
                                        .ToString()
                                        .Replace('/', Path.DirectorySeparatorChar)));
                string status =
                    "You have old versions of Module Manager (older than 1.5) or MMSarbianExt.\nYou will need to remove them for Module Manager and the mods using it to work\nExit KSP and delete those files :\n" +
                    String.Join("\n", badPaths.ToArray());
                GUI.ShowStopperAlertBox.Show(status);
                return false;
            }


            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            IEnumerable<AssemblyLoader.LoadedAssembly> eligible = from a in AssemblyLoader.loadedAssemblies
                                                                  let ass = a.assembly
                                                                  where ass.GetName().Name == currentAssembly.GetName().Name
                                                                  orderby ass.GetName().Version descending, a.path ascending
                                                                  select a;

            // Elect the newest loaded version of MM to process all patch files.
            // If there is a newer version loaded then don't do anything
            // If there is a same version but earlier in the list, don't do anything either.
            if (eligible.First().assembly != currentAssembly)
            {
                //loaded = true;
                Log("version {0} at {1} lost the election", currentAssembly.GetName().Version, currentAssembly.Location);
                Destroy(gameObject);
                return false;
            }
            string candidates = "";
            foreach (AssemblyLoader.LoadedAssembly a in eligible)
            {
                if (currentAssembly.Location != a.path)
                    candidates += string.Format("Version {0} {1} \n", a.assembly.GetName().Version, a.path);
            }
            if (candidates.Length > 0)
            {
                Log("version {0} at {1} won the election against\n{2}", currentAssembly.GetName().Version, currentAssembly.Location, candidates);
            }

            #endregion Type election

            return true;
        }

        private static void Log(String s)
        {
            ModLogger.LOG.info(s);
        }

        private static void Log(String format, params object[] p)
        {
            ModLogger.LOG.info(format, p);
        }

        private static void Detail(string s)
        {
            ModLogger.LOG.detail(s);
        }
    }
}
