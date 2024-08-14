
using Autodesk.Oss.Model;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Files.Queries.GetFiles
{
    public class GetFilesQuery : IRequest<BucketObjects>
    {
        public readonly string BucketKey;

        public GetFilesQuery(string bucketKey)
        {
            BucketKey = bucketKey;
        }
    }
}
