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
            CreateMap<CommentDB, CommentDTO_View>().ReverseMap();
            CreateMap<CommentDB, CommentDTO_Add>().ReverseMap();
            CreateMap<CommentDB, CommentDTO_Update>().ReverseMap();
        }
    }
}
