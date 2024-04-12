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
#if !KSP12
	internal class Menu
	{
		private readonly ModuleManager parent;
		private PopupDialog instance;

		internal Menu(ModuleManager parent)
		{
			this.parent = parent;
		}

		internal void Dismiss()
		{
			this.instance.Dismiss();
			this.instance = null;
		}

		internal void OnUpdate(bool inRnDCenter)
		{
			if (GameSettings.MODIFIER_KEY.GetKey() && Input.GetKeyDown(KeyCode.F11)
				&& (HighLogic.LoadedScene == GameScenes.SPACECENTER || HighLogic.LoadedScene == GameScenes.MAINMENU)
				&& !inRnDCenter)
			{
				if (null == this.instance)
					this.Show();
				else
					this.Dismiss();
			}
		}

		private void Show()
		{
			this.instance = PopupDialog.SpawnPopupDialog(
				new Vector2(0.5f, 0.5f),
				new Vector2(0.5f, 0.5f),
				new MultiOptionDialog(
					"ModuleManagerMenu",
					"",
					"ModuleManager",
					HighLogic.UISkin,
					new Rect(0.5f, 0.5f, 150f, 60f),
					new DialogGUIFlexibleSpace(),
					new DialogGUIVerticalLayout(
						new DialogGUIFlexibleSpace(),
						new DialogGUIButton("Reload Database",
							delegate
							{
								this.parent.StartCoroutine(this.parent.DataBaseReloadWithMM(false));
								this.Dismiss();
							}, 140.0f, 30.0f, false),
						new DialogGUIButton("Quick Reload Database",
							delegate
							{
								this.parent.StartCoroutine(this.parent.DataBaseReloadWithMM(true));
								this.Dismiss();
							}, 140.0f, 30.0f, false),
						new DialogGUIButton("Dump Database to Files",
							delegate
							{
								this.parent.StartCoroutine(this.parent.DumpDataBaseToFiles());
								this.Dismiss();
							}, 140.0f, 30.0f, false),
						new DialogGUIButton("Close", () => { this.Dismiss(); } , 140.0f, 30.0f, false)
						)),
				false,
				HighLogic.UISkin);
			}
	}
#endif
}
