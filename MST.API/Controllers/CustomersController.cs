using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MST.Domain;
using MST.Domain.Abstraction;

namespace MST.API.Controllers
{
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly IEventBus _eventBus;

        public CustomersController(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomer()
        {
            var customer = new Customer {Name = "XiaoMa"};
            await _eventBus.PublishAsync(new CustomerCreatedEvent(customer.Name));
            return new ObjectResult(true);
        }
    }
}