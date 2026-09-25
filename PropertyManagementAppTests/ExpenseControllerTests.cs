using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PropertyManagementApp.Controllers;
using PropertyManagementApp.Models;
using PropertyManagementApp.Services;
using System.Security.Claims;

namespace PropertyManagementAppTests
{
    public class ExpenseControllerTests
    {
        private readonly Mock<IExpenseService> _expenseServiceMock;
        private readonly Mock<IPropertyService> _propertyServiceMock;
        private readonly ExpenseController _controller;

        private const string UserId = "landlord1";

        public ExpenseControllerTests()
        {
            _expenseServiceMock = new Mock<IExpenseService>();
            _propertyServiceMock = new Mock<IPropertyService>();

            _controller = new ExpenseController(_expenseServiceMock.Object, _propertyServiceMock.Object);

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
        public async Task Index_PropertyExists_ReturnsViewWithExpenses()
        {
            var property = new Property
            {
                Id = 1
            };

            var expenses = new List<Expense>
            {
                new Expense
                {
                    Id = 1,
                    PropertyId = property.Id
                },
                new Expense
                {
                    Id = 2,
                    PropertyId = property.Id
                }
            };

            _propertyServiceMock
                .Setup(x => x.GetPropertyAsync(property.Id, UserId))
                .ReturnsAsync(property);

            _expenseServiceMock
                .Setup(x => x.GetExpensesForPropertyAsync(property.Id))
                .ReturnsAsync(expenses);

            var actionResult = await _controller.Index(property.Id);

            var viewResult = Assert.IsType<ViewResult>(actionResult);

            Assert.Equal(expenses, viewResult.Model);
            Assert.Equal(property.Id, _controller.ViewBag.PropertyId);

            _propertyServiceMock.Verify(x => x.GetPropertyAsync(property.Id, UserId));

            _expenseServiceMock.Verify(x => x.GetExpensesForPropertyAsync(property.Id));
        }

        [Fact]
        public async Task Create_PropertyExists_ReturnsView()
        {
            var property = new Property
            {
                Id = 1
            };

            _propertyServiceMock
                .Setup(x => x.GetPropertyAsync(property.Id, UserId))
                .ReturnsAsync(property);

            var actionResult = await _controller.Create(property.Id);

            Assert.IsType<ViewResult>(actionResult);

            Assert.Equal(property.Id, _controller.ViewBag.PropertyId);

            _propertyServiceMock.Verify(x => x.GetPropertyAsync(property.Id, UserId));
        }

        [Fact]
        public async Task Create_ValidExpense_RedirectsToIndex()
        {
            var property = new Property
            {
                Id = 1
            };

            var expense = new Expense
            {
                Id = 1,
                PropertyId = property.Id
            };

            var actionResult = await _controller.Create(expense);

            var redirectResult = Assert.IsType<RedirectToActionResult>(actionResult);

            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal(property.Id, redirectResult.RouteValues!["propertyId"]);

            _expenseServiceMock.Verify(x => x.CreateExpenseAsync(expense));
        }
    }
}
