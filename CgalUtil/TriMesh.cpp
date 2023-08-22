#include "TriMesh.h"

TriMesh::TriMesh(Point3dArray *vertices, MeshEdges *edges,
                 TriMeshFaces *faces) {
  m_mesh.clear();
  m_mesh.reserve(vertices->pointsCount, edges->edgesCount, faces->facesCount);

  for (int i = 0; i < vertices->pointsCount; ++i) {
    m_mesh.add_vertex(Point_3d(vertices->coordinates[3 * i + 0],
                             vertices->coordinates[3 * i + 1],
                             vertices->coordinates[3 * i + 2]));
  }

  for (int i = 0; i < edges->edgesCount; ++i) {
    m_mesh.add_edge(CGAL::SM_Vertex_index(edges->edges[2 * i + 0]),
                  CGAL::SM_Vertex_index(edges->edges[2 * i + 1]));
  }

  for (int i = 0; i < faces->facesCount; ++i) {
    m_mesh.add_face(CGAL::SM_Vertex_index(faces->faces[3 * i + 0]),
                  CGAL::SM_Vertex_index(faces->faces[3 * i + 1]),
                  CGAL::SM_Vertex_index(faces->faces[3 * i + 2]));
  }
}

TriMesh::~TriMesh() {}

void TriMesh::CreateOptimalBoundingBox(Point3dArray *corners) {
  std::array<Point_3d, 8> obb_points;
  CGAL::oriented_bounding_box(m_mesh, obb_points,
                              CGAL::parameters::use_convex_hull(true));

  corners->pointsCount = obb_points.size();
  corners->coordinates = new double[corners->pointsCount * 3];

  for (int i = 0; i < corners->pointsCount; ++i) {
    corners->coordinates[3 * i + 0] = obb_points[i].x();
    corners->coordinates[3 * i + 1] = obb_points[i].y();
    corners->coordinates[3 * i + 2] = obb_points[i].z();
  }
}

void TriMesh::CreateConvexHull(Point3dArray *vertices, TriMeshFaces *faces) {
  Surface_Mesh_3d hull;
  auto &points = m_mesh.points();
  CGAL::convex_hull_3(points.begin(), points.end(), hull);

  vertices->pointsCount = hull.vertices().size();
  vertices->coordinates = new double[vertices->pointsCount * 3];

  faces->facesCount = hull.faces().size();
  faces->faces = new int[faces->facesCount * 3];

  int i = 0;
  for (auto &point : hull.points()) {
    vertices->coordinates[i++] = point.x();
    vertices->coordinates[i++] = point.y();
    vertices->coordinates[i++] = point.z();
  }

  int k = 0;
  for (auto &face : hull.faces()) {
    for (Surface_Mesh_3d::Halfedge_index h :
         hull.halfedges_around_face(hull.halfedge(face))) {
      faces->faces[k++] = hull.target(h);
    }
  }
}
