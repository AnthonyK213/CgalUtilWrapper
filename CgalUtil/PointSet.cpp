#include "pch.h"
#include "PointSet.h"

std::vector<Point_2d> Point2dArrayToPoint2dList(Point2dArray* points)
{
    std::vector<Point_2d> pointList;
    for (int i = 0; i < points->verticesCount; ++i)
    {
        pointList.push_back(Point_2d(points->vertices[2 * i + 0],
                                     points->vertices[2 * i + 1]));
    }

    return pointList;
}

std::vector<Point_3d> Point3dArrayToPoint3dList(Point3dArray* points)
{
    std::vector<Point_3d> pointList;
    for (int i = 0; i < points->pointsCount; ++i)
    {
        pointList.push_back(Point_3d(points->coordinates[3 * i + 0],
                                     points->coordinates[3 * i + 1],
                                     points->coordinates[3 * i + 2]));
    }

    return pointList;
}

void Point2dSetCreateBoundingCircle(Point2dArray* points, double& x, double& y, double& r)
{
    auto pointList = Point2dArrayToPoint2dList(points);
    Min_Sphere_Of_Spheres_D mc(pointList.begin(), pointList.end());
    x = *mc.center_cartesian_begin();
    y = *(mc.center_cartesian_begin() + 1);
    r = mc.radius();
}

void Point2dSetCreateBoundingRectangle(Point2dArray* points, Point2dArray* rectangle)
{
    auto pointList = Point2dArrayToPoint2dList(points);
    std::vector<std::size_t> indices(pointList.size()), outIndices;
    std::iota(indices.begin(), indices.end(), 0);
    CGAL::convex_hull_2(indices.begin(), indices.end(), std::back_inserter(outIndices),
        CGAL::Convex_hull_traits_adapter_2<K, CGAL::Pointer_property_map<Point_2d>::type>(CGAL::make_property_map(pointList)));
    std::vector<Point_2d> hullPoints;
    for (auto& index : outIndices)
    {
        hullPoints.push_back(pointList[index]);
    }
    std::vector<Point_2d> corners;
    CGAL::min_rectangle_2(hullPoints.begin(), hullPoints.end(), std::back_inserter(corners));
    rectangle->verticesCount = 4;
    rectangle->vertices = new double[8];

    int i = 0;
    for (auto& pt : corners)
    {
        rectangle->vertices[i++] = pt.x();
        rectangle->vertices[i++] = pt.y();
    }
}

void Point2dSetCreateConvexHull(Point2dArray* points, Point2dArray* convexHull)
{
    auto pointList = Point2dArrayToPoint2dList(points);
    std::vector<std::size_t> indices(pointList.size()), outIndices;
    std::iota(indices.begin(), indices.end(), 0);
    CGAL::convex_hull_2(indices.begin(), indices.end(), std::back_inserter(outIndices),
        CGAL::Convex_hull_traits_adapter_2<K, CGAL::Pointer_property_map<Point_2d>::type>(CGAL::make_property_map(pointList)));
    convexHull->verticesCount = outIndices.size();
    convexHull->vertices = new double[convexHull->verticesCount * 2];

    int i = 0;
    for (auto& index : outIndices)
    {
        convexHull->vertices[i++] = pointList[index].x();
        convexHull->vertices[i++] = pointList[index].y();
    }
}

void Point3dSetCreateBoundingSphere(Point3dArray* points, double& x, double& y, double& z, double& r)
{
    std::vector<Point_3d> pointList;

    for (int i = 0; i < points->pointsCount; ++i)
    {
        pointList.push_back(Point_3d(points->coordinates[3 * i + 0],
                                     points->coordinates[3 * i + 1],
                                     points->coordinates[3 * i + 2]));
    }

    Min_Sphere_Of_Spheres_D_3 ms(pointList.begin(), pointList.end());

    x = *ms.center_cartesian_begin();
    y = *(ms.center_cartesian_begin() + 1);
    z = *(ms.center_cartesian_begin() + 2);
    r = ms.radius();
}

void Point3dSetCreateConvexHull(Point3dArray* points, Point3dArray* vertices, TriMeshFaces* faces)
{
    Surface_Mesh_3d hull;
    std::vector<Point_3d> pointList = Point3dArrayToPoint3dList(points);
    CGAL::convex_hull_3(pointList.begin(), pointList.end(), hull);
    vertices->pointsCount = hull.vertices().size();
    vertices->coordinates = new double[vertices->pointsCount * 3];

    faces->facesCount = hull.faces().size();
    faces->faces = new int[faces->facesCount * 3];

    int i = 0;
    for (auto& point : hull.points())
    {
        vertices->coordinates[i++] = point.x();
        vertices->coordinates[i++] = point.y();
        vertices->coordinates[i++] = point.z();
    }

    int k = 0;
    for (auto& face : hull.faces())
    {
        for (Surface_Mesh_3d::Halfedge_index h : hull.halfedges_around_face(hull.halfedge(face)))
        {
            faces->faces[k++] = hull.target(h);
        }
    }
}

void Point3dSetCreateOptimalBoundingBox(Point3dArray* points, Point3dArray* corners)
{
    std::array<Point_3d, 8> obb_points;
    CGAL::oriented_bounding_box(Point3dArrayToPoint3dList(points), obb_points, CGAL::parameters::use_convex_hull(true));

    corners->pointsCount = obb_points.size();
    corners->coordinates = new double[corners->pointsCount * 3];
    
    for (int i = 0; i < corners->pointsCount; ++i)
    {
        corners->coordinates[3 * i + 0] = obb_points[i].x();
        corners->coordinates[3 * i + 1] = obb_points[i].y();
        corners->coordinates[3 * i + 2] = obb_points[i].z();
    }
}
