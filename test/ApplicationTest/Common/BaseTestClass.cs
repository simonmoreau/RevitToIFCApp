using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace ApplicationTest.Common
{

    public class BaseTestClass : IDisposable, IAsyncLifetime
    {
        internal readonly ITestOutputHelper _output;

        public BaseTestClass(ITestOutputHelper output)
        {
            _output = output;
        }

        public void Dispose()
        {

        }

        public async Task InitializeAsync()
        {

        }

        public async Task DisposeAsync()
        {

        }
    }
}
