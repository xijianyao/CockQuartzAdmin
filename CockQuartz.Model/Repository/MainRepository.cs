using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FeI.Domain.Entities;
using FeI.EntityFramework;
using FeI.EntityFramework.Repository;

namespace CockQuartz.Model.Repository
{
    public class MainRepository<TEntity, TPrimaryKey> : EfRepositoryBase<CockQuartzDbContext, TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        public MainRepository(IDbContextProvider<CockQuartzDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }

    public class MainRepository<TEntity> : EfRepositoryBase<CockQuartzDbContext, TEntity>
        where TEntity : class, IEntity
    {
        public MainRepository(IDbContextProvider<CockQuartzDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }
}
