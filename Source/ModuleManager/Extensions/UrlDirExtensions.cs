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
using System.Linq;

namespace ModuleManager.Extensions
{
    public static class UrlDirExtensions
    {
        public static UrlDir.UrlFile Find(this UrlDir urlDir, string url)
        {
            if (urlDir == null) throw new ArgumentNullException(nameof(urlDir));
            if (url == null) throw new ArgumentNullException(nameof(url));
            string[] splits = url.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            UrlDir currentDir = urlDir;

            for (int i = 0; i < splits.Length - 1; i++)
            {
                currentDir = currentDir.children.FirstOrDefault(subDir => subDir.name == splits[i]);
                if (currentDir == null) return null;
            }
            
            string fileName = splits[splits.Length - 1];
            string fileExtension = null;

            int idx = fileName.LastIndexOf('.');

            if (idx > -1)
            {
                fileExtension = fileName.Substring(idx + 1);
                fileName = fileName.Substring(0, idx);
            }

            foreach (UrlDir.UrlFile file in currentDir.files)
            {
                if (file.name != fileName) continue;
                if (fileExtension != null && fileExtension != file.fileExtension) continue;
                return file;
            }

            return null;
        }
    }
}
