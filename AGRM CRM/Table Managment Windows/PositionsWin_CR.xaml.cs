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

namespace AGRM_CRM.Table_Managment_Windows
{
    /// <summary>
    /// Логика взаимодействия для PositionsWin_CR.xaml
    /// </summary>
    public partial class PositionsWin_CR : Window
    {
        string TypeWin;
        MsSQL MsSQL = new MsSQL();
        log_input log = new log_input();
        string User;

        public PositionsWin_CR(string TypeW, string txt, string user)
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
            if(TypeWin == "(Создание)")
            {
                int id = 0;
                Delete.Visibility = Visibility.Hidden;
                try { id = Convert.ToInt16(MsSQL.sqlPerform("SELECT MAX(ID) FROM Должности")); }
                catch { }
                finally { ID.Text = Convert.ToString(id + 1); }
            }
            else
            {
                Post.Text = MsSQL.sqlPerform($"SELECT Должность FROM Должности WHERE ID = {ID.Text}");
                Income.Text = MsSQL.sqlPerform($"SELECT [Заработная Плата] FROM Должности WHERE ID = {ID.Text}");
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var ResDel = MessageBox.Show("Вы действительно хотите удалить запись?", "Удаление записи", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (ResDel == MessageBoxResult.Yes)
            {
                MsSQL.sqlPerform2($"DELETE FROM Должности WHERE ID = {ID.Text}");
                log.InputLog($"Deleting an entry ID= {ID.Text} - Clousing window | User: {User} | PositionsWin_CR", "stable");
                DialogResult = true; this.Close();
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if(ID.Text.Length > 0 && Post.Text.Length > 0 && Income.Text.Length > 0)
            {
                try { Income.Text = Convert.ToString(Math.Round(Convert.ToDouble(Income.Text), 2)); } catch { }
                var word = ",";
                var replaceTo = ".";
                var words = new Regex(word).Matches(Income.Text).OfType<Match>().Select(match => match.Index).ToList();
                try { Income.Text = Income.Text.Substring(0, words[0]) + replaceTo + Income.Text.Substring(words[0] + word.Length); } catch { }
                if (TypeWin == "(Создание)")
                {
                    try
                    {
                        MsSQL.sqlPerform2($"INSERT INTO Должности(ID, Должность, [Заработная Плата]) VALUES ('{ID.Text}', '{Post.Text}', '{Income.Text}')");
                        log.InputLog($"Successfully adding a record, ID = {ID.Text} - Clousing window| User: {User} | PositionsWin_CR", "stable");
                    }
                    catch { log.InputLog($"Failed to create a record - Clousing window | User: {User} | PositionsWin_CR", "Error"); }
                }
                else
                {
                    try
                    {
                        MsSQL.sqlPerform2($"UPDATE Должности SET Должность = '{Post.Text}', [Заработная Плата] = '{Income.Text}' WHERE ID = {ID.Text}");
                        log.InputLog($"Successful update of record data ID = {ID.Text} - Clousing window | User: {User} | PositionsWin_CR", "stable");
                    }
                    catch { log.InputLog($"Failed to update the data of the table entry - Clousing window | User: {User} | PositionsWin_CR", "Error"); }
                }
                DialogResult = true; this.Close();
            }
            else MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void Income_TextInput(object sender, TextCompositionEventArgs e)
        {
            if (!(Char.IsDigit(e.Text, 0) || (e.Text == ",")
               && (!Income.Text.Contains(",")
               && Income.Text.Length != 0)))
            {
                e.Handled = true;
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
    }
}
