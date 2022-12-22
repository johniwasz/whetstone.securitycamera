using OpenCvSharp;
using static OpenCvSharp.Stitcher;
using System.IO;
using OpenCvSharp.Internal;

namespace Whetstone.SecurityCamera.OpenCV
{
    public class ImageProcessorOpenCV : IImageProcessor
    {

        private const string CatCascadePath = @"Data\ImageModels\haarcascade_frontalcatface.xml";

        public async Task<bool> GetTagsAsync(Stream imageStream, IEnumerable<KeyValuePair<string, double>> tagMinimums)
        {

            if(!File.Exists(CatCascadePath))
            {
                throw new Exception("Classification file not found");
            }

            try
            {
                using var haarCascade = new CascadeClassifier(CatCascadePath);

                Mat detectResult = DetectFace(haarCascade, imageStream);
            }
            catch (Exception e)
            {
                throw;

            }

            return await Task.FromResult(false).ConfigureAwait(false);

        }

        private Mat DetectFace(CascadeClassifier cascade, Stream imageStream)
        {
            Mat result;
       
            using (var src = Mat.FromStream(imageStream, ImreadModes.Color))
            using (var gray = new Mat())
            {
                result = src.Clone();
                Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);

                // Detect faces
                Rect[] faces = cascade.DetectMultiScale(
                    gray, 1.08, 2, HaarDetectionTypes.ScaleImage, new Size(30, 30));

                // Render all detected faces
                foreach (Rect face in faces)
                {
                    var center = new Point
                    {
                        X = (int)(face.X + face.Width * 0.5),
                        Y = (int)(face.Y + face.Height * 0.5)
                    };
                    var axes = new Size
                    {
                        Width = (int)(face.Width * 0.5),
                        Height = (int)(face.Height * 0.5)
                    };
                    Cv2.Ellipse(result, center, axes, 0, 0, 360, new Scalar(255, 0, 255), 4);
                }
            }
            return result;
        }
    }
}