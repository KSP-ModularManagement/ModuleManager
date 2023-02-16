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
using System.Linq;
using System.Text;
using NodeStack = ModuleManager.Collections.ImmutableStack<ConfigNode>;

namespace ModuleManager.Extensions
{
    public static class NodeStackExtensions
    {
        public static string GetPath(this NodeStack stack)
        {
            int length = stack.Sum(node => node.name.Length) + stack.Depth - 1;
            StringBuilder sb = new StringBuilder(length);

            foreach (ConfigNode node in stack)
            {
                string nodeName = node.name;
                sb.Insert(0, node.name);
                if (sb.Length < sb.Capacity) sb.Insert(0, '/');
            }

            return sb.ToString();
        }
    }
}
