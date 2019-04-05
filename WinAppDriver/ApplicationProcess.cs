using System;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Threading.Tasks;

namespace WinAppDriver
{
    class ApplicationProcess
    {
        public static Process StartProcessFromPath(string path, int timeout = 3000)
        {

            var process = Process.Start(path);
            process.WaitForInputIdle(timeout);
            process.Refresh();
            if (process.MainWindowHandle.ToInt32() == 0)
            {
                int time = 0;
                while (!process.HasExited)
                {
                    process.Refresh();
                    if (process.MainWindowHandle.ToInt32() != 0)
                    {
                        return process;
                    }
                    Thread.Sleep(50);
                    time += 50;
                    if (time > timeout)
                    {
                        throw new TimeoutException("Process starting timeout expired.");
                    }
                }
            }

            return process;
        }
    }
}
