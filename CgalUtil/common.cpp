#include "pch.h"

void Point3dArrayFreeMembers(Point3dArray* handle)
{
    if (nullptr != handle)
    {
        if (nullptr != handle->coordinates)
        {
            delete handle->coordinates;
            handle->coordinates = nullptr;
        }
        handle->pointsCount = 0;
    }
}
