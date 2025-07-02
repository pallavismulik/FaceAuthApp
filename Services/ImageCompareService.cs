using Emgu.CV;
using Emgu.CV.Face;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System.Drawing;

namespace FaceAuthApp.Services
{
    public class ImageCompareService
    {
        public bool CompareFacesLBPH(string registeredImagePath, string liveImagePath, double threshold = 120)
        {
            try
            {
                //Crop face of registerImage & liveImage
                var trainImage = CropFace(registeredImagePath);
                var testImage = CropFace(liveImagePath);

                if (trainImage == null || testImage == null)
                    throw new Exception("Face not detected in one of the images.");


                // Load grayscale, resize for uniformity
                //var trainImage = new Image<Gray, byte>(registeredImagePath).Resize(100, 100, Emgu.CV.CvEnum.Inter.Cubic);
                //var testImage = new Image<Gray, byte>(liveImagePath).Resize(100, 100, Emgu.CV.CvEnum.Inter.Cubic);

                //if ( trainImage.IsEmpty || testImage.IsEmpty)
                //    throw new Exception("Image not found or failed to load.");

                // Convert to Mat and VectorOfMat
                var trainMat = trainImage.Mat;
                var imagesVec = new VectorOfMat();
                imagesVec.Push(trainMat); // Push one training image

                // Label vector
                var labelsVec = new VectorOfInt(new int[] { 1 }); // must match image count

                // Create and train recognizer
                var recognizer = new LBPHFaceRecognizer(1, 8, 8, 8, threshold);
                recognizer.Train(imagesVec, labelsVec);

                // Predict
                var result = recognizer.Predict(testImage);
                Console.WriteLine($"Predicted Label: {result.Label}, Distance: {result.Distance}");

                return result.Label == 1 && result.Distance < threshold;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Face comparison failed: " + ex.Message);
                return false;
            }
        }

        public bool CompareImages(string img1Path, string img2Path)
        {
            using var bmp1 = new Bitmap(img1Path);
            using var bmp2 = new Bitmap(img2Path);

            //if (bmp1.Size != bmp2.Size) return false;

            int differentPixels = 0;
            for (int y = 0; y < bmp1.Height; y++)
            {
                for (int x = 0; x < bmp1.Width; x++)
                {
                    if (bmp1.GetPixel(x, y) != bmp2.GetPixel(x, y))
                        differentPixels++;
                }
            }

            double totalPixels = bmp1.Width * bmp1.Height;
            double differenceRatio = differentPixels / totalPixels;

            return differenceRatio < 0.10; // 90% match
        }

        #region "Private methods"

        private Image<Gray, byte> CropFace(string imagePath)
        {
            var imageColor = new Image<Bgr, byte>(imagePath);
            var gray = imageColor.Convert<Gray, byte>();

            var faceCascade = new CascadeClassifier("haarcascade_frontalface_default.xml");
            var faces = faceCascade.DetectMultiScale(gray, 1.1, 10, System.Drawing.Size.Empty);

            if (faces.Length == 0)
                return null;

            var faceRect = faces[0];
            var faceImage = gray.GetSubRect(faceRect).Resize(100, 100, Emgu.CV.CvEnum.Inter.Cubic);

            //faceImage.Save("cropped_face_temp.jpg"); // For visual check

            return faceImage;
        }


        #endregion //Private Methods
    }
}
