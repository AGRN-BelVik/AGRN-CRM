using AGRM_CRM.Logs;
using AGRM_CRM.Table_Managment_Windows;
using AGRM_CRM.THEMES;
using System;
using System.Collections.Generic;
using System.Globalization;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.AvalonDock.Themes;


namespace AGRM_CRM
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 

            

    public partial class MainWindow : Window
    {
        log_input log = new log_input();
        MsSQL MsSQL = new MsSQL();
        public MainWindow()
        {
            ChangeTheme.ChangeThemeApp();
            InitializeComponent();
            Language.Text = InputLanguageManager.Current.CurrentInputLanguage.ToString().Substring(3);
            PasswordBox.Visibility = Visibility.Hidden;
            ClosePassword.Visibility = Visibility.Hidden;
            log.InputLog("Open | MainWindow LOGIN","stable");
            List<string> styles = new List<string> { "light", "dark" };
            SQLConnectionTest();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
            log.InputLog("Close | MainWindow LOGIN", "stable");
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (this.WindowState.Equals(WindowState.Maximized))
                {
                    this.Left = 0;
                    this.Top = 0;
                    this.WindowStartupLocation = WindowStartupLocation.Manual;
                    this.WindowState = WindowState.Normal;
                }
                DragMove();
            }
        }

        private void Password_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (Password.Visibility == Visibility.Visible) PasswordBox.Text = ReversePass(Password.Password.ToString());          
        }

        private void OpenPassword_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)//показать пароль
        {
            OpenPassword.Visibility = Visibility.Hidden;
            Password.Visibility = Visibility.Hidden;
            ClosePassword.Visibility = Visibility.Visible;
            PasswordBox.Visibility = Visibility.Visible;
        }

        private void ClosePassword_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)//скрать пароль
        {
            ClosePassword.Visibility = Visibility.Hidden;
            PasswordBox.Visibility = Visibility.Hidden;
            OpenPassword.Visibility = Visibility.Visible;
            Password.Visibility = Visibility.Visible;
        }

        private void PasswordBox_Changed(object sender, TextChangedEventArgs e)//заполнение Текстбокса при заполнении пасвордбокса
        {
            if (ErrorTxt.Visibility == Visibility.Visible) ErrorTxt.Visibility = Visibility.Hidden;
            if (PasswordBox.Visibility == Visibility.Visible) Password.Password = PasswordBox.Text;
        }

        private void LoginBut_Click(object sender, RoutedEventArgs e)//кнопка "Войти"
        {
            string Test = MsSQL.sqlPerform4($"SELECT Role FROM USERS WHERE Login = '{Login.Text}' AND Password = '{PasswordBox.Text}'");
            if (Test.Length > 0)
            {
                Password.Password = "";
                this.Hide();
                CRM_Main_Win CRM = new CRM_Main_Win(Login.Text, Test);
                CRM.ShowDialog();
                if (CRM.DialogResult == true) this.Show();
                else Application.Current.Shutdown();
            }
            else
            {
                if (ErrorTxt.Text.Length < 10) ErrorTxt.Text += " Логин или пароль введены не верно\nПопробуйте снова или обратитесь к администратору";
                ErrorTxt.Visibility = Visibility.Visible;
                log.InputLog("Failed authorization attempt | LoginBut_Click (83-97) | MainWindow LOGIN", "Error");
            }
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)//Обновление раскладки по нажатию любой клавиши
        {
            Language.Text = InputLanguageManager.Current.CurrentInputLanguage.ToString().Substring(3);
        }

        private void Window_PreviewMouseMove(object sender, MouseEventArgs e)//Обновление раскладки по движению мыши в окне
        {
            Language.Text = InputLanguageManager.Current.CurrentInputLanguage.ToString().Substring(3);
        }

        public string ReversePass(string Pass)
        {
            char[] ass = Pass.ToCharArray();
            return new string(ass);
        }

        public void SQLConnectionTest()
        {
            if (MsSQL.SQLConnectionTest() == true) 
            {
                log.InputLog($"Connection to the database has been completed: {MsSQL.SQLConnectionString()}", "stable");
                Login.IsEnabled = true;
                Password.IsEnabled = true;
                OpenPassword.IsEnabled = true;
                ClosePassword.IsEnabled = true;
                LoginBut.IsEnabled = true;
                if(ErrorTxt.Visibility == Visibility.Visible) ErrorTxt.Visibility = Visibility.Hidden;
            }
            else
            {
                log.InputLog($"Сouldn't connect to the database: {MsSQL.SQLConnectionString()}", "Error");
                Login.IsEnabled = false;
                Password.IsEnabled = false;
                OpenPassword.IsEnabled = false;
                ClosePassword.IsEnabled = false;
                LoginBut.IsEnabled = false;
                if(ErrorTxt.Text.Length < 10) ErrorTxt.Text += " Не получилось подключится к серверу";
                ErrorTxt.Visibility = Visibility.Visible;  
            }
        }

        private void LoginText_Changed(object sender, TextChangedEventArgs e)
        {
            if (ErrorTxt.Visibility == Visibility.Visible) ErrorTxt.Visibility = Visibility.Hidden; 
        }

        private void SettingsClick(object sender, RoutedEventArgs e)
        {
            SettingsWin settings = new SettingsWin();
            settings.Show();
        }
    }
}
