﻿using AutoMapper;
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
        [LoggerAttribute]
        public async Task<Result<TagView>> AddAsync(TagToAdd tag, CancellationToken cancellationToken = default)
        {
            var tagToAdd = _mapper.Map<TagDB>(tag);
            _context.Tags.Add(tagToAdd);

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
                //TagDB tagAfterAdding = await _context.Tags.Where(_ => _.IndexNumber == tagToAdd.IndexNumber).Select(_ => _).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
                TagDB tagAfterAdding = await _context.Tags.Where(_ => _.TagName == tagToAdd.TagName).Select(_ => _).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
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
        ///  Asynchronously add new tag 
        /// </summary>
        /// <param name="tag">New tag to add</param>
        /// <returns>TagDB</returns>
        [LoggerAttribute]
        public async Task<Result<TagDB>> AddTagDBAsync(TagToAdd tag, CancellationToken cancellationToken = default)
        {
            var tagToAdd = _mapper.Map<TagDB>(tag);
            _context.Tags.Add(tagToAdd);

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
                //TagDB tagAfterAdding = await _context.Tags.Where(_ => _.IndexNumber == tagToAdd.IndexNumber).Select(_ => _).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
                TagDB tagAfterAdding = await _context.Tags.Where(_ => _.TagName == tagToAdd.TagName).Select(_ => _).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
                return Result<TagDB>.Ok(tagAfterAdding);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Result<TagDB>.Fail<TagDB>($"Cannot save model. {ex.Message}");
            }
            catch (DbUpdateException ex)
            {
                return Result<TagDB>.Fail<TagDB>($"Cannot save model. {ex.Message}");
            }
            catch (ArgumentNullException ex)
            {
                return Result<TagDB>.Fail<TagDB>($"Source is null. {ex.Message}");
            }
        }

        /// <summary>
        /// Asynchronously returns all tags
        /// </summary>
        [LoggerAttribute]
        public async Task<Result<IEnumerable<TagView>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var tags = await _context.Tags.AsNoTracking().ToListAsync(cancellationToken);
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
        [LoggerAttribute]
        public async Task<Result<TagView>> GetByIdAsync(string tagId, CancellationToken cancellationToken = default)
        {
            Guid id = Guid.Parse(tagId);
            try
            {
                var tag = await _context.Tags.Where(_ => _.Id == id).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
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
        [LoggerAttribute]
        public async Task<Result> RemoveAllAsync(CancellationToken cancellationToken = default)
        {
            var tag = await _context.Tags.ToListAsync(cancellationToken);
            if (tag is null)
            {
                return await Task.FromResult(Result.Fail("Tags were not found"));
            }
            try
            {
                _context.Tags.RemoveRange(tag);
                await _context.SaveChangesAsync(cancellationToken);

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
        [LoggerAttribute]
        public async Task<Result> RemoveByIdAsync(string tagId, CancellationToken cancellationToken = default)
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
                await _context.SaveChangesAsync(cancellationToken);
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
        [LoggerAttribute]
        public async Task<Result<TagToUpdate>> UpdateAsync(TagToUpdate tag, CancellationToken cancellationToken = default)
        {
            TagDB tagForUpdate = _mapper.Map<TagDB>(tag);
            tagForUpdate.Id = Guid.Parse(tag.Id);
            //_context.Entry(tagForUpdate).Property(c => c.IndexNumber).IsModified = true;
            _context.Entry(tagForUpdate).Property(c => c.TagName).IsModified = true;

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
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
