using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace RegionCollisionsTest
{
    public class PhysicsObj
    {
        public List<Force> forces = new List<Force>();
        const int GRAVITY = 3;
        int mass = 2;

        public Rectangle body = new Rectangle();
        public PhysicsObj(Rectangle _body)
        {
            body = _body;
        }

        void CombineForces()
        {
            float xComponent = 0;
            float yComponent = 0;

            foreach (Force f in forces)
            {
                xComponent += (f.magnitude * (float)Math.Cos(f.theta));
                yComponent += (f.magnitude * (float)Math.Sin(f.theta));
            }

            int tempX = (int)(xComponent * 1000);
            int tempY = (int)(yComponent * 1000);

            xComponent = tempX / 1000;
            yComponent = tempY / 1000;

            Form1.debug = xComponent + "\n" + yComponent;
            float magnitude = (float)Math.Sqrt((xComponent * xComponent) + (yComponent * yComponent));

            //Force startingForce = forces[0];
            //if (forces.Count > 1)
            //{
            //    for (int f = 1; f < forces.Count; f++)
            //    {
            //        //Add forces together using components
            //        double startingX = startingForce.magnitude * Math.Cos(startingForce.theta);
            //        double startingY = startingForce.magnitude * Math.Sin(startingForce.theta);
            //        double addedX = forces[f].magnitude * Math.Cos(forces[f].theta);
            //        double addedY = forces[f].magnitude * Math.Sin(forces[f].theta);

            //        double combinedX = startingX + addedX;
            //        combinedX = (combinedX == 0) ? 0.00001 : combinedX;
            //        double combinedY = startingY + addedY;

            //        //Find Theta and Magnitude of the new force, this is now starting force.
            //        float theta = (float)Math.Atan(combinedX / combinedY);
            //        //Quadrant 1, Right
            //        if (combinedX > -0.0 && combinedY > -0.0)
            //        {
            //            Form1.debug = "1";
            //        }
            //        //Quardrant 2, Left
            //        else if (combinedX < -0.0 && combinedY > -0.0)
            //        {
            //            theta = (float)Math.PI + theta;
            //            Form1.debug = "2";
            //        }
            //        //Quadrant 3, 
            //        else if (combinedX < -0.0 && combinedY < -0.0)
            //        {
            //            theta = theta + (float)Math.PI;
            //            Form1.debug = "3";
            //        }
            //        //Quadrant 4
            //        else if (combinedY < -0.0 && combinedX > -0.0)
            //        {
            //            theta = theta + (float)(3 * (Math.PI / 2));
            //            Form1.debug = "4";
            //        }
            //        float magnitude = (float)Math.Sqrt((combinedX * combinedX) + (combinedY * combinedY));

            //        //Set the starting force to continue
            //        startingForce = new Force(theta, magnitude);
            //    }
            //}

            //forces.Clear();
            //forces.Add(startingForce);
        }
        void Collisions(Rectangle rect)
        {
            //Clear forces
            forces.Clear();

            //Add all inputed forces and gravity
            if (Form1.wasd[3]) //Right 
            {
                forces.Add(new Force(0, 4));
            }
            if (Form1.wasd[0]) //Up
            {
                forces.Add(new Force((float)(3 * Math.PI / 2), 10));
            }
            if (Form1.wasd[1]) //Left
            {
                forces.Add(new Force((float)(Math.PI), 4));
            }
            if (Form1.wasd[2]) //Down
            {
                forces.Add(new Force((float)(Math.PI / 2), 10));
            }
            //Gravity
            forces.Add(new Force((float)(Math.PI / 2), mass * GRAVITY));

            //Combine these forces
            // CombineForces();

            //Now For Each Polygon, Find the normal force applied to the Body.
            foreach (PointF[] p in Form1.region2)
            {
                //For each line in the polygon
                for (int i = 0; i < p.Length; i++)
                {
                    int j = i + 1;
                    if (j == p.Length) { j = 0; }

                    //Do I collide with a line?
                    bool collision = Form1.lineIntersects(new Point((int)p[i].X, (int)p[i].Y), new Point((int)p[j].X, (int)p[j].Y), rect);

                    //Yes:
                    //if (collision)
                    //{
                    //    //Get Theta
                    //    float deltaX = (p[j].X - p[i].X);
                    //    deltaX = (deltaX == 0) ? (float)0.00001 : deltaX;
                    //    float deltaY = (p[j].Y - p[i].Y);
                    //    float slope = deltaY / deltaX;

                    //    float perpendicularChange = (float)(Math.PI / 2);
                    //    //float lineTop = (p[i].Y < p[j].Y) ? p[i].Y : p[j].Y;
                    //    //perpendicularChange *= ((rect.Y + (rect.Height / 2)) < lineTop) ? -1 : 1; //Am I below or above line?

                    //    float theta = (float)Math.Atan(slope) - perpendicularChange;

                    //    forces.Add(new Force(theta, GRAVITY * mass));
                    //}
                }
            }
        }
        public void Move()
        {
            Rectangle ghostBody = body;
            foreach (Force f in forces)
            {
                ghostBody.X += (int)(f.magnitude * Math.Cos(f.theta));
                ghostBody.Y += (int)(f.magnitude * Math.Sin(f.theta));
            }
            Collisions(ghostBody);

            body = ghostBody;
        }
    }
}
