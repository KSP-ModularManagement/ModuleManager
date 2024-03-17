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

namespace ModuleManager.Extensions
{
    public static class UrlFileExtensions
    {
        public static string GetUrlWithExtension(this UrlDir.UrlFile urlFile)
        {
            return $"{urlFile.url}.{urlFile.fileExtension}";
        }
        public static string GetNameWithExtension(this UrlDir.UrlFile urlFile)
        {
            return $"{urlFile.name}.{urlFile.fileExtension}";
        }
    }
}
