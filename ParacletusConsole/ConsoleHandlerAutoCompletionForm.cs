using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ParacletusConsole
{
	public partial class ConsoleHandler
	{
		void ShowAutoCompletionForm(List<string> autoCompletionStrings)
		{
			IgnoreNextLossOfFocus = true;
			AutoCompletionThread = new Thread(() =>
			{
				AutoCompletionMatchesForm.AutoCompletionListBox.Items.AddRange(autoCompletionStrings.ToArray());
				AutoCompletionMatchesForm.ShowDialog();
			}
			);
			AutoCompletionThread.Start();
		}

		void CloseAutoCompletionForm()
		{
			if (AutoCompletionThread != null)
			{
				if (Thread.CurrentThread == AutoCompletionThread)
				{
					AutoCompletionThread = null;
					ActualCloseAutoCompletionForm();
				}
				else
				{
					AutoCompletionMatchesForm.Invoke(
						(MethodInvoker)delegate
						{
							ActualCloseAutoCompletionForm();
						}
					);
					AutoCompletionThread.Join();
					AutoCompletionThread = null;
				}
			}
		}

		void ActualCloseAutoCompletionForm()
		{
			AutoCompletionMatchesForm.AutoCompletionListBox.Items.Clear();
			AutoCompletionMatchesForm.Close();
		}

		public void UpdateAutoCompletionFormPosition()
		{
			if (AutoCompletionThread != null)
			{
				AutoCompletionMatchesForm.Invoke(
					(MethodInvoker)delegate
					{
						AutoCompletionMatchesForm.Left = MainForm.Left + MainForm.InputBox.Left + 16;
						AutoCompletionMatchesForm.Top = MainForm.Top + MainForm.InputBox.Top - AutoCompletionMatchesForm.Height;
					}
				);
			}
		}

		public void OnAutoCompletionFormLoad()
		{
			int itemCount = AutoCompletionMatchesForm.AutoCompletionListBox.Items.Count;
			const int maximumCount = 10;
			if (itemCount < maximumCount)
			{
				int newHeight = OriginalAutoListBoxHeight - (maximumCount - itemCount + 1) * AutoCompletionMatchesForm.AutoCompletionListBox.ItemHeight;
				AutoCompletionMatchesForm.AutoCompletionListBox.Height = newHeight;
			}
			AutoCompletionMatchesForm.Height = AutoCompletionMatchesForm.AutoCompletionListBox.Height;
			UpdateAutoCompletionFormPosition();
			AutoCompletionMatchesForm.TopMost = true;
			AutoCompletionMatchesForm.BringToFront();
			MainForm.Invoke(
				(MethodInvoker)delegate
				{
					MainForm.Activate();
					MainForm.InputBox.Focus();
				}
			);
		}
	}
}
