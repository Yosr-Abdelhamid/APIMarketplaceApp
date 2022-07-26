using APIMarketplaceApp.Models;
using APIMarketplaceApp.Services;
using APIMarketplaceApp.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
 using MongoDB.Driver.Linq;
 using System.Text.Json;
using System.Net;
using System.Net.Http;
using AutoMapper;
using System.Web.Http.Cors;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace APIMarketplaceApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class NotificationController : Controller
    {
        private readonly IMongoCollection<Notif> notifications ;
        private readonly UserService service;
          public NotificationController (UserService _service,IConfiguration configuration ,IMapper mapper)
        {
            var client = new MongoClient(configuration.GetConnectionString("MongoDBConnection"));
            var database = client.GetDatabase("MarketplaceSiteDB");
            notifications = database.GetCollection<Notif>("Notifications");
            service = _service;
          
            
        }
         [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
         var prod = this.notifications.Find(x => x.Id == id).FirstOrDefaultAsync();

        if (prod is null)
        {
            return NotFound();
        }

        await service.RemoveAsy(id);

        return NoContent();
        }
    }
}