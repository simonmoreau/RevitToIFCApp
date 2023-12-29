using Application.Objets.Queries.GetObjetById;
using ApplicationTest.Common;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ApplicationTest.Objets.Queries
{
    [Collection("QueryCollection")]
    public class GetObjetByIdQueryHandlerTest : CommandTestBase
    {
        [Fact]
        public async Task GetObjetById()
        {
            // Arrange
            string objectId = "ZjMxODljNzMtMjIxNC00OWM1LWI4YzEtNGFlNGVhMmNjZDQw";
            GetObjetByIdQueryHandler request = new GetObjetByIdQueryHandler(_context);

            // Act
            var result = await request.Handle(new GetObjetByIdQuery { Id = objectId }, CancellationToken.None);

            // Assert
            Assert.IsType<Objet>(result);
            Assert.NotNull(result);
            Assert.Equal(objectId, result.Id);
        }
    }
}
