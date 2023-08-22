#include "PolyShape2d.h"

PolyShape2d::PolyShape2d(Point2dArray *outer, Point2dArray *holes,
                         int holesCount) {
  Polygon_2d outerPoly;
  isValid = true;

  for (int i = 0; i < outer->verticesCount; ++i) {
    outerPoly.push_back(
        Point_2d(outer->vertices[2 * i], outer->vertices[2 * i + 1]));
  }

  if (!outerPoly.is_counterclockwise_oriented()) {
    isValid = false;
    return;
  }

  poly = Polygon_2d_With_Holes(outerPoly);

  for (int i = 0; i < holesCount; ++i) {
    Polygon_2d hole;

    for (int j = 0; j < holes[i].verticesCount; ++j) {
      hole.push_back(
          Point_2d(holes[i].vertices[2 * j], holes[i].vertices[2 * j + 1]));
    }

    if (!hole.is_clockwise_oriented()) {
      isValid = false;
      return;
    }

    poly.add_hole(hole);
  }
}

PolyShape2d::~PolyShape2d() {}

bool PolyShape2d::IsValid() { return isValid; }

int PolyShape2d::GenerateStraightSkeleton(Point2dArray *outStraightSkeleton,
                                          Point2dArray *outSpokes) {
  boost::shared_ptr<Straight_Skeleton_2d> iss =
      CGAL::create_interior_straight_skeleton_2(poly);

  if (!iss->is_valid()) {
    return 1;
  }

  outStraightSkeleton->verticesCount =
      iss->size_of_halfedges() * 2; // edges * 2 (start and end)
  outStraightSkeleton->vertices =
      new double[outStraightSkeleton->verticesCount * 2];

  outSpokes->verticesCount =
      iss->size_of_halfedges() * 2; // edges * 2 (start and end)
  outSpokes->vertices = new double[outSpokes->verticesCount * 2];

  int ssIndex = 0, bsIndex = 0;

  for (auto it = iss->halfedges_begin(); it != iss->halfedges_end(); ++it) {
    auto start = it->vertex();
    auto end = it->opposite()->vertex();

    if (!it->is_bisector()) {
      continue;
    }

    auto &startPos = start->point();
    auto &endPos = end->point();

    if (start->is_skeleton() && end->is_skeleton() && it->is_inner_bisector()) {
      outStraightSkeleton->vertices[ssIndex * 4 + 0] = startPos.x();
      outStraightSkeleton->vertices[ssIndex * 4 + 1] = startPos.y();
      outStraightSkeleton->vertices[ssIndex * 4 + 2] = endPos.x();
      outStraightSkeleton->vertices[ssIndex * 4 + 3] = endPos.y();
      ssIndex++;
    } else {
      outSpokes->vertices[bsIndex * 4 + 0] = startPos.x();
      outSpokes->vertices[bsIndex * 4 + 1] = startPos.y();
      outSpokes->vertices[bsIndex * 4 + 2] = endPos.x();
      outSpokes->vertices[bsIndex * 4 + 3] = endPos.y();
      bsIndex++;
    }
  }

  outStraightSkeleton->verticesCount = ssIndex * 2;
  outSpokes->verticesCount = bsIndex * 2;

  return 0;
}

int PolyShape2d::GenerateOffsetPolygon() { return 0; }
