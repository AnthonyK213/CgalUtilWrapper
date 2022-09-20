#pragma once

#include "TriMesh.h"

CU_API void Point2dSetCreateBoundingCircle(Point2dArray* points, double& x, double& y, double& r);

CU_API void Point2dSetCreateBoundingRectangle(Point2dArray* points, Point2dArray* rectangle);

CU_API void Point2dSetCreateConvexHull(Point2dArray* points, Point2dArray* convexHull);

//CU_API void Point3dSetCreateBoundingSphere(Point3dArray* points, double& x, double& y, double& z, double& r);

CU_API void Point3dSetCreateConvexHull(Point3dArray* points, Point3dArray* vertices, TriMeshFaces* faces);

CU_API void Point3dSetCreateOptimalBoundingBox(Point3dArray* points, Point3dArray* corners);
