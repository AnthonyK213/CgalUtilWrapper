#pragma once

#include "pch.h"

class TriMesh {
public:
  TriMesh(Point3dArray *vertices, MeshEdges *edges, TriMeshFaces *faces);

  ~TriMesh();

  void CreateOptimalBoundingBox(Point3dArray *corners);

  void CreateConvexHull(Point3dArray *vertices, TriMeshFaces *faces);

private:
  Surface_Mesh_3d mesh;
};
