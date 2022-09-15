#pragma once

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
