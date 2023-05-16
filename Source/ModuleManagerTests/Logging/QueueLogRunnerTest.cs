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

using ModuleManager.Collections;
using ModuleManager.Logging;

namespace ModuleManagerTests.Logging
{
    public class QueueLogRunnerTest
    {
        [Fact]
        public void TestConstructor__LogQueueNull()
        {
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(delegate
            {
                new QueueLogRunner(null);
            });

            Assert.Equal("logQueue", ex.ParamName);
        }

        [Fact]
        public void TestConstructor__TimeToWaitForLogsMsNegative()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(delegate
            {
                new QueueLogRunner(Substitute.For<IMessageQueue<ILogMessage>>(), -1);
            });

            Assert.Contains("must be non-negative", ex.Message);
            Assert.Equal("timeToWaitForLogsMs", ex.ParamName);
        }

        [Fact]
        public void TestRun()
        {
            ILogMessage message1 = Substitute.For<ILogMessage>();
            ILogMessage message2 = Substitute.For<ILogMessage>();
            ILogMessage message3 = Substitute.For<ILogMessage>();
            ILogMessage message4 = Substitute.For<ILogMessage>();
            ILogMessage message5 = Substitute.For<ILogMessage>();
            ILogMessage message6 = Substitute.For<ILogMessage>();
            IMessageQueue<ILogMessage> messageQueue = Substitute.For<IMessageQueue<ILogMessage>>();
            QueueLogRunner logRunner = new QueueLogRunner(messageQueue, 0);
            int counter = 0;
            messageQueue.TakeAll().Returns(delegate
            {
                IMessageQueue<ILogMessage> messageQueue2 = Substitute.For<IMessageQueue<ILogMessage>>();
                if (counter == 0)
                {
                    messageQueue2.GetEnumerator().Returns(new ArrayEnumerator<ILogMessage>(message1, message2));
                }
                else if (counter == 1)
                {
                    logRunner.RequestStop(); // Called from Running state
                    messageQueue2.GetEnumerator().Returns(new ArrayEnumerator<ILogMessage>(message3, message4));
                }
                else
                {
                    logRunner.RequestStop(); // Called from StopRequested state
                    messageQueue2.GetEnumerator().Returns(new ArrayEnumerator<ILogMessage>(message5, message6));
                }
                counter++;
                return messageQueue2;
            });

            IBasicLogger logger = Substitute.For<IBasicLogger>();

            logRunner.Run(logger);

            logRunner.RequestStop(); // Called from Stopped state

            Received.InOrder(delegate
            {
                logger.Log(message1);
                logger.Log(message2);
                logger.Log(message3);
                logger.Log(message4);
                logger.Log(message5);
                logger.Log(message6);
            });
        }

        [Fact]
        public void TestRun__AlreadyStarted()
        {
            IMessageQueue<ILogMessage> messageQueue = Substitute.For<IMessageQueue<ILogMessage>>();
            QueueLogRunner logRunner = new QueueLogRunner(messageQueue, 0);
            int counter = 0;
            messageQueue.TakeAll().Returns(delegate
            {
                IMessageQueue<ILogMessage> messageQueue2 = Substitute.For<IMessageQueue<ILogMessage>>();
                if (counter == 0)
                {
                    InvalidOperationException ex = Assert.Throws<InvalidOperationException>(delegate
                    {
                        logRunner.Run(Substitute.For<IBasicLogger>());
                    });
                    Assert.Equal("Cannot run from Running state", ex.Message);
                    logRunner.RequestStop();
                    messageQueue2.GetEnumerator().Returns(new ArrayEnumerator<ILogMessage>());
                }
                else
                {
                    InvalidOperationException ex = Assert.Throws<InvalidOperationException>(delegate
                    {
                        logRunner.Run(Substitute.For<IBasicLogger>());
                    });
                    Assert.Equal("Cannot run from StopRequested state", ex.Message);
                    logRunner.RequestStop();
                    messageQueue2.GetEnumerator().Returns(new ArrayEnumerator<ILogMessage>());
                }
                counter++;
                return messageQueue2;
            });

            IBasicLogger logger = Substitute.For<IBasicLogger>();
            logRunner.Run(logger);

            InvalidOperationException ex2 = Assert.Throws<InvalidOperationException>(delegate
            {
                logRunner.Run(Substitute.For<IBasicLogger>());
            });
            Assert.Equal("Cannot run from Stopped state", ex2.Message);
        }

        [Fact]
        public void TestRun__LoggerNull()
        {
            QueueLogRunner logRunner = new QueueLogRunner(Substitute.For<IMessageQueue<ILogMessage>>());

            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(delegate
            {
                logRunner.Run(null);
            });
            Assert.Equal("logger", ex.ParamName);
        }

        [Fact]
        public void TestRequestStop__NotStarted()
        {
            QueueLogRunner logRunner = new QueueLogRunner(Substitute.For<IMessageQueue<ILogMessage>>());

            InvalidOperationException ex = Assert.Throws<InvalidOperationException>(delegate
            {
                logRunner.RequestStop();
            });
            Assert.Equal("Cannot request stop from Initialized state", ex.Message);
        }
    }
}
