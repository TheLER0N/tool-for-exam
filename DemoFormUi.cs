using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Ekzamen
{
    public partial class DemoForm
    {
        private static readonly Color BgCard = Color.FromArgb(37, 37, 38);
        private static readonly Color BgInput = Color.FromArgb(24, 24, 24);

        private void BuildUi()
        {
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(1000, 700);
            MinimumSize = new Size(900, 620);
            MaximizedBounds = Screen.PrimaryScreen.WorkingArea;
            BackColor = BgMain;
            ForeColor = TextMain;
            Font = new Font("Segoe UI", 9F);
            Text = "Панель управления формами";

            Panel header = new Panel();
            header.Dock = DockStyle.Top;
            header.Height = 96;
            header.BackColor = BgMain;
            Label title = new Label();
            title.Text = "ПАНЕЛЬ УПРАВЛЕНИЯ ФОРМАМИ";
            title.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            title.ForeColor = TextMain;
            title.AutoSize = false;
            title.Bounds = new Rectangle(28, 14, 700, 38);
            Label sub = new Label();
            sub.Text = "Тёмная тема  ·  дублирование форм  ·  .NET 4.8";
            sub.Font = new Font("Segoe UI", 9.5F);
            sub.ForeColor = TextSoft;
            sub.AutoSize = false;
            sub.Bounds = new Rectangle(28, 56, 700, 20);
            Label ver = new Label();
            ver.Text = "v1.0";
            ver.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            ver.ForeColor = Accent;
            ver.AutoSize = false;
            ver.Bounds = new Rectangle(900, 52, 72, 24);
            ver.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            ver.TextAlign = ContentAlignment.MiddleRight;
            header.Controls.Add(title);
            header.Controls.Add(sub);
            header.Controls.Add(ver);

            Panel accentLine = new Panel();
            accentLine.Dock = DockStyle.Top;
            accentLine.Height = 2;
            accentLine.BackColor = Accent;

            Panel bottom = new Panel();
            bottom.Dock = DockStyle.Bottom;
            bottom.Height = 34;
            bottom.BackColor = BgBar;
            Label sign = new Label();
            sign.Text = "by LERON";
            sign.ForeColor = Accent;
            sign.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            sign.AutoSize = false;
            sign.Bounds = new Rectangle(16, 0, 110, 34);
            sign.TextAlign = ContentAlignment.MiddleLeft;
            Label ready = new Label();
            ready.Text = "\u25CF Готово";
            ready.ForeColor = OkGreen;
            ready.AutoSize = false;
            ready.Bounds = new Rectangle(140, 0, 120, 34);
            ready.TextAlign = ContentAlignment.MiddleLeft;
            _stFound = new Label();
            _stFound.ForeColor = TextSoft;
            _stFound.AutoSize = false;
            _stFound.Bounds = new Rectangle(800, 0, 184, 34);
            _stFound.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _stFound.TextAlign = ContentAlignment.MiddleRight;
            bottom.Controls.Add(sign);
            bottom.Controls.Add(ready);
            bottom.Controls.Add(_stFound);

            Panel content = new Panel();
            content.Dock = DockStyle.Fill;
            content.Padding = new Padding(24);

            Panel cardPath = Card();
            cardPath.Dock = DockStyle.Top;
            cardPath.Height = 80;
            cardPath.Padding = new Padding(0, 4, 0, 10);
            Panel capPath = SectionCap("ПУТЬ К ПРОЕКТУ");
            Panel rowPath = new Panel();
            rowPath.Dock = DockStyle.Top;
            rowPath.Height = 34;
            rowPath.Padding = new Padding(16, 0, 16, 0);
            Label lblPath = new Label();
            lblPath.Text = "Папка проекта:";
            lblPath.ForeColor = TextMain;
            lblPath.Dock = DockStyle.Left;
            lblPath.Width = 110;
            lblPath.TextAlign = ContentAlignment.MiddleLeft;
            _btnBrowse = new Button();
            _btnBrowse.Text = "Обзор...";
            _btnBrowse.Dock = DockStyle.Right;
            _btnBrowse.Width = 110;
            StyleButton(_btnBrowse, false);
            _btnBrowse.Click += delegate { BrowseFolder(); };
            _folderText = new TextBox();
            _folderText.Name = "textBoxFolder";
            _folderText.Dock = DockStyle.Fill;
            _folderText.BackColor = BgInput;
            _folderText.ForeColor = TextMain;
            _folderText.BorderStyle = BorderStyle.FixedSingle;
            _folderText.Font = new Font("Segoe UI", 9.5F);
            rowPath.Controls.Add(_folderText);
            rowPath.Controls.Add(_btnBrowse);
            rowPath.Controls.Add(lblPath);
            cardPath.Controls.Add(rowPath);
            cardPath.Controls.Add(capPath);

            Panel mid = new Panel();
            mid.Dock = DockStyle.Top;
            mid.Height = 268;

            Panel leftCard = Card();
            leftCard.Dock = DockStyle.Left;
            leftCard.Width = 460;
            leftCard.Padding = new Padding(16, 8, 16, 12);
            Panel capForms = SectionCap("ФОРМЫ ПРОЕКТА");
            Label lblSrc = new Label();
            lblSrc.Text = "Исходная форма:";
            lblSrc.ForeColor = TextMain;
            lblSrc.Dock = DockStyle.Top;
            lblSrc.Height = 26;
            lblSrc.TextAlign = ContentAlignment.MiddleLeft;
            _formsList = new ListBox();
            _formsList.Dock = DockStyle.Fill;
            _formsList.BackColor = BgInput;
            _formsList.ForeColor = TextMain;
            _formsList.BorderStyle = BorderStyle.FixedSingle;
            _formsList.Font = new Font("Segoe UI", 9.5F);
            _formsList.ItemHeight = 28;
            _formsList.IntegralHeight = false;
            _countLabel = new Label();
            _countLabel.Dock = DockStyle.Bottom;
            _countLabel.Height = 28;
            _countLabel.ForeColor = TextSoft;
            _countLabel.TextAlign = ContentAlignment.MiddleLeft;
            leftCard.Controls.Add(_formsList);
            leftCard.Controls.Add(_countLabel);
            leftCard.Controls.Add(lblSrc);
            leftCard.Controls.Add(capForms);

            Panel gapMid = new Panel();
            gapMid.Dock = DockStyle.Left;
            gapMid.Width = 24;
            gapMid.BackColor = BgMain;

            Panel rightCard = Card();
            rightCard.Dock = DockStyle.Fill;
            rightCard.Padding = new Padding(16, 8, 16, 12);
            Panel capNew = SectionCap("НОВОЕ ИМЯ ФОРМЫ");
            Label lblName = new Label();
            lblName.Text = "Имя копии формы:";
            lblName.ForeColor = TextMain;
            lblName.Dock = DockStyle.Top;
            lblName.Height = 26;
            lblName.TextAlign = ContentAlignment.MiddleLeft;
            _nameText = new TextBox();
            _nameText.Dock = DockStyle.Top;
            _nameText.Height = 32;
            _nameText.BackColor = BgInput;
            _nameText.ForeColor = TextMain;
            _nameText.BorderStyle = BorderStyle.FixedSingle;
            _nameText.Font = new Font("Segoe UI", 9.5F);
            _nameText.Text = "MyForm";
            _nameText.TextChanged += delegate { UpdateHint(); };
            _hintLabel = new Label();
            _hintLabel.Dock = DockStyle.Top;
            _hintLabel.Height = 26;
            _hintLabel.ForeColor = TextSoft;
            _hintLabel.TextAlign = ContentAlignment.MiddleLeft;
            _btnDuplicate = new Button();
            _btnDuplicate.Text = "Дублировать форму";
            _btnDuplicate.Dock = DockStyle.Bottom;
            _btnDuplicate.Height = 42;
            StyleButton(_btnDuplicate, true);
            _btnDuplicate.Click += delegate { DuplicateForm(); };
            rightCard.Controls.Add(_btnDuplicate);
            rightCard.Controls.Add(_hintLabel);
            rightCard.Controls.Add(_nameText);
            rightCard.Controls.Add(lblName);
            rightCard.Controls.Add(capNew);

            mid.Controls.Add(rightCard);
            mid.Controls.Add(gapMid);
            mid.Controls.Add(leftCard);

            Panel cardLog = Card();
            cardLog.Dock = DockStyle.Fill;
            cardLog.Padding = new Padding(16, 8, 16, 12);
            Panel capLog = SectionCap("ЖУРНАЛ ДЕЙСТВИЙ");
            _log = new RichTextBox();
            _log.Dock = DockStyle.Fill;
            _log.BackColor = BgInput;
            _log.ForeColor = TextMain;
            _log.BorderStyle = BorderStyle.None;
            _log.ReadOnly = true;
            _log.HideSelection = false;
            _log.ScrollBars = RichTextBoxScrollBars.Vertical;
            _log.Font = new Font("Consolas", 9.5F);
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
            Controls.Add(bottom);
            Controls.Add(accentLine);
            Controls.Add(header);
            BuildTitleBar();
        }

        private void BuildTitleBar()
        {
            Panel titleBar = new Panel();
            titleBar.Dock = DockStyle.Top;
            titleBar.Height = 40;
            titleBar.BackColor = BgTitle;
            titleBar.MouseDown += TitleMouseDown;
            titleBar.MouseDoubleClick += delegate { ToggleMax(); };
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
            Label titleLabel = new Label();
            titleLabel.Text = "Панель управления формами";
            titleLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            titleLabel.ForeColor = TextMain;
            titleLabel.AutoSize = false;
            titleLabel.Bounds = new Rectangle(42, 0, 500, 40);
            titleLabel.TextAlign = ContentAlignment.MiddleLeft;
            titleLabel.MouseDown += TitleMouseDown;
            titleLabel.MouseDoubleClick += delegate { ToggleMax(); };
            Button btnClose = TitleButton("\u2715", true);
            Button btnMax = TitleButton("\u25A1", false);
            Button btnMin = TitleButton("\u2013", false);
            btnClose.Click += delegate { Close(); };
            btnMax.Click += delegate { ToggleMax(); };
            btnMin.Click += delegate { WindowState = FormWindowState.Minimized; };
            titleBar.Controls.Add(titleLabel);
            titleBar.Controls.Add(logo);
            titleBar.Controls.Add(btnMin);
            titleBar.Controls.Add(btnMax);
            titleBar.Controls.Add(btnClose);
            Controls.Add(titleBar);
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

        private Panel Spacer()
        {
            Panel p = new Panel();
            p.Dock = DockStyle.Top;
            p.Height = 20;
            p.BackColor = BgMain;
            return p;
        }

        private Panel Card()
        {
            Panel p = new Panel();
            p.BackColor = BgCard;
            p.Paint += delegate(object s, PaintEventArgs e)
            {
                using (Pen pen = new Pen(Border))
                    e.Graphics.DrawRectangle(pen, 0, 0, p.Width - 1, p.Height - 1);
            };
            return p;
        }

        private Panel SectionCap(string text)
        {
            Panel p = new Panel();
            p.Dock = DockStyle.Top;
            p.Height = 28;
            Panel bar = new Panel();
            bar.Dock = DockStyle.Left;
            bar.Width = 4;
            bar.BackColor = Accent;
            Label l = new Label();
            l.Dock = DockStyle.Fill;
            l.Text = text;
            l.ForeColor = Accent;
            l.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            l.TextAlign = ContentAlignment.MiddleLeft;
            l.Padding = new Padding(8, 0, 0, 0);
            p.Controls.Add(l);
            p.Controls.Add(bar);
            return p;
        }

        private static void StyleButton(Button b, bool primary)
        {
            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 1;
            b.Cursor = Cursors.Hand;
            b.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            b.TextAlign = ContentAlignment.MiddleCenter;
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
    }
}