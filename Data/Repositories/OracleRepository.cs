

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ebill.Data.Repository.Interface;
using ebill.Data.Models;
using ebill.Data;
using ebill.Contracts;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace ebill.Data.Repository
{
    public class OracleRepository : IOracleRepository //Repository<Product>, IProductRepository
    {
        private string strOBJ2;
        private string oradb2;
        private string oradb;
        private Connections _connections;
        private  ILogger<dynamic> _log;
        public OracleRepository(Connections connections, ILogger<dynamic> log)
        {
            _connections = connections;
             _log = log;
            strOBJ2 = connections.obj2; // ConfigurationManager.AppSettings["obj2"].ToString();
            oradb2 = connections.idl2;  //ConfigurationManager.ConnectionStrings["idl2"].ToString();
            oradb = connections.idl;
            Console.WriteLine("oracle..." + strOBJ2);
            Console.WriteLine("oracle2..." + oradb2);
        }

        public ValidationResponse Validation(ValidationRequest model)
        {
            ValidationResponse resp = new ValidationResponse();
            try
            {
                string msg;
                Console.WriteLine("here...");
                string cid = "";

                if (!string.IsNullOrEmpty(model.Params.AccountNumber))
                {
                    cid = model.Params.AccountNumber;
                }

                string CustomerName;
                string Mobile;
                string Email;

                // connect to database and retrieve customer information.
                // string strOBJ2 = ConfigurationManager.AppSettings["obj2"].ToString();
                // string oradb2 = ConfigurationManager.ConnectionStrings["idl2"].ToString();
                

                try
                {
                    string[] result = { "", "", "" };
                    //string oradb = ConfigurationManager.ConnectionStrings["idl"].ToString();
                    string sql1 = "";
                    using (SqlConnection conn = new SqlConnection(oradb))
                    {
                        conn.Open();
                        //   strResult = "{ \"items\": [ ";
                        //AGENT-BP000001
                        /*

                         SELECT DISTINCT  [BPCNUM_0] ,[BPCNAM_0] ,  [BPANUM_0]   ,[BPAADDLIG_0] ,[BPAADDLIG_1] ,[BPAADDLIG_2]   ,[MOB_0] ,[WEB_0]  FROM (SELECT * FROM [LIVE].[BPCUSTOMER]  WHERE upper(BPCNUM_0) =  upper('KAD0001') ) AS A, (SELECT * FROM [LIVE].[BPADDRESS] WHERE upper(BPANUM_0) =  upper('KAD0001')) AS B 
                        KAD0001
                         * 
                         */

                        using (SqlCommand cmd = new SqlCommand("SELECT DISTINCT  [BPCNUM_0] ,[BPCNAM_0] ,  [BPANUM_0]   ,[BPAADDLIG_0] ,[BPAADDLIG_1] ,[BPAADDLIG_2]   ,[MOB_0] ,[WEB_0]  FROM (SELECT * FROM [" + _connections.obj + "].[BPCUSTOMER]  WHERE upper(BPCNUM_0) =  upper('" + cid + "') ) AS A, (SELECT * FROM [" + _connections.obj + "].[BPADDRESS] WHERE upper(BPANUM_0) =  upper('" + cid + "')) AS B ", conn))
                        {
                            SqlDataReader reader = cmd.ExecuteReader();
                            _log.LogInformation("1");
                            if (reader.HasRows)
                            {
                                _log.LogInformation("2");
                                while (reader.Read())
                                {
                                    _log.LogInformation("3");

                                    if (reader.HasRows)
                                    {
                                        _log.LogInformation("4");
                                        resp.Params = new Params();
                                        //  while (reader.Read())
                                        //{
                                        _log.LogInformation("5");

                                        // strResult += "\"product_id\": \"" + reader["product_id"].ToString() + "\"";
                                        resp.Params.CustomerName = reader["BPCNAM_0"].ToString();
                                        //  lastname = ".";
                                        resp.Params.Mobile = reader["MOB_0"].ToString();
                                        resp.Params.Email = reader["WEB_0"].ToString();
                                        //      break; 

                                        // }
                                        _log.LogInformation("6");
                                    }
                                    else
                                    {
                                        // MessageBox.Show("No rows found.");
                                    }
                                    //reader.Close();
                                    // }



                                    break;
                                }
                            }
                            else
                            {
                                resp.Message = "Customer number " + cid + " does not exist. ";
                                resp.HasError = true;
                                resp.Amount = 0;
                                resp.ErrorMessages = null;
                                resp.Params = null;

                            }
                            reader.Close();
                        }

                        conn.Dispose();
                    }

                    resp.Message = "Success";
                    resp.HasError = false;
                    resp.Amount = 0;
                    // resp.ErrorMessages = null;
                    // resp.Param[0].Key = "CustomerName";
                    // resp.Param[0].Value = CustomerName;
                    // resp.Param[1].Key = "PhoneNo";
                    // resp.Param[1].Value = Mobile;
                    // resp.Param[2].Key = "Email";
                    // resp.Param[2].Value = Email;
                    // resp.Param[3].Key = "AccountNumber";
                    // resp.Param[3].Value = cid;
                    resp.Params.AccountNumber = cid;

                }
                catch (Exception c)
                {



                }


                return resp;
            }
            catch (Exception ex)
            {

            }
            return resp;
        }

        // public NotificationResponse Notification(NotificationRequest model)
        // {
        //     try
        //     {
        //         NotificationRequest model = JsonConvert.DeserializeObject<NotificationRequest>(JsonString);
        //         NotificationResponse resp = new NotificationResponse();
        //         string cid = "";
        //         string JsonString = "";
        //         Request.InputStream.Seek(0, SeekOrigin.Begin);
        //         JsonString = new StreamReader(Request.InputStream).ReadToEnd();


        //         if (model == null && model.BillerID == "xyz")
        //         {


        //             string CustomerName;
        //             string Mobile;
        //             string Email;
        //             // connect to database and retrieve customer information.
        //             string strOBJ2 = ConfigurationManager.AppSettings["obj2"].ToString();
        //             string oradb2 = ConfigurationManager.ConnectionStrings["idl2"].ToString();
        //             if (model.Amount > 0)
        //             {
        //                 List<Param> par = new Param();
        //                 for (int i = 0; i < model.Param.Count; i++)
        //                 {
        //                     if (Param[i].Key == "AccountNumber")
        //                     {
        //                         cid = Param[i].Vaue;
        //                     }
        //                 }
        //                 resp.Message = "Success";
        //                 resp.HasError = "False";
        //                 resp.Amount = 0;
        //                 resp.ErrorMessages = null;
        //                 resp.Param = mull;
        //             }
        //             else
        //             {
        //                 resp.Message = "Invalid payment amount";
        //                 resp.HasError = "True";
        //                 resp.Amount = 0;
        //                 resp.ErrorMessages = null;
        //                 resp.Param = mull;
        //             }
        //         }
        //         else
        //         {
        //             resp.Message = "Invalid Biller";
        //             resp.HasError = "True";
        //             resp.Amount = 0;
        //             resp.ErrorMessages = null;
        //             resp.Param = mull;
        //         }

        //         PostPayment(cid, model.Amount, model.CustomerAccountNumber);

        //         return resp;
        //     }
        //     catch (Exception ex)
        //     {

        //     }

        // }

    }

    // public class NOracleRepository<OEntity> : IRepository<OEntity> where OEntity : IConvertible
    // {
    //     protected readonly ILogger _log;
    //     protected readonly string _connectionString;
    //     //protected readonly string strOBJ2 = ConfigurationManager.AppSettings["obj2"].ToString();

    //     string[] result = { "", "", "" };


    //     public NOracleRepository(ILogger<dynamic> log)
    //     {
    //         _log = log;
    //         _connectionString = ConfigurationManager.ConnectionStrings["idl"].ToString();
    //     }

    //     public async Task<OEntity> Execute<OEntity, IEntity>(IEntity entity)
    //     {
    //         try
    //         {
    //             string oradb = _connectionString;
    //             string sql1 = "";
    //             using (SqlConnection conn = new SqlConnection(oradb))
    //             {

    //             }
    //             // await _context.AddAsync(entity);
    //             await _context.Set<OEntity>().AddAsync(entity);
    //             await _context.SaveChangesAsync();
    //             return (OEntity)Convert.ChangeType(value, typeof(OEntity));
    //         }
    //         catch (Exception ex)
    //         {
    //             _log.LogInformation(ex);
    //             return null;
    //         }

    //     }

    // }

}

