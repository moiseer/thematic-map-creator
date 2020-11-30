using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Dal
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork Create();
        IUnitOfWork Create(string tag);
        Task<IUnitOfWork> CreateAsync(CancellationToken cancellationToken = default);
        Task<IUnitOfWork> CreateAsync(string tag, CancellationToken cancellationToken = default);
    }
}
