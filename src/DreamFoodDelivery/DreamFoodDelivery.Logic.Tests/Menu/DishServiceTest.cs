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
using System.Text;
using Xunit;

namespace DreamFoodDelivery.Logic.Tests
{
    public class DishServiceTest
    {
        private readonly Faker<DishDB> _fakeDish = new Faker<DishDB>()
            .RuleFor(x => x.Id, y => y.Random.Guid())
            .RuleFor(x => x.Name, y => y.Random.Word())
            .RuleFor(x => x.Composition, y => y.Random.Words())
            .RuleFor(x => x.Weight, y => y.Random.Words())
            .RuleFor(x => x.Description, y => y.Random.Words())
            .RuleFor(x => x.Price, y => y.Random.Number(1000, 1500))
            .RuleFor(x => x.Sale, y => y.Random.Byte(0, 100));

        readonly List<DishDB> _dishes;
        readonly IMapper _mapper;
        readonly ITagService _tagService;

        public DishServiceTest()
        {
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DishView, DishDB>().ReverseMap();
                cfg.CreateMap<DishToAdd, DishDB>().ReverseMap();
                cfg.CreateMap<DishToUpdate, DishDB>().ReverseMap();
                cfg.CreateMap<TagToAdd, TagDB>().ReverseMap();
            });
            _mapper = new Mapper(mapperConfiguration);
            _dishes = _fakeDish.Generate(2);
        }

        [Fact]
        public async void Dihes_AddAsync_Positive_Test()
        {
            var options = new DbContextOptionsBuilder<DreamFoodDeliveryContext>()
                .UseInMemoryDatabase(databaseName: "Dihes_AddAsync_Positive_Test")
                .Options;

            using (var context = new DreamFoodDeliveryContext(options))
            {
                var tagService = new TagService(_mapper, context);
                var service = new DishService(_mapper, context, tagService);

                TagToAdd[] tags = new TagToAdd[] {
                    new TagToAdd
                    {
                        TagName = "New"
                    }
                };
                DishToAdd dish = new DishToAdd()
                {
                    Name = "Name",
                    Composition = "Composition",
                    Description = "Description",
                    Weight = "Weight",
                    TagNames = new HashSet<TagToAdd>(tags)

                };

                var resultPositive = await service.AddAsync(dish);
                resultPositive.IsSuccess.Should().BeTrue();
                resultPositive.Data.Name.Should().BeEquivalentTo(dish.Name);
                resultPositive.Data.Composition.Should().BeEquivalentTo(dish.Composition);
                resultPositive.Data.Description.Should().BeEquivalentTo(dish.Description);
                resultPositive.Data.Weight.Should().BeEquivalentTo(dish.Weight);
            }
        }

        [Fact]
        public async void Dishes_GetByIdAsync_PositiveAndNegative_Test()
        {
            var options = new DbContextOptionsBuilder<DreamFoodDeliveryContext>()
                .UseInMemoryDatabase(databaseName: "Dishes_GetByIdAsync_PositiveAndNegative_Test")
                .Options;

            using (var context = new DreamFoodDeliveryContext(options))
            {
                context.AddRange(_dishes);
                await context.SaveChangesAsync();
            }

            using (var context = new DreamFoodDeliveryContext(options))
            {
                var dish = await context.Dishes.AsNoTracking().FirstOrDefaultAsync();

                var service = new DishService(_mapper, context, _tagService);

                var resultPositive = await service.GetByIdAsync(dish.Id.ToString());
                var resultNegative = await service.GetByIdAsync(new Guid().ToString());

                resultPositive.IsSuccess.Should().BeTrue();
                resultPositive.Data.Name.Should().BeEquivalentTo(dish.Name);
                resultPositive.Data.Composition.Should().BeEquivalentTo(dish.Composition);
                resultPositive.Data.Description.Should().BeEquivalentTo(dish.Description);
                resultPositive.Data.Weight.Should().BeEquivalentTo(dish.Weight); 

                resultNegative.IsSuccess.Should().BeFalse();
                resultNegative.Data.Should().BeNull();
            }
        }

        [Fact]
        public async void Dishes_UpdateAsync_PositiveAndNegative_Test()
        {
            var options = new DbContextOptionsBuilder<DreamFoodDeliveryContext>()
                .UseInMemoryDatabase(databaseName: "Dishes_UpdateAsync_PositiveAndNegative_Test")
                .Options;

            using (var context = new DreamFoodDeliveryContext(options))
            {
                context.AddRange(_dishes);
                await context.SaveChangesAsync();
            }

            using (var context = new DreamFoodDeliveryContext(options))
            {
                var dish = await context.Dishes.AsNoTracking().FirstOrDefaultAsync();

                var tagService = new TagService(_mapper, context);
                var service = new DishService(_mapper, context, tagService);

                TagToAdd[] tags = new TagToAdd[] {
                    new TagToAdd
                    {
                        TagName = "New"
                    }
                };

                DishToUpdate updateDish = new DishToUpdate()
                {
                    Id = dish.Id,
                    Name = "Name",
                    Composition = "Composition",
                    Description = "Description",
                    Weight = "Weight",
                    TagNames = new HashSet<TagToAdd>(tags)
                };

                DishToUpdate failDish = new DishToUpdate()
                {
                    Id = Guid.NewGuid(),
                    Name = "Name",
                    Composition = "Composition",
                    Description = "Description",
                    Weight = "Weight",
                    TagNames = new HashSet<TagToAdd>(tags)
                };

                var resultPositive = await service.UpdateAsync(updateDish);
                var resultNegative = await service.UpdateAsync(failDish);

                resultPositive.IsSuccess.Should().BeTrue();
                resultPositive.Data.Name.Should().BeEquivalentTo(updateDish.Name);
                resultPositive.Data.Composition.Should().BeEquivalentTo(updateDish.Composition);
                resultPositive.Data.Description.Should().BeEquivalentTo(updateDish.Description);
                resultPositive.Data.Weight.Should().BeEquivalentTo(updateDish.Weight);
                resultPositive.Data.Name.Should().NotBeEquivalentTo(dish.Name);
                resultPositive.Data.Composition.Should().NotBeEquivalentTo(dish.Composition);
                resultPositive.Data.Description.Should().NotBeEquivalentTo(dish.Description);
                resultPositive.Data.Weight.Should().NotBeEquivalentTo(dish.Weight);

                resultNegative.IsSuccess.Should().BeFalse();
            }
        }

        [Fact]
        public async void Dishes_RemoveByIdAsync_PositiveAndNegative_Test()
        {
            var options = new DbContextOptionsBuilder<DreamFoodDeliveryContext>()
                .UseInMemoryDatabase(databaseName: "Dishes_RemoveByIdAsync_PositiveAndNegative_Test")
                .Options;

            using (var context = new DreamFoodDeliveryContext(options))
            {
                context.AddRange(_dishes);
                await context.SaveChangesAsync();
            }

            using (var context = new DreamFoodDeliveryContext(options))
            {
                var dish = await context.Dishes.AsNoTracking().FirstOrDefaultAsync();

                var service = new DishService(_mapper, context, _tagService);

                var resultPositive = await service.RemoveByIdAsync(dish.Id.ToString());
                var resultNegative = await service.RemoveByIdAsync(new Guid().ToString());

                resultPositive.IsSuccess.Should().BeTrue();
                resultPositive.Message.Should().BeNull();

                resultNegative.IsSuccess.Should().BeFalse();
                resultNegative.Message.Should().Contain(ExceptionConstants.DISH_WAS_NOT_FOUND);
            }
        }

        [Fact]
        public async void Dishes_RemoveAllAsync_Test()
        {
            var options = new DbContextOptionsBuilder<DreamFoodDeliveryContext>()
                .UseInMemoryDatabase(databaseName: "Dishes_RemoveAllAsync_Test")
                .Options;

            using (var context = new DreamFoodDeliveryContext(options))
            {
                context.AddRange(_dishes);
                await context.SaveChangesAsync();
            }

            using (var context = new DreamFoodDeliveryContext(options))
            {
                var service = new DishService(_mapper, context, _tagService);

                var resultPositive = await service.RemoveAllAsync();

                resultPositive.IsSuccess.Should().BeTrue();
                resultPositive.Message.Should().BeNull();
            }
        }
    }
}
