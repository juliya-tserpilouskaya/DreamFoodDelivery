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
    public interface IReviewService
    {
        /// <summary>
        /// Asynchronously returns all reviews for users
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// </summary>
        Task<Result<PageResponse<ReviewForUsersView>>> GetAllAsync(PageRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously returns all reviews for admin
        /// <param name="cancellationToken"></param>
        /// </summary>
        Task<Result<IEnumerable<ReviewView>>> GetAllAdminAsync(CancellationToken cancellationToken = default);

        /// <summary>
        ///  Asynchronously get review by review Id. Id must be verified 
        /// </summary>
        /// <param name="reviewId">ID of existing review</param>
        Task<Result<ReviewView>> GetByIdAsync(string reviewId, CancellationToken cancellationToken = default);

        /// <summary>
        ///  Asynchronously get order by userId. Id must be verified 
        /// </summary>
        /// <param name="userId">ID of user</param>
        Task<Result<IEnumerable<ReviewView>>> GetByUserIdAdminAsync(string userId, CancellationToken cancellationToken = default);

        /// <summary>
        ///  Asynchronously get order by userId. Id must be verified 
        /// </summary>
        /// <param name="userIdFromIdentity">ID of user from identity</param>
        Task<Result<IEnumerable<ReviewView>>> GetByUserIdAsync(string userIdFromIdentity, CancellationToken cancellationToken = default);

        /// <summary>
        ///  Asynchronously add new review
        /// </summary>
        /// <param name="review">New review to add</param>
        /// <param name="userIdFromIdentity">Existing user Id to add</param>
        Task<Result<ReviewView>> AddAsync(ReviewToAdd review, string userIdFromIdentity, CancellationToken cancellationToken = default);

        /// <summary>
        ///  Asynchronously update review
        /// </summary>
        /// <param name="review">Existing review to update</param>
        Task<Result<ReviewView>> UpdateAsync(ReviewToUpdate review, CancellationToken cancellationToken = default);

        /// <summary>
        ///  Asynchronously remove review by Id. Id must be verified
        /// </summary>
        /// <param name="reviewId">ID of existing review</param>
        Task<Result> RemoveByIdAsync(string reviewId, CancellationToken cancellationToken = default);

        /// <summary>
        ///  Asynchronously remove all reviews by user Id. Id must be verified 
        /// </summary>
        /// <param name="userId">ID of user</param>
        Task<Result> RemoveAllByUserIdAsync(string userId, CancellationToken cancellationToken = default);

        /// <summary>
        ///  Asynchronously remove all reviews 
        /// </summary>
        Task<Result> RemoveAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously returns rating
        /// <param name="cancellationToken"></param>
        /// </summary>
        Task<Result<RatingView>> GetRating(CancellationToken cancellationToken = default);
    }
}
