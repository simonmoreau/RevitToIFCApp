using System.Runtime.Serialization;

namespace WebClient.Models
{
    //
    // Summary:
    //     An object representing the response payload on successful execution of a Generate
    //     Signed S3 Download URL operation.
    [DataContract]
    public class Signeds3downloadResponse
    {
        //
        // Summary:
        //     Gets or Sets Status
        [DataMember(Name = "status", EmitDefaultValue = true)]
        public DownloadStatus Status { get; set; }

        //
        // Summary:
        //     A S3 signed URL with which to download the object. This attribute is returned
        //     when `status` is `complete` or `fallback`; in the latter case, this will return
        //     an OSS signed URL, not an S3 signed URL.
        //
        // Value:
        //     A S3 signed URL with which to download the object. This attribute is returned
        //     when `status` is `complete` or `fallback`; in the latter case, this will return
        //     an OSS signed URL, not an S3 signed URL.
        [DataMember(Name = "url", EmitDefaultValue = false)]
        public string Url { get; set; }

        //
        // Summary:
        //     A map of S3 signed URLs, one for each chunk of an unmerged resumable upload.
        //     This attribute is returned when `status` is `chunked`. The key of each entry
        //     is the byte range of the total file which the chunk comprises.
        //
        // Value:
        //     A map of S3 signed URLs, one for each chunk of an unmerged resumable upload.
        //     This attribute is returned when `status` is `chunked`. The key of each entry
        //     is the byte range of the total file which the chunk comprises.
        [DataMember(Name = "urls", EmitDefaultValue = false)]
        public object Urls { get; set; }

        //
        // Summary:
        //     The values that were requested for the following parameters when requesting the
        //     S3 signed URL. - `Content-Type` - `Content-Disposition` - `Cache-Control`.
        //
        // Value:
        //     The values that were requested for the following parameters when requesting the
        //     S3 signed URL. - `Content-Type` - `Content-Disposition` - `Cache-Control`.
        [DataMember(Name = "params", EmitDefaultValue = false)]
        public object Params { get; set; }

        //
        // Summary:
        //     The total amount of storage space occupied by the object, in bytes.
        //
        // Value:
        //     The total amount of storage space occupied by the object, in bytes.
        [DataMember(Name = "size", EmitDefaultValue = false)]
        public long? Size { get; set; }

        //
        // Summary:
        //     A hash value computed from the data of the object, if available.
        //
        // Value:
        //     A hash value computed from the data of the object, if available.
        [DataMember(Name = "sha1", EmitDefaultValue = false)]
        public string Sha1 { get; set; }

        //
        // Summary:
        //     Initializes a new instance of the Autodesk.Oss.Model.Signeds3downloadResponse
        //     class.
        public Signeds3downloadResponse()
        {
        }

    }

    public enum DownloadStatus
    {
        //
        // Summary:
        //     Enum Complete for value: complete
        [EnumMember(Value = "complete")]
        Complete,
        //
        // Summary:
        //     Enum Chunked for value: chunked
        [EnumMember(Value = "chunked")]
        Chunked,
        //
        // Summary:
        //     Enum Fallback for value: fallback
        [EnumMember(Value = "fallback")]
        Fallback
    }

}
