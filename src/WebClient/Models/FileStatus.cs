namespace WebClient.Models
{
    public class FileStatus
    {
        public static FileStatus GetVersion
        {
            get
            {
                return new FileStatus("Looking for the Revit Version", true);
            }
        }

        public static FileStatus ReadyToUpload
        {
            get
            {
                return new FileStatus("Your file is ready to be uploaded", false);
            }
        }

        public static FileStatus Uploading
        {
            get
            {
                return new FileStatus("Your file is uploading", true);
            }
        }

        public static FileStatus Converting
        {
            get
            {
                return new FileStatus("Your file is converting", true);
            }
        }

        public static FileStatus Converted
        {
            get
            {
                return new FileStatus("Your file is converted", false);
            }
        }

        private string _text;
        public string Text
        {
            get
            {
                return _text;
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get
            {
                return _isLoading;
            }
        }

        private FileStatus(string statusText, bool isLoading)
        {
            _text = statusText;
            _isLoading = isLoading;
        }
    }
}
