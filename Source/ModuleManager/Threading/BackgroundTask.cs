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
using System.Threading;

namespace ModuleManager.Threading
{
    public static class BackgroundTask
    {
        public static ITaskStatus Start(Action action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            TaskStatus status = new TaskStatus();

            void RunAction()
            {
                try
                {
                    action();
                    status.Finished();
                }
                catch (Exception ex)
                {
                    status.Finished(ex);
                }
            }

            Thread thread = new Thread(RunAction);
            thread.Start();

            return new TaskStatusWrapper(status);
        }
    }
}
