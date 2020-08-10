using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Geo.GMSH
{
   public class PointG : Point3d, ICoordinates
   {
      double step;
      string description;

      public PointG(double x, double y, double z)
      {

      }
   }
}
