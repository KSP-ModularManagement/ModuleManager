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
using ModuleManager.Collections;
using ModuleManager.Extensions;

namespace ModuleManagerTests.Extensions
{
    public class NodeStackExtensionsTest
    {
        [Fact]
        public void TestGetPath()
        {
            ConfigNode node1 = new ConfigNode("NODE1");
            ConfigNode node2 = new ConfigNode("NODE2");
            ConfigNode node3 = new ConfigNode("NODE3");

            ImmutableStack<ConfigNode> stack = new ImmutableStack<ConfigNode>(node1).Push(node2).Push(node3);

            Assert.Equal("NODE1/NODE2/NODE3", stack.GetPath());
        }
    }
}
