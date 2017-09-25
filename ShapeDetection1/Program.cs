using System;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using System.Collections.Generic;
using System.Diagnostics;
using Emgu.CV.Util;
using System.Drawing;

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


            CvInvoke.Imshow("input", image);
            CvInvoke.WaitKey(1000);
            CvInvoke.DestroyWindow("input");
            Console.WriteLine($"image reading: {swatch.ElapsedMilliseconds}");


            Mat matriksz = new Mat();
            CvInvoke.CvtColor(image, matriksz, ColorConversion.Bgr2Gray);
            CvInvoke.Imshow("inputGray", matriksz);
            CvInvoke.WaitKey(1000);
            CvInvoke.DestroyWindow("inputGray");


            Console.WriteLine($"image conversion: {swatch.ElapsedMilliseconds}");


            //Mat pyrLe = new Mat();
            //CvInvoke.PyrDown(matriksz, pyrLe, BorderType.Default);
            //CvInvoke.PyrUp(pyrLe, matriksz, BorderType.Default);
            CvInvoke.GaussianBlur(matriksz, matriksz, new Size(5, 5), 1);
            CvInvoke.Imshow("GaussinputGray", matriksz);
            CvInvoke.WaitKey(1000);
            CvInvoke.DestroyWindow("GaussinputGray");


            Console.WriteLine($"image pyramid: {swatch.ElapsedMilliseconds}");


            // circle detection:
            double cannyThreshold = 50;
            double circleAccumulatorThreshold = 50;

            CircleF[] circles = CvInvoke.HoughCircles(matriksz, HoughType.Gradient, 1.0, 10.0, cannyThreshold, circleAccumulatorThreshold, 20);

            // show found circles:
            Image<Bgr, byte> circleImage = new Image<Bgr, byte>(matriksz.Cols, matriksz.Rows);
            CvInvoke.cvCopy(image, circleImage, IntPtr.Zero);
            foreach (CircleF circle in circles)
            {
                circleImage.Draw(circle, new Bgr(0.0, 0.0, 0.0), 2);
            }
            CvInvoke.Imshow("circles", circleImage);
            CvInvoke.WaitKey(1000);
            CvInvoke.DestroyWindow("circles");


            Console.WriteLine($"circle search: {swatch.ElapsedMilliseconds}");


            // line search:
            cannyThreshold = 100;
            double cannyThresholdLinking = 200;
            UMat cannyEdges = new UMat();
            CvInvoke.Canny(matriksz, cannyEdges, cannyThreshold, cannyThresholdLinking, 5);


            LineSegment2D[] lines;
            lines = CvInvoke.HoughLinesP(cannyEdges, 1.0, Math.PI / 45.0, 10, 30, 10);


            // show found lines:
            Image<Bgr, byte> lineImage = new Image<Bgr, byte>(matriksz.Cols, matriksz.Rows);
            CvInvoke.cvCopy(image, lineImage, IntPtr.Zero);
            foreach (LineSegment2D line in lines)
            {
                lineImage.Draw(line, new Bgr(0.0, 0.0, 0.0), 2);
            }
            CvInvoke.Imshow("lines", lineImage);
            CvInvoke.WaitKey(2000);
            CvInvoke.DestroyWindow("lines");


            Console.WriteLine($"lines search: {swatch.ElapsedMilliseconds}");


            // find triangles and rectangles:
            List<Triangle2DF> triangles = new List<Triangle2DF>();
            List<RotatedRect> boxes = new List<RotatedRect>();

            using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
            {

                CvInvoke.FindContours(cannyEdges, contours, null, RetrType.List, ChainApproxMethod.ChainApproxSimple);

                int contourCount = contours.Size;

                for (int i = 0; i < contourCount; i++)
                {

                    using (VectorOfPoint pointVector = contours[i])
                    using (VectorOfPoint approxContour = new VectorOfPoint())
                    {
                        CvInvoke.ApproxPolyDP(pointVector, approxContour, CvInvoke.ArcLength(pointVector, true) * 0.05, true);

                        Image<Gray, byte> plot2 = new Image<Gray, byte>(image.Width, image.Height);
                        plot2.Draw(approxContour.ToArray(), new Gray(100.0), 2);

                        //ImageViewer.Show(plot2);



                        if (CvInvoke.ContourArea(approxContour, false) > 150) //only consider contours with area greater than 250
                        {
                            if (approxContour.Size == 3)
                            {
                                Point[] points = approxContour.ToArray();
                                triangles.Add(new Triangle2DF(points[0], points[1], points[2]));
                            }
                            else if (approxContour.Size == 4)
                            {
                                bool isRectangle = true;
                                Point[] points = approxContour.ToArray();
                                LineSegment2D[] edges = PointCollection.PolyLine(points, true);

                                for (int j = 0; j < edges.Length; j++)
                                {
                                    double angle = Math.Abs(edges[(j + 1) % edges.Length].GetExteriorAngleDegree(edges[j]));
                                    if (angle < 80 || angle > 100)
                                    {
                                        isRectangle = false;
                                        break;
                                    }
                                }

                                if (isRectangle)
                                {
                                    RotatedRect rotRect = CvInvoke.MinAreaRect(approxContour);
                                    boxes.Add(rotRect);
                                }

                            }


                        }

                    }


                }

                if (triangles.Count > 0 || boxes.Count > 0)
                {

                    var ch1 = new Image<Gray, byte>(image.Width, image.Height);
                    var ch2 = new Image<Gray, byte>(image.Width, image.Height);
                    var ch3 = new Image<Gray, byte>(image.Width, image.Height);
                    var multichImage = new Image<Bgr, byte>(new Image<Gray, byte>[] { ch1, ch2, ch3 });

                    Image<Gray, byte> plot1 = new Image<Gray, byte>(new Image<Gray, byte>[] { ch1, ch2, ch3 });
                    CvInvoke.cvCopy(image, multichImage, IntPtr.Zero);

                    for (int j = 0; j < circles.Length; j++)
                    {
                        multichImage.Draw(new CircleF(circles[j].Center, circles[j].Radius), new Bgr(200.0, 50.0, 50.0), 2);
                    }

                    for (int j = 0; j < triangles.Count; j++)
                    {

                        plot1.Draw(triangles[j], new Gray(200.0), 2);
                    }

                    for (int j = 0; j < boxes.Count; j++)
                    {
                        plot1.Draw(boxes[j], new Gray(200.0), 2);
                    }

                    CvInvoke.Imshow("plot1", plot1);
                    CvInvoke.WaitKey(2000);
                    CvInvoke.DestroyWindow("plot1");

                    CvInvoke.Imshow("multichImage", multichImage);
                    CvInvoke.WaitKey(2000);
                    CvInvoke.DestroyWindow("multichImage");





                }



            }


            Console.ReadKey();

        }
    }

}
