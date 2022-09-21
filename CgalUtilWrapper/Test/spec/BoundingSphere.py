ghenv.Component.NickName = "BoundingCircle"

import CgalUtilWrapper as _cuw
import Rhino.Geometry as _geo

points = points

ok, sphere = _cuw.Point3dSet.CreateBoundingSphere(points)
