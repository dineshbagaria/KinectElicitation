using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using System.Drawing;
using FaceDetection;

namespace FaceDetection
{
    class TraningDataPreprocessor
    {
        private const string MODLE_IMAGE_DIR = @"trainingData";
        private const string MODLE_IMAGE_FILE = @"faces.txt";
        private const string CROPPED_IMAGE_DIR = @"cropped_image";
        private const string CROPPED_IMAGE_FILE = @"cropped_image.conf";

        private const char SEPARATOR = ';';


        public void cropAndSaveImage()
        {
            string file = AppDomain.CurrentDomain.BaseDirectory + MODLE_IMAGE_DIR + "\\" + MODLE_IMAGE_FILE;
            if (!File.Exists(file))
            {
                string errMsg = "No valid input file was given, please check the given filename.";
                System.Console.WriteLine(errMsg);
                throw new Exception(errMsg);
            }



            try
            {
                StreamReader rd = new StreamReader(new FileStream(file, FileMode.Open, FileAccess.Read));

                String tarFile = AppDomain.CurrentDomain.BaseDirectory + CROPPED_IMAGE_DIR + "\\" + CROPPED_IMAGE_FILE;
                StreamWriter wr = new StreamWriter(new FileStream(tarFile, FileMode.Create, FileAccess.Write));

                string line, path, label;
                string croppedImagePath;
                string croppedFileName;
                while ((line = rd.ReadLine()) != null)
                {
                    String[] pathInfos = line.Split(SEPARATOR);
                    if (pathInfos.Length > 0)
                    {
                        path = pathInfos[0];
                        label = pathInfos[1];
                        Image<Bgr, byte> image = null;
                        Image<Bgr, byte> tarImage = null;

                        if (path != null && label != null)
                        {
                            path = AppDomain.CurrentDomain.BaseDirectory + MODLE_IMAGE_DIR + "\\" + path + ".jpg";

                            if (File.Exists(path))
                            {
                                image = new Image<Bgr, byte>(path);

                                long detectionTime;
                                List<Rectangle> faces = new List<Rectangle>();
                                List<Rectangle> eyes = new List<Rectangle>();
                                DetectFace.Detect(image, "haarcascade_frontalface_alt2.xml", "haarcascade_eye.xml", faces, eyes, out detectionTime);

                                if (faces.Count == 1)
                                {
                                    image.ROI = faces[0];
                                    tarImage = image.Copy();
                                    tarImage = tarImage.Resize(200, 200, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);
                                    wr.WriteLine(pathInfos[0] + ";" + label);
                                    croppedImagePath = AppDomain.CurrentDomain.BaseDirectory + CROPPED_IMAGE_DIR + "\\" + pathInfos[0].Substring(0, pathInfos[0].LastIndexOf('\\')); ;
                                    if (!Directory.Exists(croppedImagePath))
                                    {
                                        Directory.CreateDirectory(croppedImagePath);
                                    }

                                    croppedFileName = pathInfos[0].Split('\\').Last();

                                    tarImage.Save(croppedImagePath + "\\" + croppedFileName + ".jpg");
                                }
                            }
                        }
                    }
                }//end of while

                rd.Close();
                wr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


    }
}
