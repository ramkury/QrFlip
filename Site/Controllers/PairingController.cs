using Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Site.Hubs;
using Site.Storage;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Site.Controllers {
	
	[ApiController]
	[Route("[controller]")]
	public class PairingController : ControllerBase {
		private readonly IHubContext<PairingHub> hub;
		private readonly ConnectedClientStorage connectedClientStorage;

		public PairingController(
			IHubContext<PairingHub> hub,
			ConnectedClientStorage connectedClientStorage
		) {
			this.hub = hub;
			this.connectedClientStorage = connectedClientStorage;
		}

		[HttpPost]
		public async Task<IActionResult> Post([FromBody]RedirectRequest request) {
			await notifyAlias(request.ClientId, "transferUrl", request.DestinationUrl);
			return Ok();
		}

		private async Task notifyAlias(string alias, string method, object message) {
			var connectionId = connectedClientStorage.GetClientId(alias);
			var client = hub.Clients.Client(connectionId);
			await client.SendAsync(method, message);
		}
	}
}
