using Application.Objets.Commands.CreateObjetCommand;
using Application.Objets.Queries.GetObjetById;
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

namespace ApplicationTest.Objets.Commands
{
    [Collection("QueryCollection")]
    public class CreatePermisCommandTest : CommandTestBase
    {
        private readonly GetPlanListQueryHandler _sut;

        public CreatePermisCommandTest()
        {
            _sut = new(_context);
        }

        [Fact]
        public async Task CreatePermis()
        {
            // Arrange
            CreatePlanCommand.Handler command = new CreatePlanCommand.Handler(_context);

            // Act
            string id = await command.Handle(new CreatePlanCommand() 
            { 
                IdPermis = "b43f4efc2c9b4710a26d3edc53e05958",
                ConstructionSite = new ConstructionSite("a0a34baf88494274978d08099504cabb", 
                                   new Site("64417605db5e472d937ed0320cf14835",
                                   new Company("4be660373e1e4e6b86e573dcc531d7b4"))),
                 Name ="abajbz",
                 Infos = "ekofzo",
                 DateEnd = DateTime.Now,
                 DateStart = DateTime.Now,
            }, CancellationToken.None);

            // Arrange
            PlanListVm result = await _sut.Handle(new GetPlanListQuery(), CancellationToken.None);
            Assert.IsType<List<Plan>>(result.Plans);
            Assert.NotNull(result);
            Assert.Equal(1, result.Plans.Count);
        }
    }
}