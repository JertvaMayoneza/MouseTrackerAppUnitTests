using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MouseTrakerApp.Controllers;
using MouseTrakerApp.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;

namespace MouseTrackerApp.Tests
{
    public class MouseControllerTests
    {
        private readonly AppDbContext _context;
        private readonly MouseController _controller;

        public MouseControllerTests()
        {
            _context = MockAppDbContext.Create(); 
            _controller = new MouseController(_context); 
        }

        [Fact]
        public async Task SaveCoordinates_ReturnsOk_WhenDataIsValid()
        {
 
            var coordinates = new List<MouseData>
            {
                new MouseData { X = 100, Y = 200, T = DateTime.Now.Ticks }
            };

            var result = await _controller.SaveCoordinates(coordinates);

 
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Данные сохранены", okResult.Value);
            Assert.Single(await _context.MouseMovements.ToListAsync());
        }

        [Fact]
        public async Task SaveCoordinates_ReturnsBadRequest_WhenDataIsEmpty()
        {
            var coordinates = new List<MouseData>();

            var result = await _controller.SaveCoordinates(coordinates);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Нет данных для сохранения", badRequestResult.Value);
        }

        [Fact]
        public async Task SaveCoordinates_ReturnsInternalServerError_OnException()
        {
            
            var mockSet = new Mock<DbSet<MouseMovement>>();
            var mockContext = new Mock<AppDbContext>();
            mockContext.Setup(m => m.MouseMovements).Returns(mockSet.Object);

            var controller = new MouseController(mockContext.Object);

            // Мы симулируем, что при попытке сохранить данные произойдет ошибка
            mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new DbUpdateException("Ошибка базы данных"));

            var coordinates = new List<MouseData> { new MouseData { X = 10, Y = 20, T = 123456789 } };
            
            var result = await controller.SaveCoordinates(coordinates);

            var internalServerErrorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, internalServerErrorResult.StatusCode);
            Assert.StartsWith("Ошибка на сервере:", (string)internalServerErrorResult.Value);
        }


    }
}
