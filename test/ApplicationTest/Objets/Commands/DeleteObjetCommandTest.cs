using Application.Common.Exceptions;
using Application.Objets.Commands.DeleteObjetCommand;
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
using Xunit.Abstractions;

namespace ApplicationTest.Objets.Commands
{
    [Collection("QueryCollection")]
    public class DeleteObjetCommandTest : CommandTestBase
    {
        private readonly GetObjetByIdQueryHandler _sut;

        public DeleteObjetCommandTest()
        {
            // Arrange
            _sut = new GetObjetByIdQueryHandler(_context);
        }

        [Fact]
        public async Task DeleteObjet()
        {
            // Arrange : Vérification que l'objet existe dans la base
            string objectId = "ZjMxODljNzMtMjIxNC00OWM1LWI4YzEtNGFlNGVhMmNjZDQw";
            var before = await _sut.Handle(new GetObjetByIdQuery { Id = objectId }, CancellationToken.None);
            Assert.IsType<Objet>(before);
            Assert.NotNull(before);
            Assert.Equal(objectId, before.Id);

            // Act : Suppression dans la base
            DeleteObjetCommand.Handler command = new DeleteObjetCommand.Handler(_context);
            await command.Handle(new DeleteObjetCommand { Id = objectId }, CancellationToken.None);

            // Assert : On vérifie que l'objet n'est plus dans la base
            await Assert.ThrowsAsync<NotFoundException>(() => _sut.Handle(new GetObjetByIdQuery { Id = objectId }, CancellationToken.None));
        }
    }
}
