namespace ParacletusConsole
{
	partial class ConsoleForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConsoleForm));
			this.ConsoleBox = new System.Windows.Forms.RichTextBox();
			this.InputBox = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// ConsoleBox
			// 
			this.ConsoleBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.ConsoleBox.BackColor = System.Drawing.Color.Black;
			this.ConsoleBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.ConsoleBox.ForeColor = System.Drawing.Color.White;
			this.ConsoleBox.Location = new System.Drawing.Point(0, 0);
			this.ConsoleBox.Name = "ConsoleBox";
			this.ConsoleBox.ReadOnly = true;
			this.ConsoleBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.ConsoleBox.Size = new System.Drawing.Size(632, 436);
			this.ConsoleBox.TabIndex = 1;
			this.ConsoleBox.TabStop = false;
			this.ConsoleBox.Text = "";
			// 
			// InputBox
			// 
			this.InputBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.InputBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.InputBox.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.InputBox.ForeColor = System.Drawing.Color.White;
			this.InputBox.Location = new System.Drawing.Point(0, 442);
			this.InputBox.Name = "InputBox";
			this.InputBox.Size = new System.Drawing.Size(632, 11);
			this.InputBox.TabIndex = 0;
			this.InputBox.TabStop = false;
			this.InputBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.inputBox_KeyPress);
			// 
			// ConsoleForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 11F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Black;
			this.ClientSize = new System.Drawing.Size(632, 453);
			this.Controls.Add(this.InputBox);
			this.Controls.Add(this.ConsoleBox);
			this.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "ConsoleForm";
			this.Text = "Paracletus";
			this.Deactivate += new System.EventHandler(this.ConsoleForm_Deactivate);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ConsoleForm_FormClosing);
			this.Load += new System.EventHandler(this.ConsoleForm_Load);
			this.LocationChanged += new System.EventHandler(this.ConsoleForm_LocationChanged);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		public System.Windows.Forms.RichTextBox ConsoleBox;
		public System.Windows.Forms.TextBox InputBox;
	}
}

