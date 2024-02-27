using AGRM_CRM.Logs;
using AGRM_CRM.Table_Managment_Windows;
using AGRM_CRM.Table_Managment_Windows.ExpenditureWin;
using AGRM_CRM.Table_Managment_Windows.IncomingWin;
using AGRM_CRM.Table_Managment_Windows.PaymentsWin;
using AGRM_CRM.Table_Managment_Windows.WareHousesWin;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AGRM_CRM
{
    public partial class CRM_Main_Win : Window
    {
        // переменные и подключение классов
        log_input log = new log_input();
        MsSQL MsSQL = new MsSQL();
        DataTable Table = new DataTable($"");
        string user;
        string Role;

        // Запуск и инициализация, подгрузка данных в таблицы
        public CRM_Main_Win(string User, string role)
        {
            InitializeComponent();
            Role = role;
            if (role == "admin")
            { }
            else if (role == "seller")
            {
                Ware.Visibility = Visibility.Collapsed;
                Clients_Subsection.Visibility = Visibility.Collapsed;
                Workers.Visibility = Visibility.Collapsed;
                WareHouses.Visibility = Visibility.Collapsed;
            }
            user = User;
            UserTxt.Text = user;
            FilterWare.Text = "Не выбрано";
            FilterPurchasePrices.Text = "Не выбрано";
            FilterSalePrices.Text = "Не выбрано";
            FilterOrders.Text = "Не выбрано";
            FilterClients.Text = "Не выбрано";
            FilterWorkers.Text = "Не выбрано";
            FilterPositions.Text = "Не выбрано";
            FilterPayments.Text = "Не выбрано";
            FilterWareHouses.Text = "Не выбрано";
            FilterComings.Text = "Не выбрано";
            FilterExpenditure.Text = "Не выбрано";
            FilterRegister.Text = "Не выбрано";
            log.InputLog($"Successful authorization, launch of the main window of the CRM system | User: {user} | CRM_Main_Win", "stable");
            DateBefore.Text = DateTime.Now.ToString();
            DateFrom.Text = DateTime.Now.AddDays(-31).ToString();
        }
        

        #region Панель управления окном
        // кнопка закрыть
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            log.InputLog($"Close | User: {user} | CRM_Main_Win", "stable");
            this.Close();
        }

        // кнопка полный экран
        private void Full_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState.Equals(WindowState.Maximized))
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }

        //  Кнопка скрыть
        private void Hide_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        #endregion

        #region перетаскивание окна
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (this.WindowState.Equals(WindowState.Maximized))
                {
                    this.Left = (Mouse.GetPosition(null).X / 2);
                    this.Top = (Mouse.GetPosition(null).Y / 4);
                    this.WindowStartupLocation = WindowStartupLocation.Manual;
                    this.WindowState = WindowState.Normal;
                }
                DragMove();
            }
        }
        #endregion

        #region показать/скрыть подсказку, активизация поиска
        private void Seartch_TextChanged(object sender, TextChangedEventArgs e)
        {
            Search();
        }
        #endregion

        #region отчиска поискового поля
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            // Товары - Товары
            if (Ware.IsSelected == true && Ware_Subsection.IsSelected == true) WareTxt.Text = "";
            // Товары - Закупочные цены
            if (Ware.IsSelected == true && PurchasePrices_Subsection.IsSelected == true) PurchasePricesTxt.Text = "";
            // Товары - Цены продажи
            if (Ware.IsSelected == true && SalePrices_Subsection.IsSelected == true) SalePricesTxt.Text = "";
            // Заказы и клиенты - Клиенты
            if (Orders.IsSelected == true && Clients_Subsection.IsSelected == true) ClientsTxt.Text = "";
            // Заказы и клиенты - Заказы
            if (Orders.IsSelected == true && Orders_Subsection.IsSelected == true) OrdersTxt.Text = "";
            // Сотрудники - Сотрудники
            if (Workers.IsSelected == true && Workers_Subsection.IsSelected == true) WorkersTxt.Text = "";
            // Сотрудники - Должности
            if (Workers.IsSelected == true && Positions_Subsection.IsSelected == true) PositionsTxt.Text = "";
            // Сотрудники - Выплата ЗП
            if (Workers.IsSelected == true && Payments_Subsection.IsSelected == true) PaymentsTxt.Text = "";
            // Склады и наличее
            if (WareHouses.IsSelected == true) WareHousesTxt.Text = "";
            // Докусенты - Приходные
            if (Documents.IsSelected == true && Comings_Subsection.IsSelected == true) ComingsTxt.Text = "";
            // Документы - Расходные 
            if (Documents.IsSelected == true && Expenditure_Subsection.IsSelected == true) ExpenditureTxt.Text = "";
            // Документы - Регистр накоплений
            if (Documents.IsSelected == true && Register_Subsection.IsSelected == true) RegisterTxt.Text = "";
        }
        #endregion

        #region кнопка создания записи
        private void Click_Create(object sender, RoutedEventArgs e)
        {
            if (Ware.IsSelected == true && Ware_Subsection.IsSelected == true)// создание - Товары
            {
                WireWin_CR WireWinCR = new WireWin_CR("(Создание)", "", user);
                BlackBackground(1); WireWinCR.ShowDialog();
                if (WireWinCR.DialogResult.HasValue && WireWinCR.DialogResult.HasValue)
                {
                    Search(); BlackBackground(0); WareTxt.Text = "";
                }
            }
            if (Ware.IsSelected == true && PurchasePrices_Subsection.IsSelected == true)// создание - Закупочные цены
            {
                PurchasePricesWin_CR win = new PurchasePricesWin_CR("(Создание)", "", user);
                BlackBackground(1); win.ShowDialog();
                if (win.DialogResult.HasValue && win.DialogResult.HasValue)
                {
                    Search(); BlackBackground(0); PurchasePricesTxt.Text = "";
                }
            }
            if (Ware.IsSelected == true && SalePrices_Subsection.IsSelected == true)// создание - Цены продажи
            {
                SalePricesWin_CR win = new SalePricesWin_CR("(Создание)", "", user);
                BlackBackground(1); win.ShowDialog();
                if (win.DialogResult.HasValue && win.DialogResult.HasValue)
                {
                    Search(); BlackBackground(0); PurchasePricesTxt.Text = "";
                }
            }
            if (Orders.IsSelected == true && Clients_Subsection.IsSelected == true)// создание - Клиенты
            {
                ClientsWin_CR win = new ClientsWin_CR("(Создание)", "", user);
                BlackBackground(1); win.ShowDialog();
                if (win.DialogResult.HasValue && win.DialogResult.HasValue)
                {
                    Search(); BlackBackground(0); ClientsTxt.Text = "";
                }
            }
            if (Orders.IsSelected == true && Orders_Subsection.IsSelected == true)// создание - Заказы
            {
                OrdersWin_CR win = new OrdersWin_CR("(Создание)", "", user);
                BlackBackground(1); win.ShowDialog();
                if (win.DialogResult.HasValue && win.DialogResult.HasValue)
                {
                    Search(); BlackBackground(0); OrdersTxt.Text = "";
                }
            }
            if (Workers.IsSelected == true && Positions_Subsection.IsSelected == true)// Сотрудники - Должности
            {
                PositionsWin_CR win = new PositionsWin_CR("(Создание)", "", user);
                BlackBackground(1); win.ShowDialog();
                if (win.DialogResult.HasValue && win.DialogResult.HasValue)
                {
                    Search(); BlackBackground(0); PositionsTxt.Text = "";
                }
            }
            if (Workers.IsSelected == true && Workers_Subsection.IsSelected == true)// Сотрудники - Сотрудники
            {
                WorkersWin_CR win = new WorkersWin_CR("(Создание)", "", user);
                BlackBackground(1); win.ShowDialog();
                if (win.DialogResult.HasValue && win.DialogResult.HasValue)
                {
                    Search(); BlackBackground(0); WorkersTxt.Text = "";
                }
            }
            if (Workers.IsSelected == true && Payments_Subsection.IsSelected == true)// Сотрудники - Выплата ЗП
            {
                PaymentsWin_CR win = new PaymentsWin_CR("(Создание)", "", user);
                BlackBackground(1); win.ShowDialog();
                if (win.DialogResult.HasValue && win.DialogResult.HasValue)
                {
                    Search(); BlackBackground(0); PaymentsTxt.Text = "";
                }
            }
            if (WareHouses.IsSelected == true)// Склады и наличее
            {
                WareHousesWin_CR win = new WareHousesWin_CR("(Создание)", "", user);
                BlackBackground(1); win.ShowDialog();
                {
                    Search(); BlackBackground(0); WareHousesTxt.Text = "";
                }
            }
            if (Documents.IsSelected == true && Comings_Subsection.IsSelected == true)// Приходные
            {
                IncomingWin_CR win = new IncomingWin_CR("(Создание)", "", user);
                BlackBackground(1); win.ShowDialog();
                if (win.DialogResult.HasValue && win.DialogResult.HasValue)
                {
                    Search(); BlackBackground(0); ComingsTxt.Text = "";
                }
            }
            if (Documents.IsSelected == true && Expenditure_Subsection.IsSelected == true)// Расходные
            {
                ExpenditureWin_CR win = new ExpenditureWin_CR("(Создание)", "", user);
                BlackBackground(1); win.ShowDialog();
                if (win.DialogResult.HasValue && win.DialogResult.HasValue)
                {
                    Search(); BlackBackground(0); ExpenditureTxt.Text = "";
                }
            }
            if (Statement.IsSelected == true)// отчёт
            {
                if (DateFrom.Text.Length == 10 && DateBefore.Text.Length == 10)
                {
                    if (DateFrom.SelectedDate < DateBefore.SelectedDate)
                    {
                        DataStatement.ItemsSource = MsSQL.DataGridUpdate($"WHERE Дата BETWEEN CONVERT(Date, '{DateFrom.Text}') AND CONVERT(Date, '{DateBefore.Text}')", "Регистр_НакопленийВид").DefaultView;
                        double Income = MsSQL.sqlPerformSUM($"SELECT SUM(Сумма) FROM Регистр_НакопленийВид WHERE Дата BETWEEN CONVERT(Date, '{DateFrom.Text}') AND CONVERT(Date, '{DateBefore.Text}') AND [Тип Транзакции] = 'Приход'");
                        double Expend = MsSQL.sqlPerformSUM($"SELECT SUM(Сумма) FROM Регистр_НакопленийВид WHERE Дата BETWEEN CONVERT(Date, '{DateFrom.Text}') AND CONVERT(Date, '{DateBefore.Text}') AND [Тип Транзакции] = 'Расход'");
                        StatementTotalText.Text = Convert.ToString(Expend - Income);
                    }
                    else MessageBox.Show("Поле \"С:\" Должно быть меньше поля \"По:\"", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else MessageBox.Show("Заполните поля дат для формирования отчёта!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        #region открытие редактора записи
        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (Role != "seller")
            {
                var row = sender as DataGridRow;// получение номера строки
                TextBlock id;
                if (Ware.IsSelected == true && Ware_Subsection.IsSelected == true)// редактор - Товары
                {
                    id = DataWare.Columns[0].GetCellContent(row) as TextBlock;
                    WireWin_CR WireWin_CR = new WireWin_CR("(Редактирование)", id.Text, user);
                    BlackBackground(1); WireWin_CR.ShowDialog();
                    if (WireWin_CR.DialogResult.HasValue && WireWin_CR.DialogResult.HasValue)
                    {
                        Search(); BlackBackground(0); WareTxt.Text = "";
                    }
                }
                if (Ware.IsSelected == true && PurchasePrices_Subsection.IsSelected == true)// редактор - Закупочные цены
                {
                    id = DataPurchasePrices.Columns[0].GetCellContent(row) as TextBlock;
                    PurchasePricesWin_CR win = new PurchasePricesWin_CR("(Редактирование)", id.Text, user);
                    BlackBackground(1); win.ShowDialog();
                    if (win.DialogResult.HasValue && win.DialogResult.HasValue)
                    {
                        Search(); BlackBackground(0); PurchasePricesTxt.Text = "";
                    }
                }
                if (Ware.IsSelected == true && SalePrices_Subsection.IsSelected == true)// редактор - Цены продажи
                {
                    id = DataSalePrices.Columns[0].GetCellContent(row) as TextBlock;
                    SalePricesWin_CR win = new SalePricesWin_CR("(Редактирование)", id.Text, user);
                    BlackBackground(1); win.ShowDialog();
                    if (win.DialogResult.HasValue && win.DialogResult.HasValue)
                    {
                        Search(); BlackBackground(0); SalePricesTxt.Text = "";
                    }
                }
                if (Orders.IsSelected == true && Clients_Subsection.IsSelected == true)// редактор Клиенты
                {
                    id = DataClients.Columns[0].GetCellContent(row) as TextBlock;
                    ClientsWin_CR win = new ClientsWin_CR("(Редактирование)", id.Text, user);
                    BlackBackground(1); win.ShowDialog();
                    if (win.DialogResult.HasValue && win.DialogResult.HasValue)
                    {
                        Search(); BlackBackground(0); ClientsTxt.Text = "";
                    }
                }
                if (Orders.IsSelected == true && Orders_Subsection.IsSelected == true)// редактор Заказы
                {
                    id = DataOrders.Columns[0].GetCellContent(row) as TextBlock;
                    OrdersWin_CR win = new OrdersWin_CR("(Редактирование)", id.Text, user);
                    BlackBackground(1); win.ShowDialog();
                    if (win.DialogResult.HasValue && win.DialogResult.HasValue)
                    {
                        Search(); BlackBackground(0); OrdersTxt.Text = "";
                    }
                }
                if (Workers.IsSelected == true && Positions_Subsection.IsSelected == true)// редактор Должности
                {
                    id = DataPositions.Columns[0].GetCellContent(row) as TextBlock;
                    PositionsWin_CR win = new PositionsWin_CR("(Редактирование)", id.Text, user);
                    BlackBackground(1); win.ShowDialog();
                    if (win.DialogResult.HasValue && win.DialogResult.HasValue)
                    {
                        Search(); BlackBackground(0); PositionsTxt.Text = "";
                    }
                }
                if (Workers.IsSelected == true && Workers_Subsection.IsSelected == true)// редактор Сотрудники
                {
                    id = DataWorkers.Columns[0].GetCellContent(row) as TextBlock;
                    WorkersWin_CR win = new WorkersWin_CR("(Редактирование)", id.Text, user);
                    BlackBackground(1); win.ShowDialog();
                    if (win.DialogResult.HasValue && win.DialogResult.HasValue)
                    {
                        Search(); BlackBackground(0); WorkersTxt.Text = "";
                    }
                }
                if (Workers.IsSelected == true && Payments_Subsection.IsSelected == true)// редактор Выплата ЗП
                {
                    id = DataPayments.Columns[0].GetCellContent(row) as TextBlock;
                    PaymentsWin_CR win = new PaymentsWin_CR("(Редактирование)", id.Text, user);
                    BlackBackground(1); win.ShowDialog();
                    if (win.DialogResult.HasValue && win.DialogResult.HasValue)
                    {
                        Search(); BlackBackground(0); PaymentsTxt.Text = "";
                    }
                }
                if (WareHouses.IsSelected == true)// Склады и наличее
                {
                    id = DataWareHouses.Columns[0].GetCellContent(row) as TextBlock;
                    WareHousesWin_CR win = new WareHousesWin_CR("(Редактирование)", id.Text, user);
                    BlackBackground(1); win.ShowDialog();
                    if (win.DialogResult.HasValue && win.DialogResult.HasValue)
                    {
                        Search(); BlackBackground(0); WareHousesTxt.Text = "";
                    }
                }
                if (Documents.IsSelected == true && Comings_Subsection.IsSelected == true)// Приходные
                {
                    id = DataComings.Columns[0].GetCellContent(row) as TextBlock;
                    IncomingWin_CR win = new IncomingWin_CR("(Редактирование)", id.Text, user);
                    BlackBackground(1); win.ShowDialog();
                    if (win.DialogResult.HasValue && win.DialogResult.HasValue)
                    {
                        Search(); BlackBackground(0); ComingsTxt.Text = "";
                    }
                }
                if (Documents.IsSelected == true && Expenditure_Subsection.IsSelected == true)// Расходные
                {
                    id = DataExpenditure.Columns[0].GetCellContent(row) as TextBlock;
                    ExpenditureWin_CR win = new ExpenditureWin_CR("(Редактирование)", id.Text, user);
                    BlackBackground(1); win.ShowDialog();
                    if (win.DialogResult.HasValue && win.DialogResult.HasValue)
                    {
                        Search(); BlackBackground(0); ExpenditureTxt.Text = "";
                    }
                }
                if ((Documents.IsSelected == true && Register_Subsection.IsSelected == true) || Statement.IsSelected == true)// Регистр накопления
                {
                    if (Documents.IsSelected == true && Register_Subsection.IsSelected == true) id = DataRegister.Columns[1].GetCellContent(row) as TextBlock;
                    else id = DataStatement.Columns[1].GetCellContent(row) as TextBlock;
                    if (id.Text == "Приход")
                    {
                        if (Documents.IsSelected == true && Register_Subsection.IsSelected == true) id = DataRegister.Columns[2].GetCellContent(row) as TextBlock;
                        else id = DataStatement.Columns[2].GetCellContent(row) as TextBlock;
                        IncomingWin_CR win = new IncomingWin_CR("(Редактирование)", id.Text, user);
                        BlackBackground(1); win.ShowDialog();
                        if (win.DialogResult.HasValue && win.DialogResult.HasValue)
                        {
                            BlackBackground(0);
                        }
                    }
                    else
                    {
                        if (Documents.IsSelected == true && Register_Subsection.IsSelected == true) id = DataRegister.Columns[2].GetCellContent(row) as TextBlock;
                        else id = DataStatement.Columns[2].GetCellContent(row) as TextBlock;
                        ExpenditureWin_CR win = new ExpenditureWin_CR("(Редактирование)", id.Text, user);
                        BlackBackground(1); win.ShowDialog();
                        if (win.DialogResult.HasValue && win.DialogResult.HasValue)
                        {
                            BlackBackground(0);
                        }
                    }
                    if (Documents.IsSelected == true && Register_Subsection.IsSelected == true) DataRegister.ItemsSource = MsSQL.DataGridUpdate("", "Регистр_НакопленийВид").DefaultView;
                    else DataStatement.ItemsSource = MsSQL.DataGridUpdate($"WHERE Дата BETWEEN CONVERT(Date, '{DateFrom.Text}') AND CONVERT(Date, '{DateBefore.Text}')", "Регистр_НакопленийВид").DefaultView;
                }
            }
        }
        #endregion

        #region Обновление таблицы выбранной категории
        private void Restart_Click(object sender, RoutedEventArgs e)
        {
            Search();
        }
        #endregion

        #region Затемнение родительского окна при открытии дочернего окна
        public void BlackBackground(int n)// затемнение главного окна
        {
            if (n == 1) { Blackout.Visibility = Visibility.Visible; Blackout2.Visibility = Visibility.Visible; }
            else { Blackout.Visibility = Visibility.Hidden; Blackout2.Visibility = Visibility.Hidden; }
        }
        #endregion

        #region Включение фильтра для поиска
        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            // Товары - Товары
            if (CheckWare.IsChecked == true) FilterWare.IsEnabled = true;
            else { FilterWare.IsEnabled = false; FilterWare.Text = "Не выбрано"; }
            // Товары- Закупочные цены
            if (CheckPurchasePrices.IsChecked == true) FilterPurchasePrices.IsEnabled = true;
            else { FilterPurchasePrices.IsEnabled = false; FilterPurchasePrices.Text = "Не выбрано"; }
            // Товары - Цены продажи
            if (CheckSalePrices.IsChecked == true) FilterSalePrices.IsEnabled = true;
            else { FilterSalePrices.IsEnabled = false; FilterSalePrices.Text = "Не выбрано"; }
            // Заказы и клиенты - Заказы
            if (CheckOrders.IsChecked == true) FilterOrders.IsEnabled = true;
            else { FilterOrders.IsEnabled = false; FilterOrders.Text = "Не выбрано"; }
            // Заказы и клиенты - Клиенты
            if (CheckClients.IsChecked == true) FilterClients.IsEnabled = true;
            else { FilterClients.IsEnabled = false; FilterClients.Text = "Не выбрано"; }
            // Сотрудники - Должности
            if (CheckPositions.IsChecked == true) FilterPositions.IsEnabled = true;
            else { FilterPositions.IsEnabled = false; FilterPositions.Text = "Не выбрано"; }
            // Сотрудники - Сотрудники
            if (CheckWorkers.IsChecked == true) FilterWorkers.IsEnabled = true;
            else { FilterWorkers.IsEnabled = false; FilterWorkers.Text = "Не выбрано"; }
            // Сотрудники - Выплата ЗП
            if (CheckPayments.IsChecked == true) FilterPayments.IsEnabled = true;
            else { FilterPayments.IsEnabled = false; FilterPayments.Text = "Не выбрано"; }
            // Склады и наличее
            if (CheckWareHouses.IsChecked == true) FilterWareHouses.IsEnabled = true;
            else { FilterWareHouses.IsEnabled = false; FilterWareHouses.Text = "Не выбрано"; }
            // Документы - Приходные
            if (CheckComings.IsChecked == true) FilterComings.IsEnabled = true;
            else { FilterComings.IsEnabled = false; FilterComings.Text = "Не выбрано"; }
            // Документы - Расходные
            if (CheckExpenditure.IsChecked == true) FilterExpenditure.IsEnabled = true;
            else { FilterExpenditure.IsEnabled = false; FilterExpenditure.Text = "Не выбрано"; }
            // Документы - Регистр накоплений
            if (CheckRegister.IsChecked == true) FilterRegister.IsEnabled = true;
            else { FilterRegister.IsEnabled = false; FilterRegister.Text = "Не выбрано"; }
            Search();
        }
        #endregion

        #region Осуществление поиска
        public void Search()
        {
            // Товары - Товары
            if (Ware.IsSelected == true && Ware_Subsection.IsSelected == true && WareTxt.Text.Length > 0)
            {
                WareHint.Visibility = Visibility.Hidden;
                if (FilterWare.IsEnabled == true) DataWare.ItemsSource = MsSQL.DataGridUpdate($"WHERE [{FilterWare.Text}] LIKE '%{WareTxt.Text}%'", "ТоварыВид").DefaultView;
                else DataWare.ItemsSource = MsSQL.DataGridUpdate($"WHERE ID LIKE '%{WareTxt.Text}%' OR Наименование LIKE '%{WareTxt.Text}%' OR Пол LIKE '%{WareTxt.Text}%' OR Размер LIKE '%{WareTxt.Text}%' OR Состав LIKE '%{WareTxt.Text}%' OR Бренд LIKE '%{WareTxt.Text}%' OR Сезон LIKE '%{WareTxt.Text}%' OR [Страна Изготовитель] LIKE '%{WareTxt.Text}%' OR Описание LIKE '%{WareTxt.Text}%'", "ТоварыВид").DefaultView;
            }
            else if (Ware.IsSelected == true && Ware_Subsection.IsSelected == true && WareTxt.Text.Length == 0)
            {
                WareHint.Visibility = Visibility.Visible; DataWare.ItemsSource = MsSQL.DataGridUpdate("", "ТоварыВид").DefaultView;
            }
            // Товары - Закупочные цены
            if (Ware.IsSelected == true && PurchasePrices_Subsection.IsSelected == true && PurchasePricesTxt.Text.Length > 0)
            {
                PurchasePricesHint.Visibility = Visibility.Hidden;
                if (FilterPurchasePrices.IsEnabled == true) DataPurchasePrices.ItemsSource = MsSQL.DataGridUpdate($"WHERE [{FilterPurchasePrices.Text}] LIKE '%{PurchasePricesTxt.Text}%'", "Закупочные_ЦеныВид").DefaultView;
                else DataPurchasePrices.ItemsSource = MsSQL.DataGridUpdate($"WHERE ID LIKE '%{PurchasePricesTxt.Text}%' OR Товар LIKE '%{PurchasePricesTxt.Text}%' OR Цена LIKE '%{PurchasePricesTxt.Text}%'", "Закупочные_ЦеныВид").DefaultView;
            }
            else if (Ware.IsSelected == true && PurchasePrices_Subsection.IsSelected == true && PurchasePricesTxt.Text.Length == 0)
            {
                PurchasePricesHint.Visibility = Visibility.Visible; DataPurchasePrices.ItemsSource = MsSQL.DataGridUpdate($"", "Закупочные_ЦеныВид").DefaultView;
            }
            // Товары - Цены продажи
            if (Ware.IsSelected == true && SalePrices_Subsection.IsSelected == true && SalePricesTxt.Text.Length > 0)
            {
                SalePricesHint.Visibility = Visibility.Hidden;
                if (FilterSalePrices.IsEnabled == true) DataSalePrices.ItemsSource = MsSQL.DataGridUpdate($"WHERE [{FilterSalePrices.Text}] LIKE '%{SalePricesTxt.Text}%'", "Цены_ПродажиВид").DefaultView;
                else DataSalePrices.ItemsSource = MsSQL.DataGridUpdate($"WHERE ID LIKE '%{SalePricesTxt.Text}%' OR Товар LIKE '%{SalePricesTxt.Text}%' OR Цена LIKE '%{SalePricesTxt.Text}%'", "Цены_ПродажиВид").DefaultView;
            }
            else if (Ware.IsSelected == true && SalePrices_Subsection.IsSelected == true && SalePricesTxt.Text.Length == 0)
            {
                SalePricesHint.Visibility = Visibility.Visible; DataSalePrices.ItemsSource = MsSQL.DataGridUpdate($"", "Цены_ПродажиВид").DefaultView;
            }
            // Заказы и клиенты - Клиенты
            if (Orders.IsSelected == true && Clients_Subsection.IsSelected == true && ClientsTxt.Text.Length > 0)
            {
                ClientsHint.Visibility = Visibility.Hidden;
                if (FilterClients.IsEnabled == true) DataClients.ItemsSource = MsSQL.DataGridUpdate($"WHERE [{FilterClients.Text}] LIKE '%{ClientsTxt.Text}%' AND ID <> 0", "Клиенты").DefaultView;
                else DataClients.ItemsSource = MsSQL.DataGridUpdate($"WHERE (ID LIKE '%{ClientsTxt.Text}%' OR Фамилия LIKE '%{ClientsTxt.Text}%' OR Имя LIKE '%{ClientsTxt.Text}%' OR Отчество LIKE '%{ClientsTxt.Text}%' OR [Дата Рождения] LIKE '%{ClientsTxt.Text}%' OR Телефон LIKE '%{ClientsTxt.Text}%' OR [e-mail] LIKE '%{ClientsTxt.Text}%' OR Адрес LIKE '%{ClientsTxt.Text}%') AND ID <> 0", "Клиенты").DefaultView;
            }
            else if (Orders.IsSelected == true && Clients_Subsection.IsSelected == true && ClientsTxt.Text.Length == 0)
            {
                ClientsHint.Visibility = Visibility.Visible; DataClients.ItemsSource = MsSQL.DataGridUpdate($"WHERE ID <> 0", "Клиенты").DefaultView;
            }
            // Заказы и клиенты - Заказы
            if (Orders.IsSelected == true && Orders_Subsection.IsSelected == true && OrdersTxt.Text.Length > 0)
            {
                OrdersHint.Visibility = Visibility.Hidden;
                if (FilterOrders.IsEnabled == true) DataOrders.ItemsSource = MsSQL.DataGridUpdate($"WHERE [{FilterOrders.Text}] LIKE '%{OrdersTxt.Text}%'", "ЗаказыВид").DefaultView;
                else DataOrders.ItemsSource = MsSQL.DataGridUpdate($"WHERE ID LIKE '%{OrdersTxt.Text}%' OR Клиент LIKE '%{OrdersTxt.Text}%' OR Сотрудник LIKE '%{OrdersTxt.Text}%' OR [Количество Товаров] LIKE '%{OrdersTxt.Text}%' OR [Итоговая Сумма] LIKE '%{OrdersTxt.Text}%'", "ЗаказыВид").DefaultView;
            }
            else if (Orders.IsSelected == true && Orders_Subsection.IsSelected == true && OrdersTxt.Text.Length == 0)
            {
                OrdersHint.Visibility = Visibility.Visible; DataOrders.ItemsSource = MsSQL.DataGridUpdate($"", "ЗаказыВид").DefaultView;
            }
            // Сотрудники - Должности
            if (Workers.IsSelected == true && Positions_Subsection.IsSelected == true && PositionsTxt.Text.Length > 0)
            {
                PositionsHint.Visibility = Visibility.Hidden;
                if (FilterPositions.IsEnabled == true) DataPositions.ItemsSource = MsSQL.DataGridUpdate($"WHERE [{FilterPositions.Text}] LIKE '%{PositionsTxt.Text}%'", "Должности").DefaultView;
                else DataPositions.ItemsSource = MsSQL.DataGridUpdate($"WHERE ID LIKE '%{PositionsTxt}%' OR Должность LIKE '%{PositionsTxt}%' OR [Заработная Плата] LIKE '%{PositionsTxt}%'", "Должности").DefaultView;
            }
            else if (Workers.IsSelected == true && Positions_Subsection.IsSelected == true && PositionsTxt.Text.Length == 0)
            {
                PositionsHint.Visibility = Visibility.Visible; DataPositions.ItemsSource = MsSQL.DataGridUpdate($"", "Должности").DefaultView;
            }
            // Сотрудники - Сотрудники
            if (Workers.IsSelected == true && Workers_Subsection.IsSelected == true && WorkersTxt.Text.Length > 0)
            {
                WorkersHint.Visibility = Visibility.Hidden;
                if (FilterWorkers.IsEnabled == true) DataWorkers.ItemsSource = MsSQL.DataGridUpdate($"WHERE [{FilterWorkers.Text}] LIKE '%{WorkersTxt.Text}%'", "СотрудникиВид").DefaultView;
                else DataWorkers.ItemsSource = MsSQL.DataGridUpdate($"WHERE ID LIKE '%{WorkersTxt.Text}%' OR Фамилия LIKE '%{WorkersTxt.Text}%' OR Имя LIKE '%{WorkersTxt.Text}%' OR Отчество LIKE '%{WorkersTxt.Text}%' OR Должность LIKE '%{WorkersTxt.Text}%' OR Телефон LIKE '%{WorkersTxt.Text}%'OR [e-mail] LIKE '%{WorkersTxt.Text}%' OR [Серия Паспорта] LIKE '%{WorkersTxt.Text}%' OR [Номер Паспорта] LIKE '%{WorkersTxt.Text}%' OR [Дата Рождения] LIKE '%{WorkersTxt.Text}%' OR [Дата Увольнения] LIKE '%{WorkersTxt.Text}%'", "СотрудникиВид").DefaultView;
            }
            else if (Workers.IsSelected == true && Workers_Subsection.IsSelected == true && WorkersTxt.Text.Length == 0)
            {
                WorkersHint.Visibility = Visibility.Visible; DataWorkers.ItemsSource = MsSQL.DataGridUpdate($"", "СотрудникиВид").DefaultView;
            }
            // Сотрудники - Выплата ЗП
            if (Workers.IsSelected == true && Payments_Subsection.IsSelected == true && PaymentsTxt.Text.Length > 0)
            {
                PaymentsHint.Visibility = Visibility.Hidden;
                if (FilterPayments.IsEnabled == true) DataPayments.ItemsSource = MsSQL.DataGridUpdate($"WHERE [{FilterPayments.Text}] LIKE '%{PaymentsTxt.Text}%'", "Выплата_ЗПВид").DefaultView;
                else DataPayments.ItemsSource = MsSQL.DataGridUpdate($"WHERE ID LIKE '%{PaymentsTxt.Text}%' OR Ответственный LIKE '%{PaymentsTxt.Text}%' OR Дата LIKE '%{PaymentsTxt.Text}%' OR Количество LIKE '%{PaymentsTxt.Text}%' OR  [Общая Сумма] LIKE '%{PaymentsTxt.Text}%'", "Выплата_ЗПВид").DefaultView;
            }
            else if (Workers.IsSelected == true && Payments_Subsection.IsSelected == true && PaymentsTxt.Text.Length == 0)
            {
                PaymentsHint.Visibility = Visibility.Visible; DataPayments.ItemsSource = MsSQL.DataGridUpdate($"", "Выплата_ЗПВид").DefaultView;
            }
            // Склады и наличее
            if (WareHouses.IsSelected == true && WareHousesTxt.Text.Length > 0)
            {
                WareHousesHint.Visibility = Visibility.Hidden;
                if (FilterWareHouses.IsEnabled == true) DataWareHouses.ItemsSource = MsSQL.DataGridUpdate($"WHERE [{FilterWareHouses.Text}] LIKE '%{WareHousesTxt.Text}%'", "СкладыВид").DefaultView;
                else DataWareHouses.ItemsSource = MsSQL.DataGridUpdate($"WHERE ID LIKE '%{WareHousesTxt.Text}%' OR Наименование LIKE '%{WareHousesTxt.Text}%' OR Адрес LIKE '%{WareHousesTxt.Text}%' OR [Количество Наименований] LIKE '%{WareHousesTxt.Text}%' OR [Всего Товаров] LIKE '%{WareHousesTxt.Text}%'", "СкладыВид").DefaultView;
            }
            else if (WareHouses.IsSelected == true && WareHousesTxt.Text.Length == 0)
            {
                WareHousesHint.Visibility = Visibility.Visible; DataWareHouses.ItemsSource = MsSQL.DataGridUpdate($"", "СкладыВид").DefaultView;
            }
            // Документы - Приходные
            if (Documents.IsSelected == true && Comings_Subsection.IsSelected == true && ComingsTxt.Text.Length > 0)
            {
                ComingsHint.Visibility = Visibility.Hidden;
                if (FilterComings.IsEnabled == true) DataComings.ItemsSource = MsSQL.DataGridUpdate($"WHERE [{FilterComings.Text}] LIKE '%{ComingsTxt.Text}%'", "Приходная_НакладнаяВид").DefaultView;
                else DataComings.ItemsSource = MsSQL.DataGridUpdate($"WHERE ID LIKE '%{ComingsTxt.Text}%' OR Ответственный LIKE '%{ComingsTxt.Text}%' OR Дата LIKE '%{ComingsTxt.Text}%' OR Склад LIKE '%{ComingsTxt.Text}%' OR [Количество Наименований] LIKE '%{ComingsTxt.Text}%' OR Сумма LIKE '%{ComingsTxt.Text}%'", "Приходная_НакладнаяВид").DefaultView;
            }
            else if (Documents.IsSelected == true && Comings_Subsection.IsSelected == true && ComingsTxt.Text.Length == 0)
            {
                ComingsHint.Visibility = Visibility.Visible; DataComings.ItemsSource = MsSQL.DataGridUpdate($"", "Приходная_НакладнаяВид").DefaultView;
            }
            // Документы - Расходные
            if (Documents.IsSelected == true && Expenditure_Subsection.IsSelected == true && ExpenditureTxt.Text.Length > 0)
            {
                ExpenditureHint.Visibility = Visibility.Hidden;
                if (FilterExpenditure.IsEnabled == true) DataExpenditure.ItemsSource = MsSQL.DataGridUpdate($"WHERE [{FilterExpenditure.Text}] LIKE '%{ExpenditureTxt.Text}%'", "Расходная_НакладнаяВид").DefaultView;
                else DataExpenditure.ItemsSource = MsSQL.DataGridUpdate($"WHERE ID LIKE '%{ExpenditureTxt.Text}%' OR Ответственный LIKE '%{ExpenditureTxt.Text}%' OR Дата LIKE '%{ExpenditureTxt.Text}%' OR Склад LIKE '%{ExpenditureTxt.Text}%' OR [Количество Наименований] LIKE '%{ExpenditureTxt.Text}%' OR Сумма LIKE '%{ExpenditureTxt.Text}%' OR Клиент LIKE '%{ExpenditureTxt.Text}%'", "Расходная_НакладнаяВид").DefaultView;
            }
            else if (Documents.IsSelected == true && Expenditure_Subsection.IsSelected == true && ExpenditureTxt.Text.Length == 0)
            {
                ExpenditureHint.Visibility = Visibility.Visible; DataExpenditure.ItemsSource = MsSQL.DataGridUpdate($"", "Расходная_НакладнаяВид").DefaultView;
            }
            // Документы - Регистр накоплений
            if (Documents.IsSelected == true && Register_Subsection.IsSelected == true && RegisterTxt.Text.Length > 0)
            {
                RegisterHint.Visibility = Visibility.Hidden;
                if (FilterRegister.IsEnabled == true) DataRegister.ItemsSource = MsSQL.DataGridUpdate($"WHERE [{FilterRegister.Text}] LIKE '%{RegisterTxt.Text}%'", "Регистр_НакопленийВид").DefaultView;
                else DataRegister.ItemsSource = MsSQL.DataGridUpdate($"WHERE ID LIKE '%{RegisterTxt.Text}%' OR [Тип Транзакции] LIKE '%{RegisterTxt.Text}%' OR [ID Транзакции] LIKE '%{RegisterTxt.Text}%' OR Дата LIKE '%{RegisterTxt.Text}%' OR Сумма  LIKE '%{RegisterTxt.Text}%'", "Регистр_НакопленийВид").DefaultView;
            }
            else if (Documents.IsSelected == true && Register_Subsection.IsSelected == true && RegisterTxt.Text.Length == 0)
            {
                RegisterHint.Visibility = Visibility.Visible; DataRegister.ItemsSource = MsSQL.DataGridUpdate($"", "Регистр_НакопленийВид").DefaultView;
            }
        }
        #endregion

        #region Обновление таблицы при включении поиска по фильру
        private void Filter_DropDownClosed(object sender, EventArgs e)
        {
            Search();
        }
        #endregion

        #region Выход из учетной записи
        private void Logout(object sender, MouseButtonEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }
        #endregion

        #region Открытие настроек
        private void SettingsClick(object sender, RoutedEventArgs e)
        {
            SettingsWin settings = new SettingsWin();
            settings.Show();
        }
        #endregion

        #region Подгрузка данных таблиц выбранной вкладки
        private void TabItem_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Search();
        }
        #endregion

        private void DatePicker_CalendarClosed(object sender, RoutedEventArgs e)
        {
            DataStatement.Focus();
        }
    }
}
