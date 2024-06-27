using System.Threading;
using System.Threading.Tasks;

namespace Core.Dal
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork Create();
        Task<IUnitOfWork> CreateAsync(CancellationToken cancellationToken = default);
    }
}
