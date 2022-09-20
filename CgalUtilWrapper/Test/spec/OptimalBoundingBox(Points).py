ghenv.Component.NickName = "OptimalBoundingBox(Points)"

import CgalUtilWrapper as _cuw
import Rhino.Geometry as _geo

points = points

ok, obb = _cuw.Point3dSet.CreateOptimalBoundingBox(points)
