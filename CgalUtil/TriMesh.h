#pragma once

typedef struct MeshEdges
{
    int* edges;
    int edgesCount;
} MeshEdges;

typedef struct TriMeshFaces
{
    int* faces;
    int facesCount;
} TriMeshFaces;

class TriMesh
{
public:
    TriMesh(Point3dArray* vertices, MeshEdges* edges, TriMeshFaces* faces);
    ~TriMesh();
    void CreateOptimalBoundingBox(Point3dArray* corners);
    void CreateConvexHull(Point3dArray* vertices, TriMeshFaces* faces);
private:
    Surface_Mesh_3d mesh;
};

CU_API TriMesh* TriMeshNew(Point3dArray* vertices, MeshEdges* edges, TriMeshFaces* faces);

CU_API void TriMeshCreateOptimalBoundingBox(TriMesh* handle, Point3dArray* corners);

CU_API void TriMeshCreateConvexHull(TriMesh* handle, Point3dArray* vertices, TriMeshFaces* faces);

CU_API void TriMeshDrop(TriMesh* handle);

CU_API void MeshEdgesFreeMembers(MeshEdges* handle);

CU_API void TriMeshFacesFreeMembers(TriMeshFaces* handle);
