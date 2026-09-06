using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SafeWinForms;

namespace Ekzamen
{
    public partial class DemoForm : Form
    {
        public static readonly Color BgMain = Color.FromArgb(30, 30, 30);
        public static readonly Color BgSide = Color.FromArgb(37, 37, 38);
        public static readonly Color BgBar = Color.FromArgb(36, 36, 36);
        public static readonly Color BgTitle = Color.FromArgb(25, 25, 25);
        public static readonly Color Border = Color.FromArgb(63, 63, 70);
        public static readonly Color TextMain = Color.FromArgb(232, 232, 232);
        public static readonly Color TextSoft = Color.FromArgb(157, 157, 157);
        public static readonly Color Accent = Color.FromArgb(224, 166, 43);
        public static readonly Color Primary = Color.FromArgb(61, 110, 168);
        public static readonly Color OkGreen = Color.FromArgb(78, 201, 76);

        private enum TabMode { All, Open, Fav }

        private readonly List<Type> _formTypes = new List<Type>();
        private readonly HashSet<string> _favorites = new HashSet<string>();
        private TabMode _tab = TabMode.All;
        private bool _projectConfirmed = false;

        private Panel _titleBar;
        private Label _titleLabel;
        private TextBox _search;
        private ListBox _formsList;
        private Label _countLabel;
        private Button _tabAll;
        private Button _tabOpen;
        private Button _tabFav;
        private TextBox _folderText;
        private Button _btnBrowse;
        private Button _btnOpen;
        private Button _btnDupSelected;
        private Button _btnDupSelf;
        private Label _propClass;
        private Label _propFile;
        private Label _propLines;
        private Label _propMod;
        private RichTextBox _log;
        private Label _stOpen;
        private Label _stFound;
        private Label _stClock;
        private Timer _timer;

        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, int lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "SendMessageW")]
        private static extern IntPtr SendMessageStr(IntPtr hWnd, int msg, IntPtr wParam, string lParam);

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 2;
        private const int EM_SETCUEBANNER = 0x1501;

        public DemoForm()
        {
            BuildUi();
            LoadFavorites();
            Log("Панель запущена, тема: тёмная");
            ScanForms();
            Log("Автопоиск: найдено " + _formTypes.Count.ToString() + " форм в проекте");
            _timer = new Timer();
            _timer.Interval = 1000;
            _timer.Tick += (s, e) => TickStatus();
            _timer.Start();
        }

        private string FavPath { get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "favorites.txt"); } }

        private void LoadFavorites()
        {
            try
            {
                if (File.Exists(FavPath))
                    foreach (string line in File.ReadAllLines(FavPath))
                        if (line.Trim().Length > 0) _favorites.Add(line.Trim());
            }
            catch { }
        }

        private void SaveFavorites()
        {
            try { File.WriteAllLines(FavPath, new List<string>(_favorites)); } catch { }
        }

        private void ScanForms()
        {
            _formTypes.Clear();
            try
            {
                foreach (Type t in Assembly.GetExecutingAssembly().GetTypes())
                    if (t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(Form)) && t.GetConstructor(Type.EmptyTypes) != null)
                        _formTypes.Add(t);
            }
            catch (ReflectionTypeLoadException ex)
            {
                foreach (Type t in ex.Types)
                    if (t != null && t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(Form)) && t.GetConstructor(Type.EmptyTypes) != null)
                        _formTypes.Add(t);
            }
            _formTypes.Sort(delegate(Type a, Type b) { return string.Compare(a.Name, b.Name, StringComparison.Ordinal); });
            ApplyFilter();
            TickStatus();
        }

        private static bool IsOpen(Type t)
        {
            foreach (Form f in Application.OpenForms) if (f != null && f.GetType() == t) return true;
            return false;
        }

        private void ApplyFilter()
        {
            _formsList.Items.Clear();
            string q = _search.Text.Trim().ToLowerInvariant();
            foreach (Type t in _formTypes)
            {
                if (q.Length > 0 && t.Name.ToLowerInvariant().IndexOf(q, StringComparison.Ordinal) < 0) continue;
                if (_tab == TabMode.Open && !IsOpen(t)) continue;
                if (_tab == TabMode.Fav && !_favorites.Contains(t.Name)) continue;
                _formsList.Items.Add(t.Name);
            }
            if (_formsList.Items.Count > 0 && _formsList.SelectedIndex < 0) _formsList.SelectedIndex = 0;
            _countLabel.Text = "Найдено форм:  " + _formTypes.Count.ToString();
            _stFound.Text = "Форм найдено:  " + _formTypes.Count.ToString();
            UpdateProperties();
        }

        private Type SelectedType()
        {
            string name = _formsList.SelectedItem as string;
            if (name == null) return null;
            foreach (Type t in _formTypes) if (t.Name == name) return t;
            return null;
        }

        private FileInfo FindSource(string fileName)
        {
            string folder = _folderText.Text.Trim();
            if (folder.Length > 0)
            {
                string p = Path.Combine(folder, fileName);
                if (File.Exists(p)) return new FileInfo(p);
            }
            DirectoryInfo dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            for (int i = 0; i < 5 && dir != null; i++)
            {
                string p = Path.Combine(dir.FullName, fileName);
                if (File.Exists(p)) return new FileInfo(p);
                dir = dir.Parent;
            }
            return null;
        }

        private void UpdateProperties()
        {
            Type t = SelectedType();
            if (t == null) { _propClass.Text = "—"; _propFile.Text = "—"; _propLines.Text = "—"; _propMod.Text = "—"; return; }
            _propClass.Text = t.Name;
            _propFile.Text = t.Name + ".cs";
            FileInfo fi = FindSource(t.Name + ".cs");
            if (fi != null)
            {
                int lines = 0;
                try { lines = File.ReadAllLines(fi.FullName).Length; } catch { }
                _propLines.Text = lines.ToString();
                _propMod.Text = fi.LastWriteTime.ToString("dd.MM.yyyy HH:mm");
            }
            else { _propLines.Text = "—"; _propMod.Text = "—"; }
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

        private void TickStatus()
        {
            _stClock.Text = DateTime.Now.ToString("HH:mm:ss");
            _stOpen.Text = "Открытых окон:  " + Application.OpenForms.Count.ToString();
        }

                private bool EnsureProjectFolder()
        {
            if (_projectConfirmed && Directory.Exists(_folderText.Text.Trim())) return true;
            using (FolderBrowserDialog d = new FolderBrowserDialog())
            {
                d.Description = "Укажи расположение проекта (где искать файлы)";
                d.ShowNewFolderButton = false;
                string cur = _folderText.Text.Trim();
                d.SelectedPath = Directory.Exists(cur) ? cur : Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                if (d.ShowDialog(this) != DialogResult.OK)
                {
                    Log("Отмена: расположение проекта не указано");
                    return false;
                }
                _folderText.Text = d.SelectedPath;
                _projectConfirmed = true;
                UpdateProperties();
                Log("Путь проекта задан: " + d.SelectedPath);
                return true;
            }
        }
private void OpenSelected()
        {
            Type t = SelectedType();
            if (t == null) return;
            if (!EnsureProjectFolder()) return;
            Form f = (Form)Activator.CreateInstance(t);
            f.Show();
            Log("Форма " + t.Name + " открыта");
            TickStatus();
        }

        private void DuplicateSelected()
        {
            Type t = SelectedType();
            if (t == null) return;
            Form open = null;
            foreach (Form f in Application.OpenForms) if (f != null && f.GetType() == t) { open = f; break; }
            if (open != null) { SafeFormDuplicator.OpenDuplicate(open); Log("Дублирование окна " + t.Name + " выполнено успешно"); }
            else { Form f2 = (Form)Activator.CreateInstance(t); f2.Show(); Log("Форма " + t.Name + " не была открыта — открыта новая"); }
            TickStatus();
        }

        private void BrowseFolder()
        {
            using (FolderBrowserDialog d = new FolderBrowserDialog())
            {
                d.Description = "Выбери папку проекта";
                d.ShowNewFolderButton = true;
                string cur = _folderText.Text.Trim();
                d.SelectedPath = Directory.Exists(cur) ? cur : Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                if (d.ShowDialog(this) == DialogResult.OK)
                {
                    _folderText.Text = d.SelectedPath;
                _projectConfirmed = true;
                    UpdateProperties();
                    Log("Выбрана папка: " + d.SelectedPath);
                }
            }
        }

        private void SetTab(TabMode m)
        {
            _tab = m;
            StyleTab(_tabAll, m == TabMode.All);
            StyleTab(_tabOpen, m == TabMode.Open);
            StyleTab(_tabFav, m == TabMode.Fav);
            _formsList.SelectedIndex = -1;
            ApplyFilter();
        }

        private void ToggleFavorite()
        {
            string name = _formsList.SelectedItem as string;
            if (name == null) return;
            if (_favorites.Contains(name)) { _favorites.Remove(name); Log("Убрано из избранного: " + name); }
            else { _favorites.Add(name); Log("Добавлено в избранное: " + name); }
            SaveFavorites();
            if (_tab == TabMode.Fav) ApplyFilter();
        }

        private void DoTag(string tag)
        {
            if (tag == "open") OpenSelected();
            else if (tag == "dup") DuplicateSelected();
            else if (tag == "exit") Close();
            else if (tag == "copy") { Type tc = SelectedType(); if (tc != null) Clipboard.SetText(tc.Name); }
            else if (tag == "refresh") { ScanForms(); Log("Список форм обновлён"); }
            else if (tag == "settings") MessageBox.Show(this, "Панель управления формами v2.4.1\nТема: тёмная\nАвтопоиск и безопасное дублирование\nby LERON", "Параметры", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else if (tag == "about") MessageBox.Show(this, "Панель управления формами v2.4.1\n.NET 4.8 · WinForms\nby LERON", "Справка", MessageBoxButtons.OK, MessageBoxIcon.Information);
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