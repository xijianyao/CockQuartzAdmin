using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using CockQuartz.Model;

namespace CockQuartz.Application
{
    public class DbContextFactory
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
