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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using ModuleManager.Extensions;

namespace ModuleManagerTests.Extensions
{
    public class StringExtensionsTest
    {
        [Fact]
        public void TestIsBracketBalanced()
        {
            Assert.True("abc[def[ghi[jkl]mno[pqr]]stu]vwx".IsBracketBalanced());
        }

        [Fact]
        public void TestIsBracketBalanced__NoBrackets()
        {
            Assert.True("she sells seashells by the seashore".IsBracketBalanced());
        }

        [Fact]
        public void TestIsBracketBalanced__Unbalanced()
        {
            Assert.False("abc[def[ghi[jkl]mno[pqr]]stuvwx".IsBracketBalanced());
            Assert.False("abcdef[ghi[jkl]mno[pqr]]stu]vwx".IsBracketBalanced());
        }

        [Fact]
        public void TestIsBracketBalanced__BalancedButNegative()
        {
            Assert.False("abc]def[ghi".IsBracketBalanced());
        }

        [Fact]
        public void TestRemoveWS()
        {
            Assert.Equal("abcdef", " abc \tdef\r\n\t ".RemoveWS());
        }


        [InlineData("abc", "b", true, 1)]
        [InlineData("abc", "x", false, -1)]
        [Theory]
        public void TestContains(string str, string test, bool expectedResult, int expectedIndex)
        {
            bool result = str.Contains(test, out int index);
            Assert.Equal(expectedResult, result);
            Assert.Equal(expectedIndex, index);
        }

        [Fact]
        public void TestContains__NullStr()
        {
            string s = null;
            Assert.Throws<ArgumentNullException>(delegate
            {
                s.Contains("x", out int _x);
            });
        }

        [Fact]
        public void TestContains__NullValue()
        {
            Assert.Throws<ArgumentNullException>(delegate
            {
                "abc".Contains(null, out int _x);
            });
        }
    }
}
