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
