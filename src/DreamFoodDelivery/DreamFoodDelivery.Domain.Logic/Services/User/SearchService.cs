using DreamFoodDelivery.Domain.Logic.InterfaceServices;
using DreamFoodDelivery.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DreamFoodDelivery.Domain.Logic.Services
{
    public class SearchService : ISearchService
    {
        public IEnumerable<Dish> GetIndexedCourses()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Dish> IndexCourse(Dish dish)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Dish> Search(string search)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Dish>> SearchAsync(string search)
        {
            throw new NotImplementedException();
        }
    }
}
