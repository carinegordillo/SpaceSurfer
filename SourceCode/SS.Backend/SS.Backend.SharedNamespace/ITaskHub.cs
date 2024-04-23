namespace SS.Backend.SharedNamespace
{
    public interface ITaskHub
    {
        public string? hashedUsername{get; set;}
        public string? title {get; set;}
        public string? description {get;set;}
        public DateTime? dueDate {get;set;}
        public string? priority {get;set;}
        public int? notificationSetting{get;set;}
    }
}