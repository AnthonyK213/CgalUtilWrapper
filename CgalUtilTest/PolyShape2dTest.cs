using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Rhino.Runtime.InProcess;
using Rhino.Geometry;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CgalUtilWrapper;

namespace CgalUtilTest
{
  [TestClass]
  public class PolyShape2dTest
  {
    [TestMethod]
    public void StraightSkeleton_ConcurrentTest_WithPolyWithHole_ShouldNotGetFuckingHeapCorruption()
    {
      using (var rhinoCore = new RhinoCore(new string[] { "-runscript" }))
      {
        var curves = JsonConvert.DeserializeObject<List<Curve>>(File.ReadAllText("./mock/ss_test.json"));

        curves.First().ToPolyline(1e-8, 1e-3, 10, 1000).TryGetPolyline(out Polyline outer);

        List<Polyline> inners = new List<Polyline>();

        for (int i = 1; i < curves.Count; ++i)
        {
          curves[i].ToPolyline(1e-8, 1e-3, 10, 1000).TryGetPolyline(out Polyline inner);
          inners.Add(inner);
        }

        Task[] tasks = new Task[50];

        for (int i = 0; i < tasks.Length; ++i)
        {
          tasks[i] = Task.Run(() =>
          {
            PolyShape2d shape = new PolyShape2d(outer, inners);
            shape.GenerateStraightSkeleton(out List<Line> straightSkeleton,
                                           out List<Line> spokes);
          });
        }

        Task.WaitAll(tasks);
      }
    }
  }
}
