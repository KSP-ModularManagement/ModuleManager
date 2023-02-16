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
using ModuleManager.Extensions;

namespace ModuleManagerTests.Extensions
{
    public class ByteArrayExtensionsTest
    {
        [Fact]
        public void TestToHex()
        {
            byte[] data = { 0x00, 0xff, 0x01, 0xfe, 0x02, 0xfd, 0x9a };

            Assert.Equal("00ff01fe02fd9a", data.ToHex());
        }

        [Fact]
        public void TestToHex__NullData()
        {
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(delegate
            {
                ByteArrayExtensions.ToHex(null);
            });

            Assert.Equal("data", ex.ParamName);
        }
    }
}
