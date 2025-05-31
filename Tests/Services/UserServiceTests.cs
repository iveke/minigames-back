using Microsoft.EntityFrameworkCore;
using MiniGame.Data;
using MiniGame.Models;
using MiniGame.Services;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;



namespace Minigame.Tests.Services


{
    [TestFixture]
    public class UserServiceTests
    {

        private async Task<AppDbContext> GetDbContextAsync()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new AppDbContext(options);
            await context.Database.EnsureCreatedAsync();
            return context;
        }

        private async Task<User> AddUserToContext(AppDbContext context, int id, string username, string email, string password, RoleEnum role = RoleEnum.USER, string? emailVerificationCode = null, bool confirmEmail = false)
        {
            var user = new User
            {
                Id = id,
                Username = username,
                Email = email,
                Password = password,
                Role = role,
                EmailVerificationCode = emailVerificationCode,
                ConfirmEmail = confirmEmail
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();
            return user;
        }


        private async Task<AppDbContext> GetDbContextWithInitialDataAsync()
        {
            var context = await GetDbContextAsync();
            await AddUserToContext(context, 1, "olduser", "test@example.com", "SecurePassword123!", RoleEnum.USER, "12345");
            await AddUserToContext(context, 2, "takenname", "taken@example.com", "SecurePassword123!");
            await AddUserToContext(context, 3, "adminuser", "admin@example.com", "AdminPass123!", RoleEnum.ADMIN);
            return context;
        }

        // --- Тести для GetUsersAsync ---
        [Test]
        public async Task GetUsersAsync_ShouldReturnAllUsers()
        {
            var dbContext = await GetDbContextWithInitialDataAsync();
            var mockEmailService = new Mock<EmailService>();
            var userService = new UserService(dbContext, mockEmailService.Object);

            var result = await userService.GetUsersAsync();

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count());
            Assert.IsTrue(result.Any(u => u.Username == "olduser"));
            Assert.IsTrue(result.Any(u => u.Username == "takenname"));
            Assert.IsTrue(result.Any(u => u.Username == "adminuser"));
        }

        [Test]
        public async Task GetUsersAsync_ShouldReturnEmptyList_WhenNoUsersExist()
        {
            var dbContext = await GetDbContextAsync();
            var mockEmailService = new Mock<EmailService>();
            var userService = new UserService(dbContext, mockEmailService.Object);

            var result = await userService.GetUsersAsync();

            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        // --- Тести для GetUserByIdAsync ---
        [Test]
        public async Task GetUserByIdAsync_ShouldReturnUser_WhenUserExists()
        {
            var dbContext = await GetDbContextWithInitialDataAsync();
            var mockEmailService = new Mock<EmailService>();
            var userService = new UserService(dbContext, mockEmailService.Object);

            var expectedUser = await dbContext.Users.FirstAsync(u => u.Id == 1);

            var result = await userService.GetUserByIdAsync(1);

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedUser.Id, result.Id);
            Assert.AreEqual(expectedUser.Username, result.Username);
        }

        [Test]
        public async Task GetUserByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            var dbContext = await GetDbContextWithInitialDataAsync();
            var mockEmailService = new Mock<EmailService>();
            var userService = new UserService(dbContext, mockEmailService.Object);

            var result = await userService.GetUserByIdAsync(999);

            Assert.IsNull(result);
        }

        // --- Тести для CreateUserAsync ---

        [Test]
        public async Task ConfirmEmailAsync_ShouldConfirmEmailAndClearCode()
        {
            var dbContext = await GetDbContextAsync();
            var userToConfirm = await AddUserToContext(dbContext, 1, "unconfirmed", "unconfirmed@example.com", "Pass123!", RoleEnum.USER, "TESTCODE123", false);

            var mockEmailService = new Mock<EmailService>();
            var userService = new UserService(dbContext, mockEmailService.Object);

            var result = await userService.ConfirmEmailAsync("TESTCODE123", userToConfirm);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.ConfirmEmail);
            Assert.IsNull(result.EmailVerificationCode);


            var userInDb = await dbContext.Users.FindAsync(userToConfirm.Id);
            Assert.IsNotNull(userInDb);
            Assert.IsTrue(userInDb.ConfirmEmail);
            Assert.IsNull(userInDb.EmailVerificationCode);
        }

        [Test]
        public async Task ConfirmEmailAsync_ShouldHandleAlreadyConfirmedEmail()
        {
            var dbContext = await GetDbContextAsync();
            var userAlreadyConfirmed = await AddUserToContext(dbContext, 1, "confirmed", "confirmed@example.com", "Pass123!", RoleEnum.USER, null, true);

            var mockEmailService = new Mock<EmailService>();
            var userService = new UserService(dbContext, mockEmailService.Object);

            var result = await userService.ConfirmEmailAsync("ANYCODE", userAlreadyConfirmed);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.ConfirmEmail);
            Assert.IsNull(result.EmailVerificationCode);
        }

    }
}