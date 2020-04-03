using DreamFoodDelivery.Common.Helpers;
using DreamFoodDelivery.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DreamFoodDelivery.Data.Services.Interfaces
{
    public interface ICommentDbService
    {
        /// <summary>
        /// Asynchronously returns all comments
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<CommentDB>> GetAllAsync();

        /// <summary>
        /// Asynchronously returns comment by id
        /// </summary>
        /// <param name="id">comment id</param>
        /// <returns></returns>
        Task<CommentDB> GetByIdAsync(string id);

        /// <summary>
        /// Asynchronously returns comment by user id
        /// </summary>
        /// <param name="userID">User id</param>
        /// <returns></returns>
        Task<IEnumerable<CommentDB>> GetByUserIdAsync(string userID);

        /// <summary>
        /// Creates comment
        /// </summary>
        /// <param name="comment">New comment</param>
        /// <returns></returns>
        Task<Result<CommentDB>> AddAsync(CommentDB comment);

        /// <summary>
        /// Updates comment data async
        /// </summary>
        /// <param name="comment">comment</param>
        /// <returns></returns>
        Task<Result<CommentDB>> UpdateAsync(CommentDB comment);

        /// <summary>
        /// Removes comment by id
        /// </summary>
        /// <param name="id">comment id</param>
        /// <returns></returns>
        void RemoveById(string id);

        /// <summary>
        /// Removes comment by id async
        /// </summary>
        /// <param name="id">comment id</param>
        /// <returns></returns>
        Task<Result> RemoveByIdAsync(string id);

        /// <summary>
        /// Removes comments by user id
        /// </summary>
        /// <param name="id">user id</param>
        /// <returns></returns>
        void RemoveAllByUserId(string id);

        /// <summary>
        /// Removes all comments from database
        /// </summary>
        void RemoveAll();
    }
}
