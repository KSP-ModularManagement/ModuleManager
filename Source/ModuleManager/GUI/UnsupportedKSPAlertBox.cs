/*
	This file is part of Module Manager /L
		© 2018-2023 LisiasT

	Module Manager /L is licensed as follows:
		* GPL 3.0 : https://www.gnu.org/licenses/gpl-3.0.txt

	Module Manager /L is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.

	You should have received a copy of the GNU General Public License 3.0
	along with Module Manager /L. If not, see <https://www.gnu.org/licenses/>.
*/
using UnityEngine;

namespace ModuleManager.GUI
{
	internal static class UnsupportedKSPAlertBox
	{
		private static readonly string MSG = @"*THIS IS NOT* the Forum's Module Manager from Sarbian & Blowfish - don't bother them about this.

This release of Module Manager runs only on KSP {0}, but you are using {1}!";

		private static readonly string AMSG = @"download and install the appropriated version (KSP will close).";

		internal static void Show(string intendedKSP, string currentKSP)
		{
			KSPe.Common.Dialogs.ShowStopperAlertBox.Show(
				string.Format(MSG, intendedKSP, currentKSP),
				AMSG,
				() => { Application.OpenURL("https://github.com/net-lisias-ksp/ModuleManager/releases"); Application.Quit(); }
			);
			Logging.ModLogger.LOG.info("\"Houston, we have a Problem!\" about running MM on unsuppoted KSP was displayed");
		}
	}
}
