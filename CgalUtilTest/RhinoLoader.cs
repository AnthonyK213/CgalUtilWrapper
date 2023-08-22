using Microsoft.VisualStudio.TestTools.UnitTesting;
using RhinoInside;

namespace CgalUtilTest
{
  [TestClass]
  public class RhinoLoader
  {
    [AssemblyInitialize]
    public static void AssemblyInit(TestContext context)
    {
      Resolver.Initialize();
    }
  }
}
