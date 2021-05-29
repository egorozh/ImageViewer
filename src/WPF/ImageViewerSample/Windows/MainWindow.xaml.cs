using System.Windows;
using ControlzEx.Theming;
using Fluent;
using MahApps.Metro.Controls;

namespace ImageViewerSample
{
    internal partial class MainWindow
    {
        #region Public Properties

        /// <summary>
        /// Gets ribbon titlebar
        /// </summary>
        public RibbonTitleBar TitleBar
        {
            get => (RibbonTitleBar) this.GetValue(TitleBarProperty);
            private set => this.SetValue(TitleBarPropertyKey, value);
        }

        #endregion

        #region Dependency Properties

        // ReSharper disable once InconsistentNaming
        private static readonly DependencyPropertyKey TitleBarPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(TitleBar), typeof(RibbonTitleBar), typeof(MainWindow),
                new PropertyMetadata());

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="TitleBar"/>.
        /// </summary>
        public static readonly DependencyProperty TitleBarProperty = TitleBarPropertyKey.DependencyProperty;

        #endregion

        #region Constructor

        public MainWindow(IMainViewModel mainViewModel)
        {
            InitializeComponent();

            DataContext = mainViewModel;

            Loaded += MainWindow_Loaded;
            Closed += MainWindow_Closed;
        }

        #endregion

        #region Private Methods

        private void MainWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            this.TitleBar = this.FindChild<RibbonTitleBar>("RibbonTitleBar");
            TitleBar.HeaderAlignment = HorizontalAlignment.Left;
            TitleBar.Margin = new Thickness(60, 0, 0, 0);
            this.TitleBar.InvalidateArrange();
            this.TitleBar.UpdateLayout();

            ThemeManager.Current.ChangeTheme(this, ThemeManager.Current.DetectTheme(Application.Current));
            ThemeManager.Current.ThemeChanged += this.SyncThemes;
        }

        private void MainWindow_Closed(object sender, System.EventArgs e)
        {
            ThemeManager.Current.ThemeChanged -= this.SyncThemes;
        }

        private void SyncThemes(object sender, ThemeChangedEventArgs e)
        {
            if (e.Target == this)
            {
                return;
            }

            ThemeManager.Current.ChangeTheme(this, e.NewTheme);
        }

        #endregion
    }
}