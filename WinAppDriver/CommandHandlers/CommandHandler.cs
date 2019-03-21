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

namespace WinAppDriver.Server.CommandHandlers
{
    /// <summary>
    /// Provides the base class for handling for the commands.
    /// </summary>
    internal abstract class CommandHandler
    {
        protected bool _unexpectedAlertCheckRequired = true;

        public bool UnexpectedAlertCheckRequired => _unexpectedAlertCheckRequired;

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="environment">The <see cref="CommandEnvironment"/> to use in executing the command.</param>
        /// <param name="parameters">The <see cref="Dictionary{string, object}"/> containing the command parameters.</param>
        /// <returns>The JSON serialized string representing the command response.</returns>
        public virtual Response Execute(CommandEnvironment environment, Dictionary<string, object> parameters)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Evaluates a JavaScript atom in the <see cref="CommandEnvironment"/>.
        /// </summary>
        /// <param name="environment">The <see cref="CommandEnvironment"/> in which to evaluate the JavaScript atom.</param>
        /// <param name="atom">The JavaScript atom to evaluate.</param>
        /// <param name="args">An array of arguments to the JavaScript atom.</param>
        /// <returns>The string result of the atom execution.</returns>
        [Obsolete]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Catching general exception type is expressly permitted here to allow proper reporting via JSON-serialized result.")]
        protected string EvaluateAtom(CommandEnvironment environment, string atom, params object[] args)
        {
            throw new NotImplementedException();
        }
    }
}
