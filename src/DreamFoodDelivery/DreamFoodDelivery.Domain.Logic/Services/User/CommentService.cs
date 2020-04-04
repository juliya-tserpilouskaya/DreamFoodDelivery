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

        public async Task<Result<CommentDTO_Add>> AddAsync(CommentDTO_Add comment)
        {
            var commentToAdd = _mapper.Map<CommentDB>(comment);

            _commentContext.Comments.Add(commentToAdd);

            try
            {
                await _commentContext.SaveChangesAsync();

                CommentDB commentAfterAdding = await _commentContext.Comments.Where(_ => _.UserId == commentToAdd.UserId).Select(_ => _).AsNoTracking().FirstOrDefaultAsync();

                return (Result<CommentDTO_Add>)Result<CommentDTO_Add>
                    .Ok(_mapper.Map<CommentDTO_Add>(commentAfterAdding));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Result<CommentDTO_Add>.Fail<CommentDTO_Add>($"Cannot save model. {ex.Message}");
            }
            catch (DbUpdateException ex)
            {
                return Result<CommentDTO_Add>.Fail<CommentDTO_Add>($"Cannot save model. {ex.Message}");
            }
            catch (ArgumentNullException ex)
            {
                return Result<CommentDTO_Add>.Fail<CommentDTO_Add>($"Source is null. {ex.Message}");
            }
        }

        /// <summary>
        /// Asynchronously returns all comments
        /// </summary>
        public async Task<IEnumerable<CommentDTO_View>> GetAllAsync()
        {
            var comment = await _commentContext.Comments.ToListAsync();
            return _mapper.Map<IEnumerable<CommentDB>, List<CommentDTO_View>>(comment);
        }

        /// <summary>
        /// Get by commentId. Id must be verified 
        /// </summary>
        /// <param name="commentId"></param>
        /// <returns></returns>
        public async Task<CommentDTO_View> GetByIdAsync(string commentId)
        {
            Guid id = Guid.Parse(commentId);
            var comment = await _commentContext.Comments.Where(_ => _.Id == id).FirstOrDefaultAsync();
            return _mapper.Map<CommentDTO_View>(comment);
        }

        /// <summary>
        /// Get by userId. Id must be verified 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<CommentDTO_View>> GetByUserIdAsync(string userId)
        {
            Guid id = Guid.Parse(userId);
            var comment = await _commentContext.Comments.Where(_ => _.UserId == id).Select(_ => _).ToListAsync();
            return _mapper.Map<IEnumerable<CommentDB>, List<CommentDTO_View>>(comment);
        }
        
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
        /// Delete comment by Id. Id must be verified
        /// </summary>
        /// <param name="commentId"></param>
        /// <returns></returns>
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
        /// Update comment in DataBase
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        public async Task<Result<CommentDTO_Update>> UpdateAsync(CommentDTO_Update comment)
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
                return Result<CommentDTO_Update>.Ok(comment);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Result<CommentDTO_Update>.Fail<CommentDTO_Update>($"Cannot update model. {ex.Message}");
            }
            catch (DbUpdateException ex)
            {
                return Result<CommentDTO_Update>.Fail<CommentDTO_Update>($"Cannot update model. {ex.Message}");
            }
        }
    }
}
