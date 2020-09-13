using System;
using 
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace GeoTest
{
   [TestClass]
   [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
   public class LineTest
   {
      [TestMethod]
      public void Line2dTest()
      {
         Point2d p1 = new Point2d(0, 0);
      }

      private string GetDebuggerDisplay()
      {
         return ToString();
      }
   }
}
