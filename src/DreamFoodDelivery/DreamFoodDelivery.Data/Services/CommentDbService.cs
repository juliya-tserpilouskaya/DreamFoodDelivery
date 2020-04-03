using DreamFoodDelivery.Common.Helpers;
using DreamFoodDelivery.Data.Context;
using DreamFoodDelivery.Data.Models;
using DreamFoodDelivery.Data.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DreamFoodDelivery.Data.Services
{
    public class CommentDbService : ICommentDbService
    {
        private readonly DreamFoodDeliveryContext _context;
        //private bool _isDisposed;

        public CommentDbService()
        {
            this._context = new DreamFoodDeliveryContext();
        }

        public CommentDbService(DreamFoodDeliveryContext context)
        {
            this._context = context;
        }

        public Task<Result<CommentDB>> AddAsync(CommentDB comment)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Asynchronously returns all comments
        /// </summary>
        public async Task<IEnumerable<CommentDB>> GetAllAsync()
        {
            var comments = await _context.Comments.ToListAsync().ConfigureAwait(false);
            return comments;
        }

        public Task<CommentDB> GetByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CommentDB>> GetByUserIdAsync(string userID)
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

        public Task<Result<CommentDB>> UpdateAsync(CommentDB comment)
        {
            throw new NotImplementedException();
        }
    }
}
