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
using TestUtils;
using ModuleManager;

namespace ModuleManagerTests
{
    public class ProtoUrlConfigTest
    {
        [Fact]
        public void TestContructor__UrlFileNull()
        {
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(delegate
            {
                new ProtoUrlConfig(null, new ConfigNode());
            });

            Assert.Equal("urlFile", ex.ParamName);
        }

        [Fact]
        public void TestContructor__NodeNull()
        {
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(delegate
            {
                new ProtoUrlConfig(UrlBuilder.CreateFile("foo/bar"), null);
            });

            Assert.Equal("node", ex.ParamName);
        }

        [Fact]
        public void TestUrlFile()
        {
            UrlDir.UrlFile urlFile = UrlBuilder.CreateFile("abc/def.cfg");
            ProtoUrlConfig protoUrlConfig = new ProtoUrlConfig(urlFile, new ConfigNode());

            Assert.Same(urlFile, protoUrlConfig.UrlFile);
        }

        [Fact]
        public void TestNode()
        {
            ConfigNode node = new ConfigNode("NODE");
            ProtoUrlConfig protoUrlConfig = new ProtoUrlConfig(UrlBuilder.CreateFile("foo/bar"), node);

            Assert.Same(node, protoUrlConfig.Node);
        }

        [Fact]
        public void TestFileUrl()
        {
            ProtoUrlConfig protoUrlConfig = new ProtoUrlConfig(UrlBuilder.CreateFile("abc/def.cfg"), new ConfigNode());

            Assert.Equal("abc/def.cfg", protoUrlConfig.FileUrl);
        }

        [Fact]
        public void TestNodeType()
        {
            ProtoUrlConfig protoUrlConfig = new ProtoUrlConfig(UrlBuilder.CreateFile("abc/def"), new ConfigNode("SOME_NODE"));

            Assert.Equal("SOME_NODE", protoUrlConfig.NodeType);
        }

        [Fact]
        public void TestFullUrl()
        {
            ProtoUrlConfig protoUrlConfig = new ProtoUrlConfig(UrlBuilder.CreateFile("abc/def.cfg"), new ConfigNode("SOME_NODE"));

            Assert.Equal("abc/def.cfg/SOME_NODE", protoUrlConfig.FullUrl);
        }

        [Fact]
        public void TestFullUrl__NameValue()
        {
            ConfigNode node = new TestConfigNode("SOME_NODE")
            {
                { "name", "some_value" },
            };

            ProtoUrlConfig protoUrlConfig = new ProtoUrlConfig(UrlBuilder.CreateFile("abc/def.cfg"), node);

            Assert.Equal("abc/def.cfg/SOME_NODE[some_value]", protoUrlConfig.FullUrl);
        }
    }
}
