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

namespace AGRM_CRM.Table_Managment_Windows
{
    /// <summary>
    /// Логика взаимодействия для Print.xaml
    /// </summary>
    public partial class Print : Window
    {
        string ID;
        Word._Application oWord = new Word.Application();
        MsSQL MsSQL = new MsSQL();
        public Print(string IDexpenditure, string type)
        {
            InitializeComponent();
            ID = IDexpenditure;
        }
        // Environment.CurrentDirectory + "\\AGRN_s.dotx"
        // закрытие
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true; this.Close();
        }

        // печатать как документ
        private void Print_Document(object sender, RoutedEventArgs e)
        {
            _Document oDoc = oWord.Documents.Add(Environment.CurrentDirectory + "\\ШаблонРасходной.dotx");
            oDoc.Bookmarks["number"].Range.Text = ID;
            oDoc.Bookmarks["dateNow"].Range.Text = MsSQL.sqlPerform($"SELECT Дата FROM Расходная_Накладная WHERE ID = {ID}");
            oDoc.Bookmarks["total"].Range.Text = MsSQL.sqlPerform($"SELECT Сумма FROM Расходная_Накладная WHERE ID = {ID}");
            oDoc.Bookmarks["totalSum"].Range.Text = MsSQL.sqlPerform($"SELECT Сумма FROM Расходная_Накладная WHERE ID = {ID}");
            oDoc.Bookmarks["quentityTotal"].Range.Text = MsSQL.sqlPerform($"SELECT [Количество Наименований] FROM Расходная_Накладная WHERE ID = {ID}");
            string idWorker = MsSQL.sqlPerform($"SELECT Ответственный FROM Расходная_Накладная WHERE ID = {ID}");
            string surnameW = MsSQL.sqlPerform($"SELECT Фамилия FROM Сотрудники WHERE ID = {idWorker}");
            string nameW = MsSQL.sqlPerform($"SELECT Имя FROM Сотрудники WHERE ID = {idWorker}");
            string midlenameW = MsSQL.sqlPerform($"SELECT Отчество FROM Сотрудники WHERE ID = {idWorker}");
            string fioW = surnameW + ' ' + nameW + ' ' + midlenameW;
            oDoc.Bookmarks["worker"].Range.Text = fioW;
            int i = 1; int c = 0;
            int count = Convert.ToInt32(MsSQL.sqlPerform($"SELECT COUNT(ID) FROM Расходная_ПодтаблицаВид WHERE Расходная = '{ID}'"));
            string testik = "";
            Word.Table table = oDoc.Tables[2];
            Word.Range cellrange;
            while (i == 1)
            {
                c++;
                testik = MsSQL.sqlPerform4($"SELECT Товар FROM Расходная_ПодтаблицаВид WHERE Расходная = '{ID}' AND ID = '{c}'");
                if (testik == "") break;
                else
                {
                    int row = c + 1;
                    cellrange = table.Cell(row, 1).Range;
                    cellrange.Text = c.ToString();
                    cellrange = table.Cell(row, 2).Range;
                    cellrange.Text = MsSQL.sqlPerform4($"SELECT Товар FROM Расходная_ПодтаблицаВид WHERE Расходная = '{ID}' AND ID = '{c}'");
                    cellrange = table.Cell(row, 3).Range;
                    cellrange.Text = MsSQL.sqlPerform4($"SELECT Цена FROM Расходная_Подтаблица WHERE Расходная = '{ID}' AND SubID = '{c}'");
                    cellrange = table.Cell(row, 4).Range;
                    cellrange.Text = MsSQL.sqlPerform4($"SELECT Количество FROM Расходная_Подтаблица WHERE Расходная = '{ID}' AND SubID = '{c}'");
                    cellrange = table.Cell(row, 5).Range;
                    cellrange.Text = MsSQL.sqlPerform4($"SELECT Сумма FROM Расходная_Подтаблица WHERE Расходная = '{ID}' AND SubID = '{c}'");
                    table.Rows.Add();
                }
            }
            try
            {
                oDoc.SaveAs(FileName: Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + $"\\Расходная_{ID}.docx");
                oDoc.Close();
                System.Diagnostics.Process p = new System.Diagnostics.Process();
                p.StartInfo.FileName = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + $"\\Расходная_{ID}.docx";
                p.Start();
                DialogResult = true; this.Close();
            }
            catch { MessageBox.Show("Окно уже открыто.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }
            
        }

        // печатать как чек
        private void Print_Receipt(object sender, RoutedEventArgs e)
        {
            
            _Document oDoc = oWord.Documents.Add(Environment.CurrentDirectory + "\\ШаблонЧека.dotx");
            oDoc.Bookmarks["number"].Range.Text = ID;
            string idWorker = MsSQL.sqlPerform($"SELECT Ответственный FROM Расходная_Накладная WHERE ID = {ID}");
            string surnameW = MsSQL.sqlPerform($"SELECT Фамилия FROM Сотрудники WHERE ID = {idWorker}");
            string nameW = MsSQL.sqlPerform($"SELECT Имя FROM Сотрудники WHERE ID = {idWorker}");
            string midlenameW = MsSQL.sqlPerform($"SELECT Отчество FROM Сотрудники WHERE ID = {idWorker}");
            string fioW = surnameW + ' ' + nameW + ' ' + midlenameW;
            oDoc.Bookmarks["worker"].Range.Text = fioW;
            string clientID = MsSQL.sqlPerform($"SELECT Клиент FROM Расходная_Накладная WHERE ID = {ID}");
            string surnameC, nameC, midlenameC, fioC; 
            if(ID != "0")
            {
                surnameC = MsSQL.sqlPerform($"SELECT Фамилия FROM Клиенты WHERE ID = {clientID}");
                nameC = MsSQL.sqlPerform($"SELECT Имя FROM Клиенты WHERE ID = {clientID}");
                midlenameC = MsSQL.sqlPerform4($"SELECT Отчество FROM Клиенты WHERE ID = {clientID}");
                fioC = surnameC + ' ' + nameC + ' ' + midlenameC;
            }
            else { fioC = ""; }
            oDoc.Bookmarks["client"].Range.Text = fioC;
            oDoc.Bookmarks["quentityTotal"].Range.Text = MsSQL.sqlPerform($"SELECT [Количество Наименований] FROM Расходная_Накладная WHERE ID = {ID}");
            oDoc.Bookmarks["total"].Range.Text = MsSQL.sqlPerform($"SELECT Сумма FROM Расходная_Накладная WHERE ID = {ID}");
            oDoc.Bookmarks["totalSum"].Range.Text = MsSQL.sqlPerform($"SELECT Сумма FROM Расходная_Накладная WHERE ID = {ID}");
            oDoc.Bookmarks["date"].Range.Text = MsSQL.sqlPerform($"SELECT Дата FROM Расходная_Накладная WHERE ID = {ID}");
            string warehouseID = MsSQL.sqlPerform($"SELECT Склад FROM Расходная_Накладная WHERE ID = {ID}");
            oDoc.Bookmarks["warehouse"].Range.Text = MsSQL.sqlPerform($"SELECT Адрес FROM Склады WHERE ID = {warehouseID}");
            Word.Table table = oDoc.Tables[1];
            Word.Range cellrange;
            string testik = "";
            int i = 1; int c = 0;
            while (i == 1)
            {
                c++;
                testik = MsSQL.sqlPerform4($"SELECT Товар FROM Расходная_ПодтаблицаВид WHERE Расходная = '{ID}' AND ID = '{c}'");
                if (testik == "") break;
                else
                {
                    table.Rows.Add();
                    int row = c + 1;
                    cellrange = table.Cell(row, 1).Range;
                    cellrange.Text = c.ToString();
                    cellrange = table.Cell(row, 2).Range;
                    cellrange.Text = MsSQL.sqlPerform4($"SELECT Товар FROM Расходная_ПодтаблицаВид WHERE Расходная = '{ID}' AND ID = '{c}'");
                    cellrange = table.Cell(row, 3).Range;
                    cellrange.Text = MsSQL.sqlPerform4($"SELECT Цена FROM Расходная_Подтаблица WHERE Расходная = '{ID}' AND SubID = '{c}'");
                    cellrange = table.Cell(row, 4).Range;
                    cellrange.Text = MsSQL.sqlPerform4($"SELECT Количество FROM Расходная_Подтаблица WHERE Расходная = '{ID}' AND SubID = '{c}'");
                    cellrange = table.Cell(row, 5).Range;
                    cellrange.Text = MsSQL.sqlPerform4($"SELECT Сумма FROM Расходная_Подтаблица WHERE Расходная = '{ID}' AND SubID = '{c}'");                 
                }
            }
            try
            {
                oDoc.SaveAs(FileName: Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + $"\\Чек_{ID}.docx");
                oDoc.Close();
                System.Diagnostics.Process p = new System.Diagnostics.Process();
                p.StartInfo.FileName = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + $"\\Чек_{ID}.docx";
                p.Start();
                DialogResult = true; this.Close();

            }
            catch { MessageBox.Show("Окно уже открыто.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }
        }
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
