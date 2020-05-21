using DreamFoodDelivery.Common;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DreamFoodDelivery.Domain.Logic.InterfaceServices
{
    public interface IImageInterface
    {
        /// <summary>
        /// Check directory and get image
        /// </summary>
        /// <param name="imageName"></param>
        /// <param name="dishId"></param>
        /// <returns></returns>
        Result<string> GetImage(string imageName, string dishId);

        /// <summary>
        /// Check directory and get FileInfo[]
        /// </summary>
        /// <param name="dishId"></param>
        /// <returns></returns>
        Result<ICollection<string>> GetImagesInfo(string dishId);

        /// <summary>
        /// Remove file from directory by directory and file name
        /// </summary>
        /// <param name="imageName"></param>
        /// <param name="dishId"></param>
        /// <returns></returns>
        Result DeleteImageByName(string imageName, string dishId);

        /// <summary>
        /// Method to check/create directory and write file onto the disk
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        Task<Result<string>> UploadImageAsync(IFormFile file, string dishId);
    }
}
