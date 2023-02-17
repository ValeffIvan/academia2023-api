using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using NovitSoftware.Academia.Persistence;
using NovitSoftware.Academia.Services.DTOs;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;

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
        //funcion para crear reserva
        [HttpPost("ReservaNueva")]
        [AllowAnonymous]

        public ActionResult ReservaNueva([FromBody] ReservaXProducto reserva)
        {
            Reserva NewReserva = new Reserva()
            {
                Cliente = reserva.reserva.Cliente,
                IdReserva = reserva.reserva.IdReserva,
                IdVendedor = reserva.reserva.IdVendedor,
                Estado = 1,
            };

            var producto = context.Productos.FirstOrDefault(x => x.Codigo == reserva.producto.Codigo);
            try
            {
                if (producto.Precio <= 100000 )
                {
                    NewReserva.Estado = 4;

                    //actualizacion del estado del producto y asociacion con la reserva
                    producto.Estado = 2;
                    context.Entry(producto).State = EntityState.Modified;
                    context.SaveChanges();
                    NewReserva.IdProductos.Add(producto);

                    //Adicion de la nueva reserva
                    context.Reservas.Add(NewReserva);
                    context.SaveChanges();

                    return Ok("Reserva Aprobada");

                }
                else
                {
                    //por si el vendedor tiene mas de 3 reservas
                    if (reserva.reserva.IdVendedorNavigation.Reservas.Count() > 3)
                    {
                        return BadRequest("Este vendedor no puede reservar mas");
                    }
                    else
                    {
                        NewReserva.Estado = 1;

                        //actualizacion del estado del producto y asociacion con la reserva
                        producto.Estado = 2;
                        context.Entry(producto).State = EntityState.Modified;
                        context.SaveChanges();
                        NewReserva.IdProductos.Add(producto);

                        //Adicion de la nueva reserva
                        context.Reservas.Add(NewReserva);
                        context.SaveChanges();

                        return Ok("Reserva ingresada");

                    }
                }
            }
            catch
            {
                return BadRequest("La reserva no pudo ser creada");
            }
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
                        var product = context.Productos.FirstOrDefault(x => x.IdProducto == producto.IdProducto);
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

        //Funcion para cambiar estado de reserva y producto
        [HttpPut("CambiarEstado/{id}/{estado}")]
        [AllowAnonymous]
        public ActionResult CambiarEstado(int id, int estado)
        {
            try
            {
                var Reserva = context.Reservas.Include(y => y.IdProductos).FirstOrDefault(x => x.IdReserva == id);
                switch (estado)
                {
                    case 4: Reserva.Estado = 4; break;
                    case 3: Reserva.Estado = 3; break;
                    case 2: Reserva.Estado = 2; break;
                    case 5: Reserva.Estado = 5; break;
                    default: Reserva.Estado = 1; break;
                }
                context.Entry(Reserva).State = EntityState.Modified;
                context.SaveChanges();

                //cambiar el objeto a vendido
                var productoDeReserva = context.Productos.FirstOrDefault(z => z.IdProducto == Reserva.IdProductos.LastOrDefault().IdProducto);
                switch (estado)
                {
                    case 1: productoDeReserva.Estado = 2; break;
                    case 4: productoDeReserva.Estado = 3; break;
                    case 5: productoDeReserva.Estado = 1; break;
                    case 3: productoDeReserva.Estado = 1; break;
                    default: productoDeReserva.Estado = 1; break;
                }
                context.Entry(productoDeReserva).State = EntityState.Modified;
                context.SaveChanges();
                return Ok("Reserva y producto cambiado");
            }
            catch
            {
                return BadRequest("El codigo de la reserva no es valido,no existe o no tiene productos asociados");
            }
        }


    }
}
