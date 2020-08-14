using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Geo.GMSH
{
   public class PhysicalCurve : IEntityG
   {
      public int Id { get; set; }

      public EntityGType Type => throw new NotImplementedException();

      public string GeoString => throw new NotImplementedException();
   }
}
