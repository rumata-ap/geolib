namespace Geo.Triangulation
{
   public interface ISimplex
   {
      SimplexType Type { get; }
      Domain Domain { get; set; }
      int Id { get; set; }
   }

   public enum SimplexType
   {
      tri,
      quad
   }
}