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

using ModuleManager.Logging;

namespace ModuleManager.Patches
{
    public interface IPatchCompiler
    {
        IPatch CompilePatch(ProtoPatch protoPatch);
    }

    public class PatchCompiler : IPatchCompiler
    {
        private readonly IBasicLogger log;
		public PatchCompiler(IBasicLogger patchLogger)
		{
			this.log = patchLogger;
		}

		public IPatch CompilePatch(ProtoPatch protoPatch)
        {
            if (protoPatch == null) throw new ArgumentNullException(nameof(protoPatch));

            switch (protoPatch.command)
            {
                case Command.Insert:
                    return new InsertPatch(this.log, protoPatch.urlConfig, protoPatch.nodeType, protoPatch.passSpecifier);

                case Command.Edit:
                    return new EditPatch(this.log, protoPatch.urlConfig, new NodeMatcher(this.log, protoPatch.nodeType, protoPatch.nodeName, protoPatch.has), protoPatch.passSpecifier);

                case Command.Copy:
                    return new CopyPatch(this.log, protoPatch.urlConfig, new NodeMatcher(this.log, protoPatch.nodeType, protoPatch.nodeName, protoPatch.has), protoPatch.passSpecifier);

                case Command.Delete:
                    return new DeletePatch(this.log, protoPatch.urlConfig, new NodeMatcher(this.log, protoPatch.nodeType, protoPatch.nodeName, protoPatch.has), protoPatch.passSpecifier);

                default:
                    throw new ArgumentException("has an invalid command for a root node: " + protoPatch.command, nameof(protoPatch));
            }
        }
    }
}
