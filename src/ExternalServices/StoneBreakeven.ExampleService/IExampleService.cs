﻿namespace StoneBreakeven.ExampleService
{
    public interface IExampleService
    {
        Task<string> GetWithDelay(int sleep, CancellationToken cancellationToken);
        Task<string> GetWithInternalServerError(CancellationToken cancellationToken);
    }
}