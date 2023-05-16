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
using ModuleManager.Utils;

namespace ModuleManager.Progress
{
    public class ProgressCounter
    {
        public readonly Counter totalPatches = new Counter();
        public readonly Counter appliedPatches = new Counter();
        public readonly SetableCounter patchedNodes = new SetableCounter();
        public readonly Counter warnings = new Counter();
        public readonly Counter errors = new Counter();
        public readonly Counter exceptions = new Counter();
        public readonly Counter needsUnsatisfied = new Counter();

        public readonly Dictionary<String, int> warningFiles = new Dictionary<string, int>();
        public readonly Dictionary<String, int> errorFiles = new Dictionary<string, int>();
    }
}
