using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eHi.Library.Enum;
using eHi.Library.Interface;
using FeI.Configuration.Startup;
using FeI.Domain.Uow;

namespace CockQuartzAdmin.Infrastructure
{
    /// <summary>
    ///     连接字符串解析服务
    /// </summary>
    public class ConnectionStringResolver : IConnectionStringResolver
    {
        private const string DbContextTypeKey = "DbContextType";
        private readonly IStartupConfiguration _config;
        private readonly IDbConnectionStringResolver _dbConnectionStringResolver;

        private readonly Dictionary<Type, DbConfigType> _dbContextMapper = new Dictionary<Type, DbConfigType>
        {
            //{typeof(PrivilegeDbContext),DbConfigType.Privilige },
            //{typeof(BalanceDbContext),DbConfigType.Balance },
            //{typeof(IdentityDbContext),DbConfigType.Identity },
            //{typeof(EhiDbContext),DbConfigType.Default },
            //{typeof(GiftCardDbContext),DbConfigType.Default }
        };

        /// <summary>
        ///     Ctor.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="dbConnectionStringResolver"></param>
        public ConnectionStringResolver(IStartupConfiguration config, IDbConnectionStringResolver dbConnectionStringResolver)
        {
            _config = config;
            _dbConnectionStringResolver = dbConnectionStringResolver;
        }

        /// <summary>
        ///     获取连接字符串
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public string GetNameOrConnectionString(ConnectionStringResolveArgs args)
        {
            //if (!args.ContainsKey(DbContextTypeKey))
            //    return _config.DefaultNameOrConnectionString;
            //var dbContextType = args[DbContextTypeKey] as Type;
            //if (dbContextType != null && _dbContextMapper.ContainsKey(dbContextType))
            //    return _dbConnectionStringResolver.ResolveConnectionString(_dbContextMapper[dbContextType]);
            //return _dbConnectionStringResolver.ResolveConnectionString(DbConfigType.Identity);
            return @"data source=(localdb)\MSSQLLocalDB;initial catalog=CockQuartz;Integrated Security=True;MultipleActiveResultSets=True;App=EntityFramework";
        }
    }
}