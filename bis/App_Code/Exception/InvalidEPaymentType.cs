using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for InvalidEPaymentType
/// </summary>
[Serializable]
public class InvalidEPaymentType : Exception
{
    public InvalidEPaymentType(){}
    public InvalidEPaymentType(string message) :base (message){ }
    public InvalidEPaymentType(string message, Exception innerException) : base(message, innerException) { }
}