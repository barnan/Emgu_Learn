using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.UI;

namespace CameraCapture
{
    class Program
    {
        static void Main(string[] args)
        {
            ImageViewer viewer = new ImageViewer();
            Capture capture = new Capture(Emgu.CV.CvEnum.CaptureType.Any);


        }
    }
}
