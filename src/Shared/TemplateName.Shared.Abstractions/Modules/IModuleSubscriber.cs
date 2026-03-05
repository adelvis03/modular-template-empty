using System;
using System.Threading;
using System.Threading.Tasks;

namespace TemplateName.Shared.Abstractions.Modules;

public interface IModuleSubscriber
{
    IModuleSubscriber Subscribe<TRequest, TResponse>(string path,
        Func<TRequest, IServiceProvider, CancellationToken, Task<TResponse>> action)
        where TRequest : class where TResponse : class;

    IModuleSubscriber Subscribe<TRequest>(string path,
        Func<TRequest, IServiceProvider, CancellationToken, Task> action)
        where TRequest : class;
}



