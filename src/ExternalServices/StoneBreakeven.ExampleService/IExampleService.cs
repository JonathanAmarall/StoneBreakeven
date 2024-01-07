namespace StoneBreakeven.ExampleService
{
    public interface IExampleService
    {
        Task<string> GetWithDelay(int sleep, CancellationToken cancellationToken);
        Task<string> GetWithInternalServerError(CancellationToken cancellationToken);
        Task<string> GetWithUnavailable(CancellationToken cancellationToken);
        Task<string> GetRandomResult(CancellationToken cancellationToken);
    }
}