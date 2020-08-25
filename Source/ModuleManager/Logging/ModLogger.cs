using System;
using UnityEngine;

using K = KSPe.Util.Log;

namespace ModuleManager.Logging
{
    public class ModLogger : IBasicLogger
    {
        internal static readonly K.Logger LOG = K.Logger.CreateThreadUnsafeForType<ModuleManager>(); // No need to use thread safe logging. Yet.
        internal static readonly ModLogger Instance = new ModLogger(); // For legacy code

        private delegate void LogMethod(string message, params object[] @parms);
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
            LOG.level = K.Level.TRACE;
        }

        // Gambiarra porque eu não previ essa possibilidade no KSPe.Util.Log!
        private static readonly object[] NONE = new object[0];
        public void Log(LogType logType, string message)
        {
            this.methods[(int)logType](message, NONE);
        }

        public void Exception(string message, Exception exception)
        {
            LOG.error(exception, message);
        }
    }
}
