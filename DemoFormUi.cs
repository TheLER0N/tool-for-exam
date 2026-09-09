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
private static readonly Color Thumb = Color.FromArgb(100, 100, 108);
private Panel _logBar;
private bool _barDrag;
private int _barDragStartY;
private int _barDragStartFirst;
private LogScrollHook _logHook;
private sealed class LogScrollHook : NativeWindow
{
public Action OnScroll;
protected override void WndProc(ref Message m)
{
base.WndProc(ref m);
if (m.Msg == 0x020A || m.Msg == 0x00B6 || m.Msg == 0x00B5)
{
if (OnScroll != null) OnScroll();
}
}
}
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
ver.Text = "v1.1";
ver.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
ver.ForeColor = Accent;
ver.Dock = DockStyle.Right;
ver.Width = 90;
ver.TextAlign = ContentAlignment.MiddleRight;
ver.Padding = new Padding(0, 0, 24, 0);
header.Controls.Add(ver);
header.Controls.Add(title);
header.Controls.Add(sub);
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
sign.Dock = DockStyle.Left;
sign.Width = 110;
sign.TextAlign = ContentAlignment.MiddleLeft;
sign.Padding = new Padding(16, 0, 0, 0);
Label ready = new Label();
ready.Text = "\u25CF Готово";
ready.ForeColor = OkGreen;
ready.Dock = DockStyle.Left;
ready.Width = 120;
ready.TextAlign = ContentAlignment.MiddleLeft;
_stFound = new Label();
_stFound.ForeColor = TextSoft;
_stFound.Dock = DockStyle.Right;
_stFound.Width = 200;
_stFound.TextAlign = ContentAlignment.MiddleRight;
_stFound.Padding = new Padding(0, 0, 16, 0);
bottom.Controls.Add(_stFound);
_btnUndo = new Button();
_btnUndo.Text = "Отменить (Ctrl+Z)";
_btnUndo.Dock = DockStyle.Right;
_btnUndo.Width = 130;
_btnUndo.FlatStyle = FlatStyle.Flat;
_btnUndo.FlatAppearance.BorderSize = 0;
_btnUndo.ForeColor = TextSoft;
_btnUndo.BackColor = BgBar;
_btnUndo.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
_btnUndo.Click += delegate { UndoLastDuplicate(); };
_btnDelete = new Button();
_btnDelete.Text = "Удалить форму";
_btnDelete.Dock = DockStyle.Right;
_btnDelete.Width = 120;
_btnDelete.FlatStyle = FlatStyle.Flat;
_btnDelete.FlatAppearance.BorderSize = 0;
_btnDelete.ForeColor = TextSoft;
_btnDelete.BackColor = BgBar;
_btnDelete.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
_btnDelete.Click += delegate { DeleteSelectedForm(); };
bottom.Controls.Add(_btnDelete);
bottom.Controls.Add(_btnUndo);
bottom.Controls.Add(ready);
bottom.Controls.Add(sign);
Panel content = new Panel();
content.Dock = DockStyle.Fill;
content.Padding = new Padding(24);
Panel cardPath = Card();
cardPath.Dock = DockStyle.Top;
cardPath.Height = 86;
cardPath.Padding = new Padding(12, 6, 12, 10);
Panel capPath = SectionCap("ПУТЬ К ПРОЕКТУ");
Panel rowPath = new Panel();
rowPath.Dock = DockStyle.Top;
rowPath.Height = 40;
rowPath.Padding = new Padding(0, 6, 0, 6);
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
mid.Height = 240;
Panel leftCard = Card();
leftCard.Dock = DockStyle.Left;
leftCard.Width = 460;
leftCard.Padding = new Padding(12, 6, 12, 10);
Panel capForms = SectionCap("ФОРМЫ ПРОЕКТА");
Label lblSrc = new Label();
lblSrc.Text = "Исходная форма:";
lblSrc.ForeColor = TextMain;
lblSrc.Dock = DockStyle.Top;
lblSrc.Height = 24;
lblSrc.TextAlign = ContentAlignment.MiddleLeft;
_formsList = new ListBox();
_formsList.Dock = DockStyle.Fill;
_formsList.BackColor = BgInput;
_formsList.ForeColor = TextMain;
_formsList.BorderStyle = BorderStyle.FixedSingle;
_formsList.Font = new Font("Segoe UI", 9.5F);
_formsList.ItemHeight = 26;
_formsList.IntegralHeight = false;
_countLabel = new Label();
_countLabel.Dock = DockStyle.Bottom;
_countLabel.Height = 26;
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
rightCard.Padding = new Padding(12, 6, 12, 10);
Panel capNew = SectionCap("НОВОЕ ИМЯ ФОРМЫ");
Label lblName = new Label();
lblName.Text = "Имя копии формы:";
lblName.ForeColor = TextMain;
lblName.Dock = DockStyle.Top;
lblName.Height = 24;
lblName.TextAlign = ContentAlignment.MiddleLeft;
_nameText = new TextBox();
_nameText.Dock = DockStyle.Top;
_nameText.Height = 30;
_nameText.BackColor = BgInput;
_nameText.ForeColor = TextMain;
_nameText.BorderStyle = BorderStyle.FixedSingle;
_nameText.Font = new Font("Segoe UI", 9.5F);
_nameText.Text = "MyForm";
_nameText.TextChanged += delegate { UpdateHint(); };
_hintLabel = new Label();
_hintLabel.Dock = DockStyle.Top;
_hintLabel.Height = 24;
_hintLabel.ForeColor = TextSoft;
_hintLabel.TextAlign = ContentAlignment.MiddleLeft;
_btnDuplicate = new Button();
_btnDuplicate.Text = "Дублировать форму";
_btnDuplicate.Dock = DockStyle.Bottom;
_btnDuplicate.Height = 42;
StyleButton(_btnDuplicate, true);
_btnDuplicate.Click += delegate { DuplicateForm(); };
_btnDuplicateTo = new Button();
_btnDuplicateTo.Text = "Дублировать в...";
_btnDuplicateTo.Dock = DockStyle.Bottom;
_btnDuplicateTo.Height = 42;
StyleButton(_btnDuplicateTo, false);
_btnDuplicateTo.Click += delegate { DuplicateToFolder(); };
rightCard.Controls.Add(_btnDuplicateTo);
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
cardLog.Padding = new Padding(12, 6, 12, 10);
Panel capLog = SectionCap("ЖУРНАЛ ДЕЙСТВИЙ");
_log = new RichTextBox();
_log.Dock = DockStyle.Fill;
_log.BackColor = BgInput;
_log.ForeColor = TextMain;
_log.BorderStyle = BorderStyle.None;
_log.ReadOnly = true;
_log.HideSelection = false;
_log.ScrollBars = RichTextBoxScrollBars.None;
_log.Font = new Font("Consolas", 9.5F);
_logBar = new Panel();
_logBar.Dock = DockStyle.Right;
_logBar.Width = 12;
_logBar.BackColor = BgInput;
_logBar.Paint += LogBarPaint;
_logBar.MouseDown += LogBarDown;
_logBar.MouseMove += LogBarMove;
_logBar.MouseUp += LogBarUp;
_log.TextChanged += delegate { _logBar.Invalidate(); };
_logHook = new LogScrollHook();
_logHook.OnScroll = delegate { _logBar.Invalidate(); };
_logHook.AssignHandle(_log.Handle);
cardLog.Controls.Add(_log);
cardLog.Controls.Add(_logBar);
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
private int LogTotalLines() { return SendMessage(_log.Handle, 0x00BA, IntPtr.Zero, 0).ToInt32(); }
private int LogFirstLine() { return SendMessage(_log.Handle, 0x00CE, IntPtr.Zero, 0).ToInt32(); }
private int LogVisibleLines()
{
int h = _log.Font.Height;
return h > 0 ? _log.ClientSize.Height / h : 1;
}
private void LogBarPaint(object sender, PaintEventArgs e)
{
int total = LogTotalLines();
int vis = LogVisibleLines();
if (total <= vis) return;
int barH = _logBar.ClientSize.Height;
int thumbH = Math.Max(24, barH * vis / total);
int range = barH - thumbH;
if (range <= 0) return;
int y = range * LogFirstLine() / (total - vis);
using (SolidBrush br = new SolidBrush(Thumb))
e.Graphics.FillRectangle(br, 3, y, 6, thumbH);
}
private void LogBarDown(object sender, MouseEventArgs e)
{
_barDrag = true;
_barDragStartY = e.Y;
_barDragStartFirst = LogFirstLine();
}
private void LogBarMove(object sender, MouseEventArgs e)
{
if (!_barDrag) return;
int total = LogTotalLines();
int vis = LogVisibleLines();
if (total <= vis) return;
int barH = _logBar.ClientSize.Height;
int thumbH = Math.Max(24, barH * vis / total);
int range = barH - thumbH;
if (range <= 0) return;
int target = _barDragStartFirst + (e.Y - _barDragStartY) * (total - vis) / range;
if (target < 0) target = 0;
if (target > total - vis) target = total - vis;
int cur = LogFirstLine();
if (target != cur)
{
SendMessage(_log.Handle, 0x00B6, IntPtr.Zero, target - cur);
_logBar.Invalidate();
}
}
private void LogBarUp(object sender, MouseEventArgs e) { _barDrag = false; }
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
p.Height = 16;
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