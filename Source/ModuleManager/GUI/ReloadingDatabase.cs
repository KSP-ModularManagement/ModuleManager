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
#if !KSP12
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
			bool startedReload = false;
			this.parent = parent;

			UISkinDef skinDef = HighLogic.UISkin;
			UIStyle centeredTextStyle = new UIStyle() // FIXME: There must be a smarter way to do that on Unity5, right?
			{
				name = skinDef.label.name,
				normal = skinDef.label.normal,
				highlight = skinDef.label.highlight,
				disabled = skinDef.label.disabled,
				font = skinDef.label.font,
				fontSize = skinDef.label.fontSize,
				fontStyle = skinDef.label.fontStyle,
				wordWrap = skinDef.label.wordWrap,
				richText = skinDef.label.richText,
				alignment = TextAnchor.UpperCenter,
				clipping = skinDef.label.clipping,
				lineHeight = skinDef.label.lineHeight,
				stretchHeight = skinDef.label.stretchHeight,
				stretchWidth = skinDef.label.stretchWidth,
				fixedHeight = skinDef.label.fixedHeight,
				fixedWidth = skinDef.label.fixedWidth
			};

			float totalLoadWeight = GameDatabase.Instance.LoadWeight() + PartLoader.Instance.LoadWeight();

			PopupDialog reloadingDialog = PopupDialog.SpawnPopupDialog(
				new Vector2(0.5f, 0.5f),
				new Vector2(0.5f, 0.5f),
				new MultiOptionDialog(
					"ModuleManagerReloading",
					"",
					"ModuleManager - Reloading Database",
					skinDef,
					new Rect(0.5f, 0.5f, 600f, 60f),
					new DialogGUIFlexibleSpace(),
					new DialogGUIVerticalLayout(
						new DialogGUIFlexibleSpace(),
						new DialogGUILabel(delegate ()
						{
							float progressFraction;
							if (!startedReload)
							{
								progressFraction = 0f;
								startedReload = true;
							}
							else if (!GameDatabase.Instance.IsReady() || !PostPatchLoader.Instance.IsReady())
							{
								progressFraction = GameDatabase.Instance.ProgressFraction() * GameDatabase.Instance.LoadWeight();
								progressFraction /= totalLoadWeight;
							}
							else if (!PartLoader.Instance.IsReady())
							{
								progressFraction = GameDatabase.Instance.LoadWeight() + (PartLoader.Instance.ProgressFraction() * GameDatabase.Instance.LoadWeight());
								progressFraction /= totalLoadWeight;
							}
							else
							{
								progressFraction = 1f;
							}

							return $"Overall progress: {progressFraction:P0}";
						}, centeredTextStyle, expandW: true),
						new DialogGUILabel(delegate ()
						{
							if (!startedReload)
								return "Starting";
							else if (!GameDatabase.Instance.IsReady())
								return GameDatabase.Instance.ProgressTitle();
							else if (!PostPatchLoader.Instance.IsReady())
								return PostPatchLoader.Instance.ProgressTitle();
							else if (!PartLoader.Instance.IsReady())
								return PartLoader.Instance.ProgressTitle();
							else
								return "";
						}),
						new DialogGUISpace(5f),
						new DialogGUILabel(() => this.parent.patchRunner.Status)
					)
				),
				false,
				skinDef);
		}
	}
#endif
}