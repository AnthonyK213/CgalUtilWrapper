using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Rhino.Geometry;
using Rhino.Geometry.Intersect;

namespace CgalUtilWrapper
{
    public class PolyShape2d : IDisposable
    {
        private readonly IntPtr _handle = IntPtr.Zero;
        public bool IsDisposed { get; private set; } = false;
        public bool IsValid => _handle != IntPtr.Zero;

        public PolyShape2d(Curve outer, IEnumerable<Curve> inner)
        {
            int outerVerticesCount = 0;
            List<int> innerVerticesCount = new List<int>();
            List<double> vertices = new List<double>();

            foreach (Curve segment in outer.DuplicateSegments())
            {
                vertices.Add(segment.PointAtStart.X);
                vertices.Add(segment.PointAtStart.Y);
                outerVerticesCount++;
            }

            var outerBox = outer.GetBoundingBox(false);

            foreach (Curve curve in inner)
            {
                int _count = 0;

                if (Intersection.CurveCurve(outer, curve, 0.1, 0.1).Any()
                    || !outerBox.Contains(curve.GetBoundingBox(false), false))
                {
                    _handle = IntPtr.Zero;
                    return;
                }

                foreach (Curve segments in curve.DuplicateSegments())
                {
                    vertices.Add(segments.PointAtStart.X);
                    vertices.Add(segments.PointAtStart.Y);
                    _count++;
                }
                innerVerticesCount.Add(_count);
            }

            double[] vArray = vertices.ToArray();

            unsafe
            {
                try
                {
                    fixed (double* vPtr = vArray)
                    {
                        var outerPoly = new Poly2d(vPtr, outerVerticesCount);
                        var holePolys = new Poly2d[inner.Count()];
                        int holeStartIndex = outerVerticesCount << 1;

                        for (int i = 0; i < inner.Count(); ++i)
                        {
                            holePolys[i] = new Poly2d(&vPtr[holeStartIndex], innerVerticesCount[i]);
                            holeStartIndex += innerVerticesCount[i] << 1;
                        }

                        if (holePolys.Any())
                        {
                            fixed (Poly2d* holesPtr = holePolys)
                            {
                                _handle = PolyShape2dNew(&outerPoly, holesPtr, inner.Count());
                            }
                        }
                        else
                        {
                            _handle = PolyShape2dNew(&outerPoly, null, 0);
                        }
                    }
                }
                catch
                {
                    _handle = IntPtr.Zero;
                }
            }
        }

        public bool GenerateStraightSkeleton(out List<Curve> straightSkeleton)
        {
            straightSkeleton = new List<Curve>();

            if (IsDisposed || !IsValid)
            {
                return false;
            }

            Poly2d outStraightSkeleton, outSpokes;

            unsafe
            {
                try
                {
                    int code = PolyShape2dGenerateStraightSkeleton(_handle, &outStraightSkeleton, &outSpokes);

                    if (code != 0)
                    {
                        return false;
                    }

                    for (int i = 0; i < outStraightSkeleton._verticesCount; i += 2)
                    {
                        straightSkeleton.Add(new Line(
                            new Point3d(outStraightSkeleton._vertices[2 * i + 0], outStraightSkeleton._vertices[2 * i + 1], 0),
                            new Point3d(outStraightSkeleton._vertices[2 * i + 2], outStraightSkeleton._vertices[2 * i + 3], 0)).ToNurbsCurve());
                    }

                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        #region Release
        ~PolyShape2d()
        {
            Dispose(false);
            GC.SuppressFinalize(this);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected void Dispose(bool disposing)
        {
            if (IsDisposed) return;

            if (disposing)
            {

            }

            PolyShape2dDrop(_handle);

            IsDisposed = true;
        }
        #endregion

        #region FFI
        private const string DLL = "CgalUtil.dll";

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        extern private static unsafe IntPtr PolyShape2dNew(Poly2d* outer, Poly2d* holes, int holesCount);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        extern private static unsafe int PolyShape2dGenerateStraightSkeleton(IntPtr handle, Poly2d* outStraightSkeleton, Poly2d* outSpokes);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        extern private static int PolyShape2dGenerateOffsetPolygon();

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        extern private static int PolyShape2dDrop(IntPtr handle);

        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct Poly2d
        {
            public readonly double* _vertices;
            public readonly int _verticesCount;

            public Poly2d(double* vertices, int verticesCount)
            {
                _vertices = vertices;
                _verticesCount = verticesCount;
            }
        }
        #endregion
    }
}
