using System;

namespace Geo
{
   [Serializable]
   public class Vertex2d
   {
      public int Id { get; set; }
      public double Bulge { get; set; }
      public Point2d Point { get; }

      public Vertex2d(Point2d pt, int id= 0, double bulge = 0)
      {
         Id = id;
         Bulge = bulge;
         Point = pt;
      }

      public Vertex2d(Point3d pt, int id = 0, double bulge = 0)
      {
         Id = id;
         Bulge = bulge;
         Point = pt.ToPoint2d();
      }

      public Vertex2d(double x, double y, int id = 0, double bulge = 0)
      {
         Id = id;
         Bulge = bulge;
         Point = new Point2d(x, y);
      }
   }
}
