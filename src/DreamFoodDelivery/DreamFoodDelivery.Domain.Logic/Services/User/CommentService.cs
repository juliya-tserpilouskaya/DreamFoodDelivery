using AutoMapper;
using DreamFoodDelivery.Common;
using DreamFoodDelivery.Data.Context;
using DreamFoodDelivery.Data.Models;
using DreamFoodDelivery.Domain.DTO;
using DreamFoodDelivery.Domain.Logic.InterfaceServices;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        /// Asynchronously returns all comments
        /// </summary>
        [LoggerAttribute]
        public async Task<Result<IEnumerable<CommentView>>> GetAllAsync()
        {
            var comments = await _context.Comments.AsNoTracking().ToListAsync();
            if (!comments.Any())
            {
                return Result<IEnumerable<CommentView>>.Fail<IEnumerable<CommentView>>("No comments found");
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
        public async Task<Result<CommentView>> GetByIdAsync(string commentId)
        {
            Guid id = Guid.Parse(commentId);
            try
            {
                var comment = await _context.Comments.Where(_ => _.Id == id).AsNoTracking().FirstOrDefaultAsync();
                if (comment is null)
                {
                    return Result<CommentView>.Fail<CommentView>($"Comment was not found");
                }
                CommentView view = _mapper.Map<CommentView>(comment);
                var order = await _orderService.GetByIdAsync(comment.OrderId.ToString());
                view.Order = order.Data;
                return Result<CommentView>.Ok(view);
            }
            catch (ArgumentNullException ex)
            {
                return Result<CommentView>.Fail<CommentView>($"Source is null. {ex.Message}");
            }
        }

        /// <summary>
        ///  Asynchronously add new comment
        /// </summary>
        /// <param name="comment">New comment to add</param>
        [LoggerAttribute]
        public async Task<Result<CommentView>> AddAsync(CommentToAdd comment)
        {
            var commentToAdd = _mapper.Map<CommentDB>(comment);
            _context.Comments.Add(commentToAdd);
            try
            {
                await _context.SaveChangesAsync();
                CommentDB commentAfterAdding = await _context.Comments.Where(_ => _.UserId == commentToAdd.UserId).Select(_ => _).AsNoTracking().FirstOrDefaultAsync();
                CommentView view = _mapper.Map<CommentView>(commentAfterAdding);
                var order = await _orderService.GetByIdAsync(comment.OrderId.ToString());
                view.Order = order.Data;
                return Result<CommentView>.Ok(view);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Result<CommentView>.Fail<CommentView>($"Cannot save model. {ex.Message}");
            }
            catch (DbUpdateException ex)
            {
                return Result<CommentView>.Fail<CommentView>($"Cannot save model. {ex.Message}");
            }
            catch (ArgumentNullException ex)
            {
                return Result<CommentView>.Fail<CommentView>($"Source is null. {ex.Message}");
            }
        }

        /// <summary>
        ///  Asynchronously get comment by userId. Id must be verified 
        /// </summary>
        /// <param name="userId">ID of user</param>
        [LoggerAttribute]
        public async Task<Result<IEnumerable<CommentView>>> GetByUserIdAsync(string userId)
        {
            Guid id = Guid.Parse(userId);
            var comments = await _context.Comments.Where(_ => _.UserId == id).Select(_ => _).ToListAsync();
            if (!comments.Any())
            {
                return Result<IEnumerable<CommentView>>.Fail<IEnumerable<CommentView>>("No comments found");
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
        public async Task<Result> RemoveAllAsync()
        {
            var comment = await _context.Comments.ToListAsync();
            if (comment is null)
            {
                return await Task.FromResult(Result.Fail("Comments were not found"));
            }
            try
            {
                _context.Comments.RemoveRange(comment);
                await _context.SaveChangesAsync();
                return await Task.FromResult(Result.Ok());
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return await Task.FromResult(Result.Fail($"Cannot delete comments. {ex.Message}"));
            }
            catch (DbUpdateException ex)
            {
                return await Task.FromResult(Result.Fail($"Cannot delete comments. {ex.Message}"));
            }
        }

        /// <summary>
        ///  Asynchronously remove all comments by user Id. Id must be verified 
        /// </summary>
        /// <param name="userId">ID of user</param>
        [LoggerAttribute]
        public async Task<Result> RemoveAllByUserIdAsync(string userId)
        {
            Guid id = Guid.Parse(userId);
            var comment = _context.Comments.Where(_ => _.UserId == id).Select(_ => _);
            if (comment is null)
            {
                return await Task.FromResult(Result.Fail("Commenst were not found"));
            }
            try
            {
                _context.Comments.RemoveRange(comment);
                await _context.SaveChangesAsync();
                return await Task.FromResult(Result.Ok());
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return await Task.FromResult(Result.Fail($"Cannot delete comments. {ex.Message}"));
            }
            catch (DbUpdateException ex)
            {
                return await Task.FromResult(Result.Fail($"Cannot delete comments. {ex.Message}"));
            }
        }

        /// <summary>
        ///  Asynchronously remove comment by Id. Id must be verified
        /// </summary>
        /// <param name="commentId">ID of existing comment</param>
        [LoggerAttribute]
        public async Task<Result> RemoveByIdAsync(string commentId)
        {
            Guid id = Guid.Parse(commentId);
            var comment = await _context.Comments.IgnoreQueryFilters().FirstOrDefaultAsync(_ => _.Id == id);
            if (comment is null)
            {
                return await Task.FromResult(Result.Fail("Comment was not found"));
            }
            try
            {
                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();
                return await Task.FromResult(Result.Ok());
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return await Task.FromResult(Result.Fail($"Cannot delete comment. {ex.Message}"));
            }
            catch (DbUpdateException ex)
            {
                return await Task.FromResult(Result.Fail($"Cannot delete comment. {ex.Message}"));
            }
        }

        /// <summary>
        ///  Asynchronously update comment
        /// </summary>
        /// <param name="comment">Existing comment to update</param>
        [LoggerAttribute]
        public async Task<Result<CommentView>> UpdateAsync(CommentToUpdate comment)
        {
            CommentDB commentForUpdate = _mapper.Map<CommentDB>(comment);
            commentForUpdate.ModificationTime = DateTime.Now;
            _context.Entry(commentForUpdate).Property(c => c.Headline).IsModified = true;
            _context.Entry(commentForUpdate).Property(c => c.Rating).IsModified = true;
            _context.Entry(commentForUpdate).Property(c => c.Content).IsModified = true;
            _context.Entry(commentForUpdate).Property(c => c.ModificationTime).IsModified = true;

            try
            {
                await _context.SaveChangesAsync();
                var commentAfterUpdate = await _context.Comments.Where(_ => _.Id == comment.Id).AsNoTracking().FirstOrDefaultAsync();
                if (commentAfterUpdate is null)
                {
                    return Result<CommentView>.Fail<CommentView>($"Comment was not found");
                }
                CommentView view = _mapper.Map<CommentView>(commentAfterUpdate);
                var order = await _orderService.GetByIdAsync(commentAfterUpdate.OrderId.ToString());
                view.Order = order.Data;
                return Result<CommentView>.Ok(view);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Result<CommentView>.Fail<CommentView>($"Cannot update model. {ex.Message}");
            }
            catch (DbUpdateException ex)
            {
                return Result<CommentView>.Fail<CommentView>($"Cannot update model. {ex.Message}");
            }
        }
    }
}
