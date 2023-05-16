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
using System.Collections.Generic;
using ModuleManager.Logging;
using ModuleManager.Extensions;
using ModuleManager.Patches;
using ModuleManager.Progress;

namespace ModuleManager
{
    public class PatchApplier
    {
        private readonly IBasicLogger logger;
        private readonly IPatchProgress progress;

        public PatchApplier(IPatchProgress progress, IBasicLogger logger)
        {
            this.progress = progress ?? throw new ArgumentNullException(nameof(progress));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IEnumerable<IProtoUrlConfig> ApplyPatches(IEnumerable<IPass> patches)
        {
            if (patches == null) throw new ArgumentNullException(nameof(patches));

            LinkedList<IProtoUrlConfig> databaseConfigs = new LinkedList<IProtoUrlConfig>();

            foreach (IPass pass in patches)
            {
                ApplyPatches(databaseConfigs, pass);
            }

            return databaseConfigs; 
        }

        private void ApplyPatches(LinkedList<IProtoUrlConfig> databaseConfigs, IPass pass)
        {
            progress.PassStarted(pass);

            foreach (IPatch patch in pass)
            {
                try
                {
                    patch.Apply(databaseConfigs, progress, logger);
                    if (patch.CountsAsPatch) progress.PatchApplied();
                }
                catch (Exception e)
                {
                    progress.Exception(patch.UrlConfig, "Exception while processing node : " + patch.UrlConfig.SafeUrl(), e);
                    logger.Error("Processed node was\n" + patch.UrlConfig.PrettyPrint());
                }
            }
        }
    }
}
