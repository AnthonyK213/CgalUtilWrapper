ghenv.Component.NickName = "ConvexHull(Points)"

import CgalUtilWrapper as _cuw
import Rhino.Geometry as _geo

points = points

ok, hull = _cuw.Point3dSet.CreateConvexHull(points)
