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
using Xunit;
using NSubstitute;
using UnityEngine;
using ModuleManager.Logging;

namespace ModuleManagerTests.Logging
{
    public class PrefixLoggerTest
    {
        private readonly IBasicLogger innerLogger = Substitute.For<IBasicLogger>();
        private readonly PrefixLogger logger;

        public PrefixLoggerTest()
        {
            logger = new PrefixLogger("MyMod", innerLogger);
        }

        [Fact]
        public void TestConstructor__PrefixNull()
        {
            ArgumentNullException e = Assert.Throws<ArgumentNullException>(delegate
            {
                new PrefixLogger(null, innerLogger);
            });

            Assert.Equal("prefix", e.ParamName);
        }

        [Fact]
        public void TestConstructor__PrefixBlank()
        {
            ArgumentNullException e = Assert.Throws<ArgumentNullException>(delegate
            {
                new PrefixLogger("", innerLogger);
            });

            Assert.Equal("prefix", e.ParamName);
        }

        [Fact]
        public void TestConstructor__LoggerNull()
        {
            ArgumentNullException e = Assert.Throws<ArgumentNullException>(delegate
            {
                new PrefixLogger("blah", null);
            });

            Assert.Equal("logger", e.ParamName);
        }

        [Fact]
        public void TestLog()
        {
            ILogMessage logMessage = Substitute.For<ILogMessage>();
            logMessage.LogType.Returns(LogType.Log);
            logMessage.Message.Returns("well hi there");
            logMessage.Timestamp.Returns(new DateTime(2000, 1, 1, 12, 34, 45, 678));

            logger.Log(logMessage);

            innerLogger.Received().Log(Arg.Is<ILogMessage>(msg =>
                msg.LogType == LogType.Log &&
                msg.Timestamp == logMessage.Timestamp &&
                msg.Message == "[MyMod] well hi there"
            ));
        }

        [Fact]
        public void TestLog__Null()
        {
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(delegate
            {
                logger.Log(null);
            });

            Assert.Equal("message", ex.ParamName);
        }
    }
}
