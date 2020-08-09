using Geo.Calc;

namespace Geo
{
   public interface ICoordinates
   {
      double X { get; set; }
      double Y { get; set; }
      double Z { get; set; }

      bool IsMatch(ICoordinates pt);
      bool NotMath(ICoordinates pt);
      double[] ToArray();
      Vector3d ToVector3d();
      double LengthTo(ICoordinates target);
   }
}