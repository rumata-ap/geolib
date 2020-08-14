using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Geo.GMSH
{
   public class PhysicalCurve : IEntityG
   {
      public object Id { get; set; }
      public EntityGType Type => EntityGType.loop;
      public string GeoString => GetGeoString();
      public IEnumerable<int> CurvIds { get; }
      public IEnumerable<IEntityG> Curvs { get; }

      public PhysicalCurve(string id, IEnumerable<int> ids)
      {
         Id = id;
         CurvIds = ids;
      }

      public PhysicalCurve(string id, IEnumerable<IEntityG> entities)
      {
         Id = id;
         Curvs = entities;
      }

      public PhysicalCurve(string id, params int[] ids)
      {
         Id = id;
         CurvIds = ids;
      }

      string GetGeoString()
      {
         StringBuilder sb = null;
         if (CurvIds != null)
         {
            sb = new StringBuilder($"Physical Curve({Id}) = " + "{");
            foreach (int item in CurvIds)
            {
               sb.Append(item);
               sb.Append(", ");
            }
            sb.Remove(sb.Length - 2, 2);
            sb.Append("};");
         }
         else if (Curvs != null)
         {
            sb = new StringBuilder($"Physical Curve({Id}) = " + "{");
            foreach (IEntityG item in Curvs)
            {
               sb.Append(item.Id);
               sb.Append(", ");
            }
            sb.Remove(sb.Length - 2, 2);
            sb.Append("};");
         }

         return sb.ToString();
      }

   }
}
