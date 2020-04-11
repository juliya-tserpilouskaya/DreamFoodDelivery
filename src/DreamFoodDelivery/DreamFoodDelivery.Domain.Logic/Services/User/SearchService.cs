using DreamFoodDelivery.Domain.Logic.InterfaceServices;
using DreamFoodDelivery.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DreamFoodDelivery.Domain.Logic.Services
{
    public class SearchService : ISearchService
    {
        public IEnumerable<DishDTO> GetIndexedCourses()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DishDTO> IndexCourse(DishDTO dish)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DishDTO> Search(string search)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DishDTO>> SearchAsync(string search)
        {
            throw new NotImplementedException();
        }
    }
}
