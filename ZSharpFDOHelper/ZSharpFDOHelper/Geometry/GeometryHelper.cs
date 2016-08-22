using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows;

namespace ZSharpFDOHelper.Geometry
{
    public class GeometryHelper
    {
        public static double GetDistanceBetweenPoints(double ax, double ay, double bx, double by)
        {
            double a = ax - ay;
            double b = bx - by;
            double distance = Math.Sqrt(a * a + b * b);
            return distance;
        }
    }
}
