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
