using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

namespace ThreadDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Type number of threads: ");
            if (!int.TryParse(Console.ReadLine(), out int numThreads))
            {
                numThreads = 5; 
            }

            List<WorkerThread> workers = new List<WorkerThread>();
            List<Thread> threads = new List<Thread>();
            
            var events = new List<(int index, int delayMs)>();
            Random rnd = new Random();

            for (int i = 1; i <= numThreads; i++)
            {
                var worker = new WorkerThread(i, i);
                workers.Add(worker);
                
                events.Add((i - 1, rnd.Next(1000, 10001)));
                
                Thread t = new Thread(worker.Run);
                threads.Add(t);
                t.Start();
            }

            events = events.OrderBy(e => e.delayMs).ToList();

            int elapsedMs = 0;
            foreach (var e in events)
            {
                Thread.Sleep(e.delayMs - elapsedMs);
                elapsedMs = e.delayMs;
                workers[e.index].Stop();
            }

            foreach (var t in threads)
            {
                t.Join();
            }
        }
    }

    class WorkerThread
    {
        private int id;
        private int step;
        private volatile bool canStop = false;

        public WorkerThread(int id, int step)
        {
            this.id = id;
            this.step = step;
        }

        public void Stop()
        {
            canStop = true;
        }

        public void Run()
        {
            long sum = 0;
            long count = 0;
            long currentElement = 0;
            
            while (!canStop)
            {
                sum += currentElement;
                currentElement += step;
                count++;
            }
            Console.WriteLine($"Thread {id} - Sum: {sum}, Added: {count}");
        }
    }
}