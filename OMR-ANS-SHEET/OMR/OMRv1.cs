﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.InteropServices;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;

namespace OMR
{
    public class OMRv1 : IOMR
    {
        struct MyPoint
        {
            public double X { get; set; }
            public double Y { get; set; }
            public double Rad { get; set; }// r
            public MyPoint(double x, double y, double rad) { X = x; Y = y; Rad = rad; }
        }
        struct MyRow
        {
            public MyPoint point { get; set; }
            public MyRow(MyPoint mp) { point = mp; }
        }

        (List<PointProperty> pointsList, List<int> rowSize) IOMR.GetPositionPoint(Bitmap bitmap, bool getCheck)
        {
            List<PointProperty> pointProperty = new List<PointProperty>();
            List<int> rowSize = new List<int>();
            /////////////////////////////////////////////////////////////////////////////////////////////
            Image<Rgb, byte> img = new Image<Rgb, byte>(bitmap);
            Image<Gray, byte> imageGray = img.Convert<Gray, byte>();
            UMat uimage = new UMat();
            CvInvoke.CvtColor(img, uimage, ColorConversion.Bgr2Gray);
            UMat pyrDown = new UMat();
            CvInvoke.PyrDown(uimage, pyrDown);
            CvInvoke.PyrUp(pyrDown, uimage);
            CircleF[] circles = CvInvoke.HoughCircles(uimage, HoughType.Gradient, 1, 20, 20, 15, 16, 18);
            Image<Rgb, Byte> circleImage = img.CopyBlank();
            double x;//row
            double y;//col
            double r;//rad
            var MyPoints = new List<MyPoint>();
            for (int i = 0; i < circles.Length; i++)
            {
                circleImage.Draw(circles[i], new Rgb(Color.Red), 2);
                x = Math.Round(circles[i].Center.X, 0);
                y = Math.Round(circles[i].Center.Y, 0);
                r = Math.Round(circles[i].Radius, 0);
                MyPoints.Add(new MyPoint(x, y, r)); 
            }
            MyPoints = MyPoints.OrderBy(item => item.Y).ToList();
            var SortRow = new List<MyPoint>();
            var Dist = MyPoints[0].Rad;
            var y_min = MyPoints[0].Y - Dist / 2;
            var y_max = MyPoints[0].Y + Dist / 2;
            var this_row = 1;
            var MyRows = new List<MyRow>();
            for (var i = 0; i < MyPoints.Count; i++)
            {
                if (MyPoints[i].Y <= y_max && MyPoints[i].Y >= y_min)
                {
                    MyRows.Add(new MyRow(MyPoints[i]));
                }
                else
                {
                    MyRows = MyRows.OrderBy(item => item.point.X).ToList();
                    foreach (var item in MyRows)
                    {
                        pointProperty.Add(new PointProperty(new Point((int)item.point.X, (int)item.point.Y), (int)Dist, false));
                    }
                    rowSize.Add(MyRows.Count);
                    MyRows.Clear();
                    y_min = MyPoints[i].Y - Dist / 2;
                    y_max = MyPoints[i].Y + Dist / 2;
                    this_row++;
                    MyRows.Add(new MyRow(MyPoints[i]));
                }
            }
            foreach (var item in MyRows)
            {
                pointProperty.Add(new PointProperty(new Point((int)item.point.X, (int)item.point.Y), (int)Dist, false));
            }
            rowSize.Add(MyRows.Count);
            if (getCheck) // check ans
            {
                /// start
                foreach (var item in pointProperty)
                {
                    img.Draw(new CircleF(new PointF((float)item.Position.X, (float)item.Position.Y), (float)item.Rad), new Rgb(Color.White), 9);
                }

                //for (int i = 0; i < SortRow.Count; i++)
                //{
                //    img.Draw(new CircleF(new PointF((float)SortRow[i].X, (float)SortRow[i].Y), (float)SortRow[i].Rad), new Rgb(Color.White), 9);
                //}
                CvInvoke.CvtColor(img, uimage, ColorConversion.Rgba2Gray);
                CvInvoke.PyrDown(uimage, pyrDown);
                CvInvoke.PyrUp(pyrDown, uimage);
                CvInvoke.AdaptiveThreshold(uimage, uimage, 255, Emgu.CV.CvEnum.AdaptiveThresholdType.MeanC, Emgu.CV.CvEnum.ThresholdType.Binary, 999, 99);//149,99,79,59,39,19
                CircleF[] circles2 = CvInvoke.HoughCircles(uimage, HoughType.Gradient, 1, 20, 20, 8, 9, 14);
                var AnsPoint = new List<MyPoint>();
                for (int i = 0; i < circles2.Length; i++)
                {
                    //circleImage.Draw(circles[i], new Rgb(Color.Red), 1);
                    img.Draw(circles2[i], new Rgb(Color.Green), 3);
                    x = Math.Round(circles2[i].Center.X, 0);
                    y = Math.Round(circles2[i].Center.Y, 0);
                    r = Math.Round(circles2[i].Radius, 0);
                    AnsPoint.Add(new MyPoint(x, y, r));
                }

                for (int i = 0; i < AnsPoint.Count; i++)
                {
                    //foreach (var item in pointProperty)
                    //{
                    //    if (AnsPoint[i].X < item.Position.X + item.Rad && AnsPoint[i].Y < item.Position.Y + item.Rad && AnsPoint[i].X > item.Position.X - item.Rad && AnsPoint[i].Y > item.Position.Y - item.Rad)
                    //    {
                    //        item.IsCheck=true;
                    //    }
                    //}
                    for (int k = 0; k < pointProperty.Count; k++)
                    {
                        if ( (AnsPoint[i].X < pointProperty[k].Position.X + pointProperty[k].Rad ) && ( AnsPoint[i].Y < pointProperty[k].Position.Y + pointProperty[k].Rad ) && ( AnsPoint[i].X > pointProperty[k].Position.X - pointProperty[k].Rad ) && ( AnsPoint[i].Y > pointProperty[k].Position.Y - pointProperty[k].Rad ) )
                        {
                            pointProperty[k] = new PointProperty(pointProperty[k].Position, pointProperty[k].Rad, true);
                        }
                    }
                }

                /// end
            }
            return (pointProperty, rowSize);
        }
    }
}
