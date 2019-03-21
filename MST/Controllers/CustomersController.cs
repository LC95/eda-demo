using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MST.Domain;
using MST.Domain.Core;

namespace MST.Controllers {
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase {
        private readonly IEventBus _eventBus;

        public CustomersController(IEventBus eventBus)
        {
            this._eventBus = eventBus;
        }
        [HttpPost]
        public async Task<IActionResult> CreateCustomer()
        {
            var customer = new Customer() { Name = "XiaoMa"};
            await _eventBus.PublishAsync(new CustomerCreatedEvent(customer.Name));
            return new ObjectResult(true);
        }
    }
}
