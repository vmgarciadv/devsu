using System.Threading.Tasks;
using devsu.Data;

namespace devsu.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DevsuContext _context;
        public IClienteRepository Clientes { get; private set; }
        
        public UnitOfWork(DevsuContext context)
        {
            _context = context;
            Clientes = new ClienteRepository(_context);
        }
        
        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }
        
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}