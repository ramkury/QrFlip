using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Site.Storage {
	public class ConnectedClientStorage {
		private readonly ConcurrentDictionary<string, string> ConnectionAlias = new ConcurrentDictionary<string, string>();
		private readonly ConcurrentDictionary<string, string> AliasConnection = new ConcurrentDictionary<string, string>();

		public string ConnectClient(string clientId) {
			string alias;
			do {
				alias = generateAlias();
			} while (!AliasConnection.TryAdd(alias, clientId));
			ConnectionAlias.TryAdd(clientId, alias);
			return alias;
		}

		public void DisconnectClient(string clientId) {
			if (ConnectionAlias.TryRemove(clientId, out var alias)) {
				AliasConnection.Remove(alias, out _);
			}
		}

		public string GetClientId(string alias) {
			if (AliasConnection.TryGetValue(alias, out var clientId)) {
				return clientId;
			}
			return null;
		}

		private string generateAlias() {
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
