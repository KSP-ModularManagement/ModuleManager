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
using PluginConfig = KSPe.IO.Data<ModuleManager.ModuleManager>.ConfigNode;
using KspConfig = KSPe.IO.KspConfigNode;

namespace ModuleManager
{
    internal static class FilePathRepository
    {
        internal static readonly PluginConfig SHA_CONFIG = PluginConfig.For(null, "ConfigSHA.cfg");
        internal static readonly PluginConfig CACHE_CONFIG = PluginConfig.For(null, "ConfigCache.cfg");
        internal static readonly PluginConfig PHYSICS_CONFIG = PluginConfig.For(null, "Physics.cfg");
        internal static readonly PluginConfig TECHTREE_CONFIG = PluginConfig.For("TechTree");
        internal static readonly KspConfig PHYSICS_DEFAULT = new KspConfig("Physics");
        internal static readonly KspConfig PART_DATABASE = new KspConfig("PartDatabase");

        internal static readonly string MMCfgOutputPath = KSPe.IO.File<ModuleManager>.Data.Solve("_MMCfgOutput");
        internal static readonly string PATCH_LOG_FILENAME = "MMPatch";
    }
}
