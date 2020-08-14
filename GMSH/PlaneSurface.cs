using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Geo.GMSH
{
   public class PlaneSurface : IEntityG
   {
      public object Id { get; set; }
      public EntityGType Type => EntityGType.loop;
      public string GeoString => GetGeoString();
      public IEnumerable<int> CurvIds { get; }
      public IEnumerable<IEntityG> Loops { get; }

      public PlaneSurface(IEnumerable<int> ids, int id = 0)
      {
         Id = id;
         CurvIds = ids;
      }
      
      public PlaneSurface(int id, IEnumerable<IEntityG> entities)
      {
         Id = id;
         Loops = entities;
      }

      public PlaneSurface(int id, params int[] ids)
      {
         Id = id;
         CurvIds = ids;
      }

      string GetGeoString()
      {
         StringBuilder sb = null;
         if (CurvIds!=null)
         {
            sb = new StringBuilder($"Plane Surface({Id}) = " + "{");
            foreach (int item in CurvIds)
            {
               sb.Append(item);
               sb.Append(", ");
            }
            sb.Remove(sb.Length - 2, 2);
            sb.Append("};");
         }
         else if (Loops!=null)
         {
            sb = new StringBuilder($"Plane Surface({Id}) = " + "{");
            foreach (IEntityG item in Loops)
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
