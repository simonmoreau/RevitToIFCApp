using Application.Objets.Queries.GetObjetList;
using Application.Sites.Queries.GetSiteList;
using ApplicationTest.Common;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ApplicationTest.Sites.Queries
{
    [Collection("QueryCollection")]
    public class GetSiteListQueryHandlerTest : CommandTestBase
    {
        [Fact]
        public async Task GetSiteList()
        {
            // Arrange 
            GetSiteListQueryHandler request = new GetSiteListQueryHandler(_context);

            // Act 
            var result = await request.Handle(new GetSiteListQuery(), CancellationToken.None);

            // Assert 
            Assert.NotNull(result);
            Assert.IsType<List<Site>>(result.Sites);
            Assert.Equal("4744505a588b42ae8e3aa47971646960", result.Sites[0].Id);
            Assert.True(result.Sites.Count == 4);
        }
    }
}
