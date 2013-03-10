//----------------------------------------------------------------------------
//  Copyright (C) 2004-2012 by EMGU. All rights reserved.       
//----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.CV.GPU;
using System.IO;

namespace FaceDetection
{
   static class Program
   {
       private const string FILENAME = "trainingData\\faces.txt";
       private const string MODEL_DATA_DIR = "trainingData";
       private const char SEPARATOR = ';';

       //public static List<Image<Rgb, Byte>> images = new List<Image<Rgb, byte>>();


      /// <summary>
      /// The main entry point for the application.
      /// </summary>
      [STAThread]
      static void Main()
      {
         if (!IsPlaformCompatable()) return;
         Application.EnableVisualStyles();
         Application.SetCompatibleTextRenderingDefault(false);
         //Run();



         new TraningDataPreprocessor().cropAndSaveImage();
      }





      static void Run()
      {

          string file = AppDomain.CurrentDomain.BaseDirectory + FILENAME;
          if (!File.Exists(file))
          {
              string errMsg = "No valid input file was given, please check the given filename.";
              System.Console.WriteLine(errMsg);
              throw new Exception(errMsg);
          }



          StreamReader rd = new StreamReader(new FileStream(file, FileMode.Open, FileAccess.Read));
          string line, path, label;
          Image<Bgr, Byte> image = null;

          while ((line = rd.ReadLine()) != null)
          {
              String[] pathInfos = line.Split(SEPARATOR);
              if (pathInfos.Length > 0)
              {
                  path = pathInfos[0];
                  label = pathInfos[1];
                 

                  if (path != null && label != null)
                  {
                      path = AppDomain.CurrentDomain.BaseDirectory + MODEL_DATA_DIR + "\\" + path + ".jpg";

                      if (File.Exists(path))
                      {
                          image = new Image<Bgr, Byte>(path);
                          //Image<Bgr, Byte> image = new Image<Bgr, byte>("lena.jpg"); //Read the files as an 8-bit Bgr image  
                          //Image<Bgr, Byte> image = new Image<Bgr, byte>("short_hair_15.jpg"); //Read the files as an 8-bit Bgr image 
                         
                          
                          long detectionTime;
                          List<Rectangle> faces = new List<Rectangle>();
                          List<Rectangle> eyes = new List<Rectangle>();
                          DetectFace.Detect(image, "haarcascade_frontalface_alt2.xml", "haarcascade_eye.xml", faces, eyes, out detectionTime);
                          foreach (Rectangle face in faces)
                              image.Draw(face, new Bgr(Color.Red), 2);
                          foreach (Rectangle eye in eyes)
                              image.Draw(eye, new Bgr(Color.Blue), 2);

                          //display the image 
                          ImageViewer.Show(image, String.Format(
                             "Completed face and eye detection using {0} in {1} milliseconds",
                             GpuInvoke.HasCuda ? "GPU" : "CPU",
                             detectionTime));

                      }
                  }
              }

          }


       
      }

      /// <summary>
      /// Check if both the managed and unmanaged code are compiled for the same architecture
      /// </summary>
      /// <returns>Returns true if both the managed and unmanaged code are compiled for the same architecture</returns>
      static bool IsPlaformCompatable()
      {
         int clrBitness = Marshal.SizeOf(typeof(IntPtr)) * 8;
         if (clrBitness != CvInvoke.UnmanagedCodeBitness)
         {
            MessageBox.Show(String.Format("Platform mismatched: CLR is {0} bit, C++ code is {1} bit."
               + " Please consider recompiling the executable with the same platform target as C++ code.",
               clrBitness, CvInvoke.UnmanagedCodeBitness));
            return false;
         }
         return true;
      }
   }
}