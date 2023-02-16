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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ModuleManager.UnityLogHandle
{
    class InterceptLogHandler : ILogHandler
    {
        private readonly ILogHandler baseLogHandler;
        private readonly List<Assembly> brokenAssemblies = new List<Assembly>();
        private readonly int gamePathLength;

        public static string Warnings { get; private set; } = "";

        public InterceptLogHandler()
        {
            baseLogHandler = Debug.logger.logHandler;
            Debug.logger.logHandler = this;
            gamePathLength = Path.GetFullPath(KSPUtil.ApplicationRootPath).Length;
        }

        public void LogFormat(LogType logType, Object context, string format, params object[] args)
        {
            baseLogHandler.LogFormat(logType, context, format, args);
        }

        public void LogException(Exception exception, Object context)
        {
            baseLogHandler.LogException(exception, context);

            if (exception is ReflectionTypeLoadException ex)
            {
                string message = "Intercepted a ReflectionTypeLoadException. List of broken DLLs:\n";
                try
                {
					IEnumerable<Assembly> assemblies = ex.Types.Where(x => x != null).Select(x => x.Assembly).Distinct();
                    foreach (Assembly assembly in assemblies)
                    {
                        if (string.IsNullOrEmpty(Warnings))
                        {
                            Warnings = "Add'On(s) DLL that have failed to be dynamically linked on loading\n";
                        }
                        string modInfo = assembly.GetName().Name + " " + assembly.GetName().Version + " " +
                                         assembly.Location.Remove(0, gamePathLength) + "\n";
                        if (!brokenAssemblies.Contains(assembly))
                        {
                            brokenAssemblies.Add(assembly);
                            Warnings += modInfo;
                        }
                        message += modInfo;
                    }
                }
                catch (Exception e)
                {
                    message += "Exception " + e.GetType().Name + " while handling the exception...";
                }
                Logging.ModLogger.LOG.error("**FATAL** {0}", message);
                GUI.ShowStopperAlertBox.Show(message);
            }
        }
    }
}
