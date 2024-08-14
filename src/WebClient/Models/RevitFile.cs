using Microsoft.AspNetCore.Components.Forms;

namespace WebClient.Models
{
    public class RevitFile : IBrowserFile
    {
        private readonly IBrowserFile _browserFile;


        public RevitFile(IBrowserFile browserFile)
        {
            _browserFile = browserFile;
            Status = FileStatus.GetVersion;
        }

        public string Name
        {
            get { return _browserFile.Name; }
        }

        public string DownloadUrl { get; set; }

        public DateTimeOffset LastModified
        {
            get { return _browserFile.LastModified; }
        }

        public long Size
        {
            get { return _browserFile.Size; }
        }

        public string ContentType
        {
            get { return _browserFile.ContentType; }
        }

        public Stream OpenReadStream(long maxAllowedSize = 512000, CancellationToken cancellationToken = default)
        {
            return _browserFile.OpenReadStream(maxAllowedSize, cancellationToken);
        }

        private string _version;
        public string Version
        {
            get { return _version; }
            set { _version = value; }
        }

        private FileStatus _status;
        public FileStatus Status
        {
            get { return _status; }
            set { _status = value; }
        }

    }
}