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
			this.consoleBox = new System.Windows.Forms.RichTextBox();
			this.inputBox = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// consoleBox
			// 
			this.consoleBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.consoleBox.BackColor = System.Drawing.Color.Black;
			this.consoleBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.consoleBox.ForeColor = System.Drawing.Color.White;
			this.consoleBox.Location = new System.Drawing.Point(0, 0);
			this.consoleBox.Name = "consoleBox";
			this.consoleBox.ReadOnly = true;
			this.consoleBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.consoleBox.Size = new System.Drawing.Size(632, 436);
			this.consoleBox.TabIndex = 1;
			this.consoleBox.Text = "";
			// 
			// inputBox
			// 
			this.inputBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.inputBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.inputBox.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.inputBox.ForeColor = System.Drawing.Color.White;
			this.inputBox.Location = new System.Drawing.Point(0, 442);
			this.inputBox.Name = "inputBox";
			this.inputBox.Size = new System.Drawing.Size(632, 11);
			this.inputBox.TabIndex = 0;
			this.inputBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.inputBox_KeyPress);
			// 
			// ConsoleForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 11F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Black;
			this.ClientSize = new System.Drawing.Size(632, 453);
			this.Controls.Add(this.inputBox);
			this.Controls.Add(this.consoleBox);
			this.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "ConsoleForm";
			this.Text = "Paracletus";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ConsoleForm_FormClosing);
			this.Load += new System.EventHandler(this.ConsoleForm_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		public System.Windows.Forms.RichTextBox consoleBox;
		public System.Windows.Forms.TextBox inputBox;
	}
}

