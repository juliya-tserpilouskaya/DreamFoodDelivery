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
        Task<Result<IEnumerable<CommentView>>> GetAllAsync();

        /// <summary>
        ///  Asynchronously get comment by comment Id. Id must be verified 
        /// </summary>
        /// <param name="commentId">ID of existing comment</param>
        Task<Result<CommentView>> GetByIdAsync(string commentId);

        /// <summary>
        ///  Asynchronously get order by userId. Id must be verified 
        /// </summary>
        /// <param name="userId">ID of user</param>
        Task<IEnumerable<CommentView>> GetByUserIdAsync(string userId);

        /// <summary>
        ///  Asynchronously add new comment
        /// </summary>
        /// <param name="comment">New comment to add</param>
        Task<Result<CommentToAdd>> AddAsync(CommentToAdd comment);

        /// <summary>
        ///  Asynchronously update comment
        /// </summary>
        /// <param name="comment">Existing comment to update</param>
        Task<Result<CommentToUpdate>> UpdateAsync(CommentToUpdate comment);

        /// <summary>
        ///  Asynchronously remove comment by Id. Id must be verified
        /// </summary>
        /// <param name="commentId">ID of existing comment</param>
        Task<Result> RemoveByIdAsync(string commentId);

        /// <summary>
        ///  Asynchronously remove all comments by user Id. Id must be verified 
        /// </summary>
        /// <param name="userId">ID of user</param>
        Task<Result> RemoveAllByUserIdAsync(string userId);

        /// <summary>
        ///  Asynchronously remove all comments 
        /// </summary>
        Task<Result> RemoveAllAsync();
    }
}
