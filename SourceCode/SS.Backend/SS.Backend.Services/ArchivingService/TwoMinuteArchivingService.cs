using SS.Backend.SharedNamespace;
using SS.Backend.DataAccess;

using System;
using System.Threading;


namespace SS.Backend.Services.ArchivingService 
{
    public class TwoMinuteArchivingService : IReoccuringArchivingService
    {
        private readonly ITargetArchivingDestination _archivingTarget;
        private Thread _thread;
        private bool _isRunning = true;
        private AutoResetEvent _waitHandle = new AutoResetEvent(false);



        


        public TwoMinuteArchivingService(ITargetArchivingDestination archivingTarget)
        {
            _archivingTarget = archivingTarget;
        }


        public void Start()
        {
            _thread = new Thread(new ThreadStart(Run));
            Console.WriteLine("Starting TwoMinuteArchivingService...");
            PerformScheduledTask();
            _thread.Start();
        }

        public void Stop()
        {
            Console.WriteLine("Stopping TwoMinuteArchivingService...");
            _isRunning = false;
            _waitHandle.Set();  // Signal the thread to stop waiting and check the loop condition
            if (_thread.IsAlive)
            {
                _thread.Join();
            }
            Console.WriteLine("TwoMinuteArchivingService stopped.");
        }
        
        public void Run()
        {
            Console.WriteLine("TwoMinuteArchivingService running...");
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

        public DateTime GetNextRunTime()
        {
            Console.WriteLine("Getting next run time...");
            var nextMonth = DateTime.Now.AddMinutes(2);
            return nextMonth;
        }

        public async void PerformScheduledTask()
        {
            Response tableReaderResponse = new Response();
            Response tableResetterResponse = new Response();



            ReadTableTarget tableReader = new ReadTableTarget();

            try
            {
                tableReaderResponse = await tableReader.ReadSqlTable("dbo.Logs");
                if (tableReaderResponse.HasError == true)
                {
                    Console.WriteLine("Error reading table: " + tableReaderResponse.ErrorMessage);
                }
                else
                {
                    var tempFilePath = ArchivingFormats.SaveToTextFile(tableReaderResponse.ValuesRead);


                    ArchivesModel s3Info = new ArchivesModel();
                    s3Info.destination = "space-surfer-archivestest";
                    s3Info.filePath = tempFilePath;
                    
                    string dateTimeNow = DateTime.Now.ToString("yyyyMMdd_HHmm"); 
                    s3Info.fileName = $"SpaceSurferLogs_{dateTimeNow}.txt";

                    Console.WriteLine("Performing scheduled task..."+ DateTime.Now);
                    _archivingTarget.UploadFileAsync(s3Info);
                   tableResetterResponse = await tableReader.ResetSqlTable("dbo.Logs");

                   if (tableResetterResponse.HasError == true)
                   {
                       Console.WriteLine("Error resetting table: " + tableResetterResponse.ErrorMessage);
                   }    
                    
                }
            }
            catch (Exception e){
                Console.WriteLine("Error reading table: " + e.Message);
            }

        }
    }



}