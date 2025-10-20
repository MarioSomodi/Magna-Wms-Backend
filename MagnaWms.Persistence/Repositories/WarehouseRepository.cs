using MagnaWms.Application.Warehouses.Repository;
using MagnaWms.Domain.WarehouseAggregate;
using MagnaWms.Persistence.Context;

namespace MagnaWms.Persistence.Repositories;
public class WarehouseRepository : BaseRepository<Warehouse>, IWarehouseRepository
{
    public WarehouseRepository(AppDbContext context) : base(context)
    {
    }
}
