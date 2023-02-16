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
using ModuleManager.Progress;

namespace ModuleManager.Patches.PassSpecifiers
{
    public class ForPassSpecifier : IPassSpecifier
    {
        public readonly string mod;
        public readonly UrlDir.UrlConfig urlConfig;

        public ForPassSpecifier(string mod, UrlDir.UrlConfig urlConfig)
        {
            if (mod == string.Empty) throw new ArgumentException("can't be empty", nameof(mod));
            this.mod = mod ?? throw new ArgumentNullException(nameof(mod));
            this.urlConfig = urlConfig ?? throw new ArgumentNullException(nameof(urlConfig));
        }

        public bool CheckNeeds(INeedsChecker needsChecker, IPatchProgress progress)
        {
            if (needsChecker == null) throw new ArgumentNullException(nameof(needsChecker));
            if (progress == null) throw new ArgumentNullException(nameof(progress));
            bool result = needsChecker.CheckNeeds(mod);
            if (!result) progress.NeedsUnsatisfiedFor(urlConfig);
            return result;
        }

        public string Descriptor => $":FOR[{mod.ToUpper()}]";
    }
}
