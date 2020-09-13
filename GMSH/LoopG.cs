using System.Collections.Generic;
using System.Text;

namespace Geo.GMSH
{
   public class LoopG : IEntityG
   {
      public object Id { get; set; }
      public EntityGType Type => EntityGType.loop;
      public string GeoString => GetGeoString();
      public IEnumerable<int> CurvIds { get; }
      public IEnumerable<IEntityG> Curvs { get; }

      public LoopG(int id, IEnumerable<int> ids)
      {
         Id = id;
         CurvIds = ids;
      }

      public LoopG(int id, IEnumerable<IEntityG> entities)
      {
         Id = id;
         Curvs = entities;
      }

      public LoopG(int id, params int[] ids)
      {
         Id = id;
         CurvIds = ids;
      }

      private string GetGeoString()
      {
         StringBuilder sb = null;
         if (CurvIds != null)
         {
            sb = new StringBuilder($"Curve Loop({Id}) = " + "{");
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
            sb = new StringBuilder($"Curve Loop({Id}) = " + "{");
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