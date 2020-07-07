using AutoMapper;
using Bogus;
using DreamFoodDelivery.Common;
using DreamFoodDelivery.Data.Context;
using DreamFoodDelivery.Data.Models;
using DreamFoodDelivery.Domain.DTO;
using DreamFoodDelivery.Domain.Logic.Services;
using DreamFoodDelivery.Domain.View;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace DreamFoodDelivery.Logic.Tests
{
    public class ReviewServiceTest
    {
        private Faker<UserDB> _fakeUser = new Faker<UserDB>()
            .RuleFor(x => x.IdFromIdentity, y => y.Random.Guid().ToString());
        private readonly Faker<ReviewDB> _fakeReview = new Faker<ReviewDB>()
            .RuleFor(x => x.Headline, y => y.Random.Word())
            .RuleFor(x => x.Rating, y => y.Random.Byte(0, 5))
            .RuleFor(x => x.Content, y => y.Random.Words());
        private readonly Faker<OrderDB> _fakeOrder = new Faker<OrderDB>()
            .RuleFor(x => x.Address, y => y.Random.Words())
            .RuleFor(x => x.PhoneNumber, y => y.Person.Phone)
            .RuleFor(x => x.Name, y => y.Name.FirstName())
            .RuleFor(x => x.Surname, y => y.Name.LastName())
            .RuleFor(x => x.Status, y => y.Random.Words());

        readonly List<UserDB> _users;
        readonly List<ReviewDB> _reviews;
        readonly List<OrderDB> _orders;
        readonly IMapper _mapper;

        public ReviewServiceTest()
        {
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ReviewDB, ReviewForUsersView>().ReverseMap();
                cfg.CreateMap<ReviewDB, ReviewView>().ReverseMap();
                cfg.CreateMap<ReviewDB, ReviewToUpdate>().ReverseMap();
            });

            _mapper = new Mapper(mapperConfiguration);
            _orders = _fakeOrder.Generate(2);
            _reviews = _fakeReview.Generate(2);
            _users = _fakeUser.Generate(2);
        }

        [Fact]
        public async void Reviews_GetAllAsync_Test()
        {
            var options = new DbContextOptionsBuilder<DreamFoodDeliveryContext>()
                .UseInMemoryDatabase(databaseName: "Reviews_GetAllAsync_Test")
                .Options;

            // Run the test against one instance of the context
            using (var context = new DreamFoodDeliveryContext(options))
            {
                context.AddRange(_orders);
                await context.SaveChangesAsync();

                var order = context.Orders.AsNoTracking().FirstOrDefault();
                var review = _reviews.FirstOrDefault();
                review.UserId = order.UserId;
                review.OrderId = order.Id.GetValueOrDefault();

                context.AddRange(review);
                await context.SaveChangesAsync();
            }

            // Use a separate instance of the context to verify correct data was saved to database
            using (var context = new DreamFoodDeliveryContext(options))
            {
                var service = new ReviewService(_mapper, context);

                var reviewsInBase = await context.Reviews.AsNoTracking().ToListAsync();

                var result = await service.GetAllAsync(new PageRequest());

                foreach (var item in reviewsInBase)
                {
                    var itemFromResult = result.Data.Data.Where(_ => _.Headline.Equals(item.Headline, StringComparison.OrdinalIgnoreCase)).Select(_ => _).FirstOrDefault();
                    itemFromResult.Should().NotBeNull();
                }
            }
        }

        [Fact]
        public async void Reviews_GetAllAdminAsync_Test()
        {
            var options = new DbContextOptionsBuilder<DreamFoodDeliveryContext>()
                .UseInMemoryDatabase(databaseName: "Reviews_GetAllAdminAsync_Test")
                .Options;

            // Run the test against one instance of the context
            using (var context = new DreamFoodDeliveryContext(options))
            {
                context.AddRange(_reviews);
                await context.SaveChangesAsync();
            }

            // Use a separate instance of the context to verify correct data was saved to database
            using (var context = new DreamFoodDeliveryContext(options))
            {
                var service = new ReviewService(_mapper, context);

                var reviewInBase = await context.Reviews.AsNoTracking().ToListAsync();

                var result = await service.GetAllAdminAsync();

                foreach (var item in reviewInBase)
                {
                    var itemFromResult = result.Data.Where(_ => _.Headline.Equals(item.Headline, StringComparison.OrdinalIgnoreCase)).Select(_ => _).FirstOrDefault();
                    itemFromResult.Should().NotBeNull();
                }
            }
        }

        [Fact]
        public async void Reviews_GetByIdAsync_PositiveAndNegative_Test()
        {
            var options = new DbContextOptionsBuilder<DreamFoodDeliveryContext>()
                .UseInMemoryDatabase(databaseName: "Reviews_GetByIdAsync_PositiveAndNegative_Test")
                .Options;

            using (var context = new DreamFoodDeliveryContext(options))
            {
                context.AddRange(_reviews);
                await context.SaveChangesAsync();
            }

            using (var context = new DreamFoodDeliveryContext(options))
            {
                var review = await context.Reviews.AsNoTracking().FirstOrDefaultAsync();

                var service = new ReviewService(_mapper, context);

                var resultPositive = await service.GetByIdAsync(review.Id.ToString());
                var resultNegative = await service.GetByIdAsync(new Guid().ToString());

                resultPositive.IsSuccess.Should().BeTrue();
                resultPositive.Data.Headline.Should().BeEquivalentTo(review.Headline);

                resultNegative.IsSuccess.Should().BeFalse();
                resultNegative.Data.Should().BeNull();
            }
        }

        [Fact]
        public async void Reviews_GetByUserIdAsync_PositiveAndNegative_Test()
        {
            var options = new DbContextOptionsBuilder<DreamFoodDeliveryContext>()
                .UseInMemoryDatabase(databaseName: "Reviews_GetByUserIdAsync_PositiveAndNegative_Test")
                .Options;

            UserDB userWithReviews;

            using (var context = new DreamFoodDeliveryContext(options))
            {
                context.AddRange(_users);
                await context.SaveChangesAsync();

                userWithReviews = context.Users.AsNoTracking().FirstOrDefault();

                foreach (var review in _reviews)
                {
                    review.UserId = userWithReviews.Id;
                    review.OrderId = new Guid();
                }

                context.AddRange(_reviews);
                await context.SaveChangesAsync();
            }

            using (var context = new DreamFoodDeliveryContext(options))
            {
                var service = new ReviewService(_mapper, context);

                var reviewsInBase = await context.Reviews.AsNoTracking().ToListAsync();
                var userWithoutReviews = await context.Users.Where(_ => _.Id != userWithReviews.Id).FirstOrDefaultAsync();

                var resultPositive = await service.GetByUserIdAsync(userWithReviews.IdFromIdentity.ToString());
                var resultNegative = await service.GetByUserIdAsync(userWithoutReviews.IdFromIdentity.ToString());

                foreach (var review in reviewsInBase)
                {
                    resultPositive.Data
                        .Where(_ => _.Id == review.Id)
                        .FirstOrDefault()
                        .Should().NotBeNull();
                }

                resultNegative.Data.Should().BeNullOrEmpty();
            }
        }

        [Fact]
        public async void Reviews_RemoveAllAsync_Test()
        {
            var options = new DbContextOptionsBuilder<DreamFoodDeliveryContext>()
                .UseInMemoryDatabase(databaseName: "Reviews_RemoveAllAsync_Test")
                .Options;

            using (var context = new DreamFoodDeliveryContext(options))
            {
                context.AddRange(_reviews);
                int sum = 0;
                for (int i = 0; i < _reviews.Count; i++)
                {
                    sum += _reviews[i].Rating.Value;
                }
                RatingDB rating = new RatingDB()
                {
                    Sum = sum,
                    Count = _reviews.Count
                };
                context.Add(rating);
                await context.SaveChangesAsync();
            }

            using (var context = new DreamFoodDeliveryContext(options))
            {
                var service = new ReviewService(_mapper, context);

                var resultPositive = await service.RemoveAllAsync();

                resultPositive.IsSuccess.Should().BeTrue();
                resultPositive.Message.Should().BeNull();
            }
        }

        [Fact]
        public async void Reviews_RemoveAllByUserIdAsync_PositiveAndNegative_Test()
        {
            var options = new DbContextOptionsBuilder<DreamFoodDeliveryContext>()
                .UseInMemoryDatabase(databaseName: "Reviews_RemoveAllByUserIdAsync_PositiveAndNegative_Test")
                .Options;

            UserDB userWithReviews;

            using (var context = new DreamFoodDeliveryContext(options))
            {
                context.AddRange(_users);
                await context.SaveChangesAsync();

                userWithReviews = context.Users.AsNoTracking().FirstOrDefault();

                foreach (var review in _reviews)
                {
                    review.UserId = userWithReviews.Id;
                    review.OrderId = new Guid();
                }
                int sum = 0;
                for (int i = 0; i < _reviews.Count; i++)
                {
                    sum += _reviews[i].Rating.Value;
                }
                RatingDB rating = new RatingDB()
                {
                    Sum = sum,
                    Count = _reviews.Count
                };
                context.Add(rating);
                context.AddRange(_reviews);
                await context.SaveChangesAsync();
            }

            using (var context = new DreamFoodDeliveryContext(options))
            {
                //var review = await context.Reviews.AsNoTracking().FirstOrDefaultAsync();

                var service = new ReviewService(_mapper, context);

                var resultPositive = await service.RemoveAllByUserIdAsync(userWithReviews.Id.ToString());
                var resultNegative = await service.RemoveAllByUserIdAsync(new Guid().ToString());

                resultPositive.IsSuccess.Should().BeTrue();
                resultPositive.Message.Should().BeNull();

                resultNegative.IsSuccess.Should().BeFalse();
                resultNegative.Message.Should().Contain(ExceptionConstants.REVIEWS_WERE_NOT_FOUND);
            }
        }

        [Fact]
        public async void Reviews_RemoveByIdAsync_PositiveAndNegative_Test()
        {
            var options = new DbContextOptionsBuilder<DreamFoodDeliveryContext>()
                .UseInMemoryDatabase(databaseName: "Reviews_RemoveByIdAsync_PositiveAndNegative_Test")
                .Options;

            using (var context = new DreamFoodDeliveryContext(options))
            {
                context.AddRange(_reviews);
                int sum = 0;
                for (int i = 0; i < _reviews.Count; i++)
                {
                    sum += _reviews[i].Rating.Value;
                }
                RatingDB rating = new RatingDB()
                {
                    Sum = sum,
                    Count = _reviews.Count
                };
                context.Add(rating);
                await context.SaveChangesAsync();
            }

            using (var context = new DreamFoodDeliveryContext(options))
            {
                var review = await context.Reviews.AsNoTracking().FirstOrDefaultAsync();

                var service = new ReviewService(_mapper, context);

                var resultPositive = await service.RemoveByIdAsync(review.Id.ToString());
                var resultNegative = await service.RemoveByIdAsync(new Guid().ToString());

                resultPositive.IsSuccess.Should().BeTrue();
                resultPositive.Message.Should().BeNull();

                resultNegative.IsSuccess.Should().BeFalse();
                resultNegative.Message.Should().Contain(ExceptionConstants.REVIEW_WAS_NOT_FOUND);
            }
        }

        [Fact]
        public async void Reviews_UpdateAsync_PositiveAndNegative_Test()
        {
            var options = new DbContextOptionsBuilder<DreamFoodDeliveryContext>()
                .UseInMemoryDatabase(databaseName: "Reviews_UpdateAsync_PositiveAndNegative_Test")
                .Options;

            using (var context = new DreamFoodDeliveryContext(options))
            {
                context.AddRange(_reviews);
                int sum = 0;
                for (int i = 0; i < _reviews.Count; i++)
                {
                    sum += _reviews[i].Rating.Value;
                }
                RatingDB rating = new RatingDB()
                {
                    Sum = sum,
                    Count = _reviews.Count
                };
                context.Add(rating);
                await context.SaveChangesAsync();
            }

            using (var context = new DreamFoodDeliveryContext(options))
            {
                var review = await context.Reviews.AsNoTracking().FirstOrDefaultAsync();
                var rating = await context.Rating.AsNoTracking().FirstOrDefaultAsync();

                var service = new ReviewService(_mapper, context);

                ReviewToUpdate updateReview = new ReviewToUpdate()
                {
                    Id = review.Id.ToString(),
                    Headline = "Headline",
                    Content = "Content",
                    Rating = 1
                };

                ReviewToUpdate failReview = new ReviewToUpdate()
                {
                    Id = new Guid().ToString(),
                    Headline = "Headline",
                    Content = "Content",
                    Rating = 0
                };

                var resultPositive = await service.UpdateAsync(updateReview);
                var resultNegative = await service.UpdateAsync(failReview);

                resultPositive.IsSuccess.Should().BeTrue();
                resultPositive.Data.Headline.Should().BeEquivalentTo(updateReview.Headline);
                resultPositive.Data.Headline.Should().NotBeEquivalentTo(review.Headline);

                resultNegative.IsSuccess.Should().BeFalse();
            }
        }
    }
}
