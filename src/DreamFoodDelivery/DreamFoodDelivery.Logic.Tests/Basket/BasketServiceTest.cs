using AutoMapper;
using Bogus;
using DreamFoodDelivery.Common;
using DreamFoodDelivery.Data.Context;
using DreamFoodDelivery.Data.Models;
using DreamFoodDelivery.Domain.Logic.InterfaceServices;
using DreamFoodDelivery.Domain.Logic.Services;
using DreamFoodDelivery.Domain.View;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Xunit;

namespace DreamFoodDelivery.Logic.Tests
{
    public class BasketServiceTest
    {
        private readonly Faker<DishDB> _fakeDish = new Faker<DishDB>()
            .RuleFor(x => x.Id, y => y.Random.Guid())
            .RuleFor(x => x.Name, y => y.Random.Word())
            .RuleFor(x => x.Composition, y => y.Random.Words())
            .RuleFor(x => x.Weight, y => y.Random.Words())
            .RuleFor(x => x.Description, y => y.Random.Words())
            .RuleFor(x => x.Price, y => y.Random.Number(1000, 1500))
            .RuleFor(x => x.Sale, y => y.Random.Byte(0, 100));
        private readonly Faker<AppUser> _fakeUserData = new Faker<AppUser>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString())
            .RuleFor(x => x.PersonalDiscount, y => 0);
        private readonly Faker<UserDB> _fakeUser = new Faker<UserDB>()
            .RuleFor(x => x.Id, y => y.Random.Guid())
            .RuleFor(x => x.BasketId, y => y.Random.Guid());

        readonly List<DishDB> _dishes;
        readonly AppUser _userData;
        readonly UserDB _user;
        readonly IMapper _mapper;
        public BasketServiceTest()
        {
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DishDB, DishView>();
                cfg.CreateMap<BasketDB, BasketView>();
            });
            _mapper = new Mapper(mapperConfiguration);
            _dishes = _fakeDish.Generate(2);
            _userData = _fakeUserData.Generate();
            _user = _fakeUser.Generate();
        }

        [Fact]
        public async void Basket_AddUpdateDishAsync_Positive_Test()
        {
            var options = new DbContextOptionsBuilder<DreamFoodDeliveryContext>()
                .UseInMemoryDatabase(databaseName: "Basket_AddUpdateDishAsync_Positive_Test")
                .Options;

            int count = 10;

            var storeManager = new Mock<IUserStore<AppUser>>();
            storeManager.Setup(x => x.FindByIdAsync(_userData.Id, CancellationToken.None))
                .ReturnsAsync(new AppUser()
                {
                    PersonalDiscount = _userData.PersonalDiscount,
                });
            var mgr = new UserManager<AppUser>(storeManager.Object, null, null, null, null, null, null, null, null);


            using (var context = new DreamFoodDeliveryContext(options))
            {
                _user.IdFromIdentity = _userData.Id;
                context.Add(_user);
                context.AddRange(_dishes);
                BasketDB basket = new BasketDB() { Id = _user.BasketId, UserId = _user.Id };
                context.Baskets.Add(basket);
                await context.SaveChangesAsync();
            }

            var storeDishService = new Mock<IDishService>();
            storeDishService.Setup(x => x.GetByIdAsync(_dishes.FirstOrDefault().Id.ToString(), CancellationToken.None)) 
                .ReturnsAsync(Result<DishView>.Ok(_mapper.Map<DishView>(_dishes.FirstOrDefault())));

            using (var context = new DreamFoodDeliveryContext(options))
            {
                var dish = _dishes.FirstOrDefault();
                double basketPrice_first = (dish.Price * (100 - dish.Sale) / 100 * count * (100 - _userData.PersonalDiscount) / 100).Value;
                basketPrice_first = Math.Round(basketPrice_first, 2);
                var service = new BasketService(_mapper, mgr, context, storeDishService.Object);
                var resultPositive = await service.AddUpdateDishAsync(dish.Id.ToString(), _userData.Id, count);
                resultPositive.IsSuccess.Should().BeTrue();
                resultPositive.Data.BasketCost.Should().Be(basketPrice_first);
            }
        }
    }
}
