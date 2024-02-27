    using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Data;
using static System.Net.Mime.MediaTypeNames;
using AGRM_CRM.Logs;
using static System.Net.WebRequestMethods;
using File = System.IO.File;

namespace AGRM_CRM.Table_Managment_Windows
{
    /// <summary>
    /// Логика взаимодействия для wareWin_CR.xaml
    /// </summary>
    public partial class WireWin_CR : Window
    {
        string TypeWin;
        MsSQL MsSQL = new MsSQL();
        log_input log = new log_input();
        string fileName;
        byte[] image_byte;
        string GenderRed;
        string txtID;
        string User;
        string[] Man = new string[] {"S (44-46)", "M (46-48)", "L (48-50)", "XL (50-52)", "XXL (52-54)", "XXXL (54-56)", "Обувь 40", "Обувь 41", "Обувь 42", "Обувь 43", "Обувь 44", "Обувь 45"};
        string[] Woman = new string[] {"XS (40-42)", "S (42-44)", "M (44-46)", "L (46-48)", "XL (48-50)", "XXL (50-54)", "Обувь 34", "Обувь 35", "Обувь 36", "Обувь 37", "Обувь 38", "Обувь 39", "Обувь 40"};
        string[] Unisex = new string[] {"XS (42-44)", "S (44-46)", "M (46-48)", "L (48-50)", "XL (50-52)", "XXL (52-54)", "XXXL (54-56)"};
        public WireWin_CR(string TypeW, string txt, string user)
        {
            InitializeComponent();
            User = user;
            log.InputLog($"Opening a window Товары(Creating/Editing) Selected mode - {TypeW} | User: {User} | WireWin_CR", "stable");
            if (txt.Length > 0) this.txtID = txt;          
            TypeWin = TypeW;
            NameWin.Text += TypeWin;
            Launch();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            log.InputLog($"Closing the window without changes | User: {User} | WireWin_CR","stable");
            this.Close();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void wareGender_DropDownClosed(object sender, EventArgs e)
        {
            if (GenderRed != wareGender.Text)
            {
                if (wareGender.Text != "Не выбрано")
                {
                    wareSize.IsEnabled = true;
                    wareSize.Text = "Не выбрано";
                }
                try { wareSize.Items.Clear(); }
                catch { }
                if (wareGender.Text == "Мужской") { wareSize.Items.Refresh(); wareSize.ItemsSource = null; wareSize.ItemsSource = Man; }
                else if (wareGender.Text == "Женский") { wareSize.Items.Refresh(); wareSize.ItemsSource = null; wareSize.ItemsSource = Woman; }
                else if (wareGender.Text == "Унисекс") { wareSize.ItemsSource = Unisex; }
                else { wareSize.Items.Refresh(); wareSize.ItemsSource = null; wareSize.Items.Add("-"); wareSize.Text = "-"; wareSize.IsEnabled = false; }
                GenderRed = wareGender.Text;
            }
        }

        private void wareImage_Click(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Image files (*.BMP, *.JPG, *.GIF, *.TIF, *.PNG, *.ICO, *.EMF, *.WMF)|*.bmp;*.jpg;*.gif; *.tif; *.png; *.ico; *.emf; *.wmf";
            if (fileDialog.ShowDialog() == true)
            {
                image_byte = File.ReadAllBytes(fileDialog.FileName);
                fileName = fileDialog.FileName;
                wareImage.Source = new BitmapImage(new Uri(fileName));
                
            }
        }

        public void Launch()// загрузка данных
        {
            if (TypeWin == "(Создание)") // режим создание
            {
                Delete.Visibility = Visibility.Hidden;
                wareGender.Text = "Не выбрано"; 
                wareSize.IsEnabled = false; 
                wareSeason.Text = "Не выбрано";
                wareSize.Text = "Не выбрано";
                int id = 0;
                try { id = Convert.ToInt16(MsSQL.sqlPerform("SELECT MAX(ID) FROM Товары")); }
                catch{}
                finally{ ID.Text = Convert.ToString(id + 1); }
            }
            else // режим редактирование
            {
                try
                {
                    Delete.Visibility = Visibility.Visible;
                    ID.Text = txtID;
                    wareName.Text = MsSQL.sqlPerform($"SELECT Наименование FROM Товары WHERE ID = {ID.Text}");
                    wareGender.Text = MsSQL.sqlPerform($"SELECT Пол FROM Товары WHERE ID LIKE {ID.Text}");
                    GenderRed = wareGender.Text;
                    try { wareSize.Items.Clear(); }
                    catch { }
                    if (wareGender.Text == "Мужской") { wareSize.Items.Refresh(); wareSize.ItemsSource = null; wareSize.ItemsSource = Man; }
                    else if (wareGender.Text == "Женский") { wareSize.Items.Refresh(); wareSize.ItemsSource = null; wareSize.ItemsSource = Woman; }
                    else if (wareGender.Text == "Унисекс") { wareSize.ItemsSource = Unisex; }
                    else { wareSize.Items.Refresh(); wareSize.ItemsSource = null; wareSize.Items.Add("-"); wareSize.Text = "-"; wareSize.IsEnabled = false; }
                    wareSize.Text = MsSQL.sqlPerform($"SELECT Размер FROM Товары WHERE ID LIKE {ID.Text}");
                    wareComposition.Text = MsSQL.sqlPerform($"SELECT Состав FROM Товары WHERE ID LIKE {ID.Text}");
                    wareBrand.Text = MsSQL.sqlPerform($"SELECT Бренд FROM Товары WHERE ID LIKE {ID.Text}");
                    wareSeason.Text = MsSQL.sqlPerform($"SELECT Сезон FROM Товары WHERE ID LIKE 1");
                    wareCountry.Text = MsSQL.sqlPerform($"SELECT [Страна Изготовитель] FROM Товары WHERE ID LIKE {ID.Text}");
                    wareDescription.Text = MsSQL.sqlPerform($"SELECT Описание FROM Товары WHERE ID LIKE {ID.Text}");
                    image_byte = MsSQL.Sql_Image($"SELECT Изображение FROM Товары WHERE ID LIKE {ID.Text}");
                    BitmapImage bitmapImage = new BitmapImage();
                    MemoryStream memoryStream = new MemoryStream(image_byte);
                    bitmapImage.BeginInit();
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.StreamSource = memoryStream;
                    bitmapImage.EndInit();
                    wareImage.Source = bitmapImage;
                    log.InputLog($"Successful loading of data from the table, where ID={ID.Text} | User: {User} | WireWin_CR", "stable");
                }
                catch
                {
                    log.InputLog($"Data loading error - Clousing window | User: {User} | WireWin_CR", "Error");
                    DialogResult = true; this.Close();
                }
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (ID.Text.Length > 0 && wareName.Text.Length > 0 && wareName.Text.Length < 101 && wareGender.Text != "Не выбрано" && wareSize.Text != "Не выбрано" && wareSize.Text.Length > 0 && wareComposition.Text.Length > 0  && wareComposition.Text.Length < 151 && wareBrand.Text.Length > 0 && wareBrand.Text.Length < 101 && wareSeason.Text.Length > 0 && wareCountry.Text.Length > 0 && wareCountry.Text.Length < 51 && wareDescription.Text.Length > 0 && wareImage.Source != null)
            {
                if (TypeWin == "(Создание)")
                {
                    try 
                    { 
                        MsSQL.sqlImageInsert(image_byte, $"INSERT INTO Товары(ID, Наименование, Пол, Размер, Состав, Бренд, Сезон, [Страна Изготовитель], Описание, Изображение) VALUES ('{ID.Text}' , '{wareName.Text}' , '{wareGender.Text}' , '{wareSize.Text}' , '{wareComposition.Text}' , '{wareBrand.Text}' , '{wareSeason.Text}' , '{wareCountry.Text}' , '{wareDescription.Text}', (@ImageData))");
                        log.InputLog($"Successfully adding a record, ID={ID.Text} - Clousing window| User: {User} | WireWin_CR", "stable");
                    }
                    catch{ log.InputLog($"Failed to create a record - Clousing window | User: {User} | WireWin_CR", "Error"); }
                }
                else
                {
                    try 
                    { 
                        MsSQL.sqlImageInsert(image_byte, $"UPDATE Товары SET Наименование='{wareName.Text}', Пол='{wareGender.Text}', Размер='{wareSize.Text}', Состав='{wareComposition.Text}', Бренд='{wareBrand.Text}', Сезон='{wareSeason.Text}', [Страна Изготовитель]='{wareCountry.Text}', Описание='{wareDescription.Text}', Изображение=(@ImageData) WHERE ID = {ID.Text}");
                        log.InputLog($"Successful update of record data ID = {ID.Text} - Clousing window | User: {User} | WireWin_CR", "stable");
                    }
                    catch { log.InputLog($"Failed to update the data of the table entry - Clousing window | User: {User} | WireWin_CR", "Error"); }
                }
                DialogResult = true; this.Close();
            }
            else MessageBox.Show("Заполните все поля!","Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var ResDel = MessageBox.Show("Вы действительно хотите удалить запись?", "Удаление записи", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (ResDel == MessageBoxResult.Yes)
            {
                MsSQL.sqlPerform2($"DELETE FROM Товары WHERE ID = {ID.Text}");
                log.InputLog($"Deleting an entry ID= {ID.Text} - Clousing window | User: {User} | WireWin_CR", "stable");
                DialogResult = true; this.Close();
            }
        }

        private void wareSize_DropDownClosed(object sender, EventArgs e)
        {

        }
    }
}
