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

namespace AGRM_CRM.Table_Managment_Windows.PaymentsWin
{
    /// <summary>
    /// Логика взаимодействия для SubTablePaymentsWin_CR.xaml
    /// </summary>
    public partial class SubTablePaymentsWin_CR : Window
    {
        string TypeWin;
        MsSQL MsSQL = new MsSQL();
        log_input log = new log_input();
        string User;
        string surnameWorker, nameWorker, patronimicWorker, FIOworker;
        string IDt;
        public SubTablePaymentsWin_CR(string TypeW, string txt, string user, string sID)
        {
            InitializeComponent();
            int i = 1; int c = 0;
            TypeWin = TypeW;// режим работы
            ID.Text = txt;// SubID
            NameWin.Text += TypeW;
            User = user;// USER
            IDt = sID;// Выплата
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
            Launch();
        }

        public void Launch()
        {
            if(TypeWin == "(Создание)")
            {
                int id = 0;
                Delete.Visibility = Visibility.Hidden;
                try { id = Convert.ToInt16(MsSQL.sqlPerform($"SELECT MAX(SubID) FROM Подтаблица_Выплата_ЗП WHERE Выплата = {IDt}")); }
                catch { }
                finally { ID.Text = Convert.ToString(id + 1); }
                Worker.Text = "Не выбрано";
            }
            else
            {
                string WorkerID = MsSQL.sqlPerform($"SELECT Сотрудник FROM Подтаблица_Выплата_ЗП WHERE Выплата = {IDt} AND SubID = {ID.Text}");
                //
                string surnameW = MsSQL.sqlPerform4($"SELECT Фамилия From СотрудникиВид WHERE ID = {WorkerID}");
                string nameW = MsSQL.sqlPerform4($"SELECT Имя From СотрудникиВид WHERE ID = {WorkerID}");
                string patronimicW = MsSQL.sqlPerform4($"SELECT Отчество From СотрудникиВид WHERE ID = {WorkerID}");
                if (surnameW.Length > 0 || nameW.Length > 0)
                {
                    string FIOw = surnameW + " " + nameW + " " + patronimicW;
                    Worker.Text = FIOw;
                }
                //
                Salary.Text = MsSQL.sqlPerform($"SELECT [Заработная Плата] FROM Подтаблица_Выплата_ЗП WHERE Выплата = {IDt} AND SubID = {ID.Text}");
                Tax.Text = MsSQL.sqlPerform($"SELECT [Налоговый Вычет] FROM Подтаблица_Выплата_ЗП WHERE Выплата = {IDt} AND SubID = {ID.Text}");
                Fine.Text = MsSQL.sqlPerform($"SELECT Штрафы FROM Подтаблица_Выплата_ЗП WHERE Выплата = {IDt} AND SubID = {ID.Text}");
                Prize.Text = MsSQL.sqlPerform($"SELECT Надбавка FROM Подтаблица_Выплата_ЗП WHERE Выплата = {IDt} AND SubID = {ID.Text}");
                Total.Text = MsSQL.sqlPerform($"SELECT Сумма FROM Подтаблица_Выплата_ЗП WHERE Выплата = {IDt} AND SubID = {ID.Text}");
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true; this.Close();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            double test = Convert.ToDouble(Total.Text);
            if (ID.Text.Length > 0 && Worker.Text.Length > 0 && Salary.Text.Length > 0 && Total.Text.Length < 14 && Tax.Text.Length > 0 && Total.Text.Length > 0 && test > 0)
            {
                var fioWorker = Worker.Text;
                var wordsFioWorker = fioWorker.Split(' ');
                surnameWorker = wordsFioWorker[0];
                nameWorker = wordsFioWorker[1];
                patronimicWorker = wordsFioWorker[2];
                string idWorker = MsSQL.sqlPerform($"SELECT DISTINCT ID FROM Сотрудники WHERE Фамилия LIKE '{surnameWorker}' AND Имя LIKE '{nameWorker}' AND Отчество LIKE '{patronimicWorker}'");
                int id = 0;
                if (Fine.Text.Length > 0) Fine.Text = Convert.ToString(Math.Round(Convert.ToDouble(Fine.Text), 2)); else Fine.Text = "0";
                if (Prize.Text.Length > 0) Prize.Text = Convert.ToString(Math.Round(Convert.ToDouble(Prize.Text), 2)); else Prize.Text = "0";
                try { id = Convert.ToInt16(MsSQL.sqlPerform($"SELECT MAX(ID) FROM Подтаблица_Выплата_ЗП")); }
                catch { }
                finally { id += 1; }
                
                var word = ",";
                var replaceTo = ".";
                var words = new Regex(word).Matches(Total.Text).OfType<Match>().Select(match => match.Index).ToList();
                try { Total.Text = Total.Text.Substring(0, words[0]) + replaceTo + Total.Text.Substring(words[0] + word.Length); } catch { }
                if (TypeWin == "(Создание)")
                {
                    try
                    {
                        MsSQL.sqlPerform2($"INSERT INTO Подтаблица_Выплата_ЗП(ID, SubID, Выплата, Сотрудник, [Заработная Плата], [Налоговый Вычет], Штрафы, Надбавка, Сумма) VALUES ('{id}' , '{ID.Text}' , '{IDt}' , '{idWorker}' , '{Salary.Text}' , '{Tax.Text}' , '{Fine.Text}' , '{Prize.Text}' , '{Total.Text}')");
                        log.InputLog($"Successfully adding a record, (SubID = {ID.Text}, Выплата = {IDt})  - Clousing window| User: {User} | SubTablePaymentsWin_CR", "stable");
                    }
                    catch { log.InputLog($"Failed to create a record - Clousing window | User: {User} | SubTablePaymentsWin_CR", "Error"); }
                }
                else
                {
                    try
                    {
                        MsSQL.sqlPerform2($"UPDATE Подтаблица_Выплата_ЗП SET Сотрудник = '{idWorker}', [Заработная Плата] = '{Salary.Text}', [Налоговый Вычет] = '{Tax.Text}', Штрафы = '{Fine.Text}', Надбавка = '{Prize.Text}', Сумма = '{Total.Text}' WHERE SubID = {ID.Text} AND Выплата = {IDt}");
                        log.InputLog($"Successful update of record data (SubID = {ID.Text}, Выплата = {IDt})  - Clousing window | User: {User} | SubTablePaymentsWin_CR", "stable");
                    }
                    catch { log.InputLog($"Failed to update the data of the table entry - Clousing window | User: {User} | SubTablePaymentsWin_CR", "Error"); }
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
                MsSQL.sqlPerform2($"DELETE FROM Подтаблица_Выплата_ЗП WHERE SubID = {ID.Text} AND Выплата = {IDt}");
                log.InputLog($"Deleting an entry (SubID = {ID.Text}, Выплата = {IDt}) - Clousing window | User: {User} | SubTablePaymentsWin_CR", "stable");
                DialogResult = true; this.Close();
            }
        }

        private void Salary_TextChanged(object sender, TextChangedEventArgs e)
        {
            string salaryText;
            try { salaryText = Salary.Text.Replace(".", ","); } catch { salaryText = Salary.Text; }
            if (salaryText.Length > 0) try { Tax.Text = Convert.ToString(Math.Round((Convert.ToDouble(salaryText) - (Convert.ToDouble(salaryText) / 100 * 13)), 2)); } catch { Tax.Text = Convert.ToString((Convert.ToDouble(salaryText) - (Convert.ToDouble(salaryText) / 100 * 13))); }
            TotalSum();
        }

        private void Fine_TextInput(object sender, TextCompositionEventArgs e)
        {
            if (!(Char.IsDigit(e.Text, 0) || (e.Text == ",")
               && (!Fine.Text.Contains(",")
               && Fine.Text.Length != 0)))
            {
                e.Handled = true;
            }
        }

        private void Prize_TextInput(object sender, TextCompositionEventArgs e)
        {
            if (!(Char.IsDigit(e.Text, 0) || (e.Text == ",")
               && (!Prize.Text.Contains(",")
               && Prize.Text.Length != 0)))
            {
                e.Handled = true;
            }
        }

        private void Worker_DropDownClosed(object sender, EventArgs e)
        {
            if (Worker.Text.Length > 0 && Worker.Text != "Не выбрано")
            {
                var fioWorker = Worker.Text;
                var wordsFioWorker = fioWorker.Split(' ');
                surnameWorker = wordsFioWorker[0];
                nameWorker = wordsFioWorker[1];
                patronimicWorker = wordsFioWorker[2];
                string idWorker = MsSQL.sqlPerform4($"SELECT DISTINCT ID FROM Сотрудники WHERE Фамилия LIKE '{surnameWorker}' AND Имя LIKE '{nameWorker}' AND Отчество LIKE '{patronimicWorker}'");
                int idPost = Convert.ToInt32(MsSQL.sqlPerformSUM($"SELECT Должность FROM Сотрудники WHERE ID = {idWorker}"));
                Salary.Text = MsSQL.sqlPerform4($"SELECT [Заработная Плата] FROM Должности WHERE ID = {idPost}");
            }
        }

        private void Salary_TextInput(object sender, TextCompositionEventArgs e)
        {
            if (!(Char.IsDigit(e.Text, 0) || (e.Text == ",")
               && (!Salary.Text.Contains(",")
               && Salary.Text.Length != 0)))
            {
                e.Handled = true;
            }
        }

        public void TotalSum()
        {
            double tax = 0;
            double fine = 0;
            double prize = 0;
            if(Tax.Text.Length>0) tax = Convert.ToDouble(Tax.Text);
            if(Fine.Text.Length>0) fine = Convert.ToDouble(Fine.Text);
            if(Prize.Text.Length>0) prize = Convert.ToDouble(Prize.Text);

            Total.Text = Convert.ToString(Math.Round((tax-fine+prize),2));
        }

        private void Fine_TextChanged(object sender, TextChangedEventArgs e)
        {
            TotalSum();
        }
    }
}
