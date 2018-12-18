// <copyright file="CommandHandler.cs" company="Salesforce.com">
//
// Copyright (c) 2014 Salesforce.com, Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the
// following conditions are met:
//
//    Redistributions of source code must retain the above copyright notice, this list of conditions and the following
//    disclaimer.
//
//    Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the
//    following disclaimer in the documentation and/or other materials provided with the distribution.
//
//    Neither the name of Salesforce.com nor the names of its contributors may be used to endorse or promote products
//    derived from this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
// WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE
// USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace WinAppDriver.Server.CommandHandlers
{
    /// <summary>
    /// Provides the base class for handling for the commands.
    /// </summary>
    internal abstract class CommandHandler
    {
        private TimeSpan atomExecutionTimeout = TimeSpan.FromMilliseconds(-1);

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="environment">The <see cref="CommandEnvironment"/> to use in executing the command.</param>
        /// <param name="parameters">The <see cref="Dictionary{string, object}"/> containing the command parameters.</param>
        /// <returns>The JSON serialized string representing the command response.</returns>
        public abstract Response Execute(CommandEnvironment environment, Dictionary<string, object> parameters);

        /// <summary>
        /// Creates the argument string for a JavaScript execution.
        /// </summary>
        /// <param name="args">The arguments for which to create the string.</param>
        /// <returns>The string representing the arguments.</returns>
        protected static string CreateArgumentString(object[] args)
        {
            StringBuilder builder = new StringBuilder();
            foreach (object arg in args)
            {
                if (builder.Length > 0)
                {
                    builder.Append(",");
                }

                string argAsString = JsonConvert.SerializeObject(arg);
                builder.Append(argAsString);
            }

            return builder.ToString();
        }

        /// <summary>
        /// Evaluates a JavaScript atom in the <see cref="CommandEnvironment"/>.
        /// </summary>
        /// <param name="environment">The <see cref="CommandEnvironment"/> in which to evaluate the JavaScript atom.</param>
        /// <param name="atom">The JavaScript atom to evaluate.</param>
        /// <param name="args">An array of arguments to the JavaScript atom.</param>
        /// <returns>The string result of the atom execution.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Catching general exception type is expressly permitted here to allow proper reporting via JSON-serialized result.")]
        protected string EvaluateAtom(CommandEnvironment environment, string atom, params object[] args)
        {
            //WebBrowser browser = environment.Browser;
            string argumentString = CreateArgumentString(args);
            string script = "window.top.__wd_fn_result = (" + atom + ")(" + argumentString + ");";
            string result = string.Empty;
            ManualResetEvent synchronizer = new ManualResetEvent(false);
            /*browser.Dispatcher.BeginInvoke(() =>
                {
                    try
                    {
                        browser.InvokeScript("eval", script);
                        result = browser.InvokeScript("eval", "window.top.__wd_fn_result").ToString();
                    }
                    catch (Exception ex)
                    {
                        result = string.Format(CultureInfo.InvariantCulture, "{{ \"status\": {2}, \"value\": {{ \"message\": \"Unexpected exception ({0}) - '{1}'\" }} }}", ex.GetType().ToString(), ex.Message, WebDriverStatusCode.UnhandledError);
                    }
                    finally
                    {
                        synchronizer.Set();
                    }
                });

            synchronizer.WaitOne(this.atomExecutionTimeout);*/

            return result;
        }

        /// <summary>
        /// Sets the timeout for execution of an atom.
        /// </summary>
        /// <param name="timeout">The <see cref="TimeSpan"/> representing the timeout for the atom execution.</param>
        protected void SetAtomExecutionTimeout(TimeSpan timeout)
        {
             this.atomExecutionTimeout = timeout;
        }
    }
}
