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
			this.consoleBox = new System.Windows.Forms.RichTextBox();
			this.inputBox = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// consoleBox
			// 
			this.consoleBox.Location = new System.Drawing.Point(0, 1);
			this.consoleBox.Name = "consoleBox";
			this.consoleBox.ReadOnly = true;
			this.consoleBox.Size = new System.Drawing.Size(630, 426);
			this.consoleBox.TabIndex = 1;
			this.consoleBox.Text = "";
			// 
			// inputBox
			// 
			this.inputBox.Location = new System.Drawing.Point(0, 433);
			this.inputBox.Name = "inputBox";
			this.inputBox.Size = new System.Drawing.Size(633, 18);
			this.inputBox.TabIndex = 0;
			this.inputBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.inputBox_KeyPress);
			// 
			// ConsoleForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 11F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(632, 453);
			this.Controls.Add(this.inputBox);
			this.Controls.Add(this.consoleBox);
			this.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Name = "ConsoleForm";
			this.Text = "Paracletus";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		public System.Windows.Forms.RichTextBox consoleBox;
		public System.Windows.Forms.TextBox inputBox;
	}
}

