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
        public async Task<Result<TagView>> AddAsync(TagToAdd tag)
        {
            var tagToAdd = _mapper.Map<TagDB>(tag);
            _context.Tags.Add(tagToAdd);

            try
            {
                await _context.SaveChangesAsync();
                TagDB tagAfterAdding = await _context.Tags.Where(_ => _.IndexNumber == tagToAdd.IndexNumber).Select(_ => _).AsNoTracking().FirstOrDefaultAsync();
                return Result<TagView>.Ok(_mapper.Map<TagView>(tagAfterAdding));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Result<TagView>.Fail<TagView>($"Cannot save model. {ex.Message}");
            }
            catch (DbUpdateException ex)
            {
                return Result<TagView>.Fail<TagView>($"Cannot save model. {ex.Message}");
            }
            catch (ArgumentNullException ex)
            {
                return Result<TagView>.Fail<TagView>($"Source is null. {ex.Message}");
            }
        }

        /// <summary>
        /// Asynchronously returns all tags
        /// </summary>
        public async Task<Result<IEnumerable<TagView>>> GetAllAsync()
        {
            var tags = await _context.Tags.AsNoTracking().ToListAsync();
            if (!tags.Any())
            {
                return Result<IEnumerable<TagView>>.Fail<IEnumerable<TagView>>("No tags found");
            }
            return Result<IEnumerable<TagView>>.Ok(_mapper.Map<IEnumerable<TagView>>(tags));
        }

        /// <summary>
        ///  Asynchronously get tag by tag Id. Id must be verified 
        /// </summary>
        /// <param name="tagId">ID of existing tag</param>
        public async Task<Result<TagView>> GetByIdAsync(string tagId)
        {
            Guid id = Guid.Parse(tagId);
            try
            {
                var tag = await _context.Tags.Where(_ => _.Id == id).AsNoTracking().FirstOrDefaultAsync();
                if (tag is null)
                {
                    return Result<TagView>.Fail<TagView>($"Tag was not found");
                }
                return Result<TagView>.Ok(_mapper.Map<TagView>(tag));
            }
            catch (ArgumentNullException ex)
            {
                return Result<TagView>.Fail<TagView>($"Source is null. {ex.Message}");
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
        public async Task<Result<TagToUpdate>> UpdateAsync(TagToUpdate tag)
        {
            TagDB tagForUpdate = _mapper.Map<TagDB>(tag);
            _context.Entry(tagForUpdate).Property(c => c.IndexNumber).IsModified = true;

            try
            {
                await _context.SaveChangesAsync();
                return Result<TagToUpdate>.Ok(tag);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Result<TagToUpdate>.Fail<TagToUpdate>($"Cannot update model. {ex.Message}");
            }
            catch (DbUpdateException ex)
            {
                return Result<TagToUpdate>.Fail<TagToUpdate>($"Cannot update model. {ex.Message}");
            }
        }
    }
}
