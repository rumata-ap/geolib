using Geo.Calc;

using System;
using System.Linq;
using System.Collections.Generic;

namespace Geo.Triangulation
{
   public class Quad: ISimplex
   {
      public int Id { get; set; }
      public Domain Domain { get; set; }

      public int A { get; private set; }
      public int B { get; private set; }
      public int C { get; private set; }
      public int D { get; }
      public Edge E1 { get; private set; }
      public Edge E2 { get; private set; }
      public Edge E3 { get; private set; }
      public Edge E4 { get; private set; }

      public SimplexType Type => SimplexType.quad;

      public Quad(int node1, int node2, int node3, int node4)
      {
         A = node1;
         B = node2;
         C = node3;
         D = node4;
         E1 = new Edge(A, B);
         E2 = new Edge(B, C);
         E3 = new Edge(C, D);
         E4 = new Edge(D, A);
      }

      public Quadrangle ToQuadrangle(IEnumerable<Node> stor)
      {
         var v1 = from i in stor where i.Id == A select i;
         var v2 = from i in stor where i.Id == B select i;
         var v3 = from i in stor where i.Id == C select i;
         var v4 = from i in stor where i.Id == D select i;

         return new Quadrangle(v1.First(), v2.First(), v3.First(), v4.First());
      }
   }
}
