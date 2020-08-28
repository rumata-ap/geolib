using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Geo.Triangulation
{
   public class Recombiner
   {
      public List<Node> NodeSource { get; set; }
      public List<ISimplex> TriSource { get; set; }
      public List<Tri> Tris { get; set; }
      public List<Quad> Quads { get; set; }


      public void Recombine()
      {
         Tris = new List<Tri>(TriSource.Count);       
         Quads = new List<Quad>();       
         foreach (ISimplex item in TriSource) Tris.Add((Tri)item);
         List<Tri> tempTris = new List<Tri>(TriSource.Count);
         foreach (Tri item in tempTris)
         {
            Quads.Add(QuadMinA(item, out Tri del));
            Tris.Remove(item);
            Tris.Remove(del);
         }
      }

      Quad QuadMinA(Tri tri, out Tri neighbor)
      {
         List<Tri> neighbors = (from t in Tris where IsNeighbor(tri, t) select t).ToList();
         Triangle t1 = tri.ToTriangle(NodeSource);
         //List<Triangle> neighborsT = new List<Triangle>(neighbors.Count);
         List<Quadrangle> neighborsQ = new List<Quadrangle>(neighbors.Count);
         foreach (Tri item in neighbors) neighborsQ.Add(new Quadrangle(t1, item.ToTriangle(NodeSource)) { Id = item.Id });
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
