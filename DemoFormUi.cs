using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using SafeWinForms;

namespace Ekzamen
{
    public partial class DemoForm
    {
        private void BuildUi()
        {
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(1180, 720);
            MinimumSize = new Size(960, 600);
            MaximizedBounds = Screen.PrimaryScreen.WorkingArea;
            BackColor = BgMain;
            ForeColor = TextMain;
            Font = new Font("Segoe UI", 9F);
            Text = "Панель управления формами";

            BuildTitleBar();
            BuildMenu();
            BuildHeader();
            BuildStatus();
            BuildLeft();
            BuildContent();
            for (int i = 0; i < Controls.Count; i++) Controls.SetChildIndex(Controls[i], 0);
        }

        private void BuildTitleBar()
        {
            _titleBar = new Panel();
            _titleBar.Dock = DockStyle.Top;
            _titleBar.Height = 40;
            _titleBar.BackColor = BgTitle;
            _titleBar.MouseDown += TitleMouseDown;
            _titleBar.MouseDoubleClick += delegate { ToggleMax(); };

            Panel logo = new Panel();
            logo.Size = new Size(22, 22);
            logo.Location = new Point(12, 9);
            logo.Paint += delegate(object s, PaintEventArgs e)
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (SolidBrush br = new SolidBrush(Accent))
                {
                    e.Graphics.FillRectangle(br, 0, 0, 10, 10);
                    e.Graphics.FillRectangle(br, 12, 0, 10, 10);
                    e.Graphics.FillRectangle(br, 0, 12, 10, 10);
                    e.Graphics.FillRectangle(br, 12, 12, 10, 10);
                }
            };

            _titleLabel = new Label();
            _titleLabel.Text = "Панель управления формами";
            _titleLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            _titleLabel.ForeColor = TextMain;
            _titleLabel.AutoSize = false;
            _titleLabel.Bounds = new Rectangle(42, 0, 500, 40);
            _titleLabel.TextAlign = ContentAlignment.MiddleLeft;
            _titleLabel.MouseDown += TitleMouseDown;
            _titleLabel.MouseDoubleClick += delegate { ToggleMax(); };

            Button btnClose = TitleButton("\u2715", true);
            Button btnMax = TitleButton("\u25A1", false);
            Button btnMin = TitleButton("\u2013", false);
            btnClose.Click += delegate { Close(); };
            btnMax.Click += delegate { ToggleMax(); };
            btnMin.Click += delegate { WindowState = FormWindowState.Minimized; };

            _titleBar.Controls.Add(_titleLabel);
            _titleBar.Controls.Add(logo);
            _titleBar.Controls.Add(btnMin);
            _titleBar.Controls.Add(btnMax);
            _titleBar.Controls.Add(btnClose);
            Controls.Add(_titleBar);
        }

        private void ToggleMax()
        {
            WindowState = WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized;
        }

        private Button TitleButton(string glyph, bool close)
        {
            Button b = new Button();
            b.Text = glyph;
            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 0;
            b.Dock = DockStyle.Right;
            b.Width = 46;
            b.Height = 40;
            b.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            b.Cursor = Cursors.Hand;
            b.TabStop = false;
            if (close)
            {
                b.BackColor = Color.FromArgb(232, 17, 35);
                b.ForeColor = Color.White;
                b.FlatAppearance.MouseOverBackColor = Color.FromArgb(255, 60, 70);
            }
            else
            {
                b.BackColor = BgTitle;
                b.ForeColor = TextMain;
                b.FlatAppearance.MouseOverBackColor = Color.FromArgb(58, 58, 58);
            }
            return b;
        }

        private void BuildMenu()
        {
            MenuStrip menu = new MenuStrip();
            menu.BackColor = BgBar;
            menu.ForeColor = TextMain;
            menu.Font = new Font("Segoe UI", 9F);
            menu.Renderer = new DarkMenuRenderer();
            menu.Items.Add(MenuWithChildren("Файл", new ToolStripItem[] { Mi("Выход", null, "exit") }));
            menu.Items.Add(MenuWithChildren("Правка", new ToolStripItem[] { Mi("Копировать имя формы", null, "copy") }));
            menu.Items.Add(MenuWithChildren("Вид", new ToolStripItem[] { Mi("Обновить", IconFactory.Refresh(), "refresh") }));
            menu.Items.Add(MenuWithChildren("Инструменты", new ToolStripItem[] { Mi("Параметры", IconFactory.Gear(), "settings") }));
            menu.Items.Add(MenuWithChildren("Справка", new ToolStripItem[] { Mi("О программе", null, "about") }));
            ToolStripItem r1 = Mi("Параметры", IconFactory.Gear(), "settings"); r1.Alignment = ToolStripItemAlignment.Right; menu.Items.Add(r1);
            ToolStripItem r2 = Mi("Дублировать", IconFactory.Copy(), "dup"); r2.Alignment = ToolStripItemAlignment.Right; menu.Items.Add(r2);
            ToolStripItem r3 = Mi("Открыть", IconFactory.Folder(), "open"); r3.Alignment = ToolStripItemAlignment.Right; menu.Items.Add(r3);
            ToolStripItem r4 = Mi("Обновить", IconFactory.Refresh(), "refresh"); r4.Alignment = ToolStripItemAlignment.Right; menu.Items.Add(r4);
            MainMenuStrip = menu;
            Controls.Add(menu);
        }

        private ToolStripMenuItem Mi(string text, Image img, string tag)
        {
            ToolStripMenuItem mi = new ToolStripMenuItem(text);
            if (img != null) mi.Image = img;
            mi.Tag = tag;
            mi.Click += delegate { DoTag(tag); };
            return mi;
        }

        private ToolStripMenuItem MenuWithChildren(string text, ToolStripItem[] children)
        {
            ToolStripMenuItem mi = new ToolStripMenuItem(text);
            foreach (ToolStripItem c in children) mi.DropDownItems.Add(c);
            return mi;
        }

        private void BuildHeader()
        {
            Panel header = new Panel();
            header.Dock = DockStyle.Top;
            header.Height = 96;
            header.BackColor = BgMain;

            Label title = new Label();
            title.Text = "ПАНЕЛЬ УПРАВЛЕНИЯ ФОРМАМИ";
            title.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            title.ForeColor = TextMain;
            title.AutoSize = false;
            title.Bounds = new Rectangle(24, 12, 760, 40);

            Label sub = new Label();
            sub.Text = "Тёмная тема  ·  автопоиск форм  ·  безопасное дублирование  ·  .NET 4.8";
            sub.Font = new Font("Segoe UI", 9.5F);
            sub.ForeColor = TextSoft;
            sub.AutoSize = false;
            sub.Bounds = new Rectangle(24, 56, 760, 22);

            _search = new TextBox();
            _search.Size = new Size(300, 28);
            _search.Location = new Point(856, 16);
            _search.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _search.BackColor = BgSide;
            _search.ForeColor = TextMain;
            _search.BorderStyle = BorderStyle.FixedSingle;
            _search.Font = new Font("Segoe UI", 9.5F);
            _search.TextChanged += delegate { ApplyFilter(); };

            Label ver = new Label();
            ver.Text = "v2.4.1";
            ver.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            ver.ForeColor = Accent;
            ver.AutoSize = false;
            ver.Bounds = new Rectangle(1080, 52, 76, 26);
            ver.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            ver.TextAlign = ContentAlignment.MiddleRight;

            header.Controls.Add(title);
            header.Controls.Add(sub);
            header.Controls.Add(_search);
            header.Controls.Add(ver);

            Panel accentLine = new Panel();
            accentLine.Dock = DockStyle.Top;
            accentLine.Height = 2;
            accentLine.BackColor = Accent;

            Controls.Add(header);
            Controls.Add(accentLine);
            SendMessageStr(_search.Handle, EM_SETCUEBANNER, IntPtr.Zero, "Поиск формы по имени...");
        }

        private void BuildStatus()
        {
            Panel bottom = new Panel();
            bottom.Dock = DockStyle.Bottom;
            bottom.Height = 32;
            bottom.BackColor = BgBar;

            Label sign = new Label();
            sign.Text = "by LERON";
            sign.ForeColor = Accent;
            sign.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            sign.AutoSize = false;
            sign.Bounds = new Rectangle(14, 0, 110, 32);
            sign.TextAlign = ContentAlignment.MiddleLeft;

            Label ready = new Label();
            ready.Text = "\u25CF Готово";
            ready.ForeColor = OkGreen;
            ready.AutoSize = false;
            ready.Bounds = new Rectangle(150, 0, 110, 32);
            ready.TextAlign = ContentAlignment.MiddleLeft;

            _stOpen = new Label();
            _stOpen.ForeColor = TextSoft;
            _stOpen.AutoSize = false;
            _stOpen.Bounds = new Rectangle(280, 0, 190, 32);
            _stOpen.TextAlign = ContentAlignment.MiddleLeft;

            _stFound = new Label();
            _stFound.ForeColor = TextSoft;
            _stFound.AutoSize = false;
            _stFound.Bounds = new Rectangle(480, 0, 190, 32);
            _stFound.TextAlign = ContentAlignment.MiddleLeft;

            _stClock = new Label();
            _stClock.ForeColor = TextMain;
            _stClock.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            _stClock.AutoSize = false;
            _stClock.Bounds = new Rectangle(1080, 0, 86, 32);
            _stClock.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _stClock.TextAlign = ContentAlignment.MiddleRight;

            bottom.Controls.Add(sign);
            bottom.Controls.Add(ready);
            bottom.Controls.Add(_stOpen);
            bottom.Controls.Add(_stFound);
            bottom.Controls.Add(_stClock);
            Controls.Add(bottom);
        }

        private void BuildLeft()
        {
            Panel left = new Panel();
            left.Dock = DockStyle.Left;
            left.Width = 300;
            left.BackColor = BgSide;
            left.Padding = new Padding(14);

            Label cap = new Label();
            cap.Text = "ФОРМЫ ПРОЕКТА (АВТОПОИСК)";
            cap.ForeColor = Accent;
            cap.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            cap.Dock = DockStyle.Top;
            cap.Height = 30;
            cap.TextAlign = ContentAlignment.MiddleLeft;

            Panel tabs = new Panel();
            tabs.Dock = DockStyle.Top;
            tabs.Height = 34;
            _tabAll = new Button();
            _tabAll.Text = "Все";
            _tabAll.Location = new Point(0, 0);
            _tabAll.Size = new Size(86, 30);
            _tabOpen = new Button();
            _tabOpen.Text = "Открытые";
            _tabOpen.Location = new Point(90, 0);
            _tabOpen.Size = new Size(96, 30);
            _tabFav = new Button();
            _tabFav.Text = "Избранные";
            _tabFav.Location = new Point(190, 0);
            _tabFav.Size = new Size(82, 30);
            _tabAll.Click += delegate { SetTab(TabMode.All); };
            _tabOpen.Click += delegate { SetTab(TabMode.Open); };
            _tabFav.Click += delegate { SetTab(TabMode.Fav); };
            StyleTab(_tabAll, true);
            StyleTab(_tabOpen, false);
            StyleTab(_tabFav, false);
            tabs.Controls.Add(_tabAll);
            tabs.Controls.Add(_tabOpen);
            tabs.Controls.Add(_tabFav);

            Label colHead = new Label();
            colHead.Text = "Имя формы";
            colHead.ForeColor = TextSoft;
            colHead.Dock = DockStyle.Top;
            colHead.Height = 26;
            colHead.TextAlign = ContentAlignment.MiddleLeft;
            colHead.Padding = new Padding(6, 0, 0, 0);

            _formsList = new ListBox();
            _formsList.Dock = DockStyle.Fill;
            _formsList.BackColor = BgSide;
            _formsList.ForeColor = TextMain;
            _formsList.BorderStyle = BorderStyle.FixedSingle;
            _formsList.Font = new Font("Segoe UI", 10F);
            _formsList.ItemHeight = 30;
            _formsList.IntegralHeight = false;
            _formsList.SelectedIndexChanged += delegate { UpdateProperties(); };
            _formsList.DoubleClick += delegate { OpenSelected(); };
            ContextMenuStrip cm = new ContextMenuStrip();
            cm.Renderer = new DarkMenuRenderer();
            cm.BackColor = BgBar;
            cm.ForeColor = TextMain;
            ToolStripMenuItem favItem = new ToolStripMenuItem("В избранное / убрать");
            favItem.Click += delegate { ToggleFavorite(); };
            cm.Items.Add(favItem);
            _formsList.ContextMenuStrip = cm;

            _countLabel = new Label();
            _countLabel.Dock = DockStyle.Bottom;
            _countLabel.Height = 26;
            _countLabel.ForeColor = TextSoft;
            _countLabel.TextAlign = ContentAlignment.MiddleLeft;

            left.Controls.Add(_formsList);
            left.Controls.Add(_countLabel);
            left.Controls.Add(colHead);
            left.Controls.Add(tabs);
            left.Controls.Add(cap);
            Controls.Add(left);
        }

        private void BuildContent()
        {
            Panel content = new Panel();
            content.Dock = DockStyle.Fill;
            content.Padding = new Padding(16);

            Panel cardPath = Card();
            cardPath.Dock = DockStyle.Top;
            cardPath.Height = 112;
            Label capPath = CardCap("ПУТЬ К ПРОЕКТУ");
            Label lblPath = new Label();
            lblPath.Text = "Папка проекта (выбор через «Обзор»):";
            lblPath.ForeColor = TextSoft;
            lblPath.Dock = DockStyle.Top;
            lblPath.Height = 22;
            lblPath.Padding = new Padding(14, 0, 0, 0);
            lblPath.TextAlign = ContentAlignment.MiddleLeft;
            Panel rowPath = new Panel();
            rowPath.Dock = DockStyle.Top;
            rowPath.Height = 36;
            rowPath.Padding = new Padding(14, 0, 14, 0);
            _folderText = new TextBox();
            _folderText.Name = "textBoxFolder";
            _folderText.Dock = DockStyle.Fill;
            _folderText.BackColor = Color.FromArgb(24, 24, 24);
            _folderText.ForeColor = TextMain;
            _folderText.BorderStyle = BorderStyle.FixedSingle;
            _folderText.Font = new Font("Segoe UI", 9.5F);
            _folderText.Text = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            _btnBrowse = new Button();
            _btnBrowse.Text = "Обзор...";
            _btnBrowse.Dock = DockStyle.Right;
            _btnBrowse.Width = 110;
            _btnBrowse.Height = 30;
            StyleButton(_btnBrowse, false);
            _btnBrowse.Click += delegate { BrowseFolder(); };
            rowPath.Controls.Add(_folderText);
            rowPath.Controls.Add(_btnBrowse);
            cardPath.Controls.Add(rowPath);
            cardPath.Controls.Add(lblPath);
            cardPath.Controls.Add(capPath);

            Panel mid = new Panel();
            mid.Dock = DockStyle.Top;
            mid.Height = 216;

            Panel cardActs = Card();
            cardActs.Dock = DockStyle.Left;
            cardActs.Width = 470;
            Label capActs = CardCap("ДЕЙСТВИЯ");
            cardActs.Controls.Add(capActs);
            _btnOpen = new Button();
            _btnOpen.Text = "Открыть выбранную форму";
            _btnOpen.Bounds = new Rectangle(16, 38, 438, 40);
            _btnOpen.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            StyleButton(_btnOpen, true);
            _btnOpen.Click += delegate { OpenSelected(); };
            _btnDupSelected = new Button();
            _btnDupSelected.Text = "Дублировать выбранную (если открыта)";
            _btnDupSelected.Bounds = new Rectangle(16, 86, 438, 40);
            _btnDupSelected.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            StyleButton(_btnDupSelected, false);
            _btnDupSelected.Click += delegate { DuplicateSelected(); };
            _btnDupSelf = new Button();
            _btnDupSelf.Text = "Дублировать это окно";
            _btnDupSelf.Bounds = new Rectangle(16, 134, 438, 40);
            _btnDupSelf.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            StyleButton(_btnDupSelf, true);
            _btnDupSelf.Click += delegate { SafeFormDuplicator.OpenDuplicate(this); Log("Дублирование окна выполнено успешно"); };
            cardActs.Controls.Add(_btnOpen);
            cardActs.Controls.Add(_btnDupSelected);
            cardActs.Controls.Add(_btnDupSelf);

            Panel cardProps = Card();
            cardProps.Dock = DockStyle.Fill;
            Label capProps = CardCap("СВОЙСТВА ВЫБРАННОЙ ФОРМЫ");
            cardProps.Controls.Add(capProps);
            string[] names = new string[] { "Имя класса:", "Файл:", "Строк кода:", "Изменена:" };
            Label[] vals = new Label[4];
            for (int i = 0; i < 4; i++)
            {
                Label n = new Label();
                n.Text = names[i];
                n.ForeColor = TextSoft;
                n.AutoSize = false;
                n.Bounds = new Rectangle(16, 40 + i * 32, 130, 24);
                Label v = new Label();
                v.Text = "—";
                v.ForeColor = TextMain;
                v.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
                v.AutoSize = false;
                v.Bounds = new Rectangle(150, 40 + i * 32, 300, 24);
                v.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                cardProps.Controls.Add(n);
                cardProps.Controls.Add(v);
                vals[i] = v;
            }
            _propClass = vals[0];
            _propFile = vals[1];
            _propLines = vals[2];
            _propMod = vals[3];

            mid.Controls.Add(cardProps);
            mid.Controls.Add(cardActs);

            Panel cardLog = Card();
            cardLog.Dock = DockStyle.Fill;
            Label capLog = CardCap("ЖУРНАЛ ДЕЙСТВИЙ");
            _log = new RichTextBox();
            _log.Dock = DockStyle.Fill;
            _log.BackColor = Color.FromArgb(24, 24, 24);
            _log.ForeColor = TextMain;
            _log.BorderStyle = BorderStyle.None;
            _log.ReadOnly = true;
            _log.HideSelection = false;
            _log.ScrollBars = RichTextBoxScrollBars.Vertical;
            _log.Font = new Font("Consolas", 9.5F);
            _log.Margin = new Padding(0);
            _log.Location = new Point(14, 32);
            _log.Size = new Size(100, 100);
            _log.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            cardLog.Controls.Add(_log);
            cardLog.Controls.Add(capLog);

            Panel sp1 = Spacer();
            Panel sp2 = Spacer();

            content.Controls.Add(cardLog);
            content.Controls.Add(sp2);
            content.Controls.Add(mid);
            content.Controls.Add(sp1);
            content.Controls.Add(cardPath);
            Controls.Add(content);
        }

        private Panel Spacer()
        {
            Panel p = new Panel();
            p.Dock = DockStyle.Top;
            p.Height = 12;
            p.BackColor = BgMain;
            return p;
        }

        private Panel Card()
        {
            Panel p = new Panel();
            p.BackColor = Color.FromArgb(32, 32, 32);
            p.Paint += delegate(object s, PaintEventArgs e)
            {
                using (Pen pen = new Pen(Border))
                    e.Graphics.DrawRectangle(pen, 0, 0, p.Width - 1, p.Height - 1);
            };
            return p;
        }

        private Label CardCap(string text)
        {
            Label l = new Label();
            l.Text = text;
            l.Dock = DockStyle.Top;
            l.Height = 32;
            l.ForeColor = Accent;
            l.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            l.TextAlign = ContentAlignment.MiddleLeft;
            l.Padding = new Padding(14, 0, 0, 0);
            return l;
        }

        private static void StyleButton(Button b, bool primary)
        {
            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 1;
            b.Cursor = Cursors.Hand;
            b.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            if (primary)
            {
                b.BackColor = Primary;
                b.ForeColor = Color.White;
                b.FlatAppearance.BorderColor = Primary;
                b.FlatAppearance.MouseOverBackColor = Color.FromArgb(76, 127, 188);
            }
            else
            {
                b.BackColor = Color.FromArgb(45, 45, 48);
                b.ForeColor = TextMain;
                b.FlatAppearance.BorderColor = Border;
                b.FlatAppearance.MouseOverBackColor = Color.FromArgb(58, 58, 62);
            }
        }

        private void StyleTab(Button b, bool active)
        {
            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 1;
            b.Cursor = Cursors.Hand;
            b.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            if (active)
            {
                b.BackColor = Color.FromArgb(32, 32, 32);
                b.ForeColor = Accent;
                b.FlatAppearance.BorderColor = Accent;
            }
            else
            {
                b.BackColor = Color.FromArgb(45, 45, 48);
                b.ForeColor = TextSoft;
                b.FlatAppearance.BorderColor = Border;
            }
        }
    }

    internal static class IconFactory
    {
        private static readonly Color Ic = Color.FromArgb(220, 220, 220);

        internal static Image Refresh()
        {
            Bitmap b = new Bitmap(16, 16);
            using (Graphics g = Graphics.FromImage(b))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using (Pen p = new Pen(Ic, 1.6f))
                using (SolidBrush br = new SolidBrush(Ic))
                {
                    g.DrawArc(p, 2, 3, 12, 12, -60, 300);
                    g.FillPolygon(br, new Point[] { new Point(8, 0), new Point(14, 3), new Point(8, 6) });
                }
            }
            return b;
        }

        internal static Image Folder()
        {
            Bitmap b = new Bitmap(16, 16);
            using (Graphics g = Graphics.FromImage(b))
            using (SolidBrush br = new SolidBrush(Ic))
            {
                g.FillRectangle(br, 1, 3, 6, 3);
                g.FillRectangle(br, 1, 5, 14, 8);
            }
            return b;
        }

        internal static Image Copy()
        {
            Bitmap b = new Bitmap(16, 16);
            using (Graphics g = Graphics.FromImage(b))
            using (Pen p = new Pen(Ic, 1.4f))
            {
                g.DrawRectangle(p, 2, 2, 9, 9);
                g.DrawRectangle(p, 5, 5, 9, 9);
            }
            return b;
        }

        internal static Image Gear()
        {
            Bitmap b = new Bitmap(16, 16);
            using (Graphics g = Graphics.FromImage(b))
            using (Pen p = new Pen(Ic, 1.6f))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.DrawEllipse(p, 4, 4, 8, 8);
                for (int i = 0; i < 8; i++)
                {
                    double a = i * Math.PI / 4.0;
                    g.DrawLine(p, 8 + (float)Math.Cos(a) * 6, 8 + (float)Math.Sin(a) * 6, 8 + (float)Math.Cos(a) * 8, 8 + (float)Math.Sin(a) * 8);
                }
            }
            return b;
        }
    }

    internal class DarkMenuRenderer : ToolStripProfessionalRenderer
    {
        public DarkMenuRenderer() : base(new DarkColorTable()) { }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e) { }
    }

    internal class DarkColorTable : ProfessionalColorTable
    {
        public override Color MenuStripGradientBegin { get { return DemoForm.BgBar; } }
        public override Color MenuStripGradientEnd { get { return DemoForm.BgBar; } }
        public override Color MenuItemSelected { get { return Color.FromArgb(58, 58, 62); } }
        public override Color MenuItemSelectedGradientBegin { get { return Color.FromArgb(58, 58, 62); } }
        public override Color MenuItemSelectedGradientEnd { get { return Color.FromArgb(58, 58, 62); } }
        public override Color MenuItemBorder { get { return DemoForm.Accent; } }
        public override Color ImageMarginGradientBegin { get { return DemoForm.BgBar; } }
        public override Color ImageMarginGradientMiddle { get { return DemoForm.BgBar; } }
        public override Color ImageMarginGradientEnd { get { return DemoForm.BgBar; } }
        public override Color ToolStripDropDownBackground { get { return DemoForm.BgBar; } }
    }
}
