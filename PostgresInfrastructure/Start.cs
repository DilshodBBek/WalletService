using Autofac;
using System.Reflection;

namespace PostgresInfrastructure
{
    public class Start
    {
        public static void Builder(ContainerBuilder builder)
        {
            var dataAccess = Assembly.GetExecutingAssembly();

            builder.RegisterAssemblyTypes(dataAccess)
                .Where(t => t.Name.EndsWith("Service"))
                .AsImplementedInterfaces().SingleInstance();
        }
    }
}