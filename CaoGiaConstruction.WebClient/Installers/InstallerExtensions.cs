namespace CaoGiaConstruction.WebClient.Installers
{
    public static class InstallerExtensions
    {
        public static void InstallServicesInAssembly(this IServiceCollection services, IConfiguration configuration)
        {
            var installers = typeof(Program).Assembly.ExportedTypes.Where(x =>
           typeof(IInstaller).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract).Select(Activator.CreateInstance).Cast<IInstaller>().ToList();
            installers.ForEach(installer => installer.InstallServices(services, configuration));

            //Auto dependency inject transaction
            var typeTransient = typeof(ITransientService);
            var typesTransient = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeTransient.IsAssignableFrom(p) && p.IsClass);

            foreach (var implementType in typesTransient)
            {
                var interfaceType = implementType.FindInterfaces((x, y) => x.Name.Contains(implementType.Name), null).FirstOrDefault();
                if (interfaceType != null)
                {
                    services.AddTransient(interfaceType, implementType);
                }
            }
            //Auto dependency inject scope
            var typeScope = typeof(IScopeService);
            var typesScope = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeScope.IsAssignableFrom(p) && p.IsClass);

            foreach (var implementType in typesScope)
            {
                var interfaceType = implementType.FindInterfaces((x, y) => x.Name.Contains(implementType.Name), null).FirstOrDefault();
                if (interfaceType != null)
                {
                    services.AddScoped(interfaceType, implementType);
                }
            }
        }
    }
}