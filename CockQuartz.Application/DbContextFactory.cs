using CockQuartz.Model;
using System.Runtime.Remoting.Messaging;

namespace CockQuartz.Application
{
    internal class DbContextFactory
    {
        public static CockQuartzDbContext DbContext
        {
            get
            {
                if (!(CallContext.GetData("DbContext") is CockQuartzDbContext _dbContext))
                {
                    _dbContext = new CockQuartzDbContext();
                    CallContext.SetData("DbContext", _dbContext);
                }
                return _dbContext;
            }

        }
    }
}
