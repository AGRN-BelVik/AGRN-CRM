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

namespace AGRM_CRM.Table_Managment_Windows.IncomingWin
{
    /// <summary>
    /// Логика взаимодействия для SubTableIncomingWin_CR.xaml
    /// </summary>
    public partial class SubTableIncomingWin_CR : Window
    {
        string TypeWin;
        MsSQL MsSQL = new MsSQL();
        log_input log = new log_input();
        string User;
        string IDt;
        string WareHouseID;
        string LastCountWare;
        string LastWareID;
        public SubTableIncomingWin_CR(string TypeW, string txt, string user, string sID, string warehouse)
        {
            InitializeComponent();
            NameWin.Text += TypeW;
            TypeWin = TypeW;// режим работы
            ID.Text = txt;// SubID
            User = user;// USER
            IDt = sID;// приходная
            WareHouseID = MsSQL.sqlPerform4($"SELECT ID FROM Склады WHERE Наименование LIKE '{warehouse}'");
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
                try { id = Convert.ToInt16(MsSQL.sqlPerform($"SELECT MAX(SubID) FROM Приходная_Подтаблица WHERE Приходная = {IDt}")); }
                catch { }
                finally { ID.Text = Convert.ToString(id + 1); }
            }
            else
            {
                Price.Text = MsSQL.sqlPerform4($"SELECT Цена FROM Приходная_Подтаблица WHERE Приходная = {IDt} AND SubID = {ID.Text}");
                Quantity.Text = MsSQL.sqlPerform4($"SELECT Количество FROM Приходная_Подтаблица WHERE Приходная = {IDt} AND SubID = {ID.Text}");
                Total.Text = MsSQL.sqlPerform4($"SELECT Сумма FROM Приходная_Подтаблица WHERE Приходная = {IDt} AND SubID = {ID.Text}");
                string wareID = MsSQL.sqlPerform4($"SELECT Товар FROM Приходная_Подтаблица WHERE Приходная = {IDt} AND SubID = {ID.Text}");
                Ware.Text = MsSQL.sqlPerform4($"SELECT Наименование FROM Товары WHERE ID = {wareID}");
                LastCountWare = Quantity.Text;
                LastWareID = wareID;
            }
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true; this.Close();
        }

        private void Ware_DropDownClosed(object sender, EventArgs e)
        {
            if (Ware.Text != "Не выбрано" && Ware.Text.Length > 0) 
            {
                string idWare = MsSQL.sqlPerform($"SELECT DISTINCT ID FROM Товары WHERE Наименование LIKE '{Ware.Text}'");
                string price = MsSQL.sqlPerform4($"SELECT Цена FROM Закупочные_Цены WHERE Товар = {idWare} AND ID in(SELECT MAX(ID) FROM Закупочные_Цены WHERE Товар = {idWare})");
                if (price == "") price = MsSQL.sqlPerform4($"SELECT Цена FROM Закупочные_Цены WHERE Товар = {idWare})");
                Price.Text = price;
                if (Quantity.Text.Length == 0)Quantity.Text = "1"; 
                Counting();
            }
        }

        public void Counting()
        {
            int Quant = 0;
            double pr = 0;
            if (Quantity.Text.Length > 0){try { Quant = Convert.ToInt32(Quantity.Text); } catch {} }
            if (Price.Text.Length > 0)
            {
                pr = Convert.ToDouble(Price.Text);
                Total.Text = Math.Round((pr * Quant),2).ToString();
            }
        }

        private void Quantity_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!(Char.IsDigit(e.Text, 0)
               && Quantity.Text.Length != 0))
            {
                e.Handled = true;
            }
        }

        private void Quantity_TextChanged(object sender, TextChangedEventArgs e)
        {
            Counting();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var ResDel = MessageBox.Show("Вы действительно хотите удалить запись?", "Удаление записи", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (ResDel == MessageBoxResult.Yes)
            {
                MsSQL.sqlPerform2($"DELETE FROM Приходная_Подтаблица WHERE SubID = {ID.Text} AND Приходная = {IDt}");
                log.InputLog($"Deleting an entry (SubID = {ID.Text}, Заказ = {IDt})  - Clousing window | User: {User} | SubTableIncomingWin_CR", "stable");
                int QuantityAll = Convert.ToInt32(MsSQL.sqlPerform($"SELECT Количество FROM Склад_Подсистема WHERE Склад = {WareHouseID} AND Товар = {LastWareID}"));
                int Count = QuantityAll - Convert.ToInt32(LastCountWare);
                MsSQL.sqlPerform2($"UPDATE Склад_Подсистема SET Количество = {Count} WHERE Склад = {WareHouseID} AND Товар = {LastWareID}");
                DialogResult = true; this.Close();
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            int tt = Convert.ToInt32(Quantity.Text);
            if(ID.Text.Length > 0 && Ware.Text != "Не выбрано" && Ware.Text.Length > 0 && Price.Text.Length > 0 && Quantity.Text.Length > 0 && Total.Text.Length > 0 && Total.Text.Length < 14 && tt > 0)
            {
                string idWare;
                idWare = MsSQL.sqlPerform($"SELECT DISTINCT ID FROM Товары WHERE Наименование LIKE '{Ware.Text}'");
                int id = 0;
                try { id = Convert.ToInt16(MsSQL.sqlPerform($"SELECT MAX(ID) FROM Приходная_Подтаблица")); }
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
                        MsSQL.sqlPerform2($"INSERT INTO Приходная_Подтаблица(ID, Приходная, SubID, Товар, Цена, Количество, Сумма) VALUES ('{id}', '{IDt}', '{ID.Text}', '{idWare}', '{Price.Text}', '{Quantity.Text}', '{Total.Text}')");
                        log.InputLog($"Successfully adding a record, (SubID = {ID.Text}, Приходная = {IDt})  - Clousing window| User: {User} | SubTableIncomingWin_CR", "stable");
                        string test = MsSQL.sqlPerform4($"SELECT Товар FROM Склад_Подсистема WHERE Склад = {WareHouseID} AND Товар = {idWare}");
                        if (test.Length > 0)
                        {
                            int lastcount = Convert.ToInt32(MsSQL.sqlPerform($"SELECT Количество FROM Склад_Подсистема WHERE Склад = {WareHouseID} AND Товар = {idWare}"));
                            int newCount = lastcount + Convert.ToInt32(Quantity.Text);
                            MsSQL.sqlPerform2($"UPDATE Склад_Подсистема SET Количество = {newCount} WHERE Склад = {WareHouseID} AND Товар = {idWare}");
                        }
                        else
                        {
                            int idWareHouse = 0;
                            try { idWareHouse = Convert.ToInt32(MsSQL.sqlPerform("SELECT MAX(ID) FROM Склад_Подсистема")); }
                            catch { }
                            finally { idWareHouse += 1; }
                            int SubID = 0;
                            try { SubID = Convert.ToInt32(MsSQL.sqlPerform($"SELECT MAX(SubID) FROM Склад_Подсистема WHERE Склад={WareHouseID}")); }
                            catch { }
                            finally { SubID += 1; }
                            MsSQL.sqlPerform2($"INSERT INTO Склад_Подсистема(ID, SubID, Склад, Товар, Количество) VALUES ('{idWareHouse}', '{SubID}', '{WareHouseID}', '{idWare}', '{Quantity.Text}')");
                        }
                    }
                    catch { log.InputLog($"Failed to create a record - Clousing window | User: {User} | SubTableIncomingWin_CR", "Error"); }
                }
                else
                {
                    if (LastWareID == idWare && LastCountWare == Quantity.Text)
                    {
                        try
                        {
                            MsSQL.sqlPerform2($"UPDATE Приходная_Подтаблица SET Цена = '{Price.Text}', Сумма = '{Total.Text}' WHERE Приходная = {IDt} AND SubID = {ID.Text}");
                            log.InputLog($"(1)Successful update of record data (SubID = {ID.Text}, Приходная = {IDt})  - Clousing window | User: {User} | SubTableIncomingWin_CR", "stable");
                        }
                        catch { log.InputLog($"Failed to update the data of the table entry - Clousing window | User: {User} | SubTableIncomingWin_CR", "Error"); }
                    }
                    else if(LastWareID == idWare && LastCountWare != Quantity.Text)
                    {
                        MsSQL.sqlPerform2($"UPDATE Приходная_Подтаблица SET Цена = '{Price.Text}', Количество = '{Quantity.Text}', Сумма = '{Total.Text}' WHERE Приходная = {IDt} AND SubID = {ID.Text}");
                        log.InputLog($"Successful update of record data (SubID = {ID.Text}, Приходная = {IDt})  - Clousing window | User: {User} | SubTableIncomingWin_CR", "stable");
                        int QuantityAll = Convert.ToInt32(MsSQL.sqlPerform($"SELECT Количество FROM Склад_Подсистема WHERE Склад = {WareHouseID} AND Товар = {idWare}"));
                        int Count = QuantityAll - Convert.ToInt32(LastCountWare);
                        Count += Convert.ToInt32(Quantity.Text);
                        MsSQL.sqlPerform2($"UPDATE Склад_Подсистема SET Количество = {Count} WHERE Склад = {WareHouseID} AND Товар = {idWare}");
                        log.InputLog($"(2)Successful update of record data (SubID = {ID.Text}, Приходная = {IDt})  - Clousing window | User: {User} | SubTableIncomingWin_CR", "stable");
                    }
                    else
                    {
                        int QuantityAll = Convert.ToInt32(MsSQL.sqlPerform($"SELECT Количество FROM Склад_Подсистема WHERE Склад = {WareHouseID} AND Товар = {LastWareID}"));
                        int Count = QuantityAll - Convert.ToInt32(LastCountWare);
                        MsSQL.sqlPerform2($"UPDATE Склад_Подсистема SET Количество = {Count} WHERE Склад = {WareHouseID} AND Товар = {LastWareID}");
                        MsSQL.sqlPerform2($"UPDATE Приходная_Подтаблица SET Товар = '{idWare}', Цена = '{Price.Text}', Количество = '{Quantity.Text}', Сумма = '{Total.Text}' WHERE Приходная = {IDt} AND SubID = {ID.Text}");
                        string test = MsSQL.sqlPerform4($"SELECT Товар FROM Склад_Подсистема WHERE Склад = {WareHouseID} AND Товар = {idWare}");
                        if(test.Length > 0)
                        {
                            int lastcount = Convert.ToInt32(MsSQL.sqlPerform($"SELECT Количество FROM Склад_Подсистема WHERE Склад = {WareHouseID} AND Товар = {idWare}"));
                            int newCount = lastcount + Convert.ToInt32(Quantity.Text);
                            MsSQL.sqlPerform2($"UPDATE Склад_Подсистема SET Количество = {newCount} WHERE Склад = {WareHouseID} AND Товар = {idWare}");
                            log.InputLog($"(3)Successful update of record data (SubID = {ID.Text}, Приходная = {IDt})  - Clousing window | User: {User} | SubTableIncomingWin_CR", "stable");
                        }
                        else
                        {
                            int idWareHouse = 0;
                            try { idWareHouse = Convert.ToInt32(MsSQL.sqlPerform("SELECT MAX(ID) FROM Склад_Подсистема")); }
                            catch { }
                            finally { idWareHouse += 1; }
                            int SubID = 0;
                            try { SubID = Convert.ToInt32(MsSQL.sqlPerform($"SELECT MAX(SubID) FROM Склад_Подсистема WHERE Склад={WareHouseID}")); }
                            catch { }
                            finally { SubID += 1; }
                            MsSQL.sqlPerform2($"INSERT INTO Склад_Подсистема(ID, SubID, Склад, Товар, Количество) VALUES ('{idWareHouse}', '{SubID}', '{WareHouseID}', '{idWare}', '{Quantity.Text}')");
                            log.InputLog($"(5)Successful update of record data (SubID = {ID.Text}, Приходная = {IDt})  - Clousing window | User: {User} | SubTableIncomingWin_CR", "stable");
                        }
                    }
                }
                DialogResult = true; this.Close();
            }
            else MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
