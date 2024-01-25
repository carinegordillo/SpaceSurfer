namespace SS.Backend.SharedNamespace
{
    public class Response
    {
        public bool HasError { get; set; } = true;
        public string? ErrorMessage { get; set; }
        public int RowsAffected { get; set; } = 0;
        public List<List<object>>? ValuesRead { get; set; }
    }
}
