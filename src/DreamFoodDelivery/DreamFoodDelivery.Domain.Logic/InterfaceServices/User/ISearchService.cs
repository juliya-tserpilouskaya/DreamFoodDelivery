using DreamFoodDelivery.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DreamFoodDelivery.Domain.Logic.InterfaceServices
{
    public interface ISearchService
    {
        IEnumerable<DishDTO> Search(string search);
        Task<IEnumerable<DishDTO>> SearchAsync(string search); //Use it
        IEnumerable<DishDTO> GetIndexedCourses();
        IEnumerable<DishDTO> IndexCourse(DishDTO dish);
    }
}
