using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcoPowerLogisticsAPI.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Authorization;
using JWTAuthentication.Authentication;

namespace EcoPowerLogisticsAPI.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly Project2Context _context;

        public OrdersController(Project2Context context)
        {
            _context = context;
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            return await _context.Orders.ToListAsync();
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(short id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

        // POST: api/Orders
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrder", new { id = order.OrderId }, order);
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(short id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (!OrderExists(id))
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Private method to check if an Order exists
        private bool OrderExists(short id)
        {
            return (_context.Orders?.Any(e => e.OrderId == id)).GetValueOrDefault();
        }

        // PATCH: api/Orders/5
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchOrder(short id, [FromBody] JsonPatchDocument<Order> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            // Apply the patch operations manually
            patchDocument.ApplyTo(order);

            // Validate the patched entity
            TryValidateModel(order);

            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // GET: api/Orders/Customer/5
        [HttpGet("Customer/{customerId}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrdersForCustomer(short customerId)
        {
            var orders = await _context.Orders.Where(o => o.CustomerId == customerId).ToListAsync();
            if (orders == null || orders.Count == 0)
            {
                return NotFound();
            }

            return orders;
        }

    }
}