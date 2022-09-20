ghenv.Component.NickName = "OptimalBoundingBox"

import CgalUtilWrapper as _cuw
import Rhino.Geometry as _geo

mesh = mesh

triMesh = _cuw.TriMesh(mesh)

ok, obb = triMesh.CreateOptimalBoundingBox()

triMesh.Dispose()
