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
            #region User
            CreateMap<UserDB, UserDTO>().ReverseMap();
            CreateMap<UserGeneration, UserDB>().ReverseMap();
            CreateMap<UserProfile, User>().ReverseMap();
            CreateMap<UserUpdate, User>().ReverseMap();
            #endregion

            #region Comment
            CreateMap<CommentDB, CommentView>().ReverseMap();
            CreateMap<CommentDB, CommentToAdd>().ReverseMap();
            CreateMap<CommentDB, CommentToUpdate>().ReverseMap();
            #endregion

            #region Order
            CreateMap<OrderDB, OrderView>().ReverseMap();
            CreateMap<OrderDB, OrderToAdd>().ReverseMap();
            CreateMap<OrderDB, OrderToUpdate>().ReverseMap();
            CreateMap<OrderToStatusUpdate, OrderDB>().ReverseMap();
            #endregion

            #region Basket
            CreateMap<BasketDB, BasketView>().ReverseMap();
            #endregion

            #region Dish
            CreateMap<DishDTO, DishDB>().ReverseMap();
            CreateMap<DishToAdd, DishDB>().ReverseMap();
            CreateMap<DishView, DishDB>().ReverseMap();
            CreateMap<DishToUpdate, DishDB>().ReverseMap();
            #endregion

            #region DishTag
            CreateMap<DishTagView, DishTagDB>().ReverseMap();
            #endregion

            #region Tag
            CreateMap<TagView, TagDB>().ReverseMap();
            CreateMap<TagToAdd, TagDB>().ReverseMap();
            CreateMap<TagToUpdate, TagDB>().ReverseMap();
            #endregion
        }
    }
}
