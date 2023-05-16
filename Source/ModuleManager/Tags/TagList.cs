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
using System.Linq;
using ModuleManager.Collections;

namespace ModuleManager.Tags
{
    public interface ITagList : IEnumerable<Tag>
    {
        Tag PrimaryTag { get; }
    }

    public class TagList : ITagList
    {
        private readonly Tag[] tags;

        public TagList(Tag primaryTag, IEnumerable<Tag> tags)
        {
            PrimaryTag = primaryTag;
            this.tags = tags?.ToArray() ?? throw new ArgumentNullException(nameof(tags));
        }
        
        public Tag PrimaryTag { get; private set; }

        public ArrayEnumerator<Tag> GetEnumerator() => new ArrayEnumerator<Tag>(tags);
        IEnumerator<Tag> IEnumerable<Tag>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
