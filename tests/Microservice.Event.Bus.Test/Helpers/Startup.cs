using CodeDesignPlus.Core.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microservice.Event.Bus.Test.Helpers
{
    /// <summary>
    /// Implementación por defecto del servicio <see cref="IStartupServices"/>
    /// </summary>
    public class Startup :IStartupServices
    {
        /// <summary>
        /// Este metodo es invocado por SDK Microservice en el momento de iniciar la aplicación para
        /// </summary>
        /// <param name="services">Provee acceso al contenedor de dependencias de .net core</param>
        /// <param name="configuration">Provee acceso a las diferentes fuentes de configuración</param>
        /// <exception cref="NotImplementedException"></exception>
        public void Initialize(IServiceCollection services, IConfiguration configuration)
        {
            throw new NotImplementedException();
        }
    }
}
