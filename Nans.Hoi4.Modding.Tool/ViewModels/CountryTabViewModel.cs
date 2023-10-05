using Nans.Hoi4.Modding.Tool;
using Nans.Hoi4.Modding.Tool.Configuration;
using Nans.Hoi4.Modding.Tool.Forms;
using Nans.Hoi4.Modding.Tool.Localization;
using Nans.Hoi4.Modding.Tool.HOI;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Nans.Hoi4.Modding.Tool.ViewModels
{
    public sealed class CountryTabViewModel : BaseViewModel, ITabEditor, IHOITab
    {
        private HOICountry _country;
        public CountryTabViewModel()
        {
            MainWindow.Instance.countryTabSelect.SelectionChanged += CountryTabSelect_SelectionChanged;
            MainWindow.Instance.countryTabButtonAdd.Click += CountryTabButtonAdd_Click;
            HOICountry empty = new HOICountry();
            Country = empty;
            UpdateColorPicker();
            UpdateTabs();
        }
        private void CountryTabButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            HOICountry item = new HOICountry();
            MainWindow.CurrentProject.data.countrys.Add(item);
            MetroTabItem tabItem = CreateTab(item);
            MainWindow.Instance.countryTabSelect.Items.Add(tabItem);
            MainWindow.Instance.countryTabSelect.SelectedIndex = MainWindow.Instance.countryTabSelect.Items.Count - 1;
        }

        private void CountryTabSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tab = MainWindow.Instance.countryTabSelect;
            if (tab.SelectedItem != null && tab.SelectedItem is TabItem tabItem && tabItem.DataContext != null)
            {
                if (tabItem.DataContext is HOICountry selectedTabChar)
                    Country = selectedTabChar;
            }

            if (tab.SelectedItem is null)
            {
                MainWindow.Instance.countryTabGrid.IsEnabled = false;
                MainWindow.Instance.countryTabGrid.Visibility = Visibility.Collapsed;
                MainWindow.Instance.countryTabGridNoSelection.Visibility = Visibility.Visible;
            }
            else
            {
                MainWindow.Instance.countryTabGrid.IsEnabled = true;
                MainWindow.Instance.countryTabGrid.Visibility = Visibility.Visible;
                MainWindow.Instance.countryTabGridNoSelection.Visibility = Visibility.Collapsed;
            }
        }

        public void Save() { }
        public void Reset() { }

        public void UpdateTabs()
        {
            var tab = MainWindow.Instance.countryTabSelect;
            tab.Items.Clear();
            int selected = -1;
            for (int i = 0; i < MainWindow.CurrentProject.data.countrys.Count; i++)
            {
                var country = MainWindow.CurrentProject.data.countrys[i];
                if (country == _country)
                    selected = i;
                MetroTabItem tabItem = CreateTab(country);
                tab.Items.Add(tabItem);
            }
            if (selected != -1)
                tab.SelectedIndex = selected;

            if (tab.SelectedItem is null)
            {
                MainWindow.Instance.countryTabGrid.IsEnabled = false;
                MainWindow.Instance.countryTabGrid.Visibility = Visibility.Collapsed;
                MainWindow.Instance.countryTabGridNoSelection.Visibility = Visibility.Visible;
            }
            else
            {
                MainWindow.Instance.countryTabGrid.IsEnabled = true;
                MainWindow.Instance.countryTabGrid.Visibility = Visibility.Visible;
                MainWindow.Instance.countryTabGridNoSelection.Visibility = Visibility.Collapsed;
            }
        }

        private MetroTabItem CreateTab(NPCCountry country)
        {
            var binding = new Binding()
            {
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Mode = BindingMode.OneWay,
                Path = new PropertyPath("UIText")
            };
            Label l = new Label();
            l.SetBinding(Label.ContentProperty, binding);

            MetroTabItem tabItem = new MetroTabItem
            {
                CloseButtonEnabled = true,
                CloseTabCommand = CloseTabCommand,
                Header = l,
                DataContext = country
            };
            tabItem.CloseTabCommandParameter = tabItem;

            var cmenu = new ContextMenu();
            List<MenuItem> cmenuItems = new List<MenuItem>()
            {
                ContextHelper.CreateCopyButton((object sender, RoutedEventArgs e) =>
                {
                    Save();

                    ContextMenu context = (sender as MenuItem).Parent as ContextMenu;
                    MetroTabItem target = context.PlacementTarget as MetroTabItem;
                    ClipboardManager.SetObject(Universal_ItemList.ReturnType.Country, target.DataContext);
                }),
                ContextHelper.CreateDuplicateButton((object sender, RoutedEventArgs e) =>
                {
                    Save();

                    ContextMenu context = (sender as MenuItem).Parent as ContextMenu;
                    MetroTabItem target = context.PlacementTarget as MetroTabItem;
                    var cloned = (target.DataContext as NPCCountry).Clone();

                    MainWindow.CurrentProject.data.countrys.Add(cloned);
                    MetroTabItem ti = CreateTab(cloned);
                    MainWindow.Instance.countryTabSelect.Items.Add(ti);
                }),
                ContextHelper.CreatePasteButton((object sender, RoutedEventArgs e) =>
                {
                    if (ClipboardManager.TryGetObject(ClipboardManager.CountryFormat, out var obj) && !(obj is null) && obj is NPCCountry cloned)
                    {
                        MainWindow.CurrentProject.data.countrys.Add(cloned);
                        MetroTabItem ti = CreateTab(cloned);
                        MainWindow.Instance.countryTabSelect.Items.Add(ti);
                    }
                })
            };
        }
    }
}
