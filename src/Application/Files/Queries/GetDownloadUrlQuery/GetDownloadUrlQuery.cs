using Autodesk.Oss.Model;
using MediatR;

namespace Application.Files.Queries.GetDownloadUrlQuery
{
    public class GetDownloadUrlQuery : IRequest<Signeds3downloadResponse>
    {
        public readonly string ObjectKey;
        public readonly string FileName;

        public GetDownloadUrlQuery(string objectKey, string fileName)
        {
            ObjectKey = objectKey;
            FileName = fileName;
        }
    }
}
