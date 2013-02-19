// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FaceTrackingViewer.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FaceTrackingBasics
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using Microsoft.Kinect;
    using Microsoft.Kinect.Toolkit.FaceTracking;
    using Detector;

    using Point = System.Windows.Point;

    /// <summary>
    /// Class that uses the Face Tracking SDK to display a face mask for
    /// tracked skeletons
    /// </summary>
    public partial class FaceTrackingViewer : UserControl, IDisposable
    {
        public static readonly DependencyProperty KinectProperty = DependencyProperty.Register(
            "Kinect",
            typeof(KinectSensor),
            typeof(FaceTrackingViewer),
            new PropertyMetadata(
                null, (o, args) => ((FaceTrackingViewer)o).OnSensorChanged((KinectSensor)args.OldValue, (KinectSensor)args.NewValue)));

        private const uint MaxMissedFrames = 100;
        Boolean SkeletonChooseingState = false;

        private readonly Dictionary<int, SkeletonFaceTracker> trackedSkeletons = new Dictionary<int, SkeletonFaceTracker>();

        public static Dictionary<int, UserProfile> userProfiles = new Dictionary<int, UserProfile>();
        public DateTime startTime = new DateTime();
        public DateTime currentTime = new DateTime();
        public Dictionary<int, Boolean> userTrackStatus = new Dictionary<int, Boolean>();

        ChildDetector childDetector = new ChildDetector();
        GenderDetector genderDetector = GenderDetector.getGenderDetector();

        private byte[] colorImage;

        private ColorImageFormat colorImageFormat = ColorImageFormat.Undefined;

        private short[] depthImage;

        private DepthImageFormat depthImageFormat = DepthImageFormat.Undefined;

        private bool disposed;

        private Skeleton[] skeletonData;
        public static Dictionary<int, float> totalDistractiony;
        public static Dictionary<int, float> rotationOldy;
        public static Dictionary<int, List<float>> headSDDataset;
        public static Dictionary<int, Double> headSD;
        DateTime sessionStartTime = new DateTime(), sessionEndTime = new DateTime();
        private ElicitationSession session;
        public int contentCounter;

        /*public static float totalDistractionx = 0;
        public float rotationOldx = 0;
        public static float totalDistractionz = 0;
        public float rotationOldz = 0;*/



        public FaceTrackingViewer()
        {
            this.InitializeComponent();
            startTime = DateTime.Today;
            currentTime = DateTime.Today;

        }

        ~FaceTrackingViewer()
        {
            this.Dispose(false);
        }

        public KinectSensor Kinect
        {
            get
            {
                return (KinectSensor)this.GetValue(KinectProperty);
            }

            set
            {
                this.SetValue(KinectProperty, value);
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                this.ResetFaceTracking();

                this.disposed = true;
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            foreach (SkeletonFaceTracker faceInformation in this.trackedSkeletons.Values)
            {
                faceInformation.DrawFaceModel(drawingContext);

            }
        }

        private Skeleton getSkeletonForID(int userID, Skeleton[] skeletonData)
        {

            foreach (Skeleton skeleton in this.skeletonData)
            {
                if (skeleton.TrackingId == userID)
                {
                    return skeleton;

                }
            }
            return null;
        }

        private Double CalculateSD(List<float> data)
        {
            float mean = 0;
            double squaredMean = 0;
            foreach (float item in data)
            {
                mean += item;
            }
            mean /= data.Count;

            foreach (float item in data)
            {
                squaredMean += Math.Pow(System.Convert.ToDouble(mean - item), 2);
            }

            return Math.Pow(squaredMean / data.Count, 0.5);
        }
        private void OnAllFramesReady(object sender, AllFramesReadyEventArgs allFramesReadyEventArgs)
        {
            sessionEndTime = DateTime.Now;
            if (sessionEndTime.Subtract(sessionStartTime).Seconds > 120)
            {
                //publish data for the session
                //clear old variables
                userProfiles = null;
                userTrackStatus = null;
                userTrackStatus = new Dictionary<int, Boolean>();
                userProfiles = new Dictionary<int, UserProfile>();
                //start new session
                session = null;
                sessionStartTime = sessionEndTime;

            }
            else
            {
                //assign current data to the session
                List<UserProfile> sessionUsers;
                if (session == null)
                {
                    contentCounter++;
                    sessionUsers = new List<UserProfile>();
                    foreach (var item in userProfiles.Values)
                    {
                        sessionUsers.Add(item);
                    }
                    session = new ElicitationSession(sessionUsers, contentCounter);

                }
                sessionUsers = new List<UserProfile>();
                foreach (var item in userProfiles.Values)
                {
                    sessionUsers.Add(item);
                }
                session.setUsers(sessionUsers);
            }



            ColorImageFrame colorImageFrame = null;
            DepthImageFrame depthImageFrame = null;
            SkeletonFrame skeletonFrame = null;

            try
            {
                colorImageFrame = allFramesReadyEventArgs.OpenColorImageFrame();
                depthImageFrame = allFramesReadyEventArgs.OpenDepthImageFrame();
                skeletonFrame = allFramesReadyEventArgs.OpenSkeletonFrame();

                if (colorImageFrame == null || depthImageFrame == null || skeletonFrame == null)
                {
                    return;
                }

                // Check for image format changes.  The FaceTracker doesn't
                // deal with that so we need to reset.
                if (this.depthImageFormat != depthImageFrame.Format)
                {
                    this.ResetFaceTracking();
                    this.depthImage = null;
                    this.depthImageFormat = depthImageFrame.Format;
                }

                if (this.colorImageFormat != colorImageFrame.Format)
                {
                    this.ResetFaceTracking();
                    this.colorImage = null;
                    this.colorImageFormat = colorImageFrame.Format;
                }

                // Create any buffers to store copies of the data we work with
                if (this.depthImage == null)
                {
                    this.depthImage = new short[depthImageFrame.PixelDataLength];
                }

                if (this.colorImage == null)
                {
                    this.colorImage = new byte[colorImageFrame.PixelDataLength];
                }

                // Get the skeleton information
                if (this.skeletonData == null || this.skeletonData.Length != skeletonFrame.SkeletonArrayLength)
                {
                    this.skeletonData = new Skeleton[skeletonFrame.SkeletonArrayLength];
                }

                colorImageFrame.CopyPixelDataTo(this.colorImage);
                depthImageFrame.CopyPixelDataTo(this.depthImage);
                skeletonFrame.CopySkeletonDataTo(this.skeletonData);
                if (!this.Kinect.SkeletonStream.AppChoosesSkeletons)
                {
                    //this.Kinect.SkeletonStream.AppChoosesSkeletons = true; // Ensure AppChoosesSkeletons is set
                }

                //int count = 0;
                //foreach (Skeleton skeleton in this.skeletonData)
                //{
                //    if (skeleton.TrackingState == SkeletonTrackingState.Tracked
                //       || skeleton.TrackingState == SkeletonTrackingState.PositionOnly)
                //    {
                //        count++;
                //    }
                //}
                //if (count > 2)
                //{
                //    count++;
                //}




                foreach (Skeleton skeleton in this.skeletonData)
                {
                    if (skeleton.TrackingState == SkeletonTrackingState.Tracked
                       || skeleton.TrackingState == SkeletonTrackingState.PositionOnly)
                    {
                        if (!userProfiles.ContainsKey(skeleton.TrackingId))
                        {
                            UserProfile user = new UserProfile();
                            userProfiles.Add(skeleton.TrackingId, user);
                            user.userID = skeleton.TrackingId;
                            userTrackStatus.Add(skeleton.TrackingId, false);
                            totalDistractiony.Add(skeleton.TrackingId, 0);
                            rotationOldy.Add(skeleton.TrackingId, 0);
                            headSD.Add(skeleton.TrackingId, 0);
                            headSDDataset.Add(skeleton.TrackingId, new List<float>());
                        }
                    }
                }

                foreach (int userID in userProfiles.Keys)
                {
                    Skeleton skeleton = getSkeletonForID(userID, skeletonData);

                    Boolean tempBool = userTrackStatus[userID];
                    // Boolean tempBool = userTrackStatus.TryGetValue(userID, out tempBool);

                    /*if (skeleton.TrackingState != SkeletonTrackingState.Tracked && !tempBool)
                    {
                        this.Kinect.SkeletonStream.ChooseSkeletons(userID);
                        userTrackStatus.Remove(userID);
                        userTrackStatus.Add(skeleton.TrackingId, true);
                        break;

                    }

                    userTrackStatus.Remove(userID);
                    userTrackStatus.Add(skeleton.TrackingId, false);*/
                    if (skeleton != null)
                    {

                        if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
                        {

                            userTrackStatus[skeleton.TrackingId] = true;
                            Debug.WriteLine("tracked Skeleton Id:" + skeleton.TrackingId);
                            // We want keep a record of any skeleton, tracked or untracked.
                            if (!this.trackedSkeletons.ContainsKey(skeleton.TrackingId))
                            {
                                this.trackedSkeletons.Add(skeleton.TrackingId, new SkeletonFaceTracker());

                            }


                            //gender detection
                            int gender = genderDetector.detectThroughKinect(Kinect, colorImageFrame, skeleton);
                            UserProfile user = userProfiles[skeleton.TrackingId];
                            if (gender == 0)
                                user.gender = "Male";
                            else if (gender == 1)
                                user.gender = "Female";
                            else
                                user.gender = "Unknown";

                            //child detection

                            /*UserProfile tempUserProfile = new UserProfile();
                            userProfiles.TryGetValue(skeleton.TrackingId, out tempUserProfile);
                            tempUserProfile.userIsChild = childDetector.ChildOrNot(skeleton);*/

                            userProfiles[skeleton.TrackingId].userIsChild = childDetector.ChildOrNot(skeleton);



                            // Give each tracker the upated frame.
                            SkeletonFaceTracker skeletonFaceTracker;
                            if (this.trackedSkeletons.TryGetValue(skeleton.TrackingId, out skeletonFaceTracker))
                            {
                                skeletonFaceTracker.OnFrameReady(this.Kinect, colorImageFormat, colorImage, depthImageFormat, depthImage, skeleton);
                                skeletonFaceTracker.LastTrackedFrame = skeletonFrame.FrameNumber;
                                if (skeletonFaceTracker.faceTracked)
                                {
                                    if (Math.Abs(skeletonFaceTracker.rotationNew - rotationOldy[skeleton.TrackingId]) > 1.5)
                                    {
                                        /* totalDistractiony[skeleton.TrackingId] = totalDistractiony[skeleton.TrackingId] + Math.Abs(skeletonFaceTracker.rotationNew - rotationOldy[skeleton.TrackingId]);
                                         rotationOldy[skeleton.TrackingId] = skeletonFaceTracker.rotationNew;
                                         skeletonFaceTracker.faceTracked = false;*/
                                        //totalDistractiony = skeletonFaceTracker.rotationNew;                                      

                                        headSDDataset[skeleton.TrackingId].Add(Math.Abs(skeletonFaceTracker.rotationNew
                                            - rotationOldy[skeleton.TrackingId]));
                                        headSD[skeleton.TrackingId] = CalculateSD(headSDDataset[skeleton.TrackingId]);
                                        rotationOldy[skeleton.TrackingId] = skeletonFaceTracker.rotationNew;
                                        skeletonFaceTracker.faceTracked = false;

                                    }
                                    if (currentTime.Subtract(startTime).Seconds > 120)
                                    {
                                        if (userProfiles.ContainsKey(skeleton.TrackingId) && headSD[skeleton.TrackingId] > 10)
                                        {
                                            /*userDistractionStatus.Remove(skeleton.TrackingId);
                                            userDistractionStatus.Add(skeleton.TrackingId, true);
                                            UserProfile tempUser = new UserProfile();
                                            userProfiles.TryGetValue(skeleton.TrackingId, out tempUser);
                                            tempUser.userDistracted = true;*/

                                            userProfiles[skeleton.TrackingId].userDistracted = true;
                                        }
                                        startTime = currentTime;
                                    }

                                }


                            }
                        }
                    }
                }


                this.RemoveOldTrackers(skeletonFrame.FrameNumber);

                this.InvalidateVisual();
            }
            finally
            {
                if (colorImageFrame != null)
                {
                    colorImageFrame.Dispose();
                }

                if (depthImageFrame != null)
                {
                    depthImageFrame.Dispose();
                }

                if (skeletonFrame != null)
                {
                    skeletonFrame.Dispose();
                }

                bool allDone = true;
                foreach (var userID in userTrackStatus.Keys)
                {
                    bool temp = userTrackStatus[userID];
                    //  userTrackStatus.TryGetValue(userID, out temp);
                    if (!temp)
                    {
                        allDone = false;
                        break;
                    }
                }
                if (allDone)
                {
                    List<int> userIDList = new List<int>(userTrackStatus.Keys);

                    foreach (var userID in userIDList)
                    {
                        userTrackStatus[userID] = false;
                    }
                    int tempUserID = 0;
                    foreach (var userID in userTrackStatus.Keys)
                    {
                        bool temp = userTrackStatus[userID];
                        //userTrackStatus.TryGetValue(userID, out temp);
                        if (!temp)
                        {
                            this.Kinect.SkeletonStream.ChooseSkeletons(userID);
                            tempUserID = userID;
                            break;
                        }
                    }
                    userTrackStatus[tempUserID] = true;
                }
                else
                {
                    int tempUserID = 0;
                    foreach (var userID in userTrackStatus.Keys)
                    {
                        bool temp = userTrackStatus[userID];
                        //userTrackStatus.TryGetValue(userID, out temp);
                        if (!temp)
                        {
                            this.Kinect.SkeletonStream.ChooseSkeletons(userID);
                            tempUserID = userID;
                            break;
                        }
                    }
                    userTrackStatus[tempUserID] = true;

                }
            }
        }

        private void OnSensorChanged(KinectSensor oldSensor, KinectSensor newSensor)
        {
            if (oldSensor != null)
            {
                oldSensor.AllFramesReady -= this.OnAllFramesReady;
                this.ResetFaceTracking();
            }

            if (newSensor != null)
            {
                newSensor.AllFramesReady += this.OnAllFramesReady;
            }
        }

        /// <summary>
        /// Clear out any trackers for skeletons we haven't heard from for a while
        /// </summary>
        private void RemoveOldTrackers(int currentFrameNumber)
        {
            var trackersToRemove = new List<int>();

            foreach (var tracker in this.trackedSkeletons)
            {
                uint missedFrames = (uint)currentFrameNumber - (uint)tracker.Value.LastTrackedFrame;
                if (missedFrames > MaxMissedFrames)
                {
                    // There have been too many frames since we last saw this skeleton
                    trackersToRemove.Add(tracker.Key);
                }
            }

            foreach (int trackingId in trackersToRemove)
            {
                this.RemoveTracker(trackingId);
            }
        }

        private void RemoveTracker(int trackingId)
        {
            this.trackedSkeletons[trackingId].Dispose();
            this.trackedSkeletons.Remove(trackingId);
        }

        private void ResetFaceTracking()
        {
            foreach (int trackingId in new List<int>(this.trackedSkeletons.Keys))
            {
                this.RemoveTracker(trackingId);
            }
        }

        private class SkeletonFaceTracker : IDisposable
        {
            public float rotationNew;
            public bool faceTracked;
            private static FaceTriangle[] faceTriangles;

            private EnumIndexableCollection<FeaturePoint, PointF> facePoints;

            private FaceTracker faceTracker;

            private bool lastFaceTrackSucceeded;

            private SkeletonTrackingState skeletonTrackingState;

            public int LastTrackedFrame { get; set; }

            public void Dispose()
            {
                if (this.faceTracker != null)
                {
                    this.faceTracker.Dispose();
                    this.faceTracker = null;
                }
            }

            public void DrawFaceModel(DrawingContext drawingContext)
            {
                if (!this.lastFaceTrackSucceeded || this.skeletonTrackingState != SkeletonTrackingState.Tracked)
                {
                    return;
                }

                var faceModelPts = new List<Point>();
                var faceModel = new List<FaceModelTriangle>();

                for (int i = 0; i < this.facePoints.Count; i++)
                {
                    faceModelPts.Add(new Point(this.facePoints[i].X + 0.5f, this.facePoints[i].Y + 0.5f));
                }

                foreach (var t in faceTriangles)
                {
                    var triangle = new FaceModelTriangle();
                    triangle.P1 = faceModelPts[t.First];
                    triangle.P2 = faceModelPts[t.Second];
                    triangle.P3 = faceModelPts[t.Third];
                    faceModel.Add(triangle);
                }

                var faceModelGroup = new GeometryGroup();
                for (int i = 0; i < faceModel.Count; i++)
                {
                    var faceTriangle = new GeometryGroup();
                    faceTriangle.Children.Add(new LineGeometry(faceModel[i].P1, faceModel[i].P2));
                    faceTriangle.Children.Add(new LineGeometry(faceModel[i].P2, faceModel[i].P3));
                    faceTriangle.Children.Add(new LineGeometry(faceModel[i].P3, faceModel[i].P1));
                    faceModelGroup.Children.Add(faceTriangle);
                }

                drawingContext.DrawGeometry(Brushes.LightYellow, new Pen(Brushes.LightYellow, 1.0), faceModelGroup);
            }

            /// <summary>
            /// Updates the face tracking information for this skeleton
            /// </summary>
            internal void OnFrameReady(KinectSensor kinectSensor, ColorImageFormat colorImageFormat, byte[] colorImage, DepthImageFormat depthImageFormat, short[] depthImage, Skeleton skeletonOfInterest)
            {
                this.skeletonTrackingState = skeletonOfInterest.TrackingState;

                if (this.skeletonTrackingState != SkeletonTrackingState.Tracked)
                {
                    // nothing to do with an untracked skeleton.
                    return;
                }

                if (this.faceTracker == null)
                {
                    try
                    {
                        this.faceTracker = new FaceTracker(kinectSensor);
                    }
                    catch (InvalidOperationException)
                    {
                        // During some shutdown scenarios the FaceTracker
                        // is unable to be instantiated.  Catch that exception
                        // and don't track a face.
                        Debug.WriteLine("AllFramesReady - creating a new FaceTracker threw an InvalidOperationException");
                        this.faceTracker = null;
                    }
                }

                if (this.faceTracker != null)
                {
                    FaceTrackFrame frame = this.faceTracker.Track(
                        colorImageFormat, colorImage, depthImageFormat, depthImage, skeletonOfInterest);

                    this.lastFaceTrackSucceeded = frame.TrackSuccessful;

                    if (this.lastFaceTrackSucceeded)
                    {
                        if (faceTriangles == null)
                        {
                            // only need to get this once.  It doesn't change.
                            faceTriangles = frame.GetTriangles();

                            //Debug.WriteLine(rotation.X + "," + rotation.Y + "," + rotation.Z);

                        }
                        rotationNew = frame.Rotation.Y;
                        faceTracked = true;
                        Debug.WriteLine(frame.Rotation.Y);

                        this.facePoints = frame.GetProjected3DShape();
                    }
                }
            }

            private struct FaceModelTriangle
            {
                public Point P1;
                public Point P2;
                public Point P3;
            }
        }
    }
}