/// <summary>
/// Summary description for S3Model
/// </summary>
public class S3Model
{
	public S3Model()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public class S3FileUploadRequest
    {
        public string RefId { get; set; }
        public string UserId { get; set; }
        public string SelfiePhotoByByteArray { get; set; }
        public string PhotoWithIdCardByByteArray { get; set; }
    }

    public class S3UploadResponse
    {
        public string RefId { get; set; }

        public string UserId { get; set; }

        public string SelfiePhotoUrl { get; set; }

        public string PhotoWithIdCardUrl { get; set; }

        public string ResCode { get; set; }

        public string ResDescription { get; set; }
    }
}