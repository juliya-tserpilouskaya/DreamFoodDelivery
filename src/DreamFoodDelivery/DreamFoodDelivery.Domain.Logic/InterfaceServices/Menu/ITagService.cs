using DreamFoodDelivery.Common.Helpers;
using DreamFoodDelivery.Domain.Models;
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
        /// <returns></returns>
        Task<IEnumerable<Tag>> GetAllAsync();

        /// <summary>
        /// Create tag
        /// </summary>
        /// <param name="tag">New tag</param>
        /// <returns></returns>
        Task<Result<Tag>> AddAsync(Tag tag);

        /// <summary>
        /// Updates tag
        /// </summary>
        /// <param name="tag">tag</param>
        /// <returns></returns>
        Task<Result<Tag>> UpdateAsync(Tag tag);

        /// <summary>
        /// Remove tag by id async
        /// </summary>
        /// <param name="id">tag id</param>
        /// <returns></returns>
        Task<Result> RemoveByIdAsync(string id);

        /// <summary>
        /// Removes all comments from database
        /// </summary>
        void RemoveAll();
    }
}
