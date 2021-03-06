﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;

using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using Kinect.Tool;
using Microsoft.Kinect;
using FaceDetection;



namespace Detector
{
    public class GenderDetector
    {
        private List<Image<Gray, byte>> images;
        private List<int> labels;


        //const attrs
        private const string CROPPED_IMAGE_DIR = @"cropped_image";
        private const string CROPPED_IMAGE_FILE = @"cropped_image.conf";
        private const char SEPARATOR = ';';

        private static GenderDetector _genderDetector;


        public FaceRecognizer model = new FisherFaceRecognizer(0, double.MaxValue);
        // public FaceRecognizer model = new FisherFaceRecognizer(2, 3000);


        public static GenderDetector getGenderDetector()
        {
            if (_genderDetector == null)
                _genderDetector = new GenderDetector();

            return _genderDetector;

        }

        public int detect(Image<Gray, byte> img)
        {
            if (img == null) return -1;

            if (img.Height != 200 || img.Width != 200)
                img = img.Resize(200, 200, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);
            return model.Predict(img).Label;
        }

        public int detectThroughKinect(KinectSensor sensor, ColorImageFrame colorFrame, Skeleton skeleton)
        {
            Image<Gray, byte> image = ImageHelper.cropImage(sensor, colorFrame, skeleton);
            int ret = this.detect(image);
            return ret;
        }


        private GenderDetector()
        {
            //this.filename = filename;
            //this.separator = separator;

            images = new List<Image<Gray, byte>>();
            labels = new List<int>();

            prepareTrainedData();
            model.Train(images.ToArray(), labels.ToArray());
        }



        private void prepareTrainedData()
        {
            string file = AppDomain.CurrentDomain.BaseDirectory + CROPPED_IMAGE_DIR + "\\" + CROPPED_IMAGE_FILE;
            if (!File.Exists(file))
            {
                string errMsg = "No valid input file was given, please check the given filename.";
                System.Console.WriteLine(errMsg);
                throw new Exception(errMsg);
            }

            try
            {
                StreamReader rd = new StreamReader(new FileStream(file, FileMode.Open, FileAccess.Read));
                string line, path, label;
                while ((line = rd.ReadLine()) != null)
                {
                    String[] pathInfos = line.Split(SEPARATOR);
                    if (pathInfos.Length > 0)
                    {
                        path = pathInfos[0];
                        label = pathInfos[1];
                        Image<Gray, byte> img = null;

                        if (path != null && label != null)
                        {
                            path = AppDomain.CurrentDomain.BaseDirectory + CROPPED_IMAGE_DIR + "\\" + path + ".jpg";

                            if (File.Exists(path))
                            {
                                img = new Image<Gray, byte>(path);
                                if (img.Height != 200 || img.Width != 200)
                                    img = img.Resize(200, 200, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);


                                images.Add(img);
                                labels.Add(int.Parse(label));
                            }
                        }
                    }
                }//end of while

                rd.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }






    }
}
