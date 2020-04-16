using DreamFoodDelivery.Common.Helpers;
using DreamFoodDelivery.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DreamFoodDelivery.Domain.Logic.InterfaceServices
{
    public interface ITagService
    {
        /// <summary>
        /// Asynchronously returns all tags
        /// </summary>
        Task<Result<IEnumerable<TagView>>> GetAllAsync();

        /// <summary>
        ///  Asynchronously add new tag
        /// </summary>
        /// <param name="tag">New tag to add</param>
        Task<Result<TagView>> AddAsync(TagToAdd tag);

        /// <summary>
        ///  Asynchronously update tag
        /// </summary>
        /// <param name="tag">Existing tag to update</param>
        Task<Result<TagToUpdate>> UpdateAsync(TagToUpdate tag);

        /// <summary>
        ///  Asynchronously remove tag by Id. Id must be verified
        /// </summary>
        /// <param name="tagId">ID of existing tag</param>
        Task<Result> RemoveByIdAsync(string tagId);

        /// <summary>
        ///  Asynchronously remove all tags 
        /// </summary>
        Task<Result> RemoveAllAsync();

        /// <summary>
        ///  Asynchronously get tag by tag Id. Id must be verified 
        /// </summary>
        /// <param name="tagId">ID of existing tag</param>
        Task<Result<TagView>> GetByIdAsync(string tagId);
    }
}
