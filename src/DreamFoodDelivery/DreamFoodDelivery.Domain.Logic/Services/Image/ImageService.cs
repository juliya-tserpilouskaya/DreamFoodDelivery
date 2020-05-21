using DreamFoodDelivery.Common;
using DreamFoodDelivery.Common.Helpers;
using DreamFoodDelivery.Data.Context;
using DreamFoodDelivery.Domain.Logic.InterfaceServices;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DreamFoodDelivery.Domain.Logic.Services
{
    public class ImageService : IImageInterface
    {
        private readonly DreamFoodDeliveryContext _context;

        public ImageService(DreamFoodDeliveryContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Remove file from directory by directory and file name
        /// </summary>
        /// <param name="imageName"></param>
        /// <param name="dishId"></param>
        /// <returns></returns>
        public Result DeleteImageByName(string imageName, string dishId)
        {
            if (_context.Dishes.Where(_ => _.Id.ToString().Equals(dishId)) is null)
            {
                return Result<string>.Warning<string>(null, "Dish is not exists or you are not owner");
            }

            var path = Path.Combine(Directory.GetCurrentDirectory(), $"Images\\DishImages\\{dishId}");

            var fullPath = Path.Combine(path, imageName);

            var checkDirectory = IsExists(path, fullPath);
            if (!checkDirectory.IsSuccess)
            {
                if (checkDirectory.IsError)
                {
                    return Result.Fail(checkDirectory.Message);
                }

                return Result.Warning(checkDirectory.Message);
            }

            File.Delete(fullPath);

            return Result.Ok();
        }

        /// <summary>
        /// Check directory and get image
        /// </summary>
        /// <param name="imageName"></param>
        /// <param name="dishId"></param>
        /// <returns></returns>
        public Result<string> GetImage(string imageName, string dishId)
        {
            var directiryPath = $"Images\\DishImages\\{dishId}";
            var fullDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), directiryPath);

            var path = Path.Combine(directiryPath, imageName);

            var checkDirectory = IsExists(fullDirectoryPath, imageName);
            if (!checkDirectory.IsSuccess)
            {
                if (checkDirectory.IsError)
                {
                    return Result<string>.Fail<string>(checkDirectory.Message);
                }

                return Result<string>.Warning<string>(null, checkDirectory.Message);
            }

            return Result<string>.Ok<string>(path);
        }

        /// <summary>
        /// Check directory and get FileInfo[]
        /// </summary>
        /// <param name="dishId"></param>
        /// <returns></returns>
        public Result<ICollection<string>> GetImagesInfo(string dishId)
        {
            try
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), $"Images\\DishImages\\{dishId}");

                DirectoryInfo dirInfo = new DirectoryInfo(path);
                if (!dirInfo.Exists)
                {
                    return Result<ICollection<string>>.Warning<ICollection<string>>(null, "This dish doesn't have images!");
                }

                var info = dirInfo.GetFiles();

                List<string> result = new List<string>();

                foreach (var item in info)
                {
                    result.Add(item.Name);
                }

                return Result<ICollection<string>>.Ok<ICollection<string>>(result);
            }
            catch (Exception ex)
            {
                return Result<ICollection<string>>.Fail<ICollection<string>>(ex.Message);
            }
        }

        /// <summary>
        /// Method to check/create directory and write file onto the disk
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task<Result<string>> UploadImageAsync(IFormFile file, string dishId)
        {
            if (CheckIfImageFile(file))
            {
                return await WriteFileAsync(file, dishId);
            }

            return Result<string>.Warning<string>(null, "Invalid file");
        }

        /// <summary>
        /// Method to check if file is image file
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private bool CheckIfImageFile(IFormFile file)
        {
            byte[] fileBytes;
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                fileBytes = ms.ToArray();
            }

            return ImageServiceHelper.GetImageFormat(fileBytes) != ImageServiceHelper.ImageFormat.unknown;
        }

        /// <summary>
        /// Method to check/create directory and write file onto the disk
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task<Result<string>> WriteFileAsync(IFormFile file, string dishId)
        {
            if (_context.Dishes.Where(_ => _.Id.ToString().Equals(dishId)).FirstOrDefault() is null)
            {
                return Result<string>.Warning<string>(null, "Dish does not exist");
            }

            try
            {
                var extension = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];

                var fileName = Guid.NewGuid().ToString() + extension;
                var directiryPath = $"Images\\DishImages\\{dishId}";
                var fullDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), directiryPath);

                DirectoryInfo dirInfo = new DirectoryInfo(fullDirectoryPath);
                if (!dirInfo.Exists)
                {
                    dirInfo.Create();
                }

                var fullPath = Path.Combine(fullDirectoryPath, fileName);

                using (var bits = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(bits);
                }

                var pathForRote = Path.Combine(directiryPath, fileName);

                return Result<string>.Ok<string>(pathForRote);
            }
            catch (Exception e) //catch all
            {
                return Result<string>.Fail<string>(e.Message);
            }
        }

        private Result IsExists(string directory, string imageName)
        {
            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(directory);
                if (!dirInfo.Exists)
                {
                    return Result.Warning("This dish doesn't have images!");
                }

                var fullPath = Path.Combine(directory, imageName);

                FileInfo fileInfo = new FileInfo(fullPath);
                if (!fileInfo.Exists)
                {
                    return Result.Warning("Image is not exists!");
                }

                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }
    }
}
