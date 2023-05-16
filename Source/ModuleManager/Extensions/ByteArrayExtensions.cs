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

namespace ModuleManager.Extensions
{
    public static class ByteArrayExtensions
    {
        public static string ToHex(this byte[] data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            char[] result = new char[data.Length * 2];

            for (int i = 0; i < data.Length; i++)
            {
                result[i * 2] = GetHexValue(data[i] / 16);
                result[i * 2 + 1] = GetHexValue(data[i] % 16);
            }

            return new string(result);
        }

        private static char GetHexValue(int i)
        {
            if (i < 10)
                return (char)(i + '0');
            else
                return (char)(i - 10 + 'a');
        }
    }
}
