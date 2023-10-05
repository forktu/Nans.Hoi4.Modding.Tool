using Nans.Hoi4.Modding.Tool.Configuration;
using Nans.Hoi4.Modding.Tool.Localization;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Nans.Hoi4.Modding.Tool.ViewModels
{
    public sealed class MainWindowViewModel : BaseViewModel
    {
        public MainWindowViewModel(MainWindow mainWindow)
        {
            MainWindow = mainWindow;
            RoutedCommand saveHotkey = new RoutedCommand();
            saveHotkey.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Control));
            MainWindow.CommandBindings.Add(new CommandBinding(saveHotkey,
                new ExecutedRoutedEventHandler((object sender, ExecutedRoutedEventArgs e) =>
                {
                    SaveProjectCommand.Execute(null);
                })));
            RoutedCommand loadHotkey = new RoutedCommand();
            loadHotkey.InputGestures.Add(new KeyGesture(Key.O, ModifierKeys.Control));
            MainWindow.CommandBindings.Add(new CommandBinding(loadHotkey,
                new ExecutedRoutedEventHandler((object sender, ExecutedRoutedEventArgs e) =>
                {
                    LoadProjectCommand.Execute(null);
                })));
            RoutedCommand exportHotkey = new RoutedCommand();
            exportHotkey.InputGestures.Add(new KeyGesture(Key.E, ModifierKeys.Control));
            MainWindow.CommandBindings.Add(new CommandBinding(exportHotkey,
                new ExecutedRoutedEventHandler((object sender, ExecutedRoutedEventArgs e) =>
                {
                    ExportProjectCommand.Execute(null);
                })));
            RoutedCommand newFileHotkey = new RoutedCommand();
            newFileHotkey.InputGestures.Add(new KeyGesture(Key.N, ModifierKeys.Control));
            MainWindow.CommandBindings.Add(new CommandBinding(newFileHotkey,
                new ExecutedRoutedEventHandler((object sender, ExecutedRoutedEventArgs e) =>
                {
                    NewProjectCommand.Execute(null);
                })));
            RoutedCommand pasteCommand = new RoutedCommand();
            pasteCommand.InputGestures.Add(new KeyGesture(Key.V, ModifierKeys.Control));
            MainWindow.CommandBindings.Add(new CommandBinding(pasteCommand,
                new ExecutedRoutedEventHandler((object sender, ExecutedRoutedEventArgs e) =>
                {
                    switch (MainWindow.mainTabControl.SelectedIndex)
                    {
                        case 0 when CountryTabViewModel.Country != null: // Country (condition)
                            {
                                if (ClipboardManager.TryGetObject(ClipboardManager.ConditionFormat, out var obj) && obj is Condition cond)
                                {
                                    CountryTabViewModel.Country.visibilityConditions.Add(cond);
                                }
                            }
                            break;
                    }
                })));

            RoutedCommand toggleLogsCommand = new RoutedCommand();
            toggleLogsCommand.InputGestures.Add(new KeyGesture(Key.F1, ModifierKeys.Control));
            MainWindow.CommandBindings.Add(new CommandBinding(toggleLogsCommand,
                (sender, e) =>
                {
                    if (LogTabViewModel.TabVisibility == Visibility.Collapsed)
                    {
                        LogTabViewModel.TabVisibility = Visibility.Visible;
                        MainWindow.mainTabControl.SelectedItem = MainWindow.logListTab;
                    }
                    else
                    {
                        LogTabViewModel.TabVisibility = Visibility.Collapsed;
                        MainWindow.mainTabControl.SelectedItem = null;
                    }
                }));

            MainWindow.txtID.BindFindReplace(FindReplaceFormats.COUNTRY_ID);

            CountryTabViewModel = new CountryTabViewModel();
            MainWindow.mainTabControl.SelectionChanged += TabControl_SelectionChanged;

            MainWindow.CurrentProject.OnDataLoaded += async () =>
            {
                ResetAll();

                ProjectData proj = MainWindow.CurrentProject;
                HOIProject data = proj.data;

                if (data.lastCountry > -1 && data.lastCountry < data.Countrys.Count)
                {
                    CountryTabViewModel.Country = data.Countrys[data.lastCountry];
                }

                GameAssetManager.Purge(EGameAssetOrigin.Hooked);

                if (data.settings.assetDirs != null && data.settings.assetDirs.Count > 0)
                {
                    MainWindow.blockActionsOverlay.Dispatcher.Invoke(() =>
                    {
                        MainWindow.blockActionsOverlay.Visibility = Visibility.Visible;
                    });

                    foreach (var ad in data.settings.assetDirs)
                    {
                        MainWindow.textBlockActions.Dispatcher.Invoke(() =>
                        {
                            MainWindow.textBlockActions.Text = LocalizationManager.Current.Interface.Translate("StartUp_ImportGameAssets_Window_Step_Hooked", ad);
                        });

                        await GameAssetManager.Import(ad, EGameAssetOrigin.Hooked, (cur, total) =>
                        {
                            MainWindow.progrBar.Dispatcher.Invoke(() =>
                            {
                                MainWindow.progrBar.Value = cur;
                                MainWindow.progrBar.Maximum = total;
                            });
                        });
                    }

                    MainWindow.blockActionsOverlay.Dispatcher.Invoke(() =>
                    {
                        MainWindow.blockActionsOverlay.Visibility = Visibility.Collapsed;
                    });
                }

                UpdateAllTabs();
            };
        }
        public MainWindow MainWindow { get; set; }
        public CountryTabViewModel CountryTabViewModel { get; set; }
        public void ResetAll()
        {
            CountryTabViewModel.Reset();
        }
        public void SaveAll()
        {
            CountryTabViewModel.Save();

            ProjectData proj = MainWindow.CurrentProject;
            HOIProject data = proj.data;

            data.lastCountry = data.Countrys.IndexOf(CountryTabViewModel.Country);
        }
        public void UpdateAllTabs()
        {
            CountryTabViewModel.UpdateTabs();
        }
        internal void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e?.AddedItems.Count == 0 || sender == null)
            {
                return;
            }

            int selectedIndex = (sender as TabControl).SelectedIndex;
            TabItem tab = e?.AddedItems[0] as TabItem;
            if (AppConfig.Instance.animateControls && tab?.Content is Grid g)
            {
                DoubleAnimation anim = new DoubleAnimation(0, 1, new Duration(new TimeSpan(0, 0, 0, 0, 500)));
                g.BeginAnimation(MainWindow.OpacityProperty, anim);
            }
            if (selectedIndex == (sender as TabControl).Items.Count - 2);
        }
        private ICommand
            newProjectCommand,
            loadProjectCommand,
            saveProjectCommand,
            saveAsProjectCommand,
            exportProjectCommand,
            exportProjectToUnturnedCommand,
            exportProjectToWorkshopCommand,
            exitCommand,
            optionsCommand,
            projectSettingsCommand,
            aboutCommand,
            importFileCommand,
            importDirectoryCommand,
            importProjectCommand;
        public ICommand ImportProjectCommand
        {
            get
            {
                if (importProjectCommand is null)
                {
                    importProjectCommand = new BaseCommand(() =>
                    {
                        OpenFileDialog ofd = new OpenFileDialog()
                        {
                            Filter =
                            $"{LocalizationManager.Current.General["Project_SaveFilter"]}|*.hoiproj" + "|" +
                            $"{LocalizationManager.Current.General["Project_SaveFilter_Legacy"]}|*.hoi",
                            Multiselect = true,
                        };

                        if (ofd.ShowDialog() == true)
                        {
                            bool hasLoadedAnything = false;

                            int countryCount = 0;

                            foreach (var sPath in ofd.FileNames)
                            {
                                if (string.IsNullOrWhiteSpace(sPath))
                                    continue;

                                if (!File.Exists(sPath))
                                    continue;

                                ProjectData pData = new ProjectData
                                {
                                    file = sPath,
                                };

                                if (!pData.Load(null))
                                    continue;

                                hasLoadedAnything = true;

                                var project = MainWindow.CurrentProject.data;

                                foreach (var Country in pData.data.Countrys)
                                {
                                    project.Countrys.Add(Country);
                                    countryCount++;
                                }
                            }

                            if (hasLoadedAnything)
                            {
                                UpdateAllTabs();

                                App.NotificationManager.Notify(
                                    LocalizationManager.Current.Notification.Translate(
                                        "Import_Project_Done",
                                            countryCount ));
                            }
                            else
                            {
                                App.NotificationManager.Notify(LocalizationManager.Current.Notification["Import_Project_None"]);
                            }
                        }
                    });
                }

                return importProjectCommand;
            }
        }
        public ICommand ImportDirectoryCommand
        {
            get
            {
                if (importDirectoryCommand == null)
                {
                    importDirectoryCommand = new BaseCommand(() =>
                    {
                        CommonOpenFileDialog cofd = new CommonOpenFileDialog
                        {
                            IsFolderPicker = true,
                            Multiselect = false,
                            RestoreDirectory = true,
                            Title = LocalizationManager.Current.Interface.Translate("Main_Menu_File_Import_Directory_Title"),
                        };
                        CommonFileDialogResult result = cofd.ShowDialog();
                        if (result == CommonFileDialogResult.Ok)
                        {
                            ParseDirCommand pCommand = Command.GetCommand<ParseDirCommand>() as ParseDirCommand;
                            pCommand.Execute(new string[] { cofd.FileName });
                            if (pCommand.LastResult)
                            {
                                App.NotificationManager.Notify(LocalizationManager.Current.Notification.Translate("Import_Directory_Done", pCommand.LastImported, pCommand.LastSkipped));
                                MainWindow.MainWindowViewModel.UpdateAllTabs();
                            }
                        }
                    });
                }
                return importDirectoryCommand;
            }
        }
        public ICommand ImportFileCommand
        {
            get
            {
                if (importFileCommand == null)
                {
                    importFileCommand = new BaseCommand(() =>
                    {
                        OpenFileDialog ofd = new OpenFileDialog()
                        {
                            Filter = $"Unturned Asset|*.dat;*.asset",
                            Multiselect = true
                        };
                        if (ofd.ShowDialog() == true)
                        {
                            ParseCommand pCommand = Command.GetCommand<ParseCommand>() as ParseCommand;
                            foreach (string file in ofd.FileNames)
                            {
                                pCommand.Execute(new string[] { file });
                                if (pCommand.LastResult)
                                {
                                    App.NotificationManager.Notify(LocalizationManager.Current.Notification.Translate("Import_File_Done", file));
                                    MainWindow.MainWindowViewModel.UpdateAllTabs();
                                }
                                else
                                {
                                    App.NotificationManager.Notify(LocalizationManager.Current.Notification.Translate("Import_File_Fail", file));
                                }
                            }
                        }
                    });
                }
                return importFileCommand;
            }
        }
        public ICommand NewProjectCommand
        {
            get
            {
                if (newProjectCommand == null)
                {
                    newProjectCommand = new BaseCommand(() =>
                    {
                        if (MainWindow.CurrentProject.SavePrompt() == null)
                        {
                            return;
                        }

                        MainWindow.CurrentProject.data = new HOIProject();
                        MainWindow.CurrentProject.file = "";
                        ResetAll();
                        UpdateAllTabs();
                        MainWindow.CurrentProject.isSaved = true;
                        MainWindow.Started = DateTime.UtcNow;
                    });
                }
                return newProjectCommand;
            }
        }
        public ICommand SaveProjectCommand
        {
            get
            {
                if (saveProjectCommand == null)
                {
                    saveProjectCommand = new BaseCommand(() =>
                    {
                        SaveAll();

                        if (MainWindow.CurrentProject.Save())
                        {
                            App.NotificationManager.Notify(LocalizationManager.Current.Notification["Project_Saved"]);

                            MainWindow.AddToRecentList(MainWindow.CurrentProject.file);
                        }
                    });
                }
                return saveProjectCommand;
            }
        }
        public ICommand SaveAsProjectCommand
        {
            get
            {
                if (saveAsProjectCommand == null)
                {
                    saveAsProjectCommand = new BaseCommand(() =>
                    {
                        SaveAll();
                        string oldPath = MainWindow.CurrentProject.file;
                        MainWindow.CurrentProject.file = "";
                        if (!MainWindow.CurrentProject.Save())
                        {
                            MainWindow.CurrentProject.file = oldPath;
                        }
                        else
                        {
                            App.NotificationManager.Notify(LocalizationManager.Current.Notification["Project_Saved"]);

                            MainWindow.AddToRecentList(MainWindow.CurrentProject.file);
                        }
                    });
                }
                return saveAsProjectCommand;
            }
        }
        public ICommand LoadProjectCommand
        {
            get
            {
                if (loadProjectCommand == null)
                {
                    loadProjectCommand = new BaseCommand(() =>
                    {
                        string path;
                        OpenFileDialog ofd = new OpenFileDialog()
                        {
                            Filter =
                            $"{LocalizationManager.Current.General["Project_SaveFilter"]}|*.hoiproj" + "|" +
                            $"{LocalizationManager.Current.General["Project_SaveFilter_Legacy"]}|*.hoi",
                            Multiselect = false
                        };
                        bool? res = ofd.ShowDialog();
                        if (res == true)
                        {
                            path = ofd.FileName;
                        }
                        else
                        {
                            return;
                        }

                        string oldPath = MainWindow.CurrentProject.file;
                        MainWindow.CurrentProject.file = path;
                        if (MainWindow.CurrentProject.Load(null))
                        {
                            UpdateAllTabs();
                            App.NotificationManager.Clear();
                            App.NotificationManager.Notify(LocalizationManager.Current.Notification["Project_Loaded"]);
                            MainWindow.AddToRecentList(MainWindow.CurrentProject.file);
                        }
                        else
                        {
                            MainWindow.CurrentProject.file = oldPath;
                        }
                    });
                }
                return loadProjectCommand;
            }
        }
        public ICommand ExportProjectCommand
        {
            get
            {
                if (exportProjectCommand == null)
                {
                    exportProjectCommand = new BaseCommand(() =>
                    {
                        Mistakes.MistakesManager.FindMistakes();
                        if (Mistakes.MistakesManager.Criticals_Count > 0)
                        {
                            SystemSounds.Hand.Play();
                            MainWindow.mainTabControl.SelectedIndex = MainWindow.mainTabControl.Items.Count - 2;
                            return;
                        }
                        if (Mistakes.MistakesManager.Warnings_Count > 0)
                        {
                            MessageBoxResult res = MessageBox.Show(LocalizationManager.Current.Interface["Export_Warnings_Text"], LocalizationManager.Current.Interface["Export_Warnings_Caption"], MessageBoxButton.YesNo);
                            if (!(res == MessageBoxResult.OK || res == MessageBoxResult.Yes))
                            {
                                return;
                            }
                        }
                        SaveAll();
                        if (MainWindow.CurrentProject.Save())
                        {
                            App.NotificationManager.Notify(LocalizationManager.Current.Notification["Project_Saved"]);
                        }

                        Export.Exporter.ExportHOI(MainWindow.CurrentProject.data, Path.Combine(AppConfig.ExeDirectory, "results"));
                    });
                }
                return exportProjectCommand;
            }
        }
    }
}