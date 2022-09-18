ghenv.Component.NickName = "OptimalBoundingBox"

import CgalUtilWrapper as _cuw
import Rhino.Geometry as _geo

mesh = mesh

simpleMesh = _cuw.SimpleMesh(mesh)

ok, obb = simpleMesh.CreateOptimalBoundingBox()

simpleMesh.Dispose()