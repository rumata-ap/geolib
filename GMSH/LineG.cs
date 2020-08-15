using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Geo.GMSH
{
   public class LineG : Line3d, IEntityG
   {
      public object Id { get; set; }
      public int P1 { get; private set; }
      public int P2 { get; private set; }
      public string GeoString { get => GetGeoString(); }

      public EntityGType Type => EntityGType.line;

      public LineG(int id, PointG pt1, PointG pt2) : base(pt1, pt2)
      {
         Id = id;
         P1 = (int)pt1.Id;
         P2 = (int)pt2.Id;
      }

      string GetGeoString()
      {
         return $"Line({Id}) = " + "{" + $"{P1}, {P2}" + "};";
      }
   }
}
