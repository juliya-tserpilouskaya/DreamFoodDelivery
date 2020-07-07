using AutoMapper;
using Bogus;
using DreamFoodDelivery.Common;
using DreamFoodDelivery.Data.Context;
using DreamFoodDelivery.Data.Models;
using DreamFoodDelivery.Domain.DTO;
using DreamFoodDelivery.Domain.Logic.InterfaceServices;
using DreamFoodDelivery.Domain.Logic.Services;
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
    public class UserServicetest
    {
        private Faker<UserDB> _fakeUser = new Faker<UserDB>()
            .RuleFor(x => x.IdFromIdentity, y => y.Random.Guid().ToString());

        readonly List<UserDB> _users;
        readonly IMapper _mapper;
        readonly IEmailSenderService _emailSender;
        readonly IEmailBuilder _emailBuilder;

        public UserServicetest()
        {
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UserDB, UserDTO>().ReverseMap();
                cfg.CreateMap<UserProfile, AppUser>().ReverseMap();
                cfg.CreateMap<UserGeneration, UserDB>();
            });

            _mapper = new Mapper(mapperConfiguration);
            _users = _fakeUser.Generate(2);
        }


        [Fact]
        public async void User_GetUserProfileByIdFromIdentityAsync_Positive_Test()
        {
            var store = new Mock<IUserStore<AppUser>>();
            store.Setup(x => x.FindByIdAsync("11111111-222e-4276-82bf-7d0e0d12f1e9", CancellationToken.None))
                .ReturnsAsync(new AppUser()
                {
                    Id = "11111111-222e-4276-82bf-7d0e0d12f1e9",
                    UserName = "test@mail.ru",
                    Email = "test@mail.ru",
                    EmailConfirmed = true,
                    PhoneNumber = "123456789012",
                    Address = "My Adr",
                    PersonalDiscount = 0,
                    Name = "Yana",
                    Surname = "Hersurname",
                    Role = "Admin",
                    IsEmailConfirmed = true
                });

            var mgr = new UserManager<AppUser>(store.Object, null, null, null, null, null, null, null, null);
            var optionsDB = new DbContextOptionsBuilder<DreamFoodDeliveryContext>()
                .UseInMemoryDatabase(databaseName: "User_GetUserProfileByIdFromIdentityAsync_Positive_Test")
                .Options;
            using (var contextDB = new DreamFoodDeliveryContext(optionsDB))
            {
                var userDB = new UserDB
                {
                    IdFromIdentity = "11111111 - 222e-4276 - 82bf - 7d0e0d12f1e9",
                };
                var userDB2 = new UserDB
                {
                    IdFromIdentity = "22222222 - 222e-4276 - 82bf - 7d0e0d12f1e9",
                };
                contextDB.Add(userDB);
                contextDB.Add(userDB2);
                await contextDB.SaveChangesAsync();
            }
            using (var contextDB = new DreamFoodDeliveryContext(optionsDB))
            {
                var service = new UserService(_mapper, contextDB, mgr, _emailSender, _emailBuilder);

                var result2 = await service.GetUserProfileByIdFromIdentityAsync("11111111-222e-4276-82bf-7d0e0d12f1e9");

                result2.Data.Should().NotBeNull();
            }
        }

        [Fact]
        public async void User_CreateAccountAsyncById_Positive_Test()
        {
            var options = new DbContextOptionsBuilder<DreamFoodDeliveryContext>()
                .UseInMemoryDatabase(databaseName: "User_CreateAccountAsyncById_Positive_Test")
                .Options;

            var store = new Mock<IUserStore<AppUser>>();
            store.Setup(x => x.FindByIdAsync(It.IsAny<string>(), CancellationToken.None))
                .ReturnsAsync(new AppUser()
                {
                    UserName = "test@mail.ru",
                    Email = "test@mail.ru",
                    EmailConfirmed = true,
                    PhoneNumber = "123456789012",
                    Address = "My Adr",
                    PersonalDiscount = 0,
                    Name = "Yana",
                    Surname = "Hersurname",
                    Role = "Admin",
                    IsEmailConfirmed = true
                });

            var userManager = new UserManager<AppUser>(store.Object, null, null, null, null, null, null, null, null);

            var user = _users.FirstOrDefault();

            //var store = new Mock<UserService>();
            //store.Setup(x => x.GetUserProfileByIdFromIdentityAsync(user.IdFromIdentity))
            //    .ReturnsAsync(Result<UserProfile>.Ok(_mapper.Map<UserProfile>(_userProfile)));

            using (var context = new DreamFoodDeliveryContext(options))
            {
                var service = new UserService(_mapper, context, userManager, _emailSender, _emailBuilder);

                var resultPositive = await service.CreateAccountAsyncById(user.IdFromIdentity);
                resultPositive.IsSuccess.Should().BeTrue();
                resultPositive.Data.UserDTO.IdFromIdentity.Should().BeEquivalentTo(user.IdFromIdentity);
                resultPositive.Data.UserDTO.Id.Should().NotBeEmpty();
                resultPositive.Data.UserDTO.BasketId.Should().NotBeEmpty();
            }
        }
    }
}
