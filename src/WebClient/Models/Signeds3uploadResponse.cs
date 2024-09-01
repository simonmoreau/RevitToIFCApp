using System.Runtime.Serialization;

namespace WebClient.Models
{
    /// <summary>
    /// The response payload to a Generate Signed S3 Upload URL operation.
    /// </summary>
    [DataContract]
    public class Signeds3uploadResponse
    {

        /// <summary>
        ///An ID that uniquely identifies the upload session. It allows OSS to differentiate between fresh upload attempts from attempts to resume uploading data for an active upload session, in case of network interruptions. You must provide this value when:
        ///
        ///- Re-requesting chunk URLs for an active upload session. 
        ///- When calling the [Complete Upload to S3 Signed URL](/en/docs/data/v2/reference/http/buckets-:bucketKey-objects-:objectKey-signeds3upload-POST/) operation to end an active upload session.
        /// </summary>
        /// <value>
        ///An ID that uniquely identifies the upload session. It allows OSS to differentiate between fresh upload attempts from attempts to resume uploading data for an active upload session, in case of network interruptions. You must provide this value when:
        ///
        ///- Re-requesting chunk URLs for an active upload session. 
        ///- When calling the [Complete Upload to S3 Signed URL](/en/docs/data/v2/reference/http/buckets-:bucketKey-objects-:objectKey-signeds3upload-POST/) operation to end an active upload session.
        /// </value>
        [DataMember(Name = "uploadKey", EmitDefaultValue = false)]
        public string UploadKey { get; set; }

        /// <summary>
        ///An array of signed URLs. For a single-part upload, this will contain only one URL. For a multipart upload, there will be one for each chunk of a multipart upload; the index of the URL in the array corresponds to the part number of the chunk.
        /// </summary>
        /// <value>
        ///An array of signed URLs. For a single-part upload, this will contain only one URL. For a multipart upload, there will be one for each chunk of a multipart upload; the index of the URL in the array corresponds to the part number of the chunk.
        /// </value>
        [DataMember(Name = "urls", EmitDefaultValue = false)]
        public List<string> Urls { get; set; }

        /// <summary>
        ///The date and time, in the ISO 8601 format, indicating when the signed URLs will expire.
        /// </summary>
        /// <value>
        ///The date and time, in the ISO 8601 format, indicating when the signed URLs will expire.
        /// </value>
        [DataMember(Name = "urlExpiration", EmitDefaultValue = false)]
        public string UrlExpiration { get; set; }

        /// <summary>
        ///The deadline to call [Complete Upload to S3 Signed URL](/en/docs/data/v2/reference/http/buckets-:bucketKey-objects-:objectKey-signeds3upload-POST/) for the object. If not completed by this time, all uploaded data for this session will be discarded.
        /// </summary>
        /// <value>
        ///The deadline to call [Complete Upload to S3 Signed URL](/en/docs/data/v2/reference/http/buckets-:bucketKey-objects-:objectKey-signeds3upload-POST/) for the object. If not completed by this time, all uploaded data for this session will be discarded.
        /// </value>
        [DataMember(Name = "uploadExpiration", EmitDefaultValue = false)]
        public string UploadExpiration { get; set; }
    }

}
