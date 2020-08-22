using System;
using System.IO;
using UnityEngine;

using static ModuleManager.FilePathRepository;

namespace ModuleManager
{
    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    public class CustomConfigsManager : MonoBehaviour
    {
		internal static bool start_techtree_loaded = false;
		internal static bool start_physics_loaded = false;
        internal void Start()
        {
#if false
			Log("Blah");
            Log(HighLogic.CurrentGame.Parameters.Career.TechTreeUrl);
			Log(TECHTREE_CONFIG.Path);
			Log(TECHTREE_CONFIG.IsLoadable.ToString());
#endif            
			if (start_techtree_loaded)
            {
                if (HighLogic.CurrentGame.Parameters.Career.TechTreeUrl != TECHTREE_CONFIG.KspPath)
                    Log(string.Format("Tech tree was changed by third party to [{0}].", HighLogic.CurrentGame.Parameters.Career.TechTreeUrl));
			}
            else if (TECHTREE_CONFIG.IsLoadable)
            {
                Log("Setting modded tech tree as the active one");
                HighLogic.CurrentGame.Parameters.Career.TechTreeUrl = TECHTREE_CONFIG.KspPath;
				start_techtree_loaded = true;
            }

			if (start_physics_loaded)
            {
                if (PhysicsGlobals.PhysicsDatabaseFilename != PHYSICS_CONFIG.Path)
                    Log(string.Format("Physics changed by third party to [{0}].", PhysicsGlobals.PhysicsDatabaseFilename));
			}
            else if (PHYSICS_CONFIG.IsLoadable)
            {
                Log("Setting modded physics as the active one");
                PhysicsGlobals.PhysicsDatabaseFilename = PHYSICS_CONFIG.Path;
                if (!PhysicsGlobals.Instance.LoadDatabase())
                    Log("Something went wrong while setting the active physics config.");
                start_physics_loaded = true;
            }
        }

		private static readonly KSPe.Util.Log.Logger log = KSPe.Util.Log.Logger.CreateForType<CustomConfigsManager>(false, false);
        private static void Log(String s)
        {
            log.info(s);
        }

    }
}
