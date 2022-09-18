ghenv.Component.NickName = "ConvexHull"

import CgalUtilWrapper as _cuw
import Rhino.Geometry as _geo

mesh = mesh

simpleMesh = _cuw.SimpleMesh(mesh)

ok, hull = simpleMesh.CreateConvexHull()

simpleMesh.Dispose()
