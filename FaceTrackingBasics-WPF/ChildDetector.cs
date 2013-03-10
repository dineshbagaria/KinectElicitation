using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;

namespace Impeli
{
    class ChildDetector
    {
        public int mycount = 0;
        //public float A, B, C, D, F, G, H, I, P, Q, R, S, T, U, V, W, AA, BB, a, b, c, d, f, g, h, i, p, q, r, s, t, u, v, w, aa, bb, a1, b1, c1, d1, f1, g1, h1, i1, p1, q1, r1, s1, t1, u1, v1, w1, aa1, bb1;
        public bool x = false;
       // private KinectSensor _Kinect;
        //private readonly Brush[] _SkeletonBrushes;
        //private Skeleton[] _FrameSkeletons;
        //double startXAngle = 999, currentXAngle = 0, deviation = 0;

       // DateTime startTime;

        public Boolean ChildOrNot(Skeleton skeleton)
        {
            double height = Height(skeleton);
            if (height>0.3)
                if (height>1.3)
                    return false;
                else
                    return true;
            return false;
               
        }
       
        public double Height(Skeleton skeleton)
        {
            const double HEAD_DIVERGENCE = 0.1;

            var head = skeleton.Joints[JointType.Head];
            var neck = skeleton.Joints[JointType.ShoulderCenter];
            var spine = skeleton.Joints[JointType.Spine];
            var waist = skeleton.Joints[JointType.HipCenter];
            var hipLeft = skeleton.Joints[JointType.HipLeft];
            var hipRight = skeleton.Joints[JointType.HipRight];
            var kneeLeft = skeleton.Joints[JointType.KneeLeft];
            var kneeRight = skeleton.Joints[JointType.KneeRight];
            var ankleLeft = skeleton.Joints[JointType.AnkleLeft];
            var ankleRight = skeleton.Joints[JointType.AnkleRight];
            var footLeft = skeleton.Joints[JointType.FootLeft];
            var footRight = skeleton.Joints[JointType.FootRight];


            // Find which leg is tracked more accurately.
            int legLeftTrackedJoints = NumberOfTrackedJoints(hipLeft, kneeLeft, ankleLeft, footLeft);
            int legRightTrackedJoints = NumberOfTrackedJoints(hipRight, kneeRight, ankleRight, footRight);
            int uperBodyTrackedJoints = NumberOfTrackedJoints(head, neck, spine, waist);

            double legLength = legLeftTrackedJoints > legRightTrackedJoints ? Length(hipLeft, kneeLeft, ankleLeft, footLeft) : Length(hipRight, kneeRight, ankleRight, footRight);

            if (uperBodyTrackedJoints ==4)
            {
                return Length(head, neck, spine, waist) + legLength + HEAD_DIVERGENCE;
            }
            return 0;
        }

        public int NumberOfTrackedJoints(params Joint[] joints)
        {
            int trackedJoints = 0;

            foreach (var joint in joints)
            {
                if (joint.TrackingState == JointTrackingState.Tracked)
                {
                    trackedJoints++;
                }
            }


            return trackedJoints;
        }

        public double Length(params Joint[] joints)
        {
            double length = 0;

            for (int index = 0; index < joints.Length - 1; index++)
            {
                length += Length(joints[index], joints[index + 1]);
            }


            return length;
        }
        public double Length(Joint p1, Joint p2)
        {
            return Math.Sqrt(
                Math.Pow(p1.Position.X - p2.Position.X, 2) +
                Math.Pow(p1.Position.Y - p2.Position.Y, 2) +
                Math.Pow(p1.Position.Z - p2.Position.Z, 2));
        }

    }
}
