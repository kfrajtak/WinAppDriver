// <copyright file="Command.cs" company="Salesforce.com">
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
using System.Threading.Tasks;
using Newtonsoft.Json;
using WinAppDriver.Server.CommandHandlers;

namespace WinAppDriver.Server
{
    /// <summary>
    /// Represents a command to be executed.
    /// </summary>
    internal class Command
    {
        private string sessionId = string.Empty;
        private string commandName = string.Empty;
        private Dictionary<string, object> parameters;

        /// <summary>
        /// Gets or sets the session ID of the command.
        /// </summary>
        [JsonProperty("sessionId", NullValueHandling = NullValueHandling.Ignore)]
        public string SessionId
        {
            get { return this.sessionId; }
            set { this.sessionId = value; }
        }

        public CommandHandler CommandHandler { get; set; }

        /// <summary>
        /// Gets or sets the name of the command.
        /// </summary>
        [JsonProperty("name")]
        public string CommandName
        {
            get { return this.commandName; }
            set { this.commandName = value; }
        }

        /// <summary>
        /// Gets or sets the parameters of the command.
        /// </summary>
        [JsonProperty("parameters")]
        [JsonConverter(typeof(ProtocolValueJsonConverter))]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "For proper JSON serialization, this should be a Dictionary.")]
        public Dictionary<string, object> Parameters
        {
            get { return this.parameters; }
            set { this.parameters = value; }
        }

        /// <summary>
        /// Creates a <see cref="Command"/> object from a serialized JSON string.
        /// </summary>
        /// <param name="serializedCommand">The JSON-serialized command.</param>
        /// <returns>The Command object.</returns>
        public static Command FromJson(string serializedCommand)
        {
            Command command = JsonConvert.DeserializeObject<Command>(serializedCommand);
            command.CommandHandler = CommandHandlerFactory.Instance.GetHandler(command.CommandName);
            return command;
        }

        /// <summary>
        /// Executes this command.
        /// </summary>
        /// <param name="environment">The <see cref="CommandEnvironment"/> used as context for the command.</param>
        /// <returns>The <see cref="Response"/> from the command execution.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Catching general exception type is expressly permitted here to allow proper reporting via JSON-serialized result.")]
        public Response Execute(CommandEnvironment environment)
        {
            Response commandResponse = null;
            try
            {
                commandResponse = this.CommandHandler.Execute(environment, this.parameters);
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format(CultureInfo.InvariantCulture, "Unexpected exception for command {0} [{1}]: {2}", this.commandName, ex.GetType().ToString(), ex.Message);
                commandResponse = Response.CreateErrorResponse(WebDriverStatusCode.UnhandledError, errorMessage);
            }

            return commandResponse;
        }
    }
}
