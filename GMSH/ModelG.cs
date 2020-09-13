using System.Collections.Generic;

namespace Geo.GMSH
{
   public class ModelG
   {
      public List<PointG> Points { get; private set; }
      public List<LineG> Lines { get; private set; }
      public List<CircleG> Circles { get; private set; }
      public List<LoopG> Loops { get; private set; }
      public List<PlaneSurface> Surfaces { get; private set; }
      public List<PhysicalSurface> PhySurfaces { get; private set; }
      public List<PhysicalCurve> PhyCurves { get; private set; }

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

            case EntityGType.phsurf:
               PhySurfaces.Add((PhysicalSurface)entity);
               break;

            case EntityGType.phcurve:
               PhyCurves.Add((PhysicalCurve)entity);
               break;

            case EntityGType.point:
               Points.Add((PointG)entity);
               break;
         }
      }

      public IEntityG GetEntity(EntityGType type, int id)
      {
         switch (type)
         {
            case EntityGType.point:
               return Points[id];

            case EntityGType.line:
               return Lines[id];

            case EntityGType.arc:
               return Circles[id];

            case EntityGType.loop:
               return Loops[id];

            case EntityGType.surf:
               return Surfaces[id];

            case EntityGType.phsurf:
               return PhySurfaces[id];

            case EntityGType.phcurve:
               return PhyCurves[id];

            default:
               return null;
         }
      }
   }
}