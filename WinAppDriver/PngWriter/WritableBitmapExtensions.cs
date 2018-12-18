// <copyright file="WritableBitmapExtensions.cs" company="Salesforce.com">
//
// Copyright 2014 Salesforce.com
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace WindowsPhoneDriverBrowser
{
    /// <summary>
    /// Contains extension methods for the <see cref="WritableBitmap"/> class.
    /// </summary>
    public static class WritableBitmapExtensions
    {
        /// <summary>
        /// Saves a <see cref="WritableBitmap"/> to a stream as a PNG file.
        /// </summary>
        /// <param name="bitmap">The <see cref="WritableBitmap"/> to save.</param>
        /// <param name="stream">The <see cref="Stream"/> to which to save the PNG-encoded bits.</param>
        public static void SavePng(this WriteableBitmap bitmap, Stream stream)
        {
            ToolStackPNGWriterLib.PNGWriter.WritePNG(bitmap, stream);
        }
    }
}
