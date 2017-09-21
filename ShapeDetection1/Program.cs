using System;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using System.Collections.Generic;
using System.Diagnostics;

namespace ShapeDetection1
{
    class Program
    {
        public static object StopWatch { get; private set; }

        static void Main(string[] args)
        {

            Stopwatch swatch = new Stopwatch();
            swatch.Start();

            string fileName = "Opencvpic3sample.png";
            Image<Bgr, byte> image = new Image<Bgr, byte>(fileName);


            Console.WriteLine($"image reading: {swatch.ElapsedMilliseconds}");


            UMat matriksz = new UMat();
            CvInvoke.CvtColor(image, matriksz, ColorConversion.Bgr2Gray);

            //ImageViewer.Show(matriksz);

            Console.WriteLine($"image conversion: {swatch.ElapsedMilliseconds}");


            UMat pyrLe = new UMat();
            CvInvoke.PyrDown(matriksz, pyrLe, BorderType.Default);
            CvInvoke.PyrUp(pyrLe, matriksz, BorderType.Default);


            Console.WriteLine($"image pyramid: {swatch.ElapsedMilliseconds}");


            // circle detection:
            double cannyThreshold = 90.0;
            double circleAccumulatorThreshold = 60;

            CircleF[] circles = CvInvoke.HoughCircles(matriksz, HoughType.Gradient, 1.0, 20.0, cannyThreshold, circleAccumulatorThreshold, 20);

            // show found circles:
            Image<Bgr, byte> circleImage = new Image<Bgr, byte>(matriksz.Cols, matriksz.Rows);
            CvInvoke.cvCopy(image, circleImage, IntPtr.Zero);
            foreach (CircleF circle in circles)
            {
                circleImage.Draw(circle, new Bgr(0.0, 0.0, 0.0), 2);
            }
            string ablakNev2 = "circleAblak2";
            //CvInvoke.NamedWindow(ablakNev2);
            //CvInvoke.Imshow(ablakNev2, circleImage);
            //CvInvoke.WaitKey(2000);
            //CvInvoke.DestroyWindow(ablakNev2);


            Console.WriteLine($"circle search: {swatch.ElapsedMilliseconds}");


            // rectangle search:
            double cannyThresholdLinking = 100.0;
            UMat cannyEdges = new UMat();
            CvInvoke.Canny(matriksz, cannyEdges, cannyThreshold, cannyThresholdLinking, 3);

            //ImageViewer.Show(cannyEdges);

            LineSegment2D[] lines;
            lines = CvInvoke.HoughLinesP(cannyEdges, 1.0, Math.PI / 45.0, 20, 15, 10);


            // show found lines:
            Image<Bgr, byte> lineImage = new Image<Bgr, byte>(matriksz.Cols, matriksz.Rows);
            CvInvoke.cvCopy(image, lineImage, IntPtr.Zero);
            foreach (LineSegment2D line in lines)
            {
                lineImage.Draw(line, new Bgr(0.0, 0.0, 0.0), 2);
            }
            string ablakNev3 = "circleAblak3";
            //CvInvoke.NamedWindow(ablakNev3);
            //CvInvoke.Imshow(ablakNev3, lineImage);
            //CvInvoke.WaitKey(2000);
            //CvInvoke.DestroyWindow(ablakNev3);



            Console.WriteLine($"lines search: {swatch.ElapsedMilliseconds}");


            // find triangles and rectangles:
            List<Triangle2DF> triangles = new List<Triangle2DF>();
            List<RotatedRect> boxes = new List<RotatedRect>();







            Console.ReadKey();

        }
    }

}
