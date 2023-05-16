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

namespace ModuleManager
{
    public interface IUrlConfigIdentifier
    {
        string FileUrl { get; }
        string NodeType { get; }
        string FullUrl { get; }
    }

    public interface IProtoUrlConfig : IUrlConfigIdentifier
    {
        UrlDir.UrlFile UrlFile { get; }
        ConfigNode Node { get; }
    }

    public class ProtoUrlConfig : IProtoUrlConfig
    {
        public UrlDir.UrlFile UrlFile { get; }
        public ConfigNode Node { get; }
        public string FileUrl { get; }
        public string NodeType => Node.name;
        public string FullUrl { get; }

        public ProtoUrlConfig(UrlDir.UrlFile urlFile, ConfigNode node)
        {
            UrlFile = urlFile ?? throw new ArgumentNullException(nameof(urlFile));
            Node = node ?? throw new ArgumentNullException(nameof(node));
            FileUrl = UrlFile.url + '.' + urlFile.fileExtension;
            FullUrl = FileUrl + '/' + Node.name;

            if (node.GetValue("name") is string nameValue)
                FullUrl += '[' + nameValue + ']';
        }
    }
}
