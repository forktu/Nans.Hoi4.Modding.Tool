using Nans.Hoi4.Modding.Tool.Localization;
using MahApps.Metro.Controls;
using Microsoft.Win32;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Nans.Hoi4.Modding.Tool.Configuration
{
    /// <summary>
    /// Interaction logic for ConfigWindow.xaml
    /// </summary>
    public partial class ConfigWindow : MetroWindow
    {
        public ConfigWindow()
        {
            InitializeComponent();
            DataContext = this;
        }
    }
}
