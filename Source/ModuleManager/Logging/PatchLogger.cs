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
using System;
using UnityEngine;

using K = KSPe.Util.Log;

namespace ModuleManager.Logging
{
    public class PatchLogger : IBasicLogger
    {
        internal readonly K.Logger log;

        private delegate void LogMethod(string message, params object[] @parms);
        private readonly LogMethod[] methods;
        internal PatchLogger(string filename)
        {
            this.log = new K.FileChainUnityLogger<Startup>(filename);

            this.methods = new LogMethod[6];
            int i = 0;
            this.methods[i++] = new LogMethod(this.log.error);
            this.methods[i++] = new LogMethod(this.log.error);
            this.methods[i++] = new LogMethod(this.log.warn);
            this.methods[i++] = new LogMethod(this.log.info);
            this.methods[i++] = new LogMethod(this.log.detail);
            this.methods[i++] = new LogMethod(this.log.error);
            this.log.level = K.Level.TRACE;
        }

        // Gambiarra porque eu não previ essa possibilidade no KSPe.Util.Log!
        private static readonly object[] NONE = new object[0];
        public void Log(LogType logType, string message)
        {
            this.methods[(int)logType](message, NONE);
        }

        public void Exception(string message, Exception exception)
        {
            this.log.error(exception, message);
        }

        public void Finish()
        {
            this.log.Close();
        }
    }
}
