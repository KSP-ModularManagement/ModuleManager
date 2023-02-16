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
using ModuleManager.Collections;
using ModuleManager.Logging;

namespace ModuleManagerTests.Logging
{
    public class QueueLoggerTest
    {
        private readonly IMessageQueue<ILogMessage> queue = Substitute.For<IMessageQueue<ILogMessage>>();
        private readonly QueueLogger logger;

        public QueueLoggerTest()
        {
            logger = new QueueLogger(queue);
        }

        [Fact]
        public void TestConstructor__QueueNull()
        {
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(delegate
            {
                new QueueLogger(null);
            });

            Assert.Equal("queue", ex.ParamName);
        }

        [Fact]
        public void TestLog()
        {
            ILogMessage message = Substitute.For<ILogMessage>();
            logger.Log(message);
            queue.Received().Add(message);
        }

        [Fact]
        public void TestLog__MessageNull()
        {
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(delegate
            {
                logger.Log(null);
            });

            Assert.Equal("message", ex.ParamName);
        }
        
    }
}
