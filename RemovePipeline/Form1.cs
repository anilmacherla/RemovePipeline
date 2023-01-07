using System.Text;
using System.Text.RegularExpressions;

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
			if (dialog.ShowDialog() == DialogResult.OK) 
			{
				var fileLocation = File.ReadAllLines(dialog.FileName);
				List<string> lines = new List<string>(fileLocation);
				for(int i=5; i<lines.Count; i++)
				{
					string line = lines[i];
					line = CheckAndModifyForPurchaseOrderTextColumn(4,line);
					lines[i] = line;
				}
				File.WriteAllLines("C:/Modified2.txt", lines);
			}
			MessageBox.Show("File Modification Successful","Message");
		}

		private static string CheckAndModifyForPurchaseOrderTextColumn(int nthOccurrence, string line)
		{
			var index = line.TakeWhile(c => (nthOccurrence -= (c == '|' ? 1 : 0)) > 0).Count();
			if (line.Length > index + 1 && !String.IsNullOrWhiteSpace(line.Substring(index + 1, 1)) && !long.TryParse(line.Substring(index + 1, 1), out _) )
			{
				StringBuilder sb = new StringBuilder(line);
				sb.Remove(index, 1);
				return sb.ToString();
			}
			return line;
		}

	}
}