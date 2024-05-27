using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using log4net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Summary description for GETIntegrationService
/// </summary>
public class GETIntegrationService
{
    private readonly string DelayToRetry = ConfigurationManager.AppSettings["GETDelayToRetry"].ToString();
    public GETIntegrationService()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    #region Authentication
    public AuthenticationModel GetToken()
    {
        AuthenticationModel authenticationModel = new AuthenticationModel();

        Utils.WriteLog_Biller("$$$$$$$$$$$$$$This is authentication data request at GETIntegration$$$$$$$$$$$$$$$");

        // Get authentication data from Cache
        string authenticationData = CacheService.GetData(CacheKeyConsts.GETAuthKey);
        if (!string.IsNullOrWhiteSpace(authenticationData))
        {
            authenticationModel = JsonConvert.DeserializeObject<AuthenticationModel>(authenticationData);
            Utils.WriteLog_Biller("GETAuthentication Data : " + authenticationData);
            return authenticationModel;
        }

        // Get authentication from third party direct call
        authenticationModel = Authenticate();
        if (authenticationModel == null || authenticationModel.Status.ToUpper() != "SUCCESS") return null;

        string authenticationModelString = JsonConvert.SerializeObject(authenticationModel);
        bool result = CacheService.SaveData(CacheKeyConsts.GETAuthKey, authenticationModelString, authenticationModel.ExpireIn);
        Utils.WriteLog_Biller("Result GETAuthentication Data saved to cache : " + result);
        return authenticationModel;
    }
    #endregion

    #region ELoad
    public ELoadResponse ProcessForELoad(string authToken, string authId, ELoadRequest eloadRequest)
    {
        var result = string.Empty;
        ELoadResponse responseData = new ELoadResponse();
        var logAppender = "ProcessELoadAtGETIntegrationService | " + eloadRequest.OrderId + " | ";

        try
        {
            Utils.WriteLog_Biller(logAppender + "$$$$$$$$$$$$$$ELoad Process started at GETIntegration$$$$$$$$$$$$$$$");
            string confirmRequestTimeout = ConfigurationManager.AppSettings["GETConfirmRequestTimeOut"].ToString();
            string eLoadUrl = ConfigurationManager.AppSettings["GETEloadUrl"].ToString();
            HttpWebRequest httpWebRequest = PrepareHttpWebRequest(eLoadUrl, authToken, authId, confirmRequestTimeout);

            string postStr = String.Format("phoneNumber={0}&orderId={1}&amount={2}", eloadRequest.PhoneNumber, eloadRequest.OrderId, eloadRequest.Amount);

            StreamWriter postWriter = new StreamWriter(httpWebRequest.GetRequestStream());
            postWriter.Write(postStr);
            postWriter.Close();

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }

            Utils.WriteLog_Biller(logAppender + "$$$$$$$$$ Received ELoad response from Integration :" + result);

            if (result == null) throw new Exception("No Data from GET Integration for OrderId : " + eloadRequest.OrderId);

            ELoadIntegrationResponse integrationResponse = JsonConvert.DeserializeObject<ELoadIntegrationResponse>(result);

            if (integrationResponse.Status.ToUpper() == "PENDING")
            {
                Utils.WriteLog_Biller(logAppender + "Pending status from Integration response:");
                string DelayToRetry = ConfigurationManager.AppSettings["GETDelayToRetry"].ToString();
                Thread.Sleep(int.Parse(DelayToRetry));
                return CheckELoadOrderStatus(eloadRequest.OrderId);
            }

            // Populate from Integration Model to Internal Model
            responseData = PopulateELoadResponseModel(integrationResponse);
            return responseData;
        }
        catch (WebException ex)
        {
            Utils.WriteLog_Biller(logAppender + "Web Exception :" + ex.Message);
            if ((ex.Response as HttpWebResponse).StatusCode == HttpStatusCode.GatewayTimeout)
            {
                return CheckELoadGatewayTimeOutstatus(eloadRequest.OrderId);
            }
            return GetELoadResponse(eloadRequest.OrderId, TelCoStatusEnum.Failed.ToString(), ex.Message);
        }
        catch (Exception ex)
        {
            Utils.WriteLog_Biller(logAppender + "Exception occur when request for ELoad at GET Integration:" + ex.Message);
            return GetELoadResponse(eloadRequest.OrderId, TelCoStatusEnum.Failed.ToString(), ex.Message);
        }
    }
    #endregion

    #region EPin
    public EPinResponse ProcessForEPin(string authToken, string authId, EPinRequest epinRequest)
    {
        var result = string.Empty;
        EPinResponse responseData = new EPinResponse();
        var logAppender = "ProcessEPinAtGETIntegrationService | " + epinRequest.OrderId + " | ";

        try
        {
            Utils.WriteLog_Biller(logAppender + "$$$$$$$$$$$$$$EPin Process started at GETIntegration$$$$$$$$$$$$$$$");
            string confirmRequestTimeout = ConfigurationManager.AppSettings["GETConfirmRequestTimeOut"].ToString();
            string epinUrl = ConfigurationManager.AppSettings["GETEpinUrl"].ToString();
            HttpWebRequest httpWebRequest = PrepareHttpWebRequest(epinUrl, authToken, authId, confirmRequestTimeout);

            string postStr = String.Format("productCode={0}&orderId={1}", epinRequest.ProductCode, epinRequest.OrderId);

            StreamWriter postWriter = new StreamWriter(httpWebRequest.GetRequestStream());
            postWriter.Write(postStr);
            postWriter.Close();

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }

            Utils.WriteLog_Biller(logAppender + "$$$$$$$$$ Received EPIN response from Integration :" + result);

            if (result == null) throw new Exception("No Data from GET Integration for OrderId : " + epinRequest.OrderId);

            EPinIntegrationResponse integrationResponse = JsonConvert.DeserializeObject<EPinIntegrationResponse>(result);

            if (integrationResponse.Status.ToUpper() == "PENDING")
            {
                Utils.WriteLog_Biller(logAppender + "Pending status from Integration response:");
                string DelayToRetry = ConfigurationManager.AppSettings["GETDelayToRetry"].ToString();
                Thread.Sleep(int.Parse(DelayToRetry));
                return CheckEPinOrderStatus(epinRequest.OrderId);
            }

            // Populate from Integration Model to Internal Model
            responseData = PopulateEPinResponseModel(integrationResponse);
            return responseData;
        }
        catch (WebException ex)
        {
            Utils.WriteLog_Biller(logAppender + "Web Exception :" + ex.Message);
            if ((ex.Response as HttpWebResponse).StatusCode == HttpStatusCode.GatewayTimeout)
            {
                return CheckEPinGatewayTimeOutstatus(epinRequest.OrderId);
            }
            return GetEPinResponse(epinRequest.OrderId, TelCoStatusEnum.Failed.ToString(), ex.Message);

        }
        catch (Exception ex)
        {
            Utils.WriteLog_Biller(logAppender + "Exception occur when request for EPIN at GET Integration:" + ex.Message);
            return GetEPinResponse(epinRequest.OrderId, TelCoStatusEnum.Failed.ToString(), ex.Message);
        }
    }
    #endregion

    #region DataPackage
    public DataPackResponse ProcessForDataPackage(string authToken, string authId, DataPackRequest dataPackRequest)
    {
        var result = string.Empty;
        DataPackResponse responseData = new DataPackResponse();
        var logAppender = "ProcessDataPackageAtGETIntegrationService | " + dataPackRequest.OrderId + " | ";

        try
        {
            Utils.WriteLog_Biller(logAppender + "$$$$$$$$$$$$$$DataPackage Process started at GETIntegration$$$$$$$$$$$$$$$");
            string confirmRequestTimeout = ConfigurationManager.AppSettings["GETConfirmRequestTimeOut"].ToString();
            string dataPackUrl = ConfigurationManager.AppSettings["GETEpackageUrl"].ToString();
            HttpWebRequest httpWebRequest = PrepareHttpWebRequest(dataPackUrl, authToken, authId, confirmRequestTimeout);

            string postStr = String.Format("packageCode={0}&orderId={1}&phoneNumber={2}", dataPackRequest.PackageCode, dataPackRequest.OrderId, dataPackRequest.PhoneNumber);

            StreamWriter postWriter = new StreamWriter(httpWebRequest.GetRequestStream());
            postWriter.Write(postStr);
            postWriter.Close();

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }

            Utils.WriteLog_Biller(logAppender + "$$$$$$$$$ Received DataPackage response from Integration :" + result);

            if (result == null) throw new Exception("No Data from GET Integration for OrderId : " + dataPackRequest.OrderId);

            DataPackIntegrationResponse integrationResponse = JsonConvert.DeserializeObject<DataPackIntegrationResponse>(result);

            if (integrationResponse.Status.ToUpper() == "PENDING")
            {
                Utils.WriteLog_Biller(logAppender + "Pending status from Integration response:");
                string DelayToRetry = ConfigurationManager.AppSettings["GETDelayToRetry"].ToString();
                Thread.Sleep(int.Parse(DelayToRetry));
                return CheckDataPackageOrderStatus(dataPackRequest.OrderId);
            }

            // Populate from Integration Model to Internal Model
            responseData = PopulateDataPackageResponseModel(integrationResponse);
            return responseData;
        }
        catch (WebException ex)
        {
            Utils.WriteLog_Biller(logAppender + "Web Exception :" + ex.Message);
            if ((ex.Response as HttpWebResponse).StatusCode == HttpStatusCode.GatewayTimeout)
            {
                return CheckDataPackageOrderStatus(dataPackRequest.OrderId);
            }
            return GetDataPackResponse(dataPackRequest.OrderId, TelCoStatusEnum.Failed.ToString(), ex.Message);
        }
        catch (Exception ex)
        {
            Utils.WriteLog_Biller(logAppender + "Exception occur when request for EPIN at GET Integration:" + ex.Message);
            return GetDataPackResponse(dataPackRequest.OrderId, TelCoStatusEnum.Failed.ToString(), ex.Message);
        }
    }

    public List<DataPackDetail> RequestDataPackageList(string authToken, string authId)
    {
        var result = string.Empty;
        List<DataPackDetail> dataPackResponse = new List<DataPackDetail>();
        var logAppender = "RequestDataPackageListAtGETIntegrationService | ";
        try
        {
            string dataPackageListUrl = ConfigurationManager.AppSettings["GETDataPackageListUrl"].ToString();
            string requestTimeOut = ConfigurationManager.AppSettings["GETInquiryRequestTimeOut"].ToString();
            HttpWebRequest httpWebRequest = PrepareHttpWebRequest(dataPackageListUrl, authToken, authId, requestTimeOut);

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }

            Utils.WriteLog_Biller(logAppender + "$$$$$$$$$ Received DataPackageList response from Integration :" + result.Trim());

            if (result == null) throw new Exception("No Data from GET Integration");

            DataPackInquiryIntegrationResponse integrationResponse = JsonConvert.DeserializeObject<DataPackInquiryIntegrationResponse>(result);

            dataPackResponse = PopulateDataPackDetailModel(integrationResponse);
            return dataPackResponse;
        }
        catch (Exception ex)
        {
            Utils.WriteLog_Biller(logAppender + "Exception occur when request for DataPack at GET Integration:" + ex.Message);
            return dataPackResponse;
        }
    }
    #endregion

    #region Helper Methods
    private AuthenticationModel Authenticate()
    {
        AuthenticationIntegrationResponse integrationResponse = new AuthenticationIntegrationResponse();
        AuthenticationModel authenticationModel = new AuthenticationModel();
        Utils.WriteLog_Biller("$$$$$$$$$$$$$$This is authentication request for GET provider$$$$$$$$$$$$$$$$$$$");

        string clientId = ConfigurationManager.AppSettings["GETClientId"].ToString();
        string clientSecret = ConfigurationManager.AppSettings["GETClientSecret"].ToString();
        string authURL = ConfigurationManager.AppSettings["GETAuthUrl"].ToString();
        string grantType = ConfigurationManager.AppSettings["GETGrantType"].ToString();
        string requestTimeout = ConfigurationManager.AppSettings["GETAuthRequestTimeOut"].ToString();

        HttpWebRequest httpWebRequest = (HttpWebRequest)(WebRequest.Create(authURL));
        httpWebRequest.Method = WebRequestMethods.Http.Post;
        httpWebRequest.ContentType = "application/x-www-form-urlencoded";
        httpWebRequest.Timeout = int.Parse(requestTimeout);

        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

        string postStr = String.Format("clientId={0}&clientSecret={1}&grantType={2}", clientId, clientSecret, grantType);

        StreamWriter postWriter = new StreamWriter(httpWebRequest.GetRequestStream());
        postWriter.Write(postStr);
        postWriter.Close();

        try
        {
            Utils.WriteLog_Biller("$$$$$$$$$ Call authentication from GET Integration $$$$$$$$$");

            HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            StreamReader sr = new StreamReader(httpResponse.GetResponseStream());
            string rawOutput = sr.ReadToEnd();
            sr.Close();

            Utils.WriteLog_Biller("$$$$$$$$$ Received authentication response from GET Integration :" + rawOutput);

            if (rawOutput == null)
            {
                throw new NullReferenceException("Authentication response is null for GETIntegration");
            }

            integrationResponse = JsonConvert.DeserializeObject<AuthenticationIntegrationResponse>(rawOutput);
            if (integrationResponse.Status != "success")
            {
                throw new Exception("Authentication Failed");
            }

            integrationResponse.Status = TelCoStatusEnum.Success.ToString();
            authenticationModel = PopulateAuthenticationModel(integrationResponse);
        }
        catch (Exception ex)
        {
            Utils.WriteLog_Biller("Exception occur when request to authenticate:" + ex.Message);
            return null;
        }

        return authenticationModel;
    }

    private HttpWebRequest PrepareHttpWebRequest(string url, string authtoken, string authId, string timeOut)
    {
        HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
        httpWebRequest.Headers.Add("x-auth-token", authtoken);
        httpWebRequest.Headers.Add("x-secret-id", authId);
        httpWebRequest.ContentType = "application/x-www-form-urlencoded";
        httpWebRequest.Method = WebRequestMethods.Http.Post;
        httpWebRequest.Timeout = int.Parse(timeOut);
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        return httpWebRequest;
    }

    private string GetOrderStatus(string orderId, string enquiryType)
    {
        var result = string.Empty;
        var logAppender = orderId + " | " + enquiryType + " | ";

        Utils.WriteLog_Biller(logAppender + "$$$$$$$$$$$$$ Order Process started at GETIntegration$$$$$$$$$$$$$$$");
        AuthenticationModel authModel = GetToken();
        string enquiryUrl = ConfigurationManager.AppSettings["GETEnquiryUrl"].ToString();
        string enquiryRequestTimeOut = ConfigurationManager.AppSettings["GETEnquiryRequestTimeOut"].ToString();
        HttpWebRequest httpWebRequest = PrepareHttpWebRequest(enquiryUrl, authModel.Authtoken, authModel.AuthId, enquiryRequestTimeOut);

        string postStr = String.Format("orderId={0}&enquiry_type={1}", orderId, enquiryType);

        StreamWriter postWriter = new StreamWriter(httpWebRequest.GetRequestStream());
        postWriter.Write(postStr);
        postWriter.Close();

        var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
        {
            result = streamReader.ReadToEnd();
        }

        Utils.WriteLog_Biller(logAppender + "$$$$$$$$$ Received OrderDetail response from GET Integration :" + result);

        if (result == null) throw new Exception("No Data from GET Integration for OrderId : " + orderId);
        return result;

    }

    private ELoadResponse CheckELoadGatewayTimeOutstatus(string orderId)
    {
        var gatewayTimeoutWait = Convert.ToInt32(ConfigurationManager.AppSettings["GatewayTimeOutWait"].ToString());
        var gatewayTimeoutRetry = Convert.ToInt32(ConfigurationManager.AppSettings["GatewayTimeOutRetry"].ToString());
        var retryCount = 1;
        do
        {
            Thread.Sleep(gatewayTimeoutWait);
            var result = GetOrderStatus(orderId, "eload");
            var eLoadOrderDetail = JsonConvert.DeserializeObject<ELoadOrderDetailIntegrationResponse>(result);
            if (eLoadOrderDetail.StatusCode.ToUpper() != GETEnquiryStatus.success) return GetELoadResponse(eLoadOrderDetail.OrderId, TelCoStatusEnum.Failed.ToString(), eLoadOrderDetail.Reason);
            if (eLoadOrderDetail.Order_status.ToUpper() == GETOrderStatus.failed) return GetELoadResponse(eLoadOrderDetail.OrderId, TelCoStatusEnum.Failed.ToString(), eLoadOrderDetail.Reason);
            if (eLoadOrderDetail.Order_status.ToUpper() == GETOrderStatus.success) return GetELoadResponse(eLoadOrderDetail.OrderId, TelCoStatusEnum.Success.ToString(), null);

            retryCount++;
        } while (retryCount <= gatewayTimeoutRetry);

        return GetELoadResponse(orderId, TelCoStatusEnum.Failed.ToString(), "TimeOut");
    }

    private EPinResponse CheckEPinGatewayTimeOutstatus(string orderId)
    {
        var gatewayTimeoutWait = Convert.ToInt32(ConfigurationManager.AppSettings["GatewayTimeOutWait"].ToString());
        var gatewayTimeoutRetry = Convert.ToInt32(ConfigurationManager.AppSettings["GatewayTimeOutRetry"].ToString());
        var retryCount = 1;
        do
        {
            Thread.Sleep(gatewayTimeoutWait);
            var result = GetOrderStatus(orderId, "epin");
            var ePinOrderDetail = JsonConvert.DeserializeObject<EPinOrderDetailIntegrationResponse>(result);
            if (ePinOrderDetail.StatusCode.ToUpper() != GETEnquiryStatus.success) return GetEPinResponse(ePinOrderDetail.OrderId, TelCoStatusEnum.Failed.ToString(), ePinOrderDetail.Reason);
            if (ePinOrderDetail.Order_status.ToUpper() == GETOrderStatus.failed) return GetEPinResponse(ePinOrderDetail.OrderId, TelCoStatusEnum.Failed.ToString(), ePinOrderDetail.Reason);
            if (ePinOrderDetail.Order_status.ToUpper() == GETOrderStatus.success)
            {
                var epinObject = new EPinIntegrationResponse
            {
                Order_number = ePinOrderDetail.OrderId,
                Status = ePinOrderDetail.StatusCode,
                Expiry_date = ePinOrderDetail.Expiry_date,
                Pin = ePinOrderDetail.Pin,
                Serial_number = ePinOrderDetail.Serial_number
            };
                var ePinResponse = PopulateEPinResponseModel(epinObject);
                return ePinResponse;
            }

            retryCount++;
        } while (retryCount <= gatewayTimeoutRetry);

        return GetEPinResponse(orderId, TelCoStatusEnum.Failed.ToString(), "TimeOut");
    }

    private DataPackResponse CheckDataPackGatewayTimeOutstatus(string orderId)
    {
        var gatewayTimeoutWait = Convert.ToInt32(ConfigurationManager.AppSettings["GatewayTimeOutWait"].ToString());
        var gatewayTimeoutRetry = Convert.ToInt32(ConfigurationManager.AppSettings["GatewayTimeOutRetry"].ToString());
        var retryCount = 1;
        do
        {
            Thread.Sleep(gatewayTimeoutWait);
            var result = GetOrderStatus(orderId, "epackage");
            var dataPackOrderDetail = JsonConvert.DeserializeObject<DataPackOrderDetailIntegrationResponse>(result);
            if (dataPackOrderDetail.StatusCode.ToUpper() != GETEnquiryStatus.success) return GetDataPackResponse(orderId, TelCoStatusEnum.Failed.ToString(), dataPackOrderDetail.Reason);
            if (dataPackOrderDetail.Order_status.ToUpper() == GETOrderStatus.failed) return GetDataPackResponse(dataPackOrderDetail.OrderId, TelCoStatusEnum.Failed.ToString(), dataPackOrderDetail.Reason);
            if (dataPackOrderDetail.Order_status.ToUpper() == GETOrderStatus.success) return GetDataPackResponse(dataPackOrderDetail.OrderId, TelCoStatusEnum.Success.ToString(), null);
            retryCount++;
        } while (retryCount <= gatewayTimeoutRetry);

        return GetDataPackResponse(orderId, TelCoStatusEnum.Failed.ToString(), "TimeOut");
    }

    private ELoadResponse CheckELoadOrderStatus(string orderId)
    {
        var logAppender = "CheckELoadOrderStatusAtGETIntegrationService | " + orderId + " | ";
        ELoadResponse eLoadResponse = new ELoadResponse();
        try
        {
            var result = GetOrderStatus(orderId, "eload");
            var eLoadOrderDetail = JsonConvert.DeserializeObject<ELoadOrderDetailIntegrationResponse>(result);

            // Check order exist or not , order status is succes or not
            if (eLoadOrderDetail.StatusCode.ToUpper() == GETEnquiryStatus.success && eLoadOrderDetail.Order_status.ToUpper() == GETOrderStatus.success)
            {
                return GetELoadResponse(orderId, TelCoStatusEnum.Success.ToString(), null);
            }
            else
            {
                return GetELoadResponse(orderId, TelCoStatusEnum.Failed.ToString(), eLoadOrderDetail.Reason);
            }

        }
        catch (Exception ex)
        {
            Utils.WriteLog_Biller(logAppender + "Exception occur when checking the order status at GET Integration:" + ex.Message);
            return GetELoadResponse(orderId, TelCoStatusEnum.Failed.ToString(), ex.Message);
        }
    }

    private ELoadResponse GetELoadResponse(string orderId, string status, string reason)
    {
        var eLoadResponse = new ELoadResponse()
        {
            OrderNumber = orderId,
            Status = status,
            Description = reason
        };
        return eLoadResponse;
    }

    private EPinResponse GetEPinResponse(string orderId, string status, string reason)
    {
        var ePinResponse = new EPinResponse()
        {
            OrderNumber = orderId,
            Status = status,
            Description = reason
        };
        return ePinResponse;
    }

    private DataPackResponse GetDataPackResponse(string orderId, string status, string reason)
    {
        var dataPackResponse = new DataPackResponse()
        {
            OrderNumber = orderId,
            Status = status,
            Description = reason
        };
        return dataPackResponse;
    }

    private EPinResponse CheckEPinOrderStatus(string orderId)
    {
        EPinResponse ePinResponse = new EPinResponse();
        var logAppender = "CheckEPinOrderStatusAtGETIntegrationService | " + orderId + " | ";

        try
        {
            var result = GetOrderStatus(orderId, "epin");
            EPinOrderDetailIntegrationResponse ePinOrderDetail = JsonConvert.DeserializeObject<EPinOrderDetailIntegrationResponse>(result);

            // Check order exist or not
            if (ePinOrderDetail.StatusCode.ToUpper() == GETEnquiryStatus.success && ePinOrderDetail.Order_status.ToUpper() == GETOrderStatus.success)
            {
                EPinIntegrationResponse ePinIntegrationResponse = new EPinIntegrationResponse()
                {
                    Order_number = orderId,
                    Status = ePinOrderDetail.StatusCode,
                    Expiry_date = ePinOrderDetail.Expiry_date,
                    Pin = ePinOrderDetail.Pin,
                    Serial_number = ePinOrderDetail.Serial_number
                };
                return PopulateEPinResponseModel(ePinIntegrationResponse);
            }
            else
            {
                return GetEPinResponse(orderId, TelCoStatusEnum.Failed.ToString(), ePinOrderDetail.Reason);
            }
        }
        catch (Exception ex)
        {
            Utils.WriteLog_Biller(logAppender + "Exception occur when checking the order status at GET Integration:" + ex.Message);
            ePinResponse = new EPinResponse()
            {
                Status = TelCoStatusEnum.Failed.ToString(),
                Description = ex.Message
            };
            return ePinResponse;
        }
    }

    private DataPackResponse CheckDataPackageOrderStatus(string orderId)
    {
        DataPackResponse dataPackResponse = new DataPackResponse();
        var logAppender = "CheckDataPackageOrderStatusAtGETIntegrationService | " + orderId + " | ";

        try
        {
            var result = GetOrderStatus(orderId, "epackage");
            DataPackOrderDetailIntegrationResponse dataPackOrderDetail = JsonConvert.DeserializeObject<DataPackOrderDetailIntegrationResponse>(result);

            // Check order exist or not
            if (dataPackOrderDetail.StatusCode.ToUpper() == GETEnquiryStatus.success && dataPackOrderDetail.Order_status.ToUpper() == GETOrderStatus.success)
            {
                return GetDataPackResponse(orderId, TelCoStatusEnum.Success.ToString(), null);
            }
            else
            {
                return GetDataPackResponse(orderId, TelCoStatusEnum.Failed.ToString(), dataPackOrderDetail.Reason);
            }
        }
        catch (Exception ex)
        {
            Utils.WriteLog_Biller(logAppender + "Exception occur when checking the order status at GET Integration:" + ex.Message);
            return GetDataPackResponse(orderId, TelCoStatusEnum.Failed.ToString(), ex.Message);
        }
    }

    private AuthenticationModel PopulateAuthenticationModel(AuthenticationIntegrationResponse authResponse)
    {
        AuthenticationModel authenticationModel = new AuthenticationModel();
        if (authResponse != null)
        {
            authenticationModel.AuthId = authResponse.AuthId;
            authenticationModel.Authtoken = authResponse.Authtoken;
            authenticationModel.ExpireIn = authResponse.Expire_in;
            authenticationModel.Status = authResponse.Status;
            authenticationModel.GeneratedTime = DateTime.Now;
        }
        return authenticationModel;
    }

    private ELoadResponse PopulateELoadResponseModel(ELoadIntegrationResponse eLoadResponse)
    {
        ELoadResponse eLoadRes = new ELoadResponse();
        if (eLoadResponse != null)
        {
            eLoadRes.OrderNumber = eLoadResponse.Invoice_number;
            eLoadRes.Description = eLoadResponse.Reason;
            eLoadRes.Status = (eLoadResponse.Status.ToUpper() == "SUCCESS" ? TelCoStatusEnum.Success.ToString() : TelCoStatusEnum.Failed.ToString());
        }
        return eLoadRes;
    }

    private EPinResponse PopulateEPinResponseModel(EPinIntegrationResponse ePinIntegrationResponse)
    {
        EPinResponse ePinResponse = new EPinResponse();
        if (ePinIntegrationResponse != null)
        {
            ePinResponse.OrderNumber = ePinIntegrationResponse.Order_number;
            ePinResponse.Description = ePinIntegrationResponse.Reason;
            ePinResponse.Status = TelCoStatusEnum.Failed.ToString();

            if (ePinIntegrationResponse.Status.ToUpper() == "SUCCESS")
            {
                DateTime tempDate = DateTime.ParseExact(ePinIntegrationResponse.Expiry_date, "yyyy-mm-dd", null);
                ePinResponse.ExpiryDate = tempDate.ToString("dd/MM/yyyy");
                ePinResponse.Pin = ePinIntegrationResponse.Pin;
                ePinResponse.SerialNumber = ePinIntegrationResponse.Serial_number;
                ePinResponse.Status = TelCoStatusEnum.Success.ToString();
            }
        }
        return ePinResponse;
    }

    private List<DataPackDetail> PopulateDataPackDetailModel(DataPackInquiryIntegrationResponse datapackIntegrationResponse)
    {
        List<DataPackDetail> result = new List<DataPackDetail>();
        if (datapackIntegrationResponse == null || datapackIntegrationResponse.Packages_list == null || datapackIntegrationResponse.Packages_list.Count == 0)
            return result;

        foreach (DataPackageDetailIntegrationResponse dataPack in datapackIntegrationResponse.Packages_list)
        {
            DataPackDetail packageDetail = new DataPackDetail();
            packageDetail.Code = dataPack.Code;
            packageDetail.Name = dataPack.Name;
            packageDetail.OperatorName = dataPack.Operator_name;
            packageDetail.PackageType = dataPack.Package_type;
            packageDetail.Price = dataPack.Price;
            packageDetail.Validity = dataPack.Validity;

            result.Add(packageDetail);
        }
        return result;
    }

    private DataPackResponse PopulateDataPackageResponseModel(DataPackIntegrationResponse dataPackResponse)
    {
        DataPackResponse dataPackRes = new DataPackResponse();
        if (dataPackResponse != null)
        {
            dataPackRes.OrderNumber = dataPackResponse.Invoice_number;
            dataPackRes.Description = dataPackResponse.Reason;
            dataPackRes.Status = (dataPackResponse.Status.ToUpper() == "SUCCESS" ? TelCoStatusEnum.Success.ToString() : TelCoStatusEnum.Failed.ToString());
        }
        return dataPackRes;
    }
    #endregion
}