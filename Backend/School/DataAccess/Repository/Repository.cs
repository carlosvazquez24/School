using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class, new()
    {
        private readonly ApplicationDbContext _context;
        protected ApplicationDbContext Context => _context;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Método para guardar los cambios explícitamente
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        public virtual async Task<TEntity> AddRepositoryAsync(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException($"{nameof(AddRepositoryAsync)} entity must not be null");
            }

            await _context.AddAsync(entity);
            return entity;
        }

        public virtual IQueryable<TEntity> GetAllRepositoryAsync()
        {

            return _context.Set<TEntity>();

        }

        public virtual async Task<TEntity?> GetByIdRepositoryAsync(params object[] keyValues)
        {
            if (keyValues == null || keyValues.Length == 0)
            {
                throw new ArgumentException("Debe proporcionar valores de clave para la búsqueda.");
            }

            return await _context.FindAsync<TEntity>(keyValues);
        }

        public Task<TEntity> UpdateRepositoryAsync(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException($"{nameof(UpdateRepositoryAsync)} entity must not be null");
            }

            _context.Update(entity);
            return Task.FromResult(entity); // Devuelve un Task sin hacer el método async
        }

        public Task DeleteRepositoryAsync(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException($"{nameof(DeleteRepositoryAsync)} entity must not be null");
            }

            _context.Remove(entity);
            return Task.CompletedTask; // Devuelve un Task ya completado
        }

        public virtual async Task<TEntity?> GetByIdWithIncludesAsync(
            Func<IQueryable<TEntity>, IQueryable<TEntity>> includeFunc,
            params object[] keyValues)
        {
            if (keyValues == null || keyValues.Length == 0)
                throw new ArgumentException("Se requieren valores de clave primaria.");

            IQueryable<TEntity> query = _context.Set<TEntity>();

            if (includeFunc != null)
                query = includeFunc(query);

            var keyProperties = _context.Model.FindEntityType(typeof(TEntity))!
                .FindPrimaryKey()!.Properties;

            if (keyValues.Length != keyProperties.Count)
                throw new ArgumentException("Número incorrecto de valores de clave.");

            var parameter = Expression.Parameter(typeof(TEntity), "e");
            var predicate = keyProperties
                .Select((p, i) =>
                    Expression.Equal(
                        Expression.Property(parameter, p.PropertyInfo!),
                        Expression.Constant(keyValues[i])
                    )
                )
                .Aggregate((prev, next) => Expression.AndAlso(prev, next));

            var lambda = Expression.Lambda<Func<TEntity, bool>>(predicate, parameter);

            return await query.FirstOrDefaultAsync(lambda);
        }


    }
}
