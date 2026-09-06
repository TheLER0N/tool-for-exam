using System;
using System.Drawing;
using System.Windows.Forms;

namespace SafeWinForms
{
    public static class SafeFormDuplicator
    {
        public static Form OpenDuplicate(Form source)
        {
            Form copy = CreateDuplicate(source);
            if (copy != null) copy.Show();
            return copy;
        }

        public static Form CreateDuplicate(Form source)
        {
            if (source == null) throw new ArgumentNullException("source");
            Form copy = (Form)Activator.CreateInstance(source.GetType());
            CopyControlValues(source, copy);
            return copy;
        }

        public static void CopyControlValues(Control source, Control target)
        {
            if (source == null || target == null) return;

            CopySingleControl(source, target);

            foreach (Control sourceChild in source.Controls)
            {
                if (string.IsNullOrEmpty(sourceChild.Name)) continue;

                Control[] found = target.Controls.Find(sourceChild.Name, false);
                if (found != null && found.Length > 0)
                {
                    CopyControlValues(sourceChild, found[0]);
                }
            }
        }

        private static void CopySingleControl(Control source, Control target)
        {
            try
            {
                target.Text = source.Text;

                Form sourceForm = source as Form;
                Form targetForm = target as Form;
                if (sourceForm != null && targetForm != null)
                {
                    targetForm.Size = sourceForm.Size;
                    targetForm.StartPosition = sourceForm.StartPosition;
                    return;
                }

                CheckBox sourceCheck = source as CheckBox;
                CheckBox targetCheck = target as CheckBox;
                if (sourceCheck != null && targetCheck != null)
                {
                    targetCheck.Checked = sourceCheck.Checked;
                    return;
                }

                RadioButton sourceRadio = source as RadioButton;
                RadioButton targetRadio = target as RadioButton;
                if (sourceRadio != null && targetRadio != null)
                {
                    targetRadio.Checked = sourceRadio.Checked;
                    return;
                }

                ComboBox sourceCombo = source as ComboBox;
                ComboBox targetCombo = target as ComboBox;
                if (sourceCombo != null && targetCombo != null)
                {
                    if (sourceCombo.SelectedIndex >= 0 && sourceCombo.SelectedIndex < targetCombo.Items.Count)
                        targetCombo.SelectedIndex = sourceCombo.SelectedIndex;
                    else
                        targetCombo.Text = sourceCombo.Text;
                    return;
                }

                ListBox sourceList = source as ListBox;
                ListBox targetList = target as ListBox;
                if (sourceList != null && targetList != null)
                {
                    targetList.ClearSelected();
                    foreach (int index in sourceList.SelectedIndices)
                    {
                        if (index >= 0 && index < targetList.Items.Count)
                            targetList.SetSelected(index, true);
                    }
                    return;
                }

                CheckedListBox sourceChecked = source as CheckedListBox;
                CheckedListBox targetChecked = target as CheckedListBox;
                if (sourceChecked != null && targetChecked != null)
                {
                    for (int i = 0; i < sourceChecked.Items.Count && i < targetChecked.Items.Count; i++)
                        targetChecked.SetItemChecked(i, sourceChecked.GetItemChecked(i));
                    return;
                }

                NumericUpDown sourceNumeric = source as NumericUpDown;
                NumericUpDown targetNumeric = target as NumericUpDown;
                if (sourceNumeric != null && targetNumeric != null)
                {
                    decimal value = sourceNumeric.Value;
                    if (value < targetNumeric.Minimum) value = targetNumeric.Minimum;
                    if (value > targetNumeric.Maximum) value = targetNumeric.Maximum;
                    targetNumeric.Value = value;
                    return;
                }

                DateTimePicker sourceDate = source as DateTimePicker;
                DateTimePicker targetDate = target as DateTimePicker;
                if (sourceDate != null && targetDate != null)
                {
                    DateTime value = sourceDate.Value;
                    if (value < targetDate.MinDate) value = targetDate.MinDate;
                    if (value > targetDate.MaxDate) value = targetDate.MaxDate;
                    targetDate.Value = value;
                    return;
                }

                TrackBar sourceTrack = source as TrackBar;
                TrackBar targetTrack = target as TrackBar;
                if (sourceTrack != null && targetTrack != null)
                {
                    int value = sourceTrack.Value;
                    if (value < targetTrack.Minimum) value = targetTrack.Minimum;
                    if (value > targetTrack.Maximum) value = targetTrack.Maximum;
                    targetTrack.Value = value;
                    return;
                }

                TabControl sourceTabs = source as TabControl;
                TabControl targetTabs = target as TabControl;
                if (sourceTabs != null && targetTabs != null)
                {
                    if (sourceTabs.SelectedIndex >= 0 && sourceTabs.SelectedIndex < targetTabs.TabPages.Count)
                        targetTabs.SelectedIndex = sourceTabs.SelectedIndex;
                    return;
                }

                DataGridView sourceGrid = source as DataGridView;
                DataGridView targetGrid = target as DataGridView;
                if (sourceGrid != null && targetGrid != null)
                {
                    CopyGrid(sourceGrid, targetGrid);
                }
            }
            catch { }
        }

        private static void CopyGrid(DataGridView source, DataGridView target)
        {
            if (source.DataSource != null || target.DataSource != null) return;

            int rows = Math.Min(source.Rows.Count, target.Rows.Count);
            int cols = Math.Min(source.Columns.Count, target.Columns.Count);

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    try { target[x, y].Value = source[x, y].Value; } catch { }
                }
            }

            try
            {
                if (source.CurrentCell != null)
                {
                    int row = source.CurrentCell.RowIndex;
                    int col = source.CurrentCell.ColumnIndex;
                    if (row < target.Rows.Count && col < target.Columns.Count)
                        target.CurrentCell = target[col, row];
                }
            }
            catch { }
        }
    }
}