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
using System.Linq;
using Xunit;
using NSubstitute;
using ModuleManager;
using ModuleManager.Patches;

namespace ModuleManagerTests
{
    public class PassTest
    {
        [Fact]
        public void TestConstructor__NameNull()
        {
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(delegate
            {
                new Pass(null);
            });

            Assert.Equal("name", ex.ParamName);
        }

        [Fact]
        public void TestConstructor__NameEmpty()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(delegate
            {
                new Pass("");
            });

            Assert.Contains("can't be empty", ex.Message);
            Assert.Equal("name", ex.ParamName);
        }

        [Fact]
        public void TestName()
        {
            Pass pass = new Pass(":NOTINAMILLIONYEARS");

            Assert.Equal(":NOTINAMILLIONYEARS", pass.Name);
        }

        [Fact]
        public void Test__Add__Enumerator()
        {
            IPatch[] patches =
            {
                Substitute.For<IPatch>(),
                Substitute.For<IPatch>(),
                Substitute.For<IPatch>(),
            };

            Pass pass = new Pass("blah")
            {
                patches[0],
                patches[1],
                patches[2],
            };

            IPatch[] passPatches = pass.ToArray();
            Assert.Equal(patches.Length, passPatches.Length);

            for (int i = 0; i < patches.Length; i++)
            {
                Assert.Same(patches[i], passPatches[i]);
            }
        }
    }
}
