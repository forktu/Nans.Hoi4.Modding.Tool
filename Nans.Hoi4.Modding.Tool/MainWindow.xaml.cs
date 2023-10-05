using Nans.Hoi4.Modding.Tool.Localization;
using Nans.Hoi4.Modding.Tool.Logging;
using Nans.Hoi4.Modding.Tool.Themes;
using Nans.Hoi4.Modding.Tool.ViewModels;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Nans.Hoi4.Modding.Tool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            Instance = this;
            InitializeComponent();
            MainWindowViewModel = new MainWindowViewModel(this);
            DataContext = MainWindowViewModel;
        }
        public MainWindowViewModel MainWindowViewModel { get; }

        public new void Show()
        {
            #region THEME SETUP
            ThemeManager.Init(AppConfig.Instance.accentColor, AppConfig.Instance.useDarkMode);
            SetBackground(AppConfig.Instance.mainWindowBackgroundImage, AppConfig.Instance.useDarkMode);
            #endregion

            ImportAssetsForm iaf = new ImportAssetsForm();
            iaf.ShowDialog();

            Width *= AppConfig.Instance.scale;
            Height *= AppConfig.Instance.scale;
            MinWidth *= AppConfig.Instance.scale;
            MinHeight *= AppConfig.Instance.scale;
        }
    }
}