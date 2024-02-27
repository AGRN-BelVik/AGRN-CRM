using AGRM_CRM.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace AGRM_CRM.Table_Managment_Windows
{
    /// <summary>
    /// Логика взаимодействия для OrdersWin_CR.xaml
    /// </summary>
    public partial class OrdersWin_CR : Window
    {
        string TypeWin;
        MsSQL MsSQL = new MsSQL();
        log_input log = new log_input();
        string User;
        string surnameClient, nameClient, patronimicClient, FIOclient;
        string surnameWorker, nameWorker, patronimicWorker, FIOworker;
        public OrdersWin_CR(string TypeW, string txt, string user)
        {
            InitializeComponent();
            TypeWin = TypeW;
            ID.Text = txt;
            NameWin.Text += TypeW;
            User = user;

            
            int i = 1;
            int c = 0;
            while (i == 1)
            {
                c++;
                surnameClient = MsSQL.sqlPerform4($"SELECT Фамилия From Клиенты WHERE ID = {c}");
                nameClient = MsSQL.sqlPerform4($"SELECT Имя From Клиенты WHERE ID = {c}");
                patronimicClient = MsSQL.sqlPerform4($"SELECT Отчество From Клиенты WHERE ID = {c}");
                if (surnameClient.Length > 0 || nameClient.Length > 0)
                {
                    FIOclient = surnameClient + " " + nameClient + " " + patronimicClient;
                    Client.Items.Add(FIOclient);
                }
                else i = 0;
            }
            Client.Items.Add("Не выбрано");
            Client.Text = "Не выбрано";
            i = 1; c = 0;
            while (i == 1)
            {
                c++;
                surnameWorker = MsSQL.sqlPerform4($"SELECT Фамилия From СотрудникиВид WHERE ID = {c}");
                nameWorker = MsSQL.sqlPerform4($"SELECT Имя From СотрудникиВид WHERE ID = {c}");
                patronimicWorker = MsSQL.sqlPerform4($"SELECT Отчество From СотрудникиВид WHERE ID = {c}");
                if (surnameWorker.Length > 0 || nameWorker.Length > 0)
                {
                    FIOworker = surnameWorker + " " + nameWorker + " " + patronimicWorker;
                    Worker.Items.Add(FIOworker);
                }
                else i = 0;
            }
            Worker.Items.Add("Не выбрано");
            Worker.Text = "Не выбрано";
            Launch();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            SubTableOrdersWin_CR SubTableCR = new SubTableOrdersWin_CR("(Создание)", "", User, ID.Text);
            BlackBack(1); SubTableCR.ShowDialog();
            if(SubTableCR.DialogResult.HasValue && SubTableCR.DialogResult.HasValue)
            {
                Ware_subtable.ItemsSource = MsSQL.DataGridUpdate($"WHERE Заказ = {ID.Text}", "Состав_ЗаказаВид").DefaultView;
                BlackBack(0); Counting();
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        { 
            if (TypeWin == "(Создание)") { MsSQL.sqlPerform2($"DELETE FROM Состав_Заказа WHERE Заказ = {ID.Text}"); } 
            DialogResult = true; this.Close(); 
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        { DragMove(); }

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var row = sender as DataGridRow;// получение номера строки
            TextBlock id;
            id = Ware_subtable.Columns[0].GetCellContent(row) as TextBlock;
            //                                                       /   режим работы   /   SubID / USER / Заказ /
            SubTableOrdersWin_CR SubTableCR = new SubTableOrdersWin_CR("(Редактирование)", id.Text, User, ID.Text);
            BlackBack(1); SubTableCR.ShowDialog();
            if (SubTableCR.DialogResult.HasValue && SubTableCR.DialogResult.HasValue)
            {
                Ware_subtable.ItemsSource = MsSQL.DataGridUpdate($"WHERE Заказ = {ID.Text}", "Состав_ЗаказаВид").DefaultView;
                BlackBack(0); Counting();
            }
        }

        public void Launch()
        {
            if(TypeWin == "(Создание)")
            {
                int id = 0;
                Delete.Visibility = Visibility.Hidden;
                try { id = Convert.ToInt16(MsSQL.sqlPerform("SELECT MAX(ID) FROM Заказы")); }
                catch { }
                finally { ID.Text = Convert.ToString(id + 1); }
            }
            else
            {
                string ClientID = MsSQL.sqlPerform($"SELECT ID_Клиента FROM Заказы WHERE ID = {ID.Text}");
                string WorkerID = MsSQL.sqlPerform($"SELECT Сотрудник FROM Заказы WHERE ID = {ID.Text}");
                //////////////////////////////////////////////////////////////////////////////////////////////
                string surnameC = MsSQL.sqlPerform4($"SELECT Фамилия From Клиенты WHERE ID = {ClientID}");
                string nameC = MsSQL.sqlPerform4($"SELECT Имя From Клиенты WHERE ID = {ClientID}");
                string patronimicC = MsSQL.sqlPerform4($"SELECT Отчество From Клиенты WHERE ID = {ClientID}");
                if (surnameC.Length > 0 || nameC.Length > 0)
                {
                    string FIOc = surnameC + " " + nameC + " " + patronimicC;
                    Client.Text = FIOc;
                }
                //------------------------------------------------------------------------------------------//
                string surnameW = MsSQL.sqlPerform4($"SELECT Фамилия From СотрудникиВид WHERE ID = {WorkerID}");
                string nameW = MsSQL.sqlPerform4($"SELECT Имя From СотрудникиВид WHERE ID = {WorkerID}");
                string patronimicW = MsSQL.sqlPerform4($"SELECT Отчество From СотрудникиВид WHERE ID = {WorkerID}");
                if (surnameW.Length > 0 || nameW.Length > 0)
                {
                    string FIOw = surnameW + " " + nameW + " " + patronimicW;
                    Worker.Text = FIOw;
                }
                //////////////////////////////////////////////////////////////////////////////////////////////
                Quantity.Text = MsSQL.sqlPerform($"SELECT [Количество Товаров] FROM Заказы WHERE ID = {ID.Text}");
                Amount.Text = MsSQL.sqlPerform($"SELECT [Итоговая Сумма] FROM Заказы WHERE ID = {ID.Text}");
                Ware_subtable.ItemsSource = MsSQL.DataGridUpdate($"WHERE Заказ = {ID.Text}", "Состав_ЗаказаВид").DefaultView;
                Counting();
            }
        }

        public void BlackBack(int i)
        {
            if (i == 1) BlackBackground.Visibility = Visibility.Visible;
            else BlackBackground.Visibility = Visibility.Hidden;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (ID.Text.Length > 0 && Client.Text.Length > 0 && Client.Text != "Не выбрано" && Worker.Text.Length > 0 && Worker.Text != "Не выбрано")
            {
                var fioClient = Client.Text;
                var wordsFioClient = fioClient.Split(' ');
                surnameClient = wordsFioClient[0];
                nameClient = wordsFioClient[1];
                patronimicClient = wordsFioClient[2];
                string idClient = MsSQL.sqlPerform($"SELECT DISTINCT ID FROM Клиенты WHERE Фамилия LIKE '{surnameClient}' AND Имя LIKE '{nameClient}' AND Отчество LIKE '{patronimicClient}'");

                var fioWorker = Worker.Text;
                var wordsFioWorker = fioWorker.Split(' ');
                surnameWorker = wordsFioWorker[0];
                nameWorker = wordsFioWorker[1];
                patronimicWorker = wordsFioWorker[2];
                string idWorker = MsSQL.sqlPerform($"SELECT DISTINCT ID FROM Сотрудники WHERE Фамилия LIKE '{surnameWorker}' AND Имя LIKE '{nameWorker}' AND Отчество LIKE '{patronimicWorker}'");
                var word = ",";
                var replaceTo = ".";
                var words = new Regex(word).Matches(Amount.Text).OfType<Match>().Select(match => match.Index).ToList();
                int TESTnow = Convert.ToInt32(Quantity.Text);
                try { Amount.Text = Amount.Text.Substring(0, words[0]) + replaceTo + Amount.Text.Substring(words[0] + word.Length); } catch { }
                if (TypeWin == "(Создание)")
                {
                    try
                    {
                        MsSQL.sqlPerform2($"INSERT INTO Заказы(ID, ID_Клиента, Сотрудник, [Количество Товаров], [Итоговая Сумма]) VALUES ('{ID.Text}','{idClient}','{idWorker}', '{Quantity.Text}', '{Amount.Text}')");
                        log.InputLog($"Successfully adding a record, ID = {ID.Text} - Clousing window| User: {User} | OrdersWin_CR", "stable");
                    }
                    catch { log.InputLog($"Failed to create a record - Clousing window | User: {User} | OrdersWin_CR", "Error"); }
                }
                else
                {
                    try
                    {
                        MsSQL.sqlPerform2($"UPDATE Заказы SET ID_Клиента = '{idClient}', Сотрудник = '{idWorker}', [Количество Товаров] = '{Quantity.Text}', [Итоговая Сумма] = '{Amount.Text}' WHERE ID = {ID.Text}");
                        log.InputLog($"Successful update of record data ID = {ID.Text} - Clousing window | User: {User} | OrdersWin_CR", "stable");
                    }
                    catch { log.InputLog($"Failed to update the data of the table entry - Clousing window | User: {User} | OrdersWin_CR", "Error"); }
                }
                DialogResult = true; this.Close();
            }
            else MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var ResDel = MessageBox.Show("Вы действительно хотите удалить запись?", "Удаление записи", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (ResDel == MessageBoxResult.Yes)
            {
                MsSQL.sqlPerform2($"DELETE FROM Заказы WHERE ID = {ID.Text}");
                log.InputLog($"Deleting an entry ID = {ID.Text}(Заказы) - Clousing window | User: {User} | OrdersWin_CR", "stable");
                MsSQL.sqlPerform2($"DELETE FROM Состав_Заказа WHERE Заказ = {ID.Text}");
                log.InputLog($"Deleting an entry Заказа = {ID.Text}(Состав_Заказа) - Clousing window | User: {User} | OrdersWin_CR", "stable");
                DialogResult = true; this.Close();
            }
        }

        public void Counting()
        {
            Quantity.Text = MsSQL.sqlPerformSUM($"SELECT SUM(Количество) FROM Состав_Заказа WHERE Заказ = {ID.Text}").ToString();
            Amount.Text = MsSQL.sqlPerformSUM($"SELECT SUM(Сумма) FROM Состав_Заказа WHERE Заказ = {ID.Text}").ToString();
        }
    }
}
