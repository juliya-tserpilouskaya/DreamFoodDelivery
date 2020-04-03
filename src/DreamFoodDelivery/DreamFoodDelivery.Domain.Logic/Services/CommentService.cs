using AutoMapper;
using DreamFoodDelivery.Common.Helpers;
using DreamFoodDelivery.Data.Context;
using DreamFoodDelivery.Data.Models;
using DreamFoodDelivery.Data.Services.Interfaces;
using DreamFoodDelivery.Domain.DTO;
using DreamFoodDelivery.Domain.Logic.InterfaceServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DreamFoodDelivery.Domain.Logic.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentDbService _commentDb;
        private readonly IMapper _mapper;

        public CommentService(IMapper mapper, ICommentDbService commentDb)
        {
            _commentDb = commentDb;
            _mapper = mapper;
        }

        public Task<Result<CommentDTO_Add>> AddAsync(CommentDTO_Add comment)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Asynchronously returns all comments
        /// </summary>
        public async Task<IEnumerable<CommentDTO_View>> GetAllAsync()
        {
            var comment = await _commentDb.GetAllAsync();
            return _mapper.Map<IEnumerable<CommentDB>, List<CommentDTO_View>>(comment);
        }

        public Task<CommentDTO_View> GetByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CommentDTO_View>> GetByUserIdAsync(string userID)
        {
            throw new NotImplementedException();
        }

        public void RemoveAll()
        {
            throw new NotImplementedException();
        }

        public void RemoveAllByUserId(string id)
        {
            throw new NotImplementedException();
        }

        public void RemoveById(string id)
        {
            throw new NotImplementedException();
        }

        public Task<Result> RemoveByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<Result<CommentDTO_Update>> UpdateAsync(CommentDTO_Update comment)
        {
            throw new NotImplementedException();
        }
    }
}
