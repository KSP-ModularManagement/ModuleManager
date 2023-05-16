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
using System.Collections;
using System.Collections.Generic;

namespace ModuleManager.Collections
{
    public interface IMessageQueue<T> : IEnumerable<T>
    {
        void Add(T value);
        IMessageQueue<T> TakeAll();
    }

    public class MessageQueue<T> : IMessageQueue<T>, IEnumerable<T>
    {
        public sealed class Enumerator : IEnumerator<T>
        {
            private readonly MessageQueue<T> queue;
            private Node current;

            public Enumerator(MessageQueue<T> queue)
            {
                this.queue = queue;
            }

            public T Current => current.value;
            object IEnumerator.Current => Current;

            public void Dispose() { }

            public bool MoveNext()
            {
                if (current == null)
                    current = queue.head;
                else
                    current = current.next;

                return current != null;
            }

            public void Reset()
            {
                current = null;
            }
        }

        private class Node
        {
            public Node next;
            public readonly T value;

            public Node(T value)
            {
                this.value = value;
            }
        }

        private readonly object lockObject = new object();
        private Node head;
        private Node tail;

        public void Add(T value)
        {
            Node node = new Node(value);
            lock (lockObject)
            {
                if (head == null)
                {
                    head = node;
                    tail = node;
                }
                else
                {
                    tail.next = node;
                    tail = node;
                }
            }
        }

        public IMessageQueue<T> TakeAll()
        {
            MessageQueue<T> queue = new MessageQueue<T>();
            lock(lockObject)
            {
                queue.head = head;
                queue.tail = tail;
                head = null;
                tail = null;
            }
            return queue;
        }

        public Enumerator GetEnumerator() => new Enumerator(this);
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
