#pragma once

#define WIN32_LEAN_AND_MEAN
#define CGAL_EIGEN3_ENABLED
#define CU_API extern "C" _declspec(dllexport)

#include <CGAL/Exact_predicates_inexact_constructions_kernel.h>
#include <CGAL/Min_sphere_of_spheres_d.h>
#include <CGAL/Min_sphere_of_points_d_traits_2.h>
#include <CGAL/Min_sphere_of_points_d_traits_3.h>
#include <CGAL/Polygon_mesh_processing/measure.h>
#include <CGAL/Polygon_mesh_processing/triangulate_faces.h>
#include <CGAL/Polygon_with_holes_2.h>
#include <CGAL/Polyhedron_3.h>
#include <CGAL/Simple_cartesian.h>
#include <CGAL/Surface_mesh.h>
#include <CGAL/convex_hull_2.h>
#include <CGAL/convex_hull_3.h>
#include <CGAL/create_straight_skeleton_from_polygon_with_holes_2.h>
#include <CGAL/min_quadrilateral_2.h>
#include <CGAL/optimal_bounding_box.h>
#include <CGAL/Convex_hull_traits_adapter_2.h>
#include <CGAL/Min_quadrilateral_traits_2.h>
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
typedef CGAL::Min_sphere_of_points_d_traits_2<K, double> Min_Sphere_Of_Points_D_Traits_2;
typedef CGAL::Min_sphere_of_points_d_traits_3<K, double> Min_Sphere_Of_Points_D_Traits_3;
typedef CGAL::Min_sphere_of_spheres_d<Min_Sphere_Of_Points_D_Traits_2> Min_Sphere_Of_Spheres_D;
typedef CGAL::Min_sphere_of_spheres_d<Min_Sphere_Of_Points_D_Traits_3> Min_Sphere_Of_Spheres_D_3;
