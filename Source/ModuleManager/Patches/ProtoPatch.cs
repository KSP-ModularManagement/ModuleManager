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
using ModuleManager.Patches.PassSpecifiers;

namespace ModuleManager.Patches
{
    public class ProtoPatch
    {
        public readonly UrlDir.UrlConfig urlConfig;
        public readonly Command command;
        public readonly string nodeType;
        public readonly string nodeName;
        public readonly string needs = null;
        public readonly string has = null;
        public readonly IPassSpecifier passSpecifier;

        public ProtoPatch(UrlDir.UrlConfig urlConfig, Command command, string nodeType, string nodeName, string needs, string has, IPassSpecifier passSpecifier)
        {
            this.urlConfig = urlConfig;
            this.command = command;
            this.nodeType = nodeType;
            this.nodeName = nodeName;
            this.needs = needs;
            this.has = has;
            this.passSpecifier = passSpecifier;
        }
    }
}
