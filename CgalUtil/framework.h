#pragma once

#define WIN32_LEAN_AND_MEAN
#define CU_API extern "C" _declspec(dllexport)

#include <windows.h>
#include <CGAL/Exact_predicates_inexact_constructions_kernel.h>
#include <CGAL/Polygon_with_holes_2.h>
#include <CGAL/create_straight_skeleton_from_polygon_with_holes_2.h>
#include <boost/shared_ptr.hpp>
#include <cassert>

typedef CGAL::Exact_predicates_inexact_constructions_kernel K;
typedef K::Point_2 Point;
typedef CGAL::Polygon_2<K> Polygon_2;
typedef CGAL::Polygon_with_holes_2<K> Polygon_with_holes;
typedef CGAL::Straight_skeleton_2<K> Ss;
typedef boost::shared_ptr<Ss> SsPtr;

typedef struct
{
    double* vertices;
    int verticesCount;
} Poly2d;

typedef struct
{
    int length;
    Poly2d* start;
} Poly2dArray;

class PolyShape2d
{
public:
    PolyShape2d(Poly2d* outer, Poly2d* holes, int holesCount);
    ~PolyShape2d();
    bool IsValid();
    int GenerateStraightSkeleton(Poly2d* outStraightSkeleton, Poly2d* outSpokes);
    int GenerateOffsetPolygon();
private:
    bool isValid;
    Polygon_with_holes poly;
};

CU_API PolyShape2d* PolyShape2dNew(Poly2d* outer, Poly2d* holes, int holesCount);

CU_API int PolyShape2dGenerateStraightSkeleton(PolyShape2d* handle, Poly2d* outStraightSkeleton, Poly2d* outSpokes);

CU_API int PolyShape2dGenerateOffsetPolygon();

CU_API void PolyShape2dDrop(PolyShape2d* handle);

CU_API void FreePoly2dMembers(Poly2d* handle);
