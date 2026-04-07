using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        // ── Renkler ──────────────────────────────────────────────
        private readonly Color BgDeep = Color.FromArgb(13, 15, 20);
        private readonly Color BgPanel = Color.FromArgb(19, 22, 30);
        private readonly Color BgCard = Color.FromArgb(24, 28, 39);
        private readonly Color BgHover = Color.FromArgb(30, 35, 54);
        private readonly Color Accent = Color.FromArgb(0, 212, 170);
        private readonly Color AccentDim = Color.FromArgb(0, 179, 143);
        private readonly Color Gold = Color.FromArgb(240, 165, 0);
        private readonly Color Red = Color.FromArgb(255, 77, 106);
        private readonly Color Blue = Color.FromArgb(77, 159, 255);
        private readonly Color TextPrimary = Color.FromArgb(232, 236, 245);
        private readonly Color TextSecondary = Color.FromArgb(122, 130, 153);
        private readonly Color TextMuted = Color.FromArgb(69, 77, 102);

        // ── State ─────────────────────────────────────────────────
        private string selectedTF = "H1";
        private string profileType = "Session";
        private bool showPOC = true;
        private bool showVA = true;
        private bool splitMode = false;
        private double livePrice = 1.08547;
        private double priceChange = 0.0012;
        private string activeNav = "Volume Profile";
        private readonly Random rng = new Random();
        private System.Windows.Forms.Timer priceTimer;

        // ── VP Seviyeleri ─────────────────────────────────────────
        private readonly List<VPLevel> vpLevels = new List<VPLevel>
        {
            new VPLevel { Price = 1.08680, Volume = 28400, BuyPct = 62, Type = "VAH" },
            new VPLevel { Price = 1.08620, Volume = 41200, BuyPct = 58, Type = ""    },
            new VPLevel { Price = 1.08560, Volume = 52800, BuyPct = 54, Type = ""    },
            new VPLevel { Price = 1.08520, Volume = 68900, BuyPct = 51, Type = "POC" },
            new VPLevel { Price = 1.08470, Volume = 47300, BuyPct = 47, Type = ""    },
            new VPLevel { Price = 1.08400, Volume = 33100, BuyPct = 43, Type = ""    },
            new VPLevel { Price = 1.08340, Volume = 22600, BuyPct = 39, Type = "VAL" },
        };

        // ── Kontroller ────────────────────────────────────────────
        private Panel pnlTitleBar, pnlSidebar, pnlMain, pnlHeader, pnlContent, pnlStatusBar;
        private Label lblLivePrice, lblPriceChange, lblStatus, lblClock;
        private Panel pnlChart;
        private Button btnApply, btnReset;
        private ListView lvLevels;
        private Panel pnlKeyLevels, pnlSettings, pnlSession;
        private CheckBox chkPOC, chkVA, chkSplit;

        public Form1()
        {
            // InitializeComponent() YOK - tamamen kod ile yapılıyor
            BuildUI();
            StartPriceTicker();
        }

        // ─────────────────────────────────────────────────────────
        //  UI İnşa
        // ─────────────────────────────────────────────────────────
        private void BuildUI()
        {
            this.Text = "AutoScripts · Volume Profile MT5";
            this.Size = new Size(1100, 700);
            this.MinimumSize = new Size(900, 600);
            this.BackColor = BgDeep;
            this.ForeColor = TextPrimary;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 9f);
            this.DoubleBuffered = true;
            this.FormBorderStyle = FormBorderStyle.None;

            BuildTitleBar();
            BuildSidebar();
            BuildMain();
            BuildStatusBar();
            this.Resize += (s, e) => RelayoutMain();
            RelayoutMain();
        }

        // ── TitleBar ──────────────────────────────────────────────
        private void BuildTitleBar()
        {
            pnlTitleBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 46,
                BackColor = BgPanel,
            };
            pnlTitleBar.Paint += TitleBar_Paint;
            pnlTitleBar.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    NativeMethods.ReleaseCapture();
                    NativeMethods.SendMessage(this.Handle, 0xA1, (IntPtr)0x2, IntPtr.Zero);
                }
            };

            var btnClose = MakeWinBtn(Color.FromArgb(255, 95, 86));
            var btnMin = MakeWinBtn(Color.FromArgb(255, 189, 46));
            var btnMax = MakeWinBtn(Color.FromArgb(39, 201, 63));

            btnClose.Click += (s, e) => this.Close();
            btnMin.Click += (s, e) => this.WindowState = FormWindowState.Minimized;
            btnMax.Click += (s, e) => this.WindowState =
                this.WindowState == FormWindowState.Maximized
                    ? FormWindowState.Normal
                    : FormWindowState.Maximized;

            pnlTitleBar.Controls.AddRange(new Control[] { btnMin, btnMax, btnClose });

            void PositionWinBtns()
            {
                btnClose.Location = new Point(pnlTitleBar.Width - 28, 17);
                btnMax.Location = new Point(pnlTitleBar.Width - 48, 17);
                btnMin.Location = new Point(pnlTitleBar.Width - 68, 17);
            }
            PositionWinBtns();
            pnlTitleBar.Resize += (s, e) => PositionWinBtns();

            this.Controls.Add(pnlTitleBar);
        }

        private void TitleBar_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (var br = new SolidBrush(Accent))
                g.FillEllipse(br, 16, 19, 8, 8);
            using (var f = new Font("Segoe UI", 11f, FontStyle.Bold))
            using (var br = new SolidBrush(TextPrimary))
                g.DrawString("AUTOSCRIPTS", f, br, 32, 14);
            using (var f = new Font("Consolas", 8f))
            using (var br = new SolidBrush(TextMuted))
                g.DrawString("MT5 · Volume Profile", f, br, 34, 30);

            int pw = pnlTitleBar.Width;
            var pill = new Rectangle(pw - 240, 13, 158, 20);
            using (var br = new SolidBrush(Color.FromArgb(30, 0, 212, 170)))
                g.FillRectangle(br, pill);
            using (var pen = new Pen(Color.FromArgb(80, 0, 212, 170)))
                g.DrawRectangle(pen, pill);
            using (var br = new SolidBrush(Accent))
                g.FillEllipse(br, pill.X + 6, pill.Y + 7, 6, 6);
            using (var f = new Font("Consolas", 8f, FontStyle.Bold))
            using (var br = new SolidBrush(Accent))
                g.DrawString("LIVE · MetaQuotes-Demo", f, br, pill.X + 16, pill.Y + 4);

            using (var pen = new Pen(Color.FromArgb(25, 255, 255, 255)))
                g.DrawLine(pen, 0, pnlTitleBar.Height - 1, pnlTitleBar.Width, pnlTitleBar.Height - 1);
        }

        private Panel MakeWinBtn(Color col)
        {
            var p = new Panel { Size = new Size(12, 12), BackColor = col, Cursor = Cursors.Hand };
            p.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var br = new SolidBrush(p.BackColor))
                    e.Graphics.FillEllipse(br, 0, 0, 11, 11);
            };
            p.MouseEnter += (s, e) => p.BackColor = ControlPaint.Light(col, 0.3f);
            p.MouseLeave += (s, e) => p.BackColor = col;
            return p;
        }

        // ── Sidebar ───────────────────────────────────────────────
        private void BuildSidebar()
        {
            pnlSidebar = new Panel { BackColor = BgPanel };
            pnlSidebar.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(25, 255, 255, 255)))
                    e.Graphics.DrawLine(pen, pnlSidebar.Width - 1, 0,
                                             pnlSidebar.Width - 1, pnlSidebar.Height);
            };

            var items = new[]
            {
                new[]{ "MENU",       "" },
                new[]{ "▤",          "Library" },
                new[]{ "⌂",          "Home" },
                new[]{ "◈",          "Dashboard" },
                new[]{ "◎",          "Events" },
                new[]{ "INDICATORS", "" },
                new[]{ "▦",          "Volume Profile" },
                new[]{ "◧",          "Smart Money" },
                new[]{ "◫",          "Supply/Demand" },
                new[]{ "◻",          "Order Blocks" },
                new[]{ "SYSTEM",     "" },
                new[]{ "ℹ",          "About" },
                new[]{ "✉",          "Contact" },
            };

            int y = 12;
            foreach (var item in items)
            {
                string icon = item[0];
                string label = item[1];

                if (label == "")
                {
                    pnlSidebar.Controls.Add(new Label
                    {
                        Text = icon,
                        Location = new Point(14, y),
                        Size = new Size(172, 18),
                        ForeColor = TextMuted,
                        Font = new Font("Consolas", 7.5f, FontStyle.Bold),
                        BackColor = Color.Transparent,
                    });
                    y += 22;
                }
                else
                {
                    string navName = label;
                    bool isActive = navName == activeNav;

                    var nav = new Panel
                    {
                        Location = new Point(0, y),
                        Size = new Size(200, 32),
                        BackColor = isActive ? Color.FromArgb(18, 0, 212, 170) : Color.Transparent,
                        Cursor = Cursors.Hand,
                        Tag = navName,
                    };
                    nav.Paint += (s, e) =>
                    {
                        if ((string)nav.Tag == activeNav)
                            using (var pen = new Pen(Accent, 2))
                                e.Graphics.DrawLine(pen, 0, 0, 0, nav.Height);
                    };
                    nav.MouseEnter += (s, e) =>
                    {
                        if ((string)nav.Tag != activeNav)
                            nav.BackColor = BgHover;
                    };
                    nav.MouseLeave += (s, e) =>
                    {
                        nav.BackColor = (string)nav.Tag == activeNav
                            ? Color.FromArgb(18, 0, 212, 170)
                            : Color.Transparent;
                    };
                    nav.Click += (s, e) => SetActiveNav((string)nav.Tag);

                    var icLbl = new Label
                    {
                        Text = icon,
                        Location = new Point(14, 8),
                        Size = new Size(16, 16),
                        ForeColor = isActive ? Accent : TextSecondary,
                        Font = new Font("Segoe UI", 9f),
                        BackColor = Color.Transparent,
                        Tag = navName + "_ic",
                    };
                    var txtLbl = new Label
                    {
                        Text = navName,
                        Location = new Point(34, 8),
                        Size = new Size(130, 16),
                        ForeColor = isActive ? Accent : TextSecondary,
                        Font = new Font("Segoe UI", 9f),
                        BackColor = Color.Transparent,
                        Tag = navName + "_txt",
                    };
                    icLbl.Click += (s, e) => SetActiveNav(navName);
                    txtLbl.Click += (s, e) => SetActiveNav(navName);
                    nav.Controls.AddRange(new Control[] { icLbl, txtLbl });

                    if (navName == "Volume Profile")
                    {
                        var badge = new Label
                        {
                            Text = "HOT",
                            Location = new Point(150, 9),
                            Size = new Size(34, 14),
                            BackColor = Gold,
                            ForeColor = Color.Black,
                            Font = new Font("Consolas", 7f, FontStyle.Bold),
                            TextAlign = ContentAlignment.MiddleCenter,
                        };
                        badge.Click += (s, e) => SetActiveNav(navName);
                        nav.Controls.Add(badge);
                    }

                    pnlSidebar.Controls.Add(nav);
                    y += 34;
                }
            }

            // footer
            var footer = new Panel { Size = new Size(200, 52), BackColor = Color.Transparent };
            footer.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using (var pen = new Pen(Color.FromArgb(25, 255, 255, 255)))
                    g.DrawLine(pen, 0, 0, 200, 0);
                using (var br = new SolidBrush(Accent))
                    g.FillEllipse(br, 14, 18, 6, 6);
                using (var f = new Font("Consolas", 8f, FontStyle.Bold))
                using (var br = new SolidBrush(Accent))
                    g.DrawString("CONNECTED", f, br, 26, 14);
                using (var f = new Font("Consolas", 7.5f))
                using (var br = new SolidBrush(TextMuted))
                    g.DrawString("12345678 · Demo", f, br, 26, 28);
            };
            pnlSidebar.Controls.Add(footer);
            pnlSidebar.Resize += (s, e) =>
                footer.Location = new Point(0, pnlSidebar.Height - 52);

            this.Controls.Add(pnlSidebar);
        }

        private void SetActiveNav(string navName)
        {
            activeNav = navName;
            foreach (Control c in pnlSidebar.Controls)
            {
                if (!(c is Panel nav) || !(nav.Tag is string tag)) continue;
                bool active = tag == navName;
                nav.BackColor = active ? Color.FromArgb(18, 0, 212, 170) : Color.Transparent;
                nav.Invalidate();
                foreach (Control child in nav.Controls)
                {
                    if (!(child is Label lbl) || lbl.Text == "HOT") continue;
                    bool isThis = lbl.Tag is string lt &&
                                  (lt == navName + "_ic" || lt == navName + "_txt");
                    lbl.ForeColor = isThis ? Accent : TextSecondary;
                }
            }
        }

        // ── Main ──────────────────────────────────────────────────
        private void BuildMain()
        {
            pnlMain = new Panel { BackColor = BgDeep };
            BuildMainHeader();
            BuildContentArea();
            this.Controls.Add(pnlMain);
        }

        private void BuildMainHeader()
        {
            pnlHeader = new Panel { Dock = DockStyle.Top, Height = 56, BackColor = BgPanel };
            pnlHeader.Paint += Header_Paint;

            string[] tfs = { "M1", "M5", "M15", "H1", "H4", "D1" };
            int tx = 310;
            foreach (var tfStr in tfs)
            {
                string tf = tfStr;
                var b = new Button
                {
                    Text = tf,
                    Location = new Point(tx, 17),
                    Size = new Size(34, 20),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = tf == selectedTF ? Color.FromArgb(20, 0, 212, 170) : Color.Transparent,
                    ForeColor = tf == selectedTF ? Accent : TextSecondary,
                    Font = new Font("Consolas", 8f, FontStyle.Bold),
                    Cursor = Cursors.Hand,
                };
                b.FlatAppearance.BorderColor = tf == selectedTF
                    ? Color.FromArgb(80, 0, 212, 170)
                    : Color.FromArgb(25, 255, 255, 255);
                b.FlatAppearance.BorderSize = 1;
                b.Click += (s, e) =>
                {
                    selectedTF = tf;
                    foreach (Control c in pnlHeader.Controls)
                    {
                        if (!(c is Button btn) || Array.IndexOf(tfs, btn.Text) < 0) continue;
                        bool sel = btn.Text == selectedTF;
                        btn.BackColor = sel ? Color.FromArgb(20, 0, 212, 170) : Color.Transparent;
                        btn.ForeColor = sel ? Accent : TextSecondary;
                        btn.FlatAppearance.BorderColor = sel
                            ? Color.FromArgb(80, 0, 212, 170)
                            : Color.FromArgb(25, 255, 255, 255);
                    }
                    pnlChart?.Invalidate();
                };
                pnlHeader.Controls.Add(b);
                tx += 38;
            }

            lblLivePrice = new Label
            {
                Text = "1.08547",
                Location = new Point(560, 15),
                Size = new Size(90, 22),
                ForeColor = Accent,
                Font = new Font("Consolas", 13f, FontStyle.Bold),
                BackColor = Color.Transparent,
            };
            lblPriceChange = new Label
            {
                Text = "▲ +0.00012",
                Location = new Point(655, 20),
                Size = new Size(90, 16),
                ForeColor = Accent,
                Font = new Font("Consolas", 9f),
                BackColor = Color.Transparent,
            };

            btnReset = MakeBtn("↺ Reset", 80, BgPanel, TextSecondary);
            btnApply = MakeBtn("▶ Apply", 80, Accent, BgDeep);
            btnApply.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            btnApply.Click += BtnApply_Click;
            btnReset.Click += (s, e) => pnlChart?.Invalidate();

            pnlHeader.Controls.AddRange(new Control[] { lblLivePrice, lblPriceChange, btnApply, btnReset });

            void Pos()
            {
                btnApply.Location = new Point(pnlHeader.Width - 98, 15);
                btnReset.Location = new Point(pnlHeader.Width - 184, 15);
            }
            Pos();
            pnlHeader.Resize += (s, e) => Pos();
            pnlMain.Controls.Add(pnlHeader);
        }

        private Button MakeBtn(string text, int w, Color bg, Color fg)
        {
            var b = new Button
            {
                Text = text,
                Size = new Size(w, 26),
                FlatStyle = FlatStyle.Flat,
                BackColor = bg,
                ForeColor = fg,
                Font = new Font("Segoe UI", 9f),
                Cursor = Cursors.Hand,
            };
            b.FlatAppearance.BorderColor = Color.FromArgb(60, 255, 255, 255);
            b.FlatAppearance.BorderSize = 1;
            b.MouseEnter += (s, e) => b.BackColor = ControlPaint.Dark(bg, 0.1f);
            b.MouseLeave += (s, e) => b.BackColor = bg;
            return b;
        }

        private void Header_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (var br = new SolidBrush(Color.FromArgb(30, 0, 212, 170)))
                g.FillRectangle(br, 14, 10, 36, 36);
            using (var pen = new Pen(Color.FromArgb(80, 0, 212, 170)))
                g.DrawRectangle(pen, 14, 10, 35, 35);
            using (var f = new Font("Segoe UI", 16f))
            using (var br = new SolidBrush(Accent))
                g.DrawString("▦", f, br, 18, 12);
            using (var f = new Font("Segoe UI", 13f, FontStyle.Bold))
            using (var br = new SolidBrush(TextPrimary))
                g.DrawString("Volume Profile", f, br, 58, 12);
            using (var f = new Font("Consolas", 8f))
            using (var br = new SolidBrush(TextSecondary))
                g.DrawString("TradingView-style volume profile for MT5  ·  Price action & SMC", f, br, 60, 32);
            using (var pen = new Pen(Color.FromArgb(25, 255, 255, 255)))
                g.DrawLine(pen, 0, pnlHeader.Height - 1, pnlHeader.Width, pnlHeader.Height - 1);
        }

        // ── Content ───────────────────────────────────────────────
        private void BuildContentArea()
        {
            pnlContent = new Panel { BackColor = BgDeep, AutoScroll = true };

            pnlChart = new Panel { BackColor = BgDeep };
            pnlChart.Paint += Chart_Paint;

            pnlKeyLevels = BuildKeyLevelsPanel();
            pnlSettings = BuildSettingsPanel();
            pnlSession = BuildSessionPanel();
            lvLevels = BuildLevelsTable();

            pnlContent.Controls.AddRange(new Control[]
            {
                pnlChart, pnlKeyLevels, pnlSettings, pnlSession, lvLevels
            });
            pnlMain.Controls.Add(pnlContent);
            pnlContent.Resize += (s, e) => LayoutContent();
            LayoutContent();
        }

        private void LayoutContent()
        {
            if (pnlContent == null) return;
            int W = Math.Max(pnlContent.ClientSize.Width - 24, 100);
            int pad = 10;

            pnlChart.Location = new Point(12, 12);
            pnlChart.Size = new Size(W, 230);

            int colW = (W - pad * 2) / 3;
            int row2Y = pnlChart.Bottom + pad;

            pnlKeyLevels.Location = new Point(12, row2Y);
            pnlKeyLevels.Size = new Size(colW, 165);
            pnlSettings.Location = new Point(12 + colW + pad, row2Y);
            pnlSettings.Size = new Size(colW, 165);
            pnlSession.Location = new Point(12 + (colW + pad) * 2, row2Y);
            pnlSession.Size = new Size(colW, 165);

            lvLevels.Location = new Point(12, pnlKeyLevels.Bottom + pad);
            lvLevels.Size = new Size(W, 165);
        }

        // ── Chart Paint ───────────────────────────────────────────
        private void Chart_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            int W = pnlChart.Width, H = pnlChart.Height;
            if (W < 10 || H < 10) return;

            g.Clear(Color.FromArgb(13, 15, 20));

            using (var pen = new Pen(Color.FromArgb(12, 255, 255, 255), 0.5f))
            {
                for (int i = 1; i < 8; i++) g.DrawLine(pen, i * W / 8, 0, i * W / 8, H);
                for (int i = 1; i < 6; i++) g.DrawLine(pen, 0, i * H / 6, W, i * H / 6);
            }

            double priceMin = 1.0816, priceMax = 1.0878;
            double priceRange = priceMax - priceMin;
            int chartLeft = (int)(W * 0.02);
            int chartRight = (int)(W * 0.84);
            int vpLeft = chartRight + 4;

            int nBars = 55;
            float barW = (float)(chartRight - chartLeft) / nBars;
            double p = 1.0852;

            for (int i = 0; i < nBars; i++)
            {
                double o = p;
                double c = p + (rng.NextDouble() - 0.485) * 0.0006;
                double h = Math.Max(o, c) + rng.NextDouble() * 0.0003;
                double l = Math.Min(o, c) - rng.NextDouble() * 0.0003;
                p = c;

                float cx = chartLeft + i * barW + barW / 2f;
                float yH = PriceToY(h, priceMin, priceRange, H);
                float yL = PriceToY(l, priceMin, priceRange, H);
                float yO = PriceToY(o, priceMin, priceRange, H);
                float yC = PriceToY(c, priceMin, priceRange, H);
                bool bull = c >= o;
                Color col = bull ? Accent : Red;

                using (var pen = new Pen(col, 0.8f)) g.DrawLine(pen, cx, yH, cx, yL);
                float top = Math.Min(yO, yC);
                float bh = Math.Max(Math.Abs(yO - yC), 1.5f);
                using (var br = new SolidBrush(Color.FromArgb(180, col.R, col.G, col.B)))
                    g.FillRectangle(br, cx - barW * 0.35f, top, barW * 0.7f, bh);
            }

            int[] vols = { 22600, 28400, 33100, 41200, 47300, 52800, 68900, 52800, 47300, 41200, 33100, 28400, 22600 };
            float vpBarH = (float)H / vols.Length;
            int vpW = W - vpLeft - 2;

            for (int i = 0; i < vols.Length; i++)
            {
                float bw = (float)vols[i] / 68900 * vpW;
                float by = i * vpBarH;
                bool isPOC = vols[i] == 68900;
                using (var br = new SolidBrush(isPOC
                    ? Color.FromArgb(130, 240, 165, 0)
                    : Color.FromArgb(50, 0, 212, 170)))
                    g.FillRectangle(br, vpLeft, by + 1, bw, vpBarH - 2);
                if (splitMode && !isPOC)
                    using (var br = new SolidBrush(Color.FromArgb(40, 255, 77, 106)))
                        g.FillRectangle(br, vpLeft + bw * 0.6f, by + 1, bw * 0.4f, vpBarH - 2);
            }

            if (showPOC)
            {
                float pocY = PriceToY(1.08520, priceMin, priceRange, H);
                using (var pen = new Pen(Color.FromArgb(180, 240, 165, 0), 1f) { DashStyle = DashStyle.Dash })
                    g.DrawLine(pen, chartLeft, pocY, W, pocY);
                using (var f = new Font("Consolas", 7.5f))
                using (var br = new SolidBrush(Gold))
                    g.DrawString("POC 1.08520", f, br, chartLeft + 4, pocY - 12);
            }

            if (showVA)
            {
                float vahY = PriceToY(1.08680, priceMin, priceRange, H);
                float valY = PriceToY(1.08340, priceMin, priceRange, H);
                using (var pen = new Pen(Color.FromArgb(120, 0, 212, 170), 0.8f) { DashStyle = DashStyle.Dot })
                {
                    g.DrawLine(pen, chartLeft, vahY, W, vahY);
                    g.DrawLine(pen, chartLeft, valY, W, valY);
                }
                using (var f = new Font("Consolas", 7.5f))
                {
                    using (var br = new SolidBrush(Color.FromArgb(160, 0, 212, 170)))
                        g.DrawString("VAH 1.08680", f, br, chartLeft + 4, vahY - 12);
                    using (var br = new SolidBrush(Color.FromArgb(160, 255, 77, 106)))
                        g.DrawString("VAL 1.08340", f, br, chartLeft + 4, valY - 12);
                }
            }

            float curY = PriceToY(livePrice, priceMin, priceRange, H);
            using (var pen = new Pen(Color.FromArgb(60, 255, 255, 255), 0.6f) { DashStyle = DashStyle.Dot })
                g.DrawLine(pen, chartLeft, curY, chartRight, curY);

            using (var f = new Font("Consolas", 9f, FontStyle.Bold))
            using (var br = new SolidBrush(TextPrimary))
                g.DrawString($"EURUSD · {selectedTF}", f, br, chartLeft + 4, 6);
        }

        private static float PriceToY(double price, double min, double range, int H)
            => (float)(H - ((price - min) / range) * H);

        // ── Key Levels Panel ──────────────────────────────────────
        private Panel BuildKeyLevelsPanel()
        {
            var p = new Panel { BackColor = BgCard };
            p.Paint += (s, e) => PaintCard(e.Graphics, p, "Key Levels");
            var rows = new[]
            {
                ("POC (Point of Control)", "1.08520", Gold),
                ("VAH (Value Area High)",  "1.08680", Accent),
                ("VAL (Value Area Low)",   "1.08340", Red),
                ("Value Area %",           "70%",     TextPrimary),
                ("Total Volume",           "184,320", TextPrimary),
            };
            int y = 34;
            foreach (var (lbl, val, col) in rows)
            {
                p.Controls.Add(new Label { Text = lbl, Location = new Point(10, y), Size = new Size(135, 16), ForeColor = TextSecondary, Font = new Font("Segoe UI", 8f), BackColor = Color.Transparent });
                p.Controls.Add(new Label { Text = val, Location = new Point(148, y), Size = new Size(70, 16), ForeColor = col, Font = new Font("Consolas", 8.5f, FontStyle.Bold), TextAlign = ContentAlignment.MiddleRight, BackColor = Color.Transparent });
                y += 22;
            }
            return p;
        }

        // ── Settings Panel ────────────────────────────────────────
        private Panel BuildSettingsPanel()
        {
            var p = new Panel { BackColor = BgCard };
            p.Paint += (s, e) => PaintCard(e.Graphics, p, "Settings");

            p.Controls.Add(new Label { Text = "Profile Type", Location = new Point(10, 37), Size = new Size(90, 16), ForeColor = TextSecondary, Font = new Font("Segoe UI", 8f), BackColor = Color.Transparent });
            var cmb = new ComboBox { Location = new Point(105, 34), Size = new Size(105, 20), DropDownStyle = ComboBoxStyle.DropDownList, BackColor = BgHover, ForeColor = TextPrimary, FlatStyle = FlatStyle.Flat, Font = new Font("Consolas", 8f) };
            cmb.Items.AddRange(new object[] { "Session", "Fixed Range", "Visible Range", "Daily" });
            cmb.SelectedIndex = 0;
            cmb.SelectedIndexChanged += (s, e) => { profileType = cmb.SelectedItem.ToString(); pnlChart?.Invalidate(); };
            p.Controls.Add(cmb);

            chkPOC = new CheckBox { Text = "Show POC", Checked = true, Location = new Point(10, 72), ForeColor = Accent, Font = new Font("Segoe UI", 9f), BackColor = Color.Transparent };
            chkPOC.CheckedChanged += (s, e) => { showPOC = chkPOC.Checked; pnlChart?.Invalidate(); };

            chkVA = new CheckBox { Text = "Show Value Area", Checked = true, Location = new Point(10, 98), ForeColor = TextSecondary, Font = new Font("Segoe UI", 9f), BackColor = Color.Transparent };
            chkVA.CheckedChanged += (s, e) => { showVA = chkVA.Checked; pnlChart?.Invalidate(); };

            chkSplit = new CheckBox { Text = "Buy / Sell Split", Checked = false, Location = new Point(10, 124), ForeColor = TextSecondary, Font = new Font("Segoe UI", 9f), BackColor = Color.Transparent };
            chkSplit.CheckedChanged += (s, e) => { splitMode = chkSplit.Checked; pnlChart?.Invalidate(); };

            p.Controls.AddRange(new Control[] { chkPOC, chkVA, chkSplit });
            return p;
        }

        // ── Session Panel ─────────────────────────────────────────
        private Panel BuildSessionPanel()
        {
            var p = new Panel { BackColor = BgCard };
            p.Paint += (s, e) => PaintCard(e.Graphics, p, "Session Stats");
            var rows = new[]
            {
                ("Session",       "London",  Blue),
                ("High",          "1.08742", Accent),
                ("Low",           "1.08201", Red),
                ("Range",         "0.00541", TextPrimary),
                ("Dominant Side", "Buy ▲",   Accent),
            };
            int y = 34;
            foreach (var (lbl, val, col) in rows)
            {
                p.Controls.Add(new Label { Text = lbl, Location = new Point(10, y), Size = new Size(110, 16), ForeColor = TextSecondary, Font = new Font("Segoe UI", 8f), BackColor = Color.Transparent });
                p.Controls.Add(new Label { Text = val, Location = new Point(125, y), Size = new Size(80, 16), ForeColor = col, Font = new Font("Consolas", 8.5f, FontStyle.Bold), TextAlign = ContentAlignment.MiddleRight, BackColor = Color.Transparent });
                y += 22;
            }
            return p;
        }

        private void PaintCard(Graphics g, Panel p, string title)
        {
            using (var pen = new Pen(Color.FromArgb(30, 255, 255, 255)))
                g.DrawRectangle(pen, 0, 0, p.Width - 1, p.Height - 1);
            using (var br = new SolidBrush(Color.FromArgb(20, 255, 255, 255)))
                g.FillRectangle(br, 0, 0, p.Width, 24);
            using (var pen = new Pen(Color.FromArgb(25, 255, 255, 255)))
                g.DrawLine(pen, 0, 24, p.Width, 24);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (var br = new SolidBrush(Accent))
                g.FillEllipse(br, 8, 10, 4, 4);
            using (var f = new Font("Consolas", 7.5f, FontStyle.Bold))
            using (var br = new SolidBrush(TextMuted))
                g.DrawString(title.ToUpper(), f, br, 16, 7);
        }

        // ── Levels ListView ───────────────────────────────────────
        private ListView BuildLevelsTable()
        {
            var lv = new ListView
            {
                View = View.Details,
                FullRowSelect = true,
                GridLines = false,
                BackColor = BgCard,
                ForeColor = TextPrimary,
                BorderStyle = BorderStyle.None,
                Font = new Font("Consolas", 8.5f),
                OwnerDraw = true,
            };
            lv.Columns.Add("Price", 80);
            lv.Columns.Add("Volume", 80);
            lv.Columns.Add("Buy %", 60);
            lv.Columns.Add("Sell %", 60);
            lv.Columns.Add("Type", 55);
            lv.Columns.Add("Distribution", 200);

            foreach (var lvl in vpLevels)
            {
                var item = new ListViewItem(lvl.Price.ToString("F5")) { Tag = lvl };
                item.SubItems.Add(lvl.Volume.ToString("N0"));
                item.SubItems.Add(lvl.BuyPct + "%");
                item.SubItems.Add((100 - lvl.BuyPct) + "%");
                item.SubItems.Add(lvl.Type);
                item.SubItems.Add("");
                lv.Items.Add(item);
            }

            lv.DrawColumnHeader += (s, e) =>
            {
                e.Graphics.FillRectangle(new SolidBrush(BgPanel), e.Bounds);
                using (var pen = new Pen(Color.FromArgb(25, 255, 255, 255)))
                    e.Graphics.DrawLine(pen, e.Bounds.Left, e.Bounds.Bottom - 1, e.Bounds.Right, e.Bounds.Bottom - 1);
                using (var f = new Font("Consolas", 7.5f, FontStyle.Bold))
                using (var br = new SolidBrush(TextMuted))
                    e.Graphics.DrawString(e.Header.Text.ToUpper(), f, br, e.Bounds.Left + 6, e.Bounds.Top + 5);
            };

            lv.DrawItem += (s, e) => e.DrawBackground();

            lv.DrawSubItem += (s, e) =>
            {
                if (!(e.Item.Tag is VPLevel lvl)) return;
                var g = e.Graphics;
                var rc = e.Bounds;

                Color fg = TextPrimary;
                switch (e.ColumnIndex)
                {
                    case 0: fg = lvl.Type == "POC" ? Gold : lvl.Type == "VAH" ? Accent : lvl.Type == "VAL" ? Red : TextPrimary; break;
                    case 2: fg = Accent; break;
                    case 3: fg = Red; break;
                    case 4: fg = lvl.Type == "POC" ? Gold : lvl.Type == "VAH" ? Accent : lvl.Type == "VAL" ? Red : TextMuted; break;
                }

                if (e.ColumnIndex == 5)
                {
                    float pct = (float)lvl.Volume / 68900;
                    int bW = (int)(rc.Width * 0.85f);
                    int buyW = (int)(bW * pct * lvl.BuyPct / 100f);
                    int selW = (int)(bW * pct * (100 - lvl.BuyPct) / 100f);
                    int barY = rc.Y + rc.Height / 2 - 3;
                    g.FillRectangle(new SolidBrush(BgHover), rc.X + 4, barY, bW, 6);
                    g.FillRectangle(new SolidBrush(Color.FromArgb(160, 0, 212, 170)), rc.X + 4, barY, buyW, 6);
                    g.FillRectangle(new SolidBrush(Color.FromArgb(160, 255, 77, 106)), rc.X + 4 + buyW, barY, selW, 6);
                }
                else
                {
                    using (var f = new Font("Consolas", 8.5f))
                    using (var br = new SolidBrush(fg))
                        g.DrawString(e.SubItem.Text, f, br, rc.X + 4, rc.Y + 4);
                }
                using (var pen = new Pen(Color.FromArgb(8, 255, 255, 255)))
                    g.DrawLine(pen, rc.Left, rc.Bottom - 1, rc.Right, rc.Bottom - 1);
            };

            return lv;
        }

        // ── Status Bar ────────────────────────────────────────────
        private void BuildStatusBar()
        {
            pnlStatusBar = new Panel { Dock = DockStyle.Bottom, Height = 28, BackColor = BgPanel };
            pnlStatusBar.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(25, 255, 255, 255)))
                    e.Graphics.DrawLine(pen, 0, 0, pnlStatusBar.Width, 0);
                using (var f = new Font("Consolas", 8f))
                using (var br = new SolidBrush(TextMuted))
                {
                    e.Graphics.DrawString("EURUSD · Spread: 0.8", f, br, 110, 7);
                    e.Graphics.DrawString("VP v2.1.0", f, br, pnlStatusBar.Width - 80, 7);
                }
            };

            lblStatus = new Label { Text = "● Ready", Location = new Point(14, 7), Size = new Size(90, 14), ForeColor = Accent, Font = new Font("Consolas", 8f), BackColor = Color.Transparent };
            lblClock = new Label { Location = new Point(200, 7), Size = new Size(180, 14), ForeColor = TextMuted, Font = new Font("Consolas", 8f), BackColor = Color.Transparent, Text = DateTime.Now.ToString("HH:mm:ss · dd.MM.yyyy") };
            pnlStatusBar.Controls.AddRange(new Control[] { lblStatus, lblClock });
            this.Controls.Add(pnlStatusBar);
        }

        // ── Relayout ──────────────────────────────────────────────
        private void RelayoutMain()
        {
            int top = pnlTitleBar?.Height ?? 46;
            int bot = pnlStatusBar?.Height ?? 28;

            if (pnlSidebar != null)
            {
                pnlSidebar.Location = new Point(0, top);
                pnlSidebar.Size = new Size(200, this.ClientSize.Height - top - bot);
            }
            if (pnlMain != null)
            {
                pnlMain.Location = new Point(200, top);
                pnlMain.Size = new Size(this.ClientSize.Width - 200, this.ClientSize.Height - top - bot);
            }
            if (pnlContent != null && pnlHeader != null)
            {
                pnlContent.Location = new Point(0, pnlHeader.Height);
                pnlContent.Size = new Size(pnlMain.Width, pnlMain.Height - pnlHeader.Height);
            }
            LayoutContent();
        }

        // ── Price Ticker ──────────────────────────────────────────
        private void StartPriceTicker()
        {
            priceTimer = new System.Windows.Forms.Timer { Interval = 1800 };
            priceTimer.Tick += (s, e) =>
            {
                livePrice += (rng.NextDouble() - 0.49) * 0.00008;
                priceChange = livePrice - 1.08535;
                if (lblLivePrice != null) lblLivePrice.Text = livePrice.ToString("F5");
                if (lblPriceChange != null)
                {
                    lblPriceChange.Text = (priceChange >= 0 ? "▲ +" : "▼ ") + priceChange.ToString("F5");
                    lblPriceChange.ForeColor = priceChange >= 0 ? Accent : Red;
                }
                if (lblClock != null) lblClock.Text = DateTime.Now.ToString("HH:mm:ss · dd.MM.yyyy");
                pnlChart?.Invalidate();
            };
            priceTimer.Start();
        }

        // ── Apply Button ──────────────────────────────────────────
        private async void BtnApply_Click(object sender, EventArgs e)
        {
            btnApply.Text = "◌ Applying...";
            btnApply.BackColor = AccentDim;
            btnApply.Enabled = false;
            await System.Threading.Tasks.Task.Delay(1200);
            btnApply.Text = "✓ Applied";
            btnApply.BackColor = Color.FromArgb(0, 150, 120);
            if (lblStatus != null) lblStatus.Text = "● Profile applied";
            await System.Threading.Tasks.Task.Delay(2000);
            btnApply.Text = "▶ Apply";
            btnApply.BackColor = Accent;
            btnApply.Enabled = true;
            if (lblStatus != null) lblStatus.Text = "● Ready";
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            priceTimer?.Stop();
            base.OnFormClosed(e);
        }
    }

    // ── Veri Modeli ───────────────────────────────────────────────
    public class VPLevel
    {
        public double Price { get; set; }
        public int Volume { get; set; }
        public int BuyPct { get; set; }
        public string Type { get; set; }
    }

    // ── Native (form sürükleme) ───────────────────────────────────
    internal static class NativeMethods
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
    }
}
