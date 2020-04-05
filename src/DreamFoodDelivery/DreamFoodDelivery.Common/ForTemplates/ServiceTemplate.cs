//using AutoMapper;
//using DreamFoodDelivery.Domain.Logic.InterfaceServices;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace DreamFoodDelivery.Domain.Logic.Services
//{
//    //------------------------------------------------------------------
//    // Created using template, $time$
//    //------------------------------------------------------------------
//    public class ServiceTemplate : IServiceInterfaceTempale
//    {
//        private readonly DreamFoodDeliveryContext _context;
//        private readonly IMapper _mapper;

//        public ServiceTemplate(IMapper mapper, DreamFoodDeliveryContext context)
//        {
//            _context = context;
//            _mapper = mapper;
//        }

//        /// <summary>
//        ///  Asynchronously add new thing
//        /// </summary>
//        /// <param name="thing">New thing to add</param>
//        public async Task<Result<ThingToAdd>> AddAsync(ThingToAdd thing)
//        {
//            var thingToAdd = _mapper.Map<ThingDB>(thing);

//            _context.Things.Add(thingToAdd);

//            try
//            {
//                await _context.SaveChangesAsync();

//                ThingDB thingAfterAdding = await _context.Things.Where(_ => _.UserId == thingToAdd.UserId).Select(_ => _).AsNoTracking().FirstOrDefaultAsync();

//                return (Result<ThingToAdd>)Result<ThingToAdd>
//                    .Ok(_mapper.Map<ThingToAdd>(thingAfterAdding));
//            }
//            catch (DbUpdateConcurrencyException ex)
//            {
//                return Result<ThingToAdd>.Fail<ThingToAdd>($"Cannot save model. {ex.Message}");
//            }
//            catch (DbUpdateException ex)
//            {
//                return Result<ThingToAdd>.Fail<ThingToAdd>($"Cannot save model. {ex.Message}");
//            }
//            catch (ArgumentNullException ex)
//            {
//                return Result<ThingToAdd>.Fail<ThingToAdd>($"Source is null. {ex.Message}");
//            }
//        }

//        /// <summary>
//        /// Asynchronously returns all things
//        /// </summary>
//        public async Task<Result<IEnumerable<ThingView>>> GetAllAsync()
//        {
//            var things = await _context.Things.AsNoTracking().ToListAsync();
//            if (!things.Any())
//            {
//                return Result<IEnumerable<ThingView>>.Fail<IEnumerable<ThingView>>("No Users found");
//            }
//            return Result<IEnumerable<ThingView>>.Ok(_mapper.Map<IEnumerable<ThingView>>(things));
//        }

//        /// <summary>
//        ///  Asynchronously get thing by thing Id. Id must be verified 
//        /// </summary>
//        /// <param name="thingId">ID of existing thing</param>
//        public async Task<Result<ThingView>> GetByIdAsync(string thingId)
//        {
//            Guid id = Guid.Parse(thingId);
//            try
//            {
//                var thing = await _context.Things.Where(_ => _.Id == id).AsNoTracking().FirstOrDefaultAsync();
//                if (thing is null)
//                {
//                    return Result<ThingView>.Fail<ThingView>($"Thing was not found");
//                }
//                return Result<ThingView>.Ok(_mapper.Map<ThingView>(thing));
//            }
//            catch (ArgumentNullException ex)
//            {
//                return Result<ThingView>.Fail<ThingView>($"Source is null. {ex.Message}");
//            }
//        }

//        /// <summary>
//        ///  Asynchronously get by userId. Id must be verified 
//        /// </summary>
//        /// <param name="userId">ID of user</param>
//        public async Task<IEnumerable<ThingView>> GetByUserIdAsync(string userId)
//        {
//            Guid id = Guid.Parse(userId);
//            var thing = await _context.Things.Where(_ => _.UserId == id).Select(_ => _).ToListAsync();
//            return _mapper.Map<IEnumerable<ThingDB>, List<ThingView>>(thing);
//        }

//        /// <summary>
//        ///  Asynchronously remove all things 
//        /// </summary>
//        public async Task<Result> RemoveAllAsync()
//        {
//            var thing = await _context.Things.ToListAsync();
//            if (thing is null)
//            {
//                return await Task.FromResult(Result.Fail("Things were not found"));
//            }
//            try
//            {
//                _context.Things.RemoveRange(thing);
//                await _context.SaveChangesAsync();

//                return await Task.FromResult(Result.Ok());
//            }
//            catch (DbUpdateConcurrencyException ex)
//            {
//                return await Task.FromResult(Result.Fail($"Cannot delete things. {ex.Message}"));
//            }
//            catch (DbUpdateException ex)
//            {
//                return await Task.FromResult(Result.Fail($"Cannot delete things. {ex.Message}"));
//            }
//        }

//        /// <summary>
//        ///  Asynchronously remove all things by user Id. Id must be verified 
//        /// </summary>
//        /// <param name="userId">ID of user</param>
//        public async Task<Result> RemoveAllByUserIdAsync(string userId)
//        {
//            Guid id = Guid.Parse(userId);
//            var thing = _context.Things.Where(_ => _.UserId == id).Select(_ => _);

//            if (thing is null)
//            {
//                return await Task.FromResult(Result.Fail("Commenst were not found"));
//            }
//            try
//            {
//                _context.Things.RemoveRange(thing);
//                await _context.SaveChangesAsync();

//                return await Task.FromResult(Result.Ok());
//            }
//            catch (DbUpdateConcurrencyException ex)
//            {
//                return await Task.FromResult(Result.Fail($"Cannot delete things. {ex.Message}"));
//            }
//            catch (DbUpdateException ex)
//            {
//                return await Task.FromResult(Result.Fail($"Cannot delete things. {ex.Message}"));
//            }
//        }

//        /// <summary>
//        ///  Asynchronously remove thing by Id. Id must be verified
//        /// </summary>
//        /// <param name="thingId">ID of existing thing</param>
//        public async Task<Result> RemoveByIdAsync(string thingId)
//        {
//            Guid id = Guid.Parse(thingId);
//            var thing = await _context.Things.IgnoreQueryFilters().FirstOrDefaultAsync(_ => _.Id == id);

//            if (thing is null)
//            {
//                return await Task.FromResult(Result.Fail("Thing was not found"));
//            }
//            try
//            {
//                _context.Things.Remove(thing);
//                await _context.SaveChangesAsync();

//                return await Task.FromResult(Result.Ok());
//            }
//            catch (DbUpdateConcurrencyException ex)
//            {
//                return await Task.FromResult(Result.Fail($"Cannot delete thing. {ex.Message}"));
//            }
//            catch (DbUpdateException ex)
//            {
//                return await Task.FromResult(Result.Fail($"Cannot delete thing. {ex.Message}"));
//            }
//        }

//        /// <summary>
//        ///  Asynchronously update thing
//        /// </summary>
//        /// <param name="thing">Existing thing to update</param>
//        public async Task<Result<ThingToUpdate>> UpdateAsync(ThingToUpdate thing)
//        {
//            thing.ModificationTime = DateTime.Now;
//            ThingDB thingForUpdate = _mapper.Map<ThingDB>(thing);
//            _context.Entry(thingForUpdate).Property(c => c.Headline).IsModified = true;
//            _context.Entry(thingForUpdate).Property(c => c.Rating).IsModified = true;
//            _context.Entry(thingForUpdate).Property(c => c.Content).IsModified = true;
//            _context.Entry(thingForUpdate).Property(c => c.ModificationTime).IsModified = true;

//            try
//            {
//                await _context.SaveChangesAsync();
//                return Result<ThingToUpdate>.Ok(thing);
//            }
//            catch (DbUpdateConcurrencyException ex)
//            {
//                return Result<ThingToUpdate>.Fail<ThingToUpdate>($"Cannot update model. {ex.Message}");
//            }
//            catch (DbUpdateException ex)
//            {
//                return Result<ThingToUpdate>.Fail<ThingToUpdate>($"Cannot update model. {ex.Message}");
//            }
//        }
//    }
//}
