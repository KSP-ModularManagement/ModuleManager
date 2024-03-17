/*
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
using ModuleManager.Extensions;
using ModuleManager.Patches.PassSpecifiers;
using ModuleManager.Progress;
using ModuleManager.Tags;

namespace ModuleManager.Patches
{
    public interface IProtoPatchBuilder
    {
        ProtoPatch Build(UrlDir.UrlConfig urlConfig, Command command, ITagList tagList);
    }

    public class ProtoPatchBuilder : IProtoPatchBuilder
    {
        private readonly IPatchProgress progress;

        public ProtoPatchBuilder(IPatchProgress progress)
        {
            this.progress = progress ?? throw new ArgumentNullException(nameof(progress));
        }

        public ProtoPatch Build(UrlDir.UrlConfig urlConfig, Command command, ITagList tagList)
        {
            if (urlConfig == null) throw new ArgumentNullException(nameof(urlConfig));
            if (tagList == null) throw new ArgumentNullException(nameof(tagList));
            if (progress == null) throw new ArgumentNullException(nameof(progress));

            bool error = false;

            string nodeType = tagList.PrimaryTag.key;
            string nodeName = tagList.PrimaryTag.value;

            if (command == Command.Insert && nodeName != null)
            {
                progress.Error(urlConfig, "name specifier detected on insert node (not a patch): {0}", urlConfig.SafeUrl());
                error = true;
            }

            if (nodeName == string.Empty)
            {
                progress.Warning(urlConfig, "empty brackets detected on patch name: {0}", urlConfig.SafeUrl());
                nodeName = null;
            }

            if (tagList.PrimaryTag.trailer != null)
                progress.Warning(urlConfig, "unrecognized trailer: '{0}' on: {1}", tagList.PrimaryTag.trailer, urlConfig.SafeUrl());

            string needs = null;
            string has = null;
            IPassSpecifier passSpecifier = null;

            foreach (Tag tag in tagList)
            {
                if (tag.trailer != null)
                    progress.Warning(urlConfig, "unrecognized trailer: '{0}' on: {1}", tag.trailer, urlConfig.SafeUrl());

                if (tag.key.Equals("NEEDS", StringComparison.CurrentCultureIgnoreCase))
                {
                    if (needs != null)
                    {
                        progress.Warning(urlConfig, "more than one :NEEDS tag detected, ignoring all but the first: {0}", urlConfig.SafeUrl());
                        continue;
                    }
                    if (string.IsNullOrEmpty(tag.value))
                    {
                        progress.Error(urlConfig, "empty :NEEDS tag detected: {0}", urlConfig.SafeUrl());
                        error = true;
                        continue;
                    }

                    needs = tag.value;
                }
                else if (tag.key.Equals("HAS", StringComparison.CurrentCultureIgnoreCase))
                {
                    if (command == Command.Insert)
                    {
                        progress.Error(urlConfig, ":HAS detected on insert node (not a patch): {0}", urlConfig.SafeUrl());
                        error = true;
                        continue;
                    }
                    if (has != null)
                    {
                        progress.Warning(urlConfig, "more than one :HAS tag detected, ignoring all but the first: {0}", urlConfig.SafeUrl());
                        continue;
                    }
                    if (string.IsNullOrEmpty(tag.value))
                    {
                        progress.Error(urlConfig, "empty :HAS tag detected: {0}", urlConfig.SafeUrl());
                        error = true;
                        continue;
                    }

                    has = tag.value;
                }
                else if (tag.key.Equals("FIRST", StringComparison.CurrentCultureIgnoreCase))
                {
                    if (tag.value != null)
                    {
                        progress.Warning(urlConfig, "value detected on :FIRST tag: {0}", urlConfig.SafeUrl());
                    }

                    if (command == Command.Insert)
                    {
                        progress.Error(urlConfig, "pass specifier detected on insert node (not a patch): {0}", urlConfig.SafeUrl());
                        error = true;
                        continue;
                    }
                    if (passSpecifier != null)
                    {
                        progress.Warning(urlConfig, "more than one pass specifier detected, ignoring all but the first: {0}", urlConfig.SafeUrl());
                        continue;
                    }

                    passSpecifier = new FirstPassSpecifier();
                }
                else if (tag.key.Equals("BEFORE", StringComparison.CurrentCultureIgnoreCase))
                {
                    if (string.IsNullOrEmpty(tag.value))
                    {
                        progress.Error(urlConfig, "empty :BEFORE tag detected: {0}", urlConfig.SafeUrl());
                        error = true;
                        continue;
                    }

                    if (command == Command.Insert)
                    {
                        progress.Error(urlConfig, "pass specifier detected on insert node (not a patch): {0}", urlConfig.SafeUrl());
                        error = true;
                        continue;
                    }
                    if (passSpecifier != null)
                    {
                        progress.Warning(urlConfig, "more than one pass specifier detected, ignoring all but the first: " + urlConfig.SafeUrl());
                        continue;
                    }

                    passSpecifier = new BeforePassSpecifier(tag.value, urlConfig);
                }
                else if (tag.key.Equals("FOR", StringComparison.CurrentCultureIgnoreCase))
                {
                    if (string.IsNullOrEmpty(tag.value))
                    {
                        progress.Error(urlConfig, "empty :FOR tag detected: {0}", urlConfig.SafeUrl());
                        error = true;
                        continue;
                    }

                    if (command == Command.Insert)
                    {
                        progress.Error(urlConfig, "pass specifier detected on insert node (not a patch): {0}", urlConfig.SafeUrl());
                        error = true;
                        continue;
                    }
                    if (passSpecifier != null)
                    {
                        progress.Warning(urlConfig, "more than one pass specifier detected, ignoring all but the first: {0}", urlConfig.SafeUrl());
                        continue;
                    }

                    passSpecifier = new ForPassSpecifier(tag.value, urlConfig);
                }
                else if (tag.key.Equals("AFTER", StringComparison.CurrentCultureIgnoreCase))
                {
                    if (string.IsNullOrEmpty(tag.value))
                    {
                        progress.Error(urlConfig, "empty :AFTER tag detected: {0}", urlConfig.SafeUrl());
                        error = true;
                        continue;
                    }

                    if (command == Command.Insert)
                    {
                        progress.Error(urlConfig, "pass specifier detected on insert node (not a patch): {0}", urlConfig.SafeUrl());
                        error = true;
                        continue;
                    }
                    if (passSpecifier != null)
                    {
                        progress.Warning(urlConfig, "more than one pass specifier detected, ignoring all but the first: {0}", urlConfig.SafeUrl());
                        continue;
                    }

                    passSpecifier = new AfterPassSpecifier(tag.value, urlConfig);
                }
                else if (tag.key.Equals("LAST", StringComparison.CurrentCultureIgnoreCase))
                {
                    if (string.IsNullOrEmpty(tag.value))
                    {
                        progress.Error(urlConfig, "empty :LAST tag detected: {0}", urlConfig.SafeUrl());
                        error = true;
                        continue;
                    }

                    if (command == Command.Insert)
                    {
                        progress.Error(urlConfig, "pass specifier detected on insert node (not a patch): {0}", urlConfig.SafeUrl());
                        error = true;
                        continue;
                    }
                    if (passSpecifier != null)
                    {
                        progress.Warning(urlConfig, "more than one pass specifier detected, ignoring all but the first: {0}", urlConfig.SafeUrl());
                        continue;
                    }

                    passSpecifier = new LastPassSpecifier(tag.value);
                }
                else if (tag.key.Equals("FINAL", StringComparison.CurrentCultureIgnoreCase))
                {
                    if (tag.value != null)
                    {
                        progress.Warning(urlConfig, "value detected on :FINAL tag: {0}", urlConfig.SafeUrl());
                    }

                    if (command == Command.Insert)
                    {
                        progress.Error(urlConfig, "pass specifier detected on insert node (not a patch): {0}", urlConfig.SafeUrl());
                        error = true;
                        continue;
                    }
                    if (passSpecifier != null)
                    {
                        progress.Warning(urlConfig, "more than one pass specifier detected, ignoring all but the first: {0}", urlConfig.SafeUrl());
                        continue;
                    }

                    passSpecifier = new FinalPassSpecifier();
                }
                else
                {
                    progress.Warning(urlConfig, "unrecognized tag: '{0}' on: {1}", tag.key, urlConfig.SafeUrl());
                }
            }

            if (error) return null;

            if (passSpecifier == null)
            {
                if (command == Command.Insert) passSpecifier = new InsertPassSpecifier();
                else passSpecifier = new LegacyPassSpecifier();
            }

            return new ProtoPatch(urlConfig, command, nodeType, nodeName, needs, has, passSpecifier);
        }
    }
}
