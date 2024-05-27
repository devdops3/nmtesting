using System.Configuration;

/// <summary>
/// Summary description for Logger
/// </summary>
public class Logger
{
	public Logger()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public static void writeLog(string msg, ref log4net.ILog log)
    {
        //try
        //{
        //    if (ConfigurationManager.AppSettings["isFileLog"].ToString().Equals("Y"))
        //        log.Info(msg);
        //}
        //catch (Exception ex)
        //{
        //    log.Info(ex.Message);
        //}

        if (!log4net.LogManager.GetRepository().Configured)
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        if (ConfigurationManager.AppSettings["isFileLog"].ToString() == "Y")
        {
            log.Info(msg);
        }
    }
}