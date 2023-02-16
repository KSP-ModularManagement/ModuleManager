/*
	This file is part of Module Manager /L
		© 2018-2023 LisiasT
		© 2013-2018 Sarbian using System; Blowfish
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

namespace ModuleManager.Threading
{
    public class TaskStatus : ITaskStatus
    {
        private readonly object lockObject = new object();

        public bool IsRunning { get; private set; } = true;
        public Exception Exception { get; private set; } = null;

        public bool IsFinished
        {
            get
            {
                lock (lockObject)
                {
                    return !IsRunning && Exception == null;
                }
            }
        }

        public bool IsExitedWithError
        {
            get
            {
                lock (lockObject)
                {
                    return !IsRunning && Exception != null;
                }
            }
        }

        public void Finished()
        {
            lock (lockObject)
            {
                if (!IsRunning) throw new InvalidOperationException("Task is not running");
                IsRunning = false;
            }
        }

        public void Finished(Exception exception)
        {
            lock(lockObject)
            {
                if (!IsRunning) throw new InvalidOperationException("Task is not running");
                this.Exception = exception ?? throw new ArgumentNullException(nameof(exception));
                IsRunning = false;
            }
        }
    }
}
