using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using Amazon.S3;

using SS.Backend.SharedNamespace;
using Ionic.Zip;

namespace SS.Backend.Services.ArchivingService
{
    public class ArchivingService : IReoccuringArchivingService
    {
        private readonly ITargetArchivingDestination _archivingTarget;
        private Thread _thread;
        private readonly ArchivingServiceConfig _config;
        private bool _isRunning = true;
        
        private DateTime lastrun;
        private AutoResetEvent _waitHandle = new AutoResetEvent(false);

        const int MAX_SLEEP_MILLISECONDS = int.MaxValue - 1;

        public ArchivingService(ITargetArchivingDestination archivingTarget, string configFilePath)
        {
            _archivingTarget = archivingTarget;
            _config = ArchivingServiceConfig.LoadConfig(configFilePath);
            lastrun = _config.StartDate;
        }


        public void Start()
        {
            _thread = new Thread(new ThreadStart(Run));
            
            _thread.Start();
        }

        public void Stop()
        {
            Console.WriteLine("Stopping ArchivingService...");
            _isRunning = false;
            _waitHandle.Set();
            if (_thread.IsAlive)
            {
                _thread.Join();
            }
            Console.WriteLine("ArchivingService stopped.");
        }

        

        public void Run()
        {
            Console.WriteLine("ArchivingService running...");

            const int MaxSleepMilliseconds = int.MaxValue - 1; 

            Console.WriteLine(MaxSleepMilliseconds);

            if (_config.StartDate > DateTime.Now)
            {
                Console.WriteLine("Start date is in the future, waiting until start date...");
                TimeSpan initialWaitTime = _config.StartDate - DateTime.Now;

                while (initialWaitTime > TimeSpan.Zero && _isRunning)
                {
                    TimeSpan sleepInterval = initialWaitTime.TotalMilliseconds > MaxSleepMilliseconds
                        ? TimeSpan.FromMilliseconds(MaxSleepMilliseconds)
                        : initialWaitTime;
                    Console.WriteLine($"Waiting {sleepInterval} until start date...");
                    _waitHandle.WaitOne(sleepInterval);
                    initialWaitTime = _config.StartDate - DateTime.Now;
                }
            }

            while (_isRunning)
            {
                DateTime nextRun = GetNextRunTime();
                TimeSpan waitTime = nextRun - DateTime.Now;

                while (waitTime > TimeSpan.Zero && _isRunning)
                {
                    TimeSpan sleepInterval = waitTime.TotalMilliseconds > MaxSleepMilliseconds
                        ? TimeSpan.FromMilliseconds(MaxSleepMilliseconds)
                        : waitTime;

                    _waitHandle.WaitOne(sleepInterval);
                    waitTime = nextRun - DateTime.Now;
                }

                if (_isRunning)
                {
                    PerformScheduledTask();
                }
            }
        }


        public DateTime GetNextRunTime()
        {

            DateTime nextRun = lastrun;
            Console.WriteLine("Starting ArchivingService...");
            Console.WriteLine("start date: " + _config.StartDate, "interval: " + _config.Interval, "unit: " + _config.Unit);

            switch (_config.Unit)
            {
                case ScheduleUnit.Minutes:
                    nextRun = lastrun.AddMinutes(_config.Interval);
                    Console.WriteLine("minuets "+ nextRun);
                    break;

                case ScheduleUnit.Days:
                    nextRun = lastrun.AddDays(_config.Interval);
                    Console.WriteLine("days "+ nextRun);
                    break;

                case ScheduleUnit.Months:
                    nextRun = lastrun.AddMonths(_config.Interval);
                    Console.WriteLine("months "+ nextRun);
                    break;
            }

            lastrun = nextRun;

            Console.WriteLine($"Next scheduled run time: {nextRun}");
            return nextRun;
        }

        public async void PerformScheduledTask()
        {
            Response tableReaderResponse = new Response();
            Response tableResetterResponse = new Response();

            ReadTableTarget tableReader = new ReadTableTarget();

            try
            {
                tableReaderResponse = await tableReader.ReadSqlTable("dbo.Logs");
                if (tableReaderResponse.HasError)
                {
                    Console.WriteLine("Error reading table: " + tableReaderResponse.ErrorMessage);
                }
                else
                {
                    var tempFilePath = ArchivingFormats.SaveToTextFile(tableReaderResponse.ValuesRead);
                    // Create a zip file
                    string zipFilePath = Path.ChangeExtension(tempFilePath, ".zip");
                    using (ZipFile zip = new ZipFile())
                    {
                        // Add the text file to the zip correctly
                        zip.AddFile(tempFilePath, "");
                        zip.Save(zipFilePath);
                    }

                    ArchivesModel s3Info = new ArchivesModel
                    {
                        destination = _config.BucketName,
                        filePath = zipFilePath,
                        fileName = $"SpaceSurferLogs_{DateTime.Now:yyyyMMdd_HHmm}.zip"
                    };

                    Console.WriteLine("Performing scheduled task..." + DateTime.Now);
                    await _archivingTarget.UploadFileAsync(s3Info);
                    tableResetterResponse = await tableReader.ResetSqlTable("dbo.Logs");
                    if (File.Exists(tempFilePath))
                    {
                        File.Delete(tempFilePath);
                        Console.WriteLine("Temporary file deleted successfully.");
                    }
                    else
                    {
                        Console.WriteLine("No file to delete.");
                    }
                    

                    if (tableResetterResponse.HasError)
                    {
                        Console.WriteLine("Error resetting table: " + tableResetterResponse.ErrorMessage);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error reading table: " + e.Message);
            }
        }
    }
}
