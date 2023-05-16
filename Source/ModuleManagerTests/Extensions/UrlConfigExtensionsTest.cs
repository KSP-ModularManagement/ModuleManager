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
using TestUtils;
using ModuleManager.Extensions;

namespace ModuleManagerTests.Extensions
{
    public class UrlConfigExtensionsTest
    {
        [Fact]
        public void TestSafeUrl()
        {
            ConfigNode node = new TestConfigNode("SOME_NODE")
            {
                { "name", "this shouldn't show up" },
            };
            UrlDir.UrlConfig url = UrlBuilder.CreateConfig("abc/def", node);
            Assert.Equal("abc/def/SOME_NODE", url.SafeUrl());
        }

        [Fact]
        public void TestSafeUrl__Null()
        {
            UrlDir.UrlConfig url = null;
            Assert.Equal("<null>", url.SafeUrl());
        }

        [Fact]
        public void TestSafeUrl__NullParent()
        {
            UrlDir.UrlConfig url = new UrlDir.UrlConfig(null, new ConfigNode("SOME_NODE"));
            Assert.Equal("SOME_NODE", url.SafeUrl());
        }

        [Fact]
        public void TestSafeUrl__NullParent__NullName()
        {
            ConfigNode node = new ConfigNode
            {
                name = null
            };
            UrlDir.UrlConfig url = new UrlDir.UrlConfig(null, node);
            Assert.Equal("<null>", url.SafeUrl());
        }

        [Fact]
        public void TestSafeUrl__NullParent__BlankName()
        {
            UrlDir.UrlConfig url = new UrlDir.UrlConfig(null, new ConfigNode(" "));
            Assert.Equal("<blank>", url.SafeUrl());
        }

        [Fact]
        public void TestSafeUrl__NullName()
        {
            ConfigNode node = new TestConfigNode()
            {
                { "name", "this shouldn't show up" },
            };
            node.name = null;
            UrlDir.UrlConfig url = UrlBuilder.CreateConfig("abc/def", node);
            Assert.Equal("abc/def/<null>", url.SafeUrl());
        }

        [Fact]
        public void TestSafeUrl__BlankName()
        {
            ConfigNode node = new TestConfigNode(" ")
            {
                { "name", "this shouldn't show up" },
            };
            UrlDir.UrlConfig url = UrlBuilder.CreateConfig("abc/def", node);
            Assert.Equal("abc/def/<blank>", url.SafeUrl());
        }

        [Fact]
        public void TestPrettyPrint()
        {
            ConfigNode node = new TestConfigNode("SOME_NODE")
            {
                { "abc", "def" },
                { "ghi", "jkl" },
                new TestConfigNode("INNER_NODE_1")
                {
                    { "mno", "pqr" },
                },
            };

            UrlDir.UrlConfig url = UrlBuilder.CreateConfig("abc/def.cfg", node);

            string expected = @"
abc/def/SOME_NODE
  SOME_NODE
  {
    abc = def
    ghi = jkl
    INNER_NODE_1
    {
      mno = pqr
    }
  }
".TrimStart().Replace("\r", null);
            
            Assert.Equal(expected, url.PrettyPrint());
        }

        [Fact]
        public void TestPrettyPrint__NullName()
        {
            ConfigNode node = new TestConfigNode()
            {
                { "abc", "def" },
                { "ghi", "jkl" },
                new TestConfigNode("INNER_NODE_1")
                {
                    { "mno", "pqr" },
                },
            };

            node.name = null;

            UrlDir.UrlConfig url = UrlBuilder.CreateConfig("abc/def.cfg", node);

            string expected = @"
abc/def/<null>
  <null>
  {
    abc = def
    ghi = jkl
    INNER_NODE_1
    {
      mno = pqr
    }
  }
".TrimStart().Replace("\r", null);

            Assert.Equal(expected, url.PrettyPrint());
        }

        [Fact]
        public void TestPrettyPrint__BlankName()
        {
            ConfigNode node = new TestConfigNode(" ")
            {
                { "abc", "def" },
                { "ghi", "jkl" },
                new TestConfigNode("INNER_NODE_1")
                {
                    { "mno", "pqr" },
                },
            };

            UrlDir.UrlConfig url = UrlBuilder.CreateConfig("abc/def.cfg", node);

            string expected = @"
abc/def/<blank>
   
  {
    abc = def
    ghi = jkl
    INNER_NODE_1
    {
      mno = pqr
    }
  }
".TrimStart().Replace("\r", null);

            Assert.Equal(expected, url.PrettyPrint());
        }

        [Fact]
        public void TestPrettyPrint__NullNameValue()
        {
            ConfigNode node = new TestConfigNode("SOME_NODE")
            {
                { "name", "temp" },
                { "abc", "def" },
                { "ghi", "jkl" },
                new TestConfigNode("INNER_NODE_1")
                {
                    { "mno", "pqr" },
                },
            };

            node.values[0].value = null;

            UrlDir.UrlConfig url = UrlBuilder.CreateConfig("abc/def.cfg", node);

            string expected = @"
abc/def/SOME_NODE
  SOME_NODE
  {
    name = <null>
    abc = def
    ghi = jkl
    INNER_NODE_1
    {
      mno = pqr
    }
  }
".TrimStart().Replace("\r", null);

            Assert.Equal(expected, url.PrettyPrint());
        }

        [Fact]
        public void TestPrettyPrint__NullNode()
        {
            UrlDir.UrlConfig url = UrlBuilder.CreateConfig("abc/def.cfg", new ConfigNode("SOME_NODE"));
            url.config = null;

            string expected = @"
abc/def/SOME_NODE
  <null node>
".TrimStart().Replace("\r", null);

            Assert.Equal(expected, url.PrettyPrint());
        }

        [Fact]
        public void TestPrettyPrint__Null()
        {
            UrlDir.UrlConfig url = null;
            Assert.Equal("<null UrlConfig>", url.PrettyPrint());
        }
    }
}
