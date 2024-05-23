/// <summary>
/// Summary description for FaceRecognitionModel
/// </summary>
public class FaceRecognitionModel
{
	public FaceRecognitionModel()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public class FaceRecognitionRequest
    {
        public string RefId { get; set; }
        public int Expected_Similarity { get; set; }
        public string Source_Image { get; set; }
        public string Target_Image { get; set; }
        public string Note { get; set; }
    }

    public class FaceRecognitionResponse
    {
        public string RefId { get; set; }
        public string Id { get; set; }

        public decimal Expected_Similarity { get; set; }

        public decimal Actual_Similarity { get; set; }

        public string Response_Code { get; set; }

        public string Response_Description { get; set; }
    }
}