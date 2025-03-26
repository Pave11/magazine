using Microsoft.AspNetCore.Mvc;
using Magazine.Core.Models;
using Magazine.Core.Services;
using System;

namespace Magazine.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // Метод для добавления нового продукта
        [HttpPost]
        public ActionResult<Product> Add([FromBody] Product product)
        {
            if (product == null)
            {
                return BadRequest("Продукт не может быть null.");
            }

            try
            {
                var createdProduct = _productService.Add(product);
                return CreatedAtAction(nameof(GetById), new { id = createdProduct.Id }, createdProduct);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Ошибка при добавлении продукта: " + ex.Message);
            }
        }

        // Метод для удаления продукта
        [HttpDelete("{id}")]
        public ActionResult<Product> Remove(Guid id)
        {
            try
            {
                var removedProduct = _productService.Remove(id);
                if (removedProduct == null)
                {
                    return NotFound($"Продукт с ID {id} не найден.");
                }
                return Ok(removedProduct);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Ошибка при удалении продукта: " + ex.Message);
            }
        }

        // Метод для редактирования продукта
        [HttpPut]
        public ActionResult<Product> Edit([FromBody] Product product)
        {
            if (product == null)
            {
                return BadRequest("Продукт не может быть null.");
            }

            try
            {
                var updatedProduct = _productService.Edit(product);
                if (updatedProduct == null)
                {
                    return NotFound($"Продукт с ID {product.Id} не найден.");
                }
                return Ok(updatedProduct);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Ошибка при редактировании продукта: " + ex.Message);
            }
        }

        // Метод для получения продукта по ID
        [HttpGet("{id}")]
        public ActionResult<Product> GetById(Guid id)
        {
            try
            {
                var product = _productService.GetById(id);
                if (product == null)
                {
                    return NotFound($"Продукт с ID {id} не найден.");
                }
                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Ошибка при получении продукта: " + ex.Message);
            }
        }
    }
}