using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace StarScaff.Models
{
    public class Account
    {
        public int ID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ErrorMsg { set; get; }
        public string SuccessMsg { set; get; }
    }
    public class ListtoDataTableConverter
    {
        public DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {

                    var ab = Props[i].PropertyType.Name;
                    if (ab.ToString() == "DateTime")
                    {
                        string format = "yyyy-MM-dd HH:mm:ss";
                        values[i] = Props[i].GetValue(item, null) != null ? Convert.ToDateTime(Props[i].GetValue(item, null)).ToString(format) : null;
                    }
                    else
                    {
                        values[i] = Props[i].GetValue(item, null);
                    }

                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

    }
}