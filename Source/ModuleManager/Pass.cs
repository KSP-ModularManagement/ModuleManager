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
using System.Collections;
using System.Collections.Generic;
using ModuleManager.Patches;

namespace ModuleManager
{
    public interface IPass : IEnumerable<IPatch>
    {
        string Name { get; }
    }

    public class Pass : IPass
    {
        private readonly string name;
        private readonly List<IPatch> patches = new List<IPatch>(0);

        public Pass(string name)
        {
            this.name = name ?? throw new ArgumentNullException(nameof(name));
            if (name == string.Empty) throw new ArgumentException("can't be empty", nameof(name));
        }

        public string Name => name;

        public void Add(IPatch patch) => patches.Add(patch);

        public List<IPatch>.Enumerator GetEnumerator() => patches.GetEnumerator();
        IEnumerator<IPatch> IEnumerable<IPatch>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
