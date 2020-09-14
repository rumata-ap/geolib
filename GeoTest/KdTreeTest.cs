using System;

using Geo;
using Geo.Calc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeoTest
{
   [TestClass]
   public class KdTreeTest
   {
      Point3d p1 = new Point3d(0, 0, -1.2);
      Point3d p2 = new Point3d(1, 0, 0.1);
      Point3d p3 = new Point3d(0.2, 1, 0);
      Point3d p4 = new Point3d(1, 0, 2.5);
      Point3d p5 = new Point3d(0.4942, 0.6457, 0);
      Point3d p6 = new Point3d(4.4949, 1.1326, 0);
      Point3d p7 = new Point3d(1.8277, 0.808, 0);
      Point3d p8 = new Point3d(0.4942, 0.3672, 0);
      Point3d p9 = new Point3d(2.2666, 2.3147, 0);
      Point3d p10 = new Point3d(3, 1, 1);
      Point3d p11 = new Point3d(1, 1, 0);

      [TestMethod]
      public void KdTreeTest1()
      {
         KdTree tree = new KdTree();
         tree.Insert(p1);
         tree.Insert(p2);
         tree.Insert(p3);
         tree.Insert(p4);
         tree.Insert(p5);
         tree.Insert(p5);
         tree.Insert(p7);
         tree.Insert(p8);
         tree.Insert(p9);
         tree.Insert(p10);
         tree.Insert(p11);
      }

      [TestMethod]
      public void KdTreeTest2()
      {
         KdTree tree = new KdTree();
         IXYZ[] ps = { p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11 };
         tree.Insert(ps);
         Point2d n = new Point2d(tree.Nearest(p9));
      }
   }
}
