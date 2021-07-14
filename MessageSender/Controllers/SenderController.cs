using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MessageSender.Infrastructure;
using MessageSender.Models;
namespace MessageSender.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SenderController : ControllerBase
    {
        SenderService sender;
        public SenderController(SenderService sender)
        {
            this.sender = sender;
        }

        [HttpPost]
        public IActionResult Post(Employee employee)
        {
            sender.PublishMessage(employee);
            return Ok("Message Added in Queue");
        }
    }
}