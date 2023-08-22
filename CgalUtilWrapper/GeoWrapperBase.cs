using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

namespace CgalUtilWrapper
{
  public abstract class GeoWrapperBase : SafeHandleZeroOrMinusOneIsInvalid
  {
    public const string DLL = "CgalUtil.dll";

    protected GeoWrapperBase(bool ownsHandle) : base(ownsHandle) { }
  }

  [StructLayout(LayoutKind.Sequential)]
  internal unsafe struct Point2dArray
  {
    public readonly double* _vertices;
    public readonly int _verticesCount;

    public Point2dArray(double* vertices, int verticesCount)
    {
      _vertices = vertices;
      _verticesCount = verticesCount;
    }

    [DllImport(GeoWrapperBase.DLL, CallingConvention = CallingConvention.Cdecl)]
    extern public static unsafe void Point2dArrayFreeMembers(Point2dArray* handle);
  }

  [StructLayout(LayoutKind.Sequential)]
  internal unsafe struct Point3dArray
  {
    public readonly double* _coordinates;
    public readonly int _pointsCount;

    public Point3dArray(double* coordinates, int pointsCount)
    {
      _coordinates = coordinates;
      _pointsCount = pointsCount;
    }

    [DllImport(GeoWrapperBase.DLL, CallingConvention = CallingConvention.Cdecl)]
    extern public static unsafe void Point3dArrayFreeMembers(Point3dArray* handle);
  }

  [StructLayout(LayoutKind.Sequential)]
  internal unsafe struct MeshEdges
  {
    public readonly int* _edges;
    public readonly int _edgesCount;

    public MeshEdges(int* edges, int edgesCount)
    {
      _edges = edges;
      _edgesCount = edgesCount;
    }
  }

  [StructLayout(LayoutKind.Sequential)]
  internal unsafe struct TriMeshFaces
  {
    public readonly int* _faces;
    public readonly int _facesCount;

    public TriMeshFaces(int* faces, int facesCount)
    {
      _faces = faces;
      _facesCount = facesCount;
    }

    [DllImport(GeoWrapperBase.DLL, CallingConvention = CallingConvention.Cdecl)]
    extern public static unsafe void TriMeshFacesFreeMembers(TriMeshFaces* handle);
  }
}
