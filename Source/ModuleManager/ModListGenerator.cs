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
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;
using ModuleManager.Extensions;
using ModuleManager.Logging;
using ModuleManager.Utils;
using ModuleManager.Progress;

namespace ModuleManager
{
    public static class ModListGenerator
    {
        public static IEnumerable<string> GenerateModList(IEnumerable<ModAddedByAssembly> modsAddedByAssemblies, IPatchProgress progress, IBasicLogger logger)
        {
            #region List of mods

            //string envInfo = "ModuleManager env info\n";
            //envInfo += "  " + Environment.OSVersion.Platform + " " + ModuleManager.intPtr.ToInt64().ToString("X16") + "\n";
            //envInfo += "  " + Convert.ToString(ModuleManager.intPtr.ToInt64(), 2)  + " " + Convert.ToString(ModuleManager.intPtr.ToInt64() >> 63, 2) + "\n";
            //string gamePath = Environment.GetCommandLineArgs()[0];
            //envInfo += "  Args: " + gamePath.Split(Path.DirectorySeparatorChar).Last() + " " + string.Join(" ", Environment.GetCommandLineArgs().Skip(1).ToArray()) + "\n";
            //envInfo += "  Executable SHA256 " + FileSHA(gamePath);
            //
            //log(envInfo);

            List<string> mods = new List<string>();

            StringBuilder modListInfo = new StringBuilder();

            modListInfo.Append("compiling list of loaded mods...\nMod DLLs found:\n");

            string format = "  {0,-40}{1,-25}{2,-25}{3,-25}{4}\n";

            modListInfo.AppendFormat(
                format,
                "Name",
                "Assembly Version",
                "Assembly File Version",
                "KSPAssembly Version",
                "SHA256"
            );

            modListInfo.Append('\n');

            foreach (AssemblyLoader.LoadedAssembly mod in AssemblyLoader.loadedAssemblies)
            {

                if (string.IsNullOrEmpty(mod.assembly.Location)) //Diazo Edit for xEvilReeperx AssemblyReloader mod
                    continue;

                FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(mod.assembly.Location);

                AssemblyName assemblyName = mod.assembly.GetName();

                string kspAssemblyVersion;
                if (mod.versionMajor == 0 && mod.versionMinor == 0)
                    kspAssemblyVersion = "";
                else
                    kspAssemblyVersion = mod.versionMajor + "." + mod.versionMinor;

                string fileSha = "";
                try
                {
                    fileSha = FileUtils.FileSHA(mod.assembly.Location);
                }
                catch (Exception e)
                {
                    progress.Exception(e, "Exception while generating SHA for assembly {0}", assemblyName.Name);
                }

                modListInfo.AppendFormat(
                    format,
                    assemblyName.Name,
                    assemblyName.Version,
                    fileVersionInfo.FileVersion,
                    kspAssemblyVersion,
                    fileSha
                );

                // modlist += String.Format("  {0,-50} SHA256 {1}\n", modInfo, FileSHA(mod.assembly.Location));

                if (!mods.Contains(assemblyName.Name, StringComparer.OrdinalIgnoreCase))
                    mods.Add(assemblyName.Name);
            }

            modListInfo.Append("Non-DLL mods added (:FOR[xxx]):\n");
            foreach (UrlDir.UrlConfig cfgmod in GameDatabase.Instance.root.AllConfigs)
            {
                if (CommandParser.Parse(cfgmod.type, out string name) != Command.Insert)
                {
                    if (name.Contains(":FOR["))
                    {
                        name = name.RemoveWS();

                        // check for FOR[] blocks that don't match loaded DLLs and add them to the pass list
                        try
                        {
                            string dependency = name.Substring(name.IndexOf(":FOR[") + 5);
                            dependency = dependency.Substring(0, dependency.IndexOf(']'));
                            if (!mods.Contains(dependency, StringComparer.OrdinalIgnoreCase))
                            {
                                // found one, now add it to the list.
                                mods.Add(dependency);
                                modListInfo.AppendFormat("  {0}\n", dependency);
                            }

							if (name.Contains(":NEEDS["))
							{
								string needs = (name.Substring(name.IndexOf(":NEEDS[") + 7));
								needs = needs.Substring(0, needs.IndexOf(']'));
								needs = needs.Replace("!", "").Replace('&', ',').Replace('|', ',');
								foreach (string s in needs.Split(',')) if (dependency.Equals(s))
										progress.ForWithInvalidNeedsWarning(dependency, cfgmod);
							}
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            progress.Error(cfgmod,
                                "Skipping :FOR init for line {0}. The line most likely contains a space that should be removed",
                                name
                                );
                        }
                    }
                }
            }
            modListInfo.Append("Mods by directory (sub directories of GameData):\n");
            UrlDir gameData = GameDatabase.Instance.root.children.First(dir => dir.type == UrlDir.DirectoryType.GameData);
            foreach (UrlDir subDir in gameData.children)
            {
                string cleanName = subDir.name.RemoveWS();
                if (!mods.Contains(cleanName, StringComparer.OrdinalIgnoreCase))
                {
                    mods.Add(cleanName);
                    modListInfo.AppendFormat("  {0}\n", cleanName);
                }
            }

            modListInfo.Append("Mods added by assemblies:\n");
            foreach (ModAddedByAssembly mod in modsAddedByAssemblies)
            {
                if (!mods.Contains(mod.modName, StringComparer.OrdinalIgnoreCase))
                {
                    mods.Add(mod.modName);
                    modListInfo.AppendFormat("  {0}\n", mod);
                }
            }

            logger.Info("{0}", modListInfo);

            mods.Sort();

            #endregion List of mods

            return mods;
        }

        public class ModAddedByAssembly
        {
            public readonly string modName;
            public readonly string assemblyName;

            public ModAddedByAssembly(string modName, string assemblyName)
            {
                this.modName = modName ?? throw new ArgumentNullException(nameof(modName));
                this.assemblyName = assemblyName ?? throw new ArgumentNullException(nameof(assemblyName));
            }

            public override string ToString()
            {
                return $"{modName} (added by {assemblyName})";
            }
        }

        public static IEnumerable<ModAddedByAssembly> GetAdditionalModsFromStaticMethods(IBasicLogger logger)
        {
            List<ModAddedByAssembly> result = new List<ModAddedByAssembly>();
            foreach (Assembly ass in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    foreach (Type type in ass.GetTypes())
                    {
                        MethodInfo method = type.GetMethod("ModuleManagerAddToModList", BindingFlags.Public | BindingFlags.Static);

                        if (method != null && method.GetParameters().Length == 0 && typeof(IEnumerable<string>).IsAssignableFrom(method.ReturnType))
                        {
                            string methodName = $"{ass.GetName().Name}.{type.Name}.{method.Name}()";
                            try
                            {
                                logger.Info("Calling {0}", methodName);
                                IEnumerable<string> modsToAdd = (IEnumerable<string>)method.Invoke(null, null);

                                if (modsToAdd == null)
                                {
                                    logger.Error("ModuleManagerAddToModList returned null: {0}", methodName);
                                    continue;
                                }

                                foreach (string mod in modsToAdd)
                                {
                                    result.Add(new ModAddedByAssembly(mod, ass.GetName().Name));
                                }
                            }
                            catch (Exception e)
                            {
                                logger.Exception(e, "Exception while calling {0}", methodName);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    logger.Exception(e, "Add to mod list threw an exception in loading {0}", ass.FullName);
                }
            }

            foreach (MonoBehaviour obj in UnityEngine.Object.FindObjectsOfType<MonoBehaviour>())
            {
                MethodInfo method = obj.GetType().GetMethod("ModuleManagerAddToModList", BindingFlags.Public | BindingFlags.Instance);

                if (method != null && method.GetParameters().Length == 0 && typeof(IEnumerable<string>).IsAssignableFrom(method.ReturnType))
                {
                    string methodName = $"{obj.GetType().Name}.{method.Name}()";
                    try
                    {
                        logger.Info("Calling {0}", methodName);
                        IEnumerable<string> modsToAdd = (IEnumerable<string>)method.Invoke(obj, null);

                        if (modsToAdd == null)
                        {
                            logger.Error("ModuleManagerAddToModList returned null: {0}", methodName);
                            continue;
                        }

                        foreach (string mod in modsToAdd)
                        {
                            result.Add(new ModAddedByAssembly(mod, obj.GetType().Assembly.GetName().Name));
                        }
                    }
                    catch (Exception e)
                    {
                        logger.Exception(e, "Exception while calling {0}", methodName);
                    }
                }
            }

            return result;
        }
    }
}
