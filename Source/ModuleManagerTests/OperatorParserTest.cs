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
using ModuleManager;

namespace ModuleManagerTests
{
    public class OperatorParserTest
    {
        [Fact]
        public void TestParse__Null()
        {
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(delegate
            {
                OperatorParser.Parse(null, out string _);
            });

            Assert.Equal("name", ex.ParamName);
        }

        [Fact]
        public void TestParse__Empty()
        {
            Operator op = OperatorParser.Parse("", out string result);

            Assert.Equal(Operator.Assign, op);
            Assert.Equal("", result);
        }

        [Fact]
        public void TestParse__Assign()
        {
            Operator op = OperatorParser.Parse("some_stuff,1[2, ]", out string result);

            Assert.Equal(Operator.Assign, op);
            Assert.Equal("some_stuff,1[2, ]", result);
        }

        [Fact]
        public void TestParse__Add()
        {
            Operator op = OperatorParser.Parse("some_stuff,1[2, ]  +", out string result);

            Assert.Equal(Operator.Add, op);
            Assert.Equal("some_stuff,1[2, ]", result);
        }

        [Fact]
        public void TestParse__Subtract()
        {
            Operator op = OperatorParser.Parse("some_stuff,1[2, ]  -", out string result);

            Assert.Equal(Operator.Subtract, op);
            Assert.Equal("some_stuff,1[2, ]", result);
        }

        [Fact]
        public void TestParse__Multiply()
        {
            Operator op = OperatorParser.Parse("some_stuff,1[2, ]  *", out string result);

            Assert.Equal(Operator.Multiply, op);
            Assert.Equal("some_stuff,1[2, ]", result);
        }

        [Fact]
        public void TestParse__Divide()
        {
            Operator op = OperatorParser.Parse("some_stuff,1[2, ]  /", out string result);

            Assert.Equal(Operator.Divide, op);
            Assert.Equal("some_stuff,1[2, ]", result);
        }

        [Fact]
        public void TestParse__Exponentiate()
        {
            Operator op = OperatorParser.Parse("some_stuff,1[2, ]  !", out string result);

            Assert.Equal(Operator.Exponentiate, op);
            Assert.Equal("some_stuff,1[2, ]", result);
        }

        [Fact]
        public void TestParse__RegexReplace()
        {
            Operator op = OperatorParser.Parse("some_stuff,1[2, ]  ^", out string result);

            Assert.Equal(Operator.RegexReplace, op);
            Assert.Equal("some_stuff,1[2, ]", result);
        }

        [Fact]
        public void TestParse__NoSpaceMeansNoOp()
        {
            Operator op = OperatorParser.Parse("some_stuff*", out string result);

            Assert.Equal(Operator.Assign, op);
            Assert.Equal("some_stuff*", result);
        }

        [Fact]
        public void TestParse__SingleCharacterNotOp()
        {
            Operator op = OperatorParser.Parse("*", out string result);

            Assert.Equal(Operator.Assign, op);
            Assert.Equal("*", result);
        }
    }
}
