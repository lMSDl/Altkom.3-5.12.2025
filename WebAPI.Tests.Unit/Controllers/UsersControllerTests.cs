using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebAPI.Controllers;
using WebAPI.Models;
using WebAPI.Services;

namespace WebAPI.Tests.Unit.Controllers
{
    public class UsersControllerTests
    {
        [Fact]
        public async Task Get_OkWithAllUsers()
        {
            //Arrange
            var userService = new Mock<ICrudService<User>>();
            var users = new Fixture().CreateMany<User>();

            var controller = new UsersController(userService.Object);

            userService.Setup(x => x.ReadAsync()).ReturnsAsync(users);

            //Act
            var result = await controller.Get();

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnUsers = Assert.IsAssignableFrom<IEnumerable<User>>(okResult.Value);
            Assert.Equal(users, returnUsers);
        }

        [Fact]
        public async Task Get_ById_OkWithUser()
        {
            //Arrange
            var userService = new Mock<ICrudService<User>>();
            var user = new Fixture().Create<User>();
            var controller = new UsersController(userService.Object);

            userService.Setup(x => x.ReadAsync(user.Id)).ReturnsAsync(user);

            //Act
            var result = await controller.Get(user.Id);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnUser = Assert.IsType<User>(okResult.Value);
            Assert.Equal(user, returnUser);
        }

        [Fact]
        public Task Get_NotExistingId_NotFound()
        {
            return ReturnsNotFound(async controller => (await controller.Get(default)).Result!);
        }

        [Fact]
        public Task Delete_NotExistingId_NotFound()
        {
            return ReturnsNotFound(controller => controller.Delete(default));
        }

        [Fact]
        public Task Put_NotExistingId_NotFound()
        {
            return ReturnsNotFound(async controller => await controller.Put(default, default!));
        }

        private async Task ReturnsNotFound(Func<UsersController, Task<ActionResult>> func)
        {
            //Arrange
            var userService = new Mock<ICrudService<User>>();
            var controller = new UsersController(userService.Object);

            //Act
            var result = await func(controller);

            //Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
