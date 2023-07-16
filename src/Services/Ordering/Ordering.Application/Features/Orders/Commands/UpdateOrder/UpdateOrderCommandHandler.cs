using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contracts.Persistence;
using Ordering.Application.Exceptions;
using Ordering.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ordering.Application.Features.Orders.Commands.UpdateOrder
{
    public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<UpdateOrderCommandHandler> _logger;
        private readonly IMapper _mapper;

        public UpdateOrderCommandHandler(IOrderRepository orderRepository, ILogger<UpdateOrderCommandHandler> logger, IMapper mapper)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Unit> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
        {
            var orderToBeUpated = await _orderRepository.GetByIdAsync(request.Id);

            if(orderToBeUpated == null)
            {
                _logger.LogError("order {} not found");
                throw new NotFoundException(nameof(Order), orderToBeUpated.Id);
            }

            _mapper.Map(request, orderToBeUpated, typeof(UpdateOrderCommand), typeof(Order));
            await _orderRepository.UpdateAsync(orderToBeUpated);
            _logger.LogInformation($"Order with Id = {orderToBeUpated.Id} updated successfully.");
            return Unit.Value;
        }
    }
}
