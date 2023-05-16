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
using System.Collections.Generic;

namespace ModuleManager.Collections
{
    public class KeyValueCache<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> dict = new Dictionary<TKey, TValue>();
        private readonly object lockObject = new object();

        public TValue Fetch(TKey key, Func<TValue> createValue)
        {
            if (createValue == null) throw new ArgumentNullException(nameof(createValue));
            lock(lockObject)
            {
                if (dict.TryGetValue(key, out TValue value))
                {
                    return value;
                }
                else
                {
                    TValue newValue = createValue();
                    dict.Add(key, newValue);
                    return newValue;
                }
            }
        }
    }
}
