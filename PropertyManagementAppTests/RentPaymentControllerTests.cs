using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PropertyManagementApp.Controllers;
using PropertyManagementApp.Models;
using PropertyManagementApp.Services;
using System.Security.Claims;

namespace PropertyManagementAppTests
{
    public class RentPaymentControllerTests
    {
        private readonly Mock<IRentPaymentService> _rentPaymentServiceMock;
        private readonly Mock<IPropertyService> _propertyServiceMock;
        private readonly RentPaymentController _controller;

        private const string UserId = "landlord1";

        public RentPaymentControllerTests()
        {
            _rentPaymentServiceMock = new Mock<IRentPaymentService>();
            _propertyServiceMock = new Mock<IPropertyService>();

            _controller = new RentPaymentController(_rentPaymentServiceMock.Object, _propertyServiceMock.Object);

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
        public async Task Index_PropertyExists_ReturnsViewWithRentPayments()
        {
            // Arrange
            var property = new Property
            {
                Id = 1,
                Tenants = new List<Tenant>
                {
                    new Tenant
                    {
                        Id = 1,
                        FirstName = "TenantName1"
                    },
                    new Tenant
                    {
                        Id = 2,
                        FirstName = "TenantName2"
                    }
                }
            };

            var rentPayments = new List<RentPayment>
            {
                new RentPayment
                {
                    Id = 1,
                    PropertyId = property.Id,
                    TenantName = "TenantName1"
                },
                new RentPayment
                {
                    Id = 2,
                    PropertyId = property.Id,
                    TenantName = "TenantName2"
                }
            };

            _propertyServiceMock
                .Setup(x => x.GetPropertyAsync(property.Id, UserId))
                .ReturnsAsync(property);

            _rentPaymentServiceMock
                .Setup(x => x.GetRentPaymentsForPropertyAsync(property.Id))
                .ReturnsAsync(rentPayments);

            // Act
            var actionResult = await _controller.Index(property.Id, null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actionResult);

            Assert.Equal(rentPayments, viewResult.Model);
            Assert.Equal(property.Id, _controller.ViewBag.PropertyId);
            Assert.Equal(property.Tenants, _controller.ViewBag.Tenants);

            _propertyServiceMock.Verify(x => x.GetPropertyAsync(property.Id, UserId));
            _rentPaymentServiceMock.Verify(x => x.GetRentPaymentsForPropertyAsync(property.Id));
        }

        [Fact]
        public async Task Index_TenantNameProvided_ReturnsFilteredRentPayments()
        {
            // Arrange
            var property = new Property
            {
                Id = 1,
                Tenants = new List<Tenant>
                {
                    new Tenant
                    {
                        Id = 1,
                        FirstName = "TenantName1"
                    },
                    new Tenant
                    {
                        Id = 2,
                        FirstName = "TenantName2"
                    }
                }
            };

            var rentPayments = new List<RentPayment>
            {
                new RentPayment
                {
                    Id = 1,
                    PropertyId = property.Id,
                    TenantName = "TenantName1"
                },
                new RentPayment
                {
                    Id = 2,
                    PropertyId = property.Id,
                    TenantName = "TenantName2"
                }
            };

            _propertyServiceMock
                .Setup(x => x.GetPropertyAsync(property.Id, UserId))
                .ReturnsAsync(property);

            _rentPaymentServiceMock
                .Setup(x => x.GetRentPaymentsForPropertyAsync(property.Id))
                .ReturnsAsync(rentPayments);

            // Act
            var actionResult = await _controller.Index(property.Id, "TenantName1");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actionResult);

            var filteredRentPayments = Assert.IsType<List<RentPayment>>(viewResult.Model);

            Assert.Single(filteredRentPayments);
            Assert.Equal("TenantName1", filteredRentPayments[0].TenantName);
            Assert.Equal("TenantName1", _controller.ViewBag.TenantName);
            Assert.Equal(property.Id, _controller.ViewBag.PropertyId);

            _rentPaymentServiceMock.Verify(x => x.GetRentPaymentsForPropertyAsync(property.Id));
        }

        [Fact]
        public async Task Create_PropertyExists_ReturnsView()
        {
            // Arrange
            var property = new Property
            {
                Id = 1,
                Tenants = new List<Tenant>
                {
                    new Tenant
                    {
                        Id = 1,
                        FirstName = "TenantName1"
                    }
                }
            };

            _propertyServiceMock
                .Setup(x => x.GetPropertyAsync(property.Id, UserId))
                .ReturnsAsync(property);

            // Act
            var actionResult = await _controller.Create(property.Id);

            // Assert
            Assert.IsType<ViewResult>(actionResult);

            Assert.Equal(property.Id, _controller.ViewBag.PropertyId);
            Assert.Equal(property.Tenants, _controller.ViewBag.Tenants);

            _propertyServiceMock.Verify(x => x.GetPropertyAsync(property.Id, UserId));
        }

        [Fact]
        public async Task Create_ValidRentPayment_RedirectsToIndex()
        {
            // Arrange
            var rentPayment = new RentPayment
            {
                Id = 1,
                PropertyId = 1,
                TenantName = "TenantName1"
            };

            // Act
            var actionResult = await _controller.Create(rentPayment);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(actionResult);

            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal(rentPayment.PropertyId, redirectResult.RouteValues!["propertyId"]);

            _rentPaymentServiceMock.Verify(x => x.CreateRentPaymentAsync(rentPayment));
        }

        [Fact]
        public async Task RentPaid_RentPaymentExists_RedirectsToIndex()
        {
            // Arrange
            var rentPaymentId = 1;
            var propertyId = 1;

            _rentPaymentServiceMock
                .Setup(x => x.RentPaidAsync(rentPaymentId))
                .ReturnsAsync(true);

            // Act
            var actionResult = await _controller.RentPaid(rentPaymentId, propertyId);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(actionResult);

            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal(propertyId, redirectResult.RouteValues!["propertyId"]);

            _rentPaymentServiceMock.Verify(x => x.RentPaidAsync(rentPaymentId));
        }

        [Fact]
        public async Task CancelRentPayment_RentPaymentExists_RedirectsToIndex()
        {
            // Arrange
            var rentPaymentId = 1;
            var propertyId = 1;

            _rentPaymentServiceMock
                .Setup(x => x.CancelRentPaymentAsync(rentPaymentId))
                .ReturnsAsync(true);

            // Act
            var actionResult = await _controller.CancelRentPayment(rentPaymentId, propertyId);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(actionResult);

            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal(propertyId, redirectResult.RouteValues!["propertyId"]);

            _rentPaymentServiceMock.Verify(x => x.CancelRentPaymentAsync(rentPaymentId));
        }
    }
}
