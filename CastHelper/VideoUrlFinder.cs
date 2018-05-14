﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CastHelper {
	public static class VideoUrlFinder {
		// This search ignores query strings
		private static readonly Regex videoUrlRegex1 = new Regex(@"['""`]((https?:)?//[^'""`]+\.(m3u8|mp4)(\?[^'""`]+)?)['""`]", RegexOptions.IgnoreCase);
		private static readonly Regex iframeRegex = new Regex(@"<iframe[^>]+src=['""]([^'""]+)", RegexOptions.IgnoreCase);

		private static IEnumerable<string> GetUrlRegexMatches(Regex regex, int group, string html) {
			foreach (Match m in regex.Matches(html)) {
				string url = m.Groups[group].Value;
				if (url.StartsWith("//")) url = $"http:{url}";
				yield return url;
			}
		}

		public static async Task<string> GetVideoUriFromUriAsync(Uri uri, CookieContainer cookieContainer = null) {
			var req = WebRequest.CreateHttp(uri);
			req.Method = "GET";
			req.Accept = Program.Accept;
			req.UserAgent = Program.UserAgent;
			req.AllowAutoRedirect = false;
			if (cookieContainer != null) {
				req.CookieContainer = cookieContainer;
			}
			using (var resp = await req.GetResponseAsync())
			using (var sr = new StreamReader(resp.GetResponseStream())) {
				string html = await sr.ReadToEndAsync();
				return GetVideoUriFromHtml(html);
			}
		}

		public static string GetVideoUriFromHtml(string html) {
			var urls = GetUrlRegexMatches(videoUrlRegex1, 1, html).Distinct().ToList();
			if (!urls.Any()) urls = GetUrlRegexMatches(iframeRegex, 1, html).Distinct().ToList();
			if (urls.Count > 1) {
				using (var f = new SelectTypeForm<string>("Multiple possible video URLs were found.", urls)) {
					return f.ShowDialog() == DialogResult.OK
						? f.SelectedItem
						: null;
				}
			} else {
				return urls.SingleOrDefault();
			}
		}
	}
}