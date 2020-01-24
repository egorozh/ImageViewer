using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Prism.Commands;
using ReactiveUI;

namespace AvaloniaPictureAnalyser.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private Uri _imagePath;

        #region Public Properties

        public Uri ImagePath
        {
            get => _imagePath;
            set => this.RaiseAndSetIfChanged(ref _imagePath, value);
        }


        public ImageViewerController Controller { get; }

        public ObservableCollection<KeyBinding> KeyBindings { get; } = new ObservableCollection<KeyBinding>();

        #endregion

        #region Commands

        public DelegateCommand OpenImageCommand { get; }

        #endregion

        #region Constructor

        public MainWindowViewModel()
        {
            Controller = new ImageViewerController();


            OpenImageCommand = new DelegateCommand(OpenImage);

            //KeyBindings.Add(new KeyBinding(new DelegateCommand(IncreaseScale), Key.Add, ModifierKeys.None));
            //KeyBindings.Add(new KeyBinding(new DelegateCommand(DecreaseScale), Key.Subtract, ModifierKeys.None));
        }

        #endregion

        #region Command Methods

        private async void OpenImage()
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Выберите изображение",
                Filters =
                    new List<FileDialogFilter>()
                    {
                        new FileDialogFilter()
                        {
                            Extensions = new List<string>()
                            {
                                "jpg"
                            },
                            Name = "Pictures"
                        }
                    }
            };

            var applicationLifetime =
                Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;

            var result = await openFileDialog.ShowAsync(applicationLifetime.MainWindow);

            if (result.Length < 1)
                return;

            var path = result.First();

            ImagePath = new Uri(path, UriKind.RelativeOrAbsolute);
        }

        private void DecreaseScale()
        {
            Controller.DecreaseScale();
        }

        private void IncreaseScale()
        {
            Controller.IncreaseScale();
        }

        #endregion
    }
}