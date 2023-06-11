using Basket.API.Entities;
using Basket.API.Repositroy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Basket.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository _basketRepositroy;

        public BasketController(IBasketRepository basketRepositroy)
        {
            _basketRepositroy = basketRepositroy ?? throw new ArgumentNullException(nameof(basketRepositroy));
        }

        [Route("{userName}",Name ="GetBasket")]
        [HttpGet]
        [ProducesResponseType(typeof(ShoppingCart),(int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> GetBasket(string userName)
        {
            var Basket = await _basketRepositroy.GetBasket(userName);
            if (Basket == null)
                Basket = new ShoppingCart();
            return Ok(Basket);
        }
        [HttpPost]
        [ProducesResponseType(typeof(ShoppingCart),(int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> UpdateBasket([FromBody] ShoppingCart basket)
        {
            var updatedBaskt = await _basketRepositroy.UpdateBasket(basket);
            return Ok(updatedBaskt);
        }
        [HttpDelete("{userName}",Name = "DeleteBasket")]
        [ProducesResponseType(typeof(void),(int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteBasket(string userName)
        {
            var basket = await _basketRepositroy.GetBasket(userName);
            if (basket == null)
                return NotFound();

            await _basketRepositroy.DeleteBasket(userName);
            return Ok();
        }
    }
}
