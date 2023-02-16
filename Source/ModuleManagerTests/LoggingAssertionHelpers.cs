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
using UnityEngine;
using NSubstitute;
using ModuleManager.Logging;

namespace ModuleManagerTests
{
    public static class LoggingAssertionHelpers
    {
        public static void AssertInfo(this IBasicLogger logger, string message)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            logger.Received().Log(Arg.Is<ILogMessage>(msg => msg.LogType == LogType.Log && msg.Message == message));
        }

        public static void AssertNoInfo(this IBasicLogger logger)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            logger.DidNotReceive().Log(Arg.Is<ILogMessage>(msg => msg.LogType == LogType.Log));
        }

        public static void AssertWarning(this IBasicLogger logger, string message)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            logger.Received().Log(Arg.Is<ILogMessage>(msg => msg.LogType == LogType.Warning && msg.Message == message));
        }

        public static void AssertNoWarning(this IBasicLogger logger)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            logger.DidNotReceive().Log(Arg.Is<ILogMessage>(msg => msg.LogType == LogType.Warning));
        }

        public static void AssertError(this IBasicLogger logger, string message)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            logger.Received().Log(Arg.Is<ILogMessage>(msg => msg.LogType == LogType.Error && msg.Message == message));
        }

        public static void AssertNoError(this IBasicLogger logger)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            logger.DidNotReceive().Log(Arg.Is<ILogMessage>(msg => msg.LogType == LogType.Error));
        }

        public static void AssertException(this IBasicLogger logger, string message, Exception exception)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            logger.Received().Log(Arg.Is<ILogMessage>(msg => msg.LogType == LogType.Exception && msg.Message == message + ": " + exception.ToString()));
        }

        public static void AssertException(this IBasicLogger logger, Exception exception)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            logger.Received().Log(Arg.Is<ILogMessage>(msg => msg.LogType == LogType.Exception && msg.Message == exception.ToString()));
        }

        public static void AssertNoException(this IBasicLogger logger)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            logger.DidNotReceive().Log(Arg.Is<ILogMessage>(msg => msg.LogType == LogType.Exception));
        }

        public static void AssertNoLog(this IBasicLogger logger)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            logger.DidNotReceiveWithAnyArgs().Log(null);
        }
    }
}
