using AutoMapper;
using Bogus;
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
    public class MenuServiceTest
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
        public MenuServiceTest()
        {
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DishView, DishDB>().ReverseMap();
                cfg.CreateMap<TagView, TagDB>().ReverseMap();
                cfg.CreateMap<TagToAdd, TagDB>().ReverseMap();
            });
            _mapper = new Mapper(mapperConfiguration);
            _dishes = _fakeDish.Generate(2);
        }

        [Fact]
        public async void Menu_GetAllAsync_Test()
        {
            var options = new DbContextOptionsBuilder<DreamFoodDeliveryContext>()
                .UseInMemoryDatabase(databaseName: "Menu_GetAllAsync_Test")
                .Options;

            // Run the test against one instance of the context
            using (var context = new DreamFoodDeliveryContext(options))
            {
                context.AddRange(_dishes);
                await context.SaveChangesAsync();
            }

            // Use a separate instance of the context to verify correct data was saved to database
            using (var context = new DreamFoodDeliveryContext(options))
            {
                var service = new MenuService(_mapper, context);

                var dishesInBase = await context.Dishes.AsNoTracking().ToListAsync();

                var result = await service.GetAllAsync();

                foreach (var item in dishesInBase)
                {
                    var itemFromResult = result.Data.Where(_ => _.Name.Equals(item.Name, StringComparison.OrdinalIgnoreCase)).Select(_ => _).FirstOrDefault();
                    itemFromResult.Should().NotBeNull();
                }
            }
        }

        [Fact]
        public async void Menu_GetAllDishesByRequestAsync_PositiveAndNegative_Test()
        {
            var options = new DbContextOptionsBuilder<DreamFoodDeliveryContext>()
                .UseInMemoryDatabase(databaseName: "Menu_GetAllDishesByRequestAsync_PositiveAndNegative_Test")
                .Options;

            using (var context = new DreamFoodDeliveryContext(options))
            {
                _dishes[0].Sale = 10;
                _dishes[1].Sale = 10;

                context.AddRange(_dishes);
                await context.SaveChangesAsync();
            }

            using (var context = new DreamFoodDeliveryContext(options))
            {
                var service = new MenuService(_mapper, context);
                RequestParameters positiveParameters = new RequestParameters()
                {
                    Request = "",
                    TagsNames = Enumerable.Empty<string>(),
                    OnSale = true,
                    LowerPrice = 0,
                    UpperPrice = 1500
                };
                RequestParameters negativeParameters = new RequestParameters()
                {
                    Request = "testLine",
                    TagsNames = Enumerable.Empty<string>(),
                    OnSale = false,
                    LowerPrice = 0,
                    UpperPrice = 1500
                };
                var resultPositive = await service.GetAllDishesByRequestAsync(positiveParameters);
                var resultNegative = await service.GetAllDishesByRequestAsync(negativeParameters);
                resultPositive.IsSuccess.Should().BeTrue();
                resultNegative.Data.Should().BeEmpty();
            }
        }

        [Fact]
        public async void Menu_GetByNameAsync_PositiveAndNegative_Test()
        {
            var options = new DbContextOptionsBuilder<DreamFoodDeliveryContext>()
                .UseInMemoryDatabase(databaseName: "Menu_GetByNameAsync_PositiveAndNegative_Test")
                .Options;
            string testString = "NameTest";

            using (var context = new DreamFoodDeliveryContext(options))
            {
                _dishes[0].Name = testString;
                _dishes[1].Name = testString;

                context.AddRange(_dishes);
                await context.SaveChangesAsync();
            }

            using (var context = new DreamFoodDeliveryContext(options))
            {
                var service = new MenuService(_mapper, context);

                var resultPositive = await service.GetByNameAsync(testString);
                var resultNegative = await service.GetByNameAsync(testString+ testString);

                resultPositive.IsSuccess.Should().BeTrue();
                foreach (var item in resultPositive.Data)
                {
                    item.Name.Should().BeEquivalentTo(testString);
                }

                resultNegative.IsSuccess.Should().BeFalse();
                resultNegative.Data.Should().BeNull();
            }
        }

        [Fact]
        public async void Menu_GetByPriceAsync_PositiveAndNegative_Test()
        {
            var options = new DbContextOptionsBuilder<DreamFoodDeliveryContext>()
                .UseInMemoryDatabase(databaseName: "Menu_GetByPriceAsync_PositiveAndNegative_Test")
                .Options;
            double testValue = 250;
            using (var context = new DreamFoodDeliveryContext(options))
            {
                _dishes[0].Price = testValue;
                _dishes[1].Price = testValue;
                context.AddRange(_dishes);
                await context.SaveChangesAsync();
            }

            using (var context = new DreamFoodDeliveryContext(options))
            {
                var service = new MenuService(_mapper, context);

                var resultPositive = await service.GetByPriceAsync(testValue-10, testValue+10);
                var resultNegative = await service.GetByPriceAsync(testValue-20, testValue-10);

                resultPositive.IsSuccess.Should().BeTrue();
                foreach (var item in resultPositive.Data)
                {
                    item.Price.Should().Be(testValue);
                }

                resultNegative.IsSuccess.Should().BeFalse();
                resultNegative.Data.Should().BeNull();
            }
        }

        [Fact]
        public async void Menu_GetSalesAsync_Test()
        {
            var options = new DbContextOptionsBuilder<DreamFoodDeliveryContext>()
                .UseInMemoryDatabase(databaseName: "Menu_GetSalesAsync_Test")
                .Options;
            int testValue = 10;
            using (var context = new DreamFoodDeliveryContext(options))
            {
                _dishes[0].Sale = 0;
                _dishes[1].Sale = testValue;
                context.AddRange(_dishes);
                await context.SaveChangesAsync();
            }

            using (var context = new DreamFoodDeliveryContext(options))
            {
                var service = new MenuService(_mapper, context);

                var resultPositive = await service.GetSalesAsync();

                resultPositive.IsSuccess.Should().BeTrue();
                resultPositive.Data.Count().Should().Be(1);
                foreach (var item in resultPositive.Data)
                {
                    item.Sale.Should().Be(testValue);
                }              
            }
        }

        [Fact]
        public async void Menu_GetByTagIndexAsync_PositiveAndNegative_Test()
        {
            var options = new DbContextOptionsBuilder<DreamFoodDeliveryContext>()
                .UseInMemoryDatabase(databaseName: "Menu_GetByTagIndexAsync_PositiveAndNegative_Test")
                .Options;
            string tagTestName = "new";
            string tagTestSecondName = "old";
            using (var context = new DreamFoodDeliveryContext(options))
            {
                TagDB[] tags = new TagDB[]
                {
                    new TagDB
                    {
                    TagName = tagTestName
                    },
                    new TagDB
                    {
                    TagName = tagTestSecondName
                    },
                };
                DishDB dish = new DishDB
                {
                    Name = "dish"
                };

                context.AddRange(tags);
                context.Add(dish);
                DishTagDB dishTag = new DishTagDB
                {
                    TagId = tags.FirstOrDefault(x => x.TagName == tagTestName).Id,
                    DishId = dish.Id
                };
                context.Add(dishTag);

                await context.SaveChangesAsync();
            }

            using (var context = new DreamFoodDeliveryContext(options))
            {
                var service = new MenuService(_mapper, context);

                var resultPositive = await service.GetByTagIndexAsync(tagTestName);
                var resultNegative = await service.GetByTagIndexAsync(tagTestSecondName);
                resultPositive.IsSuccess.Should().BeTrue();
                resultNegative.IsSuccess.Should().BeFalse();
            }
        }
    }
}
