#include "pch.h"

void Point3dArrayFreeMembers(Point3dArray* handle)
{
    if (handle != nullptr)
    {
        delete handle->coordinates;
        handle->coordinates = nullptr;
        handle->pointsCount = 0;
    }
}
