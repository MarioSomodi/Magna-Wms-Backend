using MagnaWms.Application.UnitOfMeasures.Repository;
using MagnaWms.Domain.UnitOfMeasureAggregate;
using MagnaWms.Persistence.Context;

namespace MagnaWms.Persistence.Repositories;

public class UnitOfMeasureRepository : BaseRepository<UnitOfMeasure>, IUnitOfMeasureRepository
{
    public UnitOfMeasureRepository(AppDbContext context) : base(context) { }
}
