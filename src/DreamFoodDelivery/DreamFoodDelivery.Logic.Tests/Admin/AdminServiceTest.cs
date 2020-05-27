//using AutoMapper;
//using Bogus;
//using DreamFoodDelivery.Common.Сonstants;
//using DreamFoodDelivery.Data.Context;
//using DreamFoodDelivery.Data.Models;
//using DreamFoodDelivery.Domain.DTO;
//using DreamFoodDelivery.Domain.Logic.InterfaceServices;
//using DreamFoodDelivery.Domain.Logic.Services;
//using FluentAssertions;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Xunit;

//namespace DreamFoodDelivery.Logic.Tests
//{
//    public class AdminServiceTest
//    {
//        readonly IMapper _mapper;
//        private UserManager<User> _userManager;
//        private IUserService _userService;
//        private IEmailSenderService _emailSender;

//        public AdminServiceTest()
//        {
//            var mapperConfiguration = new MapperConfiguration(cfg =>
//            {
//                cfg.CreateMap<UserDB, UserDTO>().ReverseMap();
//                cfg.CreateMap<UserProfile, User>().ReverseMap();
//                cfg.CreateMap<UserToUpdate, User>().ReverseMap();
//            });

//            _mapper = new Mapper(mapperConfiguration);
//        }

//        [Fact]
//        public async void Admin_GetAllAsync_Test()
//        {
//            var optionsDB = new DbContextOptionsBuilder<DreamFoodDeliveryContext>()
//                .UseInMemoryDatabase(databaseName: "Admin_GetAllAsync_Test_DB")
//                .Options;
//            var options = new DbContextOptionsBuilder<UserContext>()
//                .UseInMemoryDatabase(databaseName: "Admin_GetAllAsync_Test")
//                .Options;

//            // Run the test against one instance of the context
//            using (var context = new UserContext(options))
//            using (var contextDB = new DreamFoodDeliveryContext(optionsDB))
//            {
//                var user = new User
//                {
//                    Email = SuperAdminData.EMAIL,
//                    UserName = SuperAdminData.USER_NAME,
//                    PersonalDiscount = 0,
//                    Role = "Admin",
//                    IsEmailConfirmed = false
//                };
//                var userDB = new UserDB
//                {
//                    IdFromIdentity = user.Id,
//                };
//                context.Add(user);
//                await context.SaveChangesAsync();
//                contextDB.AddRange(userDB);
//                await contextDB.SaveChangesAsync();
//            }

//            // Use a separate instance of the context to verify correct data was saved to database
//            using (var context = new UserContext(options))
//            using (var contextDB = new DreamFoodDeliveryContext(optionsDB))
//            {
//                var service = new AdminService(_mapper, contextDB, _userManager, _userService, _emailSender);

//                var usersInBase = await context.Users.AsNoTracking().ToListAsync();
//                var usersInBaseDB = await contextDB.Users.AsNoTracking().ToListAsync();

//                var result = await service.GetAllAsync();

//                foreach (var user in usersInBaseDB)
//                {
//                    var itemFromResult = result.Data.Where(_ => _.UserDTO.Id == user.Id).Select(_ => _).FirstOrDefault();
//                    itemFromResult.Should().NotBeNull();
//                }
//            }
//        }
//    }
//}
