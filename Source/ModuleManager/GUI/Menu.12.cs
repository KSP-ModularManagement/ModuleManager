/*
	This file is part of Module Manager /L
	(C) 2020 Lisias T : http://lisias.net <support@lisias.net>

	Module Manager /L is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
*/
using UnityEngine;
using UGUI = UnityEngine.GUI;

namespace ModuleManager.GUI
{
#if KSP12
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
			// TODO Make this work on KSP 1.2!
		}
	}
#endif
}
