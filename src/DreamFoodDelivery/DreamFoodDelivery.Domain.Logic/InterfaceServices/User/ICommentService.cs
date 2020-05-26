using DreamFoodDelivery.Common;
using DreamFoodDelivery.Domain.DTO;
using DreamFoodDelivery.Domain.View;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DreamFoodDelivery.Domain.Logic.InterfaceServices
{
    public interface ICommentService
    {
        /// <summary>
        /// Asynchronously returns all comments for users
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// </summary>
        Task<Result<PageResponse<CommentForUsersView>>> GetAllAsync(PageRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously returns all comments for admin
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// </summary>
        Task<Result<IEnumerable<CommentView>>> GetAllAdminAsync(CancellationToken cancellationToken = default);

        /// <summary>
        ///  Asynchronously get comment by comment Id. Id must be verified 
        /// </summary>
        /// <param name="commentId">ID of existing comment</param>
        Task<Result<CommentView>> GetByIdAsync(string commentId, CancellationToken cancellationToken = default);

        /// <summary>
        ///  Asynchronously get order by userId. Id must be verified 
        /// </summary>
        /// <param name="userId">ID of user</param>
        Task<Result<IEnumerable<CommentView>>> GetByUserIdAdminAsync(string userId, CancellationToken cancellationToken = default);

        /// <summary>
        ///  Asynchronously get order by userId. Id must be verified 
        /// </summary>
        /// <param name="userIdFromIdentity">ID of user from identity</param>
        Task<Result<IEnumerable<CommentView>>> GetByUserIdAsync(string userIdFromIdentity, CancellationToken cancellationToken = default);

        /// <summary>
        ///  Asynchronously add new comment
        /// </summary>
        /// <param name="comment">New comment to add</param>
        /// <param name="userIdFromIdentity">Existing user Id to add</param>
        Task<Result<CommentView>> AddAsync(CommentToAdd comment, string userIdFromIdentity, CancellationToken cancellationToken = default);

        /// <summary>
        ///  Asynchronously update comment
        /// </summary>
        /// <param name="comment">Existing comment to update</param>
        Task<Result<CommentView>> UpdateAsync(CommentToUpdate comment, CancellationToken cancellationToken = default);

        /// <summary>
        ///  Asynchronously remove comment by Id. Id must be verified
        /// </summary>
        /// <param name="commentId">ID of existing comment</param>
        Task<Result> RemoveByIdAsync(string commentId, CancellationToken cancellationToken = default);

        /// <summary>
        ///  Asynchronously remove all comments by user Id. Id must be verified 
        /// </summary>
        /// <param name="userId">ID of user</param>
        Task<Result> RemoveAllByUserIdAsync(string userId, CancellationToken cancellationToken = default);

        /// <summary>
        ///  Asynchronously remove all comments 
        /// </summary>
        Task<Result> RemoveAllAsync(CancellationToken cancellationToken = default);
    }
}
