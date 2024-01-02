using Application.Objets.Queries.GetObjetList;
using ApplicationTest.Common;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ApplicationTest.Objets.Queries
{
    [Collection("QueryCollection")]
    public class GetObjetListQueryHandlerTest : CommandTestBase
    {
        [Fact]
        public async Task GetObjetList()
        {
            // Arrange
            string objectId = "YTIxMzMyZjItYmZhZi00MDI3LTg0MmQtOWJkNDM5ODY5YWI5";
            GetObjetListQueryHandler request = new GetObjetListQueryHandler(_context);

            // Act 
            ObjetListVm result = await request.Handle(new GetObjetListQuery(), CancellationToken.None);

            // Assert 
            Assert.NotNull(result);
            Assert.IsType<List<Objet>>(result.Objets);
            Assert.Equal(objectId, result.Objets[0].Id);
            Assert.Equal(2, result.Objets.Count);
        }
    }
}
