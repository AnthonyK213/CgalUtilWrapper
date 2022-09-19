#include "pch.h"
#include "PolyShape2d.h"

PolyShape2d::PolyShape2d(Poly2d* outer, Poly2d* holes, int holesCount)
{
    Polygon_2d outerPoly;
    isValid = true;

    for (int i = 0; i < outer->verticesCount; ++i)
    {
        outerPoly.push_back(Point_2d(outer->vertices[2 * i], outer->vertices[2 * i + 1]));
    }

    if (!outerPoly.is_counterclockwise_oriented())
    {
        isValid = false;
        return;
    }

    poly = Polygon_2d_With_Holes(outerPoly);

    for (int i = 0; i < holesCount; ++i)
    {
        Polygon_2d hole;

        for (int j = 0; j < holes[i].verticesCount; ++j)
        {
            hole.push_back(Point_2d(holes[i].vertices[2 * j], holes[i].vertices[2 * j + 1]));
        }

        if (!hole.is_clockwise_oriented())
        {
            isValid = false;
            return;
        }

        poly.add_hole(hole);
    }
}

PolyShape2d::~PolyShape2d()
{
}

bool PolyShape2d::IsValid()
{
    return isValid;
}

int PolyShape2d::GenerateStraightSkeleton(Poly2d* outStraightSkeleton, Poly2d* outSpokes)
{
    boost::shared_ptr<Straight_Skeleton_2d> iss = CGAL::create_interior_straight_skeleton_2(poly);
    Straight_Skeleton_2d& ss = *iss;

    if (!iss->is_valid())
    {
        return 1;
    }

    outStraightSkeleton->verticesCount = ss.size_of_halfedges() * 2 * 2;  // edges * 2 (start and end) * 2 (X and Y)
    outStraightSkeleton->vertices = new double[outStraightSkeleton->verticesCount];

    outSpokes->verticesCount = ss.size_of_halfedges() * 2 * 2;  // edges * 2 (start and end) * 2 (X and Y)
    outSpokes->vertices = new double[outSpokes->verticesCount];

    // Copy vertex pairs
    int ssIndex = 0;
    int bsIndex = 0;
    for (Straight_Skeleton_2d::Halfedge_const_iterator i = ss.halfedges_begin(); i != ss.halfedges_end(); ++i)
    {
        auto start = i->vertex();
        auto end = i->opposite()->vertex();

        if (!i->is_bisector())
        {
            continue;
        }

        auto& startPos = start->point();
        auto& endPos = end->point();

        bool startSkele = start->is_skeleton();
        bool endSkele = end->is_skeleton();

        if (startSkele && endSkele && i->is_inner_bisector())
        {
            outStraightSkeleton->vertices[ssIndex * 4] = startPos.x();
            outStraightSkeleton->vertices[ssIndex * 4 + 1] = startPos.y();
            outStraightSkeleton->vertices[ssIndex * 4 + 2] = endPos.x();
            outStraightSkeleton->vertices[ssIndex * 4 + 3] = endPos.y();
            ssIndex++;
        }
        else
        {
            outSpokes->vertices[bsIndex * 4] = startPos.x();
            outSpokes->vertices[bsIndex * 4 + 1] = startPos.y();
            outSpokes->vertices[bsIndex * 4 + 2] = endPos.x();
            outSpokes->vertices[bsIndex * 4 + 3] = endPos.y();
            bsIndex++;
        }
    }

    //Set the sizes
    outStraightSkeleton->verticesCount = ssIndex * 2;
    outSpokes->verticesCount = bsIndex * 2;

    return 0;
}

int PolyShape2d::GenerateOffsetPolygon()
{
    return 0;
}

PolyShape2d* PolyShape2dNew(Poly2d* outer, Poly2d* holes, int holesCount)
{
    PolyShape2d* handle = new PolyShape2d(outer, holes, holesCount);
    return handle->IsValid() ? handle : NULL;
}

int PolyShape2dGenerateStraightSkeleton(PolyShape2d* handle, Poly2d* outStraightSkeleton, Poly2d* outSpokes)
{
    return handle->GenerateStraightSkeleton(outStraightSkeleton, outSpokes);
}

int PolyShape2dGenerateOffsetPolygon()
{
    return 0;
}

void PolyShape2dDrop(PolyShape2d* handle)
{
    if (handle != nullptr)
    {
        delete handle;
    }
}

void Poly2dFreeMembers(Poly2d* handle)
{
    if (handle != nullptr)
    {
        delete handle->vertices;
        handle->vertices = nullptr;
        handle->verticesCount = 0;
    }
}
