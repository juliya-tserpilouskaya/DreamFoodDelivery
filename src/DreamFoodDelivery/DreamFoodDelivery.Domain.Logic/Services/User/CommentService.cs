using AutoMapper;
using DreamFoodDelivery.Common;
using DreamFoodDelivery.Common.Сonstants;
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
    public class CommentService : ICommentService
    {
        private readonly DreamFoodDeliveryContext _context;
        private readonly IMapper _mapper;
        IOrderService _orderService;

        public CommentService(IMapper mapper, DreamFoodDeliveryContext commentContext, IOrderService orderService)
        {
            _context = commentContext;
            _mapper = mapper;
            _orderService = orderService;
        }

        /// <summary>
        /// Asynchronously returns all comments for users
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// </summary>
        [LoggerAttribute]
        public async Task<Result<PageResponse<CommentForUsersView>>> GetAllAsync(PageRequest request, CancellationToken cancellationToken = default)
        {
            //var comments = await _context.Comments.AsNoTracking().ToListAsync(cancellationToken);
            var count = await _context.Comments.CountAsync();
            var comments = await _context.Comments.Skip((request.PageNumber - 1) * request.PageSize)
                                                 .Take(request.PageSize)
                                                 .AsNoTracking().ToListAsync(cancellationToken);
            if (!comments.Any())
            {
                return Result<PageResponse<CommentForUsersView>>.Quite<PageResponse<CommentForUsersView>>(ExceptionConstants.COMMENTS_WERE_NOT_FOUND);
            }
            List<CommentForUsersView> views = new List<CommentForUsersView>();
            foreach (var comment in comments)
            {
                CommentForUsersView view = _mapper.Map<CommentForUsersView>(comment);
                var order = await _orderService.GetByIdAsync(comment.OrderId.ToString());
                view.Name = order.Data.Name;
                view.Surname = order.Data.Surname;
                views.Add(view);
            }
            PageResponse<CommentForUsersView> response = new PageResponse<CommentForUsersView>(_mapper.Map<IEnumerable<CommentForUsersView>>(views),
                                                                               count,
                                                                               request.PageNumber,
                                                                               request.PageSize);
            return Result<PageResponse<CommentForUsersView>>.Ok(response);
            //return Result<IEnumerable<CommentView>>.Ok(_mapper.Map<IEnumerable<CommentView>>(views));
        }

        /// <summary>
        /// Asynchronously returns all comments for admin
        /// <param name="cancellationToken"></param>
        /// </summary>
        [LoggerAttribute]
        public async Task<Result<IEnumerable<CommentView>>> GetAllAdminAsync(CancellationToken cancellationToken = default)
        {
            var comments = await _context.Comments.AsNoTracking().ToListAsync(cancellationToken);
            if (!comments.Any())
            {
                return Result<IEnumerable<CommentView>>.Quite<IEnumerable<CommentView>>(ExceptionConstants.COMMENTS_WERE_NOT_FOUND);
            }
            List<CommentView> views = new List<CommentView>();
            foreach (var comment in comments)
            {
                CommentView view = _mapper.Map<CommentView>(comment);
                var order = await _orderService.GetByIdAsync(comment.OrderId.ToString());
                view.Order = order.Data;
                views.Add(view);
            }
            return Result<IEnumerable<CommentView>>.Ok(_mapper.Map<IEnumerable<CommentView>>(views));
        }

        /// <summary>
        ///  Asynchronously get comment by comment Id. Id must be verified 
        /// </summary>
        /// <param name="commentId">ID of existing comment</param>
        [LoggerAttribute]
        public async Task<Result<CommentView>> GetByIdAsync(string commentId, CancellationToken cancellationToken = default)
        {
            Guid id = Guid.Parse(commentId);
            try
            {
                var comment = await _context.Comments.Where(_ => _.Id == id).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
                if (comment is null)
                {
                    return Result<CommentView>.Quite<CommentView>(ExceptionConstants.COMMENT_WAS_NOT_FOUND);
                }
                CommentView view = _mapper.Map<CommentView>(comment);
                var order = await _orderService.GetByIdAsync(comment.OrderId.ToString());
                view.Order = order.Data;
                return Result<CommentView>.Ok(view);
            }
            catch (ArgumentNullException ex)
            {
                return Result<CommentView>.Fail<CommentView>(ExceptionConstants.SOURCE_IS_NULL + ex.Message);
            }
        }

        /// <summary>
        ///  Asynchronously add new comment
        /// </summary>
        /// <param name="comment">New comment to add</param>
        /// <param name="userIdFromIdentity">Existing user Id to add</param>
        [LoggerAttribute]
        public async Task<Result<CommentView>> AddAsync(CommentToAdd comment, string userIdFromIdentity, CancellationToken cancellationToken = default)
        {
            UserDB userDB = await _context.Users.Where(_ => _.IdFromIdentity == userIdFromIdentity).Select(_ => _).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
            var commentToAdd = _mapper.Map<CommentDB>(comment);
            commentToAdd.UserId = userDB.Id;
            commentToAdd.OrderId = Guid.Parse(comment.OrderId);
            _context.Comments.Add(commentToAdd);
            try
            {
                await _context.SaveChangesAsync(cancellationToken);
                CommentDB commentAfterAdding = await _context.Comments.Where(_ => _.Id == commentToAdd.Id).Select(_ => _).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
                CommentView view = _mapper.Map<CommentView>(commentAfterAdding);
                var order = await _orderService.GetByIdAsync(comment.OrderId.ToString());
                view.Order = order.Data;
                return Result<CommentView>.Ok(view);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Result<CommentView>.Fail<CommentView>(ExceptionConstants.CANNOT_SAVE_MODEL + ex.Message);
            }
            catch (DbUpdateException ex)
            {
                return Result<CommentView>.Fail<CommentView>(ExceptionConstants.CANNOT_SAVE_MODEL + ex.Message);
            }
            catch (ArgumentNullException ex)
            {
                return Result<CommentView>.Fail<CommentView>(ExceptionConstants.SOURCE_IS_NULL + ex.Message);
            }
        }

        /// <summary>
        ///  Asynchronously get comment by userId. Id must be verified 
        /// </summary>
        /// <param name="userId">ID of user</param>
        ///// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        [LoggerAttribute]
        public async Task<Result<IEnumerable<CommentView>>> GetByUserIdAdminAsync(/*PageRequest<string> request*/ string userId, CancellationToken cancellationToken = default)
        {
            Guid id = Guid.Parse(userId);
            var comments = await _context.Comments.Where(_ => _.UserId == id).Select(_ => _).AsNoTracking().ToListAsync(cancellationToken);
            if (!comments.Any())
            {
                return Result<IEnumerable<CommentView>>.Quite<IEnumerable<CommentView>>(ExceptionConstants.COMMENTS_WERE_NOT_FOUND);
            }
            List<CommentView> views = new List<CommentView>();
            foreach (var comment in comments)
            {
                CommentView view = _mapper.Map<CommentView>(comment);
                var order = await _orderService.GetByIdAsync(comment.OrderId.ToString());
                view.Order = order.Data;
                views.Add(view);
            }
            return Result<IEnumerable<CommentView>>.Ok(_mapper.Map<IEnumerable<CommentView>>(views));
        }

        /// <summary>
        ///  Asynchronously get comment by userId. Id must be verified 
        /// </summary>
        /// <param name="userIdFromIdentity">ID of user from identity</param>
        [LoggerAttribute]
        public async Task<Result<IEnumerable<CommentView>>> GetByUserIdAsync(string userIdFromIdentity, CancellationToken cancellationToken = default)
        {
            UserDB user = await _context.Users.Where(_ => _.IdFromIdentity == userIdFromIdentity).Select(_ => _).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
            if (user is null)
            {
                return Result<IEnumerable<CommentView>>.Fail<IEnumerable<CommentView>>(ExceptionConstants.USER_WAS_NOT_FOUND);
            }
            var comments = await _context.Comments.Where(_ => _.UserId == user.Id).Select(_ => _).AsNoTracking().ToListAsync(cancellationToken);
            if (!comments.Any())
            {
                return Result<IEnumerable<CommentView>>.Quite<IEnumerable<CommentView>>(ExceptionConstants.COMMENTS_WERE_NOT_FOUND);
            }
            List<CommentView> views = new List<CommentView>();
            foreach (var comment in comments)
            {
                CommentView view = _mapper.Map<CommentView>(comment);
                var order = await _orderService.GetByIdAsync(comment.OrderId.ToString());
                view.Order = order.Data;
                views.Add(view);
            }
            return Result<IEnumerable<CommentView>>.Ok(_mapper.Map<IEnumerable<CommentView>>(views));
        }

        /// <summary>
        ///  Asynchronously remove all comments 
        /// </summary>
        [LoggerAttribute]
        public async Task<Result> RemoveAllAsync(CancellationToken cancellationToken = default)
        {
            var comment = await _context.Comments.AsNoTracking().ToListAsync(cancellationToken);
            if (comment is null)
            {
                return await Task.FromResult(Result.Quite(ExceptionConstants.COMMENTS_WERE_NOT_FOUND));
            }
            try
            {
                _context.Comments.RemoveRange(comment);
                await _context.SaveChangesAsync(cancellationToken);
                return await Task.FromResult(Result.Ok());
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return await Task.FromResult(Result.Fail(ExceptionConstants.CANNOT_DELETE_COMMENTS + ex.Message));
            }
            catch (DbUpdateException ex)
            {
                return await Task.FromResult(Result.Fail(ExceptionConstants.CANNOT_DELETE_COMMENTS + ex.Message));
            }
        }

        /// <summary>
        ///  Asynchronously remove all comments by user Id. Id must be verified 
        /// </summary>
        /// <param name="userId">ID of user</param>
        [LoggerAttribute]
        public async Task<Result> RemoveAllByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            Guid id = Guid.Parse(userId);
            var comment = await _context.Comments.Where(_ => _.UserId == id).Select(_ => _).AsNoTracking().ToListAsync(); //check
            if (!comment.Any())
            {
                return await Task.FromResult(Result.Quite(ExceptionConstants.COMMENTS_WERE_NOT_FOUND));
            }
            try
            {
                _context.Comments.RemoveRange(comment);
                await _context.SaveChangesAsync(cancellationToken);
                return await Task.FromResult(Result.Ok());
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return await Task.FromResult(Result.Fail(ExceptionConstants.CANNOT_DELETE_COMMENTS + ex.Message));
            }
            catch (DbUpdateException ex)
            {
                return await Task.FromResult(Result.Fail(ExceptionConstants.CANNOT_DELETE_COMMENTS + ex.Message));
            }
        }

        /// <summary>
        ///  Asynchronously remove comment by Id. Id must be verified
        /// </summary>
        /// <param name="commentId">ID of existing comment</param>
        [LoggerAttribute]
        public async Task<Result> RemoveByIdAsync(string commentId, CancellationToken cancellationToken = default)
        {
            Guid id = Guid.Parse(commentId);
            var comment = await _context.Comments.IgnoreQueryFilters().FirstOrDefaultAsync(_ => _.Id == id);
            if (comment is null)
            {
                return await Task.FromResult(Result.Quite(ExceptionConstants.COMMENT_WAS_NOT_FOUND));
            }
            try
            {
                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync(cancellationToken);
                return await Task.FromResult(Result.Ok());
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return await Task.FromResult(Result.Fail(ExceptionConstants.CANNOT_DELETE_COMMENT + ex.Message));
            }
            catch (DbUpdateException ex)
            {
                return await Task.FromResult(Result.Fail(ExceptionConstants.CANNOT_DELETE_COMMENT + ex.Message));
            }
        }

        /// <summary>
        ///  Asynchronously update comment
        /// </summary>
        /// <param name="comment">Existing comment to update</param>
        [LoggerAttribute]
        public async Task<Result<CommentView>> UpdateAsync(CommentToUpdate comment, CancellationToken cancellationToken = default)
        {
            CommentDB commentForUpdate = _mapper.Map<CommentDB>(comment);
            commentForUpdate.Id = Guid.Parse(comment.Id);
            commentForUpdate.ModificationTime = DateTime.Now;
            _context.Entry(commentForUpdate).Property(c => c.Headline).IsModified = true;
            _context.Entry(commentForUpdate).Property(c => c.Rating).IsModified = true;
            _context.Entry(commentForUpdate).Property(c => c.Content).IsModified = true;
            _context.Entry(commentForUpdate).Property(c => c.ModificationTime).IsModified = true;

            try
            {
                await _context.SaveChangesAsync();
                var commentAfterUpdate = await _context.Comments.Where(_ => _.Id == Guid.Parse(comment.Id)).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
                if (commentAfterUpdate is null)
                {
                    return Result<CommentView>.Quite<CommentView>(ExceptionConstants.COMMENT_WAS_NOT_FOUND);
                }
                CommentView view = _mapper.Map<CommentView>(commentAfterUpdate);
                var order = await _orderService.GetByIdAsync(commentAfterUpdate.OrderId.ToString());
                view.Order = order.Data;
                return Result<CommentView>.Ok(view);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Result<CommentView>.Fail<CommentView>(ExceptionConstants.CANNOT_UPDATE_MODEL + ex.Message);
            }
            catch (DbUpdateException ex)
            {
                return Result<CommentView>.Fail<CommentView>(ExceptionConstants.CANNOT_UPDATE_MODEL + ex.Message);
            }
        }
    }
}
