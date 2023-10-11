using GoogleSheetRepository.Interfaces;
using GppgleSheetRepository.Sample.Models;
using Microsoft.AspNetCore.Mvc;

namespace GppgleSheetRepository.Sample.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class CategoryCotnroller : ControllerBase
    {
        private readonly IRepository<Category> _repository;

        public CategoryCotnroller(IRepository<Category> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Category>> Get()
        {
            var categories = _repository.Get();
            return Ok(categories);
        }

        [HttpPost]
        public ActionResult<Category> Post([FromBody] Category item)
        {
            _repository.Add(item);
            return CreatedAtAction(nameof(Get), new { id = item.Id }, item);
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var items = _repository.Get();
            var item = items.FirstOrDefault(x=>x.Id == id);

            if (item == null)
            {
                return NotFound();
            }

            _repository.Delete(item);
            return NoContent();
        }

        [HttpPut("{id}")]
        public ActionResult Update(int id, [FromBody] Category updatedItem)
        {
            var items = _repository.Get();
            var existingItem = items.FirstOrDefault(x => x.Id == id);
            
            if (existingItem == null)
            {
                return NotFound();
            }

            _repository.Update(existingItem, updatedItem);
            
            return Ok(existingItem);
        }
    }
}
