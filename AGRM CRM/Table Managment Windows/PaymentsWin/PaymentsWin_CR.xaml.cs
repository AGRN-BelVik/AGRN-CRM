using AGRM_CRM.Logs;
using System;
using System.Collections.Generic;
using System.Data;
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

namespace AGRM_CRM.Table_Managment_Windows.PaymentsWin
{
    /// <summary>
    /// Логика взаимодействия для PaymentsWin_CR.xaml
    /// </summary>
    public partial class PaymentsWin_CR : Window
    {
        string TypeWin;
        MsSQL MsSQL = new MsSQL();
        log_input log = new log_input();
        string User;
        string surnameWorker, nameWorker, patronimicWorker, FIOworker;

        public PaymentsWin_CR(string TypeW, string txt, string user)
        {
            InitializeComponent();
            TypeWin = TypeW;
            ID.Text = txt;
            NameWin.Text += TypeW;
            User = user;
            //
            int i = 1; int c = 0;
            while (i == 1)
            {
                c++;
                surnameWorker = MsSQL.sqlPerform4($"SELECT Фамилия From Сотрудники WHERE ID = {c}");
                nameWorker = MsSQL.sqlPerform4($"SELECT Имя From Сотрудники WHERE ID = {c}");
                patronimicWorker = MsSQL.sqlPerform4($"SELECT Отчество From Сотрудники WHERE ID = {c}");
                if (surnameWorker.Length > 0 || nameWorker.Length > 0)
                {
                    FIOworker = surnameWorker + " " + nameWorker + " " + patronimicWorker;
                    Worker.Items.Add(FIOworker);
                }
                else i = 0;
            }
            Worker.Items.Add("Не выбрано");
            //
            Launch();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            //                                                  / режим работы/subID/USER/Выплата /
            SubTablePaymentsWin_CR win = new SubTablePaymentsWin_CR("(Создание)","",User,ID.Text);
            BlackBack(1); win.ShowDialog();
            if(win.DialogResult.HasValue && win.DialogResult.HasValue)
            {
                BlackBack(0); Counting();
                Workers_subtable.ItemsSource = MsSQL.DataGridUpdate($"WHERE Выплата = {ID.Text}", "Подтаблица_Выплата_ЗПВид").DefaultView;
            }
        }

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var row = sender as DataGridRow;// получение номера строки
            TextBlock id;
            id = Workers_subtable.Columns[0].GetCellContent(row) as TextBlock;
            //                                                     / режим работы    /  subID  / USER / Выплата /
            SubTablePaymentsWin_CR win = new SubTablePaymentsWin_CR("(Редактирование)", id.Text, User, ID.Text);
            BlackBack(1); win.ShowDialog();
            if (win.DialogResult.HasValue && win.DialogResult.HasValue)
            {
                Workers_subtable.ItemsSource = MsSQL.DataGridUpdate($"WHERE Выплата = {ID.Text}", "Подтаблица_Выплата_ЗПВид").DefaultView;
                BlackBack(0); Counting();
            }
        }

        public void Counting()
        {
            Count.Text = MsSQL.sqlPerformSUM($"SELECT COUNT(ID) FROM Подтаблица_Выплата_ЗП WHERE Выплата = {ID.Text}").ToString();
            Total.Text = MsSQL.sqlPerformSUM($"SELECT SUM(Сумма) FROM Подтаблица_Выплата_ЗП WHERE Выплата = {ID.Text}").ToString();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var ResDel = MessageBox.Show("Вы действительно хотите удалить запись?", "Удаление записи", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (ResDel == MessageBoxResult.Yes)
            {
                MsSQL.sqlPerform2($"DELETE FROM Выплата_ЗП WHERE ID = {ID.Text}");
                log.InputLog($"Deleting an entry ID= {ID.Text}(Выплата ЗП) - Clousing window | User: {User} | PaymentsWin_CR", "stable");
                MsSQL.sqlPerform2($"DELETE FROM Подтаблица_Выплата_ЗП WHERE Выплата = {ID.Text}");
                log.InputLog($"Deleting an entry Выплата = {ID.Text}(Подтаблица Выплата ЗП) - Clousing window | User: {User} | PaymentsWin_CR", "stable");
                DialogResult = true; this.Close();
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if(ID.Text.Length > 0 && Worker.Text.Length > 0 && Date.Text.Length > 0 && Count.Text.Length > 0 && Total.Text.Length > 0 && Worker.Text != "Не выбрано")
            {
                var fioWorker = Worker.Text;
                var wordsFioWorker = fioWorker.Split(' ');
                surnameWorker = wordsFioWorker[0];
                nameWorker = wordsFioWorker[1];
                patronimicWorker = wordsFioWorker[2];
                string idWorker = MsSQL.sqlPerform($"SELECT DISTINCT ID FROM Сотрудники WHERE Фамилия LIKE '{surnameWorker}' AND Имя LIKE '{nameWorker}' AND Отчество LIKE '{patronimicWorker}'");
                
                var word = ",";
                var replaceTo = ".";
                var words = new Regex(word).Matches(Total.Text).OfType<Match>().Select(match => match.Index).ToList();
                Total.Text = Total.Text.Substring(0, words[0]) + replaceTo + Total.Text.Substring(words[0] + word.Length);
                if (TypeWin == "(Создание)")
                {
                    try
                    {
                        MsSQL.sqlPerform2($"INSERT INTO Выплата_ЗП(ID, Ответственный, Дата, Количество, [Общая Сумма]) VALUES ('{ID.Text}', '{idWorker}', '{Date.Text}', '{Count.Text}', '{Total.Text}')");
                        log.InputLog($"Successfully adding a record, ID = {ID.Text} - Clousing window| User: {User} | PaymentsWin_CR", "stable");
                    }
                    catch { log.InputLog($"Failed to create a record - Clousing window | User: {User} | PaymentsWin_CR", "Error"); }
            }
                else
                {
                    try
                    {
                        MsSQL.sqlPerform2($"UPDATE Выплата_ЗП SET Ответственный = '{idWorker}', Дата = '{Date.Text}', Количество = '{Count.Text}', [Общая Сумма] = '{Total.Text}' WHERE ID = {ID.Text}");
                        log.InputLog($"Successful update of record data ID = {ID.Text} - Clousing window | User: {User} | PaymentsWin_CR", "stable");
                    }
                    catch { log.InputLog($"Failed to update the data of the table entry - Clousing window | User: {User} | PaymentsWin_CR", "Error"); }
                }
                DialogResult = true; this.Close();
            }
            else MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void Launch()
        {
            if(TypeWin == "(Создание)")
            {
                Delete.Visibility = Visibility.Hidden;
                Worker.Text = "Не выбрано";
                int id = 0;
                try { id = Convert.ToInt16(MsSQL.sqlPerform("SELECT MAX(ID) FROM Выплата_ЗП")); }
                catch { }
                finally { ID.Text = Convert.ToString(id + 1); }
            }
            else
            {
                //////////////////////////////////////////////////////////////////////////////////////////////
                string WorkerID = MsSQL.sqlPerform($"SELECT Ответственный FROM Выплата_ЗП WHERE ID = {ID.Text}");
                string surnameW = MsSQL.sqlPerform4($"SELECT Фамилия From Сотрудники WHERE ID = {WorkerID}");
                string nameW = MsSQL.sqlPerform4($"SELECT Имя From Сотрудники WHERE ID = {WorkerID}");
                string patronimicW = MsSQL.sqlPerform4($"SELECT Отчество From Сотрудники WHERE ID = {WorkerID}");
                if (surnameW.Length > 0 || nameW.Length > 0)
                {
                    Worker.Text = surnameW + " " + nameW + " " + patronimicW;
                }
                //////////////////////////////////////////////////////////////////////////////////////////////
                Workers_subtable.ItemsSource = MsSQL.DataGridUpdate($"WHERE Выплата = {ID.Text}","Подтаблица_Выплата_ЗПВид").DefaultView;
                Count.Text = MsSQL.sqlPerform($"SELECT Количество FROM Выплата_ЗП WHERE ID = {ID.Text}");
                Total.Text = MsSQL.sqlPerform($"SELECT [Общая Сумма] FROM Выплата_ЗП WHERE ID = {ID.Text}");
                Date.Text = MsSQL.sqlPerform($"SELECT Дата FROM Выплата_ЗП WHERE ID = {ID.Text}");
            }
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            if(TypeWin == "(Создание)") MsSQL.sqlPerform2($"DELETE FROM Подтаблица_Выплата_ЗП WHERE Выплата = {ID.Text}");
            DialogResult = true; this.Close();
        }

        public void BlackBack(int i)
        {
            if (i == 1) BlackBackground.Visibility = Visibility.Visible;
            else BlackBackground.Visibility = Visibility.Hidden;
        }
    }
}
