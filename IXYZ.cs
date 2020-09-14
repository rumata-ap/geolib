using Geo.Calc;

namespace Geo
{
   public interface IXYZ
   {
      double X { get; set; }
      double Y { get; set; }
      double Z { get; set; }

      bool IsMatch(IXYZ pt);
      bool IsNaN();
      double[] ToArray();
      Vector3d ToVector3d();
      double DistanceTo(IXYZ target);
      double DistanceSquaredTo(IXYZ target);
   }
}