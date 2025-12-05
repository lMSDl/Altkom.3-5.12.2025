using AutoFixture;
using Castle.Components.DictionaryAdapter.Xml;
using Moq;

namespace ConsoleApp.Tests.xUnit
{
    public class ServiceUserTests
    {
        [Fact]
        public void SetServiceFriendlyName_ShouldSetName()
        {
            // Arrange
            var mockService = new Mock<IService>();
            var serviceUser = new ServiceUser(mockService.Object);

            string expectedName = new Fixture().Create<string>();

            // Act
            serviceUser.SetServiceFriendlyName(expectedName);

            //Assert
            mockService.VerifySet(x => x.Name = expectedName, Times.Once);
        }

        [Fact]
        public void ValidateUniqueId_UniqueIdIsNullOrEmpty_False()
        {
            // Arrange
            var mockService = new Mock<IService>();
            var serviceUser = new ServiceUser(mockService.Object);
            mockService.Setup(s => s.UniqueId).Returns(string.Empty);

            // Act
            var result = serviceUser.ValidateUniqueId();

            //Assert
            Assert.False(result);
        }


        [Fact]
        public void ValidateUniqueId_UniqueIdIsValidGuid_True()
        {
            // Arrange
            var mockService = new Mock<IService>();
            var serviceUser = new ServiceUser(mockService.Object);
            mockService.Setup(s => s.UniqueId).Returns(Guid.NewGuid().ToString());

            // Act
            var result = serviceUser.ValidateUniqueId();

            //Assert
            Assert.True(result);
        }

        [Fact]
        public void ValidateUniqueId_UniqueIdIsNotValidGuid_False()
        {
            // Arrange
            var mockService = new Mock<IService>();
            var serviceUser = new ServiceUser(mockService.Object);
            mockService.Setup(s => s.UniqueId).Returns(new Fixture().Create<int>().ToString());

            // Act
            var result = serviceUser.ValidateUniqueId();

            //Assert
            Assert.False(result);
        }

        [Fact]
        public void StartService_InvokesStartService()
        {
            // Arrange
            var mockService = new Mock<IService>();
            var serviceUser = new ServiceUser(mockService.Object);
            // Act
            serviceUser.StartService();

            // Assert
            mockService.Verify(s => s.StartService(), Times.Once);
        }

        [Fact]
        public void StartService_SetsIsServiceStared()
        {
            // Arrange
            var mockService = new Mock<IService>();
            var serviceUser = new ServiceUser(mockService.Object);
            // Act
            mockService.Raise(s => s.OnServiceStarted += null, EventArgs.Empty);
            // Assert
            Assert.True(serviceUser.IsServiceStarted);
        }
    }
}
