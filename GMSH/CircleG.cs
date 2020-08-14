﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Geo.GMSH
{
   public class CircleG : IEntityG
   {
      public int Id { get; set; }
      public EntityGType Type => EntityGType.arc;
      public string GeoString => GetGeoString();
      public int P1 { get; private set; }
      public int P2 { get; private set; }
      public int P3 { get; private set; }

      public CircleG(PointG pt1, PointG pt2, PointG pt3, int id = 0)
      {
         Id = id;
         P1 = pt1.Id;
         P2 = pt2.Id;
         P3 = pt3.Id;
      }
      
      public CircleG(int pt1, int pt2, int pt3, int id = 0)
      {
         Id = id;
         P1 = pt1;
         P2 = pt2;
         P3 = pt3;
      }
            
      public CircleG(Arc2d arc, int pt1, int pt2, int idpt3, out PointG pt3, int id = 0)
      {
         Id = id;
         P1 = pt1;
         P2 = pt2;
         pt3 = new PointG(arc.Center, idpt3);
         P3 = idpt3;
      }

      string GetGeoString()
      {
         return $"Line({Id}) = " + "{" + $"{P1}, {P2}" + "};";
      }
   }
}
