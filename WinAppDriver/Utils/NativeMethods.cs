// Capturing screenshots using C# and p/invoke
// http://www.cyotek.com/blog/capturing-screenshots-using-csharp-and-p-invoke
// Copyright © 2017 Cyotek Ltd. All Rights Reserved.

// This work is licensed under the Creative Commons Attribution 4.0 International License.
// To view a copy of this license, visit http://creativecommons.org/licenses/by/4.0/.

using System;
using System.Runtime.InteropServices;

// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable InconsistentNaming

namespace Cyotek.Demo.SimpleScreenshotCapture
{
    internal static class NativeMethods
    {
        #region Externals

        [DllImport("gdi32.dll")]
        public static extern bool BitBlt(IntPtr hdcDest, int nxDest, int nyDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, RasterOperations dwRop);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        public static extern IntPtr DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        public static extern IntPtr DeleteObject(IntPtr hObject);

        [DllImport("dwmapi.dll")]
        public static extern int DwmGetWindowAttribute(IntPtr hwnd, int dwAttribute, out RECT pvAttribute, int cbAttribute);

        [DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [DllImport("user32.dll")]
        public static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDc);

        [DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hObject);

        #endregion

        #region RasterOperations enum

        [Flags]
        public enum RasterOperations
        {
            SRCCOPY = 0x00CC0020,

            SRCPAINT = 0x00EE0086,

            SRCAND = 0x008800C6,

            SRCINVERT = 0x00660046,

            SRCERASE = 0x00440328,

            NOTSRCCOPY = 0x00330008,

            NOTSRCERASE = 0x001100A6,

            MERGECOPY = 0x00C000CA,

            MERGEPAINT = 0x00BB0226,

            PATCOPY = 0x00F00021,

            PATPAINT = 0x00FB0A09,

            PATINVERT = 0x005A0049,

            DSTINVERT = 0x00550009,

            BLACKNESS = 0x00000042,

            WHITENESS = 0x00FF0062,

            CAPTUREBLT = 0x40000000 //only if WinVer >= 5.0.0 (see wingdi.h)
        }

        #endregion

        #region Constants

        public const int DWMWA_EXTENDED_FRAME_BOUNDS = 9;

        #endregion

        #region Nested type: RECT

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;

            public int top;

            public int right;

            public int bottom;
        }

        #endregion
    }
}
