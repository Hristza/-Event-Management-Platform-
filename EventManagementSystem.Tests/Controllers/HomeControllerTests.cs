using EventManagementSystem.Controllers;
using EventManagementSystem.Models;
using EventManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace EventManagementSystem.Tests.Controllers
{
    // Тук използваме Moq, за да направим "фалшив" IEventService.
    // Така тестваме САМО контролера, без истинска база данни.
    // Това е смисълът на "mocked services".
    [TestFixture]
    public class HomeControllerTests
    {
        [Test]
        public async Task Index_ReturnsView_WithUpcomingEvents()
        {
            // Arrange: подготвяме фалшиви данни и mock на сервиза.
            var fakeEvents = new List<Event>
            {
                new Event { Id = 1, Title = "Концерт" },
                new Event { Id = 2, Title = "Мач" }
            };

            var mockService = new Mock<IEventService>();
            // "Когато някой извика GetUpcomingAsync(...), върни fakeEvents."
            mockService
                .Setup(s => s.GetUpcomingAsync(It.IsAny<int>()))
                .ReturnsAsync(fakeEvents);

            var controller = new HomeController(mockService.Object);

            // Act: извикваме действието.
            var result = await controller.Index();

            // Assert: проверяваме, че връща View с точно тези събития.
            var viewResult = result as ViewResult;
            Assert.That(viewResult, Is.Not.Null);

            var model = viewResult!.Model as IEnumerable<Event>;
            Assert.That(model, Is.Not.Null);
            Assert.That(model!.Count(), Is.EqualTo(2));

            // Проверяваме, че методът на сервиза е бил извикан точно веднъж.
            mockService.Verify(s => s.GetUpcomingAsync(It.IsAny<int>()), Times.Once);
        }
    }
}
