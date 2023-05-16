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
using System.Linq;
using System.Text;
using Xunit;
using ModuleManager.Utils;

namespace ModuleManagerTests.Utils
{
    public class CounterTest
    {
        [Fact]
        public void Test__Constructor()
        {
            Counter counter = new Counter();

            Assert.Equal(0, counter.Value);
        }

        [Fact]
        public void TestIncrement()
        {
            Counter counter = new Counter();

            Assert.Equal(0, counter.Value);

            counter.Increment();

            Assert.Equal(1, counter.Value);

            counter.Increment();

            Assert.Equal(2, counter.Value);
        }

        [Fact]
        public void TestToString()
        {
            Counter counter = new Counter();
            
            Assert.Equal("0", counter.ToString());

            counter.Increment();
            
            Assert.Equal("1", counter.ToString());

            counter.Increment();
            
            Assert.Equal("2", counter.ToString());
        }

        [Fact]
        public void Test__CastAsInt()
        {
            Counter counter = new Counter();
            int i;

            i = counter;
            Assert.Equal(0, i);

            counter.Increment();

            i = counter;
            Assert.Equal(1, i);

            counter.Increment();

            i = counter;
            Assert.Equal(2, i);
        }
    }
}
