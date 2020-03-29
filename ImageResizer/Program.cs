using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ImageResizer
{
    class Program
    {
        static string sourcePath = Path.Combine(Environment.CurrentDirectory, "images");
        static string destinationPath = Path.Combine(Environment.CurrentDirectory, "output");
        private static async Task Main()
        {
            CancellationTokenSource cts = new CancellationTokenSource();


            Console.WriteLine("請輸入測試次數");
            if (int.TryParse(Console.ReadLine(), out var testCount))
            {
                #region 等候使用者輸入 取消 c 按鍵
                ThreadPool.QueueUserWorkItem(x =>
                {
                    ConsoleKeyInfo key = Console.ReadKey();
                    if (key.Key == ConsoleKey.C)
                    {
                        cts.Cancel();
                    }
                });
                #endregion

                try
                {
                    for (var i = 0; i < testCount; i++)
                    {
                        ImageProcess imageProcess = new ImageProcess();

                        ImageProcess.Clean(destinationPath);

                        Stopwatch sw = new Stopwatch();
                        //sw.Start();
                        //imageProcess.ResizeImages(sourcePath, destinationPath, 2.0);
                        //sw.Stop();
                        //var synchronousMethodTime = sw.ElapsedMilliseconds;
                        //sw.Reset();


                        //ImageProcess.Clean(destinationPath);

                        sw.Start();
                        //await imageProcess.ResizeImagesAsync(sourcePath, destinationPath, 2.0);
                        await imageProcess.ResizeImagesAsync(sourcePath, destinationPath, 2.0, cts.Token);
                        var asynchronousMethodTime = sw.ElapsedMilliseconds;
                        sw.Stop();
                        //decimal efficacy = Math.Round((decimal)(synchronousMethodTime - asynchronousMethodTime) / synchronousMethodTime, 3);
                        //Console.WriteLine($"同步花費時間: {synchronousMethodTime} ms. 異步花費時間: {asynchronousMethodTime}. 效能提升: {efficacy * 100} %");
                        Console.WriteLine($"異步花費時間: {asynchronousMethodTime}. Exception:");
                    }
                }
                catch (OperationCanceledException)
                {
                    ImageProcess.Clean(destinationPath);

                    Console.WriteLine($"{Environment.NewLine}轉檔已經取消。");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
