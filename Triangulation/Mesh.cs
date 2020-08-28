using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Geo.Triangulation
{
   public class Mesh
   {
      public List<Node> Nodes { get; set; }
      public List<ISimplex> Simplexs { get; set; }
      public List<Node> Out { get; set; }
      public List<Node> Holes { get; set; }
      public List<Node> Pathes { get; set; }
      public List<Node> Points { get; set; }
      public List<Edge> Edges { get; set; }
      public List<Tri> Tris { get; set; }
      public List<Quad> Quads { get; set; }

      public Mesh()
      {
         Nodes = new List<Node>();
         Simplexs = new List<ISimplex>();
      }

      /// <summary>
      /// Сглаживание треугольной сетки.
      /// </summary>
      /// <param name="number">Число циклов сглаживания.</param>
      public void SmoothTri(int number)
      {
         List<Node> nodes = new List<Node>(Nodes);
         nodes.AddRange(Out);
         for (int i = 0; i < number; i++)
         {
            foreach (Node item in Nodes)
            {
               var sel = from s in Simplexs where ((Tri)s).A == item.Id || ((Tri)s).B == item.Id || ((Tri)s).C == item.Id select s;
               List<ISimplex> simplices = new List<ISimplex>(sel);
               double xc = 0;
               double yc = 0;
               Triangle tria;
               foreach (Tri t in simplices)
               {
                  tria = t.ToTriangle(nodes);
                  xc += tria.Xc;
                  yc += tria.Yc;
               }

               item.X = xc / simplices.Count;
               item.Y = yc / simplices.Count;
            }
         }
      }

      public void Recombine()
      {
         Tris = new List<Tri>(Simplexs.Count);
         Quads = new List<Quad>();
         foreach (ISimplex item in Simplexs) Tris.Add((Tri)item);
         List<Tri> tempTris = new List<Tri>(Simplexs.Count);
         foreach (Tri item in Tris)
         {
            Quads.Add(QuadMinA(item, out Tri del));           
            Simplexs.Remove(item);
            Simplexs.Remove(del);
            Simplexs.Add(Quads.Last());
         }
      }

      public void SmoothQuad(int number)
      {
         List<Node> nodes = new List<Node>(Nodes);
         nodes.AddRange(Out);
         for (int i = 0; i < number; i++)
         {
            foreach (Node item in Nodes)
            {
               List<Tri> selT = (from t in Tris where t.A == item.Id || t.B == item.Id || t.C == item.Id select t).ToList();
               List<Quad> selQ = (from q in Quads where q.A == item.Id || q.B == item.Id || q.C == item.Id || q.D == item.Id select q).ToList();
               double xc = 0;
               double yc = 0;
               Triangle tria;              
               foreach (Tri t in selT)
               {
                  tria = t.ToTriangle(nodes);
                  xc += tria.Xc;
                  yc += tria.Yc;
               }
               Quadrangle quadr;
               foreach (Quad q in selQ)
               {
                  quadr = q.ToQuadrangle(nodes);
                  xc += quadr.Xc;
                  yc += quadr.Yc;
               }

               item.X = xc / (selT.Count + selQ.Count);
               item.Y = yc / (selT.Count + selQ.Count);
            }
         }
      }

      Quad QuadMinA(Tri tri, out Tri neighbor)
      {
         List<Tri> neighbors = (from t in Tris where IsNeighbor(tri, t) select t).ToList();
         Triangle t1 = tri.ToTriangle(Nodes);
         //List<Triangle> neighborsT = new List<Triangle>(neighbors.Count);
         List<Quadrangle> neighborsQ = new List<Quadrangle>(neighbors.Count);
         foreach (Tri item in neighbors) neighborsQ.Add(new Quadrangle(t1, item.ToTriangle(Nodes)) { Id = item.Id });
         List<Quadrangle> sortQ = (from q in neighborsQ orderby q.MaxAngleDeg select q).ToList();
         Quadrangle q1 = sortQ[0];
         neighbor = null;
         foreach (Tri item in neighbors) if (item.Id == q1.Id) neighbor = item;

         return new Quad(((Node)q1.Vertex1).Id, ((Node)q1.Vertex2).Id, ((Node)q1.Vertex3).Id, ((Node)q1.Vertex4).Id);
      }

      bool IsNeighbor(Tri t, Tri o)
      {
         if ((t.E1.IsMatch(o.E1) || t.E1.IsMatch(o.E2) || t.E1.IsMatch(o.E3)) ||
            (t.E2.IsMatch(o.E1) || t.E2.IsMatch(o.E2) || t.E2.IsMatch(o.E3)) ||
            (t.E3.IsMatch(o.E1) || t.E3.IsMatch(o.E2) || t.E3.IsMatch(o.E3)))
         {
            return true;
         }
         else return false;
      }

   }
}
