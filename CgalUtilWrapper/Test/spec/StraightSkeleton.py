ghenv.Component.NickName = "StraightSkeleton"

import CgalUtilWrapper as _cuw
import Rhino.Geometry as _geo

outer = outer
inner = inner

def RemoveDuplicateLines(lines, tolerance = 0.01):
    result = []
    tolSqr = tolerance ** 2
    for line in lines:
        ok = True
        for l in result:
            d1 = line.From.DistanceToSquared(l.From) <= tolSqr
            d2 = line.To.DistanceToSquared(l.To) <= tolSqr
            d3 = line.From.DistanceToSquared(l.To) <= tolSqr
            d4 = line.To.DistanceToSquared(l.From) <= tolSqr
            if (d1 and d2) or (d3 and d4):
                ok = False
                break
        if ok:
            result.append(line)
    return result

def RemoveDuplicateCircles(circles, distTolerance = 0.15, radiusTolerance = 0.15):
    result = []
    for circ in circles:
        ok = True
        for c in result:
            r1 = circ.Radius
            r2 = c.Radius
            r = max(r1, r2)
            d1 = circ.Center.DistanceTo(c.Center) <= distTolerance * r
            d2 = abs(r1 - r2) <= radiusTolerance * r
            if d1 and d2:
                ok = False
                break
        if ok:
            result.append(circ)
    return result

def GenerageEnvelope(bounds, skeleton):
    circles = []
    medial = _geo.Curve.JoinCurves([line.ToNurbsCurve() for line in skeleton], 0.1)
    for curve in medial:
        pt = curve.PointAtStart
        lenSum = 0
        length = curve.GetLength()
        while lenSum <= length:
            radius = min([pt.DistanceTo(bound.ClosestPoint(pt)) for bound in bounds])
            circles.append(_geo.Circle(_geo.Plane(pt, _geo.Plane.WorldXY.ZAxis), radius))
            if (lenSum == length):
                break
            lenSum = min(length, lenSum + max(radius, 10))
            pt = curve.PointAtLength(lenSum)
    return circles

shape = _cuw.PolyShape2d(outer, inner)

ok, straightSkeleton, spokes = shape.GenerateStraightSkeleton()

print(shape.Error)

shape.Dispose()

straightSkeleton = RemoveDuplicateLines(straightSkeleton)

envelope = GenerageEnvelope([outer] + inner, straightSkeleton)

envelope = RemoveDuplicateCircles(envelope)
