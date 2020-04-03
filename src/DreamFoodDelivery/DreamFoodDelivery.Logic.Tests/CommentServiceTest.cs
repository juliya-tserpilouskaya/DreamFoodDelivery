using AutoMapper;
using Bogus;
using DreamFoodDelivery.Data.Context;
using DreamFoodDelivery.Data.Models;
using DreamFoodDelivery.Data.Services;
using DreamFoodDelivery.Data.Services.Interfaces;
using DreamFoodDelivery.Domain.DTO;
using DreamFoodDelivery.Domain.Logic.Services;
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
        private Faker<CommentDB> _fakeComment = new Faker<CommentDB>()
            .RuleFor(x => x.Headline, y => y.Random.Word())
            .RuleFor(x => x.Rating, y => y.Random.Byte(0, 5))
            .RuleFor(x => x.Content, y => y.Random.Words());

        List<CommentDB> _comments;

        public CommentServiceTest()
        {
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CommentDB, CommentDTO_View>().ReverseMap();
            });

            _comments = _fakeComment.Generate(10);
        }

        [Fact]
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
                var service = new CommentDbService(context);
                var result = await service.GetAllAsync();

                foreach (var item in _comments) //Assert.Equal(expected.Name, actual.Name);
                {
                    var itemFromResult = result.Where(_ => _.Headline.Equals(item.Headline)).Select(_ => _).FirstOrDefault();
                    itemFromResult.Should().NotBeNull();
                }                
            }
        }
    }
}
