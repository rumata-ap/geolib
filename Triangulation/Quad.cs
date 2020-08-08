using Geo.Calc;

using System;
using System.Linq;
using System.Collections.Generic;

namespace Geo.Triangulation
{
   public class Quad
   {
      private ICoordinates vertex1;
      private ICoordinates vertex2;
      private ICoordinates vertex3;
      private ICoordinates vertex4;

      public ICoordinates Vertex1 { get => vertex1; set { vertex1 = value; CalcQuadrangle(); } }
      public ICoordinates Vertex2 { get => vertex2; set { vertex2 = value; CalcQuadrangle(); } }
      public ICoordinates Vertex3 { get => vertex3; set { vertex3 = value; CalcQuadrangle(); } }
      public ICoordinates Vertex4 { get => vertex4; set { vertex4 = value; CalcQuadrangle(); } }
      public Tria Tri1 { get; private set; }
      public Tria Tri2 { get; private set; }
      public double MaxAngleDeg { get; private set; }
      public double Area { get; private set; }
      public double Xc { get; private set; }
      public double Yc { get; private set; }

      public Quad(Tria trg1, Tria trg2)
      {
         List<ICoordinates> l1 = new List<ICoordinates> { trg1.Vertex1, trg1.Vertex2, trg1.Vertex3 };
         List<ICoordinates> l2 = new List<ICoordinates> { trg2.Vertex1, trg2.Vertex2, trg2.Vertex3 };

         for (int i = 0; i < 3; i++)
         {
            for (int j = 0; j < 3; j++)
            {
               if (l1[i].IsMatch(l2[j])) l2.RemoveAt(j);
            }
         }
         if (l2.Count != 1) throw new ArgumentException("Не возможно создать четырехугольник из заданных треугольников.");
         vertex1 = l1[0];
         vertex2 = l1[1];
         vertex3 = l2[0];
         vertex4 = l1[2];
         Tri1 = trg1;
         Tri2 = trg2;
         CalcQuadrangle();
      }

      public Quad(ICoordinates node1, ICoordinates node2, ICoordinates node3, ICoordinates node4)
      {
         vertex1 = node1;
         vertex2 = node2;
         vertex3 = node3;
         vertex4 = node4;
         Tri1 = new Tria(node1, node2, node3);
         Tri2 = new Tria(node1, node3, node4);
         CalcQuadrangle();
      }

      public void CalcQuadrangle()
      {
         ICoordinates[] verts = new ICoordinates[] { vertex1, vertex2, vertex3, vertex4 };
         var selx =  from v in verts select v.X;
         var sely = from v in verts select v.Y;

         if(selx.Distinct().Count() < 4 && sely.Distinct().Count() < 4)
         {
            throw new ArgumentException("Найдены совпадающие вершины. Не возможно создать цетырехугольник.");
         }

         Polygon2d polygon = new Polygon2d(verts);
         Area = polygon.Area;
         Xc = polygon.Centroid.X;
         Yc = polygon.Centroid.Y;
         var sel = from v in polygon.Vertices orderby v.AngleDeg select v.AngleDeg;
         MaxAngleDeg = sel.Last();
      }

      public bool IsPointIn(ICoordinates pt)
      {
         if (Tri1.IsPointIn(pt) && Tri2.IsPointIn(pt)) return true;
         else return false;
      }
   }
}
