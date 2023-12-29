using Application.Objets.Queries.GetObjetList;
using Application.Objets.Queries.GetPermisList;
using ApplicationTest.Common;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ApplicationTest.Plans.Queries
{
    [Collection("QueryCollection")]
    public class GetPlanListQueryHandlerTest : CommandTestBase
    {
        [Fact]
        public async Task GetObjetList()
        {
            // Arrange 
            GetPlanListQueryHandler request = new GetPlanListQueryHandler(_context);

            // Act 
            var result = await request.Handle(new GetPlanListQuery(), CancellationToken.None);

            // Assert 
            Assert.NotNull(result);
            Assert.IsType<List<Plan>>(result.Plans);
            Assert.True(result.Plans.Count == 0);
        }
    }
}




