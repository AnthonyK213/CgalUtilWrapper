#pragma once

typedef struct Point3dArray
{
    double* coordinates;
    int pointsCount;
} Point3dArray;

CU_API void Point3dArrayFreeMembers(Point3dArray* handle);
