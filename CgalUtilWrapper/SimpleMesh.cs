using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Rhino.Geometry;
using System.Security.Policy;

namespace CgalUtilWrapper
{
    public class SimpleMesh : GeoWrapperBase
    {
        public SimpleMesh(Mesh mesh)
        {
            Mesh m = mesh.DuplicateMesh();
            m.Vertices.UseDoublePrecisionVertices = true;
            m.Faces.ConvertQuadsToTriangles();
            m.Vertices.CombineIdentical(true, true);
            m.Vertices.CullUnused();
            m.Weld(Math.PI);
            m.FillHoles();
            m.RebuildNormals();

            if (!m.IsValid || !m.IsClosed)
            {
                _handle = IntPtr.Zero;
                _error = "Mesh is not valid.";
                return;
            }

            int _verticesCount = m.Vertices.Count;
            double[] _vertices = new double[_verticesCount * 3];
            int _edgesCount = m.TopologyEdges.Count;
            int[] _edges = new int[_edgesCount * 2];
            int _facesCount = m.Faces.Count;
            int[] _faces = m.Faces.ToIntArray(true);

            for (int i = 0; i < _verticesCount; ++i)
            {
                _vertices[3 * i + 0] = m.Vertices[i].X;
                _vertices[3 * i + 1] = m.Vertices[i].Y;
                _vertices[3 * i + 2] = m.Vertices[i].Z;
            }

            for (int i = 0; i < _edgesCount; ++i)
            {
                var edge = m.TopologyEdges.GetTopologyVertices(i);
                _edges[2 * i + 0] = edge.I;
                _edges[2 * i + 1] = edge.J;
            }

            unsafe
            {
                try
                {
                    fixed (double* vPtr = _vertices)
                    {
                        var vertices = new Point3dArray(vPtr, _verticesCount);
                        fixed (int* ePtr = _edges)
                        {
                            var edges = new MeshEdges(ePtr, _edgesCount);
                            fixed (int* fPtr = _faces)
                            {
                                var faces = new MeshFaces(fPtr, _facesCount);
                                _handle = SimpleMeshNew(&vertices, &edges, &faces);
                            }
                        }
                    }
                }
                catch
                {
                    _handle = IntPtr.Zero;
                    _error = "Unknown error.";
                }
            }
        }

        public bool CreateOptimalBoundingBox(out Box box)
        {
            box = Box.Empty;
            Point3d[] corners = new Point3d[8];

            if (IsDisposed || !IsValid)
            {
                return false;
            }

            Point3dArray outCorners;

            unsafe
            {
                try
                {
                    SimpleMeshCreateOptimalBoundingBox(_handle, &outCorners);
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
                catch
                {
                    return false;
                }
                finally
                {
                    Point3dArrayFreeMembers(&outCorners);
                }
            }
        }

        public bool CreateConvexHull(out Mesh hull)
        {
            hull = new Mesh();

            Point3dArray points;
            MeshFaces faces;

            unsafe
            {
                try
                {
                    SimpleMeshCreateConvexHull(_handle, &points, &faces);
                    Point3d[] hullPoints = new Point3d[points._pointsCount];
                    int[] hullFaces = new int[faces._facesCount];

                    for (int i = 0; i < points._pointsCount; ++i)
                    {
                        hull.Vertices.Add(new Point3d(points._coordinates[3 * i + 0],
                                                      points._coordinates[3 * i + 1],
                                                      points._coordinates[3 * i + 2]));
                    }

                    for (int i = 0; i < faces._facesCount; ++i)
                    {
                        hull.Faces.AddFace(new MeshFace(faces._faces[3 * i + 0],
                                                        faces._faces[3 * i + 1],
                                                        faces._faces[3 * i + 2]));
                    }

                    return true;
                }
                catch
                {
                    return false;
                }
                finally
                {
                    Point3dArrayFreeMembers(&points);
                    MeshFacesFreeMembers(&faces);
                }
            }
        }

        #region Finalize
        ~SimpleMesh() => Gc();

        protected override void DropManaged() => SimpleMeshDrop(_handle);
        #endregion

        #region FFI
        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        extern private unsafe static IntPtr SimpleMeshNew(Point3dArray* vertices, MeshEdges* edges, MeshFaces* faces);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        extern private static unsafe void SimpleMeshCreateOptimalBoundingBox(IntPtr handle, Point3dArray* corners);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        extern private unsafe static IntPtr SimpleMeshCreateConvexHull(IntPtr handle, Point3dArray* vertices, MeshFaces* faces);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        extern private static void SimpleMeshDrop(IntPtr handle);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        extern private static unsafe void Point3dArrayFreeMembers(Point3dArray* handle);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        extern private static unsafe void MeshEdgesFreeMembers(MeshEdges* handle);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        extern private static unsafe void MeshFacesFreeMembers(MeshFaces* handle);

        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct Point3dArray
        {
            public readonly double* _coordinates;
            public readonly int _pointsCount;

            public Point3dArray(double* coordinates, int pointsCount)
            {
                _coordinates = coordinates;
                _pointsCount = pointsCount;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct MeshEdges
        {
            public readonly int* _edges;
            public readonly int _edgesCount;

            public MeshEdges(int* edges, int edgesCount)
            {
                _edges = edges;
                _edgesCount = edgesCount;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct MeshFaces
        {
            public readonly int* _faces;
            public readonly int _facesCount;

            public MeshFaces(int* faces, int facesCount)
            {
                _faces = faces;
                _facesCount = facesCount;
            }
        }
        #endregion
    }
}
