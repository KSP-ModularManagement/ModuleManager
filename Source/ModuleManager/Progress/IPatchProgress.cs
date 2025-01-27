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

namespace ModuleManager.Progress
{
    public interface IPatchProgress
    {
        float ProgressFraction { get; }

        EventVoid OnPatchApplied { get; }
        EventData<IPass> OnPassStarted { get; }

        void Warning(UrlDir.UrlConfig url, string message, params object[] @params);
        void Error(UrlDir.UrlConfig url, string message, params object[] @params);
        void Error(string message, params object[] @params);
        void Exception(Exception exception, string message, params object[] @params);
        void Exception(Exception exception, UrlDir.UrlConfig url, string message, params object[] @params);
        void ProcessingTagList(Tags.ITagList tagList, UrlDir.UrlConfig urlConfig);
        void NeedsUnsatisfiedRoot(UrlDir.UrlConfig url);
        void NeedsUnsatisfiedNode(UrlDir.UrlConfig url, string path);
        void NeedsUnsatisfiedValue(UrlDir.UrlConfig url, string path);
        void NeedsUnsatisfiedBefore(UrlDir.UrlConfig url);
        void NeedsUnsatisfiedFor(UrlDir.UrlConfig url);
        void NeedsUnsatisfiedAfter(UrlDir.UrlConfig url);
        void ApplyingCopy(IUrlConfigIdentifier original, UrlDir.UrlConfig patch);
        void ApplyingDelete(IUrlConfigIdentifier original, UrlDir.UrlConfig patch);
        void ApplyingUpdate(IUrlConfigIdentifier original, UrlDir.UrlConfig patch);
        void PatchAdded();
        void PatchApplied();
        void PassStarted(IPass pass);
    }
}
