using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 
using System.Threading.Tasks;

namespace RegionCollisionsTest
{ 
    public class Force
    {
        public float theta, magnitude;

        public Force(float _theta, float _magnitude) 
        {
            theta = _theta;
            magnitude = _magnitude;
        }
    }
}
