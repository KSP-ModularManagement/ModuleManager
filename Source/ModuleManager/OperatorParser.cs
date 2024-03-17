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
    public static class OperatorParser
    {
        public static Operator Parse(string name, out string valueName)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            if (name.Length == 0)
            {
                valueName = string.Empty;
                return Operator.Assign;
            }
            else if (name.Length == 1 || (name[name.Length - 2] != ' ' && name[name.Length - 2] != '\t'))
            {
                valueName = name;
                return Operator.Assign;
            }

            Operator ret;
            switch (name[name.Length - 1])
            {
                case '+':
                    ret = Operator.Add;
                    break;

                case '-':
                    ret = Operator.Subtract;
                    break;

                case '*':
                    ret = Operator.Multiply;
                    break;

                case '/':
                    ret = Operator.Divide;
                    break;

                case '!':
                    ret = Operator.Exponentiate;
                    break;

                case '^':
                    ret = Operator.RegexReplace;
                    break;

                default:
                    valueName = name;
                    return Operator.Assign;
            }
            valueName = name.Substring(0, name.Length - 1).TrimEnd();
            return ret;
        }
    }
}
