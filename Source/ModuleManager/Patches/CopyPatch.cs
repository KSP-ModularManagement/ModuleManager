﻿/*
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
using System.Collections.Generic;
using NodeStack = ModuleManager.Collections.ImmutableStack<ConfigNode>;
using ModuleManager.Extensions;
using ModuleManager.Logging;
using ModuleManager.Patches.PassSpecifiers;
using ModuleManager.Progress;

namespace ModuleManager.Patches
{
    public class CopyPatch : IPatch
    {
        private readonly IBasicLogger log;
        public UrlDir.UrlConfig UrlConfig { get; }
        public INodeMatcher NodeMatcher { get; }
        public IPassSpecifier PassSpecifier { get; }
        public bool CountsAsPatch => true;

        public CopyPatch(IBasicLogger log, UrlDir.UrlConfig urlConfig, INodeMatcher nodeMatcher, IPassSpecifier passSpecifier)
        {
            this.log = log;
            UrlConfig = urlConfig ?? throw new ArgumentNullException(nameof(urlConfig));
            NodeMatcher = nodeMatcher ?? throw new ArgumentNullException(nameof(nodeMatcher));
            PassSpecifier = passSpecifier ?? throw new ArgumentNullException(nameof(passSpecifier));
        }

        public void Apply(LinkedList<IProtoUrlConfig> databaseConfigs, IPatchProgress progress, IBasicLogger logger)
        {
            if (databaseConfigs == null) throw new ArgumentNullException(nameof(databaseConfigs));
            if (progress == null) throw new ArgumentNullException(nameof(progress));
            if (logger == null) throw new ArgumentNullException(nameof(logger));

            PatchContext context = new PatchContext(UrlConfig, databaseConfigs, logger, progress);

            for (LinkedListNode<IProtoUrlConfig> listNode = databaseConfigs.First; listNode != null; listNode = listNode.Next)
            {
                IProtoUrlConfig protoConfig = listNode.Value;
                try
                {
                    if (!NodeMatcher.IsMatch(protoConfig.Node)) continue;

                    ConfigNode clone = MMPatchLoader.ModifyNode(this.log, new NodeStack(protoConfig.Node), UrlConfig.config, context);
                    if (protoConfig.Node.GetValue("name") is string name && name == clone.GetValue("name"))
                    {
                        progress.Error(UrlConfig,
                            "Error - when applying copy {0} to {1} - the copy needs to have a different name than the parent (use @name = xxx)",
                            UrlConfig.SafeUrl(), protoConfig.FullUrl
                            );
                    }
                    else
                    {
                        progress.ApplyingCopy(protoConfig, UrlConfig);
                        listNode = databaseConfigs.AddAfter(listNode, new ProtoUrlConfig(protoConfig.UrlFile, clone));
                    }
                }
                catch (Exception ex)
                {
                    progress.Exception(ex, UrlConfig,
                        "Exception while applying copy {0} to {1}",
                        UrlConfig.SafeUrl(), protoConfig.FullUrl
                        );
                }
            }
        }
    }
}
