using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Face;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Drawing;
using System.Drawing.Imaging;

namespace FaceAuthApp.Services
{
    public class FaceService
    {
        private readonly string _haarCascadePath;

        public FaceService(IWebHostEnvironment env)
        {
            _haarCascadePath = Path.Combine(env.ContentRootPath, "haarcascade_frontalface_default.xml");
        }

        // Load image and extract 1 face encoding (as float[])
        public float[] ExtractFaceFeatures(string imagePath)
        {
            if (!File.Exists(imagePath))
                throw new FileNotFoundException("Image not found", imagePath);

            using var image = new Image<Bgr, byte>(imagePath);
            using var gray = image.Convert<Gray, byte>();

            var faceCascade = new CascadeClassifier(_haarCascadePath);
            var faces = faceCascade.DetectMultiScale(gray, 1.1, 4);

            if (faces.Length == 0)
                return null;

            var faceROI = new Rectangle(faces[0].X, faces[0].Y, faces[0].Width, faces[0].Height);
            var faceImg = gray.Copy(faceROI).Resize(100, 100, Inter.Linear); // Normalize size

            // Flatten pixel intensities as naive encoding
            var encoding = new List<float>();
            for (int y = 0; y < faceImg.Height; y++)
            {
                for (int x = 0; x < faceImg.Width; x++)
                {
                    encoding.Add(faceImg.Data[y, x, 0] / 255f); // normalize pixel
                }
            }

            return encoding.ToArray();
        }

        public float[] ParseEncoding(string encoded)
        {
            return encoded.Split(',').Select(s => float.Parse(s)).ToArray();
        }

        public double CompareFaces(float[] encoding1, float[] encoding2)
        {
            double sum = 0;
            for (int i = 0; i < encoding1.Length; i++)
                sum += Math.Pow(encoding1[i] - encoding2[i], 2);

            double distance = Math.Sqrt(sum);
            return 1 / (1 + distance); // similarity score
        } 
    }
}
