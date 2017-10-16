using Autofac;
using Common.Log;
using Lykke.Job.RabbitMqMonitoring.Core.Services;
using Lykke.Job.RabbitMqMonitoring.Core.Settings.JobSettings;
using Lykke.Job.RabbitMqMonitoring.Services;
using Lykke.SettingsReader;
using Lykke.Job.RabbitMqMonitoring.PeriodicalHandlers;

namespace Lykke.Job.RabbitMqMonitoring.Modules
{
    public class JobModule : Module
    {
        private readonly RabbitMqMonitoringSettings _settings;
        private readonly IReloadingManager<DbSettings> _dbSettingsManager;
        private readonly ILog _log;

        public JobModule(RabbitMqMonitoringSettings settings, IReloadingManager<DbSettings> dbSettingsManager, ILog log)
        {
            _settings = settings;
            _log = log;
            _dbSettingsManager = dbSettingsManager;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(_log)
                .As<ILog>()
                .SingleInstance();

            builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();

            builder.RegisterType<StartupManager>()
                .As<IStartupManager>();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>();

            builder.RegisterType<RabbitMqManagementService>()
                .As<IRabbitMqManagementService>()
                .SingleInstance();

            RegisterPeriodicalHandlers(builder);
        }

        private void RegisterPeriodicalHandlers(ContainerBuilder builder)
        {
            // TODO: You should register each periodical handler in DI container as IStartable singleton and autoactivate it
            builder.RegisterType<CheckRabbitMqHandler>()
                .As<IStartable>()
                .AutoActivate()
                .WithParameter(TypedParameter.From(_settings.RabbitMqConnections))
                .WithParameter(TypedParameter.From(_settings.CheckRate))
                .WithParameter(TypedParameter.From(_settings.MaxMessagesCount))
                .SingleInstance();
        }
    }
}
