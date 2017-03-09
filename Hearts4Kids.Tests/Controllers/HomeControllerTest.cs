using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Hearts4Kids.Controllers;
using Hearts4Kids.Domain;
using Hearts4Kids.Services;
using Moq;
using SimpleFixture.Moq;
using Xunit;

namespace Hearts4Kids.Tests.Controllers
{
    public class HomeControllerTest
    {
        private static Mock<DbSet<T>> CreateDbSetMock<T>(IEnumerable<T> elements) where T : class
        {
            var elementsAsQueryable = elements.AsQueryable();
            var dbSetMock = new Mock<DbSet<T>>();

            dbSetMock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(elementsAsQueryable.Provider);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(elementsAsQueryable.Expression);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(elementsAsQueryable.ElementType);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(elementsAsQueryable.GetEnumerator());

            return dbSetMock;
        }

        [Fact]
        public void Index()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void About()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.About() as ViewResult;

            // Assert
            //Assert.AreEqual("Your application description page.", result.ViewBag.Message);
        }

        [Fact]
        public void Contact()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Contact() as ViewResult;

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void IndexShowsCorrectNumberOfProfiles()
        {
            var fixture = new MoqFixture();
            fixture.Return(true).For<bool>().WhenNamed("Approved");

            var data = CreateDbSetMock(new List<UserBio> {
                fixture.Generate<UserBio>(),
                fixture.Generate<UserBio>(),
                fixture.Generate<UserBio>()
            });
            var context = new Mock<Hearts4KidsEntities>();
            var members = new MemberDetailService(context);
            var actual = new HomeController(members).Team() as Task<ActionResult>;
            Assert.Equal(3, 1);
        }
    }
}
