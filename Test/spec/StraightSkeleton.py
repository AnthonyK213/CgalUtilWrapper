ghenv.Component.NickName = "StraightSkeletonTest"

import CgalUtilWrapper as _cuw

outer = outer
inner = inner

shape = _cuw.PolyShape2d(outer, inner)

ok, straightSkeleton = shape.GenerateStraightSkeleton()

shape.Dispose()
