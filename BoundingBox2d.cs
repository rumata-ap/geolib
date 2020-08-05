using System;

namespace Geo
{
   [Serializable]
   public class BoundingBox2d
   {
      Point2d min;
      Point2d max;

      public Point2d Min { get => min; private set => min = value; }
      public Point2d Max { get => max; private set => max = value; }

      public BoundingBox2d()
      {
         min = new Point2d();
         max = new Point2d();
      }

      public BoundingBox2d(Point2d minPt, Point2d maxPt)
      {
         min = minPt;
         max = maxPt;
      }

      public bool IsContain(ICoordinates pt)
      {
         return false;
      }

   }
}
