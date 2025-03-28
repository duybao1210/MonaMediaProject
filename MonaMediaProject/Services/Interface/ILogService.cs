namespace MonaMediaProject.Services.Interface
{
    public interface ILogService
    {
        void WriteLogError(string message, Exception ex);
    }
}
