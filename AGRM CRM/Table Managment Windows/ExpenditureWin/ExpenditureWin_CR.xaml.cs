using AGRM_CRM.Logs;
using AGRM_CRM.Table_Managment_Windows.IncomingWin;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace AGRM_CRM.Table_Managment_Windows.ExpenditureWin
{
    /// <summary>
    /// Логика взаимодействия для ExpenditureWin_CR.xaml
    /// </summary>
    public partial class ExpenditureWin_CR : Window
    {
        string TypeWin;
        MsSQL MsSQL = new MsSQL();
        log_input log = new log_input();
        string User;
        string surnameWorker, nameWorker, patronimicWorker, FIOworker;
        string surnameClient, nameClient, patronimicClient, FIOclient;
        public ExpenditureWin_CR(string TypeW, string txt, string user)
        {
            InitializeComponent();
            TypeWin = TypeW;
            ID.Text = txt;
            NameWin.Text += TypeW;
            User = user;
            int i = 1; int c = 0;
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

            i = 1; c = 0;
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
            Launch();
        }

        public void Launch()
        {
            WareHouse.Items.Clear();
            WareHouse.ItemsSource = MsSQL.SqlArray("SELECT Наименование FROM Склады", "Склады");
            Ware_subtable.ItemsSource = MsSQL.DataGridUpdate($"WHERE Расходная = {ID.Text}", "Расходная_ПодтаблицаВид").DefaultView;
            if (TypeWin == "(Создание)")
            {
                Delete.Visibility = Visibility.Hidden;
                AddBut.IsEnabled = false;
                Date.Text = DateTime.Now.ToString();
                WareHouse.Text = "Не выбрано";
                int id = 0;
                try { id = Convert.ToInt16(MsSQL.sqlPerform("SELECT MAX(ID) FROM Расходная_Накладная")); }
                catch { }
                finally { ID.Text = Convert.ToString(id + 1); }
            }
            else
            {
                string idWorker = MsSQL.sqlPerform4($"SELECT Ответственный FROM Расходная_Накладная");
                string surnameW = MsSQL.sqlPerform4($"SELECT Фамилия From СотрудникиВид WHERE ID = {idWorker}");
                string nameW = MsSQL.sqlPerform4($"SELECT Имя From СотрудникиВид WHERE ID = {idWorker}");
                string patronimicW = MsSQL.sqlPerform4($"SELECT Отчество From СотрудникиВид WHERE ID = {idWorker}");
                WareHouse.IsEnabled = false;
                if (surnameW.Length > 0 || nameW.Length > 0)
                {
                    string FIOw = surnameW + " " + nameW + " " + patronimicW;
                    Worker.Text = FIOw;
                }
                
                string idClient = MsSQL.sqlPerform($"SELECT Клиент FROM Расходная_Накладная");
                string surnameC = MsSQL.sqlPerform4($"SELECT Фамилия From Клиенты WHERE ID = {idClient}");
                string nameC = MsSQL.sqlPerform4($"SELECT Имя From Клиенты WHERE ID = {idClient}");
                string patronimicC = MsSQL.sqlPerform4($"SELECT Отчество From Клиенты WHERE ID = {idClient}");
                if (surnameC.Length > 0 || nameC.Length > 0)
                {
                    string FIOc = surnameC + " " + nameC + " " + patronimicC;
                    Client.Text = FIOc;
                }
                if (idClient == "0") { Client.Text = "Не выбрано"; }
                Date.Text = MsSQL.sqlPerform($"SELECT Дата FROM Расходная_Накладная WHERE ID = {ID.Text}");
                string WareHouseID = MsSQL.sqlPerform($"SELECT Склад FROM Расходная_Накладная WHERE ID = {ID.Text}");
                WareHouse.Text = MsSQL.sqlPerform4($"SELECT Наименование FROM Склады WHERE ID = {WareHouseID}");
                Quantity.Text = MsSQL.sqlPerform($"SELECT [Количество Наименований] FROM Расходная_Накладная WHERE ID = {ID.Text}");
                Total.Text = MsSQL.sqlPerform($"SELECT Сумма FROM Расходная_Накладная WHERE ID = {ID.Text}");
            }
            Counting();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void WareHouse_DropDownClosed(object sender, EventArgs e)
        {
            if (WareHouse.Text != "Не выбрано" && WareHouse.Text.Length > 0)
            {
                AddBut.IsEnabled = true;
                WareHouse.IsEnabled = false;
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            if (TypeWin == "(Создание)")
            {
                MsSQL.sqlPerform2($"DELETE FROM Расходная_Подтаблица WHERE Расходная = {ID.Text}");
                string warehouseID = MsSQL.sqlPerform4($"SELECT ID FROM Склады WHERE Наименование = {WareHouse.Text}");
                int CountWares = Convert.ToInt32(MsSQL.sqlPerform4($"SELECT COUNT(SubID) FROM Расходная_Подтаблица WHERE Расходная = {ID.Text}"));
                for (int i = 0; i < CountWares; i++)
                {
                    string ware = MsSQL.sqlPerform4($"SELECT Товар FROM Расходная_Подтаблица WHERE SubID = {i} AND Расходная = {ID.Text}");
                    int count = Convert.ToInt32(MsSQL.sqlPerform4($"SELECT Количество FROM Расходная_Подтаблица WHERE SubID = {i} AND Расходная = {ID.Text}"));
                    int last = Convert.ToInt32(MsSQL.sqlPerform4($"SELECT Количество FROM Склад_Подсистема WHERE Склад = {warehouseID} AND Товар = {ware}"));
                    MsSQL.sqlPerform2($"UPDATE Склад_Подсистема SET Количество = {last + count} WHERE Склад = {warehouseID} AND Товар = {ware}");
                }
            }

            DialogResult = true; this.Close();
        }

        // печать
        private void PrintClick(object sender, MouseButtonEventArgs e)
        {
            int saveTest = SaveData();
            if(saveTest == 1)
            {
                Print win = new Print(ID.Text, "расход");
                BlackBack(1); win.ShowDialog();
                if (win.DialogResult.HasValue && win.DialogResult.HasValue) 
                {
                    Delete.Visibility = Visibility.Visible; TypeWin = "(Редактирование)"; BlackBack(0);
                }
            } 
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            //                                                 /   режим работы/SubID/ USER / Заказ / Склад
            SubTableExpenditureWin_CR win = new SubTableExpenditureWin_CR("(Создание)", "", User, ID.Text, WareHouse.Text);
            BlackBack(1); win.ShowDialog();
            if (win.DialogResult.HasValue && win.DialogResult.HasValue)
            {
                Ware_subtable.ItemsSource = MsSQL.DataGridUpdate($"WHERE Расходная = {ID.Text}", "Расходная_ПодтаблицаВид").DefaultView;
                BlackBack(0); Counting();
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var ResDel = MessageBox.Show("Вы действительно хотите удалить запись?", "Удаление записи", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (ResDel == MessageBoxResult.Yes)
            {
                int ii = 1;
                string warehouseID = MsSQL.sqlPerform4($"SELECT DISTINCT ID FROM Склады WHERE Наименование LIKE '{WareHouse.Text}'");
                int CountWares = Convert.ToInt32(MsSQL.sqlPerform4($"SELECT COUNT(SubID) FROM Расходная_Подтаблица WHERE Расходная = {ID.Text}"));
                for (int i = 0; i < CountWares; i++)
                {
                    string test = MsSQL.sqlPerform4($"SELECT Товар FROM Расходная_Подтаблица WHERE SubID = {ii} AND Расходная = {ID.Text}");
                    if (test.Length > 0)
                    {
                        string ware = MsSQL.sqlPerform4($"SELECT Товар FROM Расходная_Подтаблица WHERE SubID = {ii} AND Расходная = {ID.Text}");
                        string c = MsSQL.sqlPerform4($"SELECT Количество FROM Расходная_Подтаблица WHERE SubID = {ii} AND Расходная = {ID.Text}");
                        int count = Convert.ToInt32(c);
                        string l = MsSQL.sqlPerform4($"SELECT Количество FROM Склад_Подсистема WHERE Склад = {warehouseID} AND Товар = {ware}");
                        int last = Convert.ToInt32(l);
                        int now = last + count;
                        MsSQL.sqlPerform2($"UPDATE Склад_Подсистема SET Количество = '{now}' WHERE Склад = '{warehouseID}' AND Товар = '{ware}'");
                        ii += 1;
                    }
                    else
                    {
                        break;
                    }
                }
                MsSQL.sqlPerform2($"DELETE FROM Регистр_Накоплений WHERE [Тип Транзакции] = 'Расход' AND [ID Транзакции] = {ID.Text}");
                log.InputLog($"Deleting an entry ID = {ID.Text}(Расходная Накладная)(Регистр Накоплений) - Clousing window | User: {User} | IncomingWin_CR", "stable");

                MsSQL.sqlPerform2($"DELETE FROM Расходная_Накладная WHERE ID = {ID.Text}");
                log.InputLog($"Deleting an entry ID = {ID.Text}(Расходная Накладная) - Clousing window | User: {User} | IncomingWin_CR", "stable");
                MsSQL.sqlPerform2($"DELETE FROM Расходная_Подтаблица WHERE Расходная = {ID.Text}");
                log.InputLog($"Deleting an entry Расходная = {ID.Text}(Расходная Подтаблица) - Clousing window | User: {User} | IncomingWin_CR", "stable");
                DialogResult = true; this.Close();
            }
        }

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)//
        {
            var row = sender as DataGridRow;// получение номера строки
            TextBlock id;
            id = Ware_subtable.Columns[0].GetCellContent(row) as TextBlock;
            //                                                           /   режим работы   /   SubID / USER / Заказ / Склад
            SubTableExpenditureWin_CR SubTableCR = new SubTableExpenditureWin_CR("(Редактирование)", id.Text, User, ID.Text, WareHouse.Text);
            BlackBack(1); SubTableCR.ShowDialog();
            if (SubTableCR.DialogResult.HasValue && SubTableCR.DialogResult.HasValue)
            {
                Ware_subtable.ItemsSource = MsSQL.DataGridUpdate($"WHERE Расходная = {ID.Text}", "Расходная_ПодтаблицаВид").DefaultView;
                BlackBack(0); Counting();
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            int saveTest = SaveData();
            if(saveTest == 1)
            {
                DialogResult = true; this.Close();
            }
        }
        
            public void Counting()
            {
                Quantity.Text = MsSQL.sqlPerformSUM($"SELECT COUNT(SubID) FROM Расходная_Подтаблица WHERE Расходная = {ID.Text}").ToString();
                Total.Text = MsSQL.sqlPerformSUM($"SELECT SUM(Сумма) FROM Расходная_Подтаблица WHERE Расходная = {ID.Text}").ToString();
            }

            public void BlackBack(int i)
            {
                if (i == 1) BlackBackground.Visibility = Visibility.Visible;
                else BlackBackground.Visibility = Visibility.Hidden;
            }

        public int SaveData()
        {
            if (ID.Text.Length > 0 && Worker.Text != "Не выбрано" && Worker.Text.Length > 0 && Date.Text.Length > 0 && WareHouse.Text.Length > 0 && WareHouse.Text != "Не выбрано" && Quantity.Text.Length > 0 && Total.Text.Length > 0)
            {
                var fioWorker = Worker.Text;
                var wordsFioWorker = fioWorker.Split(' ');
                surnameWorker = wordsFioWorker[0];
                nameWorker = wordsFioWorker[1];
                patronimicWorker = wordsFioWorker[2];
                string idWorker = "0";
                if (Worker.Text != "Не выбрано") idWorker = MsSQL.sqlPerform4($"SELECT DISTINCT ID FROM Сотрудники WHERE Фамилия LIKE '{surnameWorker}' AND Имя LIKE '{nameWorker}' AND Отчество LIKE '{patronimicWorker}'");
                var fioClient = Client.Text;
                var wordsFioClient = fioClient.Split(' ');
                string idClient = "";
                var word = ",";
                var replaceTo = ".";
                var words = new Regex(word).Matches(Total.Text).OfType<Match>().Select(match => match.Index).ToList();
                int TESTnow = Convert.ToInt32(Quantity.Text);
                try { Total.Text = Total.Text.Substring(0, words[0]) + replaceTo + Total.Text.Substring(words[0] + word.Length); } catch { }
                if (Client.Text != "Не выбрано")
                {
                    surnameClient = wordsFioClient[0];
                    nameClient = wordsFioClient[1];
                    patronimicClient = wordsFioClient[2];
                    idClient = MsSQL.sqlPerform($"SELECT DISTINCT ID FROM Клиенты WHERE Фамилия LIKE '{surnameClient}' AND Имя LIKE '{nameClient}' AND Отчество LIKE '{patronimicClient}'");
                }
                else idClient = "0";
                string idWareHouse = MsSQL.sqlPerform4($"SELECT DISTINCT ID FROM Склады WHERE Наименование LIKE '{WareHouse.Text}'");
                if (TypeWin == "(Создание)")
                {
                    try
                    {
                        string ClTxt = ""; string ClCmd = "";
                        if (idClient.Length > 0) { ClTxt = ", Клиент"; ClCmd = $", '{idClient}'"; }
                        MessageBox.Show($"{ClTxt} | {ClCmd}");
                        int idReg = 0;
                        try { idReg = Convert.ToInt32(MsSQL.sqlPerform($"SELECT MAX(ID) FROM Регистр_Накоплений")); }
                        catch { }
                        finally { idReg += 1; }
                        MsSQL.sqlPerform2($"INSERT INTO Расходная_Накладная(ID, Ответственный, Дата, Склад, [Количество Наименований], Сумма{ClTxt}) VALUES('{ID.Text}', '{idWorker}', '{Date.Text}', '{idWareHouse}', '{Quantity.Text}', '{Total.Text}'{ClCmd})");
                        log.InputLog($"Successfully adding a record, ID = {ID.Text} - Clousing window| User: {User} | IncomingWin_CR", "stable");
                        MsSQL.sqlPerform2($"INSERT INTO Регистр_Накоплений(ID, [Тип Транзакции], [ID Транзакции], Дата, Сумма) VALUES ('{idReg}', 'Расход', '{ID.Text}','{Date.Text}', '{Total.Text}')");
                        log.InputLog($"Successfully adding a record, ID = {idReg} - Clousing window| User: {User} | IncomingWin_CR (Register)", "stable");
                        return 1;
                    }
                    catch { log.InputLog($"Failed to create a record - Clousing window | User: {User} | IncomingWin_CR", "Error"); return 0; }
                }
                else
                {
                    try
                    {
                        string ClCmd = "";
                        if (idClient.Length > 0) { ClCmd = $", Клиент = {idClient}"; }
                        MsSQL.sqlPerform2($"UPDATE Регистр_Накоплений SET Дата = '{Date.Text}', Сумма = '{Total.Text}' WHERE [ID Транзакции] = '{ID.Text}' AND [Тип Транзакции] = 'Расход'");
                        log.InputLog($"Successful update of record data ID = {ID.Text} - Clousing window | User: {User} | IncomingWin_CR (Register)", "stable");
                        MsSQL.sqlPerform2($"UPDATE Расходная_Накладная SET Дата = '{Date.Text}', Склад = '{idWareHouse}', [Количество Наименований] = '{Quantity.Text}', Сумма = '{Total.Text}', Клиент = '{idClient}' WHERE ID = {ID.Text}");
                        log.InputLog($"Successful update of record data ID = {ID.Text} - Clousing window | User: {User} | IncomingWin_CR", "stable");
                        return 1;
                    }
                    catch { log.InputLog($"Failed to update the data of the table entry - Clousing window | User: {User} | IncomingWin_CR", "Error"); return 0; }
                }
                
            }
            else { MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); return 0; }
        }
    }
}
