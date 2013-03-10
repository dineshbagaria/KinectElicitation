using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.IO;
using Detector;
using FaceDetection;
using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;

namespace GenderTest
{
    class Program
    {
        static void Main(string[] args)
        {

            Detector.GenderDetector detector = GenderDetector.getGenderDetector();
            String path = "test_data\\test_data.txt";

            StreamReader rd = new StreamReader(new FileStream(path, FileMode.Open, FileAccess.Read));
            StreamWriter wr = new StreamWriter(new FileStream("test_result.txt", FileMode.Create, FileAccess.Write));

            string filename;
            while ((filename = rd.ReadLine()) != null)
            {

                Image<Bgr, byte> image = null;
                Image<Bgr, byte> tarImage = null;

                filename = AppDomain.CurrentDomain.BaseDirectory + "test_data\\" + filename;

                if (File.Exists(filename))
                {
                    image = new Image<Bgr, byte>(filename);

                    long detectionTime;
                    List<Rectangle> faces = new List<Rectangle>();
                    List<Rectangle> eyes = new List<Rectangle>();

                    DetectFace.Detect(image, "haarcascade_frontalface_alt2.xml", "haarcascade_eye.xml", faces, eyes, out detectionTime);

                    if (faces.Count == 1)
                    {
                        image.ROI = faces[0];
                        tarImage = image.Copy();
                        tarImage = tarImage.Resize(200, 200, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);

                    }
                    else
                    {
                        Console.WriteLine(filename + " CAN NOT BE DETECTED");
                    }


                    if (tarImage != null)
                    {

                        Console.WriteLine("Image: {0}, Result {1}", filename, detector.detect(tarImage.Convert<Gray, byte>()));
                        wr.WriteLine("Image: {0}, Result {1}", filename, detector.detect(tarImage.Convert<Gray, byte>()));
                    }

                }
            }

            rd.Close();
            wr.Close();

            Console.ReadKey();

        }


    }
}
