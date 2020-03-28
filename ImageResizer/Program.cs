using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace ImageResizer
{
    class Program
    {
        private static async Task Main()
        {
            string sourcePath = Path.Combine(Environment.CurrentDirectory, "images");
            string destinationPath = Path.Combine(Environment.CurrentDirectory, "output");
            Console.WriteLine("請輸入測試次數");
            if (int.TryParse(Console.ReadLine(), out var testCount))
            {
                for (var i = 0; i < testCount; i++)
                {
                    ImageProcess imageProcess = new ImageProcess();

                    imageProcess.Clean(destinationPath);

                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    imageProcess.ResizeImages(sourcePath, destinationPath, 2.0);
                    sw.Stop();
                    var synchronousMethodTime = sw.ElapsedMilliseconds;
                    sw.Reset();


                    imageProcess.Clean(destinationPath);

                    sw.Start();
                    await imageProcess.ResizeImagesAsync(sourcePath, destinationPath, 2.0);
                    var asynchronousMethodTime = sw.ElapsedMilliseconds;
                    sw.Stop();
                    decimal efficacy = Math.Round((decimal)(synchronousMethodTime - asynchronousMethodTime) / synchronousMethodTime, 3);
                    Console.WriteLine($"同步花費時間: {synchronousMethodTime} ms. 異步花費時間: {asynchronousMethodTime}. 效能提升: {efficacy * 100} %");
                }
            }
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
