using SS.Backend.Services.LoggingService;
using SS.Backend.SharedNamespace;
using System;
using System.Threading;


namespace SS.Backend.Services.ArchivingService
{
    public class MonthlyArchivingService : IReoccuringArchivingService
    {
        private readonly ITargetArchivingDestination _archivingTarget;
        private Thread _thread;
        private bool _isRunning = true;

        


        public MonthlyArchivingService(ITargetArchivingDestination archivingTarget)
        {
            _archivingTarget = archivingTarget;
        }

          public void Start()
        {
            PerformScheduledTask();
            _thread = new Thread(new ThreadStart(Run));
            _thread.Start();
        }

        public void Stop()
        {
            _isRunning = false;
            if (_thread.IsAlive) {
                _thread.Interrupt();
            }
            _thread.Join(); 
        }
        
        public void Run()
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

        public DateTime GetNextRunTime()
        {
            var nextMonth = DateTime.Now.AddMonths(1);
            return new DateTime(nextMonth.Year, nextMonth.Month, 1);
        }

        public async void PerformScheduledTask()
        {
            Response tableReaderResponse = new Response();



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
                    
                    string dateTimeNow = DateTime.Now.ToString("yyyyMMdd_HHmmss"); 
                    s3Info.fileName = $"SpaceSurferLogs_{dateTimeNow}.txt";

                    Console.WriteLine("Performing scheduled task..."+ DateTime.Now);
                    _archivingTarget.UploadFileAsync(s3Info);
                }
            }
            catch (Exception e){
                Console.WriteLine("Error reading table: " + e.Message);
            }

        }
    }



}
