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
