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

	You should have received a copy of the GNU General Public License 2.0
	along with TweakScale /L. If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using Xunit;
using NSubstitute;
using UnityEngine;
using ModuleManager.Logging;

namespace ModuleManagerTests.Logging
{
    public class NormalMessageTest
    {
        private IBasicLogger logger = Substitute.For<IBasicLogger>();

        [Fact]
        public void TestLogTo__Info()
        {
            NormalMessage message = new NormalMessage(LogType.Log, "everything is ok");
            message.LogTo(logger);
            logger.Received().Log(LogType.Log, "everything is ok");
        }

        [Fact]
        public void TestLogTo__Warning()
        {
            NormalMessage message = new NormalMessage(LogType.Warning, "I'm warning you");
            message.LogTo(logger);
            logger.Received().Log(LogType.Warning, "I'm warning you");
        }

        [Fact]
        public void TestLogTo__Error()
        {
            NormalMessage message = new NormalMessage(LogType.Error, "You went too far");
            message.LogTo(logger);
            logger.Received().Log(LogType.Error, "You went too far");
        }
    }
}
