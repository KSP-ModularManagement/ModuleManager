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

namespace ModuleManager
{
    public static class CommandParser
    {
        public static Command Parse(string name, out string valueName)
        {
            if (name.Length == 0)
            {
                valueName = string.Empty;
                return Command.Insert;
            }
            Command ret;
            switch (name[0])
            {
                case '@':
                    ret = Command.Edit;
                    break;

                case '%':
                    ret = Command.Replace;
                    break;

                case '-':
                case '!':
                    ret = Command.Delete;
                    break;

                case '+':
                case '$':
                    ret = Command.Copy;
                    break;

                case '|':
                    ret = Command.Rename;
                    break;

                case '#':
                    ret = Command.Paste;
                    break;

                case '*':
                    ret = Command.Special;
                    break;

                case '&':
                    ret = Command.Create;
                    break;

                default:
                    valueName = name;
                    return Command.Insert;
            }
            valueName = name.Substring(1);
            return ret;
        }
    }
}
