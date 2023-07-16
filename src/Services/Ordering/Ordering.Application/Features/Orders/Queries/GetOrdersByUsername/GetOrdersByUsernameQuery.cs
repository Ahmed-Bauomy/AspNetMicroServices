using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Application.Features.Orders.Queries.GetOrdersByUsername
{
    public class GetOrdersByUsernameQuery : IRequest<List<OrderDTO>>
    {
        public string Username { get; set; }

        public GetOrdersByUsernameQuery(string username)
        {
            Username = username;
        }
    }
}
