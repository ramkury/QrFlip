using Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Site.Hubs;
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

		public PairingController(IHubContext<PairingHub> hub) {
			this.hub = hub;
		}

		[HttpGet]
		public string Get() {
			return generateId();
		}

		[HttpPost]
		public async Task<IActionResult> Post([FromBody]RedirectRequest request) {
			await hub.Clients.Client(request.ClientId).SendAsync("transferUrl", request.DestinationUrl);
			return Ok();
		}

		private string generateId() {
			var chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
			var id = Guid.NewGuid().ToByteArray();
			var max = 1UL << 20;
			var hi = BitConverter.ToUInt64(id, 0);
			var lo = BitConverter.ToUInt64(id, 8);
			var result = ((hi % max) << 20) | (lo % max);
			var sb = new StringBuilder(8);
			for (int i = 0; i < 8; i++) {
				sb.Append(chars[(int)(result & 0x1F)]);
				result >>= 5;
			}
			return sb.ToString();
		}
	}
}
