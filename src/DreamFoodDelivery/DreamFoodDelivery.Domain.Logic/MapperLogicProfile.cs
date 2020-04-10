using AutoMapper;
using DreamFoodDelivery.Data.Models;
using DreamFoodDelivery.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Domain.Logic
{
    class MapperLogicProfile : Profile
    {
        public MapperLogicProfile()
        {
            // rethink
            CreateMap<CommentDB, CommentView>().ReverseMap();
            CreateMap<CommentDB, CommentToAdd>().ReverseMap();
            CreateMap<CommentDB, CommentToUpdate>().ReverseMap();
            CreateMap<OrderDB, OrderView>().ReverseMap();
            CreateMap<OrderDB, OrderToAdd>().ReverseMap();
            CreateMap<OrderDB, OrderToUpdate>().ReverseMap();
            CreateMap<TagDB, TagDTO>().ReverseMap();
            CreateMap<DishDB, DishDTO>().ReverseMap();
            CreateMap<BasketDB, BasketDTO>().ReverseMap();
            CreateMap<UserDB, UserDTO>().ReverseMap();
            CreateMap<UserInfoDB, UserInfoDTO>().ReverseMap();
            CreateMap<UserGeneration, UserDB>().ReverseMap();
            
        }
    }
}
