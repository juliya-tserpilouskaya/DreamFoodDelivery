//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading.Tasks;

//namespace DreamFoodDelivery.Domain.Logic.InterfaceServices
//{
//    //------------------------------------------------------------------
//    // Created using template, $time$
//    //------------------------------------------------------------------
//    public interface IServiceInterfaceTempale
//    {
//        /// <summary>
//        /// Asynchronously returns all things
//        /// </summary>
//        Task<Result<IEnumerable<ThingView>>> GetAllAsync();

//        /// <summary>
//        ///  Asynchronously get thing by thing Id. Id must be verified 
//        /// </summary>
//        /// <param name="thingId">ID of existing thing</param>
//        Task<Result<ThingView>> GetByIdAsync(string thingId);

//        /// <summary>
//        ///  Asynchronously get by userId. Id must be verified 
//        /// </summary>
//        /// <param name="userId">ID of user</param>
//        Task<IEnumerable<ThingView>> GetByUserIdAsync(string userID);

//        /// <summary>
//        ///  Asynchronously add new thing
//        /// </summary>
//        /// <param name="thing">New thing to add</param>
//        Task<Result<ThingToAdd>> AddAsync(ThingToAdd thing);

//        /// <summary>
//        ///  Asynchronously update thing
//        /// </summary>
//        /// <param name="thing">Existing thing to update</param>
//        Task<Result<ThingToUpdate>> UpdateAsync(ThingToUpdate thing);

//        /// <summary>
//        ///  Asynchronously remove thing by Id. Id must be verified
//        /// </summary>
//        /// <param name="thingId">ID of existing thing</param>
//        Task<Result> RemoveByIdAsync(string thingId);

//        /// <summary>
//        ///  Asynchronously remove all things by user Id. Id must be verified 
//        /// </summary>
//        /// <param name="userId">ID of user</param>
//        Task<Result> RemoveAllByUserIdAsync(string userId);

//        /// <summary>
//        ///  Asynchronously remove all things 
//        /// </summary>
//        Task<Result> RemoveAllAsync();
//    }
//}
