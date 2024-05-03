public interface IReoccuringArchivingService
{
    public void Start();
    public void Stop();
    public void Run();
    public DateTime GetNextRunTime();
    public void PerformScheduledTask();
}