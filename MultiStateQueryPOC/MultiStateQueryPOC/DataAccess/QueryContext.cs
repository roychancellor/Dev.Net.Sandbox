using MultiStateQueryPOC.Models.Common;
using MultiStateQueryPOC.Models.Response;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MultiStateQueryPOC.DataAccess
{
    public class QueryContext : DbContext
    {
        public QueryContext() : base("QueryContext")
        {
        }

        public DbSet<State> States { get; set; }
        public DbSet<Mock> Mocks { get; set; }
    }
}