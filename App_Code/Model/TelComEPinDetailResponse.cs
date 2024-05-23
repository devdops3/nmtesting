/// <summary>
/// Summary description for TelComEPinDetailResponse
/// </summary>
public class TelComEPinDetailResponse
{
	public TelComEPinDetailResponse()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public string ProductCode { get; set; }
    public string PinId { get; set; }
    public string SerialNumber { get; set; }
    public string ClearPin { get; set; }
    public string ExpiryDate { get; set; }
    public string ProductAmount { get; set; }
    public string ProductDescription { get; set; }
}