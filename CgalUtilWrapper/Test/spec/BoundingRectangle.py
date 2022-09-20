ghenv.Component.NickName = "BoundingRectangle"

import CgalUtilWrapper as _cuw
import Rhino.Geometry as _geo

points = points

point2ds = [_geo.Point2d(pt.X, pt.Y) for pt in points]

ok, rectangle = _cuw.Point2dSet.CreateBoundingRectangle(point2ds)
