using System;

namespace AGVMathOperation
{
    public static class Distance
    {

        public static int ManhatanDistance(int xStart,int yStart,int xEnd,int yEnd)
        {
            int diffx = xStart - xEnd;
            int diffy = yStart - yEnd;
            return Math.Abs(diffx + diffy);
        }

        public static double ManhatanDistance(double xStart, double yStart, double xEnd,double yEnd)
        {
            double diffx = xStart - xEnd;
            double diffy = yStart - yEnd;
            return Math.Abs(diffx + diffy);
        }

        public static int EuclideanDistance(int xStart,int yStart,int xEnd,int yEnd)
        {
            double diffx = (double)(xStart - xEnd);
            double diffy = (double)(yStart - yEnd);
            int distance = (int)Math.Sqrt(diffx * diffx + diffy * diffy);
            return distance;
        }

        public static double EuclideanDistance(double xStart,double yStart,double xEnd, double yEnd)
        {
            double diffx = xStart - xEnd;
            double diffy = yStart - yEnd;
            return Math.Sqrt(diffx * diffx + diffy * diffy);
        }

    }

    public static class Trigonometric
    {
        /// <summary>
        /// L * Cos(Theta)
        /// </summary>
        /// <param name="DiagonalLength"></param>
        /// <param name="radian"></param>
        /// <returns></returns>
        public static double AdjacentLength(double DiagonalLength,double radian)
        {
            return DiagonalLength * Math.Cos(radian);
        }

        /// <summary>
        /// L * Sin(Theta)
        /// </summary>
        /// <param name="DiagonalLength"></param>
        /// <param name="radian"></param>
        /// <returns></returns>
        public static double OppositeLength(double DiagonalLength,double radian)
        {
            return DiagonalLength * Math.Sin(radian);
        }

        /// <summary>
        /// (L1*L1 + L2* L2 - L3*L3) / (2*L1*L2)
        /// </summary>
        /// <param name="length1"></param>
        /// <param name="length2"></param>
        /// <param name="length3"></param>
        /// <returns></returns>
        public static double CosinRuleForTheta(double length1,double length2,double length3)
        {
            double acosResult = (length1 * length1 + length2 * length2 - length3 * length3) / (2 * length1 * length2);
            return Math.Acos(acosResult);
        }

    }

    public static class Transformation
    {
        /// <summary>
        /// Convert pole position to cartesian position of laser data relative to car position
        /// </summary>
        /// <param name="distance">Measurement Distance</param>
        /// <param name="angleBase">Start angle of mesaurement</param>
        /// <param name="resolution">Scanning resolution</param>
        /// <param name="index">Distance index</param>
        /// <param name="offsetLen">Length of laser center relative to motor center</param>
        /// <param name="carX"></param>
        /// <param name="carY"></param>
        /// <param name="carTheta"></param>
        /// <returns>Cartesian Position</returns>
        public static int[] LaserPoleToCartesian(int distance, double angleBase, double resolution, int index,
            double offsetLen, double carX, double carY, double carTheta)
        {
            int[] pos = new int[2];
            double offsetX = Trigonometric.AdjacentLength(offsetLen, carTheta * Math.PI / 180);
            double offsetY = Trigonometric.OppositeLength(offsetLen, carTheta * Math.PI / 180);
            double angle = angleBase + index * resolution + carTheta;
            pos[0] = (int)(Trigonometric.AdjacentLength(distance, angle * Math.PI / 180) + offsetX + carX);
            pos[1] = (int)(Trigonometric.OppositeLength(distance, angle * Math.PI / 180) + offsetY + carY);
            return pos;
        }

        /// <summary>
        /// Convert pole position to cartesian position of laser data relative to car position
        /// </summary>
        /// <param name="distance">Measurement Distance</param>
        /// <param name="angleBase">Start angle of mesaurement</param>
        /// <param name="resolution">Scanning resolution</param>
        /// <param name="index">Distance index</param>
        /// <param name="offsetLen">Length of laser center relative to motor center</param>
        /// <param name="carX"></param>
        /// <param name="carY"></param>
        /// <param name="carTheta"></param>
        /// <param name="angle">Data angle relative to car position</param>
        /// <returns>Cartesian Position</returns>
        public static int[] LaserPoleToCartesian(int distance,double angleBase,double resolution,int index,
            double offsetLen,double carX,double carY,double carTheta,out double angle)
        {
            int[] pos = new int[2];
            double offsetX = Trigonometric.AdjacentLength(offsetLen, carTheta * Math.PI / 180);
            double offsetY = Trigonometric.OppositeLength(offsetLen, carTheta * Math.PI / 180);
            angle = angleBase + index * resolution + carTheta;
            pos[0] = (int)(Trigonometric.AdjacentLength(distance, angle * Math.PI / 180) + offsetX + carX);
            pos[1] = (int)(Trigonometric.OppositeLength(distance, angle * Math.PI / 180) + offsetY + carY);
            return pos;
        }


        /// <summary>
        /// Convert pole position to cartesian position of laser data relative to car position
        /// </summary>
        /// <param name="distance">Measurement Distance</param>
        /// <param name="angleBase">Start angle of mesaurement</param>
        /// <param name="resolution">Scanning resolution</param>
        /// <param name="index">Distance index</param>
        /// <param name="offsetLen">Length of laser center relative to motor center</param>
        /// <param name="carX"></param>
        /// <param name="carY"></param>
        /// <param name="carTheta"></param>
        /// <param name="angle">Data angle of laser relative to car position</param>
        /// <param name="laserAngle">Data angle of laser</param>
        /// <returns>Cartesian Position</returns>
        public static int[] LaserPoleToCartesian(int distance, double angleBase, double resolution, int index,
            double offsetLen, double carX, double carY, double carTheta, out double angle,out double laserAngle)
        {
            int[] pos = new int[2];
            double offsetX = Trigonometric.AdjacentLength(offsetLen, carTheta * Math.PI / 180);
            double offsetY = Trigonometric.OppositeLength(offsetLen, carTheta * Math.PI / 180);
            laserAngle = angleBase + index * resolution;
            angle = angleBase + index * resolution + carTheta;
            pos[0] = (int)(Trigonometric.AdjacentLength(distance, angle * Math.PI / 180) + offsetX + carX);
            pos[1] = (int)(Trigonometric.OppositeLength(distance, angle * Math.PI / 180) + offsetY + carY);
            return pos;
        }

        /// <summary>
        /// Convert pole position to cartesian position of laser data relative to car position
        /// </summary>
        /// <param name="distance">Measurement Distance</param>
        /// <param name="angleBase">Start angle of mesaurement</param>
        /// <param name="resolution">Scanning resolution</param>
        /// <param name="index">Distance index</param>
        /// <param name="offsetLen">Length of laser center relative to motor center</param>
        /// <param name="carX"></param>
        /// <param name="carY"></param>
        /// <param name="carTheta"></param>
        /// <param name="angle">Data angle of laser relative to car position</param>
        /// <param name="laserAngle">Data angle of laser</param>
        /// <returns>Cartesian Position</returns>
        public static int[] LaserPoleToCartesian(int distance, double angleBase, double resolution, int index, double angleOffset,
            double offsetLen, double offsetTheta, double carX, double carY, double carTheta, out double angle, out double laserAngle) {
            int[] pos = new int[2];
            double offsetX = Trigonometric.AdjacentLength(offsetLen, (carTheta + offsetTheta) * Math.PI / 180);
            double offsetY = Trigonometric.OppositeLength(offsetLen, (carTheta + offsetTheta) * Math.PI / 180);
            laserAngle = angleBase + index * resolution + angleOffset;
            angle = laserAngle + carTheta;
            pos[0] = (int)(Trigonometric.AdjacentLength(distance, angle * Math.PI / 180) + offsetX + carX);
            pos[1] = (int)(Trigonometric.OppositeLength(distance, angle * Math.PI / 180) + offsetY + carY);
            return pos;
        }

        /// <summary>
        /// Obtain point extended from base point with specific vector
        /// </summary>
        /// <param name="x">X axis of base point</param>
        /// <param name="y">Y axis of base point</param>
        /// <param name="length">Vector Translation</param>
        /// <param name="radian">Vector Rotation</param>
        /// <returns></returns>
        public static int[] PointExtension(int x,int y,int length,double radian)
        {
            int[] pos = new int[2];
            pos[0] = (int)(x + length * Math.Cos(radian));
            pos[1] = (int)(y + length * Math.Sin(radian));
            return pos;
        }

    }
}
