using DreamFoodDelivery.Common;
using DreamFoodDelivery.Data.Models;
using DreamFoodDelivery.Domain.DTO;
using DreamFoodDelivery.Domain.View;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DreamFoodDelivery.Domain.Logic.InterfaceServices
{
    public interface ITagService
    {
        /// <summary>
        ///  Asynchronously add new tag
        /// </summary>
        /// <param name="tag">New tag to add</param>
        Task<Result<TagView>> AddAsync(TagToAdd tag, CancellationToken cancellationToken = default);

        /// <summary>
        ///  Asynchronously add new tag 
        /// </summary>
        /// <param name="tag">New tag to add</param>
        /// <returns>TagDB</returns>
        Task<Result<TagDB>> AddTagDBAsync(TagToAdd tag, CancellationToken cancellationToken = default);

        /// <summary>
        ///  Asynchronously update tag
        /// </summary>
        /// <param name="tag">Existing tag to update</param>
        Task<Result<TagToUpdate>> UpdateAsync(TagToUpdate tag, CancellationToken cancellationToken = default);

        /// <summary>
        ///  Asynchronously remove tag by Id. Id must be verified
        /// </summary>
        /// <param name="tagId">ID of existing tag</param>
        Task<Result> RemoveByIdAsync(string tagId, CancellationToken cancellationToken = default);

        /// <summary>
        ///  Asynchronously remove all tags 
        /// </summary>
        Task<Result> RemoveAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        ///  Asynchronously get tag by tag Id. Id must be verified 
        /// </summary>
        /// <param name="tagId">ID of existing tag</param>
        Task<Result<TagView>> GetByIdAsync(string tagId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get all tags from DB
        /// </summary>
        /// <returns></returns>
        Task<Result<IEnumerable<TagView>>> GetAllTagsAsync(CancellationToken cancellationToken);
    }
}
