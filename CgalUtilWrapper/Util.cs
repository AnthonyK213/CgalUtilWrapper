using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Rhino;
using Rhino.Geometry;
using Rhino.Geometry.Intersect;

namespace CgalUtilWrapper
{
    public enum CurveContainment
    {
        Unset,
        Inside,
        Outside,
        Contains,
        Intersect,
    }

    public static class Util
    {
        private const double TOL_LENGTH = 0.001;
        private const double TOL_ANGLE = 0.1;
        private const double TOL_OVERLAP = 0.001;

        /// <summary>
        /// 按照连续性炸开曲线
        /// </summary>
        /// <param name="curve">被炸的</param>
        /// <param name="continuity">怎么炸</param>
        /// <param name="tolerance">神ノ恩惠</param>
        /// <returns>碎尸</returns>
        public static Curve[] Explode(this Curve curve,
                                      Continuity continuity = Continuity.G2_continuous,
                                      double tolerance = -1)
        {
            if (tolerance < 0) continuity = Continuity.G2_continuous;
            List<Curve> curveList = new List<Curve>();
            double t0 = curve.Domain.Min;
            double t1 = curve.Domain.Max;
            double prevT = t0;
            double splitT = prevT;
            while (curve.GetNextDiscontinuity(continuity, prevT, t1, out double t))
            {
                if (tolerance > 0.0
                    && curve.DerivativeAt(t, 1, CurveEvaluationSide.Below)[1]
                            .IsParallelTo(curve.DerivativeAt(t, 1, CurveEvaluationSide.Above)[1],
                                          RhinoMath.ToRadians(tolerance)) == 1)
                {
                    prevT = t;
                }
                else
                {
                    Curve subCurve = curve.Trim(splitT, t);
                    if (subCurve != null)
                    {
                        curveList.Add(subCurve);
                    }
                    prevT = t;
                    splitT = prevT;
                }
            }
            if (splitT != t1)
            {
                Curve subCurve = curve.Trim(splitT, t1);
                if (subCurve != null)
                {
                    curveList.Add(subCurve);
                }
            }
            if (curve.IsClosed && curveList.Count > 1 && continuity != Continuity.G2_continuous)
            {
                if (tolerance > 0
                    && curveList.First()
                                .TangentAtStart
                                .IsParallelTo(curveList.Last().TangentAtEnd,
                                              RhinoMath.ToRadians(tolerance)) == 1)
                {
                    curveList[curveList.Count - 1] = JoinClosedSegmentsInOrder(new Curve[] { curveList.Last(), curveList.First() });
                    curveList.RemoveAt(0);
                }
            }
            return curveList.ToArray();
        }

        /// <summary>
        /// 将给定曲线按顺序连接
        /// </summary>
        /// <param name="curves"></param>
        /// <param name="tolerance"></param>
        /// <param name="preserveDirection"></param>
        /// <returns></returns>
        public static Curve JoinClosedSegmentsInOrder(IEnumerable<Curve> curves,
                                                      double tolerance = TOL_LENGTH,
                                                      bool preserveDirection = true)
        {
            Curve joined = Curve.JoinCurves(curves, tolerance, preserveDirection)[0];
            Point3d start = curves.First().PointAtStart;
            if (joined.ClosestPoint(start, out double t))
            {
                joined.ChangeClosedCurveSeam(t);
                joined.Reparameterize();
                return joined;
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="curve"></param>
        public static void Reparameterize(this Curve curve)
        {
            curve.Domain = new Interval(0, 1);
        }

        public static CurveContainment Contains(this Polyline @this,
                                                Polyline polyline,
                                                Plane plane,
                                                double tolerance = TOL_LENGTH,
                                                double angleTolerance = TOL_ANGLE)
        {
            var curve0 = @this.ToPolylineCurve();
            var curve1 = polyline.ToPolylineCurve();

            if (!@this.IsClosed
                || !curve0.TryGetPlane(out Plane plane0)
                || plane0.Normal.IsParallelTo(plane.Normal, angleTolerance) == 0
                || !curve1.TryGetPlane(out Plane plane1)
                || plane1.Normal.IsParallelTo(plane.Normal, angleTolerance) == 0
                || !(plane1.Origin - plane0.Origin).IsPerpendicularTo(plane.Normal, angleTolerance))
            {
                return CurveContainment.Unset;
            }

            if (Intersection.CurveCurve(curve0, curve1, TOL_LENGTH, TOL_OVERLAP).Any())
            {
                return CurveContainment.Intersect;
            }

            if (curve1.Contains(curve0.PointAtStart, plane, tolerance) == PointContainment.Inside)
            {
                return CurveContainment.Contains;
            }
            else if (curve0.Contains(curve1.PointAtStart, plane, tolerance) == PointContainment.Inside)
            {
                return CurveContainment.Inside;
            }
            else
            {
                return CurveContainment.Outside;
            }
        }
    }
}
