using System.Threading;
using MagnaWms.Application.Warehouses.Repository;
using MagnaWms.Domain.UserAggregate;
using MagnaWms.Domain.WarehouseAggregate;
using MagnaWms.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace MagnaWms.Persistence.Repositories;
public class WarehouseRepository : BaseRepository<Warehouse>, IWarehouseRepository
{
    public WarehouseRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Warehouse?> ExistsByCodeAsync(string code, CancellationToken cancellationToken) => await Context.Set<Warehouse>()
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Code == code, cancellationToken);
}
