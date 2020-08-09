using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Geo.Triangulation
{
   public class Domain
   {
      public WorkPlane WorkPlane { get; set; }
      public Polygon2d Out { get; set; }
      public List<Polygon2d> Holes { get; set; }
      public List<Pline2d> Pathes { get; set; }
      public List<Node> Points { get; set; }

      Stack<Polygon2d> work;

      public Mesh Triangulation(double step, ParamType stepType = ParamType.abs)
      {
         return new Mesh();
      }
   }
}
