using Autodesk.Oss.Model;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
