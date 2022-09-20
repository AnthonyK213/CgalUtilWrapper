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
        public static bool CreateConvexHull(IEnumerable<Point3d> points, out Mesh hull)
        {
            hull = new Mesh();

            Point3dArray point3dArray, vertices;
            TriMeshFaces faces;
            double[] coordinates = new double[points.Count() * 3];

            for (int i = 0; i < points.Count(); ++i)
            {
                coordinates[3 * i + 0] = points.ElementAt(i).X;
                coordinates[3 * i + 1] = points.ElementAt(i).Y;
                coordinates[3 * i + 2] = points.ElementAt(i).Z;
            }

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

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        extern private unsafe static void Point3dSetCreateConvexHull(Point3dArray* points, Point3dArray* vertices, TriMeshFaces* faces);
    }
}
