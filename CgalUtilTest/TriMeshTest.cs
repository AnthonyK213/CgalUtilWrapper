using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Rhino.Runtime.InProcess;
using Rhino.Geometry;
using Newtonsoft.Json;
using System.IO;
using CgalUtilWrapper;

namespace CgalUtilTest
{
  [TestClass]
  public class TriMeshTest
  {
    [TestMethod]
    public void Obb_ConcurrentTest_WithRhinoMesh_ShouldNotGetFuckingHeapCorruption()
    {
      using (var rhinoCore = new RhinoCore(new string[] { "-runscript" }))
      {
        var mesh = JsonConvert.DeserializeObject<Mesh>(File.ReadAllText("./mock/obb_test.json"));

        Task[] tasks = new Task[10];

        for (int i = 0; i < tasks.Length; ++i)
        {
          tasks[i] = Task.Run(() =>
          {
            using (TriMesh triMesh = new TriMesh(mesh))
            {
              triMesh.CreateOptimalBoundingBox(out Box obb);
            }
          });
        }

        Task.WaitAll(tasks);
      }
    }
  }
}
