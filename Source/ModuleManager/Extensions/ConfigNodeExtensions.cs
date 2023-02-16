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
using System.Text;

namespace ModuleManager.Extensions
{
    public static class ConfigNodeExtensions
    {
        public static void ShallowCopyFrom(this ConfigNode toNode, ConfigNode fromeNode)
        {
            toNode.ClearData();
            foreach (ConfigNode.Value value in fromeNode.values)
                toNode.values.Add(value);
            foreach (ConfigNode node in fromeNode.nodes)
                toNode.nodes.Add(node);
        }

        // KSP implementation of ConfigNode.CreateCopy breaks with badly formed nodes (nodes with a blank name)
        public static ConfigNode DeepCopy(this ConfigNode from)
        {
            ConfigNode to = new ConfigNode(from.name);
            foreach (ConfigNode.Value value in from.values)
                to.AddValueSafe(value.name, value.value);
            foreach (ConfigNode node in from.nodes)
            {
                ConfigNode newNode = DeepCopy(node);
                to.nodes.Add(newNode);
            }
            return to;
        }

        public static void PrettyPrint(this ConfigNode node, ref StringBuilder sb, string indent)
        {
            if (sb == null) throw new ArgumentNullException(nameof(sb));
            if (indent == null) indent = string.Empty;
            if (node == null)
            {
                sb.Append(indent + "<null node>\n");
                return;
            }
            sb.AppendFormat("{0}{1}\n{2}{{\n", indent, node.name ?? "<null>", indent);
            string newindent = indent + "  ";
            if (node.values == null)
            {
                sb.AppendFormat("{0}<null value list>\n", newindent);
            }
            else
            {
                foreach (ConfigNode.Value value in node.values)
                {
                    if (value == null)
                        sb.AppendFormat("{0}<null value>\n", newindent);
                    else
                        sb.AppendFormat("{0}{1} = {2}\n", newindent, value.name ?? "<null>", value.value ?? "<null>");
                }
            }

            if (node.nodes == null)
            {
                sb.AppendFormat("{0}<null node list>\n", newindent);
            }
            else
            {
                foreach (ConfigNode subnode in node.nodes)
                {
                    subnode.PrettyPrint(ref sb, newindent);
                }
            }

            sb.AppendFormat("{0}}}\n", indent);
        }

        public static void AddValueSafe(this ConfigNode node, string name, string value)
        {
            node.values.Add(new ConfigNode.Value(name, value));
        }

        public static void EscapeValuesRecursive(this ConfigNode theNode)
        {
            foreach (ConfigNode subNode in theNode.nodes)
            {
                subNode.EscapeValuesRecursive();
            }

            foreach (ConfigNode.Value value in theNode.values)
            {
                value.value = value.value.Replace("\n", "\\n");
                value.value = value.value.Replace("\r", "\\r");
                value.value = value.value.Replace("\t", "\\t");
            }
        }

        public static void UnescapeValuesRecursive(this ConfigNode theNode)
        {
            foreach (ConfigNode subNode in theNode.nodes)
            {
                subNode.UnescapeValuesRecursive();
            }

            foreach (ConfigNode.Value value in theNode.values)
            {
                value.value = value.value.Replace("\\n", "\n");
                value.value = value.value.Replace("\\r", "\r");
                value.value = value.value.Replace("\\t", "\t");
            }
        }
    }
}
