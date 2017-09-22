using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MapGL
{
    public partial class Form2 : Form
    {
        public class Node
        {
            public int x;
            public int y;
            public Node parent;

            public Node() : this(0, 0, null) { }

            public Node(int posX, int posY, Node pos)
            {
                x = posX;
                y = posY;
                parent = pos;
            }
        }

        private char[] mSplit = { ':', ',' };

        private EventSample sample;

        private List<Point> ObstaclePoint = new List<Point>();
        private List<CastecMapUI.Line> ObstacleLine = new List<CastecMapUI.Line>();

        private List<Node> posN = new List<Node>();

        public Form2()
        {
            InitializeComponent();

            //GetPointsFromFile("D:\\_D1703\\MapFiles\\CASTEC_Factory_2.map");//Floor3.map
            GetLinesFromFile("D:/Projects/MapFiles/CASTEC_Factory_Line.map");//Floor3.map

            sample = new EventSample();
            sample.TestValueChanged += new EventSample.TestValueHandler(sample_TestValueChanged);

            posN.Add(new Node(0, 0, null));
            for (int i = 1; i < 10; ++i)
            {
                posN.Add(new Node(i, i * 2, posN[i - 1]));
            }

            Node pare = null;
            pare = posN[9].parent;

            do
            {
                pare = pare.parent;
                //if(pare != null)
                    //Console.WriteLine("({0} , {1})", pare.x, pare.y);
            }
            while (pare != null);
        }

        private void sample_TestValueChanged(object sender, Boolean value)
        {
            MessageBox.Show(String.Format("測試值為:{0}", value));
        }

        /// <summary> 讀 *.map 檔（這個不可以寫在 CastecMapUI 裡面）
        /// </summary>
        /// <param name="filePath">檔案路徑</param>
        private void GetPointsFromFile(string filePath)
        {
            string[] pointArr;

            if (filePath != "")
            {
                string[] pointINfo = File.ReadAllLines(filePath);

                for (int i = 0; i < pointINfo.Length; i++)
                {
                    pointArr = null;
                    pointArr = pointINfo[i].Split(mSplit);

                    if (pointArr[0] == "Minimum Position")
                    {
                        castecMapUI1.MinPos = new Point(int.Parse(pointArr[1]) * castecMapUI1.Resolution, int.Parse(pointArr[2]) * castecMapUI1.Resolution);
                    }
                    else if (pointArr[0] == "Maximum Position")
                    {
                        castecMapUI1.MaxPos = new Point(int.Parse(pointArr[1]) * castecMapUI1.Resolution, int.Parse(pointArr[2]) * castecMapUI1.Resolution);
                    }
                    else if (pointArr[0] == "Obstacle Position") { }
                    else
                        ObstaclePoint.Add(new Point(int.Parse(pointArr[0]) * castecMapUI1.Resolution, int.Parse(pointArr[1]) * castecMapUI1.Resolution));
                    //castecMapUI1.AddObstaclePoint(int.Parse(pointArr[0]) * castecMapUI1.Resolution, int.Parse(pointArr[1]) * castecMapUI1.Resolution);

                }

                castecMapUI1.DrawPoints(ObstaclePoint, Color.Maroon, "Map", 2, true);

                pointArr = null;
            }
        }

        /// <summary> 讀 *.map 檔（這個不可以寫在 CastecMapUI 裡面）
        /// </summary>
        /// <param name="filePath">檔案路徑</param>
        private void GetLinesFromFile(string filePath)
        {
            string[] pointArr;

            if (filePath != "")
            {
                string[] pointINfo = File.ReadAllLines(filePath);

                for (int i = 0; i < pointINfo.Length; i++)
                {
                    pointArr = null;
                    pointArr = pointINfo[i].Split(mSplit);

                    if (pointArr[0] == "Minimum Position")
                    {
                        castecMapUI1.MinPos = new Point(int.Parse(pointArr[1]) * castecMapUI1.Resolution, int.Parse(pointArr[2]) * castecMapUI1.Resolution);
                    }
                    else if (pointArr[0] == "Maximum Position")
                    {
                        castecMapUI1.MaxPos = new Point(int.Parse(pointArr[1]) * castecMapUI1.Resolution, int.Parse(pointArr[2]) * castecMapUI1.Resolution);
                    }
                    else if (pointArr[0] == "Obstacle Lines") { }
                    else
                        ObstacleLine.Add(new CastecMapUI.Line(int.Parse(pointArr[0]) * castecMapUI1.Resolution, int.Parse(pointArr[1]) * castecMapUI1.Resolution, int.Parse(pointArr[2]) * castecMapUI1.Resolution, int.Parse(pointArr[3]) * castecMapUI1.Resolution));
                }

                castecMapUI1.DrawLines(ObstacleLine, Color.Maroon, "Map", true, 2);

                pointArr = null;
            }
        }

        private void castecMapUI1_MouseDown(object sender, MouseEventArgs e)
        {
            txtSetGoalX.Text = castecMapUI1.GLMousePoint.X.ToString();
            txtSetGoalY.Text = castecMapUI1.GLMousePoint.Y.ToString();
        }

        private void btnSetCarPosition_Click(object sender, EventArgs e)
        {
            castecMapUI1.PosCar = new CastecMapUI.Pos(double.Parse(txtSetCarX.Text), double.Parse(txtSetCarY.Text), double.Parse(txtSetCarTheta.Text));
        }

        private void btnSetGoalPosition_Click(object sender, EventArgs e)
        {
            //castecMapUI1.DrawPoint(int.Parse(txtSetGoalX.Text), int.Parse(txtSetGoalY.Text), Color.Red, 6, 6);
            castecMapUI1.AddPositonGoal(new CastecMapUI.Pos(double.Parse(txtSetGoalX.Text), double.Parse(txtSetGoalY.Text), double.Parse(txtSetGoalTheta.Text)));
        }

        private void btnClrGoal_Click(object sender, EventArgs e)
        {
            castecMapUI1.RemoveAllPositonGoal();
        }

        private void castecMapUI1_MouseSelectObj(string name, double x, double y)
        {
            string[] itemName = name.Split('_');

            // 地圖回傳誰被按下
            string msg = string.Format("Delete {0}({1},{2}) or not?", itemName[0], x, y);

            DialogResult dr = MessageBox.Show(msg, "Delete", MessageBoxButtons.OKCancel,
                MessageBoxIcon.Information);

            //判斷使用者是否要刪除點位
            if (dr == DialogResult.OK)
            {
                if (itemName[0] == CastecMapUI.ItemLayout.Goal.ToString())
                    castecMapUI1.RemovePositonGoal(uint.Parse(itemName[1]));

                if (itemName[0] == CastecMapUI.ItemLayout.Power.ToString())
                    castecMapUI1.RemovePositonPower(uint.Parse(itemName[1]));
            }
        }

        //private void castecMapUI1_MouseClickRealPos(CastecMapUI.Pos mouseRealPos)
        //{
        //    txtSetGoalX.Text = mouseRealPos.x.ToString();
        //    txtSetGoalY.Text = mouseRealPos.y.ToString();
        //    txtSetPowerX.Text = mouseRealPos.x.ToString();
        //    txtSetPowerY.Text = mouseRealPos.y.ToString();
        //}

        private void castecMapUI1_MouseClickRealPos(double posX, double posY)
        {
            txtSetGoalX.Text = posX.ToString();
            txtSetGoalY.Text = posY.ToString();
            txtSetPowerX.Text = posX.ToString();
            txtSetPowerY.Text = posY.ToString();
        }

        private void btnSetPowerPosition_Click(object sender, EventArgs e)
        {
            //castecMapUI1.DrawLine(0, 0, int.Parse(txtSetPowerX.Text), int.Parse(txtSetPowerY.Text), Color.Black, 1.0f, 6);
            //castecMapUI1.DrawPoint(int.Parse(txtSetPowerX.Text), int.Parse(txtSetPowerY.Text), Color.Orange, 15, 6);
            castecMapUI1.AddPositonPower(new CastecMapUI.Pos(double.Parse(txtSetPowerX.Text), double.Parse(txtSetPowerY.Text), double.Parse(txtSetPowerTheta.Text)));
        }

        private void btnClrPower_Click(object sender, EventArgs e)
        {
            castecMapUI1.RemoveAllPositonPower();
        }

        private void btnClearDraw_Click(object sender, EventArgs e)
        {

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //castecMapUI1.mEnableReDraw = true;
            Console.WriteLine("[MapGL - btnSave_Click] mEnableReDraw = true");
            sample.TestValue = true;
        }

        private void btnWhereCar_Click(object sender, EventArgs e)
        {
            castecMapUI1.WhereAmI();
        }

        private void btnWhereCar_Click_1(object sender, EventArgs e)
        {
            castecMapUI1.WhereAmI();
        }

        private void castecMapUI1_MouseSelectRange(int beginX, int beginY, int endX, int endY)
        {

        }
    }

    public class EventSample
    {
        public delegate void TestValueHandler(object sender, bool value);
        public event TestValueHandler TestValueChanged;

        public bool TestValue
        {
            set { OnTestValueChanged(value); }
        }

        protected virtual void OnTestValueChanged(bool value)
        {
            TestValueChanged(this, value);
        }

    }
}
