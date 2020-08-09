using System;
using System.Linq;
using System.Collections.Generic;

namespace Geo.Triangulation
{
   public class Tri : ISimplex
   {
      public int Id { get; set; }
      public Domain Domain { get; set; }

      public int A { get; private set; }
      public int B { get; private set; }
      public int C { get; private set; }

      public SimplexType Type => SimplexType.tri;

      public Tri(int node1, int node2, int node3)
      {
         A = node1;
         B = node2;
         C = node3;
      }

      public Triangle ToTriangle(IEnumerable<Node> stor)
      {
         var v1 = from i in stor where i.Id == A select i;
         var v2 = from i in stor where i.Id == B select i;
         var v3 = from i in stor where i.Id == C select i;

         return new Triangle(v1.First(), v2.First(), v3.First());
      }
   }
}
