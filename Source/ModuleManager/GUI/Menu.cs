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

		private void Show ()
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
								MMPatchLoader.keepPartDB = false;
								this.parent.StartCoroutine(this.parent.DataBaseReloadWithMM());
								this.Dismiss();
							}, 140.0f, 30.0f, false),
						new DialogGUIButton("Quick Reload Database",
							delegate
							{
								MMPatchLoader.keepPartDB = true;
								this.parent.StartCoroutine(this.parent.DataBaseReloadWithMM());
								this.Dismiss();
							}, 140.0f, 30.0f, false),
						new DialogGUIButton("Dump Database to Files",
							delegate
							{
								this.parent.StartCoroutine(this.parent.DataBaseReloadWithMM(true));
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
