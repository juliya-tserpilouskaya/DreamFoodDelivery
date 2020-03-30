using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DreamFoodDelivery.Domain.Logic.InterfaceServices;
using DreamFoodDelivery.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DreamFoodDelivery.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly ISearchService _searchService;

        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }

        /// <summary>
        /// Get indexed dishes
        /// </summary>
        /// <returns>Indexed dishes</returns>
        [HttpGet, Route("")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Indexed dishes not found")]
        [SwaggerResponse(StatusCodes.Status200OK, "Indexed dishes are found", typeof(IEnumerable<Dish>))]
        public IActionResult GetAllIndexed()
        {
            var result = _searchService.GetIndexedCourses();
            return result == null ? NotFound() : (IActionResult)Ok(result);
        }

        /// <summary>
        /// Add dish to search index
        /// </summary>
        /// <param name="dish">dish</param>
        /// <returns>Indexed dishes</returns>
        [HttpPost, Route("index")]
        [SwaggerResponse(StatusCodes.Status200OK, "Course successfully indexed", typeof(IEnumerable<Dish>))]
        public IActionResult IndexCourse([FromBody]Dish dish)
        {
            var result = _searchService.IndexCourse(dish);
            return result == null ? NotFound() : (IActionResult)Ok(result);
        }

        /// <summary>
        /// Get dishes by search query
        /// </summary>
        /// <param name="query">Search query</param>
        /// <returns>Finded dishes</returns>
        [HttpGet, Route("{query}")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Courses not found")]
        [SwaggerResponse(StatusCodes.Status200OK, "Courses are found", typeof(IEnumerable<Dish>))]
        public IActionResult SearchByString(string query)
        {
            var result = _searchService.Search(query);
            return result == null ? NotFound() : (IActionResult)Ok(result);
        }
    }
}