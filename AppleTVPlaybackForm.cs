﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CastHelper {
	public partial class AppleTVPlaybackForm : Form {
		public int PositionSec {
			get {
				return progressBar1.Value;
			}
			set {
				progressBar1.Value = value;
				label2.Text = $"{TimeSpan.FromSeconds(PositionSec)} / {TimeSpan.FromSeconds(DurationSec)}";
			}
		}

		public int DurationSec {
			get {
				return progressBar1.Maximum;
			}
			set {
				progressBar1.Maximum = value;
				label2.Text = $"{TimeSpan.FromSeconds(PositionSec)} / {TimeSpan.FromSeconds(DurationSec)}";
			}
		}

		public string LabelText {
			get {
				return label1.Text;
			}
			set {
				label1.Text = value;
			}
		}

		public bool OkToClose { get; set; }

		public AppleTVPlaybackForm() {
			InitializeComponent();
		}

		private void btnClose_Click(object sender, EventArgs e) {
			Close();
		}

		private void AppleTVPlaybackForm_FormClosing(object sender, FormClosingEventArgs e) {
			if (!OkToClose && DialogResult.Yes != MessageBox.Show(this, "Are you sure you want to close this window? This will stop video playback on the Apple TV.", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning)) {
				e.Cancel = true;
			}
		}
	}
}
