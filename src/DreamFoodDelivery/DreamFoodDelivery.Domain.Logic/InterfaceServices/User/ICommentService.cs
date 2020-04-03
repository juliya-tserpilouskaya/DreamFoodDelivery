﻿using DreamFoodDelivery.Common.Helpers;
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
        /// Returns all comments
        /// </summary>
        /// <returns></returns>
        //IEnumerable<CommentDTO_View> GetAll();

        /// <summary>
        /// Asynchronously returns all comments
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<CommentDTO_View>> GetAllAsync();

        /// <summary>
        /// Returns comment by id
        /// </summary>
        /// <param name="id">comment id</param>
        /// <returns></returns>
        //CommentDTO_View GetById(string id);

        /// <summary>
        /// Asynchronously returns comment by id
        /// </summary>
        /// <param name="id">comment id</param>
        /// <returns></returns>
        Task<CommentDTO_View> GetByIdAsync(string id);

        /// <summary>
        /// Returns comment by user id
        /// </summary>
        /// <param name="userID">User id</param>
        /// <returns></returns>
        //IEnumerable<CommentDTO_View> GetByUserId(string userID);

        /// <summary>
        /// Asynchronously returns comment by user id
        /// </summary>
        /// <param name="userID">User id</param>
        /// <returns></returns>
        Task<IEnumerable<CommentDTO_View>> GetByUserIdAsync(string userID);

        /// <summary>
        /// Creates comment
        /// </summary>
        /// <param name="comment">New comment</param>
        /// <returns></returns>
        //CommentDTO_Add Add(CommentDTO_Add comment);

        /// <summary>
        /// Creates comment
        /// </summary>
        /// <param name="comment">New comment</param>
        /// <returns></returns>
        Task<Result<CommentDTO_Add>> AddAsync(CommentDTO_Add comment);

        /// <summary>
        /// Updates comment
        /// </summary>
        /// <param name="comment">comment</param>
        /// <returns></returns>
        //CommentDTO_Update Update(CommentDTO_Update comment);

        /// <summary>
        /// Updates comment data async
        /// </summary>
        /// <param name="comment">comment</param>
        /// <returns></returns>
        Task<Result<CommentDTO_Update>> UpdateAsync(CommentDTO_Update comment);

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
