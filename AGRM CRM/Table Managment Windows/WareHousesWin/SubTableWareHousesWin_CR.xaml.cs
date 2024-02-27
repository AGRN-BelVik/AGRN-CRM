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

namespace AGRM_CRM.Table_Managment_Windows.WareHousesWin
{
    /// <summary>
    /// Логика взаимодействия для SubTableWareHousesWin_CR.xaml
    /// </summary>
    public partial class SubTableWareHousesWin_CR : Window
    {
        string TypeWin;
        MsSQL MsSQL = new MsSQL();
        log_input log = new log_input();
        string User;
        string IDt;

        public SubTableWareHousesWin_CR(string TypeW, string txt, string user, string sID)
        {
            InitializeComponent();
            TypeWin = TypeW;// режим работы
            ID.Text = txt;// SubID
            NameWin.Text += TypeW;
            User = user;// USER
            IDt = sID;// Склад
            Launch();
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
                try { id = Convert.ToInt32(MsSQL.sqlPerform($"SELECT MAX(SubID) FROM Склад_Подсистема WHERE Склад = {IDt}")); }
                catch { }
                finally { ID.Text = Convert.ToString(id + 1); }
            }
            else
            {
                Ware.Text = MsSQL.sqlPerform($"SELECT Товар FROM Склад_ПодсистемаВид WHERE ID = {ID.Text} AND Склад = {IDt}");
                Quantity.Text = MsSQL.sqlPerform($"SELECT Количество FROM Склад_Подсистема WHERE SubID = {ID.Text} AND Склад = {IDt}");
            }
        }

        private void QuantityTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!(Char.IsDigit(e.Text, 0))
               && Quantity.Text.Length != 0)
            {
                e.Handled = true;
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true; this.Close();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var ResDel = MessageBox.Show("Вы действительно хотите удалить запись?", "Удаление записи", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (ResDel == MessageBoxResult.Yes)
            {
                MsSQL.sqlPerform2($"DELETE FROM Склад_Подсистема WHERE SubID = {ID.Text} AND Склад = {IDt}");
                log.InputLog($"Deleting an entry (SubID = {ID.Text}, Склад = {IDt}) - Clousing window | User: {User} | SubTableWareHousesWin_CR", "stable");
                DialogResult = true; this.Close();
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if(ID.Text.Length > 0 && Ware.Text.Length > 0 && Ware.Text != "Не выбрано" && Quantity.Text.Length > 0)
            {
                string idWare = MsSQL.sqlPerform($"SELECT ID FROM Товары WHERE Наименование LIKE '{Ware.Text}'");
                int id = 0;
                try { id = Convert.ToInt32(MsSQL.sqlPerform($"SELECT MAX(ID) FROM Склад_Подсистема")); }
                catch { }
                finally { id += 1; }
                if (TypeWin == "(Создание)")
                {
                    try
                    {
                        MsSQL.sqlPerform2($"INSERT INTO Склад_Подсистема(ID, SubID, Склад, Товар, Количество) VALUES ('{id}', '{ID.Text}', '{IDt}', '{idWare}', '{Quantity.Text}')");
                        log.InputLog($"Successfully adding a record, (SubID = {ID.Text}, Склад = {IDt})  - Clousing window| User: {User} | SubTableWareHousesWin_CR", "stable");
                    }
                    catch { log.InputLog($"Failed to create a record - Clousing window | User: {User} | SubTableWareHousesWin_CR", "Error"); }
                }
                else
                {
                    try
                    {
                        MsSQL.sqlPerform2($"UPDATE Склад_Подсистема SET Товар = '{idWare}', Количество = '{Quantity.Text}' WHERE SubID = {ID.Text} AND Склад = {IDt}");
                        log.InputLog($"Successful update of record data (SubID = {ID.Text}, Склад = {IDt})  - Clousing window | User: {User} | SubTableWareHousesWin_CR", "stable");
                    }
                    catch { log.InputLog($"Failed to update the data of the table entry - Clousing window | User: {User} | SubTableWareHousesWin_CR", "Error"); }
                }
                DialogResult = true; this.Close();
            }
            else MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
