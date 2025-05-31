using Microsoft.EntityFrameworkCore;
using MiniGame.Data;
using MiniGame.Models;
using MiniGame.Services;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Minigame.Tests.Services
{
    [TestFixture]
    public class ResultServiceTests
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

        private async Task<Result> AddResultToContext(AppDbContext context, int id, int gameId, int userId, DateTime playTime, int points, int duration, int level)
        {
            var result = new Result
            {
                GameId = gameId,
                UserId = userId,
                PlayTime = playTime,
                Points = points,
                Duration = duration,
                Level = level
            };
            context.Results.Add(result);
            await context.SaveChangesAsync();
            return result;
        }

        private async Task<AppDbContext> GetDbContextWithInitialDataAsync()
        {
            var context = await GetDbContextAsync();
            await AddResultToContext(context, 1, 1, 101, DateTime.UtcNow.AddHours(-1), 100, 60, 1);
            await AddResultToContext(context, 2, 1, 102, DateTime.UtcNow.AddMinutes(-30), 150, 75, 2);
            await AddResultToContext(context, 3, 2, 101, DateTime.UtcNow.AddMinutes(-10), 200, 90, 3);
            return context;
        }

        [Test]
        public async Task GetResultsAsync_ShouldReturnAllResults()
        {
            var dbContext = await GetDbContextWithInitialDataAsync();
            var resultService = new ResultService(dbContext);

            var results = await resultService.GetResultsAsync();

            Assert.IsNotNull(results);
            Assert.AreEqual(3, results.Count());
            Assert.IsTrue(results.Any(r => r.GameId == 1 && r.UserId == 101 && r.Points == 100));
            Assert.IsTrue(results.Any(r => r.GameId == 1 && r.UserId == 102 && r.Points == 150));
            Assert.IsTrue(results.Any(r => r.GameId == 2 && r.UserId == 101 && r.Points == 200));
        }

        [Test]
        public async Task GetResultsAsync_ShouldReturnEmptyList_WhenNoResultsExist()
        {
            var dbContext = await GetDbContextAsync();
            var resultService = new ResultService(dbContext);

            var results = await resultService.GetResultsAsync();

            Assert.IsNotNull(results);
            Assert.IsEmpty(results);
        }

        [Test]
        public async Task CreateResultAsync_ShouldCreateNewResult()
        {
            var dbContext = await GetDbContextAsync();
            var resultService = new ResultService(dbContext);

            var saveResultData = new ResultService.SaveResult
            {
                GameId = 3,
                UserId = 103,
                PlayTime = DateTime.UtcNow,
                Points = 500,
                Duration = 120,
                Level = 5
            };

            var createdResult = await resultService.CreateResultAsync(saveResultData);

            Assert.IsNotNull(createdResult);
            Assert.Greater(createdResult.Id, 0);
            Assert.AreEqual(saveResultData.GameId, createdResult.GameId);
            Assert.AreEqual(saveResultData.UserId, createdResult.UserId);
            Assert.AreEqual(saveResultData.Points, createdResult.Points);
            Assert.AreEqual(saveResultData.Duration, createdResult.Duration);
            Assert.AreEqual(saveResultData.Level, createdResult.Level);

            var resultInDb = await dbContext.Results.FindAsync(createdResult.Id);
            Assert.IsNotNull(resultInDb);
            Assert.AreEqual(createdResult.Points, resultInDb.Points);
        }

        [Test]
        public async Task GetResultByIdAsync_ShouldReturnResult_WhenResultExists()
        {
            var dbContext = await GetDbContextWithInitialDataAsync();
            var resultService = new ResultService(dbContext);

            var expectedResult = await dbContext.Results.FirstAsync(r => r.Id == 2);

            var result = await resultService.GetResultByIdAsync(2);

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResult.Id, result.Id);
            Assert.AreEqual(expectedResult.Points, result.Points);
        }

        [Test]
        public async Task GetResultByIdAsync_ShouldReturnNull_WhenResultDoesNotExist()
        {
            var dbContext = await GetDbContextWithInitialDataAsync();
            var resultService = new ResultService(dbContext);

            var result = await resultService.GetResultByIdAsync(999);

            Assert.IsNull(result);
        }
    }
}