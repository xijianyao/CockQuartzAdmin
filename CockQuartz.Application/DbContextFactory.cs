using System.Runtime.Remoting.Messaging;
using CockQuartz.Model;

namespace CockQuartz.Core
{
    public class DbContextFactory
    {
        public static CockQuartzDbContext DbContext
        {
            get
            {
                if (!(CallContext.GetData("DbContext") is CockQuartzDbContext dbContext))
                {
                    dbContext = new CockQuartzDbContext();
                    CallContext.SetData("DbContext", dbContext);
                }
                return dbContext;
            }

        }
    }
}
