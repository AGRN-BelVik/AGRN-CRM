using AGRM_CRM.Logs;
using AGRM_CRM.Table_Managment_Windows.PaymentsWin;
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
using Xceed.Wpf.AvalonDock.Themes;

namespace AGRM_CRM.Table_Managment_Windows.WareHousesWin
{
    /// <summary>
    /// Логика взаимодействия для WareHousesWin_CR.xaml
    /// </summary>
    public partial class WareHousesWin_CR : Window
    {
        string TypeWin;
        MsSQL MsSQL = new MsSQL();
        log_input log = new log_input();
        string User;
        public WareHousesWin_CR(string TypeW, string txt, string user)
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
            if (TypeWin == "(Создание)")
            {
                Delete.Visibility = Visibility.Hidden;
                int id = 0;
                try { id = Convert.ToInt16(MsSQL.sqlPerform("SELECT MAX(ID) FROM Склады")); }
                catch { }
                finally { ID.Text = Convert.ToString(id + 1); }
                Quantity.Text = "0";
                Total.Text = "0";
            }
            else
            {
                Name.Text = MsSQL.sqlPerform($"SELECT Наименование FROM Склады WHERE ID = {ID.Text}");
                Address.Text = MsSQL.sqlPerform($"SELECT Адрес FROM Склады WHERE ID = {ID.Text}");
                Quantity.Text = MsSQL.sqlPerform($"SELECT [Количество Наименований] FROM Склады WHERE ID = {ID.Text}");
                Total.Text = MsSQL.sqlPerform($"SELECT [Всего Товаров] FROM Склады WHERE ID = {ID.Text}");
                Wares.ItemsSource = MsSQL.DataGridUpdate($"WHERE Склад = {ID.Text}", "Склад_ПодсистемаВид").DefaultView;
            }
            Counting();
        }

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var row = sender as DataGridRow;// получение номера строки
            TextBlock id;
            id = Wares.Columns[0].GetCellContent(row) as TextBlock;
            //                                                            / режим работы  /  subID  / USER / Склад /
            SubTableWareHousesWin_CR win = new SubTableWareHousesWin_CR("(Редактирование)", id.Text, User, ID.Text);
            BlackBack(1); win.ShowDialog();
            if (win.DialogResult.HasValue && win.DialogResult.HasValue)
            {
                Wares.ItemsSource = MsSQL.DataGridUpdate($"WHERE Склад = {ID.Text}", "Склад_ПодсистемаВид").DefaultView;
                BlackBack(0); Counting();
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            //                                                     / режим работы/ subID/ USER / Склад /
            SubTableWareHousesWin_CR win = new SubTableWareHousesWin_CR("(Создание)", "", User, ID.Text);
            BlackBack(1); win.ShowDialog();
            if(win.DialogResult.HasValue && win.DialogResult.HasValue)
            {
                BlackBack(0); Counting();
                Wares.ItemsSource = MsSQL.DataGridUpdate($"WHERE Склад = {ID.Text}", "Склад_ПодсистемаВид").DefaultView;
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var ResDel = MessageBox.Show("Вы действительно хотите удалить запись?", "Удаление записи", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (ResDel == MessageBoxResult.Yes)
            {
                MsSQL.sqlPerform2($"DELETE FROM Склады WHERE ID = {ID.Text}");
                log.InputLog($"Deleting an entry ID= {ID.Text}(Склады) - Clousing window | User: {User} | WareHousesWin_CR", "stable");
                MsSQL.sqlPerform2($"DELETE FROM Склад_Подсистема WHERE Склад = {ID.Text}");
                log.InputLog($"Deleting an entry Склад = {ID.Text}(Подтаблица Склады) - Clousing window | User: {User} | WareHousesWin_CR", "stable");
                DialogResult = true; this.Close();
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if(ID.Text.Length > 0 && Name.Text.Length > 0 && Address.Text.Length > 0 && Quantity.Text.Length > 0 && Total.Text.Length > 0)
            {
                if(TypeWin == "(Создание)")
                {
                    //try
                    //{
                        MsSQL.sqlPerform2($"INSERT INTO Склады(ID, Наименование, Адрес, [Количество Наименований], [Всего Товаров]) VALUES ('{ID.Text}', '{Name.Text}', '{Address.Text}', '{Quantity.Text}', '{Total.Text}')");
                        log.InputLog($"Successfully adding a record, ID = {ID.Text} - Clousing window| User: {User} | WareHousesWin_CR", "stable");
                    //}
                    //catch { log.InputLog($"Failed to create a record - Clousing window | User: {User} | WareHousesWin_CR", "Error"); }
                }
                else
                {
                    try
                    {
                        MsSQL.sqlPerform2($"UPDATE Склады SET Наименование = '{Name.Text}', Адрес = '{Address.Text}', [Количество Наименований] = '{Quantity.Text}', [Всего Товаров] = '{Total.Text}' WHERE ID = {ID.Text}");
                        log.InputLog($"Successful update of record data ID = {ID.Text} - Clousing window | User: {User} | WareHousesWin_CR", "stable");
                    }
                    catch { log.InputLog($"Failed to update the data of the table entry - Clousing window | User: {User} | WareHousesWin_CR", "Error"); }
                }
                DialogResult = true; this.Close();
            }
            else MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void BlackBack(int i)
        {
            if (i == 1) BlackBackground.Visibility = Visibility.Visible;
            else BlackBackground.Visibility = Visibility.Hidden;
        }

        public void Counting()
        {
            Quantity.Text = MsSQL.sqlPerformSUM($"SELECT COUNT(ID) FROM Склад_Подсистема WHERE Склад = {ID.Text}").ToString();
            Total.Text = MsSQL.sqlPerformSUM($"SELECT SUM(Количество) FROM Склад_Подсистема WHERE Склад = { ID.Text}").ToString();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            if(TypeWin == "(Создание)") MsSQL.sqlPerform2($"DELETE FROM Склад_Подсистема WHERE Склад = {ID.Text}");
            DialogResult = true; this.Close();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
