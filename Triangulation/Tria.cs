using System;

namespace Geo.Triangulation
{
   public class Tria
   {
      private ICoordinates vertex1;
      private ICoordinates vertex2;
      private ICoordinates vertex3;

      public double Area { get; private set; }
      public double Xc { get; private set; }
      public double Yc { get; private set; }
      //public double MaxAngleDeg { get; private set; }
      public ICoordinates Vertex1 { get => vertex1; set { vertex1 = value; CalcTriangle(); } }
      public ICoordinates Vertex2 { get => vertex2; set { vertex2 = value; CalcTriangle(); } }
      public ICoordinates Vertex3 { get => vertex3; set { vertex3 = value; CalcTriangle(); } }
         
      public Tria(ICoordinates node1, ICoordinates node2, ICoordinates node3)
      {
         vertex1 = node1;
         vertex2 = node2;
         vertex3 = node3;
         CalcTriangle();
      }

      public void CalcTriangle()
      {
         //double ang1, ang2, ang3;
         //ang1 = vertex1.AngleTo(vertex3, vertex2);
         //ang2 = vertex2.AngleTo(vertex1, vertex3);
         //ang3 = vertex3.AngleTo(vertex2, vertex1);
         //MaxAngleDeg = Math.Max(Math.Max(ang1, ang2), ang3);

         Xc = (vertex1.X + vertex2.X + vertex3.X) / 3;
         Yc = (vertex1.Y + vertex2.Y + vertex3.Y) / 3;
         Area = Math.Abs(0.5 * ((vertex2.X - vertex1.X) * (vertex3.Y - vertex1.Y) - 
            (vertex3.X - vertex1.X) * (vertex2.Y - vertex1.Y)));
      }

      public bool IsPointIn(ICoordinates pt)
      {
         if (pt.IsMatch(vertex1) || pt.IsMatch(vertex2) || pt.IsMatch(vertex3)) return true;

         double bx = vertex2.X - vertex1.X;
         double by = vertex2.Y - vertex1.Y;
         double cx = vertex3.X - vertex1.X;
         double cy = vertex3.Y - vertex1.Y;
         double px = pt.X - vertex1.X;
         double py = pt.Y - vertex1.Y;

         double m = (px * by - bx * py) / (cx * by - bx * cy);
         if (m >= 0 && m <= 1)
         {
            double l = (px - m * cx) / bx;
            if (l > 0 && m + l <= 1) return true;
            else return false;
         }
         else return false;
      }
   }
}
