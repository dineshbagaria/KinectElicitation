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
        public bool Sleep;
        public long userID;
        public float A, B, C, D, F, G, H, I, P, Q, R, S, T, U, V, W, AA, BB, a, b, c, d, f, g, h, i, p, q, r, s, t, u, v, w, aa, bb, a1, b1, c1, d1, f1, g1, h1, i1, p1, q1, r1, s1, t1, u1, v1, w1, aa1, bb1;
        public DateTime startTime = new DateTime();
        public DateTime currentTime = new DateTime();
        public Dictionary<long, float[,]> positiondata = new Dictionary<long, float[,]>();

        private static System.Timers.Timer aTimer;
        private Skeleton skeleton;

        public TimeSpan timeElapsed()
        { 
            currentTime =DateTime.Now;
            return currentTime.Subtract(startTime);

        }

        public SkeletonPosition(Skeleton skeleton, KinectSensor sensor)
        {
            startTime =DateTime.Now;
        

        /* private void SensorSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
         {
             Skeleton[] skeletons = new Skeleton[0];
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
             {
                if (skeletonFrame == null) return;
               
               
                if (skeletonFrame != null)
                 {
                   
                     skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                     skeletonFrame.CopySkeletonDataTo(skeletons);

 */
       
            userID = skeleton.TrackingId;
            //startTime = System.DateTime.Now;
            //5*60*1000
            aTimer = new System.Timers.Timer(300000);
            // Hook up the Elapsed event for the timer.
            aTimer.Elapsed += (sender, args) => OnTimedEvent(this, this.positiondata);// new ElapsedEventHandler(OnTimedEvent);
            aTimer.Enabled = true;

            Joint j = skeleton.Joints[JointType.ShoulderLeft];
            Joint k = skeleton.Joints[JointType.ShoulderRight];
            Joint l = skeleton.Joints[JointType.Head];
            Joint m = skeleton.Joints[JointType.HandLeft];
            Joint n = skeleton.Joints[JointType.HandRight];

            DepthImagePoint ShoulderLeft = sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(j.Position, sensor.DepthStream.Format);
              a1 = ShoulderLeft.X *= (int)640 / sensor.DepthStream.FrameWidth;
              b1 = ShoulderLeft.Y *= (int)480 / sensor.DepthStream.FrameHeight;
            //a1 = j.Position.X;
            //b1 = j.Position.Y;
              DepthImagePoint ShoulderRight = sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(k.Position, sensor.DepthStream.Format);
            d1 = ShoulderRight.X *= (int)640 / sensor.DepthStream.FrameWidth;
             f1 = ShoulderRight.Y *= (int)480 / sensor.DepthStream.FrameHeight;
            //d1 = k.Position.X;
            //f1 = k.Position.Y;
              DepthImagePoint Head = sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(l.Position, sensor.DepthStream.Format);
             h1 = Head.X *= (int)640 / sensor.DepthStream.FrameWidth;
             i1 = Head.Y *= (int)480 / sensor.DepthStream.FrameHeight;
           // h1 = l.Position.X;
            //i1 = l.Position.Y;


              DepthImagePoint HandLeft = sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(m.Position, sensor.DepthStream.Format);
              q1 = HandLeft.X *= (int)640 / sensor.DepthStream.FrameWidth;
              r1 = HandLeft.Y *= (int)480 / sensor.DepthStream.FrameWidth;
            //  q1 = m.Position.X;
            //r1 = m.Position.Y;
               DepthImagePoint HandRight = sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(n.Position, sensor.DepthStream.Format);
              t1 = HandRight.X *= (int)640 / sensor.DepthStream.FrameWidth;
             u1 = HandRight.Y *= (int)480 / sensor.DepthStream.FrameHeight;
           // t1 = n.Position.X;
           // u1 = n.Position.Y;




            if (j.TrackingState == JointTrackingState.Tracked & k.TrackingState == JointTrackingState.Tracked
                & l.TrackingState == JointTrackingState.Tracked & m.TrackingState == JointTrackingState.Tracked
                & n.TrackingState == JointTrackingState.Tracked)
            {
                a = a1; b = b1; d = d1; f = f1;
                h = h1; i = i1; q = q1; r = r1;
                t = t1; u = u1;
            }

            float[,] position = new float[5, 2];
            position[0, 0] = a1; position[1, 0] = b1;
            position[0, 1] = d1; position[1, 1] = f1;
            position[0, 2] = h1; position[1, 2] = i1;
            position[0, 3] = q1; position[1, 3] = r1;
            position[0, 4] = t1; position[1, 4] = u1;
            positiondata.Add(userID, position);

        }






        // Specify what you want to happen when the Elapsed event is  
        // raised. 
        public  Boolean userSleeping(SkeletonPosition ske)
        {
            
            
                ske.A = ske.a1; ske.B = ske.b1;
                ske.D = ske.d1; ske.F = ske.f1;
                ske.H = ske.h1; ske.I = ske.i1;
                ske.Q = ske.q1; ske.R = ske.r1;
                ske.T = ske.t1; ske.U = ske.u1;

                if (System.Math.Abs(ske.A - ske.a) < 15 & System.Math.Abs(ske.B - ske.b) < 15 & System.Math.Abs(ske.D - ske.d) < 15 & System.Math.Abs(ske.F - ske.f) < 15 & System.Math.Abs(ske.H - ske.h) < 15 & System.Math.Abs(ske.I - ske.i) < 15 & System.Math.Abs(ske.Q - ske.q) < 15 & System.Math.Abs(ske.R - ske.r) < 15 & System.Math.Abs(ske.T - ske.t) < 15 & System.Math.Abs(ske.U - ske.u) < 15)
                {
                    return false;
                }
                else
                {
                    return true;
                }



            }
        }
    }


}


