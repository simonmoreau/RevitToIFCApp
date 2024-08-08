using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Files.Queries.GetUploadUrl
{
    public class GetUploadUrlQuery : IRequest<GetUploadUrlVm>
    {
        public readonly int ChunksNumber;

        public GetUploadUrlQuery(int chunksNumber)
        {
            ChunksNumber = chunksNumber;
        }
    }
}
