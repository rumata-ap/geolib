using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Geo.GMSH
{
   public class PhysicalSurface : IEntityG
   {
      public object Id { get; set; }
      public EntityGType Type => EntityGType.loop;
      public string GeoString => GetGeoString();
      public IEnumerable<int> SurfIds { get; }
      public IEnumerable<IEntityG> Surfs { get; }

      public PhysicalSurface(string id, IEnumerable<int> ids)
      {
         Id = id;
         SurfIds = ids;
      }

      public PhysicalSurface(string id, IEnumerable<IEntityG> entities)
      {
         Id = id;
         Surfs = entities;
      }

      public PhysicalSurface(string id, params int[] ids)
      {
         Id = id;
         SurfIds = ids;
      }

      string GetGeoString()
      {
         StringBuilder sb = null;
         if (SurfIds != null)
         {
            sb = new StringBuilder($"Physical Surface({Id}) = " + "{");
            foreach (int item in SurfIds)
            {
               sb.Append(item);
               sb.Append(", ");
            }
            sb.Remove(sb.Length - 2, 2);
            sb.Append("};");
         }
         else if (Surfs != null)
         {
            sb = new StringBuilder($"Physical Surface({Id}) = " + "{");
            foreach (IEntityG item in Surfs)
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
