using Infrastructure;
using System;

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
