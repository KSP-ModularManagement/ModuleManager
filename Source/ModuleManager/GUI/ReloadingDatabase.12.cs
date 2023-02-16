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
using UGUI = UnityEngine.GUI;

namespace ModuleManager.GUI
{
#if KSP12
	public class ReloadingDatabaseDialog
	{
		internal static ReloadingDatabaseDialog Show(ModuleManager parent)
		{
			return new ReloadingDatabaseDialog(parent);
		}

		internal ReloadingDatabaseDialog Dismiss()
		{
			this.instance.Dismiss();
			this.instance = null;
			return null;
		}

		private ModuleManager parent;
		private PopupDialog instance;
		private ReloadingDatabaseDialog(ModuleManager parent)
		{
			this.parent = parent;
		}
	}
#endif
}