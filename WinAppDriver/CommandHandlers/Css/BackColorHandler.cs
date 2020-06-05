using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Automation;

namespace WinAppDriver.Server.CommandHandlers.Css
{
    public class BackColorHandler
    {
        public Response GetResponse(AutomationElement automationElement)
        {
            if (automationElement.TryGetCurrentPattern(TextPattern.Pattern, out var pattern))
            {
                if (pattern is TextPattern textPattern)
                {
                    var bgColor = textPattern.DocumentRange.GetAttributeValue(TextPattern.BackgroundColorAttribute);
                    return Response.CreateSuccessResponse(bgColor);
                }
            }

            var rect = automationElement.Current.BoundingRectangle;
            var x = (int)rect.X + 2;
            var y = (int)rect.Y + 2;
            var hWnd = new IntPtr(automationElement.Current.NativeWindowHandle);
            var color = GetPixelColor(hWnd, x, y);
            return Response.CreateSuccessResponse("#" + color.Name);
        }

        [DllImport("gdi32.dll")]
        private static extern int BitBlt(IntPtr srchDc, int srcX, int srcY, int srcW, int srcH, IntPtr desthDc, int destX, int destY, int op);

        private static Color GetPixelColor(IntPtr hwnd, int x, int y)
        {
            using (Bitmap screenPixel = new Bitmap(1, 1))
            {
                using (Graphics gdest = Graphics.FromImage(screenPixel))
                {
                    using (Graphics gsrc = Graphics.FromHwnd(hwnd))
                    {
                        IntPtr hsrcdc = gsrc.GetHdc();
                        IntPtr hdc = gdest.GetHdc();
                        BitBlt(hdc, 0, 0, 1, 1, hsrcdc, x, y, (int)CopyPixelOperation.SourceCopy);
                        gdest.ReleaseHdc();
                        gsrc.ReleaseHdc();
                    }
                }

                return screenPixel.GetPixel(0, 0);
            }
        }
    }
}
