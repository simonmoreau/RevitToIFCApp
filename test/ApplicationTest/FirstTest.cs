using ApplicationTest.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace ApplicationTest
{
    [Collection("DBContext")]

    public class FirstTest : BaseTestClass
    {
        public FirstTest(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task FirstTest1()
        {
            _output.WriteLine("FirstTest1");
            Assert.Empty("");
        }
    }
}

