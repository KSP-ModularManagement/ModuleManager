/*
	This file is part of Module Manager /L
		© 2018-2024 LisiasT
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
using System.Collections;

namespace TestUtils
{
    public class TestConfigNode : ConfigNode, IEnumerable
    {
        public TestConfigNode() : base() { }
        public TestConfigNode(string name) : base(name) { }

        public void Add(string name, string value) => Add(new Value(name, value));
        public void Add(Value value) => values.Add(value);
        public void Add(string name, ConfigNode node) => AddNode(name, node);
        public void Add(ConfigNode node) => AddNode(node);

        public IEnumerator GetEnumerator() => throw new NotImplementedException();
    }
}
