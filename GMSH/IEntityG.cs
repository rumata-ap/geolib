namespace Geo.GMSH
{
   public interface IEntityG
   {
      int Id { get; set; }
      EntityGType Type { get; }
      string GeoString { get; }
   }

   public enum EntityGType { line, arc, loop, surf, point, phcurve, phsurf}
}