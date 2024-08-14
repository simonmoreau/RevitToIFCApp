using Autodesk.Oss.Model;
using MediatR;

namespace Application.Files.Queries.GetUploadUrl
{
    public class GetUploadUrlQuery : IRequest<Signeds3uploadResponse>
    {
        public readonly int ChunksNumber;
        public readonly string ObjectKey;

        public GetUploadUrlQuery(int chunksNumber, string objectKey)
        {
            ChunksNumber = chunksNumber;
            ObjectKey = objectKey;
        }
    }
}
