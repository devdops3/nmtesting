using Polly;
using System;

/// <summary>
/// Summary description for RetryService
/// </summary>
public class RetryService<TException> where TException : Exception
{
    public RetryService()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public Policy<TResult> CreatePolicy<TResult>(int retryCount, int sleepInSecond, TResult failCondition,string logAppender) 
    {
        var retryPolicy = Policy.HandleResult(failCondition)
                                .Or<TException>()
                                .WaitAndRetry(retryCount: retryCount, sleepDurationProvider: (attemptCount) => TimeSpan.FromSeconds(sleepInSecond),
                                onRetry: (delegateResult, sleepDuration, attemptNumber, context) =>
                                {
                                    Utils.WriteLog_Biller(logAppender + "Retry | attempt : " + attemptNumber + " | Returned Result : " + delegateResult.Result.ToString());
                                });
        return retryPolicy;
    }
}