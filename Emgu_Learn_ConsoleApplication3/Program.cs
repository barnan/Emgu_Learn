using System;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;
using Emgu.CV.UI;

namespace Emgu_Learn_ConsoleApplication3
{
    class Program
    {
        static void Main(string[] args)
        {
            // Version 1:
            //string win1 = "AblakNeve";
            //CvInvoke.NamedWindow(win1);

            //Mat imgMat = new Mat(400, 400, Emgu.CV.CvEnum.DepthType.Cv8U, 3);
            //imgMat.SetTo(new Bgr(255, 0, 0).MCvScalar);

            //CvInvoke.PutText(imgMat, "Hello Norbika!!", new Point(100, 100), Emgu.CV.CvEnum.FontFace.HersheyTriplex, 0.5, new Bgr(0, 255, 255).MCvScalar);


            //CvInvoke.Imshow(win1, imgMat);
            //CvInvoke.WaitKey(1000);
            //CvInvoke.DestroyWindow(win1);



            // Version 2: 

            using (Mat imgMat = new Mat(400, 400, Emgu.CV.CvEnum.DepthType.Cv8U, 3))
            {
                imgMat.SetTo(new Bgr(255, 0, 0).MCvScalar);

                CvInvoke.PutText(imgMat, "Hello Norbika!!", new Point(100, 100), Emgu.CV.CvEnum.FontFace.HersheyTriplex, 0.5, new Bgr(0, 255, 255).MCvScalar);

                ImageViewer.Show(imgMat, "Kép");
            }


            Console.ReadKey();
        }
    }
}
