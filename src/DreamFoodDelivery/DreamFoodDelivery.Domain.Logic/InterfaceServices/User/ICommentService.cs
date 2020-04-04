using DreamFoodDelivery.Common.Helpers;
using DreamFoodDelivery.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DreamFoodDelivery.Domain.Logic.InterfaceServices
{
    public interface ICommentService
    {
        /// <summary>
        /// Asynchronously returns all comments
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<CommentView>> GetAllAsync();

        /// <summary>
        /// Asynchronously returns comment by id
        /// </summary>
        /// <param name="id">comment id</param>
        /// <returns></returns>
        Task<CommentView> GetByIdAsync(string id);

        /// <summary>
        /// Asynchronously returns comment by user id
        /// </summary>
        /// <param name="userID">User id</param>
        /// <returns></returns>
        Task<IEnumerable<CommentView>> GetByUserIdAsync(string userID);

        /// <summary>
        /// Creates comment
        /// </summary>
        /// <param name="comment">New comment</param>
        /// <returns></returns>
        Task<Result<CommentToAdd>> AddAsync(CommentToAdd comment);

        /// <summary>
        /// Updates comment data async
        /// </summary>
        /// <param name="comment">comment</param>
        /// <returns></returns>
        Task<Result<CommentToUpdate>> UpdateAsync(CommentToUpdate comment);

        /// <summary>
        /// Removes comment by id async
        /// </summary>
        /// <param name="id">comment id</param>
        /// <returns></returns>
        Task<Result> RemoveByIdAsync(string id);

        /// <summary>
        /// Removes comments by user id async
        /// </summary>
        /// <param name="id">user id</param>
        /// <returns></returns>
        Task<Result> RemoveAllByUserIdAsync(string userId);

        /// <summary>
        /// Removes comments by user id
        /// </summary>
        /// <param name="id">user id</param>
        /// <returns></returns>
        //void RemoveAllByUserId(string id);

        /// <summary>
        /// Removes all comments from database
        /// </summary>
        Task<Result> RemoveAllAsync();

        /// <summary>
        /// Removes all comments from database
        /// </summary>
        //void RemoveAll();
    }
}
