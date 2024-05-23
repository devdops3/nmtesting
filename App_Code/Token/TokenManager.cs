using System;
using System.Configuration;

/// <summary>
/// Summary description for TokenManager
/// </summary>
public class TokenManager
{
	public TokenManager()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    #region Token
    public static OAuthToken Token = new OAuthToken();
    public class OAuthToken
    {
        public string Token { get; set; }
        public DateTime GeneratedTime { get; set; }
    }

    public static bool IsTokenNullOrExpire(OAuthToken oAuthToken)
    {
        if (string.IsNullOrEmpty(oAuthToken.Token))
        {
            return true;
        }
        else
        {
            TimeSpan diff = DateTime.Now.Subtract(Token.GeneratedTime);
            string tokenTimeOut = ConfigurationManager.AppSettings["TokenTimeout"].ToString();
            int tokentimeout = int.Parse(tokenTimeOut);
            if (diff.Minutes >= tokentimeout)
            {
                return true;
            }
        }
        return false;
    }

    public static OAuthToken GetOAuthToken()
    {
        OAuthToken token = new OAuthToken();
        try
        {

            token.Token = Utils.oAuthRequest();
            token.GeneratedTime = DateTime.Now;
            Token = token;

        }
        catch 
        {
            token.Token = Utils.oAuthRequest();
            token.GeneratedTime = DateTime.Now;
            Token = token;
        }
        return token;
    }
    #endregion
}