using AutoMapper;
using Bogus;
using DreamFoodDelivery.Common;
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
        private readonly Faker<CommentDB> _fakeComment = new Faker<CommentDB>()
            .RuleFor(x => x.Headline, y => y.Random.Word())
            .RuleFor(x => x.Rating, y => y.Random.Byte(0, 5))
            .RuleFor(x => x.Content, y => y.Random.Words());
        readonly List<CommentDB> _comments;
        readonly IMapper _mapper;
        readonly IOrderService _orderService;

        public CommentServiceTest()
        {
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CommentDB, CommentView>().ReverseMap();
            });

            _comments = _fakeComment.Generate(10);
            _mapper = new Mapper(mapperConfiguration);
        }

        [Fact]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task", Justification = "<Pending>")]
        public async void GetAllUsersTestAsync()
        {
            var options = new DbContextOptionsBuilder<DreamFoodDeliveryContext>()
                .UseInMemoryDatabase(databaseName: "Comments_GetAllAsync_Test")
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
                var result = await service.GetAllAsync(new PageRequest());

                foreach (var item in _comments)
                {
                    var itemFromResult = result.Data.Data.Where(_ => _.Headline.Equals(item.Headline, StringComparison.OrdinalIgnoreCase)).Select(_ => _).FirstOrDefault();
                    itemFromResult.Should().NotBeNull();
                }
            }
        }
    }
}
