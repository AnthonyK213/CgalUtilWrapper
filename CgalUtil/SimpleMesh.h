#pragma once

typedef struct MeshEdges
{
    int* edges;
    int edgesCount;
} MeshEdges;

typedef struct MeshFaces
{
    int* faces;
    int facesCount;
} MeshFaces;

class SimpleMesh
{
public:
    SimpleMesh(Point3dArray* vertices, MeshEdges* edges, MeshFaces* faces);
    ~SimpleMesh();
    void CreateOptimalBoundingBox(Point3dArray* corners);
    void CreateConvexHull(Point3dArray* vertices, MeshFaces* faces);
private:
    Surface_Mesh_3d mesh;
};

CU_API SimpleMesh* SimpleMeshNew(Point3dArray* vertices, MeshEdges* edges, MeshFaces* faces);

CU_API void SimpleMeshCreateOptimalBoundingBox(SimpleMesh* handle, Point3dArray* corners);

CU_API void SimpleMeshCreateConvexHull(SimpleMesh* handle, Point3dArray* vertices, MeshFaces* faces);

CU_API void SimpleMeshDrop(SimpleMesh* handle);

CU_API void MeshEdgesFreeMembers(MeshEdges* handle);

CU_API void MeshFacesFreeMembers(MeshFaces* handle);
