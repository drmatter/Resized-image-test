using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ImageResizer
{
    public class ImageProcess
    {
        /// <summary>
        /// 清空目的目錄下的所有檔案與目錄
        /// </summary>
        /// <param name="destPath">目錄路徑</param>
        public static void Clean(string destPath)
        {
            if (!Directory.Exists(destPath))
            {
                Directory.CreateDirectory(destPath);
            }
            else
            {
                var allImageFiles = Directory.GetFiles(destPath, "*", SearchOption.AllDirectories);

                foreach (var item in allImageFiles)
                {
                    File.Delete(item);
                }
            }
        }

        /// <summary>
        /// 進行圖片的縮放作業
        /// </summary>
        /// <param name="sourcePath">圖片來源目錄路徑</param>
        /// <param name="destPath">產生圖片目的目錄路徑</param>
        /// <param name="scale">縮放比例</param>
        public void ResizeImages(string sourcePath, string destPath, double scale)
        {
            try
            {
                var allFiles = FindImages($@"{sourcePath}");
                foreach (var filePath in allFiles)
                {
                    Image imgPhoto = Image.FromFile(filePath);
                    string imgName = Path.GetFileNameWithoutExtension(filePath);

                    int sourceWidth = imgPhoto.Width;
                    int sourceHeight = imgPhoto.Height;

                    int destionatonWidth = (int)(sourceWidth * scale);
                    int destionatonHeight = (int)(sourceHeight * scale);

                    Bitmap processedImage = ProcessBitmap((Bitmap)imgPhoto,
                        sourceWidth, sourceHeight,
                        destionatonWidth, destionatonHeight);

                    string destFile = Path.Combine(destPath, imgName + ".jpg");
                    processedImage.Save(destFile, ImageFormat.Jpeg);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        internal Task ResizeImagesAsync(string sourcePath, string destPath, double scale, CancellationToken token)
        {
            var allFiles = FindImages(sourcePath);
            List<Task> tasks = allFiles.Select(filePath => Task.Run(() =>
                {
                    Image imgPhoto = Image.FromFile(filePath);

                    string imgName = Path.GetFileNameWithoutExtension(filePath);

                    int sourceWidth = imgPhoto.Width;
                    int sourceHeight = imgPhoto.Height;

                    int destionatonWidth = (int)(sourceWidth * scale);
                    int destionatonHeight = (int)(sourceHeight * scale);

                    //Bitmap processedImage = await ProcessBitmapAsync(imgPhoto, sourceWidth, sourceHeight, destionatonWidth, destionatonHeight);
                    Bitmap processedImage = ProcessBitmap((Bitmap)imgPhoto, sourceWidth, sourceHeight, destionatonWidth, destionatonHeight);
                    string destFile = Path.Combine(destPath, imgName + ".jpg");

                    processedImage.Save(destFile, ImageFormat.Jpeg);

                },token))
                .ToList();

            return Task.WhenAll(tasks);
        }

        /// <summary>
        /// 進行圖片的縮放作業
        /// </summary>
        /// <param name="sourcePath">圖片來源目錄路徑</param>
        /// <param name="destPath">產生圖片目的目錄路徑</param>
        /// <param name="scale">縮放比例</param>
        public Task ResizeImagesAsync(string sourcePath, string destPath, double scale)
        {
            return ResizeImagesAsync(sourcePath, destPath, scale, CancellationToken.None);
        }

        /// <summary>
        /// 找出指定目錄下的圖片
        /// </summary>
        /// <param name="srcPath">圖片來源目錄路徑</param>
        /// <returns></returns>
        public List<string> FindImages(string srcPath)
        {
            List<string> files = new List<string>();
            files.AddRange(Directory.GetFiles(srcPath, "*.png", SearchOption.AllDirectories));
            files.AddRange(Directory.GetFiles(srcPath, "*.jpg", SearchOption.AllDirectories));
            files.AddRange(Directory.GetFiles(srcPath, "*.jpeg", SearchOption.AllDirectories));
            return files;
        }

        /// <summary>
        /// 針對指定圖片進行縮放作業
        /// </summary>
        /// <param name="img">圖片來源</param>
        /// <param name="srcWidth">原始寬度</param>
        /// <param name="srcHeight">原始高度</param>
        /// <param name="newWidth">新圖片的寬度</param>
        /// <param name="newHeight">新圖片的高度</param>
        /// <returns></returns>
        private static Bitmap ProcessBitmap(Bitmap img, int srcWidth, int srcHeight, int newWidth, int newHeight)
        {
            Bitmap resizedbitmap = new Bitmap(newWidth, newHeight);
            Graphics g = Graphics.FromImage(resizedbitmap);
            g.InterpolationMode = InterpolationMode.High;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.Clear(Color.Transparent);
            g.DrawImage(img,
                new Rectangle(0, 0, newWidth, newHeight),
                new Rectangle(0, 0, srcWidth, srcHeight),
                GraphicsUnit.Pixel);
            return resizedbitmap;
        }

        /// <summary>
        /// 針對指定圖片進行縮放作業
        /// </summary>
        /// <param name="img">圖片來源</param>
        /// <param name="srcWidth">原始寬度</param>
        /// <param name="srcHeight">原始高度</param>
        /// <param name="newWidth">新圖片的寬度</param>
        /// <param name="newHeight">新圖片的高度</param>
        /// <returns></returns>
        private static async Task<Bitmap> ProcessBitmapAsync(Image img, int srcWidth, int srcHeight, int newWidth, int newHeight)
        {
            Bitmap bmp = new Bitmap(newWidth, newHeight);
            using (var g = Graphics.FromImage(bmp))
            {
                g.InterpolationMode = InterpolationMode.High;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.Clear(Color.Transparent);
                await Task.Run(() =>
                {
                    g.DrawImage(img,
                        new Rectangle(0, 0, newWidth, newHeight),
                        new Rectangle(0, 0, srcWidth, srcHeight),
                        GraphicsUnit.Pixel);
                });
            }
            return bmp;
        }
    }
}
