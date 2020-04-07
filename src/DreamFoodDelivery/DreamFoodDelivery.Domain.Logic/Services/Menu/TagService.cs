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
    //------------------------------------------------------------------
    // Created using template, 4/5/2020 9:45:50 PM
    //------------------------------------------------------------------
    public class TagService : ITagService
    {
        private readonly DreamFoodDeliveryContext _context;
        private readonly IMapper _mapper;

        public TagService(IMapper mapper, DreamFoodDeliveryContext context)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        ///  Asynchronously add new tag
        /// </summary>
        /// <param name="tag">New tag to add</param>
        public async Task<Result<TagDTO>> AddAsync(TagDTO tag)
        {
            var tagToAdd = _mapper.Map<TagDB>(tag);
            _context.Tags.Add(tagToAdd);

            try
            {
                await _context.SaveChangesAsync();
                TagDB tagAfterAdding = await _context.Tags.Where(_ => _.IndexNumber == tagToAdd.IndexNumber).Select(_ => _).AsNoTracking().FirstOrDefaultAsync();
                return Result<TagDTO>.Ok(_mapper.Map<TagDTO>(tagAfterAdding));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Result<TagDTO>.Fail<TagDTO>($"Cannot save model. {ex.Message}");
            }
            catch (DbUpdateException ex)
            {
                return Result<TagDTO>.Fail<TagDTO>($"Cannot save model. {ex.Message}");
            }
            catch (ArgumentNullException ex)
            {
                return Result<TagDTO>.Fail<TagDTO>($"Source is null. {ex.Message}");
            }
        }

        /// <summary>
        /// Asynchronously returns all tags
        /// </summary>
        public async Task<Result<IEnumerable<TagDTO>>> GetAllAsync()
        {
            var tags = await _context.Tags.AsNoTracking().ToListAsync();
            if (!tags.Any())
            {
                return Result<IEnumerable<TagDTO>>.Fail<IEnumerable<TagDTO>>("No tags found");
            }
            return Result<IEnumerable<TagDTO>>.Ok(_mapper.Map<IEnumerable<TagDTO>>(tags));
        }

        /// <summary>
        ///  Asynchronously get tag by tag Id. Id must be verified 
        /// </summary>
        /// <param name="tagId">ID of existing tag</param>
        public async Task<Result<TagDTO>> GetByIdAsync(string tagId)
        {
            Guid id = Guid.Parse(tagId);
            try
            {
                var tag = await _context.Tags.Where(_ => _.Id == id).AsNoTracking().FirstOrDefaultAsync();
                if (tag is null)
                {
                    return Result<TagDTO>.Fail<TagDTO>($"Tag was not found");
                }
                return Result<TagDTO>.Ok(_mapper.Map<TagDTO>(tag));
            }
            catch (ArgumentNullException ex)
            {
                return Result<TagDTO>.Fail<TagDTO>($"Source is null. {ex.Message}");
            }
        }

        /// <summary>
        ///  Asynchronously remove all tags 
        /// </summary>
        public async Task<Result> RemoveAllAsync()
        {
            var tag = await _context.Tags.ToListAsync();
            if (tag is null)
            {
                return await Task.FromResult(Result.Fail("Tags were not found"));
            }
            try
            {
                _context.Tags.RemoveRange(tag);
                await _context.SaveChangesAsync();

                return await Task.FromResult(Result.Ok());
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return await Task.FromResult(Result.Fail($"Cannot delete tags. {ex.Message}"));
            }
            catch (DbUpdateException ex)
            {
                return await Task.FromResult(Result.Fail($"Cannot delete tags. {ex.Message}"));
            }
        }

        /// <summary>
        ///  Asynchronously remove tag by Id. Id must be verified
        /// </summary>
        /// <param name="tagId">ID of existing tag</param>
        public async Task<Result> RemoveByIdAsync(string tagId)
        {
            Guid id = Guid.Parse(tagId);
            var tag = await _context.Tags.IgnoreQueryFilters().FirstOrDefaultAsync(_ => _.Id == id);
            if (tag is null)
            {
                return await Task.FromResult(Result.Fail("Tag was not found"));
            }
            try
            {
                _context.Tags.Remove(tag);
                await _context.SaveChangesAsync();
                return await Task.FromResult(Result.Ok());
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return await Task.FromResult(Result.Fail($"Cannot delete tag. {ex.Message}"));
            }
            catch (DbUpdateException ex)
            {
                return await Task.FromResult(Result.Fail($"Cannot delete tag. {ex.Message}"));
            }
        }

        /// <summary>
        ///  Asynchronously update tag
        /// </summary>
        /// <param name="tag">Existing tag to update</param>
        public async Task<Result<TagDTO>> UpdateAsync(TagDTO tag)
        {
            TagDB tagForUpdate = _mapper.Map<TagDB>(tag);
            _context.Entry(tagForUpdate).Property(c => c.IndexNumber).IsModified = true;

            try
            {
                await _context.SaveChangesAsync();
                return Result<TagDTO>.Ok(tag);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Result<TagDTO>.Fail<TagDTO>($"Cannot update model. {ex.Message}");
            }
            catch (DbUpdateException ex)
            {
                return Result<TagDTO>.Fail<TagDTO>($"Cannot update model. {ex.Message}");
            }
        }
    }
}
