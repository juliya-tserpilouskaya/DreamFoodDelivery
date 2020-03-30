using DreamFoodDelivery.Common.Helpers;
using DreamFoodDelivery.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DreamFoodDelivery.Domain.Logic.InterfaceServices
{
    public interface ICommentService
    {
        /// <summary>
        /// Returns all comments
        /// </summary>
        /// <returns></returns>
        IEnumerable<Comment> GetAll();

        /// <summary>
        /// Asynchronously returns all comments
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Comment>> GetAllAsync();

        /// <summary>
        /// Returns comment by id
        /// </summary>
        /// <param name="id">comment id</param>
        /// <returns></returns>
        Comment GetById(string id);

        /// <summary>
        /// Asynchronously returns comment by id
        /// </summary>
        /// <param name="id">comment id</param>
        /// <returns></returns>
        Task<Comment> GetByIdAsync(string id);

        /// <summary>
        /// Returns comment by user id
        /// </summary>
        /// <param name="userID">User id</param>
        /// <returns></returns>
        IEnumerable<Comment> GetByUserId(string userID);

        /// <summary>
        /// Asynchronously returns comment by user id
        /// </summary>
        /// <param name="userID">User id</param>
        /// <returns></returns>
        Task<IEnumerable<Comment>> GetByUserIdAsync(string userID);

        /// <summary>
        /// Creates comment
        /// </summary>
        /// <param name="comment">New comment</param>
        /// <returns></returns>
        Comment Add(Comment comment);

        /// <summary>
        /// Creates comment
        /// </summary>
        /// <param name="comment">New comment</param>
        /// <returns></returns>
        Task<Result<Comment>> AddAsync(Comment bookmark);

        /// <summary>
        /// Updates comment
        /// </summary>
        /// <param name="comment">comment</param>
        /// <returns></returns>
        Comment Update(Comment comment);

        /// <summary>
        /// Updates comment data async
        /// </summary>
        /// <param name="comment">comment</param>
        /// <returns></returns>
        Task<Result<Comment>> UpdateAsync(Comment comment);

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
