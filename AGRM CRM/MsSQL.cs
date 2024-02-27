using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows;
using AGRM_CRM.Logs;
using System.Runtime.Remoting.Messaging;
using System.Windows.Controls.Primitives;

namespace AGRM_CRM
{
    internal class MsSQL
    {
        // - дом
        static string ConnectString = "Data Source=MSI;Initial Catalog=AGRN_CRM_DB;Integrated Security=True";

        // - колледж
        //static string ConnectString = @"Data Source=172.16.20.140\SQLEXPRESS2;Initial Catalog=AGRN_CRM_DB_test;Integrated Security=True";

        // - //
        SqlConnection sqlConnection = new SqlConnection($"{ConnectString}");
        SqlDataAdapter adapter = new SqlDataAdapter();
        DataTable table = new DataTable();
        DataTable Table = new DataTable($"");
        log_input log = new log_input();
        string Result = "";
        SqlCommand com = new SqlCommand();


        // тест на подключение к БД
        public bool SQLConnectionTest()
        {
            try
            {
                sqlConnection.Open();
                return true;
            }
            catch{ return false; }
            finally{ sqlConnection.Close(); }
        }


        // возврат строки подключения к БД
        public string SQLConnectionString(){ return ConnectString; }


        // обновление таблиц
        public DataTable DataGridUpdate(string command, string table)
        {
            try
            {
                sqlConnection.Open();
                SqlCommand Com = new SqlCommand($"SELECT * FROM {table} {command}", sqlConnection);
                Com.ExecuteNonQuery();
                SqlDataAdapter Adapter = new SqlDataAdapter(Com);   
                Table = new DataTable($"{table}");
                Adapter.Fill(Table);
            }
            catch
            {
            }
            finally{ sqlConnection.Close(); }
            return Table;
        }


        // выполнение команды с выводом результата
        public string sqlPerform(string text)
        {
            try { sqlConnection.Open(); } catch { sqlConnection.Close(); sqlConnection.Open(); }
            com = new SqlCommand(text, sqlConnection);

                try
                { Result = com.ExecuteScalar().ToString(); }
                catch { }
                finally { sqlConnection.Close(); }
            return Result;
        }


        //выполнение команды без результата
        public void sqlPerform2(string text)
        {
            sqlConnection.Open();
            com = new SqlCommand(text, sqlConnection);
            com.ExecuteNonQuery();
            sqlConnection.Close();
            return;
        }

        // выполнение команды с выводом результата
        public string sqlPerform4(string text)
        {
            try
            {
                sqlConnection.Open();
                SqlCommand com = new SqlCommand(text, sqlConnection);
                var test = com.ExecuteScalar();
                if (test != null)
                    Result = com.ExecuteScalar().ToString();
                else Result = "";
            }
            catch
            { }
            sqlConnection.Close();
            return Result;
        }

        // ввод данных с изображением 
        public void sqlImageInsert(byte[] Image_byte, string Text)
        {
            sqlConnection.Open();
            com = new SqlCommand(Text, sqlConnection);
            com.CommandText = Text;
            com.Parameters.Add("@ImageData", SqlDbType.Image, 1000000);
            com.Parameters["@ImageData"].Value = Image_byte;
            com.ExecuteNonQuery();
            sqlConnection.Close();
        }

        // вывод данных с изображением
        public byte[] Sql_Image(string text)
        {
            byte[] Image;
            sqlConnection.Open();
            com = new SqlCommand(text, sqlConnection);
            Image = (byte[])com.ExecuteScalar();
            sqlConnection.Close();
            return Image;
        }

        // вывод данных в массиве
        public string[] SqlArray(string text, string table)
        {
            int sum = Convert.ToInt32(sqlPerform($"SELECT SUM(ID) FROM {table}"));
            string[] Items = new string[sum+1];
            sqlConnection.Open();
            try
            {
                com = new SqlCommand(text, sqlConnection);
                SqlDataReader read = com.ExecuteReader();
                int i = 0;
                while (read.Read())
                {
                    try { Items[i] += (read.GetValue(0).ToString()); }
                    catch { }
                    i++;
                }
            }
            catch {}
            finally { sqlConnection.Close(); }
            Items[sum] += "Не выбрано";
            return Items;
        }

        public string[] SqlArrayChar(string text)
        {
            string[] Items = new string[text.Length];
            return Items;
        }

        public double sqlPerformSUM(string text)
        {
            double sum = 0;
            try
            {
                sqlConnection.Open();
                SqlCommand com = new SqlCommand(text, sqlConnection);
                sum = Convert.ToDouble(com.ExecuteScalar());
            }
            catch
            { }
            sqlConnection.Close();
            return sum;
        }
    }
}

