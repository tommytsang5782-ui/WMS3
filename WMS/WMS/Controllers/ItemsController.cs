using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        //private readonly WarehouseContext _context;

        //public ItemController(WarehouseContext context)
        //{
        //    _context = context;
        //}

        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<Item>>> GetItem()
        //{
        //    try
        //    {
        //        return await _context.Item.ToListAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        // 添加日志记录或异常处理逻辑
        //        throw new InvalidOperationException("Error fetching items", ex);
        //    }
        //}

        //[HttpPost]
        //public async Task<ActionResult<Item>> AddItem(Item item)
        //{
        //    try
        //    {
        //        _context.Item.Add(item);
        //        await _context.SaveChangesAsync();
        //        return CreatedAtAction(nameof(GetItem), new { id = item.No }, item);
        //    }
        //    catch (Exception ex)
        //    {
        //        // 添加日志记录或异常处理逻辑
        //        throw new InvalidOperationException("Error adding item", ex);
        //    }
        //}

        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdateItem(string id, [FromBody] Item item)
        //{
        //    id = Uri.UnescapeDataString(id);

        //    if (item == null || string.IsNullOrEmpty(item.No))
        //    {
        //        return BadRequest("Item or Item.No cannot be null or empty.");
        //    }
        //    if (id != item.No)
        //    {
        //        return BadRequest($"ID from URL ({id}) does not match Item.No ({item.No}).");
        //    }

        //    _context.Entry(item).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!ItemExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new InvalidOperationException("Error updating item", ex);
        //    }

        //    return NoContent();
        //}

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteItem(string id)
        //{
        //    try
        //    {
        //        var item = await _context.Item.FindAsync(id);
        //        if (item == null)
        //        {
        //            return NotFound();
        //        }

        //        _context.Item.Remove(item);
        //        await _context.SaveChangesAsync();

        //        return NoContent();
        //    }
        //    catch (Exception ex)
        //    {
        //        // 添加日志记录或异常处理逻辑
        //        throw new InvalidOperationException("Error deleting item", ex);
        //    }
        //}

        //private bool ItemExists(string id)
        //{
        //    try
        //    {
        //        return _context.Item.Any(e => e.No == id);
        //    }
        //    catch (Exception ex)
        //    {
        //        // 添加日志记录或异常处理逻辑
        //        throw new InvalidOperationException("Error checking item existence", ex);
        //    }
        //}
    }
}
