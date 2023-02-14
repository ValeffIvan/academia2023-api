using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NovitSoftware.Academia.Persistence;

namespace NovitSoftware.Academia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]    
    [Authorize(Roles = "Administrador,Vendedor")]
    public class ProductoController : ControllerBase
    {
        private readonly AplicacionDbContext context;
        
        public ProductoController (AplicacionDbContext context)
        {
            this.context = context;
        }

        [HttpGet("GetProducts")]
        [AllowAnonymous]
        public ActionResult Products()
        {
            try
            {
                var productList = context.Productos.Select(x =>
                new
                {
                    x.IdProducto,
                    x.Precio,
                    x.Barrio,
                    x.Codigo,
                    x.Estado,
                    x.Imagen,
                }).ToList();
                return Ok(productList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("PostProducts")]
        [AllowAnonymous]
        public ActionResult AddProduct (Producto producto)
        {
            try
            {
                var newProduct = new Producto()
                {
                    Codigo = producto.Codigo,
                    Barrio = producto.Barrio,
                    Precio = producto.Precio,
                    Imagen = producto.Imagen,
                    Estado = 1,
                };

                context.Productos.Add(newProduct);

                context.SaveChanges();

                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeleteProducts" + "{idProducto}")]
        [AllowAnonymous]
        public ActionResult RemoveProduct (int idProducto)
        {
            try
            {
                var product = context.Productos.FirstOrDefault(x => x.IdProducto == idProducto);
                context.Productos.Remove(product);
                context.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("PutProducts" + "{product}")]
        [AllowAnonymous]
        public ActionResult ChangeProduct (Producto product)
        {
            var productoActualizado = context.Productos.FirstOrDefault(x=>x.IdProducto == product.IdProducto);
            productoActualizado.Precio = product.Precio;
            productoActualizado.Imagen = product.Imagen;
            productoActualizado.Barrio = product.Barrio;
            productoActualizado.Codigo = product.Codigo;
            productoActualizado.Estado = product.Estado;
            context.Entry(productoActualizado).State = EntityState.Modified;
            context.SaveChanges();
            return Ok();
        }
    }
}
