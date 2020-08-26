using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Site.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Site.Hubs {
	public class PairingHub : Hub {
		private readonly ILogger<PairingHub> logger;
		private readonly ConnectedClientStorage connectedClientStorage;

		public PairingHub(
			ILogger<PairingHub> logger,
			ConnectedClientStorage connectedClientStorage
		) {
			this.logger = logger;
			this.connectedClientStorage = connectedClientStorage;
		}

		public override async Task OnConnectedAsync() {
			var alias = connectedClientStorage.ConnectClient(Context.ConnectionId);
			logger.LogTrace("Client connected: {ConnectionId} = {Alias}", Context.ConnectionId, alias);
			await Clients.Caller.SendAsync("aliasAssigned", alias);
			await base.OnConnectedAsync();
		}
	}
}
