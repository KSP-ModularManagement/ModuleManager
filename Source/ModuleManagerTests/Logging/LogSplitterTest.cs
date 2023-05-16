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
using ModuleManager.Logging;

namespace ModuleManagerTests.Logging
{
    public class LogSplitterTest
    {
        [Fact]
        public void TestConstructor__Logger1Null()
        {
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(delegate
            {
                new LogSplitter(null, Substitute.For<IBasicLogger>());
            });

            Assert.Equal("logger1", ex.ParamName);
        }

        [Fact]
        public void TestConstructor__Logger2Null()
        {
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(delegate
            {
                new LogSplitter(Substitute.For<IBasicLogger>(), null);
            });

            Assert.Equal("logger2", ex.ParamName);
        }

        [Fact]
        public void TestLog()
        {
            IBasicLogger logger1 = Substitute.For<IBasicLogger>();
            IBasicLogger logger2 = Substitute.For<IBasicLogger>();
            LogSplitter logSplitter = new LogSplitter(logger1, logger2);
            ILogMessage message = Substitute.For<ILogMessage>();
            logSplitter.Log(message);
            logger1.Received().Log(message);
            logger2.Received().Log(message);
        }

        [Fact]
        public void TestLog__MessageNull()
        {
            LogSplitter logSplitter = new LogSplitter(Substitute.For<IBasicLogger>(), Substitute.For<IBasicLogger>());
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(delegate
            {
                logSplitter.Log(null);
            });

            Assert.Equal("message", ex.ParamName);
        }
    }
}
