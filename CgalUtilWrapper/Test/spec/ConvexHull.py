ghenv.Component.NickName = "ConvexHull"

import CgalUtilWrapper as _cuw
import Rhino.Geometry as _geo

mesh = mesh

triMesh = _cuw.TriMesh(mesh)

ok, hull = triMesh.CreateConvexHull()

triMesh.Dispose()
