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
using ModuleManager.Extensions;

namespace ModuleManager.Utils
{
    public static class FileUtils
    {
        public static string FileSHA(string filename)
        {
            if (!File.Exists(filename)) throw new FileNotFoundException("File does not exist", filename);

            byte[] data = null;

            using (SHA256 sha = SHA256.Create())
            {
                using (FileStream fs = File.Open(filename, FileMode.Open, FileAccess.Read))
                {
                    data = sha.ComputeHash(fs);
                }
            }

            return data.ToHex();
        }
    }
}
