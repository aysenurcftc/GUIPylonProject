using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myPylonProject.Models
{
    internal class CameraSetting
    {
        public double ExposureTime { get; set; }
        public double Gain { get; set; }
        public double Gamma { get; set; }
        public string PixelFormat { get; set; }
       
       public double BlackLevel { get; set; }


    }
}
