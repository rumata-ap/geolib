using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;

namespace Geo.Calc
{
   /// <summary>
   /// Служебные математические функции и константы.
   /// </summary>
   public static class Calcs
   {
      #region constants

      /// <summary>
      /// Константа для преобразования угла между градусами и радианами.
      /// </summary>
      public const double DegToRad = Math.PI / 180.0;

      /// <summary>
      /// Константа для преобразования угла между градусами и радианами.
      /// </summary>
      public const double RadToDeg = 180.0 / Math.PI;

      /// <summary>
      /// Константа для преобразования угла между градусами и градами.
      /// </summary>
      public const double DegToGrad = 10.0 / 9.0;

      /// <summary>
      /// Константа для преобразования угла между градусами и градами.
      /// </summary>
      public const double GradToDeg = 9.0 / 10.0;

      /// <summary>
      /// PI/2 (90 degrees)
      /// </summary>
      public const double HalfPI = Math.PI * 0.5;

      /// <summary>
      /// PI (180 degrees)
      /// </summary>
      public const double PI = Math.PI;

      /// <summary>
      /// 3*PI/2 (270 degrees)
      /// </summary>
      public const double ThreeHalfPI = 3 * Math.PI * 0.5;

      /// <summary>
      /// 2*PI (360 degrees)
      /// </summary>
      public const double TwoPI = 2 * Math.PI;

      #endregion

      #region public properties

      private static double epsilon = 1e-12;

      /// <summary>
      /// Представляет наименьшее число, используемое для сравнения.
      /// </summary>
      /// <remarks>
      /// Значение epsilon должно быть положительным числом больше нуля.
      /// </remarks>
      public static double Epsilon
      {
         get { return epsilon; }
         set
         {
            if (value <= 0.0)
               throw new ArgumentOutOfRangeException(nameof(value), value, "Значение epsilon должно быть положительным числом больше нуля.");
            epsilon = value;
         }
      }

      #endregion

      #region static methods

      /// <summary>
      /// Возвращает значение, указывающее знак числа двойной точности с плавающей запятой.
      /// </summary>
      /// <param name="number">Число двойной точности.
      /// </param>
      /// <returns>
      /// Число, обозначающее знак значения.
      /// -1 значение меньше.
      /// 0 значение равно нулю.
      /// 1 значение больше нуля.
      /// </returns>
      /// <remarks>Этот метод проверяет значения чисел, очень близких к нулю.</remarks>
      public static int Sign(double number)
      {
         return IsZero(number) ? 0 : Math.Sign(number);
      }

      /// <summary>
      /// Возвращает значение, указывающее знак числа двойной точности с плавающей запятой.
      /// </summary>
      /// <param name="number">Число двойной точности.
      /// <param name="threshold">Допуск.</param>
      /// </param>
      /// <returns>
      /// A number that indicates the sign of value.
      /// Return value Meaning -1 value is less than zero.
      /// 0 value is equal to zero.
      /// 1 value is greater than zero.
      /// </returns>
      /// <remarks>Этот метод проверяет значения чисел, очень близких к нулю.</remarks>
      public static int Sign(double number, double threshold)
      {
         return IsZero(number, threshold) ? 0 : Math.Sign(number);
      }

      /// <summary>
      /// Проверяет, близко ли число к единице.
      /// </summary>
      /// <param name="number">Число двойной точности.</param>
      /// <returns>True if its close to one or false in any other case.</returns>
      public static bool IsOne(double number)
      {
         return IsOne(number, Epsilon);
      }

      /// <summary>
      /// Проверяет, близко ли число к единице.
      /// </summary>
      /// <param name="number">Число двойной точности.</param>
      /// <param name="threshold">Допуск.</param>
      /// <returns>True, если оно близко к единице, или false в любом другом случае.</returns>
      public static bool IsOne(double number, double threshold)
      {
         return IsZero(number - 1, threshold);
      }

      /// <summary>
      /// Проверяет, близко ли число к нулю.
      /// </summary>
      /// <param name="number">Double precision number.</param>
      /// <returns>True if its close to one or false in any other case.</returns>
      public static bool IsZero(double number)
      {
         return IsZero(number, Epsilon);
      }

      /// <summary>
      /// Проверяет, близко ли число к нулю.
      /// </summary>
      /// <param name="number">Double precision number.</param>
      /// <param name="threshold">Tolerance.</param>
      /// <returns>True if its close to one or false in any other case.</returns>
      public static bool IsZero(double number, double threshold)
      {
         return number >= -threshold && number <= threshold;
      }

      /// <summary>
      /// Проверяет, близость координат точечного объекта к нулю.
      /// </summary>
      /// <param name="point">Координаты точечного объекта.</param>
      /// <returns>True if its close to one or false in any other case.</returns>
      public static bool IsZero(ICoordinates point)
      {
         return IsZero(point, Epsilon);
      }

      /// <summary>
      /// Проверяет, близость координат точечного объекта к нулю.
      /// </summary>
      /// <param name="point">Координаты точечного объекта.</param>
      /// <param name="threshold">Допуск.</param>
      /// <returns>True if its close to one or false in any other case.</returns>
      public static bool IsZero(ICoordinates point, double threshold)
      {
         return point.X >= -threshold && point.X <= threshold &&
                point.Y >= -threshold && point.Y <= threshold &&
                point.Z >= -threshold && point.Z <= threshold;
      }

      /// <summary>
      /// Проверяет, совпадает ли одно число с другим.
      /// </summary>
      /// <param name="a">Double precision number.</param>
      /// <param name="b">Double precision number.</param>
      /// <returns>True if its close to one or false in any other case.</returns>
      public static bool IsEqual(double a, double b)
      {
         return IsEqual(a, b, Epsilon);
      }

      /// <summary>
      /// Проверяет, совпадает ли одно число с другим.
      /// </summary>
      /// <param name="a">Число двойной точности.</param>
      /// <param name="b">Число двойной точности.</param>
      /// <param name="threshold">Допуск.</param>
      /// <returns>True if its close to one or false in any other case.</returns>
      public static bool IsEqual(double a, double b, double threshold)
      {
         return IsZero(a - b, threshold);
      }

      /// <summary>
      /// Проверяет, совпадают ли координаты точечных объектов.
      /// </summary>
      /// <param name="a">Координаты точечного объекта.</param>
      /// <param name="b">Координаты точечного объекта.</param>
      /// <returns>True if its close to one or false in any other case.</returns>
      public static bool IsEqual(ICoordinates a, ICoordinates b)
      {
         return IsEqual(a, b, Epsilon);
      }

      /// <summary>
      /// Проверяет, совпадают ли координаты точечных объектов.
      /// </summary>
      /// <param name="a">Координаты точечного объекта.</param>
      /// <param name="b">Координаты точечного объекта.</param>
      /// <param name="threshold">Tolerance.</param>
      /// <returns>True if its close to one or false in any other case.</returns>
      public static bool IsEqual(ICoordinates a, ICoordinates b, double threshold)
      {
         return IsZero((a.ToVector3d() - b.ToVector3d()).ToPoint3d(), threshold);
      }

      /// <summary>
      /// Преобразует точку между системами координат.
      /// </summary>
      /// <param name="point">Преобразуемая точка.</param>
      /// <param name="rotation">Угол вращения в радианах.</param>
      /// <param name="from">Исходная система координат точки.</param>
      /// <param name="to">Система координат в которую преобразовывается точка.</param>
      /// <returns>Преобразованноя точка.</returns>
      public static Vector2d Transform(Vector2d point, double rotation, CoordinateSystem from, CoordinateSystem to)
      {
         // если поворот равен 0, преобразование не требуется, матрица преобразования является идентичной
         if (IsZero(rotation))
            return point;

         double sin = Math.Sin(rotation);
         double cos = Math.Cos(rotation);
         if (from == CoordinateSystem.World && to == CoordinateSystem.Object)
         {
            return new Vector2d(point.Vx * cos + point.Vy * sin, -point.Vx * sin + point.Vy * cos);
         }
         if (from == CoordinateSystem.Object && to == CoordinateSystem.World)
         {
            return new Vector2d(point.Vx * cos - point.Vy * sin, point.Vx * sin + point.Vy * cos);
         }
         return point;
      }

      /// <summary>
      /// Transforms a point list between coordinate systems.
      /// </summary>
      /// <param name="points">Point list to transform.</param>
      /// <param name="rotation">Rotation angle in radians.</param>
      /// <param name="from">Point coordinate system.</param>
      /// <param name="to">Coordinate system of the transformed point.</param>
      /// <returns>Transformed point list.</returns>
      public static List<Vector2d> Transform(IEnumerable<Vector2d> points, double rotation, CoordinateSystem from, CoordinateSystem to)
      {
         if (points == null)
            throw new ArgumentNullException(nameof(points));

         //  если поворот равен 0, преобразование не требуется, матрица преобразования является идентичной
         if (IsZero(rotation))
            return new List<Vector2d>(points);

         double sin = Math.Sin(rotation);
         double cos = Math.Cos(rotation);

         List<Vector2d> transPoints;
         if (from == CoordinateSystem.World && to == CoordinateSystem.Object)
         {
            transPoints = new List<Vector2d>();
            foreach (Vector2d p in points)
            {
               transPoints.Add(new Vector2d(p.Vx * cos + p.Vy * sin, -p.Vx * sin + p.Vy * cos));
            }
            return transPoints;
         }
         if (from == CoordinateSystem.Object && to == CoordinateSystem.World)
         {
            transPoints = new List<Vector2d>();
            foreach (Vector2d p in points)
            {
               transPoints.Add(new Vector2d(p.Vx * cos - p.Vy * sin, p.Vx * sin + p.Vy * cos));
            }
            return transPoints;
         }
         return new List<Vector2d>(points);
      }

      /// <summary>
      /// Transforms a point between coordinate systems.
      /// </summary>
      /// <param name="point">Point to transform.</param>
      /// <param name="zAxis">Object normal vector.</param>
      /// <param name="from">Point coordinate system.</param>
      /// <param name="to">Coordinate system of the transformed point.</param>
      /// <returns>Transformed point.</returns>
      public static Point3d Transform(ICoordinates point, Vector3d zAxis, CoordinateSystem from, CoordinateSystem to)
      {
         // if the normal is (0,0,1) no transformation is needed the transformation matrix is the identity
         if (zAxis.Equals(Vector3d.UnitZ))
            return new Point3d(point);

         Matrix trans = ArbitraryAxis(zAxis);
         if (from == CoordinateSystem.World && to == CoordinateSystem.Object)
         {
            trans = trans.Transpose();
            return (trans * point.ToVector3d()).ToPoint3d();
         }
         if (from == CoordinateSystem.Object && to == CoordinateSystem.World)
         {
            return (trans * point.ToVector3d()).ToPoint3d();
         }
         return new Point3d(point);
      }

      /// <summary>
      /// Преобразует список точек между системами координат.
      /// </summary>
      /// <param name="points">Точки для преобразования.</param>
      /// <param name="zAxis">Object normal vector.</param>
      /// <param name="from">Points coordinate system.</param>
      /// <param name="to">Coordinate system of the transformed points.</param>
      /// <returns>Transformed point list.</returns>
      public static List<ICoordinates> Transform(IEnumerable<ICoordinates> points, Vector3d zAxis, CoordinateSystem from, CoordinateSystem to)
      {
         if (points == null)
            throw new ArgumentNullException(nameof(points));

         if (zAxis.Equals(Vector3d.UnitZ)) return new List<ICoordinates>(points);

         Matrix trans = ArbitraryAxis(zAxis);
         List<ICoordinates> transPoints;
         if (from == CoordinateSystem.World && to == CoordinateSystem.Object)
         {
            transPoints = new List<ICoordinates>();
            trans = trans.Transpose();
            foreach (ICoordinates p in points) transPoints.Add((trans * p.ToVector3d()).ToPoint3d());
            return transPoints;
         }
         if (from == CoordinateSystem.Object && to == CoordinateSystem.World)
         {
            transPoints = new List<ICoordinates>();
            foreach (ICoordinates p in points) transPoints.Add((trans * p.ToVector3d()).ToPoint3d());
            return transPoints;
         }

         return new List<ICoordinates>(points);
      }

      /// <summary>
      /// Получает матрицу вращения из вектора нормали (направления выдавливания) объекта.
      /// </summary>
      /// <param name="zAxis">Вектор нормали.</param>
      /// <returns>Матрица вращения.</returns>
      public static Matrix ArbitraryAxis(Vector3d zAxis)
      {
         zAxis = new Vector3d(zAxis.Unit);

         if (zAxis.Equals(Vector3d.UnitZ)) return Matrix.Identity;

         Vector3d wY = Vector3d.UnitY;
         Vector3d wZ = Vector3d.UnitZ;
         Vector3d aX;

         if ((Math.Abs(zAxis.Vx) < 1 / 64.0) && (Math.Abs(zAxis.Vy) < 1 / 64.0))
         {
            aX = wY ^ zAxis;
         }
         else
         {
            aX = wZ ^ zAxis;
         }

         aX = new Vector3d(aX.Unit);

         Vector3d aY = zAxis ^ aX;
         aY = new Vector3d(aY.Unit);

         return new Matrix(new double[,] { { aX.Vx, aY.Vx, zAxis.Vx }, { aX.Vy, aY.Vy, zAxis.Vy }, { aX.Vz, aY.Vz, zAxis.Vz } });
      }

      /// <summary>
      /// Вычисляет минимальное расстояние между точкой и линией.
      /// </summary>
      /// <param name="p">Точка.</param>
      /// <param name="origin">Точка, определяющая линию.</param>
      /// <param name="dir">Направляющий вектор линии.</param>
      /// <returns>Минимальное расстояние между точкой и линией.</returns>
      public static double PointLineDistance(ICoordinates p, ICoordinates origin, Vector3d dir)
      {
         double t = dir % (p.ToVector3d() - origin.ToVector3d());
         Vector3d pPrime = origin.ToVector3d() + dir * t;
         Vector3d vec = p.ToVector3d() - pPrime;
         double distanceSquared = vec.Norma;
         return Math.Sqrt(distanceSquared);
      }

      /// <summary>
      /// Проверяет, находится ли точка внутри области, определяемой линейным сегментом.
      /// </summary>
      /// <param name="p">Точка.</param>
      /// <param name="start">Начальная точка сегмента.</param>
      /// <param name="end">Конечная точка сегмента.</param>
      /// <returns>
      /// 0, если точка находится внутри сегмента, 
      /// 1, если точка находится после конечной точки, и 
      /// -1, если точка находится перед начальной точкой.</returns>
      /// <remarks>
      /// Для целей тестирования точка считается внутри сегмента, 
      /// если она попадает в область от начала до конца сегмента, которая простирается бесконечно перпендикулярно его направлению.
      /// Позже, если необходимо, вы можете использовать метод PointLineDistance для определеня расстояния от точки до сегмента. 
      /// Если это расстояние равно нулю, то точка находится вдоль линии, определяемой начальной и конечной точками.
      /// </remarks>
      public static int PointInSegment(ICoordinates p, ICoordinates start, ICoordinates end)
      {
         Vector3d dir = end.ToVector3d() - start.ToVector3d();
         Vector3d pPrime = p.ToVector3d() - start.ToVector3d();
         double t = dir % pPrime;
         if (t < 0)
         {
            return -1;
         }
         double dot = dir % dir;
         if (t > dot)
         {
            return 1;
         }
         return 0;
      }
      
      /// <summary>
      /// Проверяет, находится ли точка внутри области, определяемой линейным сегментом.
      /// </summary>
      /// <param name="p">Точка.</param>
      /// <param name="start">Начальная точка сегмента.</param>
      /// <param name="end">Конечная точка сегмента.</param>
      /// <returns>
      /// 0, если точка находится внутри сегмента, 
      /// 1, если точка находится после конечной точки, и 
      /// -1, если точка находится перед начальной точкой.</returns>
      /// <remarks>
      /// Для целей тестирования точка считается внутри сегмента, 
      /// если она попадает в область от начала до конца сегмента, которая простирается бесконечно перпендикулярно его направлению.
      /// Позже, если необходимо, вы можете использовать метод PointLineDistance для определеня расстояния от точки до сегмента. 
      /// Если это расстояние равно нулю, то точка находится вдоль линии, определяемой начальной и конечной точками.
      /// </remarks>
      public static int PointInSegmentNoBounds(ICoordinates p, ICoordinates start, ICoordinates end)
      {
         Vector3d dir = end.ToVector3d() - start.ToVector3d();
         Vector3d pPrime = p.ToVector3d() - start.ToVector3d();
         double t = dir % pPrime;
         if (t < 0 || IsZero(t)) return -1;

         double dot = dir % dir;
         if (t > dot || IsEqual(t, dot)) return 1;

         return 0;
      }

      /// <summary>
      /// Вычисление точки прересечения двух плоских линий.
      /// </summary>
      /// <param name="l1">Первая линия.</param>
      /// <param name="l2">Вторая линия.</param>
      /// <returns>Точка прересечения двух плоских линий.</returns>
      public static Point2d FindIntersection(Line2d l1, Line2d l2)
      {
         return FindIntersection(l1.StartPoint.ToPoint2d(), l1.Directive, l2.StartPoint.ToPoint2d(), l2.Directive, Epsilon);
      }

      /// <summary>
      /// Вычисление точки прересечения двух плоских линий.
      /// </summary>
      /// <param name="point0">Точка определяющая превую линию.</param>
      /// <param name="dir0">Направляющий вектор первой линии.</param>
      /// <param name="point1">Точка определяющая вторую линию.</param>
      /// <param name="dir1">Направляющий вектор второй линии.</param>
      /// <param name="threshold">Допуск.</param>
      /// <returns>Точка прересечения двух плоских линий.</returns>
      public static Point2d FindIntersection(Point2d point0, Vector2d dir0, Point2d point1, Vector2d dir1, double threshold)
      {
         // Проверка на параллельность.
         if (Vector2d.AreParallel(dir0, dir1, threshold))
            return new Point2d(double.NaN, double.NaN);

         // линии не параллельны
         Vector2d vect = point1 - point0;
         double cross = dir0 % dir1;
         double s = (vect.Vx * dir1.Vy - vect.Vy * dir1.Vx) / cross;
         return (point0.ToVector2d() + dir0 * s).ToPoint2d();
      }

      /// <summary>
      /// Вычисление пересечения линии и сферы.
      /// </summary>
      /// <param name="p1">Начальная точка линии.</param>
      /// <param name="p2">Конечная точка линии.</param>
      /// <param name="sc">Центр сферы.</param>
      /// <param name="r">Радиус сферы.</param>
      /// <param name="res1">Первая точка пересечения.</param>
      /// <param name="res2">Вторая точка пересечения.</param>
      /// <remarks> 
      ///  Отрезок определяется от p1 до p2.
      ///  Сфера определяется радиусом r с центром в sc.
      ///  Есть потенциально две точки пересечения.
      /// </remarks>
      /// <returns>FALSE, если линия не пересекает сферу.</returns>
      public static bool FindIntersection(ICoordinates p1, ICoordinates p2, ICoordinates sc, double r, out Point3d res1, out Point3d res2)
      {
         return FindIntersection(p1, p2, sc, r, out res1, out res2, epsilon);
      }

      /// <summary>
      /// Вычисление пересечения линии и сферы.
      /// </summary>
      /// <param name="p1">Начальная точка линии.</param>
      /// <param name="p2">Конечная точка линии.</param>
      /// <param name="sc">Центр сферы.</param>
      /// <param name="r">Радиус сферы.</param>
      /// <param name="res1">Первая точка пересечения.</param>
      /// <param name="res2">Вторая точка пересечения.</param>
      /// <param name="threshold">Допуск.</param>
      /// <remarks> 
      ///  Отрезок определяется от p1 до p2.
      ///  Сфера определяется радиусом r с центром в sc.
      ///  Есть потенциально две точки пересечения.
      /// </remarks>
      /// <returns>FALSE, если линия не пересекает сферу.</returns>
      public static bool FindIntersection(ICoordinates p1, ICoordinates p2, 
                                          ICoordinates sc, double r, 
                                          out Point3d res1, out Point3d res2, 
                                          double threshold)
      {
         double a, b, c, bb4ac, mu1, mu2;
         Vector3d dp = p2.ToVector3d() - p1.ToVector3d();

         a = dp.Norma;
         b = 2 * (dp.Vx * (p1.X - sc.X) + dp.Vy * (p1.Y - sc.Y) + dp.Vz * (p1.Z - sc.Z));
         c = sc.ToVector3d().Norma;
         c += p1.ToVector3d().Norma;
         c -= 2 * (sc.X * p1.X + sc.Y * p1.Y + sc.Z * p1.Z);
         c -= r * r;
         bb4ac = b * b - 4 * a * c;
         if (Math.Abs(a) < threshold || bb4ac < 0)
         {
            res1 = null;
            res2 = null;
            return false;
         }
         else if (bb4ac > 0 && bb4ac <= threshold)
         {
            mu1 = -b / (2 * a);
            res1 = (p1.ToVector3d() + (p2.ToVector3d() - p1.ToVector3d()) * mu1).ToPoint3d();
            res2 = null;
            return false;
         }

         mu1 = (-b + Math.Sqrt(bb4ac)) / (2 * a);
         mu2 = (-b - Math.Sqrt(bb4ac)) / (2 * a);
         res1 = (p1.ToVector3d() + (p2.ToVector3d() - p1.ToVector3d()) * mu1).ToPoint3d();
         res2 = (p1.ToVector3d() + (p2.ToVector3d() - p1.ToVector3d()) * mu2).ToPoint3d();

         return true;
      }

      /// <summary>
      /// Вычисление точки пересечения прямой с плоскостью.
      /// </summary>
      /// <param name="p1">Первая точка, определяющая плоскость.</param>
      /// <param name="p2">Вторая точка, определяющая плоскость.</param>
      /// <param name="p3">Третья точка, определяющая плоскость.</param>
      /// <param name="p4">Первая точка, определяющая прямую.</param>
      /// <param name="p5">Вторая точка, определяющая прямую.</param>
      /// <param name="res">Результирующая точка пересечения.</param>
      /// <param name="t">Параметр, результирующей точки пересечения</param>
      /// <returns>TRUE, если точка пересечения найдена.</returns>
      public static bool FindIntersection(ICoordinates p1, ICoordinates p2, ICoordinates p3,
                                          ICoordinates p4, ICoordinates p5,
                                          out Point3d res, out double t)
      {
         return FindIntersection(p1, p2, p3, p4, p5, out res, out t, epsilon);
      }
      
      /// <summary>
      /// Вычисление точки пересечения прямой с плоскостью.
      /// </summary>
      /// <param name="p1">Первая точка, определяющая плоскость.</param>
      /// <param name="p2">Вторая точка, определяющая плоскость.</param>
      /// <param name="p3">Третья точка, определяющая плоскость.</param>
      /// <param name="p4">Первая точка, определяющая прямую.</param>
      /// <param name="p5">Вторая точка, определяющая прямую.</param>
      /// <param name="res">Результирующая точка пересечения.</param>
      /// <param name="t">Параметр, результирующей точки пересечения</param>
      /// <param name="threshold">Допуск.</param>
      /// <returns>TRUE, если точка пересечения найдена.</returns>
      public static bool FindIntersection(ICoordinates p1, ICoordinates p2, ICoordinates p3,
                                          ICoordinates p4, ICoordinates p5,
                                          out Point3d res, out double t, double threshold)
      {
         Vector3d normal = (p2.ToVector3d() - p1.ToVector3d()) ^ (p3.ToVector3d() - p1.ToVector3d());
         double denom = normal % (p5.ToVector3d() - p4.ToVector3d());
         double nom = normal % (p1.ToVector3d() - p4.ToVector3d());
         if (IsZero(denom, threshold))
         {
            res = null;
            t = double.NaN;
            return false;
         }
         else
         {
            t = nom / denom;
            res = new Point3d(p4.ToVector3d() + (p5.ToVector3d() - p4.ToVector3d()) * t);
            return true;
         }
      }

      /// <summary>
      /// Вычисление линии пересечения двух плоскостей.
      /// </summary>
      /// <param name="p1">Первая точка, определяющая 1-ю плоскость.</param>
      /// <param name="p2">Вторая точка, определяющая 1-ю плоскость.</param>
      /// <param name="p3">Третья точка, определяющая 1-ю плоскость.</param>
      /// <param name="p4">Первая точка, определяющая 2-ю плоскость.</param>
      /// <param name="p5">Вторая точка, определяющая 2-ю плоскость.</param>
      /// <param name="p6">Третья точка, определяющая 2-ю плоскость.</param>
      /// <param name="res">Результирующая прямая.</param>
      /// <returns>TRUE, если линия пересечения найдена.</returns>
      public static bool FindIntersection(ICoordinates p1, ICoordinates p2, ICoordinates p3,
                                          ICoordinates p4, ICoordinates p5, ICoordinates p6,
                                          out Line3d res)
      {
         return FindIntersection(p1, p2, p3, p4, p5, p6, out res, epsilon);
      }
      
      /// <summary>
      /// Вычисление линии пересечения двух плоскостей.
      /// </summary>
      /// <param name="p1">Первая точка, определяющая 1-ю плоскость.</param>
      /// <param name="p2">Вторая точка, определяющая 1-ю плоскость.</param>
      /// <param name="p3">Третья точка, определяющая 1-ю плоскость.</param>
      /// <param name="p4">Первая точка, определяющая 2-ю плоскость.</param>
      /// <param name="p5">Вторая точка, определяющая 2-ю плоскость.</param>
      /// <param name="p6">Третья точка, определяющая 2-ю плоскость.</param>
      /// <param name="res">Результирующая прямая.</param>
      /// <param name="threshold">Допуск определения параллельности плоскостей.</param>
      /// <returns>TRUE, если линия пересечения найдена.</returns>
      public static bool FindIntersection(ICoordinates p1, ICoordinates p2, ICoordinates p3,
                                          ICoordinates p4, ICoordinates p5, ICoordinates p6,
                                          out Line3d res, double threshold)
      {
         Vector3d n1 = (p2.ToVector3d() - p1.ToVector3d()) ^ (p3.ToVector3d() - p1.ToVector3d());
         Vector3d n2 = (p5.ToVector3d() - p4.ToVector3d()) ^ (p6.ToVector3d() - p4.ToVector3d());
         if (Vector3d.AreParallel(n1, n2, threshold))
         {
            res = null;
            return false;
         }
         else
         {
            //Vector3d d = n1 ^ n2 ^ n1;
            //Vector3d dp1 = p1.ToVector3d();
            //Vector3d dp2 = p1.ToVector3d() + d;
            //FindIntersection(p4, p5, p6, dp1.ToPoint3d(), dp2.ToPoint3d(), out Point3d ip1, out double t, threshold);
            //Point3d ip2 = ip1 + (n1 ^ n2);

            double det = (n1 % n1) * (n2 % n2) - Math.Pow(n1 % n2, 2);
            double d1 = n1 % p1.ToVector3d();
            double d2 = n2 % p4.ToVector3d();
            double c1 = (d1 * (n2 % n2) - d2 * (n1 % n2)) / det;
            double c2 = (d2 * (n1 % n1) - d1 * (n1 % n2)) / det;

            Vector3d ip1 = c1 * n1 + c2 * n2;
            Vector3d ip2 = c1 * n1 + c2 * n2 + (n1 ^ n2); 
            res = new Line3d(ip1.ToPoint3d(), ip2.ToPoint3d());
            return true;
         }
      }

      /// <summary>
      /// Вычисляет отрезок линии пересечения между 2 линиями (не отрезками).
      /// </summary>
      /// <returns>Возвращает FALSE, если решение не найдено.</returns>
      public static bool FindIntersection(ICoordinates line1Point1, ICoordinates line1Point2,
                                          ICoordinates line2Point1, ICoordinates line2Point2,
                                          out Point3d resultSegmentPoint1, out Point3d resultSegmentPoint2,
                                          double threshold = 1e-12)
      {
         // Algorithm is ported from the C algorithm of 
         // Paul Bourke at http://local.wasp.uwa.edu.au/~pbourke/geometry/lineline3d/
         resultSegmentPoint1 = null;
         resultSegmentPoint2 = null;

         Vector3d p1 = line1Point1.ToVector3d();
         Vector3d p2 = line1Point2.ToVector3d();
         Vector3d p3 = line2Point1.ToVector3d();
         Vector3d p4 = line2Point2.ToVector3d();
         Vector3d p13 = p1 - p3;
         Vector3d p43 = p4 - p3;

         if (p43.Norma < threshold)
         {
            return false;
         }
         Vector3d p21 = p2 - p1;
         if (p21.Norma < threshold)
         {
            return false;
         }

         double d1343 = p13 % p43;
         double d4321 = p43 % p21;
         double d1321 = p13 % p21;
         double d4343 = p43 % p43;
         double d2121 = p21 % p21;

         double denom = d2121 * d4343 - d4321 * d4321;
         if (Math.Abs(denom) < threshold)
         {
            return false;
         }
         double numer = d1343 * d4321 - d1321 * d4343;

         double mua = numer / denom;
         double mub = (d1343 + d4321 * (mua)) / d4343;

         resultSegmentPoint1 = (p1 + mua * p21).ToPoint3d();
         resultSegmentPoint2 = (p3 + mub * p43).ToPoint3d();

         return true;
      }

      /// <summary>
      /// Normalizes the value of an angle in degrees between [0, 360[.
      /// </summary>
      /// <param name="angle">Angle in degrees.</param>
      /// <returns>The equivalent angle in the range [0, 360[.</returns>
      /// <remarks>Negative angles will be converted to its positive equivalent.</remarks>
      public static double NormalizeAngle(double angle)
      {
         double normalized = angle % 360.0;
         if (normalized < 0) return 360.0 + normalized;
         return normalized;
      }

      /// <summary>
      /// Round off a numeric value to the nearest of another value.
      /// </summary>
      /// <param name="number">Number to round off.</param>
      /// <param name="roundTo">The number will be rounded to the nearest of this value.</param>
      /// <returns>The number rounded to the nearest value.</returns>
      public static double RoundToNearest(double number, double roundTo)
      {
         double multiplier = Math.Round(number / roundTo, 0);
         return multiplier * roundTo;
      }

      /// <summary>
      /// Вращение заданного вектора на угол в радианах вокруг указанной оси.
      /// </summary>
      /// <param name="v">Вектор для вращения.</param>
      /// <param name="axis">Ось вращения. Этот вектор должен быть единичным.</param>
      /// <param name="angle">Угол вращения в радианах.</param>
      /// <remarks>Method provided by: Idelana. Original Author: Paul Bourke ( http://paulbourke.net/geometry/rotate/ )</remarks>
      public static Vector3d RotateAboutAxis(Vector3d v, Vector3d axis, double angle)
      {
         Vector3d q = new Vector3d();
         double cos = Math.Cos(angle);
         double sin = Math.Sin(angle);

         q.Vx += (cos + (1 - cos) * axis.Vx * axis.Vx) * v.Vx;
         q.Vx += ((1 - cos) * axis.Vx * axis.Vy - axis.Vz * sin) * v.Vy;
         q.Vx += ((1 - cos) * axis.Vx * axis.Vz + axis.Vy * sin) * v.Vz;

         q.Vy += ((1 - cos) * axis.Vx * axis.Vy + axis.Vz * sin) * v.Vx;
         q.Vy += (cos + (1 - cos) * axis.Vy * axis.Vy) * v.Vy;
         q.Vy += ((1 - cos) * axis.Vy * axis.Vz - axis.Vx * sin) * v.Vz;

         q.Vz += ((1 - cos) * axis.Vx * axis.Vz - axis.Vy * sin) * v.Vx;
         q.Vz += ((1 - cos) * axis.Vy * axis.Vz + axis.Vz * sin) * v.Vy;
         q.Vz += (cos + (1 - cos) * axis.Vz * axis.Vz) * v.Vz;

         return q;
      }

      /// <summary>
      /// Swaps two variables.
      /// </summary>
      /// <typeparam name="T">Variable type.</typeparam>
      /// <param name="obj1">An object of type T.</param>
      /// <param name="obj2">An object of type T.</param>
      public static void Swap<T>(ref T obj1, ref T obj2)
      {
         T tmp = obj1;
         obj1 = obj2;
         obj2 = tmp;
      }

      #endregion
   }

   /// <summary>
   /// Определяет ссылку на систему координат.
   /// </summary>
   public enum CoordinateSystem
   {
      /// <summary>
      /// Глобальные мировые координаты.
      /// </summary>
      World,

      /// <summary>
      /// Координаты объекта.
      /// </summary>
      Object
   }
}