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
using ModuleManager;
using ModuleManager.Patches.PassSpecifiers;
using ModuleManager.Progress;

namespace ModuleManagerTests.Patches
{
    public class FirstPassSpecifierTest
    {
        public readonly INeedsChecker needsChecker = Substitute.For<INeedsChecker>();
        public readonly IPatchProgress progress = Substitute.For<IPatchProgress>();
        private readonly FirstPassSpecifier passSpecifier = new FirstPassSpecifier();

        [Fact]
        public void TestCheckNeeds()
        {
            Assert.True(passSpecifier.CheckNeeds(needsChecker, progress));
        }

        [Fact]
        public void TestCheckNeeds__NeedsCheckerNull()
        {
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(delegate
            {
                passSpecifier.CheckNeeds(null, progress);
            });

            Assert.Equal("needsChecker", ex.ParamName);

            progress.DidNotReceiveWithAnyArgs().NeedsUnsatisfiedAfter(null);
        }

        [Fact]
        public void TestCheckNeeds__ProgressNull()
        {
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(delegate
            {
                passSpecifier.CheckNeeds(needsChecker, null);
            });

            Assert.Equal("progress", ex.ParamName);
        }

        [Fact]
        public void TestDescriptor()
        {
            Assert.Equal(":FIRST", passSpecifier.Descriptor);
        }
    }
}
