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
using ModuleManager;

namespace ModuleManagerTests
{
    public class CommandParserTest
    {
        [Fact]
        public void TestParse__Insert()
        {
            Assert.Equal(Command.Insert, CommandParser.Parse("PART", out string newName));
            Assert.Equal("PART", newName);
        }

        [Fact]
        public void TestParse__Delete()
        {
            Assert.Equal(Command.Delete, CommandParser.Parse("!PART", out string newName1));
            Assert.Equal("PART", newName1);
            Assert.Equal(Command.Delete, CommandParser.Parse("-PART", out string newName2));
            Assert.Equal("PART", newName2);
        }

        [Fact]
        public void TestParse__Edit()
        {
            Assert.Equal(Command.Edit, CommandParser.Parse("@PART", out string newName));
            Assert.Equal("PART", newName);
        }

        [Fact]
        public void TestParse__Replace()
        {
            Assert.Equal(Command.Replace, CommandParser.Parse("%PART", out string newName));
            Assert.Equal("PART", newName);
        }

        [Fact]
        public void TestParse__Copy()
        {
            Assert.Equal(Command.Copy, CommandParser.Parse("+PART", out string newName1));
            Assert.Equal("PART", newName1);
            Assert.Equal(Command.Copy, CommandParser.Parse("$PART", out string newName2));
            Assert.Equal("PART", newName2);
        }

        [Fact]
        public void TestParse__Rename()
        {
            Assert.Equal(Command.Rename, CommandParser.Parse("|PART", out string newName));
            Assert.Equal("PART", newName); ;
        }

        [Fact]
        public void TestParse__Paste()
        {
            Assert.Equal(Command.Paste, CommandParser.Parse("#PART", out string newName));
            Assert.Equal("PART", newName);
        }

        [Fact]
        public void TestParse__Special()
        {
            Assert.Equal(Command.Special, CommandParser.Parse("*PART", out string newName));
            Assert.Equal("PART", newName);
        }

        [Fact]
        public void TestParse__Special__Chained()
        {
            Assert.Equal(Command.Special, CommandParser.Parse("*@PART", out string newName));
            Assert.Equal("@PART", newName);
        }

        [Fact]
        public void TestParse__Create()
        {
            Assert.Equal(Command.Create, CommandParser.Parse("&PART", out string newName));
            Assert.Equal("PART", newName);
        }
    }
}
