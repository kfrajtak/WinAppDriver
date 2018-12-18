// <copyright file="CommandDispatcher.cs" company="Salesforce.com">
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
using System.Threading;
using System.Windows.Forms;

namespace WinAppDriver.Server
{
    /// <summary>
    /// Dispatches received commands to the correct handlers.
    /// </summary>
    public class CommandDispatcher
    {
        private CommandEnvironment _commandEnvironment;
        private readonly Uri _uri;

        public static ManualResetEvent allDone = new ManualResetEvent(false);

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandDispatcher"/> class.
        /// </summary>
        /// <param name="browser">The <see cref="WebBrowser"/> to use in executing the commands.</param>
        public CommandDispatcher(IntPtr hwnd)
        {
            var builder = new UriBuilder("http://localhost")
            {
                Port = 12345
            };

            _uri = builder.Uri;

            _commandEnvironment = new CommandEnvironment(hwnd);            
        }

        public CommandDispatcher(string uri)
        {
            _uri = new Uri(uri);
            _commandEnvironment = new CommandEnvironment();
        }

        public void Start()
        {
            var t = new Thread(StartListening)
            {
                Name = "WinAppDriver"
            };
            t.Start();
        }

        /// <summary>
        /// Event triggered when the address information of the dispatcher is updated.
        /// </summary>
        public event EventHandler<TextEventArgs> AddressInfoUpdated;

        /// <summary>
        /// Event triggered when data is received over the wire.
        /// </summary>
        public event EventHandler<TextEventArgs> DataReceived;

        /// <summary>
        /// Dispatches a command and returns its response.
        /// </summary>
        /// <param name="serializedCommand">A JSON serialized representation of a <see cref="Command"/> object.</param>
        /// <returns>A JSON value serializing the response for the command.</returns>
        public string DispatchCommand(string serializedCommand)
        {
            Command command = Command.FromJson(serializedCommand);
            Response response = command.Execute(this._commandEnvironment);
            return response.ToJson();
        }

        private Host _host;
        private ManualResetEvent manualResetEvent = new ManualResetEvent(false);

        private void StartListening()
        {
            try
            {
                _host = new Host(_uri, _commandEnvironment);
                _host.Start();
                Console.WriteLine($"Running on {_uri}");
                manualResetEvent.WaitOne();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
            }
        }

        public void Stop()
        {
            manualResetEvent.Set();
        }
    }
}
