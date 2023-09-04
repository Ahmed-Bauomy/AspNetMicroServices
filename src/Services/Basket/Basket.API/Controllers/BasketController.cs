using AutoMapper;
using Basket.API.Entities;
using Basket.API.GrpcServices;
using Basket.API.Repositroy;
using EventBus.Messages.Event;
using MassTransit;
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
        private readonly DiscountGrpcService _discountGrpcService;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IMapper _mapper;

        public BasketController(IBasketRepository basketRepositroy, DiscountGrpcService discountGrpcService, IPublishEndpoint publishEndpoint, IMapper mapper)
        {
            _basketRepositroy = basketRepositroy ?? throw new ArgumentNullException(nameof(basketRepositroy));
            _discountGrpcService = discountGrpcService ?? throw new ArgumentNullException(nameof(discountGrpcService));
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
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
            // TODO: communicate with Discount.Grpc
            // calculate latest prices of product in shopping cart
            foreach(var item in basket.Items)
            {
                var coupon = await _discountGrpcService.GetDiscount(item.ProductName);
                item.Price -= coupon.Amount;
            }
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
        [Route("[action]")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Checkout([FromBody]BasketCheckout basketCheckout)
        {
            // get current basket from db
            var basket = await _basketRepositroy.GetBasket(basketCheckout.UserName);
            if(basket == null)
            {
                return NotFound();
            }
            // create basketcheckoutevent with toatalPrice updated
            var basketCheckoutEvent = _mapper.Map<BasketCheckoutEvent>(basketCheckout);
            basketCheckoutEvent.TotalPrice = basket.TotalPrice;
            // publish basket checkout event
            await _publishEndpoint.Publish(basketCheckoutEvent);

            // remove basket
            await _basketRepositroy.DeleteBasket(basket.UserName);
            return Accepted();
        }
    }
}
