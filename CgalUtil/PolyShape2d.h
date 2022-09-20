#pragma once

typedef struct Poly2dArray
{
    int length;
    Point2dArray* start;
} Poly2dArray;

class PolyShape2d
{
public:
    PolyShape2d(Point2dArray* outer, Point2dArray* holes, int holesCount);
    ~PolyShape2d();
    bool IsValid();
    int GenerateStraightSkeleton(Point2dArray* outStraightSkeleton, Point2dArray* outSpokes);
    int GenerateOffsetPolygon();
private:
    bool isValid;
    Polygon_2d_With_Holes poly;
};

CU_API PolyShape2d* PolyShape2dNew(Point2dArray* outer, Point2dArray* holes, int holesCount);

CU_API int PolyShape2dGenerateStraightSkeleton(PolyShape2d* handle, Point2dArray* outStraightSkeleton, Point2dArray* outSpokes);

CU_API int PolyShape2dGenerateOffsetPolygon();

CU_API void PolyShape2dDrop(PolyShape2d* handle);

CU_API void Point2dArrayFreeMembers(Point2dArray* handle);
