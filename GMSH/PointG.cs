using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Geo.GMSH
{
   public class PointG : Point3d, ICoordinates, IEntityG
   {
      public object Id { get; set; }
      public string GeoString { get => GetGeoString(); }
      public double Step { get; set; }
      public EntityGType Type => EntityGType.point;

      public PointG(double x, double y, double z, int id = 0, double st = 1000) : base(x, y, z)
      {
         Step = st;
         Id = id;
      }

      public PointG(ICoordinates src, int id = 0, double st = 1000): base(src)
      {
         Step = st;
         Id = id;
      }

      string GetGeoString()
      {
         return $"Point({Id}) = " + "{" + $"{X}, {Y}, {Z}, {Step}" + "};";
      }
   }
}
