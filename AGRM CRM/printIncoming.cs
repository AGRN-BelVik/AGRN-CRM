using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Office;
using Microsoft.Office.Interop.Word;
using Window = System.Windows.Window;
using Word = Microsoft.Office.Interop.Word;

namespace AGRM_CRM
{
    internal class printIncoming
    {
        Word._Application oWord = new Word.Application();
        MsSQL MsSQL = new MsSQL();
        public void PrintIncoming(string ID)
        {
            _Document oDoc = oWord.Documents.Add(Environment.CurrentDirectory + "\\ШаблонПриходной.dotx");
            oDoc.Bookmarks["number"].Range.Text = ID;
            oDoc.Bookmarks["dateNow"].Range.Text = MsSQL.sqlPerform($"SELECT Дата FROM Приходная_Накладная WHERE ID = {ID}");
            oDoc.Bookmarks["total"].Range.Text = MsSQL.sqlPerform($"SELECT Сумма FROM Приходная_Накладная WHERE ID = {ID}");
            oDoc.Bookmarks["totalSum"].Range.Text = MsSQL.sqlPerform($"SELECT Сумма FROM Приходная_Накладная WHERE ID = {ID}");
            oDoc.Bookmarks["quentityTotal"].Range.Text = MsSQL.sqlPerform($"SELECT [Количество Наименований] FROM Приходная_Накладная WHERE ID = {ID}");
            string idWorker = MsSQL.sqlPerform($"SELECT Ответственный FROM Приходная_Накладная WHERE ID = {ID}");
            string surnameW = MsSQL.sqlPerform($"SELECT Фамилия FROM Сотрудники WHERE ID = {idWorker}");
            string nameW = MsSQL.sqlPerform($"SELECT Имя FROM Сотрудники WHERE ID = {idWorker}");
            string midlenameW = MsSQL.sqlPerform($"SELECT Отчество FROM Сотрудники WHERE ID = {idWorker}");
            string fioW = surnameW + ' ' + nameW + ' ' + midlenameW;
            oDoc.Bookmarks["worker"].Range.Text = fioW;
            int i = 1; int c = 0;
            int count = Convert.ToInt32(MsSQL.sqlPerform($"SELECT COUNT(ID) FROM Приходная_ПодтаблицаВид WHERE Приходная = '{ID}'"));
            string testik = "";
            Word.Table table = oDoc.Tables[2];
            Word.Range cellrange;
            while (i == 1)
            {
                c++;
                testik = MsSQL.sqlPerform4($"SELECT Товар FROM Приходная_ПодтаблицаВид WHERE Приходная = '{ID}' AND ID = '{c}'");
                if (testik == "") break;
                else
                {
                    int row = c + 1;
                    cellrange = table.Cell(row, 1).Range;
                    cellrange.Text = c.ToString();
                    cellrange = table.Cell(row, 2).Range;
                    cellrange.Text = MsSQL.sqlPerform4($"SELECT Товар FROM Приходная_ПодтаблицаВид WHERE Приходная = '{ID}' AND ID = '{c}'");
                    cellrange = table.Cell(row, 3).Range;
                    cellrange.Text = MsSQL.sqlPerform4($"SELECT Цена FROM Приходная_Подтаблица WHERE Приходная = '{ID}' AND SubID = '{c}'");
                    cellrange = table.Cell(row, 4).Range;
                    cellrange.Text = MsSQL.sqlPerform4($"SELECT Количество FROM Приходная_Подтаблица WHERE Приходная = '{ID}' AND SubID = '{c}'");
                    cellrange = table.Cell(row, 5).Range;
                    cellrange.Text = MsSQL.sqlPerform4($"SELECT Сумма FROM Приходная_Подтаблица WHERE Приходная = '{ID}' AND SubID = '{c}'");
                    table.Rows.Add();
                }
            }
            try
            {
                oDoc.SaveAs(FileName: Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + $"\\Приходная_{ID}.docx");
                oDoc.Close();
                System.Diagnostics.Process p = new System.Diagnostics.Process();
                p.StartInfo.FileName = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + $"\\Приходная_{ID}.docx";
                p.Start();
            }
            catch { MessageBox.Show("Окно уже открыто.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }
        }
    }
}
