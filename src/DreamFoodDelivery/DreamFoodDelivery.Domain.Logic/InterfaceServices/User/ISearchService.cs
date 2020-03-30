using DreamFoodDelivery.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DreamFoodDelivery.Domain.Logic.InterfaceServices
{
    public interface ISearchService
    {
        IEnumerable<Dish> Search(string search);
        Task<IEnumerable<Dish>> SearchAsync(string search); //Use it
        IEnumerable<Dish> GetIndexedCourses();
        IEnumerable<Dish> IndexCourse(Dish dish);
    }
}
