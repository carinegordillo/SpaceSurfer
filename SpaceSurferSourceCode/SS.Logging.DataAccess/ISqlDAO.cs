using SS.SharedNamespace;

namespace SS.Logging.DataAccess
{
    public interface ISqlDAO
    {
        public Task<Response> WriteData(LogEntry log);

        public Task<Response> ReadData_Singular(int id);

        public Task<Response> ReadData_Multiple(int num);

        public Task<Response> UpdateData(int id, string column, string oldData, string newData);

        public Task<Response> DeleteData(int id);

    }
}
