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
using System;
using UnityEngine;

using K = KSPe.Util.Log;

namespace ModuleManager.Logging
{
    public class ModLogger : IBasicLogger
    {
        internal static readonly K.Logger LOG = K.Logger.CreateThreadUnsafeForType<ModuleManager>(); // No need to use thread safe logging. Yet.
        internal static readonly ModLogger Instance = new ModLogger(); // For legacy code
        internal static bool DebugMode => KSPe.Globals<ModuleManager>.DebugMode;

        private delegate void LogMethod(string message, params object[] @params);
        private readonly LogMethod[] methods;
        private ModLogger()
        {
            this.methods = new LogMethod[6];
            int i = 0;
            this.methods[i++] = new LogMethod(LOG.error);
            this.methods[i++] = new LogMethod(LOG.error);
            this.methods[i++] = new LogMethod(LOG.warn);
            this.methods[i++] = new LogMethod(LOG.info);
            this.methods[i++] = new LogMethod(LOG.detail);
            this.methods[i++] = new LogMethod(LOG.error);
        }

        public void Log(K.Level logType, string message, params object[] @params) => this.methods[(int)logType](message, @params);

        public void Exception(Exception exception, string message, params object[] @params)
        {
            LOG.error(exception, message, @params);
        }

        public void Finish() { }
    }
}
