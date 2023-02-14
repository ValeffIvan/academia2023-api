using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NovitSoftware.Academia.Persistence;
using System.Runtime.CompilerServices;

namespace NovitSoftware.Academia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Administrador,Comercial")]
    public class ReservaController : ControllerBase
    {
        private readonly AplicacionDbContext context;

        public ReservaController(AplicacionDbContext context)
        {
            this.context = context;
        }

        [HttpGet("GetReservas")]
        [AllowAnonymous]
        public ActionResult Reservas()
        {
            try
            {
                var productList = context.Reservas.Select(x =>
                    new
                    {
                        x.IdReserva,
                        x.Cliente,
                        x.IdVendedor,
                        x.Estado,
                    }).ToList();
                return Ok(productList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost("PostReservas")]
        [AllowAnonymous]
        public ActionResult AddReservas(Reserva reservas)
        {
            var newReserva = new Reserva()
            {
                IdVendedor = reservas.IdVendedor,
                Cliente = reservas.Cliente,
                Estado = reservas.Estado,
                IdReserva = reservas.IdReserva
            };

            context.Reservas.Add(newReserva);

            context.SaveChanges();

            var reservasCreated = context.Reservas.FirstOrDefault(x => x.IdReserva == reservas.IdReserva);

            return Ok(new { IdVendedor = newReserva.IdVendedor, Cliente = newReserva.Cliente, Estado = newReserva.Estado, IdReserva = newReserva.IdReserva });
        }

        [HttpDelete("DeleteReserva" + "{idReserva}")]
        [AllowAnonymous] 
        public ActionResult RemoveReserva(int idReserva)
        {
            try
            {
                var reserva = context.Reservas.FirstOrDefault(x => x.IdReserva == idReserva);
                if (reserva != null)
                {
                    foreach (var producto in reserva.IdProductos)
                    {
                        var product = context.Productos.FirstOrDefault(x => x.IdProducto== producto.IdProducto);
                        product.Estado = 0;
                        context.Entry(product).State = EntityState.Modified;
                        context.SaveChanges();
                    }
                    context.Reservas.Remove(reserva);
                    context.SaveChanges();

                    return Ok();
                }
                else
                    return BadRequest("No existe la reserva");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("PutReserva" + "{reserva}")]
        [AllowAnonymous]
        public ActionResult ChangeReserva(Reserva reserva)
        {
            var reservaActualizada = context.Reservas.FirstOrDefault(x => x.IdReserva == reserva.IdReserva);
            reservaActualizada.IdReserva = reserva.IdReserva;
            reservaActualizada.Cliente = reserva.Cliente;
            reservaActualizada.Estado = reserva.Estado;
            reservaActualizada.IdVendedor = reserva.IdVendedor;
            context.Entry(reservaActualizada).State = EntityState.Modified;
            context.SaveChanges();
            return Ok();
        }

        //Funcion para aprobacion
        [HttpPut("AceptarReserva")]
        [AllowAnonymous] //esto solo para comerciante
        public ActionResult aceptar(int id)
        {
            try
            {
                var aprobReserva = context.Reservas.Include(y => y.IdProductos).FirstOrDefault(x => x.IdReserva == id);
                aprobReserva.Estado = 4;
                context.Entry(aprobReserva).State = EntityState.Modified;
                context.SaveChanges();

                //cambiar el objeto a vendido
                var objVend = context.Productos.FirstOrDefault(z => z.IdProducto == aprobReserva.IdProductos.LastOrDefault().IdProducto);
                objVend.Estado = 3;
                context.Entry(objVend).State = EntityState.Modified;
                context.SaveChanges();
                return Ok("Reserva confirmada");
            }
            catch
            {
                return BadRequest("El codigo de la reserva no es valido,no existe o no tiene productos asociados");
            }
        }
    }
}
