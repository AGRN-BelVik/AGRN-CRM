using AGRM_CRM.Logs;
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
using Xceed.Wpf.AvalonDock.Themes;

namespace AGRM_CRM.Table_Managment_Windows.IncomingWin
{
    /// <summary>
    /// Логика взаимодействия для IncomingWin_CR.xaml
    /// </summary>
    public partial class IncomingWin_CR : Window
    {
        string TypeWin;
        MsSQL MsSQL = new MsSQL();
        log_input log = new log_input();
        string User;
        string surnameWorker, nameWorker, patronimicWorker, FIOworker;
        public IncomingWin_CR(string TypeW, string txt, string user)
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
            Launch();
        }

        public void Launch()
        {
            WareHouse.Items.Clear();
            WareHouse.ItemsSource = MsSQL.SqlArray("SELECT Наименование FROM Склады", "Склады");
            Ware_subtable.ItemsSource = MsSQL.DataGridUpdate($"WHERE Приходная = {ID.Text}", "Приходная_ПодтаблицаВид").DefaultView;
            if (TypeWin == "(Создание)")
            {
                Delete.Visibility = Visibility.Hidden;
                AddBut.IsEnabled = false;
                Date.Text = DateTime.Now.ToString();
                WareHouse.Text = "Не выбрано";
                int id = 0;
                try { id = Convert.ToInt16(MsSQL.sqlPerform("SELECT MAX(ID) FROM Приходная_Накладная")); }
                catch { }
                finally { ID.Text = Convert.ToString(id + 1); }
            }
            else
            {
                string idWorker = MsSQL.sqlPerform($"SELECT Ответственный FROM Приходная_Накладная");
                string surnameW = MsSQL.sqlPerform4($"SELECT Фамилия From СотрудникиВид WHERE ID = {idWorker}");
                string nameW = MsSQL.sqlPerform4($"SELECT Имя From СотрудникиВид WHERE ID = {idWorker}");
                string patronimicW = MsSQL.sqlPerform4($"SELECT Отчество From СотрудникиВид WHERE ID = {idWorker}");
                WareHouse.IsEnabled = false;
                if (surnameW.Length > 0 || nameW.Length > 0)
                {
                    string FIOw = surnameW + " " + nameW + " " + patronimicW;
                    Worker.Text = FIOw;
                }
                Date.Text = MsSQL.sqlPerform($"SELECT Дата FROM Приходная_Накладная WHERE ID = {ID.Text}");
                string WareHouseID = MsSQL.sqlPerform($"SELECT Склад FROM Приходная_Накладная WHERE ID = {ID.Text}");
                WareHouse.Text = MsSQL.sqlPerform4($"SELECT Наименование FROM Склады WHERE ID = {WareHouseID}");
                Quantity.Text = MsSQL.sqlPerform($"SELECT [Количество Наименований] FROM Приходная_Накладная WHERE ID = {ID.Text}");
                Total.Text = MsSQL.sqlPerform($"SELECT Сумма FROM Приходная_Накладная WHERE ID = {ID.Text}");
            }
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            if (TypeWin == "(Создание)") 
            { 
                MsSQL.sqlPerform2($"DELETE FROM Приходная_Подтаблица WHERE Приходная = {ID.Text}");
                string warehouseID = MsSQL.sqlPerform4($"SELECT ID FROM Склады WHERE Наименование = {WareHouse.Text}");
                int CountWares = Convert.ToInt32(MsSQL.sqlPerform4($"SELECT COUNT(SubID) FROM Приходная_Подтаблица WHERE Приходная = {ID.Text}"));
                for (int i = 0; i < CountWares; i++)
                {
                    string ware = MsSQL.sqlPerform4($"SELECT Товар FROM Приходная_Подтаблица WHERE SubID = {i} AND Приходная = {ID.Text}");
                    int count = Convert.ToInt32(MsSQL.sqlPerform4($"SELECT Количество FROM Приходная_Подтаблица WHERE SubID = {i} AND Приходная = {ID.Text}"));
                    int last = Convert.ToInt32(MsSQL.sqlPerform4($"SELECT Количество FROM Склад_Подсистема WHERE Склад = {warehouseID} AND Товар = {ware}"));
                    MsSQL.sqlPerform2($"UPDATE Склад_Подсистема SET Количество = {last - count} WHERE Склад = {warehouseID} AND Товар = {ware}");
                }
            }

            DialogResult = true; this.Close();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            //                                                /   режим работы/SubID/ USER / Заказ /
            SubTableIncomingWin_CR win = new SubTableIncomingWin_CR("(Создание)", "", User, ID.Text, WareHouse.Text);
            BlackBack(1); win.ShowDialog();
            if(win.DialogResult.HasValue && win.DialogResult.HasValue)
            {
                Ware_subtable.ItemsSource = MsSQL.DataGridUpdate($"WHERE Приходная = {ID.Text}", "Приходная_ПодтаблицаВид").DefaultView;
                BlackBack(0); Counting();
            }
        }

        public void Counting()
        {
            Quantity.Text = MsSQL.sqlPerformSUM($"SELECT COUNT(ID) FROM Приходная_Подтаблица WHERE Приходная = {ID.Text}").ToString();
            Total.Text = MsSQL.sqlPerformSUM($"SELECT SUM(Сумма) FROM Приходная_Подтаблица WHERE Приходная = {ID.Text}").ToString();
        }

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var row = sender as DataGridRow;// получение номера строки
            TextBlock id;
            id = Ware_subtable.Columns[0].GetCellContent(row) as TextBlock;
            //                                                       /   режим работы   /   SubID / USER / Заказ /
            SubTableIncomingWin_CR SubTableCR = new SubTableIncomingWin_CR("(Редактирование)", id.Text, User, ID.Text, WareHouse.Text);
            BlackBack(1); SubTableCR.ShowDialog();
            if (SubTableCR.DialogResult.HasValue && SubTableCR.DialogResult.HasValue)
            {
                Ware_subtable.ItemsSource = MsSQL.DataGridUpdate($"WHERE Приходная = {ID.Text}", "Приходная_ПодтаблицаВид").DefaultView;
                BlackBack(0); Counting();
            }
        }

        public void BlackBack(int i)
        {
            if (i == 1) BlackBackground.Visibility = Visibility.Visible;
            else BlackBackground.Visibility = Visibility.Hidden;
        }

        private void PrintClick(object sender, MouseButtonEventArgs e)
        {
            printIncoming print = new printIncoming();
            int saveTest = SaveData();
            if (saveTest == 1) print.PrintIncoming(ID.Text);
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var ResDel = MessageBox.Show("Вы действительно хотите удалить запись?", "Удаление записи", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (ResDel == MessageBoxResult.Yes)
            {
                int ii = 1;
                string warehouseID = MsSQL.sqlPerform4($"SELECT DISTINCT ID FROM Склады WHERE Наименование = '{WareHouse.Text}'");
                int CountWares = Convert.ToInt32(MsSQL.sqlPerform4($"SELECT COUNT(SubID) FROM Приходная_Подтаблица WHERE Приходная = {ID.Text}"));
                for (int i = 0; i < CountWares; i++)
                {
                    string test = MsSQL.sqlPerform4($"SELECT Товар FROM Приходная_Подтаблица WHERE SubID = {ii} AND Приходная = {ID.Text}");
                    if (test.Length > 0)
                    {
                        string ware = MsSQL.sqlPerform4($"SELECT Товар FROM Приходная_Подтаблица WHERE SubID = {ii} AND Приходная = {ID.Text}");
                        string c = MsSQL.sqlPerform4($"SELECT Количество FROM Приходная_Подтаблица WHERE SubID = {ii} AND Приходная = {ID.Text}");
                        int count = Convert.ToInt32(c);
                        string l = MsSQL.sqlPerform4($"SELECT Количество FROM Склад_Подсистема WHERE Склад = '{warehouseID}' AND Товар = '{ware}'");
                        int last = Convert.ToInt32(l);
                        int now = last - count;
                        MessageBox.Show($"{now} | {warehouseID}({WareHouse.Text}) | {ware}");
                        MsSQL.sqlPerform2($"UPDATE Склад_Подсистема SET Количество = '{now}' WHERE Склад = '{warehouseID}' AND Товар = '{ware}'");
                        ii += 1;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            MsSQL.sqlPerform2($"DELETE FROM Регистр_Накоплений WHERE [Тип Транзакции] = 'Приход' AND [ID Транзакции] = {ID.Text}");
            log.InputLog($"Deleting an entry ID = {ID.Text}(Приходная Накладная)(Регистр Накоплений) - Clousing window | User: {User} | IncomingWin_CR", "stable");
            MsSQL.sqlPerform2($"DELETE FROM Приходная_Накладная WHERE ID = {ID.Text}");
            log.InputLog($"Deleting an entry ID = {ID.Text}(Приходная Накладная) - Clousing window | User: {User} | IncomingWin_CR", "stable");
            MsSQL.sqlPerform2($"DELETE FROM Приходная_Подтаблица WHERE Приходная = {ID.Text}");
            log.InputLog($"Deleting an entry Приходная = {ID.Text}(Приходная Подтаблица) - Clousing window | User: {User} | IncomingWin_CR", "stable");
            DialogResult = true; this.Close();
        }
        

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            int saveTest = SaveData();
            if(saveTest == 1) { DialogResult = true; this.Close(); }
        }

        private void WareHouse_DropDownClosed(object sender, EventArgs e)
        {
            if (WareHouse.Text != "Не выбрано" && WareHouse.Text.Length > 0)
            {
                AddBut.IsEnabled = true;
                WareHouse.IsEnabled = false;
            }
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
                string idWorker = MsSQL.sqlPerform($"SELECT DISTINCT ID FROM Сотрудники WHERE Фамилия LIKE '{surnameWorker}' AND Имя LIKE '{nameWorker}' AND Отчество LIKE '{patronimicWorker}'");
                string idWareHouse = MsSQL.sqlPerform($"SELECT DISTINCT ID FROM Склады WHERE Наименование = '{WareHouse.Text}'");
                var word = ",";
                var replaceTo = ".";
                var words = new Regex(word).Matches(Total.Text).OfType<Match>().Select(match => match.Index).ToList();
                int TESTnow = Convert.ToInt32(Quantity.Text);
                try { Total.Text = Total.Text.Substring(0, words[0]) + replaceTo + Total.Text.Substring(words[0] + word.Length); } catch { }
                if (TypeWin == "(Создание)")
                {
                    try
                    {
                        int idReg = 0;
                        try { idReg = Convert.ToInt32(MsSQL.sqlPerform($"SELECT MAX(ID) FROM Регистр_Накоплений")); }
                        catch { }
                        finally { idReg += 1; }
                        MsSQL.sqlPerform2($"INSERT INTO Регистр_Накоплений(ID, [Тип Транзакции], [ID Транзакции], Дата, Сумма) VALUES ('{idReg}', 'Приход', '{ID.Text}','{Date.Text}', '{Total.Text}')");
                        log.InputLog($"Successfully adding a record, ID = {ID.Text} - Clousing window| User: {User} | IncomingWin_CR (Register)", "stable");
                        MsSQL.sqlPerform2($"INSERT INTO Приходная_Накладная(ID, Ответственный, Дата, Склад, [Количество Наименований], Сумма) VALUES('{ID.Text}', '{idWorker}', '{Date.Text}', '{idWareHouse}', '{Quantity.Text}', '{Total.Text}')");
                        log.InputLog($"Successfully adding a record, ID = {ID.Text} - Clousing window| User: {User} | IncomingWin_CR", "stable");
                        return 1;
                    }
                    catch { log.InputLog($"Failed to create a record - Clousing window | User: {User} | IncomingWin_CR", "Error"); return 0; }
                }
                else
                {
                    //try
                    //{
                        MsSQL.sqlPerform2($"UPDATE Регистр_Накоплений SET Дата = '{Date.Text}', Сумма = '{Total.Text}' WHERE [ID Транзакции] = '{ID.Text}' AND [Тип Транзакции] = 'Расход'");
                        log.InputLog($"Successful update of record data ID = {ID.Text} - Clousing window | User: {User} | IncomingWin_CR (Register)", "stable");
                        MsSQL.sqlPerform2($"UPDATE Приходная_Накладная SET Ответственный = '{idWorker}', Дата = '{Date.Text}', Склад = '{idWareHouse}', [Количество Наименований] = '{Quantity.Text}', Сумма = '{Total.Text}' WHERE ID = {ID.Text}");
                        log.InputLog($"Successful update of record data ID = {ID.Text} - Clousing window | User: {User} | IncomingWin_CR", "stable");
                        return 1;
                    //}
                    //catch { log.InputLog($"Failed to update the data of the table entry - Clousing window | User: {User} | IncomingWin_CR", "Error"); return 0; }
                }
                
            }
            else MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); return 0;
        }
    }
}
