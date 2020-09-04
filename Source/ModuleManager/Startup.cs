using System;
using UnityEngine;

using ModuleManager.Logging;

namespace ModuleManager
{
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    internal class Startup : MonoBehaviour
	{
        private void Start()
        {
            ModLogger.LOG.force("Version {0}", Version.Text);
        #if KSP12
            if (KSPe.Util.KSP.Version.Current != KSPe.Util.KSP.Version.FindByVersion(1,2,2))
                GUI.UnsupportedKSPAlertBox.Show("1.2.2", KSPe.Util.KSP.Version.Current.ToString());
        #else
            if (KSPe.Util.KSP.Version.Current < KSPe.Util.KSP.Version.FindByVersion(1,3,0))
                GUI.UnsupportedKSPAlertBox.Show(">= 1.3.0", KSPe.Util.KSP.Version.Current.ToString());
        #endif
        }
	}

}
