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
            _nameText.Text = GenerateFreeName();
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

                private string GenerateFreeName()
        {
            string folder = _folderText.Text.Trim();
            for (int i = 1; i < 1000; i++)
            {
                string n = "NewForm" + i;
                string path = System.IO.Path.Combine(folder, n + ".cs");
                if (!System.IO.File.Exists(path) && !_forms.Exists(x => x.Name == n)) return n;
            }
            return "Form" + DateTime.Now.Ticks.ToString();
        }
private void UpdateHint()
        {
            string n = _nameText.Text.Trim();
            if (n.Length == 0) { _hintLabel.Text = "Введи имя новой формы"; return; }
            if (!Regex.IsMatch(n, @"^[A-Za-z_]\w*$")) { _hintLabel.Text = "Недопустимое имя (только латиница, цифры, _)"; return; }
            string folder = _folderText.Text.Trim();
            if (Directory.Exists(folder))
            {
                foreach (string f in Directory.GetFiles(folder, "*.cs", SearchOption.AllDirectories))
                {
                    string fn = Path.GetFileNameWithoutExtension(f);
                    if (fn.Equals(n, StringComparison.OrdinalIgnoreCase) ||
                        fn.Equals(n + ".Designer", StringComparison.OrdinalIgnoreCase) ||
                        fn.StartsWith(n + ".", StringComparison.OrdinalIgnoreCase))
                    {
                        _hintLabel.Text = "Конфликт: " + Path.GetFileName(f) + " уже существует";
                        return;
                    }
                }
            }
            _hintLabel.Text = "Файл будет создан как " + n + ".cs (+ Designer.cs если есть у оригинала)";
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
            if (src == null) { Log("Ошибка: выбери исходную форму"); MessageBox.Show(this, "Выбери форму из списка слева", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            string newName = _nameText.Text.Trim();
            if (!Regex.IsMatch(newName, @"^[A-Za-z_]\w*$")) { Log("Ошибка: недопустимое имя " + newName); MessageBox.Show(this, "Недопустимое имя формы." + Environment.NewLine + "Допустимы: латиница, цифры, _ (начинается с буквы или _).", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            string folder = _folderText.Text.Trim();
            // Проверяем конфликты ДО создания файлов (ищем все возможные целевые имена)
            string[] allCs = Directory.GetFiles(folder, "*.cs", SearchOption.AllDirectories);
            foreach (string check in allCs)
            {
                string fn = Path.GetFileNameWithoutExtension(check);
                string ext = Path.GetExtension(check);
                if (fn.Equals(src.Name, StringComparison.OrdinalIgnoreCase) ||
                    fn.Equals(src.Name + ".Designer", StringComparison.OrdinalIgnoreCase) ||
                    fn.StartsWith(src.Name + ".", StringComparison.OrdinalIgnoreCase))
                {
                    string targetFn = newName + fn.Substring(src.Name.Length);
                    string target = Path.Combine(Path.GetDirectoryName(check), targetFn + ext);
                    if (File.Exists(target))
                    {
                        Log("Конфликт: " + target + " уже существует");
                        MessageBox.Show(this, "Файл уже существует:" + Environment.NewLine + target + Environment.NewLine + Environment.NewLine + "Введи другое имя.", "Конфликт имён", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
            }
            try
            {
                var created = new List<string>();
                created.Add(DuplicatePart(src.Path, src.Name, newName));
                // Ищем все partial-части рекурсивно
                foreach (string extra in allCs)
                {
                    if (string.Equals(extra, src.Path, StringComparison.OrdinalIgnoreCase)) continue;
                    string t2;
                    try { t2 = File.ReadAllText(extra); } catch { continue; }
                    // Подходит если содержит "partial class OldName" или "class OldName"
                    bool isPartial = Regex.IsMatch(t2, @"\bpartial\s+class\s+" + Regex.Escape(src.Name) + @"\b");
                    bool isMain = Regex.IsMatch(t2, @"\bclass\s+" + Regex.Escape(src.Name) + @"\s*:\s*[^{]*\bForm\b");
                    if (!isPartial && !isMain) continue;
                    created.Add(DuplicatePart(extra, src.Name, newName));
                }
                foreach (string f in created) Log("Создан: " + f);
                Log("Дублирование: " + src.Name + " → " + newName + " выполнено (" + created.Count + " файлов)");
                RegisterInCsproj(Path.GetDirectoryName(src.Path), newName);
                var sb = new System.Text.StringBuilder();
                sb.AppendLine("Форма успешно продублирована!");
                sb.AppendLine();
                sb.AppendLine("Исходная: " + src.Name);
                sb.AppendLine("Создана:  " + newName);
                sb.AppendLine();
                sb.AppendLine("Создано файлов: " + created.Count);
                foreach (string f in created) sb.AppendLine("  • " + f);
                MessageBox.Show(this, sb.ToString(), "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ScanForms();
                for (int i = 0; i < _forms.Count; i++)
                {
                    if (_forms[i].Name == newName) { _formsList.SelectedIndex = i; break; }
                }
                _nameText.Text = GenerateFreeName();
            }
            catch (Exception ex)
            {
                Log("Ошибка дублирования: " + ex.Message);
                MessageBox.Show(this, ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string DuplicatePart(string sourcePath, string oldName, string newName)
        {
            string text = File.ReadAllText(sourcePath);
            // Замена только в объявлениях классов, не в строках/комментариях
            string newText = Regex.Replace(text,
                @"(\b(?:public|internal|private|protected)\s+(?:static\s+|sealed\s+|abstract\s+|partial\s+)*class\s+)" + Regex.Escape(oldName) + @"\b",
                "$1" + newName);
            newText = Regex.Replace(newText, @"(\bpartial\s+class\s+)" + Regex.Escape(oldName) + @"\b", "$1" + newName);
            // Замена конструктора: public OldName() -> public NewName()
            newText = Regex.Replace(newText,
                @"(\b(?:public|internal|private|protected)\s+)" + Regex.Escape(oldName) + @"\s*\(",
                "$1" + newName + "(");
            string oldFile = Path.GetFileNameWithoutExtension(sourcePath);
            string ext = Path.GetExtension(sourcePath);
            string dir = Path.GetDirectoryName(sourcePath);
            string newFile;
            if (oldFile.EndsWith(".Designer", StringComparison.OrdinalIgnoreCase))
                newFile = newName + ".Designer" + ext;
            else if (oldFile.Equals(oldName, StringComparison.OrdinalIgnoreCase))
                newFile = newName + ext;
            else
                newFile = Regex.Replace(oldFile, @"\b" + Regex.Escape(oldName) + @"\b", newName) + ext;
            string fullPath = Path.Combine(dir, newFile);
            File.WriteAllText(fullPath, newText, new System.Text.UTF8Encoding(false));
            return fullPath;
        }

            private void RegisterInCsproj(string folder, string newName)
    {
        if (string.IsNullOrEmpty(folder) || !Directory.Exists(folder)) return;
        string[] projects = Directory.GetFiles(folder, "*.csproj", SearchOption.TopDirectoryOnly);
        if (projects.Length == 0) return;
        string projPath = projects[0];
        string xml = File.ReadAllText(projPath);
        if (xml.Contains("<Project Sdk=")) return;
        if (xml.Contains("Include=\"" + newName + ".cs\"")) return;
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendLine("    <Compile Include=\"" + newName + ".cs\">");
        sb.AppendLine("      <SubType>Form</SubType>");
        sb.AppendLine("    </Compile>");
        if (File.Exists(Path.Combine(folder, newName + ".Designer.cs")))
        {
            sb.AppendLine("    <Compile Include=\"" + newName + ".Designer.cs\">");
            sb.AppendLine("      <DependentUpon>" + newName + ".cs</DependentUpon>");
            sb.AppendLine("    </Compile>");
        }
        if (File.Exists(Path.Combine(folder, newName + ".resx")))
        {
            sb.AppendLine("    <EmbeddedResource Include=\"" + newName + ".resx\">");
            sb.AppendLine("      <DependentUpon>" + newName + ".cs</DependentUpon>");
            sb.AppendLine("    </EmbeddedResource>");
        }
        int idx = xml.IndexOf("<Compile Include=\"Program.cs\"", StringComparison.OrdinalIgnoreCase);
        if (idx < 0) idx = xml.IndexOf("</ItemGroup>", StringComparison.OrdinalIgnoreCase);
        if (idx < 0) { Log("Не удалось вставить " + newName + " в .csproj"); return; }
        xml = xml.Insert(idx, sb.ToString());
        File.WriteAllText(projPath, xml, new System.Text.UTF8Encoding(false));
        Log("Форма " + newName + " добавлена в " + Path.GetFileName(projPath) + " (VS увидит окно)");
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