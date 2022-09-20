ghenv.Component.NickName = "ConvexHull(2)"

import CgalUtilWrapper as _cuw
import Rhino.Geometry as _geo

points = points

point2ds = [_geo.Point2d(pt.X, pt.Y) for pt in points]

ok, convexHull = _cuw.Point2dSet.CreateConvexHull(point2ds)
