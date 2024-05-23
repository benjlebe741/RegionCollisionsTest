using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RegionCollisionsTest
{
    public partial class Form1 : Form
    {
        public static string debug;
        public static bool[] wasd = new bool[] { false, false, false, false };

        PhysicsObj player = new PhysicsObj(new Rectangle(100, 100, 20, 30));

        List<PointF> points = new List<PointF>();
        List<PointF[]> region1 = new List<PointF[]>();
        public static List<PointF[]> region2 = new List<PointF[]>();
        List<PointF[]> region3 = new List<PointF[]>();

        List<PointF[]> regionB1 = new List<PointF[]>();
        List<PointF[]> regionB2 = new List<PointF[]>();
        List<PointF[]> regionB3 = new List<PointF[]>();

        List<PointF[]> regionF1 = new List<PointF[]>();
        List<PointF[]> regionF2 = new List<PointF[]>();
        List<PointF[]> regionF3 = new List<PointF[]>();

        Rectangle rect = new Rectangle(0, 0, 20, 30);
        Point cursorPos;
        public Form1()
        {
            InitializeComponent();
        }


        private void gameTimer_Tick(object sender, EventArgs e)
        {
            cursorPos = PointToClient(Cursor.Position);
            button7.Text = debug;

            rect.Location = cursorPos;
            player.Move();
            Refresh();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(new SolidBrush(Color.BurlyWood), rect);

            Brush brush = new SolidBrush(Color.FromArgb(255, 255, 0, 255));
            e.Graphics.FillEllipse(brush, new Rectangle(cursorPos.X - 5, cursorPos.Y - 5, 10, 10));

            //Unnasigned Points
            brush = new SolidBrush(Color.FromArgb(205, 255, 0, 255));
            if (points.Count > 2)
            {
                e.Graphics.FillPolygon(brush, points.ToArray());
            }
            foreach (PointF p in points)
            {
                e.Graphics.FillEllipse(brush, new Rectangle((int)p.X - 4, (int)p.Y - 4, 8, 8));
            }

            //Assigned Points
            brush = new SolidBrush(Color.FromArgb(205, 205, 230, 215));
            paintRegion(region1, brush, e);

            brush = new SolidBrush(Color.FromArgb(205, 0, 230, 215));
            paintRegion(region2, brush, e);

            brush = new SolidBrush(Color.FromArgb(205, 205, 0, 0));
            paintRegion(region3, brush, e);

            e.Graphics.FillRectangle(new SolidBrush(Color.CadetBlue), player.body);
        }

        private void paintRegion(List<PointF[]> region, Brush brush, PaintEventArgs e)
        {
            if (region.Count != 0)
            {
                foreach (PointF[] p in region)
                {
                    if (p.Length > 2)
                    {
                        e.Graphics.FillPolygon(brush, p);
                    }

                    #region show collision working
                    for (int i = 0; i < p.Length; i++)
                    {
                        int j = i + 1;
                        if (j == p.Length) { j = 0; }

                        //Do I collide with a line?
                        //                       bool drawThis = lineIntersects(new Point((int)p[i].X, (int)p[i].Y), new Point((int)p[j].X, (int)p[j].Y), player.body);
                        bool drawThis = lineIntersects(new Point((int)p[i].X, (int)p[i].Y), new Point((int)p[j].X, (int)p[j].Y), rect);

                        //Yes:
                        if (drawThis)
                        {
                            e.Graphics.DrawLine(new Pen(new SolidBrush(Color.FromArgb(120, 0, 0, 0)), 2), p[i], p[j]);
                        }
                    }

                    #endregion


                }
            }
        }

        private void Form1_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        void addToRegion(List<PointF[]> pointList)
        {
            List<PointF> ghostPoints = points;
            pointList.Add(ghostPoints.ToArray());
            points.Clear();

        }
        private void button1_Click(object sender, EventArgs e)
        {
            addToRegion(region1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            addToRegion(region2);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            addToRegion(region3);
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            points.Add(cursorPos);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.X:
                    player.body.Location = cursorPos;
                    break;
                case Keys.W:
                    wasd[0] = true;
                    break;
                case Keys.A:
                    wasd[1] = true;
                    break;
                case Keys.S:
                    wasd[2] = true;
                    break;
                case Keys.D:
                    wasd[3] = true;
                    break;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    wasd[0] = false;
                    break;
                case Keys.A:
                    wasd[1] = false;
                    break;
                case Keys.S:
                    wasd[2] = false;
                    break;
                case Keys.D:
                    wasd[3] = false;
                    break;
            }
        }
        public static bool lineIntersects(Point _pOne, Point _pTwo, Rectangle _rect)
        {
            Point lrTop = new Point((_pOne.X < _pTwo.X) ? _pOne.X : _pTwo.X, (_pOne.Y < _pTwo.Y) ? _pOne.Y : _pTwo.Y);
            Point lrBottom = new Point((_pOne.X > _pTwo.X) ? _pOne.X : _pTwo.X, (_pOne.Y > _pTwo.Y) ? _pOne.Y : _pTwo.Y);

            Rectangle lineRect = new Rectangle(lrTop.X, lrTop.Y, lrBottom.X - lrTop.X, lrBottom.Y - lrTop.Y);
            if (lineRect.Width < 30) { lineRect.Width = 30; } //Protection from lines that are quite vertical
            if (lineRect.Height < 30) { lineRect.Height = 30; } //Protection from lines that are quite horizontal
            //First check, is it possible that the rectangle and line overlap, or is the rectangle further away from the line to the point where we dont need to go further.
            if (!_rect.IntersectsWith(lineRect))
            {
                return false;
            }

            Point leftMost = (_pOne.X < _pTwo.X) ? _pOne : _pTwo;
            Point rightMost = (leftMost == _pTwo) ? _pOne : _pTwo;

            bool increasingSlope = (leftMost.Y < rightMost.Y) ? true : false;

            //Find what points are inside the line-to-be-checked's rectangle, we can also narrow down the points to be checked based on slope
            List<Point> rectCorners;
            if (_rect.Contains(_pOne) || _rect.Contains(_pTwo))
            {
                rectCorners = new List<Point>
                {

                new Point(_rect.X + _rect.Width - 1, _rect.Y + _rect.Height - 1), //Bottom Right, sub
                new Point(_rect.X + 1, _rect.Y + 1),//Top Left
                                    new Point(_rect.X + 1, _rect.Y + _rect.Height - 1), // Bottom Left
                new Point(_rect.X + _rect.Width - 1, _rect.Y + 1), //Top Right
                };
            }
            else if (!increasingSlope)
            {
                rectCorners = new List<Point>
                {
                new Point(_rect.X + _rect.Width - 1,_rect.Y + _rect.Height - 1), //Bottom Right, sub
                new Point(_rect.X + 1,_rect.Y + 1),//Top Left
                //new Point(_rect.X,_rect.Y + _rect.Height),
                // new Point(_rect.X + _rect.Width,_rect.Y), 
                };
            }
            else
            {
                rectCorners = new List<Point>
                {
                //new Point(_rect.X,_rect.Y),
                new Point(_rect.X + 1,_rect.Y + _rect.Height - 1), // Bottom Left
                new Point(_rect.X + _rect.Width - 1,_rect.Y + 1), //Top Right
                //new Point(_rect.X + _rect.Width,_rect.Y + _rect.Height),
                };
            }



            for (int i = 0; i < rectCorners.Count; i++)
            {
                if (lineRect.Contains(rectCorners[i]))
                {
                    //Project this point onto the line-to-be-checked, both using the points x and y coord
                    int x = rectCorners[i].X;
                    int y = rectCorners[i].Y;

                    float deltaX = ((_pTwo.X - _pOne.X) == 0) ? ((_pTwo.X - _pOne.X) + (float)0.00001) : (_pTwo.X - _pOne.X);
                    float slope = (_pTwo.Y - _pOne.Y) / deltaX;
                    //Rearrange the line equation with those x and y coordinates as one of the unknowns, get this new point
                    Point subbedInX, subbedInY;

                    float newY = -((slope * (_pOne.X - x)) - _pOne.Y);
                    float newX = -(((_pOne.Y - y) / slope) - _pOne.X);

                    subbedInX = new Point(x, (int)newY);
                    subbedInY = new Point((int)newX, y);


                    //(Does this projected point exist inside the inputed rectangle) ? Intersection : Continue
                    bool returnTrue = false;
                    if (_rect.Contains(subbedInX)) //Y coord is new rects coord
                    {
                        returnTrue = true;
                        _rect.Y = subbedInX.Y - (y - _rect.Y);
                        //                        rect.Location = new Point(subbedInX.X - widthDifference, subbedInX.Y - heightDifference);
                    }
                    if (_rect.Contains(subbedInY)) //X coord is new rects coord
                    {

                        returnTrue = true;
                        _rect.X = subbedInY.X - (x - _rect.X);
                        //rect.Location = new Point(subbedInY.X - widthDifference, subbedInY.Y - heightDifference);  
                    }
                    if (returnTrue)
                    {
                        return true;
                    }
                }
            }


            return false;
        }
    }
}
