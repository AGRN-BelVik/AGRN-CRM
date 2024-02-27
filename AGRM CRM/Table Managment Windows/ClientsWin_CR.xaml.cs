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
    /// Логика взаимодействия для ClientsWin_CR.xaml
    /// </summary>
    public partial class ClientsWin_CR : Window
    {
        string TypeWin;
        MsSQL MsSQL = new MsSQL();
        log_input log = new log_input();
        string User;

        public ClientsWin_CR(string TypeW, string txt, string user)
        {
            InitializeComponent();
            TypeWin = TypeW;
            ID.Text = txt;
            NameWin.Text += TypeW;
            User = user;
            Launch();
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
            if(ID.Text.Length > 0 && Surname.Text.Length > 0 && Name.Text.Length > 0 && DateBirth.Text.Length > 0 && Phone.Text.Length > 0 && email.Text.Length > 0 && Adress.Text.Length > 0)
            {
                if(TypeWin == "(Создание)")
                {
                    try
                    {
                        MsSQL.sqlPerform2($"INSERT INTO Клиенты(ID, Фамилия, Имя, Отчество, [Дата Рождения], Телефон, [e-mail], Адрес) VALUES ('{ID.Text}', '{Surname.Text}', '{Name.Text}', '{Midllename.Text}', '{DateBirth.Text}', '{Phone.Text}', '{email.Text}', '{Adress.Text}')");
                        log.InputLog($"Successfully adding a record, ID = {ID.Text} - Clousing window| User: {User} | ClientsWin_CR", "stable");
                    }
                    catch { log.InputLog($"Failed to create a record - Clousing window | User: {User} | ClientsWin_CR", "Error"); }
                }
                else
                {
                    try
                    {
                        MsSQL.sqlPerform2($"UPDATE Клиенты SET Фамилия='{Surname.Text}', Имя='{Name.Text}', Отчество='{Midllename.Text}', [Дата Рождения]='{DateBirth.Text}', Телефон='{Phone.Text}', [e-mail]='{email.Text}', Адрес='{Adress.Text}' WHERE ID = {ID.Text}");
                        log.InputLog($"Successful update of record data ID = {ID.Text} - Clousing window | User: {User} | ClientsWin_CR","stable");
                    }
                    catch { log.InputLog($"Failed to update the data of the table entry - Clousing window | User: {User} | ClientsWin_CR", "Error"); }
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
                MsSQL.sqlPerform2($"DELETE FROM Клиенты WHERE ID = {ID.Text}");
                log.InputLog($"Deleting an entry ID= {ID.Text} - Clousing window | User: {User} | ClientsWin_CR", "stable");
                DialogResult = true; this.Close();
            }
        }

        public void Launch()
        {
            if(TypeWin == "(Создание)")
            {
                int id = 0;
                Delete.Visibility = Visibility.Hidden;
                try { id = Convert.ToInt16(MsSQL.sqlPerform("SELECT MAX(ID) FROM Клиенты")); }
                catch { }
                finally { ID.Text = Convert.ToString(id + 1); }
            }
            else
            {
                Surname.Text = MsSQL.sqlPerform($"SELECT Фамилия FROM Клиенты WHERE ID = {ID.Text}");
                Name.Text = MsSQL.sqlPerform($"SELECT Имя FROM Клиенты WHERE ID = {ID.Text}");
                try { Midllename.Text = MsSQL.sqlPerform($"SELECT Отчество FROM Клиенты WHERE ID = {ID.Text}"); } catch { }
                DateBirth.Text = MsSQL.sqlPerform($"SELECT [Дата Рождения] FROM Клиенты WHERE ID = {ID.Text}");
                Phone.Text = MsSQL.sqlPerform($"SELECT Телефон FROM Клиенты WHERE ID = {ID.Text}");
                email.Text = MsSQL.sqlPerform($"SELECT [e-mail] FROM Клиенты WHERE ID = {ID.Text}");
                Adress.Text = MsSQL.sqlPerform($"SELECT Адрес FROM Клиенты WHERE ID = {ID.Text}");
            }
        }
    }
}
