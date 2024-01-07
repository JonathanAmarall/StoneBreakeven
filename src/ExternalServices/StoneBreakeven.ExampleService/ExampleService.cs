namespace StoneBreakeven.ExampleService
{
    public class ExampleService : IExampleService
    {
        private readonly HttpClient _client;

        public ExampleService(HttpClient client)
        {
            _client = client;
        }

        public async Task<string> GetRandomResult(CancellationToken cancellationToken)
        {
            return await _client.GetStringAsync($"/Random/500,200,500", cancellationToken);
        }

        public async Task<string> GetWithDelay(int sleep, CancellationToken cancellationToken)
        {
            return await _client.GetStringAsync($"/200?sleep={sleep}", cancellationToken);
        }

        public async Task<string> GetWithInternalServerError(CancellationToken cancellationToken)
        {
            return await _client.GetStringAsync($"/500", cancellationToken);
        }

        public async Task<string> GetWithUnavailable(CancellationToken cancellationToken)
        {
            return await _client.GetStringAsync($"/503", cancellationToken);
        }
    }
}