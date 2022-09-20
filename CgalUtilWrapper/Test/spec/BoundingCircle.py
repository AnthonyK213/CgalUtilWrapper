ghenv.Component.NickName = "BoundingCircle"

import CgalUtilWrapper as _cuw
import Rhino.Geometry as _geo

points = points

point2ds = [_geo.Point2d(pt.X, pt.Y) for pt in points]

ok, x, y, r = _cuw.Point2dSet.CreateBoundingCircle(point2ds)
