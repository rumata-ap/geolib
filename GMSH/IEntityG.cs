namespace Geo.GMSH
{
   public interface IEntityG
   {
      object Id { get; set; }
      EntityGType Type { get; }
      string GeoString { get; }
   }

   public enum EntityGType { line, arc, loop, surf, point, phcurve, phsurf}
}