using PropertyManagementApp.Models;
using PropertyManagementApp.Controllers;
using PropertyManagementApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;

namespace PropertyManagementAppTests
{
    public class PropertyControllerTests
    {
        private readonly Mock<IPropertyService> _propertyServiceMock;
        private readonly PropertyController _controller;

        private const string UserId = "landlord1";

        public PropertyControllerTests()
        {
            _propertyServiceMock = new Mock<IPropertyService>();

            _controller = new PropertyController(_propertyServiceMock.Object);

            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, UserId)
            };

            var userIdentity = new ClaimsIdentity(userClaims, "Authentication1");

            var userPrincipal = new ClaimsPrincipal(userIdentity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = userPrincipal
                }
            };
        }

        [Fact]
        public async Task Index_ReturnsViewWithProperties()
        {
            // Arrange
            var properties = new List<Property>
            {
                new Property { Id = 1 },
                new Property { Id = 2 }
            };

            _propertyServiceMock
                .Setup(x => x.GetPropertiesAsync(UserId))
                .ReturnsAsync(properties);

            // Act
            var actionResult = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actionResult);

            Assert.Equal(properties, viewResult.Model);

            _propertyServiceMock.Verify(x => x.GetPropertiesAsync(UserId));
        }

        [Fact]
        public async Task Create_ValidProperty_RedirectsToIndex()
        {
            // Arrange
            var property = new Property
            {
                Id = 1
            };

            // Act
            var actionResult = await _controller.Create(property);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(actionResult);

            Assert.Equal(nameof(_controller.Index), redirectResult.ActionName);

            _propertyServiceMock.Verify(x => x.CreatePropertyAsync(property, UserId));
        }

        [Fact]
        public async Task Edit_ExistingProperty_ReturnsViewWithProperty()
        {
            // Arrange
            var property = new Property
            {
                Id = 1
            };

            _propertyServiceMock
                .Setup(x => x.GetPropertyAsync(property.Id, UserId))
                .ReturnsAsync(property);

            // Act
            var actionResult = await _controller.Edit(property.Id);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actionResult);

            Assert.Equal(property, viewResult.Model);

            _propertyServiceMock.Verify(x => x.GetPropertyAsync(property.Id, UserId));
        }

        [Fact]
        public async Task Edit_ValidProperty_RedirectsToIndex()
        {
            // Arrange
            var property = new Property
            {
                Id = 1
            };

            _propertyServiceMock
                .Setup(x => x.UpdatePropertyAsync(property, UserId))
                .ReturnsAsync(true);

            // Act
            var actionResult = await _controller.Edit(property);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(actionResult);

            Assert.Equal(nameof(_controller.Index), redirectResult.ActionName);

            _propertyServiceMock.Verify(x => x.UpdatePropertyAsync(property, UserId));
        }

        [Fact]
        public async Task Delete_ExistingProperty_RedirectsToIndex()
        {
            // Arrange
            var propertyId = 1;

            _propertyServiceMock
                .Setup(x => x.DeletePropertyAsync(propertyId, UserId))
                .ReturnsAsync(true);

            // Act
            var actionResult = await _controller.Delete(propertyId);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(actionResult);

            Assert.Equal(nameof(_controller.Index), redirectResult.ActionName);

            _propertyServiceMock.Verify(x => x.DeletePropertyAsync(propertyId, UserId));
        }
    }
}
