using AGRM_CRM.Logs;
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

namespace AGRM_CRM.Table_Managment_Windows
{
    /// <summary>
    /// Логика взаимодействия для WorkersWin_CR.xaml
    /// </summary>
    public partial class WorkersWin_CR : Window
    {
        string TypeWin;
        MsSQL MsSQL = new MsSQL();
        log_input log = new log_input();
        string User;

        public WorkersWin_CR(string TypeW, string txt, string user)
        {
            InitializeComponent();
            TypeWin = TypeW;
            ID.Text = txt;
            NameWin.Text += TypeW;
            User = user;
            Launch();
        }

        public void Launch()
        {
            Positions.Items.Clear();
            Positions.ItemsSource = MsSQL.SqlArray("SELECT Должность FROM Должности", "Должности");
            if(TypeWin == "(Создание)")
            {
                Positions.Text = "Не выбрано";
                Delete.Visibility = Visibility.Hidden;
                int id = 0;
                try { id = Convert.ToInt16(MsSQL.sqlPerform("SELECT MAX(ID) FROM Сотрудники")); }
                catch { }
                finally { ID.Text = Convert.ToString(id + 1); }
            }
            else
            {
                //SurName.Text = MsSQL.sqlPerform($"SELECT Фамилия FROM Сотрудники WHERE ID = {ID.Text}");
                //Name.Text = MsSQL.sqlPerform($"SELECT Имя FROM Сотрудники WHERE ID = {ID.Text}");
                //MidlleName.Text = MsSQL.sqlPerform($"SELECT Отчество FROM Сотрудники WHERE ID = {ID.Text}");
                //Positions.Text = MsSQL.sqlPerform($"SELECT Должность FROM СотрудникиВид WHERE ID = {ID.Text}");
                //Phone.Text = MsSQL.sqlPerform($"SELECT Телефон FROM Сотрудники WHERE ID = {ID.Text}");
                //email.Text = MsSQL.sqlPerform($"SELECT [e-mail] FROM Сотрудники WHERE ID = {ID.Text}");
                //Series.Text = MsSQL.sqlPerform($"SELECT [Серия Паспорта] FROM Сотрудники WHERE ID = {ID.Text}");
                //Number.Text = MsSQL.sqlPerform($"SELECT [Номер Паспорта] FROM Сотрудники WHERE ID = {ID.Text}");
                //DateBirth.Text = MsSQL.sqlPerform($"SELECT [Дата Рождения] FROM Сотрудники WHERE ID = {ID.Text}");
                //DateDismissal.Text = MsSQL.sqlPerform4($"SELECT [Дата Увольнения] FROM Сотрудники WHERE ID = {ID.Text}");
                string[] mas = MsSQL.SqlArrayChar($"SELECT Фамилия, Имя, Отчество, Должность, Телефон, [e-mail], [Серия Паспорта], [Номер Паспорта], [Дата Рождения], [Дата Увольнения] FROM Сотрудники WHERE ID = {ID.Text}");
                MessageBox.Show($"{mas[0]}");
                if (mas[0] == "System.Char[]") { }
                else
                {
                    SurName.Text = mas[0].ToString();
                    Name.Text = mas[1].ToString();
                    MidlleName.Text = mas[2].ToString();
                    Positions.Text = mas[3].ToString();
                    Phone.Text = mas[4].ToString();
                    email.Text = mas[5].ToString();
                    Series.Text = mas[6].ToString();
                    Number.Text = mas[7].ToString();
                    DateBirth.Text = mas[8].ToString();
                    DateDismissal.Text = mas[9].ToString();
                }
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var ResDel = MessageBox.Show("Вы действительно хотите удалить запись?", "Удаление записи", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (ResDel == MessageBoxResult.Yes)
            {
                MsSQL.sqlPerform2($"DELETE FROM Сотрудники WHERE ID = {ID.Text}");
                log.InputLog($"Deleting an entry ID= {ID.Text} - Clousing window | User: {User} | WorkersWin_CR", "stable");
                DialogResult = true; this.Close();
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (ID.Text.Length > 0 && SurName.Text.Length > 0 && Name.Text.Length > 0 && Positions.Text != "Не выбрано" && Positions.Text.Length>0 && Phone.Text.Length > 0 && email.Text.Length > 0 && Series.Text.Length > 0 && Number.Text.Length > 0 && DateBirth.Text.Length > 0)
            {
                string idPositions = MsSQL.sqlPerform($"SELECT DISTINCT ID FROM Должности WHERE Должность = '{Positions.Text}'");
                if (TypeWin == "(Создание)")
                {
                    try
                    {
                        MsSQL.sqlPerform2($"INSERT INTO Сотрудники(ID, Фамилия, Имя, Отчество, Должность, Телефон, [e-mail], [Серия Паспорта], [Номер Паспорта], [Дата Рождения], [Дата Увольнения]) VALUES ('{ID.Text}', '{SurName.Text}', '{Name.Text}', '{MidlleName.Text}', '{idPositions}', '{Phone.Text}', '{email.Text}', '{Series.Text}', '{Number.Text}', '{DateBirth.Text}', '{DateDismissal.Text}')");
                        log.InputLog($"Successfully adding a record, ID = {ID.Text} - Clousing window| User: {User} | WorkersWin_CR", "stable");
                    }
                    catch { log.InputLog($"Failed to create a record - Clousing window | User: {User} | WorkersWin_CR", "Error"); }
                }
                else
                {
                    try
                    {
                        MsSQL.sqlPerform2($"UPDATE Сотрудники SET Фамилия = '{SurName.Text}', Имя = '{Name.Text}', Отчество = '{MidlleName.Text}', Должность = '{idPositions}', Телефон = '{Phone.Text}', [e-mail] = '{email.Text}', [Серия Паспорта] = '{Series.Text}', [Номер Паспорта] = '{Number.Text}', [Дата Рождения] = '{DateBirth.Text}', [Дата Увольнения] = '{DateDismissal.Text}' WHERE ID = {ID.Text}");
                        log.InputLog($"Successful update of record data ID = {ID.Text} - Clousing window | User: {User} | WorkersWin_CR", "stable");
                    }
                    catch { log.InputLog($"Failed to update the data of the table entry - Clousing window | User: {User} | WorkersWin_CR", "Error"); }
                }
                DialogResult = true; this.Close();
            }
            else MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true; this.Close();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
