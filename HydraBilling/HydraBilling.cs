using Newtonsoft.Json;
using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydraBilling
{
    public class HydraBilling
    {
        public readonly static HydraBilling Context = new HydraBilling();
        public HydraBillingComments Comments;
        public HydraBillingUsers Users;
        public HydraBillingPersons Persons;

        public OracleConnection connect { get; private set; }

        public void Init(OracleConnection conn)
        {
            connect = conn;
            InitApp();

            Comments = new HydraBillingComments() { conn = connect };
            Users = new HydraBillingUsers() { conn = connect };
            Persons = new HydraBillingPersons() { conn = connect };
        }

        public void InitApp()
        {
            OracleCommand cmd = GetOracleCommand("MAIN.INIT", connect);
            cmd.Parameters.Add("vch_VC_IP", OracleDbType.Varchar2, "92.255.206.103", ParameterDirection.Input);
            cmd.Parameters.Add("vch_VC_USER", OracleDbType.Varchar2, "hid", ParameterDirection.Input);
            cmd.Parameters.Add("vch_VC_PASS", OracleDbType.Varchar2, "fCHc2cYuwNuEs", ParameterDirection.Input);
            cmd.Parameters.Add("vch_VC_APP_CODE", OracleDbType.Varchar2, "NETSERV_HID", ParameterDirection.Input);
            cmd.Parameters.Add("vch_VC_CLN_APPID", OracleDbType.Varchar2, "1CPozitel", ParameterDirection.Input);
            cmd.ExecuteNonQuery();

            cmd = GetOracleCommand("MAIN.SET_ACTIVE_FIRM", connect);
            cmd.Parameters.Add("num_N_FIRM_ID", OracleDbType.Int32, 100, ParameterDirection.Input);
            cmd.ExecuteNonQuery();
        }

        public static OracleCommand GetOracleCommand(string command, OracleConnection conn)
        {
            OracleCommand cmd = new OracleCommand(command, conn);
            cmd.BindByName = true;
            cmd.CommandType = CommandType.StoredProcedure;
            return cmd;
        }
        
        public class HydraBillingUsers
        {
            public OracleConnection conn { get; set; }
        }

        public class HydraBillingPersons
        {
            public OracleConnection conn { get; set; }
        }

        public class HydraBillingComments
        {
            public OracleConnection conn { get; set; }

            public string Put(int UserID, string text)
            {
                conn.Open();
                OracleCommand cmd = HydraBilling.GetOracleCommand("SI_SUBJECTS_PKG.SI_SUBJ_COMMENTS_PUT", conn);
                cmd.Parameters.Add("num_N_LINE_ID", OracleDbType.Int32);
                cmd.Parameters["num_N_LINE_ID"].Direction = ParameterDirection.InputOutput;
                cmd.Parameters["num_N_LINE_ID"].Value = null;
                cmd.Parameters.Add("num_N_SUBJECT_ID", OracleDbType.Int32, UserID, ParameterDirection.Input);
                cmd.Parameters.Add("clb_CL_COMMENT", OracleDbType.Clob, text, ParameterDirection.Input);
                cmd.ExecuteNonQuery();
                conn.Close();

                return new Result().GetResultString("OK", "", int.Parse(cmd.Parameters["num_N_LINE_ID"].Value.ToString()));
            }

            public string Get(int UserID)
            {
                return "";
            }

            public string Del(int MessageID)
            {
                return "";
            }
        }
    }  
    
    public class Result
    {
        public string Status;
        public string Message;
        public dynamic Data;

        public string GetResultString(string Status, string Message, dynamic Data)
        {
            this.Status = Status;
            this.Message = Message;
            this.Data = Data;
            return JsonConvert.SerializeObject(this);
        }
    }
}
