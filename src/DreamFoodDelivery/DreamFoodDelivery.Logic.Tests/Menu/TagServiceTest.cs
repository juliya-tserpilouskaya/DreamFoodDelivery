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
    public class TagServiceTest
    {
        private readonly Faker<DishDB> _fakeDish = new Faker<DishDB>()
            .RuleFor(x => x.Id, y => y.Random.Guid())
            .RuleFor(x => x.Name, y => y.Random.Word())
            .RuleFor(x => x.Composition, y => y.Random.Words())
            .RuleFor(x => x.Weight, y => y.Random.Words())
            .RuleFor(x => x.Description, y => y.Random.Words())
            .RuleFor(x => x.Price, y => y.Random.Number(1000, 1500))
            .RuleFor(x => x.Sale, y => y.Random.Byte(0, 100));
        private readonly Faker<TagDB> _fakeTag = new Faker<TagDB>()
            .RuleFor(x => x.Id, y => y.Random.Guid())
            .RuleFor(x => x.TagName, y => y.Random.Word());

        readonly List<TagDB> _tags;
        readonly List<DishDB> _dishes;
        readonly IMapper _mapper;
        public TagServiceTest()
        {
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DishView, DishDB>().ReverseMap();
                cfg.CreateMap<TagView, TagDB>().ReverseMap();
                cfg.CreateMap<TagToAdd, TagDB>().ReverseMap();
                cfg.CreateMap<TagToUpdate, TagDB>().ReverseMap();
            });
            _mapper = new Mapper(mapperConfiguration);
            _dishes = _fakeDish.Generate(2);
            _tags = _fakeTag.Generate(2);
        }

        [Fact]
        public async void Tags_GetAllTagsAsync_Test()
        {
            var options = new DbContextOptionsBuilder<DreamFoodDeliveryContext>()
                .UseInMemoryDatabase(databaseName: "Tag_GetAllTagsAsync_Test")
                .Options;

            // Run the test against one instance of the context
            using (var context = new DreamFoodDeliveryContext(options))
            {
                context.AddRange(_dishes);
                context.AddRange(_tags);
                DishTagDB dishTag = new DishTagDB
                {
                    TagId = _tags.FirstOrDefault().Id,
                    DishId = _dishes.FirstOrDefault().Id,
                };
                context.Add(dishTag);
                await context.SaveChangesAsync();
            }

            // Use a separate instance of the context to verify correct data was saved to database
            using (var context = new DreamFoodDeliveryContext(options))
            {
                var service = new TagService(_mapper, context);

                var tagFromBase = await context.Tags.FirstOrDefaultAsync();

                var result = await service.GetAllTagsAsync();
                result.IsSuccess.Should().BeTrue();
                result.Data.Count().Should().Be(1);

                foreach (var item in result.Data)
                {
                    item.TagName.Should().Be(tagFromBase.TagName);
                }
            }
        }

        [Fact]
        public async void Tags_RemoveByIdAsync_PositiveAndNegative_Test()
        {
            var options = new DbContextOptionsBuilder<DreamFoodDeliveryContext>()
                .UseInMemoryDatabase(databaseName: "Tags_RemoveByIdAsync_PositiveAndNegative_Test")
                .Options;

            using (var context = new DreamFoodDeliveryContext(options))
            {
                context.AddRange(_tags);
                await context.SaveChangesAsync();
            }

            using (var context = new DreamFoodDeliveryContext(options))
            {
                var tag = await context.Tags.AsNoTracking().FirstOrDefaultAsync();

                var service = new TagService(_mapper, context);

                var resultPositive = await service.RemoveByIdAsync(tag.Id.ToString());
                var resultNegative = await service.RemoveByIdAsync(new Guid().ToString());

                resultPositive.IsSuccess.Should().BeTrue();
                resultPositive.Message.Should().BeNull();

                resultNegative.IsSuccess.Should().BeFalse();
                resultNegative.Message.Should().Contain(ExceptionConstants.TAG_WAS_NOT_FOUND);
            }
        }

        [Fact]
        public async void Tags_AddTagDBAsync_Positive_Test()
        {
            var options = new DbContextOptionsBuilder<DreamFoodDeliveryContext>()
                .UseInMemoryDatabase(databaseName: "Tags_AddTagDBAsync_Positive_Test")
                .Options;

            using (var context = new DreamFoodDeliveryContext(options))
            {
                var service = new TagService(_mapper, context);
                string newTag = "NewTag";

                TagToAdd tag = new TagToAdd()
                {
                    TagName = newTag,
                };

                var resultPositive = await service.AddTagDBAsync(tag);
                resultPositive.IsSuccess.Should().BeTrue();
                resultPositive.Data.TagName.Should().BeEquivalentTo(tag.TagName);
            }
        }

        [Fact]
        public async void Tags_UpdateAsync_PositiveAndNegative_Test()
        {
            var options = new DbContextOptionsBuilder<DreamFoodDeliveryContext>()
                .UseInMemoryDatabase(databaseName: "Tags_UpdateAsync_PositiveAndNegative_Test")
                .Options;

            using (var context = new DreamFoodDeliveryContext(options))
            {
                context.AddRange(_tags);
                await context.SaveChangesAsync();
            }

            using (var context = new DreamFoodDeliveryContext(options))
            {
                var tag = await context.Tags.AsNoTracking().FirstOrDefaultAsync();

                var service = new TagService(_mapper, context);
                string testName = "TestName";
                TagToUpdate updateTag = new TagToUpdate()
                {
                    Id = tag.Id.ToString(),
                    TagName = testName,
                };

                TagToUpdate failTag = new TagToUpdate()
                {
                    Id = new Guid().ToString(),
                    TagName = testName,
                };

                var resultPositive = await service.UpdateAsync(updateTag);
                var resultNegative = await service.UpdateAsync(failTag);

                resultPositive.IsSuccess.Should().BeTrue();
                resultPositive.Data.TagName.Should().BeEquivalentTo(updateTag.TagName);
                resultPositive.Data.TagName.Should().NotBeEquivalentTo(tag.TagName);

                resultNegative.IsSuccess.Should().BeFalse();
            }
        }

        [Fact]
        public async void Tags_AddAsync_Positive_Test()
        {
            var options = new DbContextOptionsBuilder<DreamFoodDeliveryContext>()
                .UseInMemoryDatabase(databaseName: "Tags_AddAsync_Positive_Test")
                .Options;

            using (var context = new DreamFoodDeliveryContext(options))
            {
                var service = new TagService(_mapper, context);
                string newTag = "NewTag";

                TagToAdd tag = new TagToAdd()
                {
                    TagName = newTag,
                };

                var resultPositive = await service.AddAsync(tag);
                resultPositive.IsSuccess.Should().BeTrue();
                resultPositive.Data.TagName.Should().BeEquivalentTo(tag.TagName);
            }
        }

        [Fact]
        public async void Tags_GetByIdAsync_PositiveAndNegative_Test()
        {
            var options = new DbContextOptionsBuilder<DreamFoodDeliveryContext>()
                .UseInMemoryDatabase(databaseName: "Tags_GetByIdAsync_PositiveAndNegative_Test")
                .Options;

            using (var context = new DreamFoodDeliveryContext(options))
            {
                context.AddRange(_tags);
                await context.SaveChangesAsync();
            }

            using (var context = new DreamFoodDeliveryContext(options))
            {
                var tag = await context.Tags.AsNoTracking().FirstOrDefaultAsync();

                var service = new TagService(_mapper, context);

                var resultPositive = await service.GetByIdAsync(tag.Id.ToString());
                var resultNegative = await service.GetByIdAsync(new Guid().ToString());

                resultPositive.IsSuccess.Should().BeTrue();
                resultPositive.Data.TagName.Should().BeEquivalentTo(tag.TagName);

                resultNegative.IsSuccess.Should().BeFalse();
                resultNegative.Data.Should().BeNull();
            }
        }

        [Fact]
        public async void Tags_RemoveAllAsync_Test()
        {
            var options = new DbContextOptionsBuilder<DreamFoodDeliveryContext>()
                .UseInMemoryDatabase(databaseName: "Tags_RemoveAllAsync_Test")
                .Options;

            using (var context = new DreamFoodDeliveryContext(options))
            {
                context.AddRange(_tags);
                await context.SaveChangesAsync();
            }

            using (var context = new DreamFoodDeliveryContext(options))
            {
                var service = new TagService(_mapper, context);

                var resultPositive = await service.RemoveAllAsync();

                resultPositive.IsSuccess.Should().BeTrue();
                resultPositive.Message.Should().BeNull();
            }
        }
    }
}
