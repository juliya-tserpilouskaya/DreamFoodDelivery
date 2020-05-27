using AutoMapper;
using Bogus;
using DreamFoodDelivery.Common;
using DreamFoodDelivery.Common.Сonstants;
using DreamFoodDelivery.Data.Context;
using DreamFoodDelivery.Data.Models;
using DreamFoodDelivery.Domain.DTO;
using DreamFoodDelivery.Domain.Logic.InterfaceServices;
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
    public class CommentServiceTest
    {
        private Faker<UserDB> _fakeUser = new Faker<UserDB>()
            .RuleFor(x => x.IdFromIdentity, y => y.Random.Guid().ToString());
        private readonly Faker<CommentDB> _fakeComment = new Faker<CommentDB>()
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
        readonly List<CommentDB> _comments;
        readonly List<OrderDB> _orders;
        readonly IMapper _mapper;
        readonly IOrderService _orderService;

        public CommentServiceTest()
        {
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CommentDB, CommentForUsersView>().ReverseMap();
                cfg.CreateMap<CommentDB, CommentView>().ReverseMap();
                cfg.CreateMap<CommentDB, CommentToUpdate>().ReverseMap();
            });

            _mapper = new Mapper(mapperConfiguration);
            _orders = _fakeOrder.Generate(2);
            _comments = _fakeComment.Generate(2);
            _users = _fakeUser.Generate(2);

        }

        [Fact]
        public async void Comments_GetAllAsync_Test()
        {
            var options = new DbContextOptionsBuilder<DreamFoodDeliveryContext>()
                .UseInMemoryDatabase(databaseName: "Comments_GetAllAsync_Test")
                .Options;

            // Run the test against one instance of the context
            using (var context = new DreamFoodDeliveryContext(options))
            {
                context.AddRange(_orders);
                await context.SaveChangesAsync();

                var order = context.Orders.AsNoTracking().FirstOrDefault();
                var comment = _comments.FirstOrDefault();
                comment.UserId = order.UserId;
                comment.OrderId = order.Id.GetValueOrDefault();

                context.AddRange(comment);
                await context.SaveChangesAsync();
            }

            // Use a separate instance of the context to verify correct data was saved to database
            using (var context = new DreamFoodDeliveryContext(options))
            {
                var service = new CommentService(_mapper, context, _orderService);

                var commentsInBase = await context.Comments.AsNoTracking().ToListAsync();

                var result = await service.GetAllAsync(new PageRequest());

                foreach (var item in commentsInBase)
                {
                    var itemFromResult = result.Data.Data.Where(_ => _.Headline.Equals(item.Headline, StringComparison.OrdinalIgnoreCase)).Select(_ => _).FirstOrDefault();
                    itemFromResult.Should().NotBeNull();
                }
            }
        }

        [Fact]
        public async void Comments_GetAllAdminAsync_Test()
        {
            var options = new DbContextOptionsBuilder<DreamFoodDeliveryContext>()
                .UseInMemoryDatabase(databaseName: "Comments_GetAllAdminAsync_Test")
                .Options;

            // Run the test against one instance of the context
            using (var context = new DreamFoodDeliveryContext(options))
            {
                context.AddRange(_comments);
                await context.SaveChangesAsync();
            }

            // Use a separate instance of the context to verify correct data was saved to database
            using (var context = new DreamFoodDeliveryContext(options))
            {
                var service = new CommentService(_mapper, context, _orderService);

                var commentsInBase = await context.Comments.AsNoTracking().ToListAsync();

                var result = await service.GetAllAdminAsync();

                foreach (var item in commentsInBase)
                {
                    var itemFromResult = result.Data.Where(_ => _.Headline.Equals(item.Headline, StringComparison.OrdinalIgnoreCase)).Select(_ => _).FirstOrDefault();
                    itemFromResult.Should().NotBeNull();
                }
            }
        }

        [Fact]
        public async void Comments_GetByIdAsync_PositiveAndNegative_Test()
        {
            var options = new DbContextOptionsBuilder<DreamFoodDeliveryContext>()
                .UseInMemoryDatabase(databaseName: "Comments_GetByIdAsync_PositiveAndNegative_Test")
                .Options;

            using (var context = new DreamFoodDeliveryContext(options))
            {
                context.AddRange(_comments);
                await context.SaveChangesAsync();
            }

            using (var context = new DreamFoodDeliveryContext(options))
            {
                var comment = await context.Comments.AsNoTracking().FirstOrDefaultAsync();

                var service = new CommentService(_mapper, context, _orderService);

                var resultPositive = await service.GetByIdAsync(comment.Id.ToString());
                var resultNegative = await service.GetByIdAsync(new Guid().ToString());

                resultPositive.IsSuccess.Should().BeTrue();
                resultPositive.Data.Headline.Should().BeEquivalentTo(comment.Headline);

                resultNegative.IsSuccess.Should().BeFalse();
                resultNegative.Data.Should().BeNull();
            }
        }

        [Fact]
        public async void Comments_GetByUserIdAsync_PositiveAndNegative_Test()
        {
            var options = new DbContextOptionsBuilder<DreamFoodDeliveryContext>()
                .UseInMemoryDatabase(databaseName: "Comments_GetByUserIdAsync_PositiveAndNegative_Test")
                .Options;

            UserDB userWithComments;

            using (var context = new DreamFoodDeliveryContext(options))
            {
                context.AddRange(_users);
                await context.SaveChangesAsync();

                userWithComments = context.Users.AsNoTracking().FirstOrDefault();

                foreach (var comment in _comments)
                {
                    comment.UserId = userWithComments.Id;
                    comment.OrderId = new Guid();
                }

                context.AddRange(_comments);
                await context.SaveChangesAsync();
            }

            using (var context = new DreamFoodDeliveryContext(options))
            {
                var service = new CommentService(_mapper, context, _orderService);

                var commentsInBase = await context.Comments.AsNoTracking().ToListAsync();
                var userWithoutComments = await context.Users.Where(_ => _.Id != userWithComments.Id).FirstOrDefaultAsync();

                var resultPositive = await service.GetByUserIdAsync(userWithComments.IdFromIdentity.ToString());
                var resultNegative = await service.GetByUserIdAsync(userWithoutComments.IdFromIdentity.ToString());

                foreach (var comment in commentsInBase)
                {
                    resultPositive.Data
                        .Where(_ => _.Id == comment.Id)
                        .FirstOrDefault()
                        .Should().NotBeNull();
                }

                resultNegative.Data.Should().BeNullOrEmpty();
            }
        }

        [Fact]
        public async void Comments_RemoveAllAsync_Test()
        {
            var options = new DbContextOptionsBuilder<DreamFoodDeliveryContext>()
                .UseInMemoryDatabase(databaseName: "Comments_RemoveAllAsync_Test")
                .Options;

            using (var context = new DreamFoodDeliveryContext(options))
            {
                context.AddRange(_comments);
                await context.SaveChangesAsync();
            }

            using (var context = new DreamFoodDeliveryContext(options))
            {
                var comment = await context.Comments.AsNoTracking().FirstOrDefaultAsync();

                var service = new CommentService(_mapper, context, _orderService);

                var resultPositive = await service.RemoveAllAsync();

                resultPositive.IsSuccess.Should().BeTrue();
                resultPositive.Message.Should().BeNull();
            }
        }

        [Fact]
        public async void Comments_RemoveAllByUserIdAsync_PositiveAndNegative_Test()
        {
            var options = new DbContextOptionsBuilder<DreamFoodDeliveryContext>()
                .UseInMemoryDatabase(databaseName: "Comments_RemoveAllByUserIdAsync_PositiveAndNegative_Test")
                .Options;

            UserDB userWithComments;

            using (var context = new DreamFoodDeliveryContext(options))
            {
                context.AddRange(_users);
                await context.SaveChangesAsync();

                userWithComments = context.Users.AsNoTracking().FirstOrDefault();

                foreach (var comment in _comments)
                {
                    comment.UserId = userWithComments.Id;
                    comment.OrderId = new Guid();
                }

                context.AddRange(_comments);
                await context.SaveChangesAsync();
            }

            using (var context = new DreamFoodDeliveryContext(options))
            {
                var comment = await context.Comments.AsNoTracking().FirstOrDefaultAsync();

                var service = new CommentService(_mapper, context, _orderService);

                var resultPositive = await service.RemoveAllByUserIdAsync(userWithComments.Id.ToString());
                var resultNegative = await service.RemoveAllByUserIdAsync(new Guid().ToString());

                resultPositive.IsSuccess.Should().BeTrue();
                resultPositive.Message.Should().BeNull();

                resultNegative.IsSuccess.Should().BeFalse();
                resultNegative.Message.Should().Contain(ExceptionConstants.COMMENTS_WERE_NOT_FOUND);
            }
        }

        [Fact]
        public async void Comments_RemoveByIdAsync_PositiveAndNegative_Test()
        {
            var options = new DbContextOptionsBuilder<DreamFoodDeliveryContext>()
                .UseInMemoryDatabase(databaseName: "Comments_RemoveByIdAsync_PositiveAndNegative_Test")
                .Options;

            using (var context = new DreamFoodDeliveryContext(options))
            {
                context.AddRange(_comments);
                await context.SaveChangesAsync();
            }

            using (var context = new DreamFoodDeliveryContext(options))
            {
                var comment = await context.Comments.AsNoTracking().FirstOrDefaultAsync();

                var service = new CommentService(_mapper, context, _orderService);

                var resultPositive = await service.RemoveByIdAsync(comment.Id.ToString());
                var resultNegative = await service.RemoveByIdAsync(new Guid().ToString());

                resultPositive.IsSuccess.Should().BeTrue();
                resultPositive.Message.Should().BeNull();

                resultNegative.IsSuccess.Should().BeFalse();
                resultNegative.Message.Should().Contain(ExceptionConstants.COMMENT_WAS_NOT_FOUND);
            }
        }

        [Fact]
        public async void Comments_UpdateAsync_PositiveAndNegative_Test()
        {
            var options = new DbContextOptionsBuilder<DreamFoodDeliveryContext>()
                .UseInMemoryDatabase(databaseName: "Comments_UpdateAsync_PositiveAndNegative_Test")
                .Options;

            using (var context = new DreamFoodDeliveryContext(options))
            {
                context.AddRange(_comments);
                await context.SaveChangesAsync();
            }

            using (var context = new DreamFoodDeliveryContext(options))
            {
                var comment = await context.Comments.AsNoTracking().FirstOrDefaultAsync();

                var service = new CommentService(_mapper, context, _orderService);

                CommentToUpdate updateComment = new CommentToUpdate()
                {
                    Id = comment.Id.ToString(),
                    Headline = "Headline",
                    Content = "Content"
                };

                CommentToUpdate failComment = new CommentToUpdate()
                {
                    Id = new Guid().ToString(),
                    Headline = "Headline",
                    Content = "Content"
                };

                var resultPositive = await service.UpdateAsync(updateComment);
                var resultNegative = await service.UpdateAsync(failComment);

                resultPositive.IsSuccess.Should().BeTrue();
                resultPositive.Data.Headline.Should().BeEquivalentTo(updateComment.Headline);
                resultPositive.Data.Headline.Should().NotBeEquivalentTo(comment.Headline);

                resultNegative.IsSuccess.Should().BeFalse();
            }
        }
    }
}
