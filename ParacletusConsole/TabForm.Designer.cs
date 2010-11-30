namespace ParacletusConsole
{
	partial class TabForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TabForm));
			this.TabListBox = new System.Windows.Forms.ListBox();
			this.SuspendLayout();
			// 
			// TabListBox
			// 
			this.TabListBox.BackColor = System.Drawing.Color.Black;
			resources.ApplyResources(this.TabListBox, "TabListBox");
			this.TabListBox.ForeColor = System.Drawing.Color.White;
			this.TabListBox.FormattingEnabled = true;
			this.TabListBox.Name = "TabListBox";
			// 
			// TabForm
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Black;
			this.ControlBox = false;
			this.Controls.Add(this.TabListBox);
			this.ForeColor = System.Drawing.Color.White;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "TabForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Load += new System.EventHandler(this.TabForm_Load);
			this.ResumeLayout(false);

		}

		#endregion

		public System.Windows.Forms.ListBox TabListBox;
	}
}