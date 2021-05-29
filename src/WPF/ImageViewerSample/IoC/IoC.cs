using Autofac;

namespace ImageViewerSample
{
    internal class IoC
    {
        #region Singleton Instance

        public static IoC Instance { get; } = new IoC();

        #endregion

        #region Private Fields

        private IContainer _container;

        #endregion

        #region Public Properties

        public MainWindow MainWindow => _container.Resolve<MainWindow>();

        public IMainViewModel MainViewModel => _container.Resolve<IMainViewModel>();

        public ILogger Logger => _container.Resolve<ILogger>();

        #endregion

        #region Constructor

        public IoC()
        {
            Build();
        }

        #endregion

        #region Private Methods

        private void Build()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<Logger>().As<ILogger>().SingleInstance();

            builder.RegisterType<MainViewModel>().As<IMainViewModel>().SingleInstance();
            builder.RegisterType<MainWindow>().AsSelf().SingleInstance();

            _container = builder.Build();
        }

        #endregion
    }
}