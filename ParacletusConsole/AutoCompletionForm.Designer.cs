namespace ParacletusConsole
{
	partial class AutoCompletionForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AutoCompletionForm));
			this.AutoCompletionListBox = new System.Windows.Forms.ListBox();
			this.SuspendLayout();
			// 
			// AutoCompletionListBox
			// 
			this.AutoCompletionListBox.BackColor = System.Drawing.Color.Black;
			resources.ApplyResources(this.AutoCompletionListBox, "AutoCompletionListBox");
			this.AutoCompletionListBox.ForeColor = System.Drawing.Color.White;
			this.AutoCompletionListBox.FormattingEnabled = true;
			this.AutoCompletionListBox.Name = "AutoCompletionListBox";
			// 
			// AutoCompletionForm
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Black;
			this.ControlBox = false;
			this.Controls.Add(this.AutoCompletionListBox);
			this.ForeColor = System.Drawing.Color.White;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "AutoCompletionForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Load += new System.EventHandler(this.TabForm_Load);
			this.ResumeLayout(false);

		}

		#endregion

		public System.Windows.Forms.ListBox AutoCompletionListBox;
	}
}