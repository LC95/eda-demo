using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MST.Domain;
using MST.Domain.Abstraction;
using MST.Domain.Abstraction.Events;
using MST.Domain.Events;

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
            await _eventBus.PublishAsync(new AddCustomerEvent());
            return new ObjectResult(true);
        }
    }
}