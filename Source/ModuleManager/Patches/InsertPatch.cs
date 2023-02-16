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
using ModuleManager.Extensions;
using ModuleManager.Logging;
using ModuleManager.Patches.PassSpecifiers;
using ModuleManager.Progress;

namespace ModuleManager.Patches
{
    public class InsertPatch : IPatch
    {
        public UrlDir.UrlConfig UrlConfig { get; }
        public string NodeType { get; }
        public IPassSpecifier PassSpecifier { get; }
        public bool CountsAsPatch => false;

        public InsertPatch(UrlDir.UrlConfig urlConfig, string nodeType, IPassSpecifier passSpecifier)
        {
            UrlConfig = urlConfig ?? throw new ArgumentNullException(nameof(urlConfig));
            NodeType = nodeType ?? throw new ArgumentNullException(nameof(nodeType));
            PassSpecifier = passSpecifier ?? throw new ArgumentNullException(nameof(passSpecifier));
        }

        public void Apply(LinkedList<IProtoUrlConfig> configs, IPatchProgress progress, IBasicLogger logger)
        {
            if (configs == null) throw new ArgumentNullException(nameof(configs));
            if (progress == null) throw new ArgumentNullException(nameof(progress));
            if (logger == null) throw new ArgumentNullException(nameof(logger));

            ConfigNode node = UrlConfig.config.DeepCopy();
            node.name = NodeType;
            configs.AddLast(new ProtoUrlConfig(UrlConfig.parent, node));
        }
    }
}
