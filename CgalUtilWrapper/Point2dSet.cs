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
        public static bool CreateBoundingCircle(IEnumerable<Point2d> points, out Circle circle)
        {
            circle = Circle.Unset;
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
                        double x = 0, y = 0, r = 0;
                        point2dArray = new Point2dArray(coordinatesPtr, points.Count());
                        Point2dSetCreateBoundingCircle(&point2dArray, ref x, ref y, ref r);
                        circle = new Circle(new Point3d(x, y, 0), r);
                        return true;
                    }
                }
                catch
                {
                    return false;
                }
            }
        }
 
        public static bool CreateBoundingRectangle(IEnumerable<Point2d> points, out Polyline rectangle)
        {
            rectangle = new Polyline();
            Point2dArray point2dArray;
            double[] coordinates = new double[points.Count() * 2];
            for (int i = 0; i < points.Count(); i++)
            {
                coordinates[2 * i + 0] = points.ElementAt(i).X;
                coordinates[2 * i + 1] = points.ElementAt(i).Y;
            }

            Point2dArray poly;

            unsafe
            {
                try
                {
                    fixed (double* coordinatesPtr = coordinates)
                    {
                        point2dArray = new Point2dArray(coordinatesPtr, points.Count());
                        Point2dSetCreateBoundingRectangle(&point2dArray, &poly);
                        for (int i = 0; i < poly._verticesCount; ++i)
                        {
                            rectangle.Add(new Point3d(poly._vertices[2 * i + 0],
                                                      poly._vertices[2 * i + 1], 0));
                        }
                        return true;
                    }
                }
                catch
                {
                    return false;
                }
                finally
                {
                    Point2dArray.Point2dArrayFreeMembers(&poly);
                }
            }
        }

        public static bool CreateConvexHull(IEnumerable<Point2d> points, out Polyline convexHull)
        {
            convexHull = new Polyline();
            Point2dArray point2dArray;
            double[] coordinates = new double[points.Count() * 2];
            for (int i = 0; i < points.Count(); i++)
            {
                coordinates[2 * i + 0] = points.ElementAt(i).X;
                coordinates[2 * i + 1] = points.ElementAt(i).Y;
            }

            Point2dArray poly;

            unsafe
            {
                try
                {
                    fixed (double* coordinatesPtr = coordinates)
                    {
                        point2dArray = new Point2dArray(coordinatesPtr, points.Count());
                        Point2dSetCreateConvexHull(&point2dArray, &poly);
                        for (int i = 0; i < poly._verticesCount; ++i)
                        {
                            convexHull.Add(new Point3d(poly._vertices[2 * i + 0],
                                                       poly._vertices[2 * i + 1], 0));
                        }
                        return true;
                    }
                }
                catch
                {
                    return false;
                }
                finally
                {
                    Point2dArray.Point2dArrayFreeMembers(&poly);
                }
            }
        }

        [DllImport(GeoWrapperBase.DLL, CallingConvention = CallingConvention.Cdecl)]
        extern private unsafe static void Point2dSetCreateBoundingCircle(Point2dArray* points, ref double x, ref double y, ref double r);

        [DllImport(GeoWrapperBase.DLL, CallingConvention = CallingConvention.Cdecl)]
        extern private unsafe static void Point2dSetCreateBoundingRectangle(Point2dArray* points, Point2dArray* rectangle);

        [DllImport(GeoWrapperBase.DLL, CallingConvention = CallingConvention.Cdecl)]
        extern private unsafe static void Point2dSetCreateConvexHull(Point2dArray* points, Point2dArray* convexHull);
    }
}
