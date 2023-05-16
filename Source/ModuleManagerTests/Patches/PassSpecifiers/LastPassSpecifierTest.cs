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
using ModuleManager;
using ModuleManager.Patches.PassSpecifiers;
using ModuleManager.Progress;

namespace ModuleManagerTests.Patches
{
    public class LastPassSpecifierTest
    {
        public readonly INeedsChecker needsChecker = Substitute.For<INeedsChecker>();
        public readonly IPatchProgress progress = Substitute.For<IPatchProgress>();
        public readonly LastPassSpecifier passSpecifier;

        public LastPassSpecifierTest()
        {
            passSpecifier = new LastPassSpecifier("mod1");
        }

        [Fact]
        public void TestConstructor__ModNull()
        {
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(delegate
            {
                new LastPassSpecifier(null);
            });

            Assert.Equal("mod", ex.ParamName);
        }

        [Fact]
        public void TestConstructor__ModEmpty()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(delegate
            {
                new LastPassSpecifier("");
            });

            Assert.Equal("mod", ex.ParamName);
            Assert.Contains("can't be empty", ex.Message);
        }

        [Fact]
        public void TestCheckNeeds()
        {
            passSpecifier.CheckNeeds(needsChecker, progress);

            needsChecker.DidNotReceiveWithAnyArgs().CheckNeeds(null);
        }

        [Fact]
        public void TestDescriptor()
        {
            Assert.Equal(":LAST[MOD1]", passSpecifier.Descriptor);
        }
    }
}
