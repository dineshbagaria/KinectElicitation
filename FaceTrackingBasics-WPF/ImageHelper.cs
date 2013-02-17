using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;

using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Drawing;


using Microsoft.Kinect;

namespace Kinect.Tool
{
    class ImageHelper
    {
        
        public static Image<Gray, Byte> extractImageByHeadShoulder(ColorImageFrame colorFrame, DepthImageFrame depthFrame, Skeleton skeleton) 
        {
            // step compute the rect for cropping
            Joint head = skeleton.Joints[JointType.Head];
            Joint shoulderCenter = skeleton.Joints[JointType.ShoulderCenter];
            Joint shoulderLeft = skeleton.Joints[JointType.ShoulderLeft];
            Joint shoulderRight = skeleton.Joints[JointType.ShoulderRight];
            DepthImagePoint headPoint = depthFrame.MapFromSkeletonPoint(head.Position);
            DepthImagePoint shoulderCPoint = depthFrame.MapFromSkeletonPoint(shoulderCenter.Position);
            DepthImagePoint shoulderLPoint = depthFrame.MapFromSkeletonPoint(shoulderLeft.Position);
            DepthImagePoint shoulderRPoint = depthFrame.MapFromSkeletonPoint(shoulderRight.Position);
            
            int leftX = shoulderLPoint.X, rightX = shoulderRPoint.X;
            int width = rightX - leftX;
            if (width < 254)
            {
                int leftShift = (254 - width + 1) / 2;
                if (leftX - leftShift < 0) leftShift = leftX;
                int rightShift = 254 - width - leftShift;
                leftX = leftX - leftShift;
                rightX = rightX + rightShift;
            }

            int upY = headPoint.Y, downY = shoulderCPoint.Y;
            int height = downY - upY;
            upY = Math.Min(upY - height / 2, 0);
            downY = Math.Max(downY, upY + 255);

            //step 2 build image byte array

            var colorPixelData = new byte[colorFrame.PixelDataLength];
            colorFrame.CopyPixelDataTo(colorPixelData);
            var depthPixelData = new short[depthFrame.PixelDataLength];
            depthFrame.CopyPixelDataTo(depthPixelData);

        //    byte[] tarImage = new byte[(rightX - leftX + 1) * (downY - upY + 1) * 4];
        //   int tarImageIdx = 0;

            int tarPlayerIdx = headPoint.PlayerIndex;
            int playerIdx = 0;
            int rowOffset = 0;
            int depthIdx = 0;
            int colorIdx = 0;
            ColorImagePoint colorPoint;

            Bitmap bmp = new Bitmap(rightX - leftX + 1, downY - upY + 1, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            bmp.SetResolution(96, 96);
            int red, green, blue, alpha;

            for (int depthY = upY; depthY <= downY; depthY++)
            {
                rowOffset = depthFrame.Width * depthY;
                for (int depthX = leftX; depthX <= rightX; depthX++)
                {
                    blue = 0xFF;              //blue
                    green = 0xFF;          //green
                    red = 0xFF;          //red
                    alpha = 0xFF;          //alpha

                    depthIdx = depthX + rowOffset;
                    playerIdx = depthPixelData[depthIdx] & DepthImageFrame.PlayerIndexBitmask;

                    if (playerIdx == tarPlayerIdx)
                    {
                        colorPoint = depthFrame.MapToColorImagePoint(depthX, depthY, colorFrame.Format);
                        colorIdx = (colorPoint.X + colorPoint.Y * colorFrame.Width) * colorFrame.BytesPerPixel;
                        blue = colorPixelData[colorIdx];                  //blue
                        green = colorPixelData[colorIdx + 1];          //green
                        red = colorPixelData[colorIdx + 2];          //red
                        alpha = 0xFF;                                  //alpha
                        bmp.SetPixel(depthX - leftX, downY - upY, System.Drawing.Color.FromArgb(alpha, red, green, blue));
                    }
                }

               // tarImageIdx += 4;
            }

           

            Image<Gray, byte> image = new Image<Gray, byte>(bmp);
            
            return image;
        }




        //public static Image<Gray, Byte> extractImage(ColorImageFrame colorFrame, DepthImageFrame depthFrame, Skeleton skeleton)
        //{
        //    var colorPixelData = new byte[colorFrame.PixelDataLength];
        //    colorFrame.CopyPixelDataTo(colorPixelData);

        //    var depthPixelData = new short[depthFrame.PixelDataLength];
        //    depthFrame.CopyPixelDataTo(depthPixelData);


        //    Joint head = skeleton.Joints[JointType.Head];
        //    //Joint shoulderCenter = skeleton.Joints[JointType.ShoulderCenter];
        //    DepthImagePoint headPoint = depthFrame.MapFromSkeletonPoint(head.Position);
        //    int targetUserIdx = headPoint.PlayerIndex;

        //    int userIdx = 0;
        //    int colorStride = colorFrame.Width * colorFrame.BytesPerPixel;
        //    //int depthStride = depthFrame.Width * depthFrame.BytesPerPixel;
        //    byte[] tarImage = new byte[colorPixelData.Length];

        //    int tarImageIdx = 0;
        //    int depthIdx = 0;
        //    int colorIdx = 0;
        //    ColorImagePoint colorPoint;

        //    int minWidth = int.MaxValue, maxWidth = int.MinValue;
        //    int minHeight = int.MaxValue, maxHeight = int.MinValue;

        //    int curRowIdx = 0;
        //    for ( int depthY = 0; depthY < depthFrame.Height; depthY++ )
        //    {
        //        curRowIdx = depthFrame.Width * depthY;
        //        for (int depthX = 0; depthX < depthFrame.Width; depthX++ )
        //        {
        //            tarImage[tarImageIdx] = 0xFF;              //blue
        //            tarImage[tarImageIdx + 1] = 0xFF;          //green
        //            tarImage[tarImageIdx + 2] = 0xFF;          //red
        //            tarImage[tarImageIdx + 3] = 0xFF;       //alpha

        //            depthIdx = depthX + curRowIdx;
        //            userIdx = depthPixelData[depthIdx] & DepthImageFrame.PlayerIndexBitmask;

        //            if (userIdx == targetUserIdx)
        //            {
        //                colorPoint = depthFrame.MapToColorImagePoint(depthX, depthY, colorFrame.Format);
                        
        //                minWidth = Math.Min(colorPoint.X, minWidth);
        //                maxWidth = Math.Max(colorPoint.X, maxWidth);
        //                minHeight = Math.Min(colorPoint.Y, minHeight);
        //                maxHeight = Math.Max(colorPoint.Y, maxHeight);

        //                colorIdx = (colorPoint.X + colorPoint.Y * colorFrame.Width) * colorFrame.BytesPerPixel;

        //                tarImage[tarImageIdx] = colorPixelData[colorIdx];                  //blue
        //                tarImage[tarImageIdx + 1] = colorPixelData[colorIdx + 1];          //green
        //                tarImage[tarImageIdx + 2] = colorPixelData[colorIdx + 2];          //red
        //                tarImage[tarImageIdx + 3] = 0xFF;                                  //alpha
        //            }

        //            tarImageIdx += 4;
        //        }

        //    }

        //    Bitmap bmp = new Bitmap(maxWidth - minWidth + 1, maxHeight - minHeight + 1, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
        //    bmp.SetResolution(96, 96);
        //    cropImage(bmp, tarImage, colorFrame.BytesPerPixel, colorStride, minWidth, maxWidth, minHeight, maxHeight);

        //    Image<Gray, byte> image = new Image<Gray, byte>(bmp);
        //    return image;
        //}


        //private static void cropImage(Bitmap bmp, byte[] tarImage, int bytesPerPixel, int tarStride, int minW, int maxW, int minH, int maxH)
        //{
        //   // int length = (maxH - minH + 1) * (maxW - minW + 1) * bytesPerPixel;
            
        //    int origIdx = 0;
        //    int red, green, blue, alpha;
        //    for (int y = minH; y <= maxH; y++)
        //    {
        //        for (int x = minW; x <= maxW; x++)
        //        {
        //            origIdx = x * bytesPerPixel + y * tarStride;
        //            blue = tarImage[origIdx];              //blue
        //            green = tarImage[origIdx + 1];          //green
        //            red = tarImage[origIdx + 2];          //red
        //            alpha = tarImage[origIdx + 3];          //alpha
        //            bmp.SetPixel(x - minW, y - minH, System.Drawing.Color.FromArgb(alpha, red, green, blue));
        //        }
        //    }
        //}

    }
}
