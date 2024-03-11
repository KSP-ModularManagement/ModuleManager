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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using ModuleManager.Extensions;
using ModuleManager.Logging;

using static ModuleManager.FilePathRepository;
using System.Diagnostics.CodeAnalysis;

namespace ModuleManager
{
    public delegate void ModuleManagerPostPatchCallback();

    public class PostPatchLoader : LoadingSystem
    {
        public static PostPatchLoader Instance { get; private set; }

        public IEnumerable<IProtoUrlConfig> databaseConfigs = null;

        private static readonly List<ModuleManagerPostPatchCallback> postPatchCallbacks = new List<ModuleManagerPostPatchCallback>();

        private readonly IBasicLogger logger = ModLogger.Instance;
        private Progress.Timings timings;

        private bool ready = false;

        private string progressTitle = "ModuleManager: Post patch";

        public static void AddPostPatchCallback(ModuleManagerPostPatchCallback callback)
        {
            if (!postPatchCallbacks.Contains(callback))
                postPatchCallbacks.Add(callback);
        }

        [SuppressMessage("CodeQuality", "IDE0051", Justification = "Called by Unity")]
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
                return;
            }
            Instance = this;
        }

        public override bool IsReady() => ready;

    #if !KSP12
        public override float LoadWeight() => 0;
    #endif

        public override float ProgressFraction() => 1;

        public override string ProgressTitle() => progressTitle;

        public override void StartLoad()
        {
            ready = false;
            StartCoroutine(Run());
        }

        internal void Set(Progress.Timings timings)
        {
            this.timings = timings;
        }

        private IEnumerator Run()
        {
            while (null == this.timings) yield return null;

            this.timings.Wait.Start();

            progressTitle = "ModuleManager: Waiting for patching to finish";

            while (databaseConfigs == null) yield return null;

            this.timings.Wait.Stop();
            logger.Info("Waited " + this.timings.Wait + " for patching to finish");

            this.timings.PostPatching.Start();

            progressTitle = "ModuleManager: Applying patched game database";
            logger.Info("Applying patched game database");

            foreach (UrlDir.UrlFile file in GameDatabase.Instance.root.AllConfigFiles)
            {
                file.configs.Clear();
            }

            foreach (IProtoUrlConfig protoConfig in databaseConfigs)
            {
                protoConfig.UrlFile.AddConfig(protoConfig.Node);
            }

            databaseConfigs = null;

            yield return null;

#if false
            // Using an dedicated external log is nice. Dumping it into KSP.log breaking the known formats is not.
            if (File.Exists(logPath))
            {
                progressTitle = "ModuleManager: Dumping log to KSP log";
                logger.Info("Dumping ModuleManager log to main log");
                logger.Info("\n#### BEGIN MODULEMANAGER LOG ####\n\n\n" + File.ReadAllText(logPath) + "\n\n\n#### END MODULEMANAGER LOG ####");
            }
            else
            {
                logger.Error("ModuleManager log does not exist: " + logPath);
            }
#endif
            yield return null;

#if DEBUG
            InGameTestRunner testRunner = new InGameTestRunner(logger);
            testRunner.RunTestCases(GameDatabase.Instance.root);
#endif

            yield return null;

            progressTitle = "ModuleManager: Reloading things";

            logger.Info("Reloading resources definitions");
            PartResourceLibrary.Instance.LoadDefinitions();

            logger.Info("Reloading Trait configs");
            GameDatabase.Instance.ExperienceConfigs.LoadTraitConfigs();

            logger.Info("Reloading Part Upgrades");
            PartUpgradeManager.Handler.FillUpgrades();

            LoadModdedPhysics();

            yield return null;

            progressTitle = "ModuleManager: Running post patch callbacks";
            logger.Info("Running post patch callbacks");

            foreach (ModuleManagerPostPatchCallback callback in postPatchCallbacks)
            {
                try
                {
                    callback();
                }
                catch (Exception e)
                {
                    logger.Exception(e, "Exception while running a post patch callback");
                }
                yield return null;
            }
            yield return null;

            // Call all "public static void ModuleManagerPostLoad()" on all class
            foreach (Assembly ass in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    foreach (Type type in ass.GetTypes())
                    {
                        MethodInfo method = type.GetMethod("ModuleManagerPostLoad", BindingFlags.Public | BindingFlags.Static);

                        if (method != null && method.GetParameters().Length == 0)
                        {
                            try
                            {
                                logger.Info("Calling " + ass.GetName().Name + "." + type.Name + "." + method.Name + "()");
                                method.Invoke(null, null);
                            }
                            catch (Exception e)
                            {
                                logger.Exception(e, "Exception while calling " + ass.GetName().Name + "." + type.Name + "." + method.Name + "()");
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    logger.Exception(e, "Post run call threw an exception in loading " + ass.FullName);
                }
            }

            yield return null;

            // Call "public void ModuleManagerPostLoad()" on all active MonoBehaviour instance
            foreach (MonoBehaviour obj in FindObjectsOfType<MonoBehaviour>())
            {
                MethodInfo method = obj.GetType().GetMethod("ModuleManagerPostLoad", BindingFlags.Public | BindingFlags.Instance);

                if (method != null && method.GetParameters().Length == 0)
                {
                    try
                    {
                        logger.Info("Calling " + obj.GetType().Name + "." + method.Name + "()");
                        method.Invoke(obj, null);
                    }
                    catch (Exception e)
                    {
                        logger.Exception(e, "Exception while calling " + obj.GetType().Name + "." + method.Name + "() :");
                    }
                }
            }

            yield return null;

            if (ModuleManager.dumpPostPatch)
                ModuleManager.OutputAllConfigs();

            this.timings.PostPatching.Stop();
            logger.Info("Post patch ran in " + this.timings.PostPatching);

            ready = true;
        }

        private void LoadModdedPhysics()
        {
            if (PHYSICS_CONFIG.IsLoadable)
            {
                logger.Info("Setting modded physics as the active one");
                PhysicsGlobals.PhysicsDatabaseFilename = PHYSICS_CONFIG.Path;
                if (!PhysicsGlobals.Instance.LoadDatabase())
                    logger.Info("Something went wrong while setting the active physics config.");
            }
            else
                logger.Error("Physics file not found");
        }
    }
}
