using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Geo.GMSH
{
   public class LoopG : IEntityG
   {
      public int Id { get; set; }
      public EntityGType Type => EntityGType.loop;
      public string GeoString => GetGeoString();
      public IEnumerable<int> CurvIds { get; }

      public LoopG(IEnumerable<int> ids, int id = 0)
      {
         Id = id;
         CurvIds = ids;
      }

      public LoopG(int id, params int[] ids)
      {
         Id = id;
         CurvIds = ids;
      }

      string GetGeoString()
      {
         StringBuilder sb = new StringBuilder($"Curve Loop({Id}) = " + "{");
         foreach (int item in CurvIds)
         {
            sb.Append(item);
            sb.Append(", ");
         }
         sb.Remove(sb.Length - 2, 2);
         sb.Append("};");

         return sb.ToString();
      }
   }
}
