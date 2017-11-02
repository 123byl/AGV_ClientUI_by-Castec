using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.Threading;
using AGVMathOperation;
using KDTree;
using GLCore;

namespace MapProcessing
{
    public class MapRecording
    {

        #region - Constructor -

        public MapRecording()
        {
            InspectDirectory();
        }

        #endregion

        #region - Outer Method -

        /// <summary>
        /// Empty the file
        /// </summary>
        /// <returns></returns>
        public static bool EmptyFile(string filePath)
        {
            File.Create(filePath).Close();
            return true;
        }

        /// <summary>
        /// Write scanning information
        /// </summary>
        /// <param name="str">Recording Information</param>
        /// <param name="filePath">File path</param>
        /// <returns>State</returns>
        public static bool WriteCurrentScanningInfo(StringBuilder str, string filePath)
        {
            if (!File.Exists(filePath)) return false;
            using (StreamWriter sw = new StreamWriter(filePath, true))
            {
                sw.WriteLine(str);
            }

            return true;
        }

        /// <summary>
        /// Write the minimum and maximum position into file
        /// </summary>
        public static bool WriteMinMaxInfo(int min_x, int min_y, int max_x, int max_y, string filePath)
        {
            if (!File.Exists(filePath)) return false;
            List<string> info = File.ReadAllLines(filePath).ToList();
            info.Insert(0, String.Format("Minimum Position:{0},{1}", min_x, min_y));
            info.Insert(1, String.Format("Maximum Position:{0},{1}", max_x, max_y));

            using (File.Create(filePath)) { };
            File.AppendAllLines(filePath, info);
            return true;
        }

        /// <summary>
        /// Write the goal into file
        /// </summary>
        public static bool WriteGoal(int x, int y, double theta, string filePath)
        {
            if (!File.Exists(filePath)) return false;
            List<string> info = File.ReadAllLines(filePath).ToList();
            int startIndex = info.IndexOf("Obstacle Lines");
            info.Insert(startIndex, String.Format("{0},{1},{2}", x, y, theta));

            if (info.IndexOf("GoalList") == -1)
                info.Insert(startIndex, "GoalList");
            using (File.Create(filePath)) { }
            File.WriteAllLines(filePath, info);

            return true;
        }

        /// <summary>
        /// Write the goal into file
        /// </summary>
        public static bool OverWriteGoal(List<CartesianPos> goalList, string filePath)
        {
            if (!File.Exists(filePath)) return false;
            List<string> info = File.ReadAllLines(filePath).ToList();
            int startIndex = info.IndexOf("GoalList");
            int endIndex = info.IndexOf("Obstacle Lines");
            if (startIndex != -1)
            {
                info.RemoveRange(startIndex, endIndex - startIndex);
                endIndex = info.IndexOf("Obstacle Lines");
            }
            if (goalList.Count > 0)
            {
                for (int i = goalList.Count - 1; i >= 0; i--)
                {
                    info.Insert(endIndex, string.Format("{0},{1},{2}", goalList[i].x, goalList[i].y, goalList[i].theta));
                }
                info.Insert(endIndex, "GoalList");
            }
            File.WriteAllLines(filePath, info);
            info = null;
            return true;
        }

        #endregion

        #region - Inner Method -

        /// <summary>
        /// Confirm directory is exists
        /// </summary>
        private void InspectDirectory()
        {
            if (!Directory.Exists("D:/MapInfo")) Directory.CreateDirectory("D:/MapInfo");
        }

        #endregion

    }

    public class MapReading : IDisposable
    {
        #region - Member -

        string path;
        List<string> data;
        bool isDisposed;
        private char[] splitChar = new char[2] { ':', ',' };

        #endregion

        #region - Constructor -

        public MapReading(string path)
        {
            this.path = path;
        }

        ~MapReading()
        {
            Dispose(false);
        }

        #endregion

        #region - Method -

        /// <summary>
        /// Release resouce
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Override method of base class
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    data = null;
                    splitChar = null;
                    path = null;
                }
                isDisposed = true;
            }
        }

        /// <summary>
        /// Open origin file and read all scanning information
        /// </summary>
        /// <returns>Information length 0:Error Other:Read success</returns>
        public int OpenFile()
        {
            if (!FileIsExist()) return 0;
            data = File.ReadAllLines(path).ToList();
            return data.Count;
        }

        /// <summary>
        /// Extract signle origin recording information
        /// </summary>
        /// <param name="index">Extract index</param>
        /// <param name="carPos">Car Position</param>
        /// <param name="laserData">Laser Measurement Data</param>
        /// <returns>T:Read success F:File or data not exists</returns>
        public bool ReadScanningInfo(int index, out CartesianPos carPos, out List<CartesianPos> laserData)
        {
            try
            {
                carPos = new CartesianPos();
                laserData = new List<CartesianPos>();
                if (!FileIsExist()) return false;

                string[] info = data[index].Split(splitChar);
                carPos.SetPosition(double.Parse(info[0]), double.Parse(info[1]), double.Parse(info[2]));
                for (int m = 3; m < info.Length - 1; m += 2)
                {
                    laserData.Add(new CartesianPos(double.Parse(info[m]), double.Parse(info[m + 1])));
                }
                return true;
            }
            catch (Exception error)
            {
                carPos = null;
                laserData = null;
                Console.WriteLine(error);
                return false;
            }

        }

        /// <summary>
        /// Read goal list of assigned .map file
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="goalList"></param>
        /// <returns>T:Read success F:File or data not exists</returns>
        public bool ReadMapBoundary(out CartesianPos minimumPos, out CartesianPos maximumPos)
        {
            minimumPos = new CartesianPos();
            maximumPos = new CartesianPos();
            if (!FileIsExist()) return false;

            string[] split = data[0].Split(splitChar);
            if (split[0] != "Minimum Position")
                return false;
            else
                minimumPos.SetPosition(double.Parse(split[1]), double.Parse(split[2]), 0);

            split = data[1].Split(splitChar);
            if (split[0] != "Maximum Position")
                return false;
            else
                maximumPos.SetPosition(double.Parse(split[1]), double.Parse(split[2]), 0);

            return true;
        }

        /// <summary>
        /// Read goal list of assigned .map file
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="goalList"></param>
        /// <returns>T:Read success F:File or data not exists</returns>
        public bool ReadMapGoalList(out List<CartesianPosInfo> goalList)
        {
            goalList = new List<CartesianPosInfo>();

            if (!FileIsExist()) return false;
            int startIndex = data.IndexOf("GoalList");
            if (startIndex == -1)
                return false;
            else
                startIndex++;

            int endIndex = data.IndexOf("Obstacle Lines");
            for (int i = startIndex; i < endIndex; i++)
            {
                string[] split = data[i].Split(splitChar);
                if (split.Length == 4)
                    goalList.Add(new CartesianPosInfo(int.Parse(split[1]), int.Parse(split[2]), double.Parse(split[3]), split[0], Database.ID.GenerateID()));
                else
                    goalList.Add(new CartesianPosInfo(int.Parse(split[0]), int.Parse(split[1]), double.Parse(split[2]), "Goal" + (i - startIndex), Database.ID.GenerateID()));
            }
            return true;
        }

        /// <summary>
        /// Read obstacle line of .map file
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="obstacleLine"></param>
        /// <returns>T:Read success F:File or data not exists</returns>
        public bool ReadMapObstacleLines(out List<MapLine> obstacleLine)
        {
            obstacleLine = new List<MapLine>();
            if (!FileIsExist()) return false;

            int startIndex = data.IndexOf("Obstacle Lines") + 1;
            int endIndex = data.IndexOf("Obstacle Points");
            for (int i = startIndex; i < endIndex; i++)
            {
                string[] split = data[i].Split(splitChar);
                CartesianPos start = new CartesianPos(int.Parse(split[0]), int.Parse(split[1]));
                CartesianPos end = new CartesianPos(int.Parse(split[2]), int.Parse(split[3]));
                obstacleLine.Add(new MapLine(start, end));
            }
            return true;
        }

        /// <summary>
        /// Read obstacle point of .map file
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="obstacleLine"></param>
        /// <returns>T:Read success F:File or data not exists</returns>
        public bool ReadMapObstaclePoints(out List<CartesianPos> obstaclePoint)
        {
            obstaclePoint = new List<CartesianPos>();
            if (!FileIsExist()) return false;
            int startIndex = data.IndexOf("Obstacle Points") + 1;
            int endIndex = data.Count - 1;
            for (int i = startIndex; i < endIndex; i++)
            {
                string[] split = data[i].Split(splitChar);
                obstaclePoint.Add(new CartesianPos(int.Parse(split[0]), int.Parse(split[1])));
            }
            return true;
        }


        #endregion

        #region - Inner Method -

        /// <summary>
        /// Inspect file is exist
        /// </summary>
        /// <returns></returns>
        private bool FileIsExist()
        {
            if (!File.Exists(path))
                return false;
            else
                return true;
        }


        #endregion
    }

    public class MapSimplication
    {
        #region - Member -

        public string path;
        private CartesianPos minimumPos;
        private CartesianPos maximumPos;
        private List<Line> mapLines = new List<Line>();
        private List<List<LineSegment>> map;

        #endregion

        #region - Constructor -

        public MapSimplication(string path)
        {
            this.path = path;
        }

        #endregion

        #region - Method -

        public void Reset()
        {
            mapLines.Clear();

            if (map != null)
                map.Clear();
        }

        /// <summary>
        /// Read .map file to transfer map information into line segments
        /// </summary>
        /// <param name="noiseThreshold">Line segment minimum allowable length</param>
        /// <returns>Line segments of map information</returns>
        public void ReadMapAllTransferToLine(List<CartesianPos> mapPoint, CartesianPos minimumPos, CartesianPos maximumPos
            , int segmentThreshold, int noiseThreshold, out List<Line> lines, out List<CartesianPos> points)
        {

            this.minimumPos = minimumPos;
            this.maximumPos = maximumPos;
            int maxX = (int)maximumPos.x;
            int maxY = (int)maximumPos.y;
            int minX = (int)minimumPos.x;
            int minY = (int)minimumPos.y;
            int mapWidth = maxX - minX + 1;
            int mapHeight = maxY - minY + 1;
            int x = 0, y = 0;
            points = new List<CartesianPos>();
            map = new List<List<LineSegment>>();
            for (int i = 0; i < mapHeight; i++)
            {
                map.Add(new List<LineSegment>());
            }

            //Assign obstacle point
            for (int i = 0; i < mapPoint.Count; i++)
            {
                x = ((int)mapPoint[i].x) - minX;
                y = ((int)mapPoint[i].y) - minY;
                AddObstacle(y, new LineSegment(x, 0));
                SearchLineSegment(map[y], segmentThreshold);
            }


            //Remove line segment
            if (noiseThreshold != 0)
                RemoveLineSegment(noiseThreshold);

            //Update file
            MapRecording.EmptyFile(path);
            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.WriteLine(String.Format("Minimum Position:{0},{1}", minX, minY));
                sw.WriteLine(String.Format("Maximum Position:{0},{1}", maxX, maxY));
                sw.WriteLine("Obstacle Lines");
                for (int i = 0; i < map.Count; i++)
                {
                    for (int j = 0; j < map[i].Count; j++)
                    {
                        if (map[i][j].length > 0)
                        {
                            mapLines.Add(new Line(map[i][j].start + minX, i + minY,
                                map[i][j].start + map[i][j].length + minX, i + minY));
                            sw.WriteLine(string.Format("{0},{1},{2},{3}", map[i][j].start + minX, i + minY,
                                map[i][j].start + map[i][j].length + minX, i + minY));
                        }
                        else
                        {
                            points.Add(new CartesianPos(map[i][j].start + minX, i + minY));
                        }
                    }
                }

                if (points.Count > 0)
                {
                    sw.WriteLine("Obstacle Points");
                    for (int i = 0; i < points.Count; i++)
                    {
                        sw.WriteLine(string.Format("{0},{1}", points[i].x, points[i].y));
                    }
                }
                sw.Close();
            }
            lines = mapLines.ToArray().ToList();
        }

        /// <summary>
        /// According to line start position to insert
        /// </summary>
        /// <param name="Row">Line segments of current row</param>
        /// <param name="line">Line segment of current update</param>
        public void AddObstacle(int row, LineSegment line)
        {
            if (map[row].Count == 0)
                map[row].Add(line);
            else
                map[row].Insert(InsertIndex(map[row], line), line); //Insert data into specific index
            line = null;
        }

        /// <summary>
        /// Inspect input position is occupied or not
        /// </summary>
        /// <param name="pX"></param>
        /// <param name="pY"></param>
        public bool CheckIsObstacle(int x, int y)
        {
            ////Check index is valid
            //if (y > maxY - minY || x > maxX - minX)
            //    return true;

            //if (map[y].Count > 0)
            //{
            //    for (int i = 0; i < map[y].Count; i++)
            //    {
            //        //If start position of first line segment is larger then inquiry index
            //        //It means it's position is in the blank segment
            //        if (i == 0)
            //            if (map[y][i].start > x)
            //                return false;

            //        //If line segment of obstacle is enclude the inquiry index,it is obstacle
            //        if (map[y][i].start <= x && (map[y][i].start + map[y][i].length) >= x)
            //            return true;

            //        //If inquiry index is in blank segment,it is free grid
            //        if (i != 0)
            //            if (map[y][i - 1].start + map[y][i - 1].length < x && map[y][i].start > x)
            //                return false;
            //    }
            //}
            return false;
        }

        /// <summary>
        /// Checking specific area is free space or not
        /// </summary>
        /// <param name="startx"></param>
        /// <param name="endy"></param>
        /// <param name="starty"></param>
        /// <param name="endy"></param>
        /// <returns></returns>
        public bool CheckSpaceClear(int startx, int endx, int starty, int endy)
        {
            ////Check index is valid
            //if (starty > maxY - minY || startx > maxX - minX || endy > maxY - minY || endx > maxX - minX
            //    || starty < 0 || startx < 0)
            //    return false;

            //for (int row = starty; row < endy; row++)
            //{
            //    for (int col = 0; col < map[row].Count; col++)
            //    {

            //        if (map[row][col].start > endx)
            //            break;

            //        if (map[row][col].start >= startx)
            //            return false;

            //        if (map[row][col].start < startx && map[row][col].start + map[row][col].length >= startx)
            //            return false;

            //    }
            //}
            return true;
        }

        #endregion

        #region - Inner Method -

        /// <summary>
        /// Combine line segments which distances is below threshold
        /// </summary>
        /// <param name="row">Line segments of current row</param>
        /// <param name="segmentThreshold">Comibnation threshold</param>
        private void SearchLineSegment(List<LineSegment> row, int segmentThreshold)
        {
            for (int i = 0; i < row.Count - 1; i++)
            {
                if (Math.Abs(row[i].start + row[i].length - row[i + 1].start) <= segmentThreshold)
                {
                    row[i].length += Math.Abs(row[i].start + row[i].length - row[i + 1].start) + row[i + 1].length;
                    row.Remove(row[i + 1]);
                    i -= 1;
                }

            }
        }

        /// <summary>
        /// Determine position to insert data
        /// </summary>
        /// <param name="">Line segments of current row</param>
        /// <param name="">Line segment of current update</param>
        /// <returns>Insert Position</returns>
        private int InsertIndex(List<LineSegment> row, LineSegment line)
        {
            int left = 0;
            int right = row.Count - 1;
            int middle = 0;
            do
            {
                if (left == right)
                    if (line.start > row[left].start)
                        return right + 1;
                    else
                        return right;

                if (Math.Abs(left - right) == 1)
                    if (line.start <= row[left].start && line.start <= row[right].start)
                        return left;
                    else if (line.start >= row[left].start && line.start <= row[right].start)
                        return left + 1;
                    else if (line.start >= row[left].start && line.start >= row[right].start)
                        return right + 1;

                middle = (left + right) / 2;
                if (line.start > row[middle].start)
                    left = middle + 1;
                else
                    right = middle - 1;

            } while (true);

        }

        /// <summary>
        /// Remove linesegment which length is smaller then threshold
        /// </summary>
        /// <param name="noiseThreshold"></param>
        private void RemoveLineSegment(int noiseThreshold)
        {
            for (int i = 0; i < map.Count; i++)
            {
                for (int j = 0; j < map[i].Count; j++)
                {
                    if (map[i][j].length <= noiseThreshold)
                    {
                        map[i].Remove(map[i][j]);
                        j -= 1;
                    }
                }
            }
        }

        #endregion

        #region - Data Type Class -

        public class LineSegment : IComparable<LineSegment>
        {
            public int start;
            public int length;

            public LineSegment(int start, int length)
            {
                this.start = start;
                this.length = length;
            }

            public int CompareTo(LineSegment value)
            {
                return this.start.CompareTo(value.start);
            }
        }

        public class Line
        {
            public int startX;
            public int startY;
            public int endX;
            public int endY;
            public Line(int startX, int startY, int endX, int endY)
            {
                this.startX = startX;
                this.startY = startY;
                this.endX = endX;
                this.endY = endY;
            }
        }

        #endregion

    }

    public class MapMatching
    {
        #region - Member -

        public Dictionary<int, List<int>> mapPoint = new Dictionary<int, List<int>>();
        public List<CartesianPos> parseMap = new List<CartesianPos>();
        public CartesianPos minimumPos = new CartesianPos(double.MaxValue, double.MaxValue, 0);
        public CartesianPos maximumPos = new CartesianPos(double.MinValue, double.MinValue, 0);
        private KDTree<CartesianPos> kdTree = new KDTree<CartesianPos>(2);
        private KDTree<CartesianPos> pairKDTree = new KDTree<CartesianPos>(2);
        private Dictionary<CartesianPos, List<CartesianPos>> modelPointWRTdataPoint = new Dictionary<CartesianPos, List<CartesianPos>>();
        private double preError;

        #endregion

        #region - Constructor -

        public MapMatching()
        {

        }

        #endregion

        #region - Method -

        /// <summary>
        /// Add new Line
        /// </summary>
        /// <param name="line">Obstacle Line</param>
        /// <returns>State</returns>
        public void AddLine(MapLine line)
        {
            int start = (int)line.start.x;
            int end = (int)line.end.x;
            int y = (int)line.start.y;
            for (int x = start; x < end; x++)
            {
                kdTree.AddPoint(new double[2] { x, y }, new CartesianPos(x, y));
            }
        }

        public void Reset()
        {
            mapPoint.Clear();
            parseMap.Clear();
            modelPointWRTdataPoint.Clear();
            kdTree = null;
            kdTree = new KDTree<CartesianPos>(2);
            pairKDTree = null;
            pairKDTree = new KDTree<CartesianPos>(2);
        }

        #region - Matching operation -

        /// <summary>
        /// Add new point
        /// </summary>
        /// <param name="point">Obstacle point</param>
        /// <returns>State</returns>
        public void AddPoint(CartesianPos point)
        {
            kdTree.AddPoint(new double[2] { point.x, point.y }, point);
        }

        /// <summary>
        /// Add new point
        /// </summary>
        /// <param name="point">Obstacle point</param>
        /// <returns>State</returns>
        public void AddPoint(List<CartesianPos> point)
        {
            foreach (var item in point)
            {
                kdTree.AddPoint(new double[2] { item.x, item.y }, new CartesianPos(item.x, item.y));
            }
        }

        /// <summary>
        /// Inspect obstacle is exists in the specific range
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool PointIsExists(CartesianPos point, int searchRange)
        {
            var pIter = kdTree.NearestNeighbors(new double[] { point.x, point.y }, 1, searchRange * searchRange);
            if (pIter.MoveNext())
            {
                pIter = null;
                return true;
            }
            else
            {
                pIter = null;
                return false;
            }
        }

        /// <summary>
        /// Matching two sets of scanning data
        /// </summary>
        /// <param name="refSet">Previous data set</param>
        /// <param name="set2">Current data set</param>
        /// <returns>Matching error</returns>
        public double FindClosetMatching(List<CartesianPos> data, int samplingSize, double mseThreshold, double convergenceThreshold, int iterativeCycle
            , int searchDistance, bool rejectMode, CartesianPos transResult)
        {
            List<CartesianPos> set1 = new List<CartesianPos>();
            List<CartesianPos> set2 = new List<CartesianPos>();
            List<CartesianPos> result = new List<CartesianPos>();
            CartesianPos center1 = new CartesianPos();
            CartesianPos center2 = new CartesianPos();
            CartesianPos trans;
            int count = 0;
            double gValue = double.MaxValue;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            set2.AddRange(SamplingData(data, samplingSize));
            for (int n = 0; n < iterativeCycle; n++)
            {

                count++;
                //Stopwatch sws = new Stopwatch();
                //sws.Start();
                //Find closet point of each new data points relative reference points
                set1.Clear();
                SearchNearestPoint(kdTree, set1, set2, searchDistance, rejectMode);
                //sws.Stop();
                //Console.WriteLine("[ICP Matching]Searching Time:{0:F4}", sws.Elapsed.TotalMilliseconds);


                //Calculate center
                CalculateSetCenter(set1, set2, out center1, out center2);

                //Calculate transfromation
                trans = CalculateTransformation(set1, set2, center1, center2);
                transResult.x += trans.x;
                transResult.y += trans.y;
                transResult.theta += trans.theta;

                //Calculate MSE
                gValue = CalculateMSE(set1, set2, trans);

                //Calculate new data set
                result = null;
                result = CalculateNewDataSet(data, transResult, samplingSize);
                set2.Clear();
                set2.AddRange(result);

                //Checking MSE and convergence state 
                if (gValue <= mseThreshold) break;
                if (Math.Abs(preError - gValue) <= convergenceThreshold) break;

                preError = gValue;
                trans = null;

                if (searchDistance > 100)
                    searchDistance /= 2;
            }

            #region Fine Tuning 

            set1.Clear();
            SearchNearestPoint(kdTree, set1, set2, 10, rejectMode);
            CalculateSetCenter(set1, set2, out center1, out center2);
            trans = CalculateTransformation(set1, set2, center1, center2);
            transResult.x += trans.x;
            transResult.y += trans.y;
            transResult.theta += trans.theta;
            gValue = CalculateMSE(set1, set2, trans);
            result = null;
            result = CalculateNewDataSet(data, transResult, samplingSize);
            set2.Clear();
            set2.AddRange(result);

            #endregion

            sw.Stop();

            if (samplingSize != 0)
                result = CalculateNewDataSet(data, transResult, samplingSize);

            data.Clear();
            data.AddRange(result);
            //Console.WriteLine("[ICP Matching]Iterative Count:{0} Time:{1:F4}", count, sw.Elapsed.TotalMilliseconds);
            return gValue;

        }

        /// <summary>
        /// Matching two sets of scanning data
        /// </summary>
        /// <param name="refSet">Previous data set</param>
        /// <param name="set2">Current data set</param>
        /// <returns>Matching error</returns>
        public double FindClosetMatching(List<CartesianPos> data, int samplingSize, double mseThreshold, double convergenceThreshold, int iterativeCycle
            , int searchDistance, bool rejectMode, CartesianPos transResult, out List<CartesianPos> modelSet)
        {
            List<CartesianPos> set1 = new List<CartesianPos>();
            List<CartesianPos> set2 = new List<CartesianPos>();
            List<CartesianPos> result = new List<CartesianPos>();
            CartesianPos center1 = new CartesianPos();
            CartesianPos center2 = new CartesianPos();
            CartesianPos trans = new CartesianPos();
            int count = 0;
            double gValue = double.MaxValue;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            set2.AddRange(SamplingData(data, samplingSize));
            for (int n = 0; n < iterativeCycle; n++)
            {

                count++;
                //Stopwatch sws = new Stopwatch();
                //sws.Start();
                //Find closet point of each new data points relative reference points
                set1.Clear();
                SearchNearestPoint(kdTree, set1, set2, searchDistance, rejectMode);
                //sws.Stop();
                //Console.WriteLine("[ICP Matching]Searching Time:{0:F4}", sws.Elapsed.TotalMilliseconds);


                //Calculate center
                CalculateSetCenter(set1, set2, out center1, out center2);

                //Calculate transfromation
                trans = CalculateTransformation(set1, set2, center1, center2);
                transResult.x += trans.x;
                transResult.y += trans.y;
                transResult.theta += trans.theta;

                //Calculate MSE
                gValue = CalculateMSE(set1, set2, trans);

                //Calculate new data set
                result = null;
                result = CalculateNewDataSet(data, transResult, samplingSize);
                set2.Clear();
                set2.AddRange(result);

                //Checking MSE and convergence state 
                if (gValue <= mseThreshold) break;
                if (Math.Abs(preError - gValue) <= convergenceThreshold) break;

                preError = gValue;
                trans = null;

                if (searchDistance > 100)
                    searchDistance /= 2;
            }
            sw.Stop();

            #region Fine Tuning 

            set1.Clear();
            SearchNearestPoint(kdTree, set1, set2, 10, rejectMode);
            CalculateSetCenter(set1, set2, out center1, out center2);
            trans = CalculateTransformation(set1, set2, center1, center2);
            transResult.x += trans.x;
            transResult.y += trans.y;
            transResult.theta += trans.theta;
            gValue = CalculateMSE(set1, set2, trans);
            result = null;
            result = CalculateNewDataSet(data, transResult, samplingSize);
            set2.Clear();
            set2.AddRange(result);

            #endregion

            if (samplingSize != 0)
                result = CalculateNewDataSet(data, transResult, samplingSize);

            data.Clear();
            data.AddRange(result);
            modelSet = set1;
            //Console.WriteLine("[ICP Matching]Iterative Count:{0} Time:{1:F4}", count, sw.Elapsed.TotalMilliseconds);
            return gValue;

        }

        /// <summary>
        /// Matching two sets of scanning data
        /// </summary>
        /// <param name="refSet">Previous data set</param>
        /// <param name="set2">Current data set</param>
        /// <returns>Matching error</returns>
        public double PairwiseMatching(List<CartesianPos> model, List<CartesianPos> data, int samplingSize, double mseThreshold, double convergenceThreshold
            , int iterativeCycle, double searchDistance, bool rejectMode, CartesianPos transResult)
        {
            List<CartesianPos> set1 = new List<CartesianPos>();
            List<CartesianPos> set2 = new List<CartesianPos>();
            List<CartesianPos> result = new List<CartesianPos>();
            CartesianPos center1 = new CartesianPos();
            CartesianPos center2 = new CartesianPos();
            int count = 0;
            double gValue = double.MaxValue;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            set2.AddRange(SamplingData(data, samplingSize));
            pairKDTreeUpdate(model);
            for (int n = 0; n < iterativeCycle; n++)
            {
                count++;
                //Stopwatch sws = new Stopwatch();
                //sws.Start();
                //Find closet point of each new data points relative reference points
                set1.Clear();
                SearchNearestPoint(pairKDTree, set1, set2, searchDistance, rejectMode);
                //sws.Stop();
                //Console.WriteLine("[ICP Matching]Searching Time:{0:F4}", sws.Elapsed.TotalMilliseconds);

                //Calculate set center
                CalculateSetCenter(set1, set2, out center1, out center2);

                //Calculate transfromation
                CartesianPos trans = CalculateTransformation(set1, set2, center1, center2);
                transResult.x += trans.x;
                transResult.y += trans.y;
                transResult.theta += trans.theta;

                //Calculate MSE
                gValue = CalculateMSE(set1, set2, trans);

                //Calculate new data set
                result = null;
                result = CalculateNewDataSet(data, transResult, samplingSize);
                set2.Clear();
                set2.AddRange(result);

                //Checking MSE and convergence state 
                if (gValue <= mseThreshold)
                    break;

                if (Math.Abs(preError - gValue) <= convergenceThreshold)
                    break;

                preError = gValue;

            }
            //sw.Stop();
            //Console.WriteLine("[ICP Matching]Iterative Count:{0} Time:{1:F4}", count, sw.Elapsed.TotalMilliseconds);

            if (samplingSize != 0)
                result = CalculateNewDataSet(data, transResult, samplingSize);

            data.Clear();
            data.AddRange(result);
            pairKDTree = null;
            pairKDTree = new KDTree<CartesianPos>(2);
            return gValue;

        }

        /// <summary>
        /// Compute simlarity of two data sets
        /// </summary>
        /// <param name="modelSet"></param>
        /// <param name="dataSet"></param>
        /// <returns></returns>
        public double SimilarityEvalute(List<CartesianPos> modelSet, List<CartesianPos> dataSet)
        {
            double molecular = 0;
            double denominator1 = 0;
            double denominator2 = 0;
            for (int i = 0; i < modelSet.Count; i++)
            {
                molecular += modelSet[i].x * dataSet[i].x + modelSet[i].y * dataSet[i].y;
                denominator1 += modelSet[i].x * modelSet[i].x + modelSet[i].y * modelSet[i].y;
                denominator2 += dataSet[i].x * dataSet[i].x + dataSet[i].y * dataSet[i].y;
            }
            denominator1 = Math.Sqrt(denominator1);
            denominator2 = Math.Sqrt(denominator2);

            return molecular / (denominator1 * denominator2);
        }


        /// <summary>
        /// Data sampling
        /// </summary>
        /// <param name="data">Data set</param>
        /// <param name="samplingSize">Sampling step</param>
        /// <returns>Result</returns>
        private List<CartesianPos> SamplingData(List<CartesianPos> data, int samplingSize)
        {
            List<CartesianPos> result = new List<CartesianPos>();
            if (samplingSize != 0)
            {
                preError = double.MaxValue;
                for (int n = 0; n < data.Count; n += samplingSize)
                {
                    result.Add(data[n]);
                }
            }
            else
            {
                result.AddRange(data);
            }

            return result;
        }

        /// <summary>
        /// Search nearest point of model set relative to data set
        /// </summary>
        /// <param name="modelSet">Model set</param>
        /// <param name="dataSet">Data set</param>
        /// <param name="searchDistance">Distance threshold</param>
        private void SearchNearestPoint(KDTree<CartesianPos> kdTree, List<CartesianPos> modelSet,
            List<CartesianPos> dataSet, double searchDistance, bool rejectMode)
        {
            for (int i = 0; i < dataSet.Count; i++)
            {
                //Perform a nearest neighbour search around that point.
                var pIter = kdTree.NearestNeighbors(new double[] { dataSet[i].x, dataSet[i].y }, 1, searchDistance * searchDistance);
                CartesianPos minPos = null;
                if (pIter.MoveNext())
                    minPos = pIter.Current;


                if (minPos != null)
                {
                    modelSet.Add(minPos);
                    if (rejectMode)
                        addPair(minPos, dataSet[i]);
                }
                else
                {
                    dataSet.Remove(dataSet[i]);
                    i--;
                }
                minPos = null;
            }
            if (rejectMode)
                rejectPair(modelSet, dataSet);
        }

        /// <summary>
        /// Calculate set center
        /// </summary>
        /// <param name="modelSet">Model set</param>
        /// <param name="dataSet">Data set</param>
        /// <param name="modelCenter">Center of model</param>
        /// <param name="dataCenter">Center of data</param>
        private void CalculateSetCenter(List<CartesianPos> modelSet, List<CartesianPos> dataSet, out CartesianPos modelCenter, out CartesianPos dataCenter)
        {
            double avgX1 = 0, avgY1 = 0, avgX2 = 0, avgY2 = 0;
            for (int i = 0; i < modelSet.Count; i++)
            {
                avgX1 += modelSet[i].x;
                avgY1 += modelSet[i].y;
                avgX2 += dataSet[i].x;
                avgY2 += dataSet[i].y;
            }
            modelCenter = new CartesianPos(avgX1 / modelSet.Count, avgY1 / modelSet.Count);
            dataCenter = new CartesianPos(avgX2 / modelSet.Count, avgY2 / modelSet.Count);
        }

        /// <summary>
        /// Calculate mean square error of two data set
        /// </summary>
        /// <param name="modelSet">Model set</param>
        /// <param name="dataSet">Data set</param>
        /// <param name="transformation">Transformation</param>
        /// <returns>Mean square error</returns>
        private double CalculateMSE(List<CartesianPos> modelSet, List<CartesianPos> dataSet, CartesianPos transformation)
        {
            double nx = 0, ny = 0;
            double xValue = 0;
            double yValue = 0;
            double gValue = 0;
            for (int i = 0; i < modelSet.Count; i++)
            {
                nx = dataSet[i].x * Math.Cos(transformation.theta) - dataSet[i].y * Math.Sin(transformation.theta) + transformation.x;
                ny = dataSet[i].x * Math.Sin(transformation.theta) + dataSet[i].y * Math.Cos(transformation.theta) + transformation.y;
                xValue = modelSet[i].x - nx;
                yValue = modelSet[i].y - ny;
                gValue += Math.Sqrt(xValue * xValue + yValue * yValue);
            }
            gValue /= modelSet.Count;
            return gValue;
        }

        /// <summary>
        /// Calcualte transformation of two data set
        /// </summary>
        /// <param name="modelSet">Model set</param>
        /// <param name="dataSet">Data set</param>
        /// <param name="modelCenter">Center of model</param>
        /// <param name="dataCenter">Center of data</param>
        /// <returns>Transformation result</returns>
        private CartesianPos CalculateTransformation(List<CartesianPos> modelSet, List<CartesianPos> dataSet, CartesianPos modelCenter, CartesianPos dataCenter)
        {
            CartesianPos result = new CartesianPos();
            double molecular = 0, denominator = 0;
            for (int i = 0; i < modelSet.Count; i++)
            {
                molecular += ((modelSet[i].y - modelCenter.y) * (dataSet[i].x - dataCenter.x) - (modelSet[i].x - modelCenter.x) * (dataSet[i].y - dataCenter.y));
                denominator += ((modelSet[i].x - modelCenter.x) * (dataSet[i].x - dataCenter.x) + (modelSet[i].y - modelCenter.y) * (dataSet[i].y - dataCenter.y));
            }
            result.theta = Math.Atan2(molecular, denominator);

            result.x = modelCenter.x - (dataCenter.x * Math.Cos(result.theta) - dataCenter.y * Math.Sin(result.theta));
            result.y = modelCenter.y - (dataCenter.x * Math.Sin(result.theta) + dataCenter.y * Math.Cos(result.theta));
            return result;
        }

        /// <summary>
        /// Calculate new data set 
        /// </summary>
        /// <param name="dataSet">Data set</param>
        /// <param name="transformation">Transfrmation</param>
        /// <param name="samplingSize">Sampling step</param>
        /// <returns>New data set after transformation</returns>
        private List<CartesianPos> CalculateNewDataSet(List<CartesianPos> dataSet, CartesianPos transformation, int samplingSize)
        {
            List<CartesianPos> result = new List<CartesianPos>();
            double nx = 0, ny = 0;
            for (int i = 0; i < dataSet.Count; i += samplingSize)
            {
                nx = dataSet[i].x * Math.Cos(transformation.theta) - dataSet[i].y * Math.Sin(transformation.theta) + transformation.x;
                ny = dataSet[i].x * Math.Sin(transformation.theta) + dataSet[i].y * Math.Cos(transformation.theta) + transformation.y;
                result.Add(new CartesianPos(nx, ny));
            }
            return result;
        }

        /// <summary>
        /// Recording nearest point of model set relative to data set
        /// </summary>
        /// <param name="model">Model point</param>
        /// <param name="data">Data point</param>
        private void addPair(CartesianPos model, CartesianPos data)
        {
            List<CartesianPos> dataPoint;
            if (modelPointWRTdataPoint.TryGetValue(model, out dataPoint))
            {
                modelPointWRTdataPoint[model].Add(data);
            }
            else
            {
                modelPointWRTdataPoint.Add(model, new List<CartesianPos>() { data });
            }
        }

        /// <summary>
        /// Rejecting pair that has same model point
        /// </summary>
        /// <param name="modelSet">Model set</param>
        /// <param name="dataSet">Data set</param>
        private void rejectPair(List<CartesianPos> modelSet, List<CartesianPos> dataSet)
        {

            var removePair = modelPointWRTdataPoint.Where(kvp => kvp.Value.Count > 1)
                  .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            foreach (KeyValuePair<CartesianPos, List<CartesianPos>> pairCount in removePair)
            {
                for (int i = 0; i < pairCount.Value.Count; i++)
                {
                    modelSet.Remove(pairCount.Key);
                    dataSet.Remove(pairCount.Value[i]);
                }
            }
            modelPointWRTdataPoint.Clear();
        }

        /// <summary>
        /// Update model points into kd tree that used to search nearest point of pairwise matching
        /// </summary>
        /// <param name="modelSet"></param>
        private void pairKDTreeUpdate(List<CartesianPos> modelSet)
        {
            for (int i = 0; i < modelSet.Count; i++)
            {
                pairKDTree.AddPoint(new double[] { modelSet[i].x, modelSet[i].y }, modelSet[i]);
            }
        }

        #endregion

        #region - Map operation -

        /// <summary>
        /// Update new information into map
        /// </summary>
        /// <param name="newInfo"></param>
        public List<CartesianPos> GlobalMapUpdate(List<CartesianPos> newInfo)
        {
            int x = 0, y = 0;
            List<int> xRecord;
            List<CartesianPos> addedPoints = new List<CartesianPos>();
            for (int n = 0; n < newInfo.Count; n++)
            {

                y = (int)newInfo[n].y;
                x = (int)newInfo[n].x;

                if (!mapPoint.TryGetValue(y, out xRecord))
                {

                    //Create new x record
                    xRecord = new List<int>();
                    xRecord.Add(x);
                    mapPoint.Add(y, xRecord);
                    addedPoints.Add(new CartesianPos(x, y));
                    kdTree.AddPoint(new double[] { x, y }, new CartesianPos(x, y));
                }
                else
                {

                    //Insert data into specific index 
                    if (!isExists(xRecord, x))
                    {
                        xRecord.Insert(InsertIndex(ref xRecord, x), x);
                        mapPoint.Remove(y);
                        mapPoint.Add(y, xRecord);
                        addedPoints.Add(new CartesianPos(x, y));
                        kdTree.AddPoint(new double[] { x, y }, new CartesianPos(x, y));
                    }

                }
                xRecord = null;
            }
            return addedPoints;
        }

        /// <summary>
        /// Update new information into map
        /// </summary>
        /// <param name="newInfo"></param>
        public List<CartesianPos> IncrementalMapUpdate(List<CartesianPos> newInfo, double minDistance)
        {
            double x = 0, y = 0;
            List<CartesianPos> addedPoints = new List<CartesianPos>();
            for (int n = 0; n < newInfo.Count; n++)
            {
                x = newInfo[n].x;
                y = newInfo[n].y;

                //Perform a nearest neighbour search around that point.
                var pIter = kdTree.NearestNeighbors(new double[] { x, y }, 1, minDistance * minDistance);
                if (pIter.MoveNext()) continue;
                parseMap.Add(new CartesianPos(x, y));
                addedPoints.Add(new CartesianPos(x, y));

                BoundaryUpdate(x, y);

            }
            for (int i = 0; i < addedPoints.Count; i++)
            {
                kdTree.AddPoint(new double[] { addedPoints[i].x, addedPoints[i].y }, new CartesianPos(addedPoints[i].x, addedPoints[i].y));
            }
            return addedPoints;
        }

        /// <summary>
        /// Obtain number of corresponding points
        /// </summary>
        /// <param name="newInfo"></param>
        /// <param name="minDistance"></param>
        /// <returns>Correspoinding number</returns>
        public bool EstimateCorresponingPoints(List<CartesianPos> newInfo, double minDistance, int correspondThreshold, out int correspondNum, out List<CartesianPos> newPoint)
        {
            correspondNum = 0;
            double x = 0, y = 0;
            newPoint = new List<CartesianPos>();
            for (int n = 0; n < newInfo.Count; n++)
            {
                x = newInfo[n].x;
                y = newInfo[n].y;

                //Perform a nearest neighbour search around that point.
                var pIter = kdTree.NearestNeighbors(new double[] { x, y }, 1, minDistance * minDistance);
                if (pIter.MoveNext())
                {
                    correspondNum++;
                    continue;
                }
                newPoint.Add(new CartesianPos(x, y));
                BoundaryUpdate(x, y);
            }

            if (correspondNum >= correspondThreshold)
            {
                parseMap.AddRange(newPoint);
                for (int i = 0; i < newPoint.Count; i++)
                {
                    kdTree.AddPoint(new double[] { newPoint[i].x, newPoint[i].y }, new CartesianPos(newPoint[i].x, newPoint[i].y));
                }
                return true;
            }

            return false;
        }

        /// <summary>
        /// Update map boundary
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void BoundaryUpdate(double x, double y)
        {

            if (minimumPos.x > x)
                minimumPos.x = x;
            if (minimumPos.y > y)
                minimumPos.y = y;
            if (maximumPos.x < x)
                maximumPos.x = x;
            if (maximumPos.y < y)
                maximumPos.y = y;
        }

        /// <summary>
        /// Binary search for value is exists in current list
        /// </summary>
        /// <param name="xRecord"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        private bool isExists(List<int> xRecord, int searchValue)
        {
            int left = 0;
            int right = xRecord.Count - 1;
            while (left <= right)
            {
                int middle = (left + right) / 2;
                if (xRecord[middle] == searchValue)
                    return true;
                else if (xRecord[middle] < searchValue)
                    left = middle + 1;
                else if (xRecord[middle] > searchValue)
                    right = middle - 1;
            }
            return false;
        }

        /// <summary>
        /// Determine position to insert data
        /// </summary>
        /// <param name="">Line segments of current row</param>
        /// <param name="">Line segment of current update</param>
        /// <returns>Insert Position</returns>
        private int InsertIndex(ref List<int> row, int x)
        {
            int left = 0;
            int right = row.Count - 1;
            int middle = 0;
            do
            {
                if (left == right)
                    if (x > row[left])
                        return right + 1;
                    else
                        return right;

                if (Math.Abs(left - right) == 1)
                    if (x < row[left] && x < row[right])
                        return left;
                    else if (x > row[left] && x < row[right])
                        return left + 1;
                    else if (x > row[left] && x > row[right])
                        return right + 1;

                middle = (left + right) / 2;
                if (x > row[middle])
                    left = middle + 1;
                else
                    right = middle - 1;

            } while (true);

        }

        /// <summary>
        /// Obtain data sets in specific range
        /// </summary>
        /// <param name="row"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        private void SearchBoundary(List<int> xRecord, int minValue, int maxValue, out int start, out int end)
        {
            int left = 0;
            int right = xRecord.Count - 1;
            int middle = 0;
            start = -1;
            end = -1;
            //Search start index
            while (left <= right)
            {
                if (xRecord[left] >= minValue)
                {
                    start = left;
                    break;
                }

                middle = (left + right) / 2;
                if (xRecord[middle] == minValue)
                    break;
                else if (xRecord[middle] < minValue)
                    left = middle + 1;
                else if (xRecord[middle] > minValue)
                    right = middle - 1;

            }

            left = 0;
            right = xRecord.Count - 1;
            middle = 0;

            //Search start index
            while (left <= right)
            {
                if (xRecord[right] <= maxValue)
                {
                    end = right;
                    break;
                }
                middle = (left + right) / 2;
                if (xRecord[middle] == maxValue)
                    break;
                else if (xRecord[middle] < maxValue)
                    left = middle + 1;
                else if (xRecord[middle] > maxValue)
                    right = middle - 1;

            }


        }

        /// <summary>
        /// Search nearest point
        /// </summary>
        /// <param name="point">Reference point</param>
        /// <param name="range">Search Range</param>
        /// <returns></returns>
        private CartesianPos SearchNearestPoint(CartesianPos point, int range)
        {
            int x = (int)point.x;
            int y = (int)point.y;
            CartesianPos minPos = new CartesianPos();
            int minDistance = int.MaxValue;
            int distance;
            bool pointsExist = false;
            //int start = -1, end = -1;
            List<int> xRecord = new List<int>();
            for (int i = y - range; i < y + range; i++)
            {
                if (!mapPoint.TryGetValue(i, out xRecord))
                    continue;
                foreach (int xPos in xRecord)
                {
                    if (xPos >= x - range && xPos <= x + range)
                    {
                        pointsExist = true;
                        distance = Distance.EuclideanDistance(xPos, i, x, y);
                        if (minDistance > distance)
                        {
                            minDistance = distance;
                            minPos.x = xPos;
                            minPos.y = i;
                        }
                    }
                }
                //SearchBoundary(xRecord, x - range, x + range, out start, out end);
                //if (start == -1 || end == -1)
                //    continue;

                //for (int n = start; n < end; n++)
                //{
                //    pointsExist = true;
                //    distance = Distance.EuclideanDistance(xRecord[n], i, x, y);
                //    if (minDistance > distance)
                //    {
                //        minDistance = distance;
                //        minPos.x = xRecord[n];
                //        minPos.y = i;
                //    }
                //}
            }

            if (pointsExist)
                return minPos;
            else
                return null;

        }

        #endregion

        #region - Line segment operation and classification -

        /// <summary>
        /// Split scanning data and merge into several line segments
        /// </summary>
        /// <param name="data">Scaning set</param>
        /// <returns>Data set of line segments</returns>
        public List<LineSegment> DataSplitMerge(List<CartesianPos> obstaclePos, int neighborDistanceThreshold, int allowSegmentDistance)
        {
            int index = 0;
            double nDistance = 0;
            List<LineSegment> lineSegment = new List<LineSegment>();
            LineSegment line;
            for (int i = 0; i < obstaclePos.Count - 1; i++)
            {

                #region 1.Inspect current point is single or group

                nDistance = Distance.EuclideanDistance(obstaclePos[i].x, obstaclePos[i].y,
                    obstaclePos[i + 1].x, obstaclePos[i + 1].y);
                line = null;
                line = new LineSegment();
                line.Add(obstaclePos[i]);
                index = 0;

                #endregion

                #region 2.if neighbor distance is smaller threshold,start to record segment points

                while (nDistance <= neighborDistanceThreshold)
                {
                    line.Add(obstaclePos[i + index + 1]);
                    index++;

                    if (i + index + 1 >= obstaclePos.Count)
                        break;
                    nDistance = Distance.EuclideanDistance(obstaclePos[i + index].x, obstaclePos[i + index].y,
                                obstaclePos[i + index + 1].x, obstaclePos[i + index + 1].y);
                }

                #endregion

                #region 3.If index is over or equal to 1.Save line segment.

                if (index >= 1)
                {
                    if (line.GetHeadTailLength() >= allowSegmentDistance)
                        lineSegment.Add(line);
                    i += index + 1;
                }

                #endregion

            }
            return lineSegment;
        }

        /// <summary>
        /// Split line segment has corner points
        /// </summary>
        public void LineSegmentDeepSplit(List<LineSegment> linesegment, int trunPointDistanceThreshold)
        {
            double len1 = 0, len2 = 0, len3 = 0;
            double theta = 0;
            double normalDisatnce = 0;
            double maximum = 0;
            int turnIndex = 0;

            for (int n = 0; n < linesegment.Count; n++)
            {
                CartesianPos startPoint = linesegment[n].startPoint;
                CartesianPos endPoint = linesegment[n].endPoint;
                len1 = Distance.EuclideanDistance(startPoint.x, startPoint.y, endPoint.x, endPoint.y);

                #region 1.Start search maximum distance of normal line relative to each points

                turnIndex = 0;
                normalDisatnce = 0;
                maximum = 0;
                for (int i = 1; i < linesegment[n].point.Count - 1; i++)
                {
                    len2 = Distance.EuclideanDistance(startPoint.x, startPoint.y, linesegment[n].point[i].x, linesegment[n].point[i].y);
                    len3 = Distance.EuclideanDistance(linesegment[n].point[i].x, linesegment[n].point[i].y, endPoint.x, endPoint.y);
                    theta = Trigonometric.CosinRuleForTheta(len1, len2, len3);
                    normalDisatnce = Trigonometric.OppositeLength(len2, theta);
                    if (normalDisatnce >= trunPointDistanceThreshold)
                        if (normalDisatnce > maximum)
                        {
                            maximum = normalDisatnce;
                            turnIndex = i;
                        }
                }

                #endregion

                #region 2.Split into new segment and remove origin segment

                if (turnIndex != 0)
                {
                    LineSegment newSegment1 = new LineSegment();
                    LineSegment newSegment2 = new LineSegment();
                    for (int i = 0; i < linesegment[n].point.Count; i++)
                    {
                        if (i <= turnIndex)
                            newSegment1.Add(linesegment[n].point[i]);

                        if (i >= turnIndex)
                            newSegment2.Add(linesegment[n].point[i]);
                    }

                    newSegment1.GetHeadTailLength();
                    linesegment.Add(newSegment1);
                    newSegment2.GetHeadTailLength();
                    linesegment.Add(newSegment2);

                    linesegment.Remove(linesegment[n]);
                    n--;
                }


                #endregion

            }

        }

        /// <summary>
        /// Search pairwise line segments 
        /// </summary>
        /// <param name="refSegment">Reference line segment</param>
        /// <param name="matchSegment">Match line sement</param>
        /// <returns>Pairwise line segments</returns>
        public List<LinePairwise> SearchPairwiseLineSegment(List<LineSegment> refSegment, List<LineSegment> matchSegment
            , int deviationThreshold, int distanceThreshold)
        {
            List<LinePairwise> pairwiseSet = new List<LinePairwise>();
            double minDeviation = double.MaxValue;
            double deviation = 0;
            double distance = 0;
            int minIndex = -1;
            for (int s2 = 0; s2 < matchSegment.Count; s2++)
            {
                minIndex = -1;
                minDeviation = double.MaxValue;
                for (int s1 = 0; s1 < refSegment.Count; s1++)
                {
                    deviation = Math.Abs(matchSegment[s2].headTailLength - refSegment[s1].headTailLength);
                    distance = Distance.EuclideanDistance(matchSegment[s2].startPoint.x, matchSegment[s2].startPoint.y,
                        refSegment[s1].startPoint.x, refSegment[s1].startPoint.y);
                    if (deviation <= deviationThreshold && distance <= distanceThreshold)
                    {
                        if (minDeviation > deviation)
                        {
                            minIndex = s1;
                            minDeviation = deviation;
                        }
                    }
                }

                if (minIndex != -1)
                    pairwiseSet.Add(new LinePairwise(refSegment[minIndex], matchSegment[s2]));
            }
            return pairwiseSet;
        }

        /// <summary>
        /// Result classification and select best group as transformation result
        /// </summary>
        /// <param name="results">None Classifiction Data</param>
        /// <param name="goupThreshold">Classify Parameter</param>
        /// <param name="mode">Classify Mode 0:Degree 1:Distance</param>
        /// <returns>Best group</returns>
        public RTGroup ClassificationandSearchBestGroup(List<Transformation> results, int goupThreshold, int mode)
        {
            List<RTGroup> rtGroup = new List<RTGroup>();
            Transformation result;
            int maxCount = 0;
            int maxIndex = 0;

            #region - Classification -

            for (int n = 0; n < results.Count; n++)
            {
                bool classify = false;
                //Group classification following center deviation.Create new group if not found appropriate group.
                for (int gIndex = 0; gIndex < rtGroup.Count; gIndex++)
                {
                    result = rtGroup[gIndex].GetCenter();
                    if (mode == 0)
                    {
                        if (Math.Abs(results[n].transT - result.transT) <= goupThreshold)
                        {
                            classify = true;
                            rtGroup[gIndex].RT.Add(results[n]);
                            break;
                        }
                    }
                    else
                    {
                        if (Distance.EuclideanDistance(result.transX, result.transY, results[n].transX, results[n].transY) <= goupThreshold)
                        {
                            classify = true;
                            rtGroup[gIndex].RT.Add(results[n]);
                            break;
                        }
                    }
                }

                //If no appropriate group or first data.Create new group
                if ((!classify && rtGroup.Count != 0) || rtGroup.Count == 0)
                    rtGroup.Add(new RTGroup(new List<Transformation> { results[n] }));

            }

            #endregion

            #region - Select maximum quantity of group as best result -

            if (rtGroup.Count > 0)
            {
                for (int i = 0; i < rtGroup.Count; i++)
                {
                    if (maxCount < rtGroup[i].RT.Count)
                    {
                        maxCount = rtGroup[i].RT.Count;
                        maxIndex = i;
                    }
                }

                return rtGroup[maxIndex];
            }
            else
                return null;

            #endregion

        }


        #endregion

        #region - Transformation operation -

        /// <summary>
        /// Trans current data set with transformation to new data set
        /// </summary>
        /// <param name="current">Current data set</param>
        /// <param name="transX">X transformation</param>
        /// <param name="transY">Y transformation</param>
        /// <param name="transTheta">Theta transformation</param>
        /// <returns>New data set</returns>
        public void NewPosTransformation(List<CartesianPos> current, double transX, double transY, double transTheta)
        {
            for (int i = 0; i < current.Count; i++)
            {
                double nx = current[i].x * Math.Cos(transTheta) - current[i].y * Math.Sin(transTheta) + transX;
                double ny = current[i].x * Math.Sin(transTheta) + current[i].y * Math.Cos(transTheta) + transY;
                current[i].x = nx;
                current[i].y = ny;
            }
        }

        /// <summary>
        /// Trans current data set with transformation to new data set
        /// </summary>
        /// <param name="current">Current data set</param>
        /// <param name="transX">X transformation</param>
        /// <param name="transY">Y transformation</param>
        /// <param name="transTheta">Theta transformation</param>
        /// <returns>New data set</returns>
        public void NewPosTransformation(CartesianPos current, double transX, double transY, double transTheta)
        {
            double nx = current.x * Math.Cos(transTheta) - current.y * Math.Sin(transTheta) + transX;
            double ny = current.x * Math.Sin(transTheta) + current.y * Math.Cos(transTheta) + transY;
            current.x = nx;
            current.y = ny;
            current.theta += transTheta;
        }

        /// <summary>
        /// Trans current data set with transformation to new data set
        /// </summary>
        /// <param name="current">Current data set</param>
        /// <param name="transX">X transformation</param>
        /// <param name="transY">Y transformation</param>
        /// <param name="transTheta">Theta transformation</param>
        /// <returns>New data set</returns>
        public List<CartesianPos> NewPosLocalTransformation(List<CartesianPos> current, double transX, double transY, double transTheta)
        {
            double cx = 0, cy = 0;
            List<CartesianPos> newPos = new List<CartesianPos>();
            for (int i = 0; i < current.Count; i++)
            {
                cx += current[i].x;
                cy += current[i].y;
            }
            cx /= current.Count;
            cy /= current.Count;

            //All points refer group center to rotate and translate
            for (int i = 0; i < current.Count; i++)
            {
                double diffx = current[i].x - cx;
                double diffy = current[i].y - cy;
                double distance = Math.Sqrt(Math.Pow(diffx, 2) + Math.Pow(diffy, 2));
                double otheta = Math.Atan2(diffy, diffx);
                double x = cx + distance * Math.Cos(otheta + transTheta) + transX;
                double y = cy + distance * Math.Sin(otheta + transTheta) + transY;
                newPos.Add(new CartesianPos(x, y));
            }
            return newPos;
        }

        /// <summary>
        /// Get average transformation of pairwise list
        /// </summary>
        /// <param name="pairwise"></param>
        /// <returns></returns>
        public Transformation CalculateRegidTransformation(List<LinePairwise> pairwise)
        {
            List<Transformation> results = new List<Transformation>();
            for (int n = 0; n < pairwise.Count; n++)
            {
                CartesianPos startP1 = pairwise[n].set1.startPoint;
                CartesianPos endP1 = pairwise[n].set1.endPoint;
                CartesianPos startP2 = pairwise[n].set2.startPoint;
                CartesianPos endP2 = pairwise[n].set2.endPoint;

                double centerX1 = (startP1.x + endP1.x) / 2;
                double centerY1 = (startP1.y + endP1.y) / 2;
                double centerX2 = (startP2.x + endP2.x) / 2;
                double centerY2 = (startP2.y + endP2.y) / 2;
                double theta1 = Math.Atan2(startP1.y - endP1.y, startP1.x - endP1.x);
                double theta2 = Math.Atan2(startP2.y - endP2.y, startP2.x - endP2.x);
                double diffTheta = theta1 - theta2;

                List<CartesianPos> set1 = new List<CartesianPos>() { startP1, endP1 };
                List<CartesianPos> set2 = new List<CartesianPos>() { startP2, endP2 };
                set1 = NewPosLocalTransformation(set1, 0, 0, diffTheta);
                set2 = NewPosLocalTransformation(set2, 0, 0, diffTheta);

                //centerX1 = (set1[0].x + set1[1].x) / 2;
                //centerY1 = (set1[0].y + set1[1].y) / 2;
                //centerX2 = (set2[0].x + set2[1].x) / 2;
                //centerY2 = (set2[0].y + set2[1].y) / 2;

                results.Add(new Transformation(centerX1 - centerX2, centerY1 - centerY2, diffTheta));

            }
            RTGroup bestGroup = ClassificationandSearchBestGroup(results, 1, 0);
            return bestGroup.GetCenter();
        }

        #endregion

        #endregion

        #region - Data Type -



        public class LineSegment
        {
            #region - Member -

            public double totalLength;
            public double headTailLength;
            public CartesianPos startPoint;
            public CartesianPos endPoint;
            public List<CartesianPos> point = new List<CartesianPos>();

            #endregion

            #region - Constructor -

            public LineSegment()
            {

            }

            /// <summary>
            /// Assign line total length 
            /// </summary>
            /// <param name="length"></param>
            /// <param name="point"></param>
            public LineSegment(List<CartesianPos> point)
            {
                this.point = point;
                if (point.Count > 0)
                {
                    startPoint = point[0];
                    endPoint = point[point.Count - 1];
                }
            }

            #endregion

            #region - Method -

            /// <summary>
            /// Add new point into line segment
            /// </summary>
            /// <param name="point"></param>
            public void Add(CartesianPos pos)
            {
                if (point.Count == 0)
                    startPoint = pos;
                else
                    endPoint = pos;

                point.Add(pos);
            }

            /// <summary>
            /// Obtain total length of line segment
            /// </summary>
            /// <returns>Length</returns>
            public double GetTotalLength()
            {
                double length = 0;
                if (point.Count > 0)
                {
                    for (int i = 0; i < point.Count - 1; i++)
                    {
                        length += Distance.EuclideanDistance(point[i].x, point[i].y, point[i + 1].x, point[i + 1].y);
                    }
                    totalLength = length;
                }
                return length;
            }

            /// <summary>
            /// Obtain length from start point to end point
            /// </summary>
            /// <returns>Length</returns>
            public double GetHeadTailLength()
            {
                double length = 0;
                if (point.Count > 0)
                {
                    length = Distance.EuclideanDistance(point[0].x, point[0].y, point[point.Count - 1].x, point[point.Count - 1].y);
                    headTailLength = length;
                }
                return length;
            }
            #endregion
        }

        public class LinePairwise
        {
            public LineSegment set1 = new LineSegment();
            public LineSegment set2 = new LineSegment();

            public LinePairwise(LineSegment refSet, LineSegment curSet)
            {
                set1 = refSet;
                set2 = curSet;
            }
        }

        public class Transformation
        {
            public double transX;
            public double transY;
            public double transT;

            public Transformation(double x, double y, double theta)
            {
                transX = x;
                transY = y;
                transT = theta;
            }
        }

        public class RTGroup
        {
            #region - Member -

            public List<Transformation> RT = new List<Transformation>();

            #endregion

            #region - Constructor -

            public RTGroup(List<Transformation> rt)
            {
                RT = rt;
            }

            #endregion

            #region - Method -

            /// <summary>
            /// Get transformation value of group center
            /// </summary>
            /// <param name="x">X translation</param>
            /// <param name="y">Y translation</param>
            /// <param name="t">Theta rotation</param>
            public void GetCenter(out double cx, out double cy, out double ct)
            {
                double x = 0, y = 0, t = 0;
                for (int i = 0; i < RT.Count; i++)
                {
                    x += RT[i].transX;
                    y += RT[i].transY;
                    t += RT[i].transT;
                }
                x /= RT.Count;
                y /= RT.Count;
                t /= RT.Count;
                cx = x;
                cy = y;
                ct = t;
            }

            /// <summary>
            /// Get transformation value of group center
            /// </summary>
            /// <param name="x">X translation</param>
            /// <param name="y">Y translation</param>
            /// <param name="t">Theta rotation</param>
            public Transformation GetCenter()
            {
                double x = 0, y = 0, t = 0;
                for (int i = 0; i < RT.Count; i++)
                {
                    x += RT[i].transX;
                    y += RT[i].transY;
                    t += RT[i].transT;
                }
                x /= RT.Count;
                y /= RT.Count;
                t /= RT.Count;
                return new Transformation(x, y, t);
            }

            #endregion

        }

        #endregion

    }

    public class CartesianPos
    {
        #region - Member -

        public double x;
        public double y;
        public double theta;

        #endregion

        #region - Constrcutor -

        public CartesianPos()
        {

        }

        public CartesianPos(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public CartesianPos(double x, double y, double theta)
        {
            this.x = x;
            this.y = y;
            this.theta = theta;
        }

        #endregion

        #region - Method -

        public void SetPosition(double x, double y, double theta)
        {
            this.x = x;
            this.y = y;
            this.theta = theta;
        }

        #endregion


    }

    public class CartesianPosInfo : CartesianPos
    {
        public CartesianPosInfo(double x, double y, double theta, string name, uint id)
        {
            this.x = x;
            this.y = y;
            this.theta = theta;
            this.name = name;
            this.id = id;
        }
        public string name;
        public uint id;
    }

    public class MapLine
    {
        #region - Member -

        public CartesianPos start;
        public CartesianPos end;

        #endregion

        #region - Constrcutor -

        public MapLine()
        {

        }

        public MapLine(double x1, double y1, double x2, double y2) : this(new CartesianPos(x1, y1), new CartesianPos(x2, y2))
        {

        }

        public MapLine(CartesianPos start, CartesianPos end)
        {
            this.start = start;
            this.end = end;
        }


        #endregion

    }
}
