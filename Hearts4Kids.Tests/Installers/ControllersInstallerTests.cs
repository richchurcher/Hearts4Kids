using System;
using System.Linq;
using System.Web.Mvc;
using Castle.Core;
using Castle.Core.Internal;
using Castle.MicroKernel;
using Castle.Windsor;
using Hearts4Kids.Controllers;
using Hearts4Kids.Installers;
using Xunit;

namespace Hearts4Kids.Tests.Installers
{
    public class ControllersInstallerTests
    {
        private readonly IWindsorContainer _containerWithControllers;

        private static IHandler[] GetAllHandlers(IWindsorContainer container)
        {
            return GetHandlersFor(typeof(object), container);
        }

        private static IHandler[] GetHandlersFor(Type type, IWindsorContainer container)
        {
            return container.Kernel.GetAssignableHandlers(type);
        }

        private static Type[] GetImplementationTypesFor(Type type, IWindsorContainer container)
        {
            return GetHandlersFor(type, container)
                .Select(h => h.ComponentModel.Implementation)
                .OrderBy(t => t.Name)
                .ToArray();
        }

        private static Type[] GetPublicClassesFromApplicationAssembly(Predicate<Type> where)
        {
            return typeof(HomeController).Assembly.GetExportedTypes()
                .Where(t => t.IsClass)
                .Where(t => t.IsAbstract == false)
                .Where(where.Invoke)
                .OrderBy(t => t.Name)
                .ToArray();
        }

        public ControllersInstallerTests()
        {
            _containerWithControllers = new WindsorContainer()
                .Install(new ControllersInstaller());
        }

        [Fact]
        public void All_controllers_implement_IController()
        {
            var allHandlers = GetAllHandlers(_containerWithControllers);
            var controllerHandlers = GetHandlersFor(typeof(IController), _containerWithControllers);

            Assert.NotEmpty(allHandlers);
            Assert.Equal(allHandlers, controllerHandlers);
        }

        [Fact]
        public void All_controllers_are_registered()
        {
            var allControllers = GetPublicClassesFromApplicationAssembly(c => c.Is<IController>());
            var registeredControllers = GetImplementationTypesFor(typeof(IController), _containerWithControllers);
            Assert.Equal(allControllers, registeredControllers);
        }

        [Fact]
        public void All_and_only_controllers_have_Controllers_suffix()
        {
            var allControllers = GetPublicClassesFromApplicationAssembly(c => c.Name.EndsWith("Controller"));
            var registeredControllers = GetImplementationTypesFor(typeof(IController), _containerWithControllers);
            Assert.Equal(allControllers, registeredControllers);
        }

        [Fact]
        public void All_and_only_controllers_live_in_Controllers_namespace()
        {
            var allControllers = GetPublicClassesFromApplicationAssembly(c => c.Namespace.Contains("Controllers"));
            var registeredControllers = GetImplementationTypesFor(typeof(IController), _containerWithControllers);
            Assert.Equal(allControllers, registeredControllers);
        }

        [Fact]
        public void All_controllers_are_transient()
        {
            var nonTransientControllers = GetHandlersFor(typeof(IController), _containerWithControllers)
                .Where(controller => controller.ComponentModel.LifestyleType != LifestyleType.Transient)
                .ToArray();

            Assert.Empty(nonTransientControllers);
        }

        [Fact]
        public void All_controllers_expose_themselves_as_service()
        {
            var controllersWithWrongName = GetHandlersFor(typeof(IController), _containerWithControllers)
                .Where(controller => controller.ComponentModel.Services.Single() != controller.ComponentModel.Implementation)
                .ToArray();

            Assert.Empty(controllersWithWrongName);
        }
    }
}
