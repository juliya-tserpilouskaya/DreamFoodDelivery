using AutoMapper;
using DreamFoodDelivery.Common;
using DreamFoodDelivery.Data.Context;
using DreamFoodDelivery.Data.Models;
using DreamFoodDelivery.Domain.DTO;
using DreamFoodDelivery.Domain.Logic.InterfaceServices;
using DreamFoodDelivery.Domain.View;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DreamFoodDelivery.Domain.Logic.Services
{
    public class ReviewService : IReviewService
    {
        private readonly DreamFoodDeliveryContext _context;
        private readonly IMapper _mapper;

        public ReviewService(IMapper mapper, DreamFoodDeliveryContext reviewContext)
        {
            _context = reviewContext;
            _mapper = mapper;
        }

        /// <summary>
        /// Asynchronously returns all reviews for users
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// </summary>
        [LoggerAttribute]
        public async Task<Result<PageResponse<ReviewForUsersView>>> GetAllAsync(PageRequest request, CancellationToken cancellationToken = default)
        {
            var count = await _context.Reviews.CountAsync();
            var reviews = await _context.Reviews.Skip((request.PageNumber - 1) * request.PageSize)
                                                 .Take(request.PageSize)
                                                 .AsNoTracking().ToListAsync(cancellationToken);
            if (!reviews.Any())
            {
                return Result<PageResponse<ReviewForUsersView>>.Quite<PageResponse<ReviewForUsersView>>(ExceptionConstants.REVIEWS_WERE_NOT_FOUND);
            }
            List<ReviewForUsersView> views = new List<ReviewForUsersView>();
            foreach (var review in reviews)
            {
                ReviewForUsersView view = _mapper.Map<ReviewForUsersView>(review);
                var order = await _context.Orders.Where(_ => _.Id == review.OrderId).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
                view.Name = order.Name;
                view.Surname = order.Surname;
                views.Add(view);
            }
            PageResponse<ReviewForUsersView> response = new PageResponse<ReviewForUsersView>(_mapper.Map<IEnumerable<ReviewForUsersView>>(views),
                                                                               count,
                                                                               request.PageNumber,
                                                                               request.PageSize);
            return Result<PageResponse<ReviewForUsersView>>.Ok(response);
        }

        /// <summary>
        /// Asynchronously returns all reviews for admin
        /// <param name="cancellationToken"></param>
        /// </summary>
        [LoggerAttribute]
        public async Task<Result<IEnumerable<ReviewView>>> GetAllAdminAsync(CancellationToken cancellationToken = default)
        {
            var reviews = await _context.Reviews.AsNoTracking().ToListAsync(cancellationToken);
            return !reviews.Any()
                ? Result<IEnumerable<ReviewView>>.Quite<IEnumerable<ReviewView>>(ExceptionConstants.REVIEWS_WERE_NOT_FOUND)
                : Result<IEnumerable<ReviewView>>.Ok(_mapper.Map<IEnumerable<ReviewView>>(reviews));
        }

        /// <summary>
        ///  Asynchronously get review by review Id. Id must be verified 
        /// </summary>
        /// <param name="reviewId">ID of existing review</param>
        [LoggerAttribute]
        public async Task<Result<ReviewView>> GetByIdAsync(string reviewId, CancellationToken cancellationToken = default)
        {
            Guid id = Guid.Parse(reviewId);
            try
            {
                var review = await _context.Reviews.Where(_ => _.Id == id).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
                return review is null
                    ? Result<ReviewView>.Quite<ReviewView>(ExceptionConstants.REVIEW_WAS_NOT_FOUND)
                    : Result<ReviewView>.Ok(_mapper.Map<ReviewView>(review));
            }
            catch (ArgumentNullException ex)
            {
                return Result<ReviewView>.Fail<ReviewView>(ExceptionConstants.SOURCE_IS_NULL + ex.Message);
            }
        }

        /// <summary>
        ///  Asynchronously add new review
        /// </summary>
        /// <param name="review">New review to add</param>
        /// <param name="userIdFromIdentity">Existing user Id to add</param>
        [LoggerAttribute]
        public async Task<Result<ReviewView>> AddAsync(ReviewToAdd review, string userIdFromIdentity, CancellationToken cancellationToken = default)
        {
            UserDB userDB = await _context.Users.Where(_ => _.IdFromIdentity == userIdFromIdentity).Select(_ => _).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
            var reviewToAdd = _mapper.Map<ReviewDB>(review);
            reviewToAdd.UserId = userDB.Id;
            reviewToAdd.OrderId = Guid.Parse(review.OrderId);
            _context.Reviews.Add(reviewToAdd);
            var rating = await _context.Rating.FirstOrDefaultAsync(cancellationToken);
            if (rating  is null)
            {
                rating = new RatingDB() { Count = 0, Sum = 0 };
                _context.Rating.Add(rating);
            }
            rating.Sum += reviewToAdd.Rating;
            rating.Count++;
            _context.Entry(rating).Property(c => c.Sum).IsModified = true;
            _context.Entry(rating).Property(c => c.Count).IsModified = true;
            try
            {
                await _context.SaveChangesAsync(cancellationToken);
                ReviewDB reviewAfterAdding = await _context.Reviews.Where(_ => _.Id == reviewToAdd.Id).Select(_ => _).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
                return Result<ReviewView>.Ok(_mapper.Map<ReviewView>(reviewAfterAdding));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Result<ReviewView>.Fail<ReviewView>(ExceptionConstants.CANNOT_SAVE_MODEL + ex.Message);
            }
            catch (DbUpdateException ex)
            {
                return Result<ReviewView>.Fail<ReviewView>(ExceptionConstants.CANNOT_SAVE_MODEL + ex.Message);
            }
            catch (ArgumentNullException ex)
            {
                return Result<ReviewView>.Fail<ReviewView>(ExceptionConstants.SOURCE_IS_NULL + ex.Message);
            }
        }

        /// <summary>
        ///  Asynchronously get review by userId. Id must be verified 
        /// </summary>
        /// <param name="userId">ID of user</param>
        /// <param name="cancellationToken"></param>
        [LoggerAttribute]
        public async Task<Result<IEnumerable<ReviewView>>> GetByUserIdAdminAsync(string userId, CancellationToken cancellationToken = default)
        {
            Guid id = Guid.Parse(userId);
            var reviews = await _context.Reviews.Where(_ => _.UserId == id).Select(_ => _).AsNoTracking().ToListAsync(cancellationToken);
            return !reviews.Any()
                ? Result<IEnumerable<ReviewView>>.Quite<IEnumerable<ReviewView>>(ExceptionConstants.REVIEWS_WERE_NOT_FOUND)
                : Result<IEnumerable<ReviewView>>.Ok(_mapper.Map<IEnumerable<ReviewView>>(reviews));
        }

        /// <summary>
        ///  Asynchronously get review by userId. Id must be verified 
        /// </summary>
        /// <param name="userIdFromIdentity">ID of user from identity</param>
        [LoggerAttribute]
        public async Task<Result<IEnumerable<ReviewView>>> GetByUserIdAsync(string userIdFromIdentity, CancellationToken cancellationToken = default)
        {
            UserDB user = await _context.Users.Where(_ => _.IdFromIdentity == userIdFromIdentity).Select(_ => _).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
            if (user is null)
            {
                return Result<IEnumerable<ReviewView>>.Fail<IEnumerable<ReviewView>>(ExceptionConstants.USER_WAS_NOT_FOUND);
            }
            var reviews = await _context.Reviews.Where(_ => _.UserId == user.Id).Select(_ => _).AsNoTracking().ToListAsync(cancellationToken);
            return !reviews.Any()
                ? Result<IEnumerable<ReviewView>>.Quite<IEnumerable<ReviewView>>(ExceptionConstants.REVIEWS_WERE_NOT_FOUND)
                : Result<IEnumerable<ReviewView>>.Ok(_mapper.Map<IEnumerable<ReviewView>>(reviews));
        }

        /// <summary>
        ///  Asynchronously remove all reviews 
        /// </summary>
        [LoggerAttribute]
        public async Task<Result> RemoveAllAsync(CancellationToken cancellationToken = default)
        {
            var reviews = await _context.Reviews.AsNoTracking().ToListAsync(cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            var result = await RatingDown(reviews);
            if (!result.IsSuccess)
            {
                return Result.Fail(ExceptionConstants.CANNOT_DELETE_REVIEWS);
            }
            return await RemoveReviewList(reviews);
        }

        /// <summary>
        ///  Asynchronously remove all reviews by user Id. Id must be verified 
        /// </summary>
        /// <param name="userId">ID of user</param>
        [LoggerAttribute]
        public async Task<Result> RemoveAllByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            Guid id = Guid.Parse(userId);
            var reviews = await _context.Reviews.Where(_ => _.UserId == id).Select(_ => _).AsNoTracking().ToListAsync(); //check
            var result = await RatingDown(reviews);
            if (!result.IsSuccess)
            {
                return Result.Fail(ExceptionConstants.CANNOT_DELETE_REVIEWS);
            }
            return await RemoveReviewList(reviews);
        }

        /// <summary>
        ///  Asynchronously remove review by Id. Id must be verified
        /// </summary>
        /// <param name="reviewId">ID of existing review</param>
        [LoggerAttribute]
        public async Task<Result> RemoveByIdAsync(string reviewId, CancellationToken cancellationToken = default)
        {
            Guid id = Guid.Parse(reviewId);
            var review = await _context.Reviews.IgnoreQueryFilters().FirstOrDefaultAsync(_ => _.Id == id);
            if (review is null)
            {
                return await Task.FromResult(Result.Quite(ExceptionConstants.REVIEW_WAS_NOT_FOUND));
            }
            var rating = await _context.Rating.FirstOrDefaultAsync(cancellationToken);
            if (rating is null)
            {
                rating = new RatingDB() { Count = 0, Sum = 0 };
                _context.Rating.Add(rating);
            }
            rating.Sum -= review.Rating;
            rating.Count--;
            _context.Entry(rating).Property(c => c.Sum).IsModified = true;
            _context.Entry(rating).Property(c => c.Count).IsModified = true;
            try
            {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync(cancellationToken);
                return await Task.FromResult(Result.Ok());
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return await Task.FromResult(Result.Fail(ExceptionConstants.CANNOT_DELETE_REVIEW + ex.Message));
            }
            catch (DbUpdateException ex)
            {
                return await Task.FromResult(Result.Fail(ExceptionConstants.CANNOT_DELETE_REVIEW + ex.Message));
            }
        }

        /// <summary>
        ///  Asynchronously update review
        /// </summary>
        /// <param name="review">Existing review to update</param>
        [LoggerAttribute]
        public async Task<Result<ReviewView>> UpdateAsync(ReviewToUpdate review, CancellationToken cancellationToken = default)
        {
            ReviewDB reviewForUpdate = _mapper.Map<ReviewDB>(review);
            ReviewDB reviewOld = await _context.Reviews.AsNoTracking().FirstOrDefaultAsync(_ => _.Id == Guid.Parse(review.Id));
            if (reviewOld is null)
            {
                return Result<ReviewView>.Quite<ReviewView>(ExceptionConstants.REVIEW_WAS_NOT_FOUND);
            }
            var rating = await _context.Rating.AsNoTracking().FirstOrDefaultAsync(cancellationToken);
            if (rating is null)
            {
                rating = new RatingDB() { Count = 0, Sum = 0 };
                _context.Rating.Add(rating);
            }
            rating.Sum = rating.Sum.Value - reviewOld.Rating + reviewForUpdate.Rating;
            _context.Entry(rating).Property(c => c.Sum).IsModified = true;
            reviewForUpdate.Id = Guid.Parse(review.Id);
            reviewForUpdate.ModificationTime = DateTime.Now;
            _context.Entry(reviewForUpdate).Property(c => c.Headline).IsModified = true;
            _context.Entry(reviewForUpdate).Property(c => c.Rating).IsModified = true;
            _context.Entry(reviewForUpdate).Property(c => c.Content).IsModified = true;
            _context.Entry(reviewForUpdate).Property(c => c.ModificationTime).IsModified = true;

            try
            {
                await _context.SaveChangesAsync();
                var reviewAfterUpdate = await _context.Reviews.Where(_ => _.Id == Guid.Parse(review.Id)).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
                return reviewAfterUpdate is null
                    ? Result<ReviewView>.Quite<ReviewView>(ExceptionConstants.REVIEW_WAS_NOT_FOUND)
                    : Result<ReviewView>.Ok(_mapper.Map<ReviewView>(reviewAfterUpdate));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Result<ReviewView>.Fail<ReviewView>(ExceptionConstants.CANNOT_UPDATE_MODEL + ex.Message);
            }
            catch (DbUpdateException ex)
            {
                return Result<ReviewView>.Fail<ReviewView>(ExceptionConstants.CANNOT_UPDATE_MODEL + ex.Message);
            }
        }

        /// <summary>
        /// Gets reviews and delete them
        /// </summary>
        /// <param name="reviews">Reviews list</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Result> RemoveReviewList(List<ReviewDB> reviews, CancellationToken cancellationToken = default)
        {
            if (reviews.Count is 0)
            {
                return await Task.FromResult(Result.Quite(ExceptionConstants.REVIEWS_WERE_NOT_FOUND));
            }
            try
            {
                _context.Reviews.RemoveRange(reviews);
                await _context.SaveChangesAsync(cancellationToken);
                return await Task.FromResult(Result.Ok());
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return await Task.FromResult(Result.Fail(ExceptionConstants.CANNOT_DELETE_REVIEWS + ex.Message));
            }
            catch (DbUpdateException ex)
            {
                return await Task.FromResult(Result.Fail(ExceptionConstants.CANNOT_DELETE_REVIEWS + ex.Message));
            }
        }

        /// <summary>
        /// Get rating summ and count
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Result<RatingView>> GetRating(CancellationToken cancellationToken = default)
        {
            //throw new InvalidOperationException("MY TEST STRING!!!");
            var rating = await _context.Rating.FirstOrDefaultAsync(cancellationToken);
            return rating is null
                ? Result<RatingView>.Quite<RatingView>(ExceptionConstants.EMPTY_RATING)
                : Result<RatingView>.Ok(_mapper.Map<RatingView>(rating));
        }

        /// <summary>
        /// Delete ratings data
        /// </summary>
        /// <param name="reviews"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<Result> RatingDown(List<ReviewDB> reviews, CancellationToken cancellationToken = default)
        {
            RatingDB rating = await _context.Rating.FirstOrDefaultAsync(cancellationToken);
            if (rating is null)
            {
                rating = new RatingDB() { Count = 0, Sum = 0 };
                _context.Rating.Add(rating);
            }
            int count = reviews.Count;
            int sum = 0;
            for (int i = 0; i < reviews.Count; i++)
            {
                sum += reviews[i].Rating.Value;
            }
            rating.Count -= count;
            rating.Sum -= sum;
            if (rating.Count < 0 || rating.Sum <0)
            {
                return Result.Quite();
            }

            _context.Entry(rating).Property(c => c.Sum).IsModified = true;
            _context.Entry(rating).Property(c => c.Count).IsModified = true;
            try
            {
                await _context.SaveChangesAsync(cancellationToken);
                return Result.Ok();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Result.Fail(ExceptionConstants.CANNOT_SAVE_MODEL + ex.Message);
            }
            catch (DbUpdateException ex)
            {
                return Result.Fail(ExceptionConstants.CANNOT_SAVE_MODEL + ex.Message);
            }
        }
    }
}
