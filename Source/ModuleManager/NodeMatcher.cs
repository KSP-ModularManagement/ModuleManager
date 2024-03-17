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
using ModuleManager.Extensions;
using ModuleManager.Logging;
using ModuleManager.Utils;

namespace ModuleManager
{
    public interface INodeMatcher
    {
        bool IsMatch(ConfigNode node);
    }

    public class NodeMatcher : INodeMatcher
    {
        private readonly IBasicLogger log;
        private readonly string type;
        private readonly string[] namePatterns = null;
        private readonly string constraints = "";

        public NodeMatcher(IBasicLogger log, string type, string name, string constraints)
        {
            this.log = log;

            if (type == string.Empty) throw new ArgumentException("can't be empty", nameof(type));
            this.type = type ?? throw new ArgumentNullException(nameof(type));

            if (name == string.Empty) throw new ArgumentException("can't be empty (null allowed)", nameof(name));
            if (constraints == string.Empty) throw new ArgumentException("can't be empty (null allowed)", nameof(constraints));

            if (name != null) namePatterns = name.Split(',', '|');
            if (constraints != null)
            {
                if (!constraints.IsBracketBalanced()) throw new ArgumentException("is not bracket balanced: " + constraints, nameof(constraints));
                this.constraints = constraints;
            }
        }

        public bool IsMatch(ConfigNode node)
        {
            if (node.name != type) return false;

            if (namePatterns != null)
            {
                string name = node.GetValue("name");
                if (name == null) return false;

                bool match = false;
                foreach (string pattern in namePatterns)
                {
                    if (ConfigNodeEditUtils.Instance.WildcardMatch(name, pattern))
                    {
                        match = true;
                        break;
                    }
                }

                if (!match) return false;
            }

            return MMPatchLoader.CheckConstraints(this.log, node, constraints);
        }
    }
}
