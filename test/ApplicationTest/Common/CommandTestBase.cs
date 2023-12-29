using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationTest.Common
{
    public class CommandTestBase : IDisposable
    {
        protected readonly AppDbContext _context;

        public CommandTestBase()
        {
            _context = AppContextFactory.Create();
        }

        public void Dispose()
        {
            AppContextFactory.Destroy(_context);
        }
    }
}
