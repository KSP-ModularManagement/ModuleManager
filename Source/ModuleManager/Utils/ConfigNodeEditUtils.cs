/*
	This file is part of Module Manager /L
		© 2018-2023 LisiasT

	Module Manager /L is licensed as follows:
		* GPL 3.0 : https://www.gnu.org/licenses/gpl-3.0.txt

	Module Manager /L is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.

	You should have received a copy of the GNU General Public License 3.0
	along with Module Manager /L. If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using ModuleManager.Collections;

namespace ModuleManager.Utils
{
	public class ConfigNodeEditUtils
	{
		private static ConfigNodeEditUtils _instance = null;
		public static ConfigNodeEditUtils Instance = _instance??(_instance = new ConfigNodeEditUtils());

		private readonly KeyValueCache<string, Regex> regexCache = new KeyValueCache<string, Regex>();

		private ConfigNodeEditUtils() { }

		public void Destroy()
		{
			_instance = null;
		}

		public string FindAndReplaceValue(
			ConfigNode mod,
			ref string valName,
			string value,
			ConfigNode newNode,
			Operator op,
			int index,
			out ConfigNode.Value origVal,
			PatchContext context,
			bool hasPosIndex = false,
			int posIndex = 0,
			bool hasPosStar = false,
			char seperator = ','
		) {
			origVal = FindValueIn(newNode, valName, index);
			if (origVal == null)
				return null;
			string oValue = origVal.value;

			string[] strArray = new string[] { oValue };
			if (hasPosIndex)
			{
				strArray = oValue.Split(new char[] { seperator }, StringSplitOptions.RemoveEmptyEntries);
				if (posIndex >= strArray.Length)
				{
					context.progress.Error(context.patchUrl, "Invalid Vector Index!");
					return null;
				}
			}
			string backupValue = value;
			while (posIndex < strArray.Length)
			{
				value = backupValue;
				oValue = strArray[posIndex];
				if (op != Operator.Assign)
				{
					if (op == Operator.RegexReplace)
					{
						try
						{
							string[] split = value.Split(value[0]);

							Regex replace = this.Fetch(split[1], delegate
							{
								return new Regex(split[1]);
							});

							value = replace.Replace(oValue, split[2]);
						}
						catch (Exception ex)
						{
							context.progress.Exception(ex, context.patchUrl,
								"Error - Failed to do a regexp replacement: {0} : original value=\"{1}\" regexp=\"{2}\" \nNote - to use regexp, the first char is used to subdivide the string (much like sed)",
								mod.name, oValue, value
								);
							return null;
						}
					}
					else if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out double s)
							 && double.TryParse(oValue, NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out double os))
					{
						switch (op)
						{
							case Operator.Multiply:
								value = (os * s).ToString(CultureInfo.InvariantCulture);
								break;

							case Operator.Divide:
								value = (os / s).ToString(CultureInfo.InvariantCulture);
								break;

							case Operator.Add:
								value = (os + s).ToString(CultureInfo.InvariantCulture);
								break;

							case Operator.Subtract:
								value = (os - s).ToString(CultureInfo.InvariantCulture);
								break;

							case Operator.Exponentiate:
								value = Math.Pow(os, s).ToString(CultureInfo.InvariantCulture);
								break;
						}
					}
					else
					{
						context.progress.Error(context.patchUrl,
							"Error - Failed to do a maths replacement: {0} : original value=\"{1}\" operator={3} mod value=\"{4}\"",
							mod.name, oValue, op, value
							);
						return null;
					}
				}
				strArray[posIndex] = value;
				if (hasPosStar) posIndex++;
				else break;
			}
			value = String.Join(new string(seperator, 1), strArray);
			return value;
		}

		public ConfigNode.Value FindValueIn(ConfigNode newNode, string valName, int index)
		{
			ConfigNode.Value v = null;
			for (int i = 0; i < newNode.values.Count; ++i)
			{
				if (WildcardMatch(newNode.values[i].name, valName))
				{
					v = newNode.values[i];
					if (--index < 0)
						return v;
				}
			}
			return v;
		}

		public bool WildcardMatch(string s, string wildcard)
		{
			if (wildcard == null)
				return true;
			string pattern = "^" + Regex.Escape(wildcard).Replace(@"\*", ".*").Replace(@"\?", ".") + "$";

			Regex regex = ConfigNodeEditUtils.Instance.Fetch(pattern, delegate
			{
				return new Regex(pattern);
			});
			return regex.IsMatch(s);
		}

		private Regex Fetch(string key, Func<Regex> createValue) => this.regexCache.Fetch(key, createValue);
	}
}
