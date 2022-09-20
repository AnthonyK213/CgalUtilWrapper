#pragma once

#include "TriMesh.h"

CU_API void Point2dSetCreateBoundingCircle(Point2dArray* points, double& x, double& y, double& r);

CU_API void Point3dSetCreateConvexHull(Point3dArray* points, Point3dArray* vertices, TriMeshFaces* faces);
