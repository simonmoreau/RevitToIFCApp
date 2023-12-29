using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ApplicationTest.Common
{
    public class QueryTestFixture : IDisposable
    {
        public AppDbContext Context { get; private set; }

        public QueryTestFixture()
        {
            Context = AppContextFactory.Create();
        }

        public void Dispose()
        {
            AppContextFactory.Destroy(Context);
        }
    }

    [CollectionDefinition("QueryCollection")]
    public class QueryCollection : ICollectionFixture<QueryTestFixture> { }
}
