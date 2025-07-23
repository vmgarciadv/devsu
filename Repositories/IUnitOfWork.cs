using System;
using System.Threading.Tasks;

namespace devsu.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IClienteRepository Clientes { get; }
        ICuentaRepository Cuentas { get; }
        Task<int> CompleteAsync();
    }
}