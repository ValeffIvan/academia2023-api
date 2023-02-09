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

        [HttpPost("PostReservas")]
        [AllowAnonymous]
        public ActionResult AddReservas (Reserva reservas)
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

            return Ok(new { IdVendedor = newReserva.IdVendedor, Cliente = newReserva.Cliente, Estado = newReserva.Estado, IdReserva = newReserva.IdReserva});
        }

        [HttpDelete("DeleteReserva")]
        [AllowAnonymous]
        public ActionResult RemoveReserva(int idReserva)
        {
            try
            {
                if (idReserva <= 0)
                    return BadRequest("Not a valid product id");
                else
                {
                    var reserva = context.Reservas.FirstOrDefault(x => x.IdReserva == idReserva);
                    context.Reservas.Remove(reserva);
                    context.SaveChanges();
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("PutReserva")]
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
    }
}
