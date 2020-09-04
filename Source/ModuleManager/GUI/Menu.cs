/*
	This file is part of Module Manager /L
	(C) 2020 Lisias T : http://lisias.net <support@lisias.net>

	Module Manager /L is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
*/
using UnityEngine;

namespace ModuleManager.GUI
{
#if !KSP12
	internal class Menu
	{
		internal static Menu Show(ModuleManager parent)
		{
			return new Menu(parent);
		}

		internal Menu Dismiss()
		{
			this.instance.Dismiss();
			this.instance = null;
			return null;
		}

		private ModuleManager parent;
		private PopupDialog instance;
		private Menu(ModuleManager parent)
		{
			this.parent = parent;
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
								MMPatchLoader.keepPartDB = false;
								this.parent.StartCoroutine(this.parent.DataBaseReloadWithMM());
							}, 140.0f, 30.0f, true),
						new DialogGUIButton("Quick Reload Database",
							delegate
							{
								MMPatchLoader.keepPartDB = true;
								this.parent.StartCoroutine(this.parent.DataBaseReloadWithMM());
							}, 140.0f, 30.0f, true),
						new DialogGUIButton("Dump Database to Files",
							delegate
							{
								this.parent.StartCoroutine(this.parent.DataBaseReloadWithMM(true));
							}, 140.0f, 30.0f, true),
						new DialogGUIButton("Close", () => { }, 140.0f, 30.0f, true)
						)),
				false,
				HighLogic.UISkin);
			}
	}
#endif
}
