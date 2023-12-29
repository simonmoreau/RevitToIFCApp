using Application.Common.Exceptions;
using Application.Objets.Queries.GetObjetById;
using ApplicationTest.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Application.Objets.Commands.CreateObjetCommand;
using Domain.Entities;

namespace ApplicationTest.Objets.Commands
{
    [Collection("QueryCollection")]
    public class CreateObjetCommandTest : CommandTestBase
    {
        private readonly GetObjetByIdQueryHandler _sut;

        public CreateObjetCommandTest()
        {
            _sut = new GetObjetByIdQueryHandler(_context);
        }

        [Fact]
        public async Task CreateObjet()
        {
            // Arrange
            CreateObjetCommand.Handler command = new CreateObjetCommand.Handler(_context);

            // Act
            string id = await command.Handle(new CreateObjetCommand(), CancellationToken.None);

            // Arrange
            var result = await _sut.Handle(new GetObjetByIdQuery { Id = id }, CancellationToken.None);
            Assert.IsType<Objet>(result);
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
        }
    }
}
