using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Rhino.Geometry;
using Rhino.Geometry.Intersect;

namespace CgalUtilWrapper
{
  public class PolyShape2d : GeoWrapperBase
  {
    public PolyShape2d(Polyline outer, IEnumerable<Polyline> inner) : base(true)
    {
      int outerVerticesCount;
      List<int> innerVerticesCount = new List<int>();
      List<double> vertices = new List<double>();

      if (!outer.IsClosed)
        throw new Exception("Outer polyline is not closed.");

      var outerCurve = outer.ToNurbsCurve();

      if (Intersection.CurveSelf(outerCurve, 0.01).Any())
        throw new Exception("Outer polyline has self intersection.");

      if (!outerCurve.TryGetPlane(out Plane outerPlane, 0.01))
        throw new Exception("Outer polyline is not planar.");

      int outerPara = Brep.CreatePlanarBreps(outerCurve, 0.01)[0].Faces[0].NormalAt(0.5, 0.5).IsParallelTo(Plane.WorldXY.ZAxis, 0.1);

      if (outerPara == 0 || Math.Abs(outerPlane.OriginZ) > 1e-8)
        throw new Exception("Outer polyline is not in WorldXY plane.");
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
        if (!poly.IsClosed)
          throw new Exception("An inner polyline is not closed.");

        var innerCurve = poly.ToNurbsCurve();

        if (Intersection.CurveSelf(innerCurve, 0.01).Any())
          throw new Exception("An inner polyline has self intersection.");

        if (!innerCurve.TryGetPlane(out Plane innerPlane, 0.01))
          throw new Exception("An inner polyline is not planar.");

        if (outer.Contains(poly, Plane.WorldXY) != CurveContainment.Inside)
          throw new Exception("An inner polyline is not inside the outer polyline.");

        int innerPara = Brep.CreatePlanarBreps(innerCurve, 0.01)[0].Faces[0].NormalAt(0.5, 0.5).IsParallelTo(Plane.WorldXY.ZAxis, 0.1);

        if (innerPara == 0 || Math.Abs(innerPlane.OriginZ) > 1e-8)
          throw new Exception("An inner polyline is not in WorldXY plane.");
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
          var _apl = inner.ElementAt(i);
          var _bpl = inner.ElementAt(j);
          if (_apl.Contains(_bpl, Plane.WorldXY) != CurveContainment.Outside)
            throw new Exception("Invalid position among inner polylines.");
        }
      }

      double[] vArray = vertices.ToArray();

      unsafe
      {
        try
        {
          fixed (double* vPtr = vArray)
          {
            var outerPoly = new Point2dArray(vPtr, outerVerticesCount);
            var holePolys = new Point2dArray[inner.Count()];
            int holeStartIndex = outerVerticesCount << 1;

            for (int i = 0; i < inner.Count(); ++i)
            {
              holePolys[i] = new Point2dArray(&vPtr[holeStartIndex], innerVerticesCount[i]);
              holeStartIndex += innerVerticesCount[i] << 1;
            }

            if (holePolys.Any())
            {
              fixed (Point2dArray* holesPtr = holePolys)
              {
                handle = PolyShape2dNew(&outerPoly, holesPtr, inner.Count());
              }
            }
            else
            {
              handle = PolyShape2dNew(&outerPoly, null, 0);
            }
          }
        }
        catch
        {
          return;
        }
      }
    }

    public bool GenerateStraightSkeleton(out List<Line> straightSkeleton, out List<Line> spokes)
    {
      straightSkeleton = new List<Line>();
      spokes = new List<Line>();

      if (IsClosed)
      {
        return false;
      }

      Point2dArray outStraightSkeleton, outSpokes;

      unsafe
      {
        try
        {
          int code = PolyShape2dGenerateStraightSkeleton(handle, &outStraightSkeleton, &outSpokes);

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
          Point2dArray.Point2dArrayFreeMembers(&outStraightSkeleton);
          Point2dArray.Point2dArrayFreeMembers(&outSpokes);
        }
      }
    }

    protected override bool ReleaseHandle()
    {
      PolyShape2dDrop(handle);
      return true;
    }

    #region FFI
    [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
    extern private static unsafe IntPtr PolyShape2dNew(Point2dArray* outer, Point2dArray* holes, int holesCount);

    [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
    extern private static unsafe int PolyShape2dGenerateStraightSkeleton(IntPtr handle, Point2dArray* outStraightSkeleton, Point2dArray* outSpokes);

    [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
    extern private static int PolyShape2dGenerateOffsetPolygon();

    [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
    extern private static void PolyShape2dDrop(IntPtr handle);
    #endregion
  }
}
