using System;
using System.Threading.Tasks;

namespace TemplateName.Shared.Infrastructure.Postgres;

public interface IUnitOfWork
{
    Task ExecuteAsync(Func<Task> action);
}

