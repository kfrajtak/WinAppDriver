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
    public static class ApplicationProcess
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static Process StartProcessFromPath(string path, string processName = null, string mainWindowTitle = null, int timeout = 3000)
        {
            if (!File.Exists(path))
            {
                path = @"shell:appsFolder\" + path;
            }

            Logger.Info("Starting process from path " + path);

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = path
            };

            var process = Process.Start(startInfo);

            Thread.Sleep(1000);

            if (process == null && (processName != null || mainWindowTitle != null) && path.StartsWith("shell:"))
            {
                Logger.Debug($"Process not detected, looking for process with name='{processName}' and/or MainWindowTitle='{mainWindowTitle}'.");
                var processes = Process.GetProcessesByName(processName);
                if (processes.Length != 0)
                {
                    if (processes.Length == 1)
                    {
                        process = processes[0];
                    }
                    else
                    {
                        process = processes.FirstOrDefault(p => p.MainWindowTitle == mainWindowTitle);
                    }
                }
            }

            if (process == null)
            {
                throw new NullReferenceException("Process was not started.");
            }

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
