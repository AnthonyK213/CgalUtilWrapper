#include "pch.h"
#include "SimpleMesh.h"

SimpleMesh::SimpleMesh(Point3dArray* vertices, MeshEdges* edges, MeshFaces* faces)
{
    mesh.clear();
    mesh.reserve(vertices->pointsCount, edges->edgesCount, faces->facesCount);

    for (int i = 0; i < vertices->pointsCount; ++i)
    {
        mesh.add_vertex(Point_3d(vertices->coordinates[3 * i + 0],
                                 vertices->coordinates[3 * i + 1],
                                 vertices->coordinates[3 * i + 2]));
    }

    for (int i = 0; i < edges->edgesCount; ++i)
    {
        mesh.add_edge(CGAL::SM_Vertex_index(edges->edges[2 * i + 0]),
                      CGAL::SM_Vertex_index(edges->edges[2 * i + 1]));
    }

    for (int i = 0; i < faces->facesCount; ++i)
    {
        mesh.add_face(CGAL::SM_Vertex_index(faces->faces[3 * i + 0]),
                      CGAL::SM_Vertex_index(faces->faces[3 * i + 1]),
                      CGAL::SM_Vertex_index(faces->faces[3 * i + 2]));
    }
}

SimpleMesh::~SimpleMesh()
{

}

void SimpleMesh::CreateOptimalBoundingBox(Point3dArray* corners)
{
    std::array<Point_3d, 8> obb_points;
    CGAL::oriented_bounding_box(mesh, obb_points, CGAL::parameters::use_convex_hull(true));

    corners->pointsCount = obb_points.size();
    corners->coordinates = new double[corners->pointsCount * 3];
    
    for (int i = 0; i < corners->pointsCount; ++i)
    {
        corners->coordinates[3 * i + 0] = obb_points[i].x();
        corners->coordinates[3 * i + 1] = obb_points[i].y();
        corners->coordinates[3 * i + 2] = obb_points[i].z();
    }
}

void SimpleMesh::CreateConvexHull(Point3dArray* coordinates, MeshEdges* edges, MeshFaces* faces)
{

}

SimpleMesh* SimpleMeshNew(Point3dArray* vertices, MeshEdges* edges, MeshFaces* faces)
{
    SimpleMesh* handle = new SimpleMesh(vertices, edges, faces);
    return handle;
}

void SimpleMeshCreateOptimalBoundingBox(SimpleMesh* handle, Point3dArray* corners)
{
    handle->CreateOptimalBoundingBox(corners);
}

void SimpleMeshCreateConvexHull(SimpleMesh* handle, Point3dArray* vertices, MeshEdges* edges, MeshFaces* faces)
{
    handle->CreateConvexHull(vertices, edges, faces);
}

void SimpleMeshDrop(SimpleMesh* handle)
{
    if (handle != nullptr)
    {
        delete handle;
    }
}
