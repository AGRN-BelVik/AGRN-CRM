using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AGRM_CRM.THEMES
{
    internal class ChangeTheme
    {
        public static void ChangeThemeApp()
        {
            Uri theme = new Uri(Properties.Settings.Default.SelectedTheme, UriKind.Relative);
            ResourceDictionary Theme = new ResourceDictionary() { Source = theme};  
            App.Current.Resources.Clear();
            App.Current.Resources.MergedDictionaries.Add(Theme);
        }
    }
}
