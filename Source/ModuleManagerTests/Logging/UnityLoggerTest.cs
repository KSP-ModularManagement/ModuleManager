/*
	This file is part of Module Manager /L
		© 2018-2023 LisiasT
		© 2013-2018 Sarbian; Blowfish
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
using Xunit;
using NSubstitute;
using UnityEngine;
using ModuleManager.Extensions;
using ModuleManager.Logging;

namespace ModuleManagerTests.Logging
{
    public class UnityLoggerTest
    {
        private readonly ILogger innerLogger = Substitute.For<ILogger>();
        private readonly UnityLogger logger;

        public UnityLoggerTest()
        {
            logger = new UnityLogger(innerLogger);
        }

        [Fact]
        public void TestConstructor__LoggerNull()
        {
            ArgumentNullException e = Assert.Throws<ArgumentNullException>(delegate
            {
                new UnityLogger(null);
            });

            Assert.Equal("logger", e.ParamName);
        }

        [Fact]
        public void TestLog__Info()
        {
            logger.Info("well hi there");

            innerLogger.Received().Log(LogType.Log, "well hi there");
        }

        [Fact]
        public void TestLog__Warning()
        {
            logger.Warning("I'm warning you");

            innerLogger.Received().Log(LogType.Warning, "I'm warning you");
        }

        [Fact]
        public void TestLog__MessageNull()
        {
            ArgumentNullException e = Assert.Throws<ArgumentNullException>(delegate
            {
                logger.Log(null);
            });

            Assert.Equal("message", e.ParamName);
        }
    }
}
