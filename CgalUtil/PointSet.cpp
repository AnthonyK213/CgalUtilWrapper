#include "pch.h"
#include "PointSet.h"

void Point2dSetCreateBoundingCircle(Point2dArray* points, double& x, double& y, double& r)
{
    std::vector<Point_2d> pointList;

    for (int i = 0; i < points->verticesCount; ++i)
    {
        pointList.push_back(Point_2d(points->vertices[2 * i + 0],
                                     points->vertices[2 * i + 1]));
    }

    Min_Sphere_Of_Spheres_D mc(pointList.begin(), pointList.end());

    // ?
    x = 0;
    y = 0;
    r = mc.radius();
}

void Point3dSetCreateConvexHull(Point3dArray* points, Point3dArray* vertices, TriMeshFaces* faces)
{
    Surface_Mesh_3d hull;
    std::vector<Point_3d> pointList;

    for (int i = 0; i < points->pointsCount; ++i)
    {
        pointList.push_back(Point_3d(points->coordinates[3 * i + 0],
                                     points->coordinates[3 * i + 1],
                                     points->coordinates[3 * i + 2]));
    }

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


