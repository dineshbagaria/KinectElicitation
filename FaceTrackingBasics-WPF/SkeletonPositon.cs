using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media;
using Microsoft.Kinect;
using System.Linq;
using System.Timers;
using System.Text;
using System.Windows.Controls;
using System.Collections.Generic;
using System;
namespace FaceTrackingBasics
{
    class SkeletonPosition
    {
      
        public float shoulderLeftX, shoulderLeftY, shoulderRightX, shoulderRightY, headX, headY, handLeftX, handLeftY, handRightX, handRightY;
        public DateTime startTime = new DateTime();
        public DateTime currentTime = new DateTime();
        // public Dictionary<long, float[,]> positiondata = new Dictionary<long, float[,]>();

    

        public TimeSpan timeElapsed()
        {
            currentTime = DateTime.Now;
            return currentTime.Subtract(startTime);

        }

        public SkeletonPosition(Skeleton skeleton, KinectSensor sensor)
        {

            Joint j = skeleton.Joints[JointType.ShoulderLeft];
            Joint k = skeleton.Joints[JointType.ShoulderRight];
            Joint l = skeleton.Joints[JointType.Head];
            Joint m = skeleton.Joints[JointType.HandLeft];
            Joint n = skeleton.Joints[JointType.HandRight];

            DepthImagePoint ShoulderLeft = sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(j.Position, sensor.DepthStream.Format);
          
            // t1 = n.Position.X;
            // u1 = n.Position.Y;




            if (j.TrackingState != JointTrackingState.NotTracked 
                & k.TrackingState != JointTrackingState.NotTracked
                & l.TrackingState != JointTrackingState.NotTracked 
                & m.TrackingState != JointTrackingState.NotTracked
                & n.TrackingState != JointTrackingState.NotTracked)
            {

                shoulderLeftX = ShoulderLeft.X *= (int)640 / sensor.DepthStream.FrameWidth;
                shoulderLeftY = ShoulderLeft.Y *= (int)480 / sensor.DepthStream.FrameHeight;
                //a1 = j.Position.X;
                //b1 = j.Position.Y;
                DepthImagePoint ShoulderRight = sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(k.Position, sensor.DepthStream.Format);
                shoulderRightX = ShoulderRight.X *= (int)640 / sensor.DepthStream.FrameWidth;
                shoulderRightY = ShoulderRight.Y *= (int)480 / sensor.DepthStream.FrameHeight;
                //d1 = k.Position.X;
                //f1 = k.Position.Y;
                DepthImagePoint Head = sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(l.Position, sensor.DepthStream.Format);
                headX = Head.X *= (int)640 / sensor.DepthStream.FrameWidth;
                headY = Head.Y *= (int)480 / sensor.DepthStream.FrameHeight;
                // h1 = l.Position.X;
                //i1 = l.Position.Y;


                DepthImagePoint HandLeft = sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(m.Position, sensor.DepthStream.Format);
                handLeftX = HandLeft.X *= (int)640 / sensor.DepthStream.FrameWidth;
                handLeftY = HandLeft.Y *= (int)480 / sensor.DepthStream.FrameWidth;
                //  q1 = m.Position.X;
                //r1 = m.Position.Y;
                DepthImagePoint HandRight = sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(n.Position, sensor.DepthStream.Format);
                handRightX = HandRight.X *= (int)640 / sensor.DepthStream.FrameWidth;
                handRightY = HandRight.Y *= (int)480 / sensor.DepthStream.FrameHeight;
               
                
            }


        }



        public Boolean userSleeping(SkeletonPosition ske)
        {


            if (System.Math.Abs(ske.shoulderLeftX - this.shoulderLeftX) < 15 & System.Math.Abs(ske.shoulderLeftY - this.shoulderLeftY) < 15
                & System.Math.Abs(ske.shoulderRightX - this.shoulderRightX) < 15 & System.Math.Abs(ske.shoulderRightY - this.shoulderRightY) < 15
                & System.Math.Abs(ske.headX - this.headX) < 15 & System.Math.Abs(ske.headY - this.headY) < 15
                & System.Math.Abs(ske.handLeftX - this.handLeftX) < 15 & System.Math.Abs(ske.handLeftY - this.handLeftY) < 15
                & System.Math.Abs(ske.handRightX - this.handRightX) < 15 & System.Math.Abs(ske.handRightY - this.handRightY) < 15)
            {
                return true;
            }
            else
            {
                return false;
            }




        }
    }
}





