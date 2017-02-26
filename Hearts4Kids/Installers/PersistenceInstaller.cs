using System.Data.Entity;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Hearts4Kids.Domain;
using Hearts4Kids.Services;

namespace Hearts4Kids.Installers
{
    public class PersistenceInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<DbContext>()
                                        .ImplementedBy<Hearts4KidsEntities>()
                                        .LifestylePerWebRequest()
                              );
            container.Register(Component.For<MemberDetailService>());
        }
    }
}