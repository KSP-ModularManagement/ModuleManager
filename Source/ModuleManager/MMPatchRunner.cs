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
using System.Collections;
using System.Collections.Generic;
using ModuleManager.Extensions;
using ModuleManager.Logging;
using ModuleManager.Threading;

using static ModuleManager.FilePathRepository;

namespace ModuleManager
{
    public class MMPatchRunner
    {
        private readonly IBasicLogger kspLogger;
        private readonly Progress.ProgressCounter counter;
        private readonly Progress.Timings timings;

        public string Status { get; private set; } = "";
        public string Errors { get; private set; } = "";

        internal MMPatchLoader patchLoader;

        public MMPatchRunner(IBasicLogger kspLogger, Progress.ProgressCounter counter, Progress.Timings timings)
        {
            this.kspLogger = kspLogger ?? throw new ArgumentNullException(nameof(kspLogger));
            this.counter = counter;
            this.timings = timings;
        }

        public IEnumerator Run()
        {
            PostPatchLoader.Instance.databaseConfigs = null;

            // Wait for game database to be initialized for the 2nd time and wait for any plugins to initialize
            yield return null;
            yield return null;

            IEnumerable<ModListGenerator.ModAddedByAssembly> modsAddedByAssemblies = ModListGenerator.GetAdditionalModsFromStaticMethods(ModLogger.Instance);

            IEnumerable<IProtoUrlConfig> databaseConfigs = null;
            this.patchLoader = new MMPatchLoader(modsAddedByAssemblies, ModLogger.Instance, this.counter, this.timings);

            ITaskStatus patchingThreadStatus = BackgroundTask.Start(delegate
            {
                databaseConfigs = patchLoader.Run();
            });

            while(true)
            {
                yield return null;

                Status = patchLoader.status;
                Errors = patchLoader.errors;

                if (!patchingThreadStatus.IsRunning) break;
            }

            if (patchingThreadStatus.IsExitedWithError)
            {
                kspLogger.Exception("The patching thread threw an exception", patchingThreadStatus.Exception);
                FatalErrorHandler.HandleFatalError("The patching thread threw an exception");
                yield break;
            }

            if (databaseConfigs == null)
            {
                kspLogger.Error("The patcher returned a null collection of configs");
                FatalErrorHandler.HandleFatalError("The patcher returned a null collection of configs");
                yield break;
            }

            PostPatchLoader.Instance.databaseConfigs = databaseConfigs;
        }
    }
}
