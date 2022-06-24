

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
        private ILogger<dynamic> _log;
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
                    string oradb = _connections.obj;//ConfigurationManager.ConnectionStrings["idl"].ToString();
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
        private string getPath(string add)
        {
            string strPath = "c:\\db\\a\\loggs\\" + add;
            if (System.IO.Directory.Exists("f:\\"))
            {
                strPath = "F:\\AGL_IDL_NIBSS\\bin\\loggs\\" + add;

            }
            try
            {
                if (!System.IO.Directory.Exists(strPath))
                {
                    System.IO.Directory.CreateDirectory(strPath);
                }
            }
            catch
            {
                strPath = "c:\\db\\a\\loggs\\" + add;
                if (!System.IO.Directory.Exists(strPath))
                {
                    System.IO.Directory.CreateDirectory(strPath);
                }
            }
            return strPath;

        }
        public NotificationResponse Notification(NotificationRequest model)
        {
            NotificationResponse resp = new NotificationResponse();
            try
            {
                // NotificationRequest model = JsonConvert.DeserializeObject<NotificationRequest>(JsonString);

                string cid = "";
                string JsonString = "";
                // Request.InputStream.Seek(0, SeekOrigin.Begin);
                // JsonString = new StreamReader(Request.InputStream).ReadToEnd();


                if (model == null && model.BillerID == "xyz")
                {


                    string CustomerName;
                    string Mobile;
                    string Email;
                    // connect to database and retrieve customer information.
                    // string strOBJ2 = ConfigurationManager.AppSettings["obj2"].ToString();
                    // string oradb2 = ConfigurationManager.ConnectionStrings["idl2"].ToString();
                    if (model.Amount > 0)
                    {
                        List<Contracts.Item> par = new List<Contracts.Item>();
                        for (int i = 0; i < model.Param.Count; i++)
                        {
                            if (model.Param[i].Key == "AccountNumber")
                            {
                                cid = model.Param[i].Value;
                            }
                        }
                        resp.Message = "Success";
                        resp.HasError = false;
                        resp.Amount = 0;
                        resp.ErrorMessages = null;
                        //resp.Param = mull;
                    }
                    else
                    {
                        resp.Message = "Invalid payment amount";
                        resp.HasError = true;
                        resp.Amount = 0;
                        resp.ErrorMessages = null;
                        //resp.Param = null;
                    }
                }
                else
                {
                    resp.Message = "Invalid Biller";
                    resp.HasError = true; ;
                    resp.Amount = 0;
                    resp.ErrorMessages = null;
                    //resp.Param = null;
                }

                // PostPayment needs to be written
                PostPayment(cid, Convert.ToDecimal (model.Amount), model.CustomerAccountNumber);

                return resp;
            }
            catch (Exception ex)
            {

            }
            return resp;
        }


        private void PostPayment(string customerID, decimal Amount, string CustomerAccountNumber)
        {
            string strOBJ = _connections.obj;//ConfigurationManager.AppSettings["obj"].ToString();
            Boolean ok = false;
            string extra = "";
            int add = 0;
            string BPANUM_0 = "";
            try
            {
                string strLine = "";
                string strDate = "";
                string[] result = { "", "", "" };
                string BPCNAM_0 = "";
                //string BPANUM_0 = "";
               // string BPANUM_0 = "";
                string BPAADDLIG_0 = "";
                string BPAADDLIG_1 = "";
                string BPAADDLIG_2 = "";
                string MOB_0 = "";
                string WEB_0 = "";
                string POSCOD_0 = "";
                string CTY_0 = "";
                string SAT_0 = "";
                string CRY_0 = "";
                string CRYNAM_0 = "";
                string sessionID = "";


                string sql = "";
                string COUNTT = "";
                string oradb = _connections.obj; //ConfigurationManager.ConnectionStrings["idl"].ToString();
                string sql1 = "";
                using (SqlConnection conn = new SqlConnection(oradb))
                {
                    conn.Open();
                    //   strResult = "{ \"items\": [ ";
                    //AGENT-BP000001
                    /*

                    SELECT [BPCNUM_0] ,[BPCNAM_0] ,  [BPANUM_0]   ,[BPAADDLIG_0] ,[BPAADDLIG_1] ,[BPAADDLIG_2]   ,[MOB_0] ,[WEB_0], [POSCOD_0] ,[CTY_0] ,[SAT_0] ,[CRY_0] ,[CRYNAM_0] FROM [LIVE].[BPCUSTOMER]  inner join  [LIVE].[BPADDRESS] on BPCNUM_0 = BPANUM_0 WHERE upper(rtrim(ltrim(BPCNUM_0))) =  upper('KAD0001')
                    */
                    //
                    //"SELECT DISTINCT  [BPCNUM_0] ,[BPCNAM_0] ,  [BPANUM_0]   ,[BPAADDLIG_0] ,[BPAADDLIG_1] ,[BPAADDLIG_2]   ,[MOB_0] ,[WEB_0]  FROM (SELECT * FROM [" + ConfigurationManager.AppSettings["obj"].ToString() + "].[BPCUSTOMER]  WHERE upper(BPCNUM_0) =  upper('" + customerID + "') ) AS A, (SELECT * FROM [" + ConfigurationManager.AppSettings["obj"].ToString() + "].[BPADDRESS] WHERE upper(BPANUM_0) =  upper('" + customerID + "')) AS B "
                    //"SELECT           [BPCNUM_0] ,[BPCNAM_0] ,  [BPANUM_0]   ,[BPAADDLIG_0] ,[BPAADDLIG_1] ,[BPAADDLIG_2]   ,[MOB_0] ,[WEB_0], [POSCOD_0] ,[CTY_0] ,[SAT_0] ,[CRY_0] ,[CRYNAM_0] FROM [" + strOBJ + "].[BPCUSTOMER]  inner join  [" + strOBJ + "].[BPADDRESS] on BPCNUM_0 = BPANUM_0 WHERE upper(rtrim(ltrim(BPCNUM_0))) =  upper('" + customerID.Trim() + "')"

                    try
                    {

                        strLine = System.DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss") + " (Notification SESSION ) :" + sessionID; //+ "SELECT [BPCNUM_0] ,[BPCNAM_0] ,  [BPANUM_0]   ,[BPAADDLIG_0] ,[BPAADDLIG_1] ,[BPAADDLIG_2]   ,[MOB_0] ,[WEB_0], [POSCOD_0] ,[CTY_0] ,[SAT_0] ,[CRY_0] ,[CRYNAM_0] FROM [" + strOBJ + "].[BPCUSTOMER]  inner join  [" + strOBJ + "].[BPADDRESS] on BPCNUM_0 = BPANUM_0 WHERE upper(rtrim(ltrim(BPCNUM_0))) =  upper('" + customerID.Trim() + "')";
                        System.IO.File.AppendAllText(getPath(extra) + strDate, strLine + "\n\r");
                        add++;
                        ok = true;
                    }
                    catch (Exception ds)
                    {

                    }

                    using (SqlCommand cmd = new SqlCommand("SELECT [BPCNUM_0] ,[BPCNAM_0] ,  [BPANUM_0]   ,[BPAADDLIG_0] ,[BPAADDLIG_1] ,[BPAADDLIG_2]   ,[MOB_0] ,[WEB_0], [POSCOD_0] ,[CTY_0] ,[SAT_0] ,[CRY_0] ,[CRYNAM_0] FROM [" + strOBJ + "].[BPCUSTOMER]  inner join  [" + strOBJ + "].[BPADDRESS] on BPCNUM_0 = BPANUM_0 WHERE upper(rtrim(ltrim(BPCNUM_0))) =  upper('" + customerID.Trim() + "')", conn))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();


                        try
                        {

                            strLine = System.DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss") + " \r\n\r\n(Notification request SQL) :" + " selected";
                            System.IO.File.AppendAllText(getPath(extra) + strDate, strLine + "\n\r");
                            add++;
                            ok = true;
                        }
                        catch (Exception ds)
                        {

                        }

                        if (reader.HasRows)
                        {
                            try
                            {

                                strLine = System.DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss") + " \r\n\r\n(Notification request SQL) :" + " has rows";
                                System.IO.File.AppendAllText(getPath(extra) + strDate, strLine + "\n\r");
                                add++;
                                ok = true;
                            }
                            catch (Exception ds)
                            {

                            }

                            while (reader.Read())
                            {
                                try
                                {

                                    strLine = System.DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss") + " \r\n\r\n(Notification request SQL) :" + " in read";
                                    System.IO.File.AppendAllText(getPath(extra) + strDate, strLine + "\n\r");
                                    add++;
                                    ok = true;
                                }
                                catch (Exception ds)
                                {

                                }

                                if (reader.HasRows)
                                {



                                    try
                                    {

                                        strLine = System.DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss") + " \r\n\r\n(Notification request SQL) :" + " start read";
                                        System.IO.File.AppendAllText(getPath(extra) + strDate, strLine + "\n\r");
                                        add++;
                                        ok = true;
                                    }
                                    catch (Exception ds)
                                    {

                                    }
                                    
                                    // strResult += "\"product_id\": \"" + reader["product_id"].ToString() + "\"";
                                    BPCNAM_0 = reader["BPCNAM_0"].ToString();
                                    BPANUM_0 = reader["BPANUM_0"].ToString();
                                    BPAADDLIG_0 = reader["BPAADDLIG_0"].ToString();
                                    BPAADDLIG_1 = reader["BPAADDLIG_1"].ToString();
                                    BPAADDLIG_2 = reader["BPAADDLIG_2"].ToString();
                                    MOB_0 = reader["MOB_0"].ToString();
                                    WEB_0 = reader["WEB_0"].ToString();
                                    POSCOD_0 = reader["POSCOD_0"].ToString();
                                    CTY_0 = reader["CTY_0"].ToString();
                                    SAT_0 = reader["SAT_0"].ToString();
                                    CRY_0 = reader["CRY_0"].ToString();
                                    CRYNAM_0 = reader["CRYNAM_0"].ToString();
                                    try
                                    {

                                        strLine = System.DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss") + " \r\n\r\n(Notification request SQL) :" + " done read";
                                        System.IO.File.AppendAllText(getPath(extra) + strDate, strLine + "\n\r");
                                        add++;
                                        ok = true;
                                    }
                                    catch (Exception ds)
                                    {


                                    }

                                    break;

                                }
                                else
                                {
                                    // MessageBox.Show("No rows found.");
                                }
                                reader.Close();
                                // }



                                break;
                            }
                        }
                        else
                        {
                            // MessageBox.Show("No rows found.");
                        }
                        reader.Close();
                    }

                    conn.Dispose();
                }



            }
            catch (Exception c)
            {
                try
                {
                    string strLine = "";
                    string strDate = "";
                    strLine = System.DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss") + " \r\n\r\n(Notification request SQL) :" + " error : " + c.Message;
                    System.IO.File.AppendAllText(getPath(extra) + strDate, strLine + "\n\r");
                    add++;
                    ok = true;
                }
                catch (Exception d1s)
                {

                }
            }

            //string BPANUM_0 = "";



            if (BPANUM_0.Trim().Length > 0)
            {






                try
                {
                    string sessionID;
                    string[] result = { "", "", "" };
                    string oradb = _connections.obj;// ConfigurationManager.ConnectionStrings["idl"].ToString();
                    string sql1 = "";
                    string COUNTT = "";
                    using (SqlConnection conn = new SqlConnection(oradb))
                    {
                        conn.Open();
                        //   strResult = "{ \"items\": [ ";
                        //AGENT-BP000001
                        //
                        //string d = (System.DateTime.Now.ToString("yyMMddHHmmss"));
                        //string h = toHex(d);
                        ////Taslim
                        string strCode = "";//= /"RECAIDL"/ "REC" + customerID.ToUpper() + toHex(System.DateTime.Now.ToString("yyMMddHHmmss"));
                        using (SqlCommand cmd = new SqlCommand("SELECT  count(*) c  FROM  [" + strOBJ + "].[PAYMENTH]   where rtrim(ltrim(upper(NUM_0))) like '" + strCode.ToUpper() + "%'", conn))
                        {
                            SqlDataReader reader = cmd.ExecuteReader();

                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {

                                    if (reader.HasRows)
                                    {
                                        while (reader.Read())
                                        {

                                            COUNTT = reader["c"].ToString();
                                            strCode += (long.Parse(COUNTT) + 1).ToString();
                                            break;

                                        }
                                    }
                                    else
                                    {
                                        // MessageBox.Show("No rows found.");
                                    }
                                    reader.Close();
                                    // }



                                    break;
                                }
                            }
                            else
                            {
                                // MessageBox.Show("No rows found.");
                            }
                            reader.Close();
                        }

                        conn.Dispose();
                    }



                }
                catch (Exception c)
                {

                }

                string strLongDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                string sql = "";
                string strDate = "";
                //0 - strCode
                //1 - customerID
                //2 - item.Amount.Replace(",","")
                //3 - item.Amount.Replace(",","")
                //4 - strLongDate
                //5 - strLongDate
                //6 - strOBJ


                int n = -1;
                try
                {
                    string[] result = { "", "", "" };
                    string oradb = _connections.obj; //ConfigurationManager.ConnectionStrings["idl"].ToString();
                    string sql1 = "";
                    using (SqlConnection conn = new SqlConnection(oradb))
                    {
                        conn.Open();
                        //   strResult = "{ \"items\": [ ";
                        //AGENT-BP000001
                        //Taslim
                        string strCode; //=  /"RECAIDL"/  "REC" + customerID.ToUpper() + toHex(System.DateTime.Now.ToString("yyMMddHHmmss"));//"RECAIDL" + System.DateTime.Now.ToString("yymm");

                        String SQL = ""; //string.Format(ResourceManager.GetString("pd.sql"), strCode.Replace("'", "`"), customerID.Replace("'", "`"), Amount.Replace(",", ""), Amount.Replace(",", ""), strLongDate.Replace("'", "`"), strLongDate.Replace("'", "`"), strOBJ.Replace("'", "`"), sessionID.Replace("'", "`"));
                        sql += SQL;
                        using (SqlCommand cmd = new SqlCommand(SQL, conn))
                        {
                            // n =  cmd.ExecuteNonQuery();


                        }

                        conn.Dispose();
                    }



                }
                catch (Exception c)
                {



                    try
                    {

                        string strLine = System.DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss") + " (Notification D ERROR ) :" + c.Message; //+ "SELECT [BPCNUM_0] ,[BPCNAM_0] ,  [BPANUM_0]   ,[BPAADDLIG_0] ,[BPAADDLIG_1] ,[BPAADDLIG_2]   ,[MOB_0] ,[WEB_0], [POSCOD_0] ,[CTY_0] ,[SAT_0] ,[CRY_0] ,[CRYNAM_0] FROM [" + strOBJ + "].[BPCUSTOMER]  inner join  [" + strOBJ + "].[BPADDRESS] on BPCNUM_0 = BPANUM_0 WHERE upper(rtrim(ltrim(BPCNUM_0))) =  upper('" + customerID.Trim() + "')";
                        System.IO.File.AppendAllText(getPath(extra) + strDate, strLine + "\n\r");
                        add++;
                        ok = true;
                    }
                    catch (Exception ds)
                    {

                    }

                }






                try
                {

                    string[] result = { "", "", "" };
                    string oradb = _connections.obj; //ConfigurationManager.ConnectionStrings["idl"].ToString();
                    string sql1 = "";
                    using (SqlConnection conn = new SqlConnection(oradb))
                    {
                        conn.Open();
                        //   strResult = "{ \"items\": [ ";
                        //AGENT-BP000001
                        //Taslim
                        string strCode; //= /"RECAIDL"/  "REC" + customerID.ToUpper() + toHex(System.DateTime.Now.ToString("yyMMddHHmmss")); ;// "RECAIDL" + System.DateTime.Now.ToString("yymm");
                        ///


                        //String SQL = string.Format(ResourceManager.GetString("ph.sql"), strOBJ, strCode, customerID, item.Amount.Replace(",", ""), strLongDate, "NIBSS - IDL integration", BPCNAM_0, BPAADDLIG_0, BPAADDLIG_1, BPAADDLIG_2, POSCOD_0, CTY_0, SAT_0, CRY_0, CRYNAM_0, bankCode);

                        String SQL = ""; // string.Format(ResourceManager.GetString("ph.sql"), strOBJ.Replace("'", "`"), strCode.Replace("'", "`"), customerID.Replace("'", "`"), Amount.Replace(",", ""), strLongDate.Replace("'", "`"), CustomerAccountNumber.Replace("'", "`"), BPCNAM_0.Replace("'", "`"), BPAADDLIG_0.Replace("'", "`"), BPAADDLIG_1.Replace("'", "`"), BPAADDLIG_2.Replace("'", "`"), POSCOD_0.Replace("'", "`"), CTY_0.Replace("'", "`"), SAT_0.Replace("'", "`"), CRY_0.Replace("'", "`"), CRYNAM_0.Replace("'", "`"), bankCode.Replace("'", "`"), sessionID.Replace("'", "`"));


                        sql += ";;;;;;" + SQL + ";;;;;;";

                    }

                    //conn.Dispose();
                }


                
                //}
                catch (Exception c)
                {


                    try
                    {

                        string strLine = System.DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss") + " (Notification H ERROR ) :" + c.Message; //+ "SELECT [BPCNUM_0] ,[BPCNAM_0] ,  [BPANUM_0]   ,[BPAADDLIG_0] ,[BPAADDLIG_1] ,[BPAADDLIG_2]   ,[MOB_0] ,[WEB_0], [POSCOD_0] ,[CTY_0] ,[SAT_0] ,[CRY_0] ,[CRYNAM_0] FROM [" + strOBJ + "].[BPCUSTOMER]  inner join  [" + strOBJ + "].[BPADDRESS] on BPCNUM_0 = BPANUM_0 WHERE upper(rtrim(ltrim(BPCNUM_0))) =  upper('" + customerID.Trim() + "')";
                       /// System.IO.File.AppendAllText(getPath(extra) + strDate, strLine + "\n\r");
                        add++;
                        ok = true;
                    }
                    catch (Exception ds)
                    {

                    }
                }

            }
        }

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

