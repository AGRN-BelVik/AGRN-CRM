using AGRM_CRM.THEMES;
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
    /// Логика взаимодействия для SettingsWin.xaml
    /// </summary>
    public partial class SettingsWin : Window
    {
        ChangeTheme ChangeTheme = new ChangeTheme();

        public SettingsWin()
        {
            InitializeComponent();
            if (Properties.Settings.Default.SelectedTheme == "THEMES/Dark.xaml") ThemeSelectedBox.Text = "Темная";
            else ThemeSelectedBox.Text = "Светлая";
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ThemeSelectedEvent(object sender, EventArgs e)
        {
            if (ThemeSelectedBox.Text == "Темная") Properties.Settings.Default.SelectedTheme = "THEMES/Dark.xaml";
            else Properties.Settings.Default.SelectedTheme = "THEMES/Light.xaml";
            Properties.Settings.Default.Save();
            ChangeTheme.ChangeThemeApp();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}
