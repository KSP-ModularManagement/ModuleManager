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
using TestUtils;
using ModuleManager;
using ModuleManager.Patches.PassSpecifiers;
using ModuleManager.Progress;

namespace ModuleManagerTests.Patches
{
    public class AfterPassSpecifierTest
    {
        public readonly UrlDir.UrlConfig urlConfig = UrlBuilder.CreateConfig("abc/def", new ConfigNode("NODE"));
        public readonly INeedsChecker needsChecker = Substitute.For<INeedsChecker>();
        public readonly IPatchProgress progress = Substitute.For<IPatchProgress>();
        public readonly AfterPassSpecifier passSpecifier;

        public AfterPassSpecifierTest()
        {
            passSpecifier = new AfterPassSpecifier("mod1", urlConfig);
        }

        [Fact]
        public void TestConstructor__ModNull()
        {
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(delegate
            {
                new AfterPassSpecifier(null, urlConfig);
            });

            Assert.Equal("mod", ex.ParamName);
        }

        [Fact]
        public void TestConstructor__ModEmpty()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(delegate
            {
                new AfterPassSpecifier("", urlConfig);
            });

            Assert.Equal("mod", ex.ParamName);
            Assert.Contains("can't be empty", ex.Message);
        }

        [Fact]
        public void TestConstructor__UrlConfigNull()
        {
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(delegate
            {
                new AfterPassSpecifier("mod1", null);
            });

            Assert.Equal("urlConfig", ex.ParamName);
        }

        [Fact]
        public void TestCheckNeeds__False()
        {
            needsChecker.CheckNeeds("mod1").Returns(false);
            Assert.False(passSpecifier.CheckNeeds(needsChecker, progress));

            progress.Received().NeedsUnsatisfiedAfter(urlConfig);
        }

        [Fact]
        public void TestCheckNeeds__True()
        {
            needsChecker.CheckNeeds("mod1").Returns(true);
            Assert.True(passSpecifier.CheckNeeds(needsChecker, progress));

            progress.DidNotReceiveWithAnyArgs().NeedsUnsatisfiedAfter(null);
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
            Assert.Equal(":AFTER[MOD1]", passSpecifier.Descriptor);
        }
    }
}
