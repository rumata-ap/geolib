using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Geo.Triangulation
{
   public class Edge
   {
      public int A { get; set; }
      public int B { get; set; }

      public Edge(int a, int b)
      {
         A = a;
         B = b;
      }

      public bool IsMatch(Edge e)
      {
         return (A == e.A && B == e.B) || (A == e.B && B == e.A);
      }
   }
}
