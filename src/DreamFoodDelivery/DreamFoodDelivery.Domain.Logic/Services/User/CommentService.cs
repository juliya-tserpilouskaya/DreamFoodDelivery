using AutoMapper;
using DreamFoodDelivery.Common.Helpers;
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
        private readonly DreamFoodDeliveryContext _commentContext;
        private readonly IMapper _mapper;

        public CommentService(IMapper mapper, DreamFoodDeliveryContext commentContext)
        {
            _commentContext = commentContext;
            _mapper = mapper;
        }

        /// <summary>
        ///  Asynchronously add new comment
        /// </summary>
        /// <param name="comment">New comment to add</param>
        public async Task<Result<CommentToAdd>> AddAsync(CommentToAdd comment)
        {
            var commentToAdd = _mapper.Map<CommentDB>(comment);

            _commentContext.Comments.Add(commentToAdd);

            try
            {
                await _commentContext.SaveChangesAsync();

                CommentDB commentAfterAdding = await _commentContext.Comments.Where(_ => _.UserId == commentToAdd.UserId).Select(_ => _).AsNoTracking().FirstOrDefaultAsync();

                return (Result<CommentToAdd>)Result<CommentToAdd>
                    .Ok(_mapper.Map<CommentToAdd>(commentAfterAdding));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Result<CommentToAdd>.Fail<CommentToAdd>($"Cannot save model. {ex.Message}");
            }
            catch (DbUpdateException ex)
            {
                return Result<CommentToAdd>.Fail<CommentToAdd>($"Cannot save model. {ex.Message}");
            }
            catch (ArgumentNullException ex)
            {
                return Result<CommentToAdd>.Fail<CommentToAdd>($"Source is null. {ex.Message}");
            }
        }

        /// <summary>
        /// Asynchronously returns all comments
        /// </summary>
        public async Task<Result<IEnumerable<CommentView>>> GetAllAsync()
        {
            var comments = await _commentContext.Comments.AsNoTracking().ToListAsync();
            if (!comments.Any())
            {
                return Result<IEnumerable<CommentView>>.Fail<IEnumerable<CommentView>>("No Users found");
            }
            return Result<IEnumerable<CommentView>>.Ok(_mapper.Map<IEnumerable<CommentView>>(comments));
        }

        /// <summary>
        ///  Asynchronously get comment by comment Id. Id must be verified 
        /// </summary>
        /// <param name="commentId">ID of existing comment</param>
        public async Task<Result<CommentView>> GetByIdAsync(string commentId)
        {
            Guid id = Guid.Parse(commentId);
            try
            {
                var comment = await _commentContext.Comments.Where(_ => _.Id == id).AsNoTracking().FirstOrDefaultAsync();
                if (comment is null)
                {
                    return Result<CommentView>.Fail<CommentView>($"Comment was not found");
                }
                return Result<CommentView>.Ok(_mapper.Map<CommentView>(comment));
            }
            catch (ArgumentNullException ex)
            {
                return Result<CommentView>.Fail<CommentView>($"Source is null. {ex.Message}");
            }
        }

        /// <summary>
        ///  Asynchronously get by userId. Id must be verified 
        /// </summary>
        /// <param name="userId">ID of user</param>
        public async Task<IEnumerable<CommentView>> GetByUserIdAsync(string userId)
        {
            Guid id = Guid.Parse(userId);
            var comment = await _commentContext.Comments.Where(_ => _.UserId == id).Select(_ => _).ToListAsync();
            return _mapper.Map<IEnumerable<CommentDB>, List<CommentView>>(comment);
        }

        /// <summary>
        ///  Asynchronously remove all comments 
        /// </summary>
        public async Task<Result> RemoveAllAsync()
        {
            var comment = await _commentContext.Comments.ToListAsync();
            if (comment is null)
            {
                return await Task.FromResult(Result.Fail("Comments were not found"));
            }
            try
            {
                _commentContext.Comments.RemoveRange(comment);
                await _commentContext.SaveChangesAsync();

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
        public async Task<Result> RemoveAllByUserIdAsync(string userId)
        {
            Guid id = Guid.Parse(userId);
            var comment = _commentContext.Comments.Where(_ => _.UserId == id).Select(_ => _);

            if (comment is null)
            {
                return await Task.FromResult(Result.Fail("Commenst were not found"));
            }
            try
            {
                _commentContext.Comments.RemoveRange(comment);
                await _commentContext.SaveChangesAsync();

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
        public async Task<Result> RemoveByIdAsync(string commentId)
        {
            Guid id = Guid.Parse(commentId);
            var comment = await _commentContext.Comments.IgnoreQueryFilters().FirstOrDefaultAsync(_ => _.Id == id);
            
            if (comment is null)
            {
                return await Task.FromResult(Result.Fail("Comment was not found"));
            }
            try
            {
                _commentContext.Comments.Remove(comment);
                await _commentContext.SaveChangesAsync();

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
        public async Task<Result<CommentToUpdate>> UpdateAsync(CommentToUpdate comment)
        {
            comment.ModificationTime = DateTime.Now;
            CommentDB commentForUpdate = _mapper.Map<CommentDB>(comment);
            _commentContext.Entry(commentForUpdate).Property(c => c.Headline).IsModified = true;
            _commentContext.Entry(commentForUpdate).Property(c => c.Rating).IsModified = true;
            _commentContext.Entry(commentForUpdate).Property(c => c.Content).IsModified = true;
            _commentContext.Entry(commentForUpdate).Property(c => c.ModificationTime).IsModified = true;

            try
            {
                await _commentContext.SaveChangesAsync();
                return Result<CommentToUpdate>.Ok(comment);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Result<CommentToUpdate>.Fail<CommentToUpdate>($"Cannot update model. {ex.Message}");
            }
            catch (DbUpdateException ex)
            {
                return Result<CommentToUpdate>.Fail<CommentToUpdate>($"Cannot update model. {ex.Message}");
            }
        }
    }
}
