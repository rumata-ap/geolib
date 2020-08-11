using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Geo.GMSH
{
   public class ModelG
   {
      public List<PointG> Points { get; private set; }
      public List<LineG> Lines { get; private set; }
      public List<CircleG> Circles { get; private set; }
      public List<LoopG> Loops { get; private set; }
      public List<PlaneSurface> Surfaces { get; private set; }

      public ModelG()
      {
         Points = new List<PointG>();
         Lines = new List<LineG>();
         Circles = new List<CircleG>();
         Loops = new List<LoopG>();
         Surfaces = new List<PlaneSurface>();
      }

      public void AddEntity(IEntityG entity)
      {
         switch (entity.Type)
         {
            case EntityGType.line:
               Lines.Add((LineG)entity);
               break;
            case EntityGType.arc:
               Circles.Add((CircleG)entity);
               break;
            case EntityGType.loop:
               Loops.Add((LoopG)entity);
               break;
            case EntityGType.surf:
               Surfaces.Add((PlaneSurface)entity);
               break;
            case EntityGType.point:
               Points.Add((PointG)entity);
               break;
         }
      }

      public void AddPointRange(IEnumerable<IEntityG> pts)
      {
         List<IEntityG> entities = pts.ToList();
         foreach (IEntityG item in entities) AddEntity(item);
      }
      
   }
}
