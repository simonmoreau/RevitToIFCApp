using Autodesk.Oss.Model;
using MediatR;

namespace Application.Files.Queries.GetDownloadUrlQuery
{
    public class GetDownloadUrlQuery : IRequest<Signeds3downloadResponse>
    {
        public readonly string ObjectKey;

        public GetDownloadUrlQuery(string objectKey)
        {
            ObjectKey = objectKey;
        }
    }
}
