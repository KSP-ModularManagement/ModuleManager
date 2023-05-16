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

namespace ModuleManager.Tags
{
    public struct Tag
    {
        public readonly string key;
        public readonly string value;
        public readonly string trailer;

        public Tag(string key, string value, string trailer)
        {
            this.key = key ?? throw new ArgumentNullException(nameof(key));
            if (key == string.Empty) throw new ArgumentException("can't be empty", nameof(key));

            if (value == null && trailer != null)
                throw new ArgumentException("trailer must be null if value is null");

            if (trailer == string.Empty) throw new ArgumentException("can't be empty (null allowed)", nameof(trailer));

            this.value = value;
            this.trailer = trailer;
        }

        public override string ToString()
        {
            string s = "< '" + key + "' ";
            if (value != null) s += "[ '" + value + "' ] ";
            if (trailer != null) s += "'" + trailer + "' ";
            s += ">";
            return s;
        }
    }
}
