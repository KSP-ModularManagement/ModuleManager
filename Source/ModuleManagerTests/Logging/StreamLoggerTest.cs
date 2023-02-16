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
using System.IO;
using Xunit;
using NSubstitute;
using ModuleManager.Logging;

namespace ModuleManagerTests.Logging
{
    public class StreamLoggerTest
    {
        [Fact]
        public void TestConstructor__StreamNull()
        {
            Assert.Throws<ArgumentNullException>(delegate
            {
                new StreamLogger(null);
            });
        }

        [Fact]
        public void TestConstructor__CantWrite()
        {
            using (MemoryStream stream = new MemoryStream(new byte[0], false))
            {
                Assert.Throws<ArgumentException>(delegate
                {
                    new StreamLogger(stream);
                });
            }
        }

        [Fact]
        public void TestLog__AlreadyDisposed()
        {
            using (MemoryStream stream = new MemoryStream(new byte[0], true))
            {
                StreamLogger streamLogger = new StreamLogger(stream);
                streamLogger.Dispose();

                InvalidOperationException ex = Assert.Throws<InvalidOperationException>(delegate
                {
                    streamLogger.Log(Substitute.For<ILogMessage>());
                });

                Assert.Contains("Object has already been disposed", ex.Message);
            }
        }

        [Fact]
        public void TestLog()
        {
            ILogMessage message = Substitute.For<ILogMessage>();
            message.ToLogString().Returns("[OMG wtf] bbq");
            byte[] bytes = new byte[15];
            using (MemoryStream stream = new MemoryStream(bytes, true))
            {
                using (StreamLogger streamLogger = new StreamLogger(stream))
                {
                    streamLogger.Log(message);
                }
            }

            using (MemoryStream stream = new MemoryStream(bytes, false))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string result = reader.ReadToEnd().Trim('\r', '\n', '\0');
                    Assert.Equal("[OMG wtf] bbq", result);
                }
            }
        }

        [Fact]
        public void TestLog__MessageNull()
        {
            using (MemoryStream stream = new MemoryStream(new byte[0], true))
            {
                StreamLogger streamLogger = new StreamLogger(stream);

                ArgumentNullException ex = Assert.Throws<ArgumentNullException>(delegate
                {
                    streamLogger.Log(null);
                });

                Assert.Equal("message", ex.ParamName);
            }
        }
    }
}
