using Geo.Triangulation;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Geo
{
   public class Quadrangle
   {
      public int Id { get; set; }

      private IXYZ vertex1;
      private IXYZ vertex2;
      private IXYZ vertex3;
      private IXYZ vertex4;

      public IXYZ Vertex1 { get => vertex1; set { vertex1 = value; CalcQuadrangle(); } }
      public IXYZ Vertex2 { get => vertex2; set { vertex2 = value; CalcQuadrangle(); } }
      public IXYZ Vertex3 { get => vertex3; set { vertex3 = value; CalcQuadrangle(); } }
      public IXYZ Vertex4 { get => vertex4; set { vertex4 = value; CalcQuadrangle(); } }
      public Triangle Tri1 { get; private set; }
      public Triangle Tri2 { get; private set; }
      public double MaxAngleDeg { get; private set; }
      public double Area { get; private set; }
      public double Xc { get; private set; }
      public double Yc { get; private set; }

      public Quadrangle(Triangle trg1, Triangle trg2)
      {
         List<IXYZ> l1 = new List<IXYZ> { trg1.Vertex1, trg1.Vertex2, trg1.Vertex3 };
         List<IXYZ> l2 = new List<IXYZ> { trg2.Vertex1, trg2.Vertex2, trg2.Vertex3 };
         List<IXYZ> pt = new List<IXYZ>(2);
         for (int i = 0; i < 3; i++)
         {
            for (int j = 0; j < 3; j++)
            {
               if (l1[i].IsMatch(l2[j])) pt.Add(l2[j]);
            }
         }
         l2.Remove(pt[0]);
         l2.Remove(pt[1]);
         
         if (l2.Count != 1) throw new ArgumentException("Не возможно создать четырехугольник из заданных треугольников.");
         l1.Add(l2[0]);
         List<IXYZ> selx = l1.OrderBy(t => t.X).ToList();         
         vertex1 = selx[0];
         vertex3 = selx[3];
         selx.Remove(vertex1);
         selx.Remove(vertex3);
         List<IXYZ> sely = selx.OrderByDescending(t => t.Y).ToList();
         vertex2 = sely[0];        
         vertex4 = sely[1];
         Line2d lin2 = new Line2d(new Point2d(vertex2.ToArray()), new Point2d(vertex3.ToArray()));
         Line2d lin4 = new Line2d(new Point2d(vertex4.ToArray()), new Point2d(vertex1.ToArray()));
         Line2d lin1 = new Line2d(new Point2d(vertex1.ToArray()), new Point2d(vertex2.ToArray()));
         Line2d lin3 = new Line2d(new Point2d(vertex3.ToArray()), new Point2d(vertex4.ToArray()));
         lin2.IntersectionSegments(lin4, out IntersectResult intersect);
         lin1.IntersectionSegments(lin3, out IntersectResult intersect1);
         if (intersect.res)
         {
            vertex4 = vertex3;
            vertex3 = sely[1];
         }
         if (intersect1.res)
         {
            vertex2 = vertex3;
            vertex3 = sely[0];
         }
         Tri1 = trg1;
         Tri2 = trg2;
         CalcQuadrangle();
      }

      public Quadrangle(IXYZ node1, IXYZ node2, IXYZ node3, IXYZ node4)
      {
         vertex1 = node1;
         vertex2 = node2;
         vertex3 = node3;
         vertex4 = node4;
         Tri1 = new Triangle(node1, node2, node3);
         Tri2 = new Triangle(node1, node3, node4);
         CalcQuadrangle();
      }

      public void CalcQuadrangle()
      {
         IXYZ[] verts = new IXYZ[] { vertex1, vertex2, vertex3, vertex4 };
         //var selx = from v in verts select v.X;
         //var sely = from v in verts select v.Y;

         //if (selx.Distinct().Count() < 4 && sely.Distinct().Count() < 4)
         //{
         //   throw new ArgumentException("Найдены совпадающие вершины. Не возможно создать цетырехугольник.");
         //}

         Polygon2d polygon = new Polygon2d(verts);
         if (!polygon.IsClockwise()) polygon.Inverse();
         Area = polygon.Area;
         Xc = polygon.Centroid.X;
         Yc = polygon.Centroid.Y;
         MaxAngleDeg = polygon.MaxAngleDeg();
      }

      public bool IsPointIn(IXYZ pt)
      {
         if (Tri1.IsPointIn(pt) && Tri2.IsPointIn(pt)) return true;
         else return false;
      }
   }
}
