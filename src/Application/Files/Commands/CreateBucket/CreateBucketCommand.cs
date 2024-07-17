using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Files.Commands.CreateBucket
{
    public class CreateBucketCommand : IRequest<string>
    {
        public string? Name { get; set; }
    }
}
