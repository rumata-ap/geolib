using System;

using Geo;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeoTest
{
   [TestClass]
   public class LineTest
   {
      [TestMethod]
      public void Line2dTest()
      {
         Point2d p1 = new Point2d(0, 0);
         Point2d p2 = new Point2d(1, 1);
         Line2d line = new Line2d(p1, p2);
      }

      [TestMethod]
      public void Line3dTest()
      {
         Point2d p1 = new Point2d(0, 0);
         Point2d p2 = new Point2d(1, 1);
         Line3d line = new Line3d(p1, p2);
      }
   }
}
