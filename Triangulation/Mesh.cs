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

      public Mesh()
      {
         Nodes = new List<Node>();
         Simplexs = new List<ISimplex>();
      }
   }
}
