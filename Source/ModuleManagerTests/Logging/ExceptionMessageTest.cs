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
using ModuleManager.Logging;

namespace ModuleManagerTests.Logging
{
    public class ExceptionMessageTest
    {
        [Fact]
        public void TestLogTo()
        {
            IBasicLogger logger = Substitute.For<IBasicLogger>();

            Exception e = new Exception();
            ExceptionMessage message = new ExceptionMessage("An exception was thrown", e);
            message.LogTo(logger);

            logger.Received().Exception("An exception was thrown", e);
        }
    }
}
