using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Ekzamen
{
    public partial class DemoForm : Form
    {
        public static readonly Color BgMain = Color.FromArgb(30, 30, 30);
        public static readonly Color BgBar = Color.FromArgb(36, 36, 36);
        public static readonly Color BgTitle = Color.FromArgb(25, 25, 25);
        public static readonly Color Border = Color.FromArgb(63, 63, 70);
        public static readonly Color TextMain = Color.FromArgb(232, 232, 232);
        public static readonly Color TextSoft = Color.FromArgb(157, 157, 157);
        public static readonly Color Accent = Color.FromArgb(224, 166, 43);
        public static readonly Color Primary = Color.FromArgb(61, 110, 168);
        public static readonly Color OkGreen = Color.FromArgb(78, 201, 76);

        private sealed class FormEntry
        {
            public string Name;
            public string Path;
        }

        private readonly List<FormEntry> _forms = new List<FormEntry>();
        private TextBox _folderText;
        private Button _btnBrowse;
        private ListBox _formsList;
        private Label _countLabel;
        private TextBox _nameText;
        private Label _hintLabel;
        private Button _btnDuplicate;
        private RichTextBox _log;
        private Label _stFound;

        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, int lParam);

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 2;

        public DemoForm()
        {
            BuildUi();
            _folderText.Text = GuessProjectFolder();
            ScanForms();
            UpdateHint();
            Log("Панель запущена, тема: тёмная");
            Log("Автопоиск: найдено " + _forms.Count.ToString() + " форм в проекте");
        }

        private static string GuessProjectFolder()
        {
            DirectoryInfo dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            for (int i = 0; i < 5 && dir != null; i++)
            {
                if (Directory.GetFiles(dir.FullName, "*.csproj").Length > 0) return dir.FullName;
                dir = dir.Parent;
            }
            return Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        }

        private void ScanForms()
        {
            _forms.Clear();
            _formsList.Items.Clear();
            string folder = _folderText.Text.Trim();
            if (Directory.Exists(folder))
            {
                foreach (string file in Directory.GetFiles(folder, "*.cs", SearchOption.TopDirectoryOnly))
                {
                    string fn = Path.GetFileName(file);
                    if (fn.EndsWith(".Designer.cs", StringComparison.OrdinalIgnoreCase)) continue;
                    string text;
                    try { text = File.ReadAllText(file); } catch { continue; }
                    Match m = Regex.Match(text, @"(?:public\s+|internal\s+|private\s+)?(?:partial\s+)?class\s+(\w+)\s*:\s*[^\n{]*\bForm\b");
                    if (m.Success) _forms.Add(new FormEntry { Name = m.Groups[1].Value, Path = file });
                }
            }
            foreach (FormEntry e in _forms) _formsList.Items.Add(e.Name);
            if (_formsList.Items.Count > 0) _formsList.SelectedIndex = 0;
            _countLabel.Text = "Найдено форм:  " + _forms.Count.ToString();
            _stFound.Text = "Форм найдено:  " + _forms.Count.ToString();
        }

        private void BrowseFolder()
        {
            using (FolderBrowserDialog d = new FolderBrowserDialog())
            {
                d.Description = "Папка проекта с .cs файлами форм";
                d.ShowNewFolderButton = false;
                string cur = _folderText.Text.Trim();
                d.SelectedPath = Directory.Exists(cur) ? cur : Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                if (d.ShowDialog(this) == DialogResult.OK)
                {
                    _folderText.Text = d.SelectedPath;
                    ScanForms();
                    Log("Выбрана папка: " + d.SelectedPath);
                }
            }
        }

        private void UpdateHint()
        {
            string n = _nameText.Text.Trim();
            _hintLabel.Text = n.Length > 0 ? "Файл будет создан как " + n + ".cs" : "Введи имя новой формы";
        }

        private FormEntry SelectedEntry()
        {
            int i = _formsList.SelectedIndex;
            if (i < 0 || i >= _forms.Count) return null;
            return _forms[i];
        }

        private void DuplicateForm()
        {
            if (!Directory.Exists(_folderText.Text.Trim()))
            {
                BrowseFolder();
                if (!Directory.Exists(_folderText.Text.Trim())) return;
            }
            FormEntry src = SelectedEntry();
            if (src == null) { Log("Ошибка: выбери исходную форму"); return; }
            string newName = _nameText.Text.Trim();
            if (!Regex.IsMatch(newName, @"^[A-Za-z_]\w*$")) { Log("Ошибка: недопустимое имя '" + newName + "'"); return; }
            string folder = _folderText.Text.Trim();
            if (File.Exists(Path.Combine(folder, newName + ".cs"))) { Log("Ошибка: файл " + newName + ".cs уже существует"); return; }
            try
            {
                DuplicatePart(src.Path, src.Name, newName, true);
                foreach (string extra in Directory.GetFiles(folder, "*.cs", SearchOption.TopDirectoryOnly))
                {
                    if (string.Equals(extra, src.Path, StringComparison.OrdinalIgnoreCase)) continue;
                    string t2;
                    try { t2 = File.ReadAllText(extra); } catch { continue; }
                    if (!Regex.IsMatch(t2, @"partial\s+class\s+" + Regex.Escape(src.Name) + @"\b")) continue;
                    DuplicatePart(extra, src.Name, newName, false);
                }
                Log("Дублирование: " + src.Name + ".cs  →  " + newName + ".cs выполнено");
                ScanForms();
            }
            catch (Exception ex)
            {
                Log("Ошибка дублирования: " + ex.Message);
            }
        }

        private void DuplicatePart(string sourcePath, string oldName, string newName, bool isMain)
        {
            string text = File.ReadAllText(sourcePath);
            string newText = Regex.Replace(text, @"\b" + Regex.Escape(oldName) + @"\b", newName);
            string oldFile = Path.GetFileName(sourcePath);
            string newFile = isMain
                ? newName + ".cs"
                : Regex.Replace(oldFile, @"\b" + Regex.Escape(oldName) + @"\b", newName);
            if (string.Equals(newFile, oldFile, StringComparison.OrdinalIgnoreCase)) newFile = newName + "." + oldFile;
            File.WriteAllText(Path.Combine(Path.GetDirectoryName(sourcePath), newFile), newText, new System.Text.UTF8Encoding(false));
        }

        private void Log(string msg)
        {
            _log.SelectionStart = _log.TextLength;
            _log.SelectionLength = 0;
            _log.SelectionColor = Accent;
            _log.AppendText(DateTime.Now.ToString("HH:mm:ss") + "   ");
            _log.SelectionColor = TextMain;
            _log.AppendText(msg + Environment.NewLine);
            _log.ScrollToCaret();
        }

        private void TitleMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            ReleaseCapture();
            SendMessage(Handle, WM_NCLBUTTONDOWN, (IntPtr)HTCAPTION, 0);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x84 && WindowState == FormWindowState.Normal)
            {
                base.WndProc(ref m);
                int lp = m.LParam.ToInt32();
                int x = unchecked((short)(lp & 0xFFFF));
                int y = unchecked((short)((lp >> 16) & 0xFFFF));
                Point p = PointToClient(new Point(x, y));
                int b = 6;
                bool left = p.X <= b, right = p.X >= ClientSize.Width - b, top = p.Y <= b, bottom = p.Y >= ClientSize.Height - b;
                if (left && top) { m.Result = (IntPtr)13; return; }
                if (right && top) { m.Result = (IntPtr)14; return; }
                if (left && bottom) { m.Result = (IntPtr)16; return; }
                if (right && bottom) { m.Result = (IntPtr)17; return; }
                if (left) { m.Result = (IntPtr)10; return; }
                if (right) { m.Result = (IntPtr)11; return; }
                if (top) { m.Result = (IntPtr)12; return; }
                if (bottom) { m.Result = (IntPtr)15; return; }
                return;
            }
            base.WndProc(ref m);
        }
    }
}