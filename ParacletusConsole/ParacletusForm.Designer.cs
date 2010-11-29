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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConsoleForm));
			this.ConsoleBox = new System.Windows.Forms.RichTextBox();
			this.InputBox = new System.Windows.Forms.TextBox();
			this.TabContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.test1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.test2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.test3ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.TabContextMenuStrip.SuspendLayout();
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
			// TabContextMenuStrip
			// 
			this.TabContextMenuStrip.AutoSize = false;
			this.TabContextMenuStrip.BackColor = System.Drawing.Color.Black;
			this.TabContextMenuStrip.Font = new System.Drawing.Font("Lucida Console", 8F);
			this.TabContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.test1ToolStripMenuItem,
            this.test2ToolStripMenuItem,
            this.test3ToolStripMenuItem});
			this.TabContextMenuStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.VerticalStackWithOverflow;
			this.TabContextMenuStrip.Name = "contextMenuStrip1";
			this.TabContextMenuStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.TabContextMenuStrip.ShowImageMargin = false;
			this.TabContextMenuStrip.ShowItemToolTips = false;
			this.TabContextMenuStrip.Size = new System.Drawing.Size(153, 92);
			// 
			// test1ToolStripMenuItem
			// 
			this.test1ToolStripMenuItem.ForeColor = System.Drawing.Color.White;
			this.test1ToolStripMenuItem.Name = "test1ToolStripMenuItem";
			this.test1ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.test1ToolStripMenuItem.Text = "test1";
			this.test1ToolStripMenuItem.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// test2ToolStripMenuItem
			// 
			this.test2ToolStripMenuItem.ForeColor = System.Drawing.Color.White;
			this.test2ToolStripMenuItem.Name = "test2ToolStripMenuItem";
			this.test2ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.test2ToolStripMenuItem.Text = "test2";
			this.test2ToolStripMenuItem.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// test3ToolStripMenuItem
			// 
			this.test3ToolStripMenuItem.ForeColor = System.Drawing.Color.White;
			this.test3ToolStripMenuItem.Name = "test3ToolStripMenuItem";
			this.test3ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.test3ToolStripMenuItem.Text = "test3";
			this.test3ToolStripMenuItem.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ConsoleForm_FormClosing);
			this.Load += new System.EventHandler(this.ConsoleForm_Load);
			this.TabContextMenuStrip.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		public System.Windows.Forms.RichTextBox ConsoleBox;
		public System.Windows.Forms.TextBox InputBox;
		public System.Windows.Forms.ContextMenuStrip TabContextMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem test1ToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem test2ToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem test3ToolStripMenuItem;
	}
}

