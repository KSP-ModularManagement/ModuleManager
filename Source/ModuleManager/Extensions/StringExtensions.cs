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
using System.Text.RegularExpressions;

namespace ModuleManager.Extensions
{
    public static class StringExtensions
    {
        public static bool IsBracketBalanced(this string s)
        {
            int level = 0;
            foreach (char c in s)
            {
                if (c == '[') level++;
                else if (c == ']') level--;

                if (level < 0) return false;
            }
            return level == 0;
        }

        private static readonly Regex whitespaceRegex = new Regex(@"\s+");

        public static string RemoveWS(this string withWhite)
        {
            return whitespaceRegex.Replace(withWhite, "");
        }

        public static bool Contains(this string str, string value, out int index)
        {
            if (str == null) throw new ArgumentNullException(nameof(str));
            if (value == null) throw new ArgumentNullException(nameof(value));

            index = str.IndexOf(value, StringComparison.CurrentCultureIgnoreCase);
            return index != -1;
        }
    }
}
