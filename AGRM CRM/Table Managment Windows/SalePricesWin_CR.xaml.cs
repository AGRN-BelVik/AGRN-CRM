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
    /// Логика взаимодействия для SalePricesWin_CR.xaml
    /// </summary>
    public partial class SalePricesWin_CR : Window
    {
        string TypeWin;
        MsSQL MsSQL = new MsSQL();
        log_input log = new log_input();
        string User;

        public SalePricesWin_CR(string TypeW, string txt, string user)
        {
            InitializeComponent();
            User = user;
            TypeWin = TypeW;
            if (txt.Length > 0) ID.Text = txt;
            NameWin.Text += TypeWin;
            Launch();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        public void Launch()
        {
            Ware.Items.Clear();
            Ware.ItemsSource = MsSQL.SqlArray("SELECT Наименование FROM Товары", "Товары");
            if (TypeWin == "(Создание)")
            {
                Delete.Visibility = Visibility.Hidden;
                Ware.Text = "Не выбрано";

                int id = 0;
                try { id = Convert.ToInt16(MsSQL.sqlPerform("SELECT MAX(ID) FROM [Цены_Продажи]")); }
                catch { }
                finally { ID.Text = Convert.ToString(id + 1); }
            }
            else
            {
                try { Ware.Text = MsSQL.sqlPerform($"SELECT Товар FROM [Цены_ПродажиВид] WHERE ID = {ID.Text}"); }
                catch { }
                Price.Text = MsSQL.sqlPerform($"SELECT Цена FROM [Цены_Продажи] WHERE ID = {ID.Text}");
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            string idWare = "";
            
            if (ID.Text.Length > 0 && Ware.Text != "Не выбрано" && Price.Text.Length > 0)
            {
                try { idWare = MsSQL.sqlPerform($"SELECT DISTINCT ID FROM Товары WHERE Наименование LIKE '{Ware.Text}'"); } catch { }
                Price.Text = Convert.ToString(Math.Round(Convert.ToDouble(Price.Text), 2));
                var word = ",";
                var replaceTo = ".";
                var words = new Regex(word).Matches(Price.Text).OfType<Match>().Select(match => match.Index).ToList();
                try { Price.Text = Price.Text.Substring(0, words[0]) + replaceTo + Price.Text.Substring(words[0] + word.Length); } catch { }
                if (TypeWin == "(Создание)" && idWare != "")
                {
                    try
                    {
                        MsSQL.sqlPerform2($"INSERT INTO [Цены_Продажи](ID, Товар, Цена) VALUES ('{ID.Text}' , '{idWare}' , '{Price.Text}')");
                        log.InputLog($"Successfully adding a record, ID = {ID.Text} - Clousing window| User: {User} | SalePricesWin_CR", "stable");
                    }
                    catch { log.InputLog($"Failed to create a record - Clousing window | User: {User} | SalePricesWin_CR", "Error"); }
                }
                else if ( idWare != "")
                {
                    try
                    {
                        MsSQL.sqlPerform2($"UPDATE [Цены_Продажи] SET Товар='{idWare}', Цена='{Price.Text}' WHERE ID = {ID.Text}");
                        log.InputLog($"Successful update of record data ID = {ID.Text} - Clousing window | User: {User} | SalePricesWin_CR", "stable");
                    }
                    catch { log.InputLog($"Failed to update the data of the table entry - Clousing window | User: {User} | SalePricesWin_CR", "Error"); }
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
                MsSQL.sqlPerform2($"DELETE FROM [Цены_Продажи] WHERE ID = {ID.Text}");
                log.InputLog($"Deleting an entry ID= {ID.Text} - Clousing window | User: {User} | SalePricesWin_CR", "stable");
                DialogResult = true; this.Close();
            }
        }

        private void Price_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!(Char.IsDigit(e.Text, 0) || (e.Text == ",")
               && (!Price.Text.Contains(",")
               && Price.Text.Length != 0)))
            {
                e.Handled = true;
            }
        }
    }
}
