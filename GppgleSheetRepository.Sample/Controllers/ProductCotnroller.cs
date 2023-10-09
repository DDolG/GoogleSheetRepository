using GoogleSheetRepository.Interfaces;
using GppgleSheetRepository.Sample.Models;
using Microsoft.AspNetCore.Mvc;

namespace GppgleSheetRepository.Sample.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductCotnroller : ControllerBase
    {
        private readonly IRepository<Product> _repository;

        public ProductCotnroller(IRepository<Product> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Product>> Get()
        {
            var products = _repository.Get();
            return Ok(products);
        }

        [HttpPost]
        public ActionResult<Product> Post([FromBody] Product item)
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
        public ActionResult Update(int id, [FromBody] Product updatedItem)
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
