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

using static ModuleManager.FilePathRepository;

namespace ModuleManager
{
	[KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
	public class CustomConfigsManager : MonoBehaviour
	{
		internal static bool start_techtree_loaded = false;
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
		}

		private static readonly KSPe.Util.Log.Logger log = KSPe.Util.Log.Logger.CreateThreadUnsafeForType<CustomConfigsManager>(); // No need to use thread safe logging. Yet.
		private static void Log(String s)
		{
			log.info(s);
		}

	}
}
