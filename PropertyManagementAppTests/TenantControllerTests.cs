using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PropertyManagementApp.Controllers;
using PropertyManagementApp.Models;
using PropertyManagementApp.Services;
using System.Security.Claims;

namespace PropertyManagementAppTests
{
    public class TenantControllerTests
    {
        private readonly Mock<ITenantService> _tenantServiceMock;
        private readonly Mock<IPropertyService> _propertyServiceMock;
        private readonly TenantController _controller;

        private const string UserId = "landlord1";

        public TenantControllerTests()
        {
            _tenantServiceMock = new Mock<ITenantService>();
            _propertyServiceMock = new Mock<IPropertyService>();

            _controller = new TenantController(_tenantServiceMock.Object, _propertyServiceMock.Object);

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
        public async Task Index_PropertyExists_ReturnsViewWithTenants()
        {
            // Arrange
            var property = new Property
            {
                Id = 1,
                Tenants = new List<Tenant>
                {
                    new Tenant { Id = 1 },
                    new Tenant { Id = 2 }
                }
            };

            _propertyServiceMock
                .Setup(x => x.GetPropertyAsync(property.Id, UserId))
                .ReturnsAsync(property);

            // Act
            var actionResult = await _controller.Index(property.Id);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actionResult);

            Assert.Equal(property.Tenants, viewResult.Model);
            Assert.Equal(property.Id, _controller.ViewBag.PropertyId);

            _propertyServiceMock.Verify(x => x.GetPropertyAsync(property.Id, UserId));
        }

        [Fact]
        public async Task Create_PropertyExists_ReturnsView()
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
            var actionResult = await _controller.Create(property.Id);

            // Assert
            Assert.IsType<ViewResult>(actionResult);

            Assert.Equal(property.Id, _controller.ViewBag.PropertyId);

            _propertyServiceMock.Verify(x => x.GetPropertyAsync(property.Id, UserId));
        }

        [Fact]
        public async Task Create_ValidTenant_RedirectsToIndex()
        {
            // Arrange
            var property = new Property
            {
                Id = 1
            };

            var tenant = new Tenant
            {
                Id = 1,
                LeaseStartDate = DateOnly.FromDateTime(DateTime.Today),
                LeaseEndDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(12))
            };

            _propertyServiceMock
                .Setup(x => x.GetPropertyAsync(property.Id, UserId))
                .ReturnsAsync(property);

            // Act
            var actionResult = await _controller.Create(property.Id, tenant);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(actionResult);

            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Tenant", redirectResult.ControllerName);
            Assert.Equal(property.Id, redirectResult.RouteValues!["propertyId"]);
            Assert.Equal(property.Id, tenant.PropertyId);

            _tenantServiceMock.Verify(x => x.CreateTenantAsync(tenant));
        }

        [Fact]
        public async Task Edit_ExistingTenant_ReturnsViewWithTenant()
        {
            // Arrange
            var tenant = new Tenant
            {
                Id = 1
            };

            _tenantServiceMock
                .Setup(x => x.GetTenantAsync(tenant.Id))
                .ReturnsAsync(tenant);

            // Act
            var actionResult = await _controller.Edit(tenant.Id);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actionResult);

            Assert.Equal(tenant, viewResult.Model);

            _tenantServiceMock.Verify(x => x.GetTenantAsync(tenant.Id));
        }

        [Fact]
        public async Task Edit_ValidTenant_RedirectsToIndex()
        {
            // Arrange
            var tenant = new Tenant
            {
                Id = 1,
                PropertyId = 1,
                LeaseStartDate = DateOnly.FromDateTime(DateTime.Today),
                LeaseEndDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(12))
            };

            _tenantServiceMock
                .Setup(x => x.UpdateTenantAsync(tenant))
                .ReturnsAsync(true);

            // Act
            var actionResult = await _controller.Edit(tenant);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(actionResult);

            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Tenant", redirectResult.ControllerName);
            Assert.Equal(tenant.PropertyId, redirectResult.RouteValues!["propertyId"]);

            _tenantServiceMock.Verify(x => x.UpdateTenantAsync(tenant));
        }

        [Fact]
        public async Task Delete_ExistingTenant_RedirectsToIndex()
        {
            // Arrange
            var tenant = new Tenant
            {
                Id = 1,
                PropertyId = 1
            };

            _tenantServiceMock
                .Setup(x => x.GetTenantAsync(tenant.Id))
                .ReturnsAsync(tenant);

            // Act
            var actionResult = await _controller.Delete(tenant.Id);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(actionResult);

            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Tenant", redirectResult.ControllerName);
            Assert.Equal(tenant.PropertyId, redirectResult.RouteValues!["propertyId"]);

            _tenantServiceMock.Verify(x => x.GetTenantAsync(tenant.Id));
            _tenantServiceMock.Verify(x => x.DeleteTenantAsync(tenant.Id));
        }
    }
}
