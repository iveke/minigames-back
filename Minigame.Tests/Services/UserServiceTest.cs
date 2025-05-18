using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MiniGame.Data;
using MiniGame.Models;
using MiniGame.Services;
using Moq;
using Xunit;

namespace MyApp.Tests.Services
{
    public class UserServiceTests
    {
        private async Task<AppDbContext> GetDbContextWithDataAsync()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // унікальна база для кожного тесту
                .Options;

            var context = new AppDbContext(options);

            // додати початкового користувача
            context.Users.Add(
                new User
                {
                    Id = 1,
                    Username = "olduser",
                    Email = "test@example.com",
                    Phone = "12345",
                    Age = 20,
                    Country = "Ukraine",
                    Role = RoleEnum.USER,
                    ConfirmEmail = false,
                }
            );

            await context.SaveChangesAsync();

            return context;
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldUpdateFields_WhenValidData()
        {
            // Arrange
            var dbContext = await GetDbContextWithDataAsync();

            var mockEmailService = new Mock<EmailService>(null); // якщо EmailService має залежності — замокати

            var userService = new UserService(dbContext, mockEmailService.Object);

            var user = await dbContext.Users.FirstAsync();

            var updatePayload = new UpdateModel
            {
                username = "newuser",
                phone = "98765",
                Age = 25,
                Country = "Poland",
            };

            // Act
            var updatedUser = await userService.UpdateUserAsync(user, updatePayload);

            // Assert
            Assert.Equal("newuser", updatedUser.Username);
            Assert.Equal("98765", updatedUser.Phone);
            Assert.Equal(25, updatedUser.Age);
            Assert.Equal("Poland", updatedUser.Country);
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldNotChangeUsername_IfAlreadyTaken()
        {
            // Arrange
            var dbContext = await GetDbContextWithDataAsync();

            dbContext.Users.Add(
                new User
                {
                    Id = 2,
                    Username = "takenname",
                    Email = "taken@example.com",
                }
            );
            await dbContext.SaveChangesAsync();

            var mockEmailService = new Mock<EmailService>(null);

            var userService = new UserService(dbContext, mockEmailService.Object);

            var user = await dbContext.Users.FirstAsync();

            var updatePayload = new UpdateModel
            {
                username = "takenname", // вже існує в базі
            };

            // Act
            var updatedUser = await userService.UpdateUserAsync(user, updatePayload);

            // Assert
            Assert.Equal("olduser", updatedUser.Username); // username не має змінитися
        }
    }
}
