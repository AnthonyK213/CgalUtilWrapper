#pragma once

#define WIN32_LEAN_AND_MEAN
#define CU_API extern "C" _declspec(dllexport)

#include <CGAL/Exact_predicates_inexact_constructions_kernel.h>
#include <CGAL/Polygon_mesh_processing/measure.h>
#include <CGAL/Polygon_mesh_processing/triangulate_faces.h>
#include <CGAL/Polygon_with_holes_2.h>
#include <CGAL/Polyhedron_3.h>
#include <CGAL/Surface_mesh.h>
#include <CGAL/convex_hull_3.h>
#include <CGAL/create_straight_skeleton_from_polygon_with_holes_2.h>
#include <CGAL/optimal_bounding_box.h>
#include <boost/shared_ptr.hpp>
#include <cassert>
#include <vector>
#include <windows.h>

typedef CGAL::Exact_predicates_inexact_constructions_kernel K;
typedef K::Point_2 Point_2d;
typedef K::Point_3 Point_3d;
typedef CGAL::Polygon_2<K> Polygon_2d;
typedef CGAL::Polygon_with_holes_2<K> Polygon_2d_With_Holes;
typedef CGAL::Polyhedron_3<K> Polyhedron_3d;
typedef CGAL::Straight_skeleton_2<K> Straight_Skeleton_2d;
typedef CGAL::Surface_mesh<Point_3d> Surface_Mesh_3d;
