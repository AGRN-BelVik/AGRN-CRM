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
    /// Логика взаимодействия для SubTableOrdersWin_CR.xaml
    /// </summary>
    public partial class SubTableOrdersWin_CR : Window
    {
        string TypeWin;
        MsSQL MsSQL = new MsSQL();
        log_input log = new log_input();
        string User;
        string IDt;
        public SubTableOrdersWin_CR(string TypeW, string txt, string user, string sID)
        {
            InitializeComponent();
            TypeWin = TypeW;// режим работы
            ID.Text = txt; // SubID
            NameWin.Text += TypeW;
            User = user;// USER
            IDt = sID; // Заказ
            Launch();
        }

        public void Launch()
        {
            Ware.Items.Clear();
            Ware.ItemsSource = MsSQL.SqlArray("SELECT Наименование FROM Товары", "Товары");
            if (TypeWin == "(Создание)")
            {
                Ware.Text = "Не выбрано";
                int id = 0;
                Delete.Visibility = Visibility.Hidden;
                try { id = Convert.ToInt16(MsSQL.sqlPerform($"SELECT MAX(SubID) FROM Состав_Заказа WHERE Заказ = {IDt}")); }
                catch { }
                finally { ID.Text = Convert.ToString(id + 1); }
            }
            else
            {      
                Price.Text = MsSQL.sqlPerform4($"SELECT Цена FROM Состав_Заказа WHERE Заказ = {IDt} AND SubID = {ID.Text}");
                Quantity.Text = MsSQL.sqlPerform4($"SELECT Количество FROM Состав_Заказа WHERE Заказ = {IDt} AND SubID = {ID.Text}");
                Sum.Text = MsSQL.sqlPerform4($"SELECT Сумма FROM Состав_Заказа WHERE Заказ = {IDt} AND SubID = {ID.Text}");
                string wareID = MsSQL.sqlPerform4($"SELECT Товар FROM Состав_Заказа WHERE Заказ = {IDt} AND SubID = {ID.Text}");
                Ware.Text = MsSQL.sqlPerform4($"SELECT Наименование FROM Товары WHERE ID = {wareID}");
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true; this.Close();
        }

        private void Ware_DropDownClosed(object sender, EventArgs e)
        {
            if(Ware.Text != "Не выбрано")
            {
                Price.Text = MsSQL.sqlPerform4($"SELECT Цена FROM Цены_ПродажиВид WHERE Товар LIKE '{Ware.Text}'");
            }
        }

        private void Quantity_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Quantity.Text.Length > 0) Sum.Text = Convert.ToString(Math.Round((Convert.ToDouble(Price.Text) * Convert.ToDouble(Quantity.Text)), 2));
            else Sum.Text = "";
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if(ID.Text.Length > 0 && Ware.Text != "Не выбрано" && Price.Text.Length > 0 && Quantity.Text.Length > 0 && Sum.Text.Length > 0)
            {
                string idWare = "";
                try { idWare = MsSQL.sqlPerform($"SELECT DISTINCT ID FROM Товары WHERE Наименование LIKE '{Ware.Text}'"); } catch { }
                int id = 0;
                try { id = Convert.ToInt16(MsSQL.sqlPerform($"SELECT MAX(ID) FROM Состав_Заказа")); }
                catch { }
                finally { id += 1; }
                var word = ",";
                var replaceTo = ".";
                var words = new Regex(word).Matches(Sum.Text).OfType<Match>().Select(match => match.Index).ToList();
                try { Sum.Text = Sum.Text.Substring(0, words[0]) + replaceTo + Sum.Text.Substring(words[0] + word.Length); } catch { }
                if (TypeWin == "(Создание)")
                {
                    try
                    {
                        MsSQL.sqlPerform2($"INSERT INTO Состав_Заказа(ID, SubID, Заказ, Товар, Количество, Цена, Сумма) VALUES ('{id}', '{ID.Text}', '{IDt}', '{idWare}', '{Quantity.Text}', '{Price.Text}', '{Sum.Text}')");
                        log.InputLog($"Successfully adding a record, (SubID = {ID.Text}, Заказ = {IDt})  - Clousing window| User: {User} | SubTableOrdersWin_CR", "stable");
                    }
                    catch { log.InputLog($"Failed to create a record - Clousing window | User: {User} | SubTableOrdersWin_CR", "Error"); }
                }
                else
                {
                    try
                    {
                        MsSQL.sqlPerform2($"UPDATE Состав_Заказа SET Товар = '{idWare}', Количество = '{Quantity.Text}', Цена = '{Price.Text}', Сумма = '{Sum.Text}' WHERE SubID = {ID.Text} AND Заказ = {IDt}");
                        log.InputLog($"Successful update of record data (SubID = {ID.Text}, Заказ = {IDt})  - Clousing window | User: {User} | SubTableOrdersWin_CR", "stable");
                    }
                    catch { log.InputLog($"Failed to update the data of the table entry - Clousing window | User: {User} | SubTableOrdersWin_CR", "Error"); }
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
                MsSQL.sqlPerform2($"DELETE FROM Состав_Заказа WHERE SubID = {ID.Text} AND Заказ = {IDt}");
                log.InputLog($"Deleting an entry (SubID = {ID.Text}, Заказ = {IDt})  - Clousing window | User: {User} | SubTableOrdersWin_CR", "stable");
                DialogResult = true; this.Close();
            }
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Quantity_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }
    }
}
