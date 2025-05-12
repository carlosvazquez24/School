using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public interface IRepository<TEntity> where TEntity : class, new()
    {
        Task<IDbContextTransaction> BeginTransactionAsync();
        IQueryable<TEntity> GetAllRepositoryAsync();

        Task<TEntity?> GetByIdRepositoryAsync(params object[] keyValues);

        Task<TEntity> AddRepositoryAsync(TEntity entity);

        Task<TEntity> UpdateRepositoryAsync(TEntity entity);

        Task DeleteRepositoryAsync(TEntity entity);

        Task SaveChangesAsync();


        Task<TEntity?> GetByIdWithIncludesAsync(
            Func<IQueryable<TEntity>, IQueryable<TEntity>> includeFunc,
            params object[] keyValues
        );
    }
}
