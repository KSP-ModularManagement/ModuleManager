/*
	This file is part of Module Manager /L
		© 2018-2024 LisiasT

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
	internal static class ShowStopperAlertBox
	{
		private static readonly string MSG = @"*THIS IS NOT* the Forum's Module Manager from Sarbian & Blowfish - don't bother them about this.

{0}";

		private static readonly string AMSG = @"call for help on the Module Manager /L GitHub page (KSP will close). We will help you on diagnosing the Add'On that is troubling you. ";

		internal static void Show(string message)
		{
			KSPe.Common.Dialogs.ShowStopperErrorBox.Show(
				string.Format(MSG, message),
				AMSG,
				() => { Application.OpenURL("https://github.com/net-lisias-ksp/ModuleManager/issues/2"); Application.Quit(); }
			);
			Logging.ModLogger.LOG.info("\"Houston, we have a Problem!\" was displayed with message {0}", message);
		}
	}
}
