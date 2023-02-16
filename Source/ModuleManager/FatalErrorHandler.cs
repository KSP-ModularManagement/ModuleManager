/*
	This file is part of Module Manager /L
		© 2018-2023 LisiasT
		© 2013-2018 Sarbian using System; Blowfish
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

namespace ModuleManager
{
    public static class FatalErrorHandler
    {
        public static void HandleFatalError(string message)
        {
            Logging.ModLogger.LOG.error(message);
            try
            {
                PopupDialog.SpawnPopupDialog(new Vector2(0.5f, 0.5f),
                    new Vector2(0.5f, 0.5f),
                    new MultiOptionDialog(
#if !KSP12
                        "ModuleManagerFatalError",
#endif
                        $"ModuleManager has encountered a fatal error and KSP needs to close.\n\n{message}\n\nPlease see KSP's log for addtional details",
                        "ModuleManager - Fatal Error",
                        HighLogic.UISkin,
                        new Rect(0.5f, 0.5f, 500f, 60f),
                        new DialogGUIFlexibleSpace(),
                        new DialogGUIHorizontalLayout(
                            new DialogGUIFlexibleSpace(),
                            new DialogGUIButton("Quit", Application.Quit, 140.0f, 30.0f, true),
                            new DialogGUIFlexibleSpace()
                        )
                    ),
                    true,
                    HighLogic.UISkin);
            }
            catch(Exception ex)
            {
                Logging.ModLogger.LOG.error(ex, "Exception while trying to create the fatal exception dialog");
                Application.Quit();
            }
        }
    }
}
