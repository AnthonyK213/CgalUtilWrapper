#pragma once

#include "pch.h"

#include "PolyShape2d.h"
#include "TriMesh.h"

#ifndef CU_API
#define CU_API extern "C" _declspec(dllexport)
#endif

CU_API void Point2dSetCreateBoundingCircle(Point2dArray *points, double &x,
                                           double &y, double &r);

CU_API void Point2dSetCreateBoundingRectangle(Point2dArray *points,
                                              Point2dArray *rectangle);

CU_API void Point2dSetCreateConvexHull(Point2dArray *points,
                                       Point2dArray *convexHull);

CU_API void Point2dArrayFreeMembers(Point2dArray *handle);

CU_API void Point3dSetCreateBoundingSphere(Point3dArray *points, double &x,
                                           double &y, double &z, double &r);

CU_API void Point3dSetCreateConvexHull(Point3dArray *points,
                                       Point3dArray *vertices,
                                       TriMeshFaces *faces);

CU_API void Point3dSetCreateOptimalBoundingBox(Point3dArray *points,
                                               Point3dArray *corners);

CU_API void Point3dArrayFreeMembers(Point3dArray *handle);

CU_API PolyShape2d *PolyShape2dNew(Point2dArray *outer, Point2dArray *holes,
                                   int holesCount);

CU_API int
PolyShape2dGenerateStraightSkeleton(PolyShape2d *handle,
                                    Point2dArray *outStraightSkeleton,
                                    Point2dArray *outSpokes);

CU_API int PolyShape2dGenerateOffsetPolygon();

CU_API void PolyShape2dDrop(PolyShape2d *handle);

CU_API TriMesh *TriMeshNew(Point3dArray *vertices, MeshEdges *edges,
                           TriMeshFaces *faces);

CU_API void TriMeshCreateOptimalBoundingBox(TriMesh *handle,
                                            Point3dArray *corners);

CU_API void TriMeshCreateConvexHull(TriMesh *handle, Point3dArray *vertices,
                                    TriMeshFaces *faces);

CU_API void TriMeshDrop(TriMesh *handle);

CU_API void MeshEdgesFreeMembers(MeshEdges *handle);

CU_API void TriMeshFacesFreeMembers(TriMeshFaces *handle);

#undef CU_API
