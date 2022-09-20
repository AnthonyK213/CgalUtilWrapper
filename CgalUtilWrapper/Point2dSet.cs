using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Rhino.Geometry;

namespace CgalUtilWrapper
{
    public class Point2dSet
    {
        public static bool CreateBoundingCircle(IEnumerable<Point2d> points, out double x, out double y, out double r)
        {
            x = 0;
            y = 0;
            r = 0;

            Point2dArray point2dArray;
            double[] coordinates = new double[points.Count() * 2];
            for (int i = 0; i < points.Count(); i++)
            {
                coordinates[2 * i + 0] = points.ElementAt(i).X;
                coordinates[2 * i + 1] = points.ElementAt(i).Y;
            }

            unsafe
            {
                try
                {
                    fixed (double* coordinatesPtr = coordinates)
                    {
                        double _x = 0, _y = 0, _r = 0;
                        point2dArray = new Point2dArray(coordinatesPtr, points.Count());
                        Point2dSetCreateBoundingCircle(&point2dArray, ref _x, ref _y, ref _r);
                        x = _x;
                        y = _y;
                        r = _r;
                        return true;
                    }
                }
                catch
                {
                    return false;
                }
            }
        }


        [DllImport(GeoWrapperBase.DLL, CallingConvention = CallingConvention.Cdecl)]
        extern private unsafe static void Point2dSetCreateBoundingCircle(Point2dArray* points, ref double x, ref double y, ref double r);
    }
}
