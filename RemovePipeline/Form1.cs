using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.LinkLabel;

namespace RemovePipeline
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void Button2_Click(object sender, EventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.Title = "Browse Text Files Only";
			dialog.Filter = "Text files | *.txt";
			dialog.Multiselect = false;
			int[] lengths = new int[] { 10, 4, 40, 10, 10, 10, 11, 13, 10, 20, 35, 10 };
			try
			{
				if (dialog.ShowDialog() == DialogResult.OK)
				{
					var fileLocation = File.ReadAllLines(dialog.FileName);
					List<string> lines = new List<string>(fileLocation);
					string filePath = "C:/Modified.txt";
					for (int i = 5; i < lines.Count; i++)
					{
						string line = lines[i];
						if (line.Contains("---")) continue;
						int nthOccurrence = 1;
						foreach (int length in lengths)
						{
							if (nthOccurrence > 13) continue;
							line = CheckAndModifyString(nthOccurrence, line, length);
							nthOccurrence++;
						}
						lines[i] = line;
					}
					File.Delete(filePath);
					File.WriteAllLines(filePath, lines);
				}
				MessageBox.Show("File Modification Successful", "Message");
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error Occurred. Error message : " + ex.Message + "\n" + "Stack Trace: " + ex.StackTrace);
			}

		}

		private static string CheckAndModifyString(int nthOccurrence, string line, int length)
		{
			string value = GetCellValue(nthOccurrence, line, out int index);
			do
			{
				if (value.Length != length)
				{
					StringBuilder sb = new StringBuilder(line);
					sb.Remove(index, 1);
					line = sb.ToString();
				}
				value = GetCellValue(nthOccurrence, line, out int index2);
				index = index2;
			} while (value.Length != length);
			return line;
		}

		private static string GetCellValue(int nthOccurrence, string line, out int index)
		{
			int nthOccurrence2 = nthOccurrence + 1;
			int startIndex = line.TakeWhile(c => (nthOccurrence -= (c == '|' ? 1 : 0)) > 0).Count();
			int endIndex = line.TakeWhile(c => (nthOccurrence2 -= (c == '|' ? 1 : 0)) > 0).Count();
			index = endIndex;
			if (line.Length <= endIndex || line.Length <= startIndex)
			{
				return string.Empty;
			}
			string value = line.Substring(startIndex + 1, Math.Abs(endIndex - startIndex - 1));
			return value;
		}

	}
}