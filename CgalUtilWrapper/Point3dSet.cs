using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Rhino.Geometry;

namespace CgalUtilWrapper
{
    public class Point3dSet : GeoWrapperBase
    {
        private static double[] PointsToDoubleArray(IEnumerable<Point3d> points)
        {
            double[] coordinates = new double[points.Count() * 3];
            for (int i = 0; i < points.Count(); i++)
            {
                coordinates[3 * i + 0] = points.ElementAt(i).X;
                coordinates[3 * i + 1] = points.ElementAt(i).Y;
                coordinates[3 * i + 2] = points.ElementAt(i).Z;
            }

            return coordinates;
        }

        public static bool CreateBoundingCircle(IEnumerable<Point3d> points, out Sphere sphere)
        {
            sphere = Sphere.Unset;
            Point3dArray point3dArray;
            double[] coordinates = PointsToDoubleArray(points);

            unsafe
            {
                try
                {
                    fixed (double* coordinatesPtr = coordinates)
                    {
                        double x = 0, y = 0, z = 0, r = 0;
                        point3dArray = new Point3dArray(coordinatesPtr, points.Count());
                        Point3dSetCreateBoundingSphere(&point3dArray, ref x, ref y, ref z, ref r);
                        sphere = new Sphere(new Point3d(x, y, z), r);
                        return true;
                    }
                }
                catch
                {
                    return false;
                }
            }
        }

        public static bool CreateConvexHull(IEnumerable<Point3d> points, out Mesh hull)
        {
            hull = new Mesh();

            Point3dArray point3dArray, vertices;
            TriMeshFaces faces;
            double[] coordinates = PointsToDoubleArray(points);

            unsafe
            {
                try
                {
                    fixed (double* coordinatesPtr = coordinates)
                    {
                        point3dArray = new Point3dArray(coordinatesPtr, points.Count());
                        Point3dSetCreateConvexHull(&point3dArray, &vertices, &faces);
                        Point3d[] hullPoints = new Point3d[vertices._pointsCount];
                        int[] hullFaces = new int[faces._facesCount];

                        for (int i = 0; i < vertices._pointsCount; ++i)
                        {
                            hull.Vertices.Add(new Point3d(vertices._coordinates[3 * i + 0],
                                                          vertices._coordinates[3 * i + 1],
                                                          vertices._coordinates[3 * i + 2]));
                        }

                        for (int i = 0; i < faces._facesCount; ++i)
                        {
                            hull.Faces.AddFace(new MeshFace(faces._faces[3 * i + 0],
                                                            faces._faces[3 * i + 1],
                                                            faces._faces[3 * i + 2]));
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
                    Point3dArray.Point3dArrayFreeMembers(&vertices);
                    TriMeshFaces.TriMeshFacesFreeMembers(&faces);
                }
            }
        }

        public static bool CreateOptimalBoundingBox(IEnumerable<Point3d> points, out Box box)
        {
            box = Box.Empty;
            Point3d[] corners = new Point3d[8];

            if (!points.Any())
            {
                return false;
            }

            double[] coordinates = PointsToDoubleArray(points);
            Point3dArray point3dArray, outCorners;

            unsafe
            {
                try
                {
                    fixed (double* coordinatesPtr = coordinates)
                    {
                        point3dArray = new Point3dArray(coordinatesPtr, points.Count());
                        Point3dSetCreateOptimalBoundingBox(&point3dArray, &outCorners);
                        for (int i = 0; i < outCorners._pointsCount; ++i)
                        {
                            corners[i] = new Point3d(
                                outCorners._coordinates[3 * i + 0],
                                outCorners._coordinates[3 * i + 1],
                                outCorners._coordinates[3 * i + 2]);
                        }
                        List<Vector3d> axis = new List<Vector3d>
                        {
                            corners[1] - corners[0],
                            corners[3] - corners[0],
                            corners[5] - corners[0]
                        };
                        axis.Sort((a, b) => b.Length.CompareTo(a.Length));
                        Plane plane = new Plane(corners[0], axis[0], axis[1]);
                        box = new Box(plane, corners);
                        return true;
                    }
                }
                catch
                {
                    return false;
                }
                finally
                {
                    Point3dArray.Point3dArrayFreeMembers(&outCorners);
                }
            }
        }

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        extern private unsafe static void Point3dSetCreateConvexHull(Point3dArray* points, Point3dArray* vertices, TriMeshFaces* faces);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        extern private unsafe static void Point3dSetCreateBoundingSphere(Point3dArray* points, ref double x, ref double y, ref double z, ref double r);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        extern private unsafe static void Point3dSetCreateOptimalBoundingBox(Point3dArray* points, Point3dArray* corners);
    }
}
