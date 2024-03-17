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
using System.Text;

namespace ModuleManager.Extensions
{
    public static class UrlConfigExtensions
    {
        public static string SafeUrl(this UrlDir.UrlConfig url)
        {
            if (url == null) return "<null>";

            string nodeName;

            if (!string.IsNullOrEmpty(url.type?.Trim()))
            {
                nodeName = url.type;
            }
            else if (url.type == null)
            {
                nodeName = "<null>";
            }
            else
            {
                nodeName = "<blank>";
            }

            string parentUrl = null;

            if (url.parent != null)
            {
                try
                {
                    parentUrl = url.parent.url;
                }
                catch
                {
                    parentUrl = "<unknown>";
                }
            }

            if (parentUrl == null)
                return nodeName;
            else
                return parentUrl + "/" + nodeName;
        }

        public static string PrettyPrint(this UrlDir.UrlConfig config)
        {
            if (config == null) return "<null UrlConfig>";

            StringBuilder sb = new StringBuilder();

            sb.Append(config.SafeUrl());
            sb.Append('\n');
            config.config.PrettyPrint(ref sb, "  ");
            
            return sb.ToString();
        }
    }
}
