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
						line = CheckAndModifyForItemColumn(3, line);
						line = CheckAndModifyForPurchaseOrderTextColumn(4, line);
						line = ValidateForDate(5, line);
						line = ValidateForDate(6, line);
						line = CheckAndModifyForPostgDateColumn(7, line);
						line = CheckAndModifyForVblValueColumn(8, line);
						line = CheckAndModifyForAuxAcctAs1Column(9, line);
						line = CheckAndModifyForCostElemColumn(10, line);
						line = CheckAndModifyForCostElemNameColumn(11, line);
						line = CheckAndModifyForOffsettingAccountNameColumn(12, line);
						line = CheckAndModifyForOffstAccountColumn(13, line);
						lines[i] = line;
					}
					File.Delete(filePath);
					File.WriteAllLines(filePath, lines);
				}
				MessageBox.Show("File Modification Successful", "Message");
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error Occurred. Error message : " + ex.Message);
			}

		}

		private static string CheckAndModifyForItemColumn(int nthOccurrence, string line)
		{
			string value = GetColumnValue(nthOccurrence, line, out int index);
			string output = line;
			bool canContinue = true;
			do
			{
				if (value != string.Empty && long.TryParse(value, out _))
				{
					StringBuilder sb = new StringBuilder(output);
					sb.Remove(index, 1);
					output = sb.ToString();
				}
				value = GetColumnValue(nthOccurrence, output, out int index2);
				index = index2;
				if (value == string.Empty || (value != string.Empty && !long.TryParse(value, out _)))
				{
					canContinue = false;
				}
			} while (canContinue);
			return output;
		}

		private static string CheckAndModifyForPurchaseOrderTextColumn(int nthOccurrence, string line)
		{
			int nthOccurrence2 = nthOccurrence + 1;
			var index = line.TakeWhile(c => (nthOccurrence -= (c == '|' ? 1 : 0)) > 0).Count();
			var nextIndex = line.TakeWhile(c => (nthOccurrence2 -= (c == '|' ? 1 : 0)) > 0).Count();

			if (line.Length > index + 1 && !String.IsNullOrWhiteSpace(line.Substring(index + 1, nextIndex - 1).Trim())
				&& !long.TryParse(line.Substring(index + 1, 1), out _))
			{
				StringBuilder sb = new StringBuilder(line);
				sb.Remove(index, 1);
				return sb.ToString();
			}
			return line;
		}

		private static string ValidateForDate(int nthOccurrence, string line)
		{
			string value = GetColumnValue(nthOccurrence, line, out int index).Trim();
			if (value != string.Empty)
			{
				if (!DateTime.TryParseExact(value, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
				{
					StringBuilder sb = new StringBuilder(line);
					sb.Remove(index, 1);
					return sb.ToString();
				}
			}
			return line;
		}

		private static string CheckAndModifyForPostgDateColumn(int nthOccurrence, string line)
		{
			string value = GetColumnValue(nthOccurrence, line, out int index);
			if (value != string.Empty)
			{
				if (long.TryParse(value, out _))
				{
					StringBuilder sb = new StringBuilder(line);
					sb.Remove(index, 1);
					return sb.ToString();
				}
			}
			return line;
		}

		private static string CheckAndModifyForVblValueColumn(int nthOccurrence, string line)
		{
			string value = GetColumnValue(nthOccurrence, line, out int index);
			if (value != string.Empty)
			{
				if (float.TryParse(value, out _))
				{
					StringBuilder sb = new StringBuilder(line);
					sb.Remove(index, 1);
					return sb.ToString();
				}
			}
			return line;
		}

		private static string CheckAndModifyForOffstAccountColumn(int nthOccurrence, string line)
		{
			string value = GetColumnValue(nthOccurrence, line, out int index);
			if (value != string.Empty)
			{
				StringBuilder sb = new StringBuilder(line);
				sb.Remove(index, 1);
				return sb.ToString();
			}
			return line;
		}

		private static string CheckAndModifyForAuxAcctAs1Column(int nthOccurrence, string line)
		{
			string currValue = GetColumnValue(nthOccurrence - 1, line, out int i).Trim();
			if (currValue == string.Empty)
			{
				return line;
			}
			string value = GetColumnValue(nthOccurrence, line, out int index);
			string nextColValue = GetColumnValue(nthOccurrence + 1, line, out int index2) ?? "0";

			if (value != string.Empty)
			{
				if (long.TryParse(value, out _) && long.TryParse(nextColValue, out _))
				{
					StringBuilder sb = new StringBuilder(line);
					sb.Remove(index, 1);
					return sb.ToString();
				}
			}
			return line;
		}

		private static string CheckAndModifyForCostElemColumn(int nthOccurrence, string line)
		{
			string value = GetColumnValue(nthOccurrence, line, out int index);
			if (value != string.Empty)
			{
				if (long.TryParse(value, out _))
				{
					StringBuilder sb = new StringBuilder(line);
					sb.Remove(index, 1);
					return sb.ToString();
				}
			}
			return line;
		}

		private static string CheckAndModifyForCostElemNameColumn(int nthOccurrence, string line)
		{
			string value = GetColumnValue(nthOccurrence, line, out int index);
			string nextColValue = GetColumnValue(nthOccurrence + 1, line, out int index2) ?? "0";

			if (nextColValue != string.Empty)
			{
				if (!long.TryParse(nextColValue, out _))
				{
					StringBuilder sb = new StringBuilder(line);
					sb.Remove(index, 1);
					return sb.ToString();
				}
			}
			return line;
		}

		private static string CheckAndModifyForOffsettingAccountNameColumn(int nthOccurrence, string line)
		{
			string value = GetColumnValue(nthOccurrence, line, out int index);
			string output = line;
			bool canContinue = true;
			do
			{
				if (value != string.Empty)
				{
					if (!long.TryParse(value, out _))
					{
						StringBuilder sb = new StringBuilder(output);
						sb.Remove(index, 1);
						output = sb.ToString();
					}
				}
				value = GetColumnValue(nthOccurrence, output, out int index2);
				index = index2;
				if (value == string.Empty || (value != string.Empty && long.TryParse(value, out _)))
				{
					canContinue = false;
				}
			} while (canContinue);
			return output;
		}

		private static string GetColumnValue(int nthOccurrence, string line, out int index)
		{
			int nthOccurrence2 = nthOccurrence + 1;
			int startIndex = line.TakeWhile(c => (nthOccurrence -= (c == '|' ? 1 : 0)) > 0).Count();
			index = startIndex;
			int endIndex = line.TakeWhile(c => (nthOccurrence2 -= (c == '|' ? 1 : 0)) > 0).Count();
			if (line.Length <= endIndex || line.Length <= startIndex)
			{
				return string.Empty;
			}
			string value = line.Substring(startIndex + 1, Math.Abs(endIndex - startIndex - 1)).Trim();
			return value;
		}

	}
}