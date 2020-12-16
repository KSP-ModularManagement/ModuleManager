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
	public class ReloadingDatabaseDialog
	{
		internal static ReloadingDatabaseDialog Show(ModuleManager parent)
		{
			return new ReloadingDatabaseDialog(parent);
		}

		internal Menu Dismiss()
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