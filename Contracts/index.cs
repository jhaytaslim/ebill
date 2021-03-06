using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ebill.Contracts;

public class MailVM
    {
        [EmailAddress, Required]
        public string Recipient { get; set; }

        [Required]
        public string Title { get; set; }

        [DataType(DataType.MultilineText), Required]
        public string Message { get; set; }

        public List<string> Copy { get; set; }
    }

public class Customer
{
    public string CustomerName { get; set; }
    public string Mobile { get; set; }
    public string Email { get; set; }
}

public class Params
{
    public string CustomerName { get; set; }
    public string Mobile { get; set; }
    public string? Email { get; set; }
    public string? AccountNumber { get; set; }
}

public class ValidationRequest
{
    public string SourceBankCode { get; set; }
    public string SourceBankName { get; set; }
    public string InstitutionCode { get; set; }
    public string ChannelCode { get; set; }
    public string BillerId { get; set; }
    public string BillerName { get; set; }
    public string ProductID { get; set; }
    public string ProductName { get; set; }
    public double Amount { get; set; }
    public Params Params { get; set; }
}


    public class ValidationResponse
    {
        public string Message { get; set; }
        public int Amount { get; set; }
        public bool HasError { get; set; }
        public Params Params { get; set; }
        public List<string> ErrorMessages { get; set; }
    }



public class Item
{
    public string Key { get; set; }
    public string Value { get; set; }
}

public class ValidationRequestOld
{
    public string SourceBankCode { get; set; }
    public string SourceBankName { get; set; }
    public string InstitutionCode { get; set; }
    public string ChannelCode { get; set; }
    public string CustomerName { get; set; }
    public string CustomerAccountNumber { get; set; }
    public string BillerID { get; set; }
    public string BillerName { get; set; }
    public string ProductID { get; set; }
    public string ProductName { get; set; }
    public string Amount { get; set; }
    public List<Item> Param { get; set; }
}

public class ValidationResponseOld
{
    public string Message { get; set; }
    public int Amount { get; set; }
    public bool HasError { get; set; }
    public List<Item> Param { get; set; }
    public List<object> ErrorMessages { get; set; }
}

public class NotificationRequest
{
    public string SourceBankCode { get; set; }
    public string SourceBankName { get; set; }
    public string InstitutionCode { get; set; }
    public string ChannelCode { get; set; }
    public string CustomerName { get; set; }
    public string CustomerAccountNumber { get; set; }
    public string BillerID { get; set; }
    public string BillerName { get; set; }
    public string ProductID { get; set; }
    public string ProductName { get; set; }
    public float Amount { get; set; }
    public List<Item> Param { get; set; }
}

public class NotificationResponse
{
    public string Message { get; set; }
    public bool HasError { get; set; }
    public float Amount { get; set; }
    public List<object> ErrorMessages { get; set; }
}

public class Settings
{
    public string Url { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}

public class HmacObject
{
    public object request { get; set; }
    public string iv { get; set; }
    public string key { get; set; }
}

public class Connections
{
     public string obj { get; set; }
    public string obj2 { get; set; }
    public string idl2 { get; set; }
    public string idl { get; set; }
    public string DefaultConnection { get; set; }
}
