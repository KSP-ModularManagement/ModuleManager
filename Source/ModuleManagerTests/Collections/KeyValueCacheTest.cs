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
using Xunit;
using ModuleManager.Collections;

namespace ModuleManagerTests.Collections
{
    public class KeyValueCacheTest
    {
        [Fact]
        public void TestFetch__CreateValueNull()
        {
            KeyValueCache<object, object> cache = new KeyValueCache<object, object>();
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(delegate
            {
                cache.Fetch(new object(), null);
            });

            Assert.Equal("createValue", ex.ParamName);
        }

        [Fact]
        public void TestFetch__KeyNotPresent()
        {
            object key = new object();
            object value = new object();
            KeyValueCache<object, object> cache = new KeyValueCache<object, object>();

            object fetchedValue = cache.Fetch(key, () => value);

            Assert.Same(value, fetchedValue);
        }

        [Fact]
        public void TestFetch__KeyPresent()
        {
            object key = new object();
            object value = new object();
            KeyValueCache<object, object> cache = new KeyValueCache<object, object>();

            cache.Fetch(key, () => value);

            bool called2ndTime = false;
            object fetchedValue = cache.Fetch(key, delegate
            {
                called2ndTime = true;
                return null;
            });

            Assert.Same(value, fetchedValue);
            Assert.False(called2ndTime);
        }
    }
}
