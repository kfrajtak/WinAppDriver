using System.Collections.Generic;
using System.Windows;

namespace WinAppDriver.Utils
{
    public class PointComparer : IComparer<Point>
    {
        public int Compare(Point a, Point b)
        {
            if (a.Y < b.Y)
            {
                return -1;
            }

            if (a.Y > b.Y)
            {
                return 1;
            }

            if (a.X == b.X)
            {
                return 0;
            }

            return a.X < b.X ? -1 : 1;
        }
    }
}
