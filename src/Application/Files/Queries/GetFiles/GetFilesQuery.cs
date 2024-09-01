
using Autodesk.Oss.Model;
using MediatR;

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
