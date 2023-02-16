/*
	This file is part of Module Manager /L
		© 2018-2023 LisiasT
		© 2013-2018 Sarbian using System; Blowfish
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
    public class MessageQueueTest
    {
        private class TestClass { }

        private readonly MessageQueue<TestClass> queue = new MessageQueue<TestClass>();

        [Fact]
        public void Test__Empty()
        {
            Assert.Empty(queue);
        }

        [Fact]
        public void TestAdd()
        {
            TestClass o1 = new TestClass();
            TestClass o2 = new TestClass();
            TestClass o3 = new TestClass();

            queue.Add(o1);
            queue.Add(o2);
            queue.Add(o3);

            Assert.Equal(new[] { o1, o2, o3 }, queue);
        }

        [Fact]
        public void TestTakeAll()
        {
            TestClass o1 = new TestClass();
            TestClass o2 = new TestClass();
            TestClass o3 = new TestClass();
            TestClass o4 = new TestClass();

            queue.Add(o1);
            queue.Add(o2);
            queue.Add(o3);

            MessageQueue<TestClass> queue2 = Assert.IsType<MessageQueue<TestClass>>(queue.TakeAll());

            queue.Add(o4);

            Assert.Equal(new[] { o4 }, queue);
            Assert.Equal(new[] { o1, o2, o3 }, queue2);
        }
    }
}
