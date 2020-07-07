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
using MockQueryable.Moq;
using Moq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DreamFoodDelivery.Logic.Tests
{
    public class AdminServiceTest
    {
        readonly IMapper _mapper;
        readonly UserManager<AppUser> _manager;
        //private Mock<UserManager<User>> _userManager;
        private IUserService _userService;
        private IEmailSenderService _emailSender;
        private IEmailBuilder _emailBuilder;

        readonly UserProfile _userProfile;

        public AdminServiceTest()
        {
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UserDB, UserDTO>().ReverseMap();
                cfg.CreateMap<UserProfile, AppUser>().ReverseMap();
                cfg.CreateMap<UserToUpdate, AppUser>().ReverseMap();
            });

            _mapper = new Mapper(mapperConfiguration);
        }

        // TODO: clean
        [Fact]
        public async void Admin_GetAllAsync_Test()
        {
            var users = new List<AppUser>();

            users.Add(new AppUser()
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

            users.Add(new AppUser()
            {
                Id = "22222222-222e-4276-82bf-7d0e0d12f1e9",
                UserName = "test2@mail.ru",
                Email = "test2@mail.ru",
                EmailConfirmed = true,
                PhoneNumber = "123456789012",
                Address = "My Adr2",
                PersonalDiscount = 0,
                Name = "NAAAAA",
                Surname = "AAAAAAAAAAA",
                Role = "User",
                IsEmailConfirmed = true
            });

            var mock = users.AsQueryable().BuildMock();

            var mockUserManager = new Mock<UserManager<AppUser>>();
            var mockUserStore = new Mock<IUserStore<AppUser>>();
            var mockUserRoleStore = mockUserStore.As<IQueryableUserStore<AppUser>>();
            mockUserManager.Setup(x => x.Users).Returns(mock.Object);
            mockUserRoleStore.Setup(x => x.Users).Returns(mock.Object);
            var manager = new UserManager<AppUser>(mockUserStore.Object, null, null, null, null, null, null, null, null);

            //UserManager<UserData> manager = mockUserManager.Object;

            //Mock<EntityFrameworkQueryableExtensions> mockToList = new Mock<EntityFrameworkQueryableExtensions>();
            //mockToList.Setup(x => x.To).Returns(GetTestUsers());

            ////mockUser.Setup(userManager => userManager.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new IdentityUser { ... });
            //mockUser.Setup(p => p.Users).Returns(GetTestUsers());
            ////mockUser.Setup(userManager => userManager.IsInRoleAsync(It.IsAny<User>(), "SweetTooth")).ReturnsAsync(true);
            //UserManager<User> manager = mockUser.Object;

            //var userManagerMocked = new Mock<UserManager<User>>();
            //userManagerMocked.Setup(p => p.Users.ToListAsync(CancellationToken.None)).ReturnsAsync(GetTestUsers());
            ////userManagerMocked.Setup(p => p.Users.ToList()).Returns(GetTestUsers());
            //UserManager<User> manager = userManagerMocked.Object;

            //var store = new Mock<UserManager<User>>();
            //store.Setup(x => x.Users.ToListAsync(CancellationToken.None)).ReturnsAsync(GetTestUsers());
            //UserManager<User> manager = store.Object;

            //var MockUserManager = Microsoft.AspNet.Identity.Test.MockHelpers.MockUserManager<User>();

            //var mockUserStore = new Mock<IUserStore<UserData>>();
            //var mockUserRoleStore = mockUserStore.As<IQueryableUserStore<UserData>>();
            ////mockUserRoleStore.Setup(x => x.Users).Returns(mock.Object);

            //var manager = new UserManager<UserData>(mockUserStore.Object, null, null, null, null, null, null, null, null);



            //mockUserRoleStore.Setup(x => x.Users.ToListAsync(CancellationToken.None)).ReturnsAsync(GetTestUsers());

            //var userManager = new UserManager<User>(mockUserStore.Object);

            //store.Setup(x => x.IsInRoleAsync(User, "Admin")).ReturnsAsync(GetTestUsers());

            


            var optionsDB = new DbContextOptionsBuilder<DreamFoodDeliveryContext>()
                .UseInMemoryDatabase(databaseName: "Admin_GetAllAsync_Test")
                .Options;
            //var options = new DbContextOptionsBuilder<UserContext>()
            //    .UseInMemoryDatabase(databaseName: "Admin_GetAllAsync_Test")
            //    .Options;

            // Run the test against one instance of the context
            //using (var context = new UserContext(options))
            using (var contextDB = new DreamFoodDeliveryContext(optionsDB))
            {
                //var user = new User
                //{
                //    Email = SuperAdminData.EMAIL,
                //    UserName = SuperAdminData.USER_NAME,
                //    PersonalDiscount = 0,
                //    Role = "Admin",
                //    IsEmailConfirmed = false
                //};
                var userDB = new UserDB
                {
                    IdFromIdentity = "11111111-222e-4276-82bf-7d0e0d12f1e9",
                };
                var userDB2 = new UserDB
                {
                    IdFromIdentity = "22222222-222e-4276-82bf-7d0e0d12f1e9",
                };
                //context.Add(user);
                //await context.SaveChangesAsync();
                contextDB.Add(userDB);
                contextDB.Add(userDB2);
                await contextDB.SaveChangesAsync();
            }

            var store = new Mock<IUserService>();
            store.Setup(x => x.GetUserProfileByIdFromIdentityAsync(It.IsAny<string>())) //"11111111-222e-4276-82bf-7d0e0d12f1e9"
                .ReturnsAsync(Result<UserProfile>.Ok(_userProfile));
            //store.Setup(x => x.GetUserProfileByIdFromIdentityAsync("22222222-222e-4276-82bf-7d0e0d12f1e9")) // It.IsAny<string>()  - give exception
            //    .ReturnsAsync(Result<UserProfile>.Ok(_mapper.Map<UserProfile>(_userProfile)));

            // Use a separate instance of the context to verify correct data was saved to database
            //using (var context = new UserContext(options))
            using (var contextDB = new DreamFoodDeliveryContext(optionsDB))
            {
                //var userService = new UserService(_mapper, contextDB, manager, _emailSender, _emailBuilder);
                var service = new AdminService(_mapper, contextDB, manager, store.Object, _emailSender);

                //var usersInBase = await context.Users.AsNoTracking().ToListAsync();
                //var usersInBase = GetTestUsers();
                var usersInBaseDB = await contextDB.Users.AsNoTracking().ToListAsync();

                var result = await service.GetAllAsync();

                foreach (var user in usersInBaseDB)
                {
                    var itemFromResult = result.Data.Where(_ => _.UserDTO.Id == user.Id).Select(_ => _).FirstOrDefault();
                    itemFromResult.Should().NotBeNull();
                }
            }
        }

        //[Fact]
        //public async void Admin_GetAllAsync_Test_Second()
        //{
        //    //var mockUserStore = new Mock<IUserStore<UserData>>();
        //    //var mockUserRoleStore = mockUserStore.As<Task<IQueryableUserStore<UserData>>>();
        //    var mockUserStore = new Mock<UserManager<UserData>>();
        //    var mockUserRoleStore = mockUserStore.As<Task<IQueryableUserStore<UserData>>>();
        //    mockUserStore.Setup(x => x.Users).Returns(GetTestUsers());

        //    //var manager = new UserManager<UserData>(mockUserStore.Object, null, null, null, null, null, null, null, null);

        //    var optionsDB = new DbContextOptionsBuilder<DreamFoodDeliveryContext>()
        //        .UseInMemoryDatabase(databaseName: "Admin_GetAllAsync_Test_Second")
        //        .Options;
        //    using (var contextDB = new DreamFoodDeliveryContext(optionsDB))
        //    {
        //        var userDB = new UserDB
        //        {
        //            IdFromIdentity = "11111111 - 222e-4276 - 82bf - 7d0e0d12f1e9",
        //        };
        //        var userDB2 = new UserDB
        //        {
        //            IdFromIdentity = "22222222 - 222e-4276 - 82bf - 7d0e0d12f1e9",
        //        };
        //        contextDB.Add(userDB);
        //        contextDB.Add(userDB2);
        //        await contextDB.SaveChangesAsync();
        //    }

        //    using (var contextDB = new DreamFoodDeliveryContext(optionsDB))
        //    {
        //        var service = new AdminService(_mapper, contextDB, _manager, _userService, _emailSender);
        //        var usersInBaseDB = await contextDB.Users.AsNoTracking().ToListAsync();
        //        var result = await service.GetAllAsync();
        //        foreach (var user in usersInBaseDB)
        //        {
        //            var itemFromResult = result.Data.Where(_ => _.UserDTO.Id == user.Id).Select(_ => _).FirstOrDefault();
        //            itemFromResult.Should().NotBeNull();
        //        }
        //    }
        //}

        private IAsyncEnumerable<AppUser> GetTestUsers()
        {
            var users = new List<AppUser>();

            users.Add(new AppUser()
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

            users.Add(new AppUser()
            {
                Id = "22222222-222e-4276-82bf-7d0e0d12f1e9",
                UserName = "test2@mail.ru",
                Email = "test2@mail.ru",
                EmailConfirmed = true,
                PhoneNumber = "123456789012",
                Address = "My Adr2",
                PersonalDiscount = 0,
                Name = "NAAAAA",
                Surname = "AAAAAAAAAAA",
                Role = "User",
                IsEmailConfirmed = true
            });

            return users as IAsyncEnumerable<AppUser>;
        }
    }
}
