using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using StarScaff.Models;

namespace StarScaff.Database_Access_Layer
{    
    public class db
    {
        private SqlConnection con;
        DataTable dt = new DataTable();
        public void connection()
        {
            string constr = ConfigurationManager.ConnectionStrings["conn"].ToString();
            con = new SqlConnection(constr);
        }
        public DataTable LoginDetails(string UserName, string Password )
        {
            try
            {
                connection();
                using (SqlCommand cmd = new SqlCommand("Login_User",con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserName", UserName);
                    cmd.Parameters.AddWithValue("@Password", Password);
                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                    con.Close();
                }                                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }
        public string changepassword(string old_password, string new_password, string c_password)
        {
            string res = "";
            try
            {
                connection();
                //SqlCommand cmd = new SqlCommand("Select * from User_Registration where User_Password ='" + old_password + "'", con);
                //SqlDataReader dr = cmd.ExecuteReader();
                //if (dr.HasRows ==true)
                //{
                //    dr.Read();
                //    con.Close();
                //    if (new_password ==confirm_password)
                //    {
                //        con.Open();
                //        cmd = new SqlCommand("update User_Registration set User_Password ='"+new_password+ "' where User_Password ='"+old_password+"'", con);
                //        cmd.ExecuteNonQuery();
                //        con.Close();
                //        res = "Password has been changed successfully.";
                //    }
                //}
                //else
                //{
                //    res = "Invalid password,Please enter correct password";
                //}
                using (SqlCommand cmd = new SqlCommand("Prc_LoginChangePassword", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Oldpassword", old_password);
                    cmd.Parameters.AddWithValue("@Newpassword", new_password);
                    cmd.Parameters.AddWithValue("@Confirmpassword", c_password);

                    SqlParameter oblogin = new SqlParameter();
                    oblogin.ParameterName = "@message";
                    oblogin.SqlDbType = SqlDbType.NVarChar;
                    oblogin.Size = 100;
                    oblogin.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(oblogin);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    res = Convert.ToString(oblogin.Value);
                    con.Close();
                }

            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            return res;
        }
        public string CreateUserRegistration(DataTable objUserModel)
        {            
            string res = "";
            try
            {
                connection();
                using (SqlCommand cmd = new SqlCommand("PRC_UserRegistration_INSERT", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserRegistrationType", objUserModel);                    
                    SqlParameter oblogin = new SqlParameter();
                    oblogin.ParameterName = "@message";
                    oblogin.SqlDbType = SqlDbType.NVarChar;
                    oblogin.Size = 50;
                    oblogin.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(oblogin);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    res = Convert.ToString(oblogin.Value);
                    con.Close();
                    return res;
                }
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            return res;
        }
    }
}