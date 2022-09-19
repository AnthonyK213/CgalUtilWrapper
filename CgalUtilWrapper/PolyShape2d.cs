using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Rhino.Geometry;
using Rhino.Geometry.Intersect;
using System.Security.Policy;

namespace CgalUtilWrapper
{
    public class PolyShape2d : GeoWrapperBase
    {
        public PolyShape2d(Polyline outer, IEnumerable<Polyline> inner)
        {
            int outerVerticesCount = 0;
            List<int> innerVerticesCount = new List<int>();
            List<double> vertices = new List<double>();

            if (!outer.IsClosed)
            {
                _error = "Outer polyline is not closed.";
                return;
            }

            var outerBox = outer.BoundingBox;
            var outerCurve = outer.ToNurbsCurve();

            //using (var propOuter = AreaMassProperties.Compute(outer.ToNurbsCurve()))
            //{
            //    if (propOuter == null || propOuter.Area < 1e-2)
            //    {
            //        _error = "Outer polyline has no area.";
            //        return;
            //    }
            //}

            if (!outerCurve.TryGetPlane(out Plane outerPlane, 0.01))
            {
                _error = "Outer polyline is not planar.";
                return;
            }

            int outerPara = Brep.CreatePlanarBreps(outerCurve, 0.01)[0].Faces[0].NormalAt(0.5, 0.5).IsParallelTo(Plane.WorldXY.ZAxis, 0.1);

            if (outerPara == 0 || Math.Abs(outerPlane.OriginZ) > 1e-8)
            {
                _error = "Outer polyline is not in WorldXY plane.";
                return;
            }
            else if (outerPara == 1)
            {
                for (int i = 0; i < outer.Count - 1; ++i)
                {
                    vertices.Add(outer[i].X);
                    vertices.Add(outer[i].Y);
                }
            }
            else
            {
                for (int i = outer.Count - 1; i >= 1; --i)
                {
                    vertices.Add(outer[i].X);
                    vertices.Add(outer[i].Y);
                }
            }

            outerVerticesCount = outer.Count - 1;

            foreach (Polyline poly in inner)
            {
                var innerBox = poly.BoundingBox;
                var innerCurve = poly.ToNurbsCurve();

                if (!poly.IsClosed)
                {
                    _error = "An inner polyline is not closed.";
                    return;
                }

                //using (var propInner = AreaMassProperties.Compute(poly.ToNurbsCurve()))
                //{
                //    if (propInner == null || propInner.Area < 10)
                //    {
                //        _error = "An inner polyline has no area.";
                //        return;
                //    }
                //}

                if (!innerCurve.TryGetPlane(out Plane innerPlane, 0.01))
                {
                    _error = "An inner polyline is not planar.";
                    return;
                }

                if (Intersection.CurveCurve(outerCurve, innerCurve, 0.1, 0.1).Any()
                    || !outerBox.Contains(innerBox))
                {
                    _handle = IntPtr.Zero;
                    _error = "Invalid position between outer polyline and inner polylines.";
                    return;
                }

                int innerPara = Brep.CreatePlanarBreps(innerCurve, 0.01)[0].Faces[0].NormalAt(0.5, 0.5).IsParallelTo(Plane.WorldXY.ZAxis, 0.1);

                if (innerPara == 0 || Math.Abs(innerPlane.OriginZ) > 1e-8)
                {
                    _error = "An inner polyline is not in WorldXY plane.";
                    return;
                }
                else if (innerPara == 1)
                {
                    for (int i = poly.Count - 1; i >= 1; --i)
                    {
                        vertices.Add(poly[i].X);
                        vertices.Add(poly[i].Y);
                    }
                }
                else
                {
                    for (int i = 0; i < poly.Count - 1; ++i)
                    {
                        vertices.Add(poly[i].X);
                        vertices.Add(poly[i].Y);
                    }
                }

                innerVerticesCount.Add(poly.Count - 1);
            }

            for (int i = 0; i < inner.Count(); ++i)
            {
                for (int j = i + 1; j < inner.Count(); ++j)
                {
                    (Curve _a, Curve _b) = (inner.ElementAt(i).ToNurbsCurve(), inner.ElementAt(j).ToNurbsCurve());
                    var _abb = inner.ElementAt(i).BoundingBox;
                    var _bbb = inner.ElementAt(j).BoundingBox;

                    if (Intersection.CurveCurve(_a, _b, 0.1, 0.1).Any()
                        || _abb.Contains(_bbb)
                        || _bbb.Contains(_abb))
                    {
                        _handle = IntPtr.Zero;
                        _error = "Invalid position among inner polylines.";
                        return;
                    }
                }
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
                    _error = "Unknown error.";
                }
            }
        }

        public bool GenerateStraightSkeleton(out List<Line> straightSkeleton, out List<Line> spokes)
        {
            straightSkeleton = new List<Line>();
            spokes = new List<Line>();

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
                            new Point3d(outStraightSkeleton._vertices[2 * i + 2], outStraightSkeleton._vertices[2 * i + 3], 0)));
                    }

                    for (int i = 0; i < outSpokes._verticesCount; i += 2)
                    {
                        spokes.Add(new Line(
                            new Point3d(outSpokes._vertices[2 * i + 0], outSpokes._vertices[2 * i + 1], 0),
                            new Point3d(outSpokes._vertices[2 * i + 2], outSpokes._vertices[2 * i + 3], 0)));
                    }

                    return true;
                }
                catch
                {
                    return false;
                }
                finally
                {
                    Poly2dFreeMembers(&outStraightSkeleton);
                    Poly2dFreeMembers(&outSpokes);
                }
            }
        }

        #region Finalize
        ~PolyShape2d() => Gc();

        protected override void DropUnmanaged() => PolyShape2dDrop(_handle);
        #endregion

        #region FFI
        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        extern private static unsafe IntPtr PolyShape2dNew(Poly2d* outer, Poly2d* holes, int holesCount);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        extern private static unsafe int PolyShape2dGenerateStraightSkeleton(IntPtr handle, Poly2d* outStraightSkeleton, Poly2d* outSpokes);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        extern private static int PolyShape2dGenerateOffsetPolygon();

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        extern private static void PolyShape2dDrop(IntPtr handle);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        extern private static unsafe void Poly2dFreeMembers(Poly2d* handle);

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
