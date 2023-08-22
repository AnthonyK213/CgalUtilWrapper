#pragma once

#include "pch.h"

class PolyShape2d {
public:
  PolyShape2d(Point2dArray *outer, Point2dArray *holes, int holesCount);

  ~PolyShape2d();

  bool IsValid();

  int GenerateStraightSkeleton(Point2dArray *outStraightSkeleton,
                               Point2dArray *outSpokes);

  int GenerateOffsetPolygon();

private:
  bool m_isValid;

  Polygon_2d_With_Holes m_poly;
};
