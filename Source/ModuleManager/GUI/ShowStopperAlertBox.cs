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
	internal static class ShowStopperAlertBox
	{
		private static readonly string MSG = @"*THIS IS NOT* the Forum's Module Manager from Sarbian & Blowfish - don't bother them about this.

{0}";

		private static readonly string AMSG = @"call for help on the Module Manager /L GitHub page (KSP will close). We will help you on diagnosing the Add'On that is troubling you. ";

		internal static void Show(string message)
		{
			KSPe.Common.Dialogs.ShowStopperAlertBox.Show(
				string.Format(MSG, message),
				AMSG,
				() => { Application.OpenURL("https://github.com/net-lisias-ksp/ModuleManager/issues/2"); Application.Quit(); }
			);
			Logging.ModLogger.LOG.info("\"Houston, we have a Problem!\" was displayed with message {0}", message);
		}
	}
}
