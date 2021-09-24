using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadTest
{
    class Program
    {
        private const string OLD_FILES = @"D:\old_files";
        private const string NEW_FILES = @"D:\new_files";
        private const int THREAD_QTY = 100;
        private const int FILE_QTY = 10000;

        static void Main(string[] args)
        {
            List<Task> tasks = new List<Task>();

            if (!Directory.Exists(OLD_FILES))
                Directory.CreateDirectory(OLD_FILES);
            if (!Directory.Exists(NEW_FILES))
                Directory.CreateDirectory(NEW_FILES);

            List<string> filesToDelete = Directory.EnumerateFiles(NEW_FILES).OrderBy(f => f).ToList();
            if (filesToDelete.Count > 0)
            {
                for (int i = 0; i < THREAD_QTY; i++)
                {
                    int h = i;
                    tasks.Add(new Task(() =>
                    {
                        for (int j = 1; j <= FILE_QTY / THREAD_QTY; j++)
                            if(filesToDelete.Count >= FILE_QTY / THREAD_QTY * h + j)
                                File.Delete(filesToDelete[FILE_QTY / THREAD_QTY * h + j - 1]);
                    }));
                }
            }
            Task.WaitAll(tasks.ToArray());
            tasks.Clear();
            filesToDelete.Clear();

            filesToDelete = Directory.EnumerateFiles(OLD_FILES).OrderBy(f => f).ToList();
            if (filesToDelete.Count > 0)
            {
                for (int i = 0; i < THREAD_QTY; i++)
                {
                    int h = i;
                    tasks.Add(new Task(() =>
                    {
                        for (int j = 1; j <= FILE_QTY / THREAD_QTY; j++)
                            if (filesToDelete.Count >= FILE_QTY / THREAD_QTY * h + j)
                            {
                                File.Delete(filesToDelete[FILE_QTY / THREAD_QTY * h + j - 1]);
                            }
                    }));
                }
            }
            tasks.ForEach(t => t.Start());
            Task.WaitAll(tasks.ToArray());
            tasks.Clear();
            DateTime start = DateTime.Now;
            for (int i = 0; i< THREAD_QTY; i++)
            {
                int h = i;
                tasks.Add(new Task(() => {
                    for (int j = 1; j <= FILE_QTY / THREAD_QTY; j++)
                        File.WriteAllText($@"{NEW_FILES}\{FILE_QTY / THREAD_QTY * h + j}.txt", "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Maecenas porttitor congue massa. Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.");
                }));
            }

            tasks.ForEach(t => t.Start());

            Task.WaitAll(tasks.ToArray());

            tasks.Clear();

            List<string> files = Directory.EnumerateFiles(NEW_FILES).OrderBy(f => f).ToList();

            for (int i = 0; i < THREAD_QTY; i++)
            {
                int h = i;
                tasks.Add(new Task(() => {
                    for (int j = 1; j <= FILE_QTY / THREAD_QTY; j++)
                        File.Move(files[FILE_QTY / THREAD_QTY * h + j - 1], $@"{OLD_FILES}\{FILE_QTY / THREAD_QTY * h + j}.txt");
                }));
            }

            tasks.ForEach(t => t.Start());

            Task.WaitAll(tasks.ToArray());
            Console.WriteLine($"Il processo è durato {(DateTime.Now - start).TotalMilliseconds} millisecondi");
        }
    }
}
