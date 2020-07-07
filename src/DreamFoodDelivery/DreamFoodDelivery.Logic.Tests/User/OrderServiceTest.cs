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
    public class OrderServiceTest
    {
        private readonly Faker<OrderDB> _fakeOrder = new Faker<OrderDB>()
            .RuleFor(x => x.Id, y => y.Random.Guid())
            .RuleFor(x => x.Address, y => y.Random.Words())
            .RuleFor(x => x.PhoneNumber, y => y.Person.Phone)
            .RuleFor(x => x.Name, y => y.Name.FirstName())
            .RuleFor(x => x.Surname, y => y.Name.LastName())
            .RuleFor(x => x.Status, y => y.Random.Words());
        private readonly Faker<BasketDishDB> _fakeBasketDishDB = new Faker<BasketDishDB>()
            .RuleFor(x => x.Id, y => y.Random.Guid())
            .RuleFor(x => x.BasketId, y => y.Random.Guid())
            .RuleFor(x => x.DishId, y => y.Random.Guid())
            .RuleFor(x => x.DishPrice, y => y.Random.Number(1000, 1500))
            .RuleFor(x => x.Sale, y => y.Random.Byte(0, 100))
            .RuleFor(x => x.Quantity, y => y.Random.Byte(0, 10));
        //orderID

        readonly List<OrderDB> _orders;
        readonly List<BasketDishDB> _basketDishes;
        readonly IMapper _mapper;
        readonly UserManager<AppUser> _userManager;
        readonly IEmailSenderService _emailSender;
        readonly DishView _dishView = new DishView()
        {
            Quantity = 10
        };

        public OrderServiceTest()
        {
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<OrderDB, OrderView>().ReverseMap();
            });

            _mapper = new Mapper(mapperConfiguration);
            _orders = _fakeOrder.Generate(2);
            _basketDishes = _fakeBasketDishDB.Generate(2);
        }

        [Fact]
        public async void Orders_GetAllAsync_Test()
        {
            var options = new DbContextOptionsBuilder<DreamFoodDeliveryContext>()
                .UseInMemoryDatabase(databaseName: "Orders_GetAllAsync_Test")
                .Options;

            // Run the test against one instance of the context
            using (var context = new DreamFoodDeliveryContext(options))
            {
                context.AddRange(_orders);
                _basketDishes[0].OrderId = _orders.FirstOrDefault().Id;
                _basketDishes[1].OrderId = _orders.LastOrDefault().Id;
                context.AddRange(_basketDishes);
                await context.SaveChangesAsync();
            }

            var storeDishService = new Mock<IDishService>();
            storeDishService.Setup(x => x.GetByIdAsync(It.IsAny<string>(), CancellationToken.None)) 
                .ReturnsAsync(Result<DishView>.Ok(_dishView));

            // Use a separate instance of the context to verify correct data was saved to database
            using (var context = new DreamFoodDeliveryContext(options))
            {
                var service = new OrderService(_mapper, _userManager, context, storeDishService.Object, _emailSender);
                var orderInBase = await context.Orders.AsNoTracking().ToListAsync();
                var result = await service.GetAllAsync();
                foreach (var item in orderInBase)
                {
                    var itemFromResult = result.Data.Where(_ => _.Name.Equals(item.Name, StringComparison.OrdinalIgnoreCase)).Select(_ => _).FirstOrDefault();
                    itemFromResult.Should().NotBeNull();
                }
            }
        }
    }
}
