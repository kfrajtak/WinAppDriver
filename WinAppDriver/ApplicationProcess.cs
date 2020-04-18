using System;
using System.Diagnostics;
using System.Threading;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using WinAppDriver.Extensions;

namespace WinAppDriver
{
    public static class ApplicationProcess
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static Process StartProcessFromPath(string path, CancellationToken cancellationToken, string processName = null, string mainWindowTitlePattern = null)
        {
            Logger.Info("Starting process from path " + path);

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = path
            };

            processName = processName?.Trim();

            var process = Process.Start(startInfo);

            Regex regex;
            try
            {
                regex = new Regex(mainWindowTitlePattern);
            }
            catch (ArgumentException e)
            {
                throw new ArgumentException("Regex for MainWindowTitle is not valid", e);
            }

            while (true)
            {
                Thread.Sleep(500);

                if (process == null && (processName != null || regex != null))
                {   
                    Process[] processes;
                    if (string.IsNullOrEmpty(processName))
                    {
                        Logger.Debug($"Process not detected, looking for process with MainWindowTitle matching '{regex}' (process name was not provided).");
                        processes = Process.GetProcesses();
                    }
                    else
                    {
                        Logger.Debug($"Process not detected, looking for process with name='{processName}' and MainWindowTitle matching '{regex}'.");
                        processes = Process.GetProcessesByName(processName);
                    }

                    var matchingProcesses = processes.Where(p => regex.IsMatch(p.MainWindowTitle));

                    if (matchingProcesses.Count() == 1)
                    {
                        if (matchingProcesses.First().MainWindowHandle.ToInt32() == 0)
                        {
                            // wait for window
                            continue;
                        }

                        process = matchingProcesses.First();
                        break;
                    }

                    if (!matchingProcesses.Any())
                    {
                        continue;
                    }

                    throw new Exception("Multiple processes matching criteria found.");
                }

                if (process != null)
                {
                    break;
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
            }

            if (process == null)
            {
                throw new NullReferenceException("Process was not started.");
            }

            return process;
        }

        public static Process AttachToProcess(Dictionary<string, object> desiredCapabilities)
        {
            if (desiredCapabilities.TryGetParameterValue<int>("processId", out var processId))
            {
                return Process.GetProcessById(processId);
            }

            var processName = desiredCapabilities.GetParameterValue<string>("processName");
            var process = Process.GetProcessesByName(processName).FirstOrDefault();

            // searching by name as regular expression pattern
            if (process == null)
            {
                var regex = new Regex(processName);
                return Process.GetProcesses().FirstOrDefault(x => regex.IsMatch(x.ProcessName));
            }

            return process;
        }
    }
}
