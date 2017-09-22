using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapGL
{
    /// <summary>
    /// Common math function for AGV project
    /// Company:CASTEC
    /// Engineer:Denny,Emay.
    /// 2017/05/31
    ///     1.Add transformation function of real to graphic.
    ///     2.Add transformation function of graphic to real.
    /// 2017/06/01
    ///     1.Add distance calculation of graphic system.
    ///     2.Add position calculation of graphic system with displacement vector.
    ///     3.Add obstacle calculation of graphic system relative to car position.
    ///     4.Add degree calculation of LRF relative to car position.
    ///     5.Add degree calculation of LRF.
    /// </summary>
    public class AGVMath
    {
        private const double DtoR = Math.PI / 180;
        private const double RtoD = 180 / Math.PI;

        /// <summary>
        /// Compute distance between two points in graphic system
        /// </summary>
        /// <param name="oriX">X-Axis of Origin</param>
        /// <param name="oriY">Y-Axis of Origin</param>
        /// <param name="goalX">X-Axis of Goal</param>
        /// <param name="goalY">Y-Axis of Goal</param>
        /// <returns>Distance</returns>
        public static int ComputeGraphicDistance(int oriX,int oriY,int goalX,int goalY)
        {
            int diffx = goalX - oriX;
            int diffy = goalY - oriY;
            double sqrt = Math.Sqrt(diffx * diffx + diffy * diffy);
            return  Convert.ToInt32(sqrt);
        }

        /// <summary>
        /// Compute point relative to base position with specific vector
        /// </summary>
        /// <param name="angle">Vector angle</param>
        /// <param name="length">Vector length</param>
        /// <param name="x">X-Axis of base</param>
        /// <param name="y">Y-Axis of base</param>
        /// <param name="pX">X-Axis of result</param>
        /// <param name="pY">Y-Axis of result</param>
        public static void ObtainGraphicPoint(int length,int oriX, int oriY,int goalX,int goalY, out int pX, out int pY)
        {
            double diffx =goalX - oriX;
            double diffy =goalY - oriY;
            double posX = Math.Cos(Math.Atan2(diffy, diffx)) * length;
            double posY = Math.Sin(Math.Atan2(diffy, diffx)) * length;

            pX = (int)(posX + oriX);
            pY = (int)(posY + oriY);
        }

        /// <summary>
        /// Compute obstacle position relative to car position and transfer to graphic system
        /// </summary>
        /// <param name="angle">Laser degree</param>
        /// <param name="length">Laser length</param>
        /// <param name="offset_len">Offset length between laser and motor center</param>
        /// <param name="offset_theta">Offset angle of laser rlative to motor center</param>
        /// <param name="pictWidth">Image width</param>
        /// <param name="pictHeight">Image height</param>
        /// <param name="scale">Image scale ratio</param>
        /// <param name="carX">X-axis of AGV in real system</param>
        /// <param name="carY">Y-axis of AGV in real system</param>
        /// <param name="carTheta">Theta of AGV in real system</param>
        /// <param name="gCarx">X-axis of AGV in graphic system</param>
        /// <param name="gCary">Y-axis of AGV in graphic system</param>
        public static void ObtainGraphicObstacle(double angle,int length,double offset_len,double offset_theta,
           int pictWidth,int pictHeight,int scale,double carX,double carY,double carTheta,out int gCarx,out int gCary)
        {
            double posX = Math.Cos(-(angle) * DtoR) * length;
            double posY = Math.Sin(-(angle) * DtoR) * length;
            double offsetX = offset_len * Math.Cos(-(carTheta + offset_theta) * DtoR);
            double offsetY = offset_len * Math.Sin(-(carTheta + offset_theta) * DtoR);

            gCarx =(int) ((posX + offsetX) / scale + pictWidth / 2 + (carX / scale));
            gCary =(int) ((posY + offsetY) / scale + pictHeight / 2 - (carY / scale));
        }


        /// <summary>
        /// Compute obstacle position relative to car position and transfer to graphic system
        /// </summary>
        /// <param name="angle">Laser degree</param>
        /// <param name="length">Laser length</param>
        /// <param name="offset_len">Offset length between laser and motor center</param>
        /// <param name="offset_theta">Offset angle of laser rlative to motor center</param>
        /// <param name="pictWidth">Image width</param>
        /// <param name="pictHeight">Image height</param>
        /// <param name="scale">Image scale ratio</param>
        /// <param name="carX">X-axis of AGV in real system</param>
        /// <param name="carY">Y-axis of AGV in real system</param>
        /// <param name="carTheta">Theta of AGV in real system</param>
        /// <param name="gCarx">X-axis of AGV in graphic system</param>
        /// <param name="gCary">Y-axis of AGV in graphic system</param>
        public static void ObtainObstacle(double angle, int length, double offset_len, double offset_theta
          , int scale, double carX, double carY, double carTheta, out int gCarx, out int gCary)
        {
            double posX = Math.Cos((angle) * DtoR) * length;
            double posY = Math.Sin((angle) * DtoR) * length;
            double offsetX = offset_len * Math.Cos((carTheta + offset_theta) * DtoR);
            double offsetY = offset_len * Math.Sin((carTheta + offset_theta) * DtoR);

            gCarx = (int)((posX + offsetX) / scale + (carX / scale));
            gCary = (int)((posY + offsetY) / scale + (carY / scale));
        }

        /// <summary>
        /// Compute measurement degree relative to data index
        /// </summary>
        /// <param name="angeBase">Based degree</param>
        /// <param name="degIndex">Degree index</param>
        /// <param name="resolution">Resolution</param>
        /// <param name="baseTheta">Based Theta</param>
        /// <returns>Measurement degree</returns>
        public static double ObtainLaserDegree(double angeBase,int degIndex,double resolution,double baseTheta)
        {
            return angeBase + degIndex * resolution + baseTheta;
        }

        /// <summary>
        /// Compute measurement degree relative to data index
        /// </summary>
        /// <param name="angeBase">Based degree</param>
        /// <param name="degIndex">Degree index</param>
        /// <param name="resolution">Resolution</param>
        /// <returns>Measurement degree</returns>
        public static double ObtainLaserDegree(double angeBase, int degIndex, double resolution)
        {
            return angeBase + degIndex * resolution ;
        }

        /// <summary>
        /// Transfer single point of real system to graphic system
        /// </summary>
        /// <param name="x">x of real system</param>
        /// <param name="y">y of real sytem</param>
        /// <param name="mapWidth">Map width</param>
        /// <param name="mapHeight">Map height</param>
        /// <param name="scale">Ratio</param>
        /// <param name="transx">x of graphic system</param>
        /// <param name="transy">y of graphic system</param>
        public static void RealToGraphic(double x,double y,int mapWidth, int mapHeight,
            int scale,out int transx,out int transy)
        {
            transx = (int)(x / scale + mapWidth / 2);
            transy = (int)(-y / scale + mapHeight / 2);
        }


        /// <summary>
        /// Transfer single point of real system to graphic system
        /// </summary>
        /// <param name="x">x of real system</param>
        /// <param name="y">y of real sytem</param>
        /// <param name="mapWidth">Map width</param>
        /// <param name="mapHeight">Map height</param>
        /// <param name="scale">Ratio</param>
        /// <param name="transx">x of graphic system</param>
        /// <param name="transy">y of graphic system</param>
        public static void RealToGraphic2(double x, double y, int offsetX, int offsetY,
            int scale, out int transx, out int transy)
        {
            transx = (int)(x / scale -offsetX);
            transy = (int)(y / scale -offsetY);
        }
        /// <summary>
        /// Transfer single point of real system to graphic system
        /// </summary>
        /// <param name="x">x of real system</param>
        /// <param name="y">y of real sytem</param>
        /// <param name="mapWidth">Map width</param>
        /// <param name="mapHeight">Map height</param>
        /// <param name="scale">Ratio</param>
        /// <param name="transx">x of graphic system</param>
        /// <param name="transy">y of graphic system</param>
        public static void GraphicToReal(double x, double y, int mapWidth, int mapHeight,
            int scale, out double transx, out double transy)
        {
            transx = (x - mapWidth / 2) * scale;
            transy = -(y - mapHeight / 2) * scale;
        }

        /// <summary>
        /// Transfer single point of real system to graphic system
        /// </summary>
        /// <param name="point">Position of real system</param>
        /// <param name="mapWidth">Map width</param>
        /// <param name="mapHeight">Map height</param>
        /// <param name="scale">Ratio</param>
        /// <returns>Position of graphic system</returns>
        public static Point RealToGraphic(Point point,int mapWidth,int mapHeight,int scale)
        {
            Point newP = new Point();
            newP.X = point.X / scale + mapWidth / 2;
            newP.Y = -point.Y / scale + mapHeight / 2;
            return newP;
        }

        /// <summary>
        /// Transfer single point of graphic system to real system
        /// </summary>
        /// <param name="point">Position of graphic system</param>
        /// <param name="mapWidth">Map width</param>
        /// <param name="mapHeight">Map height</param>
        /// <param name="scale">Ratio</param>
        /// <returns>Position of real system</returns>
        public static Point GraphicToReal(Point point, int mapWidth, int mapHeight, int scale)
        {
            Point newP = new Point();
            newP.X = (point.X - mapWidth / 2) * scale;
            newP.Y = -(point.Y - mapHeight / 2) * scale;
            return newP;
        }


        /// <summary>
        /// Transfer  point list of real system to graphic system
        /// </summary>
        /// <param name="point">Position of real system</param>
        /// <param name="mapWidth">Map width</param>
        /// <param name="mapHeight">Map height</param>
        /// <param name="scale">Ratio</param>
        /// <returns>Point list of graphic system</returns>
        public static List<Point> RealToGraphic(List<Point> point, int mapWidth, int mapHeight, int scale)
        {
            List<Point> newP = new List<Point>();
            for (int i = 0; i < point.Count; i++)
            {
                Point p = new Point();
                p.X = (int)(point[i].X / scale + mapWidth / 2);
                p.Y = (int)(-point[i].Y / scale+ mapHeight / 2);
                newP.Add(p);
            }
            return newP;
        }

        /// <summary>
        /// Transfer  point list of real system to graphic system
        /// </summary>
        /// <param name="point">Position of real system</param>
        /// <param name="mapWidth">Map width</param>
        /// <param name="mapHeight">Map height</param>
        /// <param name="scale">Ratio</param>
        /// <returns>Point list of graphic system</returns>
        public static List<Point> RealToGraphic2(List<Point> point, int offsetX, int offsetY, int scale)
        {
            List<Point> newP = new List<Point>();
            for (int i = 0; i < point.Count; i++)
            {
                Point p = new Point();
                p.X = (int)(point[i].X / scale - offsetX);
                p.Y = (int)(point[i].Y / scale - offsetY);
                newP.Add(p);
            }
            return newP;
        }

        /// <summary>
        /// Transfer  point list of real system to graphic system
        /// </summary>
        /// <param name="point">Position of real system</param>
        /// <param name="mapWidth">Map width</param>
        /// <param name="mapHeight">Map height</param>
        /// <param name="scale">Ratio</param>
        /// <returns>Point list of graphic system</returns>
        public static List<Point> GraphicToReal(List<Point> point, int mapWidth, int mapHeight, int scale)
        {
            List<Point> newP = new List<Point>();
            for (int i = 0; i < point.Count; i++)
            {
                Point p = new Point();
                p.X = (point[i].X - mapWidth / 2) * scale;
                p.Y = -(point[i].Y - mapHeight / 2) * scale;
                newP.Add(p);
            }
            return newP;
        }

        /// <summary>
        /// Transfer  point list of real system to graphic system
        /// </summary>
        /// <param name="point">Position of real system</param>
        /// <param name="mapWidth">Map width</param>
        /// <param name="mapHeight">Map height</param>
        /// <param name="scale">Ratio</param>
        /// <returns>Point list of graphic system</returns>
        public static List<Point> GraphicToReal2(List<Point> point, int offsetX, int offsetY, int scale)
        {
            List<Point> newP = new List<Point>();
            for (int i = 0; i < point.Count; i++)
            {
                Point p = new Point();
                p.X = (point[i].X + offsetX) * scale;
                p.Y = (point[i].Y + offsetY) * scale;
                newP.Add(p);
            }
            return newP;
        }
    }
}
