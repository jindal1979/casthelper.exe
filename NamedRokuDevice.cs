﻿using RokuDotNet.Client;
using RokuDotNet.Client.Input;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CastHelper {
	public class NamedRokuDevice : IVideoDevice, IAudioDevice {
		private readonly IHttpRokuDevice _device;
		public string Name { get; private set; }
		public bool ShowControls { get; set; }

		public NamedRokuDevice(IHttpRokuDevice device, string name) {
			_device = device ?? throw new ArgumentNullException(nameof(device));
			Name = name ?? throw new ArgumentNullException(nameof(name));
		}

		public async Task PlayVideoAsync(string mediaUrl) {
			try {
				var req = WebRequest.CreateHttp(new Uri(_device.Location, $"/input/15985?t=v&u={WebUtility.UrlEncode(mediaUrl)}&k=(null)"));
				req.Method = "POST";
				req.UserAgent = Program.UserAgent;
				using (var resp = await req.GetResponseAsync())
				using (var s = resp.GetResponseStream()) { }
			} catch (Exception ex) {
				throw new Exception("Could not send media to Roku.", ex);
			}

			if (ShowControls) {
				using (var f = new RokuRemote(_device.Input)) {
					f.LabelText = $"Currently playing video on {Name}.";
					f.Text = Name;
					f.ShowDialog();
					await f.LastTask;
				}
			}
		}

		public async Task PlayAudioAsync(string mediaUrl, string contentType = null) {
			try {
				string mediatype;
				switch (contentType) {
					case "audio/mpeg":
						mediatype = "mp3";
						break;
					case "audio/mp4":
						mediatype = "m4a";
						break;
					default:
						mediatype = mediaUrl.Split('.').Last();
						break;
				}
				var req = WebRequest.CreateHttp(new Uri(_device.Location, $"/input/15985?t=a&u={WebUtility.UrlEncode(mediaUrl)}&songname=(null)&artistname=(null)&songformat={WebUtility.UrlEncode(mediatype)}&albumarturl=(null)"));
				req.Method = "POST";
				req.UserAgent = Program.UserAgent;
				using (var resp = await req.GetResponseAsync())
				using (var s = resp.GetResponseStream()) { }
			} catch (Exception ex) {
				throw new Exception("Could not send media to Roku.", ex);
			}

			if (ShowControls) {
				using (var f = new RokuRemote(_device.Input)) {
					f.LabelText = $"Currently playing video on {Name}.";
					f.Text = Name;
					f.ShowDialog();
					await f.LastTask;
				}
			}
		}

		public override string ToString() {
			return $"{Name} ({_device.Location.Host})";
		}
	}
}
