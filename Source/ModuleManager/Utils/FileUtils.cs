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
using System.IO;
using System.Security.Cryptography;
using System.Threading;

using ModuleManager.Extensions;
using Log = ModuleManager.Logging.ModLogger;

namespace ModuleManager.Utils
{
    public static class FileUtils
    {
        public static string FileSHA(string filename)
        {
            if (!File.Exists(filename)) throw new FileNotFoundException("File does not exist", filename);

            byte[] data = null;

            {
				/*
				 * This one is hairy. Absolutely hairy.
				 *
				 * By some reason, pretty powerful rigs are getting screwed by what I think is a race condition (perhaps induced by MM being
				 * instantiated twice somehow), with the file failing being opened due it being already opened by someone else.
				 *
				 * It was postulated that KSP would be the one keeping these files opened. I think it's not because by the time
				 * Module Manager is started up, KSP had already loaded and resolved every single DLL (and its dependencies) and, so,
				 * there will be just not a single logic reason to keep them opened in memory - unless KSP is not calling the Dispose
				 * or not using the `using` construction and, so, we have a file handlers leaking on this damned thing.
				 *
				 * In a way or another, the proposed solution on pull/request 180 to the upstream is, IMHO, less than ideal.
				 * **WE JUST DON'T** open executable files with Writing privileges, **POINT**. At very least, this will prevent anti-virus
				 * software from being triggered on us, avoiding slowing down KSP's file accesses.
				 *
				 * So I will not use `FileShare.ReadWrite` no matter what. I terminantly refuse to do so.
				 * 
				 * But I had to reconsider and use `FileShare.Read` after doing some benchmarks, as by some reason it's the fastest 
				 * way to open a file under C#, even on a UNIX.
				 *
				 * See:
				 *	+ https://github.com/sarbian/ModuleManager/pull/180
				 *	+ https://forum.kerbalspaceprogram.com/index.php?/topic/50533-18x-112x-module-manager-422-june-18th-2022-the-heatwave-edition/page/302/#comment-4283448
				 *	+ https://forum.kerbalspaceprogram.com/index.php?/topic/50533-18x-112x-module-manager-422-june-18th-2022-the-heatwave-edition/page/303/#comment-4284427
				 *	+ https://github.com/net-lisias-ksp/KSPe/commit/4fcced165ce72edcf5db2c95311ebafc02d6a921
				 */
				Exception ex = null;
				int i = 8;	// Max wait: 1 second
				while (i-- > 0) try
				{
					using (SHA256 sha = SHA256.Create())
						using (FileStream fs = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
							data = sha.ComputeHash(fs);
					break;
				}
				catch (Exception e)
				{
					ex = e;
					Log.LOG.detail("Error {0} on reading {1} on regressive count {2}!", e.Message, filename, i);
					GC.Collect();		// For the hypothesis KSP is leaking file handlers
					Thread.Sleep(125);	// In milliseconds
				}
				if (0 == i && null != ex) throw ex;
				if (null != ex)
					Log.LOG.warn("File {0} suffered at least one Exception with message \"{1}\" while being processed. The problem was recovered, but this log was issued to mark the event.", filename, ex.Message);
            }

            return data.ToHex();
        }
    }
}
