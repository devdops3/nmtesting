using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

/// <summary>
/// Summary description for CacheService
/// </summary>
public static class CacheService
{
    private static readonly string _host;
    private static readonly int _port;
    private static readonly int _bufferSeconds;

	static CacheService()
	{
        _host = ConfigurationManager.AppSettings["RedisHost"].ToString();
        _port = int.Parse(ConfigurationManager.AppSettings["RedisPort"].ToString());
        _bufferSeconds = int.Parse(ConfigurationManager.AppSettings["RedisBufferSeconds"].ToString());
    }

    public static string GetData(string key)
    {
        try
        {
            using (RedisClient redisClient = new RedisClient(_host, _port))
            {
                return redisClient.Get<string>(key);
            }
        } catch(Exception){
            return null;
        }
    }

    public static bool SaveData(string key, string value, int apiExpireSeconds)
    {
        try
        {
            int cacheExpireSeconds = (_bufferSeconds >= apiExpireSeconds) ? apiExpireSeconds : apiExpireSeconds - _bufferSeconds;

            using (RedisClient redisClient = new RedisClient(_host, _port))
            {
                if (redisClient.Get<string>(key) != null) redisClient.Remove(key);
                return redisClient.Set(key, value, TimeSpan.FromSeconds(cacheExpireSeconds));
            }
        }
        catch (Exception)
        {
            return false;
        }   
    }
}