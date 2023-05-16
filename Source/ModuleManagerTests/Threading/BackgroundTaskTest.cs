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
using Xunit;
using ModuleManager.Threading;

namespace ModuleManagerTests.Threading
{
    public class BackgroundTaskTest
    {
        [Fact]
        public void Test__Start()
        {
            bool finish = false;
            void Run()
            {
                while (!finish) continue;
            }

            ITaskStatus status = BackgroundTask.Start(Run);

            Assert.True(status.IsRunning);
            Assert.False(status.IsFinished);
            Assert.False(status.IsExitedWithError);
            Assert.Null(status.Exception);

            finish = true;

            while (status.IsRunning) continue;

            Assert.False(status.IsRunning);
            Assert.True(status.IsFinished);
            Assert.False(status.IsExitedWithError);
            Assert.Null(status.Exception);
        }

        [Fact]
        public void Test__Start__Exception()
        {
            bool finish = false;
            Exception ex = new Exception();
            void Run()
            {
                while (!finish) continue;
                throw ex;
            }

            ITaskStatus status = BackgroundTask.Start(Run);

            Assert.True(status.IsRunning);
            Assert.False(status.IsFinished);
            Assert.False(status.IsExitedWithError);
            Assert.Null(status.Exception);

            finish = true;

            while (status.IsRunning) continue;

            Assert.False(status.IsRunning);
            Assert.False(status.IsFinished);
            Assert.True(status.IsExitedWithError);
            Assert.Same(ex, status.Exception);
        }

        [Fact]
        public void Test__Run__ArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(delegate
            {
                BackgroundTask.Start(null);
            });
        }
    }
}
