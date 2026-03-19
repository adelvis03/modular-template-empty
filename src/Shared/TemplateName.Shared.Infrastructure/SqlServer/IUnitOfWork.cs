using System;
using System.Threading.Tasks;

namespace TemplateName.Shared.Infrastructure.SqlServer;

public interface IUnitOfWork
{
    Task ExecuteAsync(Func<Task> action);
}



