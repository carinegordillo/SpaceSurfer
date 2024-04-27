using SS.Backend.SharedNamespace;
using System;
using System.Threading;


namespace SS.Backend.Services.ArchivingService
{
    public class MonthlyArchivingService
    {
        private readonly ITargetArchivingDestination _archivingTarget;
        private Thread _thread;
        private bool _isRunning = true;


        // public MonthlyArchivingService(ITargetArchivingDestination archivingTarget)
        // {
        //     _archivingTarget = archivingTarget;
        // }


          public void Start()
        {
            _thread = new Thread(new ThreadStart(Run));
            _thread.Start();
        }

        public void Stop()
        {
            _isRunning = false;
            if (_thread.IsAlive) {
                _thread.Interrupt();
            }
            _thread.Join(); // Ensure the thread finishes cleanly
        }
        
        private void Run()
        {
            while (_isRunning)
            {
                try
                {
                    DateTime nextRun = GetNextRunTime();
                    TimeSpan waitTime = nextRun - DateTime.Now;

                    if (waitTime > TimeSpan.Zero)
                    {
                        Thread.Sleep(waitTime);
                    }

                    PerformScheduledTask();
                }
                catch (ThreadInterruptedException)
                {
                    Console.WriteLine("Scheduler thread interrupted.");
                }
            }
        }

        private DateTime GetNextRunTime()
        {
            var nextMonth = DateTime.Now.AddMonths(1);
            return new DateTime(nextMonth.Year, nextMonth.Month, 1);
        }

        private void PerformScheduledTask()
        {
            Console.WriteLine("Performing scheduled task..."+ DateTime.Now);
            
        }
    }



}
