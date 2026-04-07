#nullable disable
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        // ── Tema: Koyu deniz mavisi ───────────────────────────────
        readonly Color BG = Color.FromArgb(7, 9, 14);
        readonly Color SURF = Color.FromArgb(11, 14, 22);
        readonly Color CARD = Color.FromArgb(15, 18, 29);
        readonly Color ELEV = Color.FromArgb(20, 24, 38);
        readonly Color HOVER = Color.FromArgb(26, 30, 46);

        readonly Color MINT = Color.FromArgb(30, 220, 180);  // ana accent
        readonly Color MINTD = Color.FromArgb(20, 165, 135);  // dim
        readonly Color RED = Color.FromArgb(255, 65, 95);
        readonly Color AMBER = Color.FromArgb(255, 185, 40);
        readonly Color BLUE = Color.FromArgb(70, 155, 255);
        readonly Color PURP = Color.FromArgb(155, 100, 255);

        readonly Color TXT = Color.FromArgb(208, 218, 244);
        readonly Color TXT2 = Color.FromArgb(82, 92, 122);
        readonly Color TXT3 = Color.FromArgb(36, 42, 64);
        readonly Color BDR = Color.FromArgb(18, 255, 255, 255);

        // ── State ─────────────────────────────────────────────────
        string selNav = "Scalping Bot";
        string selPair = "EURUSD";
        string selTF = "M1";
        string platform = "MT5";           // MT4 / MT5
        bool connected = false;
        bool botRunning = false;
        string connServer = "MetaQuotes-Demo";
        string connLogin = "12345678";
        double livePrice = 1.08312;
        double botPnl = 0.0;
        int botTrades = 0;
        int botWins = 0;
        double balance = 10000.0;
        double lotSize = 0.10;
        double riskPct = 1.0;
        int slPips = 10;
        int tpPips = 20;
        readonly Random RNG = new Random();

        System.Windows.Forms.Timer ticker;
        System.Windows.Forms.Timer botTimer;

        // ── Sinyal ────────────────────────────────────────────────
        class Signal
        {
            public string Time = "", Pair = "", TF = "", Dir = "", Entry = "", SL = "", TP = "", Lot = "", Status = "", PnL = "";
            public bool IsWin;
        }

        readonly List<Signal> signals = new List<Signal>
        {
            new Signal{Time="11:42",Pair="EURUSD",TF="M1",Dir="BUY", Entry="1.08290",SL="1.08190",TP="1.08490",Lot="0.10",Status="CLOSED",PnL="+$20.00",IsWin=true },
            new Signal{Time="11:28",Pair="EURUSD",TF="M5",Dir="SELL",Entry="1.08380",SL="1.08480",TP="1.08180",Lot="0.10",Status="CLOSED",PnL="-$10.00",IsWin=false},
            new Signal{Time="10:55",Pair="EURUSD",TF="M1",Dir="BUY", Entry="1.08210",SL="1.08110",TP="1.08410",Lot="0.10",Status="CLOSED",PnL="+$20.00",IsWin=true },
            new Signal{Time="10:30",Pair="GBPUSD",TF="M5",Dir="BUY", Entry="1.27310",SL="1.27210",TP="1.27510",Lot="0.10",Status="CLOSED",PnL="+$20.00",IsWin=true },
        };

        // ── Controls ──────────────────────────────────────────────
        Panel pnlSidebar, pnlTopBar, pnlContent, pnlStatus;
        Panel pnlChart, pnlConnect, pnlBotCtrl, pnlStats, pnlSignals;
        Label lblPrice, lblPriceChg, lblClock, lblStatusTxt;
        Label lblConnState, lblPlatform, lblBotState;
        Label lblPnl, lblWinRate, lblTrades, lblBalance;
        Button btnConnect, btnBotStart, btnBotStop;
        ComboBox cmbPlatform, cmbServer;
        TextBox txtLogin, txtPassword;
        ListView lvSignals;
        TrackBar trkLot, trkSL, trkTP;
        Label lblLotV, lblSLV, lblTPV;

        // ═════════════════════════════════════════════════════════
        public Form1()
        {
            this.Text = "Scalping Bot · MT4/MT5 · AutoScripts";
            this.Size = new Size(1220, 760);
            this.MinimumSize = new Size(1000, 640);
            this.BackColor = BG;
            this.ForeColor = TXT;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 9f);
            this.DoubleBuffered = true;
            this.FormBorderStyle = FormBorderStyle.None;

            MakeTopBar();
            MakeStatusBar();
            MakeSidebar();
            MakeContent();
            StartTimers();
        }

        // ── TopBar ────────────────────────────────────────────────
        void MakeTopBar()
        {
            pnlTopBar = new Panel { Dock = DockStyle.Top, Height = 48, BackColor = SURF };
            pnlTopBar.Paint += (s, e) =>
            {
                var g = e.Graphics;
                Color ac = connected ? MINT : TXT3;
                using (var br = new LinearGradientBrush(new Point(0, 0), new Point(pnlTopBar.Width, 0), Color.FromArgb(70, ac.R, ac.G, ac.B), Color.Transparent))
                    g.FillRectangle(br, 0, 0, pnlTopBar.Width, 3);
                using (var p = new Pen(BDR)) g.DrawLine(p, 0, 47, pnlTopBar.Width, 47);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                // bot ikon (şimşek)
                DrawBotIcon(g, 14, 12, connected ? MINT : TXT2);
                using (var f = new Font("Segoe UI", 11f, FontStyle.Bold)) using (var br = new SolidBrush(TXT))
                    g.DrawString("SCALPING BOT", f, br, 46, 12);
                using (var f = new Font("Consolas", 7.5f)) using (var br = new SolidBrush(TXT3))
                    g.DrawString("1M / 5M  ·  MT4 / MT5  ·  Auto Signal & Execution", f, br, 48, 29);
            };
            pnlTopBar.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                { NativeMethods.ReleaseCapture(); NativeMethods.SendMessage(Handle, 0xA1, (IntPtr)2, IntPtr.Zero); }
            };

            var bCl = WinBtn(Color.FromArgb(255, 85, 75)); bCl.Click += (s, e) => Close();
            var bMn = WinBtn(Color.FromArgb(255, 185, 40)); bMn.Click += (s, e) => WindowState = FormWindowState.Minimized;
            var bMx = WinBtn(Color.FromArgb(35, 195, 55)); bMx.Click += (s, e) => WindowState = WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized;
            pnlTopBar.Controls.AddRange(new Control[] { bMn, bMx, bCl });
            void PW() { bCl.Location = new Point(pnlTopBar.Width - 26, 18); bMx.Location = new Point(pnlTopBar.Width - 46, 18); bMn.Location = new Point(pnlTopBar.Width - 66, 18); }
            PW(); pnlTopBar.Resize += (s, e) => PW();

            // Pair seçici
            string[] pairs = { "EURUSD", "GBPUSD", "USDJPY", "XAUUSD", "AUDUSD", "USDCHF" };
            int px = 290;
            foreach (var pr in pairs)
            {
                string p2 = pr;
                var b = new Button
                {
                    Text = p2,
                    Location = new Point(px, 11),
                    Size = new Size(68, 26),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = p2 == selPair ? Color.FromArgb(28, 30, 220, 180) : Color.Transparent,
                    ForeColor = p2 == selPair ? MINT : TXT2,
                    Font = new Font("Consolas", 8.5f, FontStyle.Bold),
                    Cursor = Cursors.Hand
                };
                b.FlatAppearance.BorderColor = p2 == selPair ? Color.FromArgb(80, MINT.R, MINT.G, MINT.B) : Color.FromArgb(18, 255, 255, 255);
                b.FlatAppearance.BorderSize = 1;
                b.Click += (s, e) => {
                    selPair = p2;
                    foreach (Control c in pnlTopBar.Controls)
                    {
                        if (!(c is Button btn) || Array.IndexOf(pairs, btn.Text) < 0) continue;
                        bool sel = btn.Text == selPair;
                        btn.BackColor = sel ? Color.FromArgb(28, 30, 220, 180) : Color.Transparent;
                        btn.ForeColor = sel ? MINT : TXT2;
                        btn.FlatAppearance.BorderColor = sel ? Color.FromArgb(80, MINT.R, MINT.G, MINT.B) : Color.FromArgb(18, 255, 255, 255);
                    }
                    pnlChart?.Invalidate();
                };
                pnlTopBar.Controls.Add(b); px += 72;
            }

            // TF seçici
            string[] tfs = { "M1", "M5", "M15", "M30" };
            int tx = 720;
            foreach (var tf in tfs)
            {
                string t2 = tf;
                var b = new Button
                {
                    Text = t2,
                    Location = new Point(tx, 11),
                    Size = new Size(40, 26),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = t2 == selTF ? Color.FromArgb(28, 70, 155, 255) : Color.Transparent,
                    ForeColor = t2 == selTF ? BLUE : TXT2,
                    Font = new Font("Consolas", 8f, FontStyle.Bold),
                    Cursor = Cursors.Hand
                };
                b.FlatAppearance.BorderColor = t2 == selTF ? Color.FromArgb(80, BLUE.R, BLUE.G, BLUE.B) : Color.FromArgb(18, 255, 255, 255);
                b.FlatAppearance.BorderSize = 1;
                b.Click += (s, e) => {
                    selTF = t2;
                    foreach (Control c in pnlTopBar.Controls)
                    {
                        if (!(c is Button btn) || Array.IndexOf(tfs, btn.Text) < 0) continue;
                        bool sel = btn.Text == selTF;
                        btn.BackColor = sel ? Color.FromArgb(28, 70, 155, 255) : Color.Transparent;
                        btn.ForeColor = sel ? BLUE : TXT2;
                        btn.FlatAppearance.BorderColor = sel ? Color.FromArgb(80, BLUE.R, BLUE.G, BLUE.B) : Color.FromArgb(18, 255, 255, 255);
                    }
                    pnlChart?.Invalidate();
                };
                pnlTopBar.Controls.Add(b); tx += 44;
            }

            lblPrice = new Label { Text = "1.08312", Location = new Point(878, 10), Size = new Size(105, 26), ForeColor = MINT, Font = new Font("Consolas", 15f, FontStyle.Bold), BackColor = Color.Transparent };
            lblPriceChg = new Label { Text = "▲ +0.00021", Location = new Point(986, 17), Size = new Size(110, 16), ForeColor = MINT, Font = new Font("Consolas", 9f), BackColor = Color.Transparent };
            pnlTopBar.Controls.AddRange(new Control[] { lblPrice, lblPriceChg });
            this.Controls.Add(pnlTopBar);
        }

        void DrawBotIcon(Graphics g, int x, int y, Color col)
        {
            using (var p = new Pen(col, 1.8f))
            {
                // şimşek/bot ikonu
                g.DrawPolygon(p, new Point[] { new Point(x + 13, y), new Point(x + 6, y + 12), new Point(x + 12, y + 12), new Point(x + 5, y + 24), new Point(x + 18, y + 10), new Point(x + 11, y + 10) });
            }
        }

        // ── StatusBar ─────────────────────────────────────────────
        void MakeStatusBar()
        {
            pnlStatus = new Panel { Dock = DockStyle.Bottom, Height = 26, BackColor = SURF };
            pnlStatus.Paint += (s, e) => {
                var g = e.Graphics;
                using (var p = new Pen(BDR)) g.DrawLine(p, 0, 0, pnlStatus.Width, 0);
                Color sc = connected ? MINT : TXT3;
                using (var br = new LinearGradientBrush(new Point(0, 24), new Point(pnlStatus.Width, 24), Color.FromArgb(40, sc.R, sc.G, sc.B), Color.Transparent))
                    g.FillRectangle(br, 0, 24, pnlStatus.Width, 2);
                using (var f = new Font("Consolas", 8f)) using (var br = new SolidBrush(TXT3))
                {
                    g.DrawString($"Platform: {platform}   Server: {connServer}   Login: {connLogin}   Trades: {botTrades}   Win Rate: {(botTrades > 0 ? (int)((double)botWins / botTrades * 100) : 0)}%", f, br, 210, 7);
                    g.DrawString("ScalpBot v1.0", f, br, pnlStatus.Width - 100, 7);
                }
            };
            lblStatusTxt = new Label { Text = "○ Disconnected", Location = new Point(12, 6), Size = new Size(190, 14), ForeColor = TXT2, Font = new Font("Consolas", 8f, FontStyle.Bold), BackColor = Color.Transparent };
            lblClock = new Label { Text = "", Location = new Point(800, 6), Size = new Size(230, 14), ForeColor = TXT3, Font = new Font("Consolas", 8f), BackColor = Color.Transparent };
            pnlStatus.Controls.AddRange(new Control[] { lblStatusTxt, lblClock });
            pnlStatus.Resize += (s, e) => { lblClock.Location = new Point(pnlStatus.Width - 240, 6); pnlStatus.Invalidate(); };
            this.Controls.Add(pnlStatus);
        }

        // ── Sidebar ───────────────────────────────────────────────
        void MakeSidebar()
        {
            pnlSidebar = new Panel { Dock = DockStyle.Left, Width = 212, BackColor = SURF };
            pnlSidebar.Paint += (s, e) => {
                var g = e.Graphics;
                using (var p = new Pen(BDR)) g.DrawLine(p, pnlSidebar.Width - 1, 0, pnlSidebar.Width - 1, pnlSidebar.Height);
                Color sc = connected ? MINT : TXT3;
                using (var br = new LinearGradientBrush(new Point(0, 0), new Point(0, pnlSidebar.Height), Color.FromArgb(60, sc.R, sc.G, sc.B), Color.Transparent))
                    g.FillRectangle(br, 0, 0, 3, pnlSidebar.Height);
            };

            // Logo
            var logo = new Panel { Height = 56, Dock = DockStyle.Top, BackColor = Color.Transparent };
            logo.Paint += (s, e) => {
                var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
                DrawBotIcon(g, 14, 16, MINT);
                using (var f = new Font("Segoe UI", 10f, FontStyle.Bold)) using (var br = new SolidBrush(TXT)) g.DrawString("SCALPING BOT", f, br, 44, 15);
                using (var f = new Font("Consolas", 7.5f)) using (var br = new SolidBrush(TXT3)) g.DrawString("MT4 / MT5 Auto", f, br, 46, 31);
                using (var p = new Pen(BDR)) g.DrawLine(p, 8, 54, 204, 54);
            };
            pnlSidebar.Controls.Add(logo);

            // Nav
            var navDefs = new[]{
                ("","── MENU ──"),("▤","Library"),("⌂","Home"),("◈","Dashboard"),
                ("","── BOTS ──"),("⚡","Scalping Bot"),("⊛","Account Protector"),("⊞","Position Sizer"),("▦","Volume Profile"),
                ("","── SYSTEM ──"),("ℹ","About"),("✉","Contact"),
            };
            int ny = 60;
            foreach (var (icon, label) in navDefs)
            {
                if (icon == "")
                {
                    pnlSidebar.Controls.Add(new Label { Text = label, Location = new Point(10, ny), Size = new Size(192, 18), ForeColor = TXT3, Font = new Font("Consolas", 7.5f, FontStyle.Bold), BackColor = Color.Transparent });
                    ny += 22;
                }
                else
                {
                    string nav = label; bool act = nav == selNav;
                    var row = new Panel { Location = new Point(0, ny), Size = new Size(211, 34), BackColor = act ? Color.FromArgb(22, 30, 220, 180) : Color.Transparent, Cursor = Cursors.Hand, Tag = nav };
                    row.Paint += (s, e) => { if ((string)row.Tag == selNav) using (var p = new Pen(MINT, 2)) e.Graphics.DrawLine(p, row.Width - 1, 0, row.Width - 1, row.Height); };
                    row.MouseEnter += (s, e) => { if ((string)row.Tag != selNav) row.BackColor = HOVER; };
                    row.MouseLeave += (s, e) => row.BackColor = (string)row.Tag == selNav ? Color.FromArgb(22, 30, 220, 180) : Color.Transparent;
                    row.Click += (s, e) => NavClick((string)row.Tag);
                    var icL = new Label { Text = icon, Location = new Point(14, 9), Size = new Size(18, 16), ForeColor = act ? MINT : TXT2, Font = new Font("Segoe UI", 9f), BackColor = Color.Transparent, Tag = nav + "_i" };
                    var txL = new Label { Text = nav, Location = new Point(36, 9), Size = new Size(124, 16), ForeColor = act ? MINT : TXT2, Font = new Font("Segoe UI", 9f), BackColor = Color.Transparent, Tag = nav + "_t" };
                    icL.Click += (s, e) => NavClick(nav); txL.Click += (s, e) => NavClick(nav);
                    row.Controls.AddRange(new Control[] { icL, txL });
                    if (nav == "Scalping Bot")
                    {
                        var bdg = new Label { Text = "1M", Location = new Point(162, 10), Size = new Size(26, 14), BackColor = act ? MINT : TXT3, ForeColor = BG, Font = new Font("Consolas", 7f, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter };
                        bdg.Click += (s, e) => NavClick(nav); row.Controls.Add(bdg);
                    }
                    pnlSidebar.Controls.Add(row); ny += 36;
                }
            }

            // Footer — bot stats
            var foot = new Panel { Dock = DockStyle.Bottom, Height = 96, BackColor = Color.Transparent };
            foot.Paint += (s, e) => {
                var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
                using (var p = new Pen(BDR)) g.DrawLine(p, 8, 0, 204, 0);
                SbStat(g, 14, 8, "BOT P&L", botPnl >= 0 ? $"+${botPnl:N2}" : $"-${Math.Abs(botPnl):N2}", botPnl >= 0 ? MINT : RED);
                SbStat(g, 14, 34, "TRADES", botTrades.ToString(), TXT);
                SbStat(g, 14, 60, "WIN RATE", botTrades > 0 ? $"{(int)((double)botWins / botTrades * 100)}%" : "—", MINT);
                SbStat(g, 110, 60, "BALANCE", $"${balance:N0}", TXT2);
            };
            pnlSidebar.Controls.Add(foot);
            this.Controls.Add(pnlSidebar);
        }

        void SbStat(Graphics g, int x, int y, string lbl, string val, Color col)
        {
            using (var f = new Font("Consolas", 7f)) using (var br = new SolidBrush(TXT3)) g.DrawString(lbl, f, br, x, y);
            using (var f = new Font("Consolas", 8.5f, FontStyle.Bold)) using (var br = new SolidBrush(col)) g.DrawString(val, f, br, x, y + 13);
        }

        void NavClick(string nav)
        {
            selNav = nav;
            foreach (Control c in pnlSidebar.Controls)
            {
                if (!(c is Panel p) || !(p.Tag is string t)) continue;
                bool act = t == nav;
                p.BackColor = act ? Color.FromArgb(22, 30, 220, 180) : Color.Transparent; p.Invalidate();
                foreach (Control ch in p.Controls)
                {
                    if (!(ch is Label l) || l.Text == "1M") continue;
                    bool mine = l.Tag is string lt && (lt == nav + "_i" || lt == nav + "_t");
                    l.ForeColor = mine ? MINT : TXT2;
                }
            }
        }

        // ── Content ───────────────────────────────────────────────
        void MakeContent()
        {
            pnlContent = new Panel { Dock = DockStyle.Fill, BackColor = BG, Padding = new Padding(8) };

            // ── Üst satır: Chart (Fill) + Connect+BotCtrl (Right) ─
            var pnlTop = new Panel { Dock = DockStyle.Top, Height = 320, BackColor = BG };

            // Sağ sütun: Connect + BotCtrl üst üste
            var pnlRight = new Panel { Dock = DockStyle.Right, Width = 258, BackColor = BG };

            pnlBotCtrl = new Panel { Dock = DockStyle.Fill, BackColor = CARD };
            pnlBotCtrl.Paint += (s, e) => CardHdr(e.Graphics, pnlBotCtrl, "Bot Control", MINT);
            BuildBotCtrl(pnlBotCtrl);

            pnlConnect = new Panel { Dock = DockStyle.Top, Height = 156, BackColor = CARD };
            pnlConnect.Paint += (s, e) => CardHdr(e.Graphics, pnlConnect, "MT4 / MT5 Connect", BLUE);
            BuildConnectPanel(pnlConnect);

            var gapRC = new Panel { Dock = DockStyle.Top, Height = 6, BackColor = BG };
            pnlRight.Controls.Add(pnlBotCtrl);   // Fill
            pnlRight.Controls.Add(gapRC);
            pnlRight.Controls.Add(pnlConnect);    // Top

            pnlChart = new Panel { Dock = DockStyle.Fill, BackColor = CARD };
            pnlChart.Paint += Chart_Paint;

            var gapTR = new Panel { Dock = DockStyle.Right, Width = 6, BackColor = BG };
            pnlTop.Controls.Add(pnlChart);        // Fill
            pnlTop.Controls.Add(gapTR);
            pnlTop.Controls.Add(pnlRight);        // Right

            // ── Alt satır: Stats (Left) + Signals table (Fill) ────
            var pnlBot = new Panel { Dock = DockStyle.Fill, BackColor = BG };

            pnlStats = new Panel { Dock = DockStyle.Left, Width = 258, BackColor = CARD };
            pnlStats.Paint += Stats_Paint;
            BuildStatsPanel(pnlStats);

            pnlSignals = new Panel { Dock = DockStyle.Fill, BackColor = CARD };
            pnlSignals.Paint += (s, e) => CardHdr(e.Graphics, pnlSignals, "Signal History", PURP);
            BuildSignalsLV(pnlSignals);

            var gapBL = new Panel { Dock = DockStyle.Left, Width = 6, BackColor = BG };
            pnlBot.Controls.Add(pnlSignals);      // Fill
            pnlBot.Controls.Add(gapBL);
            pnlBot.Controls.Add(pnlStats);        // Left

            var gapMid = new Panel { Dock = DockStyle.Top, Height = 6, BackColor = BG };
            pnlContent.Controls.Add(pnlBot);      // Fill
            pnlContent.Controls.Add(gapMid);
            pnlContent.Controls.Add(pnlTop);      // Top

            this.Controls.Add(pnlContent);
        }

        // ── Connect Panel ─────────────────────────────────────────
        void BuildConnectPanel(Panel p)
        {
            int y = 30;

            // Platform seçici
            p.Controls.Add(new Label { Text = "Platform", Location = new Point(10, y), Size = new Size(80, 15), ForeColor = TXT2, Font = new Font("Segoe UI", 8f), BackColor = Color.Transparent });
            cmbPlatform = new ComboBox { Location = new Point(10, y + 16), Size = new Size(115, 22), DropDownStyle = ComboBoxStyle.DropDownList, BackColor = ELEV, ForeColor = TXT, FlatStyle = FlatStyle.Flat, Font = new Font("Consolas", 9f, FontStyle.Bold) };
            cmbPlatform.Items.AddRange(new object[] { "MT5", "MT4" });
            cmbPlatform.SelectedIndex = 0;
            cmbPlatform.SelectedIndexChanged += (s, e) => {
                platform = cmbPlatform.SelectedItem.ToString();
                pnlTopBar?.Invalidate(); pnlStatus?.Invalidate();
            };

            // Server
            p.Controls.Add(new Label { Text = "Server", Location = new Point(134, y), Size = new Size(80, 15), ForeColor = TXT2, Font = new Font("Segoe UI", 8f), BackColor = Color.Transparent });
            cmbServer = new ComboBox { Location = new Point(134, y + 16), Size = new Size(114, 22), DropDownStyle = ComboBoxStyle.DropDownList, BackColor = ELEV, ForeColor = TXT, FlatStyle = FlatStyle.Flat, Font = new Font("Consolas", 8f) };
            cmbServer.Items.AddRange(new object[] { "MetaQuotes-Demo", "ICMarkets-Demo", "Pepperstone-Demo", "XM-Demo", "Exness-Demo" });
            cmbServer.SelectedIndex = 0;
            cmbServer.SelectedIndexChanged += (s, e) => { connServer = cmbServer.SelectedItem.ToString(); pnlStatus?.Invalidate(); };

            p.Controls.AddRange(new Control[] { cmbPlatform, cmbServer });
            y += 42;

            // Login / Password
            p.Controls.Add(new Label { Text = "Login", Location = new Point(10, y), Size = new Size(110, 15), ForeColor = TXT2, Font = new Font("Segoe UI", 8f), BackColor = Color.Transparent });
            p.Controls.Add(new Label { Text = "Password", Location = new Point(134, y), Size = new Size(110, 15), ForeColor = TXT2, Font = new Font("Segoe UI", 8f), BackColor = Color.Transparent });
            y += 15;
            txtLogin = new TextBox { Text = "12345678", Location = new Point(10, y), Size = new Size(115, 22), BackColor = ELEV, ForeColor = TXT, BorderStyle = BorderStyle.FixedSingle, Font = new Font("Consolas", 9f, FontStyle.Bold) };
            txtPassword = new TextBox { Text = "••••••••", Location = new Point(134, y), Size = new Size(114, 22), BackColor = ELEV, ForeColor = TXT2, BorderStyle = BorderStyle.FixedSingle, Font = new Font("Consolas", 9f), PasswordChar = '•' };
            txtLogin.TextChanged += (s, e) => connLogin = txtLogin.Text;
            p.Controls.AddRange(new Control[] { txtLogin, txtPassword });
            y += 30;

            // Connect butonu
            btnConnect = new Button
            {
                Text = "▶  CONNECT",
                Location = new Point(10, y),
                Size = new Size(238, 28),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(22, 70, 155, 255),
                ForeColor = BLUE,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Cursor = Cursors.Hand,
            };
            btnConnect.FlatAppearance.BorderColor = Color.FromArgb(70, BLUE.R, BLUE.G, BLUE.B);
            btnConnect.FlatAppearance.BorderSize = 1;
            btnConnect.Click += ToggleConnect;
            p.Controls.Add(btnConnect);

            // Conn state label
            lblConnState = new Label { Text = "○  Not connected", Location = new Point(10, y + 32), Size = new Size(238, 16), ForeColor = TXT2, Font = new Font("Consolas", 8f), BackColor = Color.Transparent, TextAlign = ContentAlignment.MiddleCenter };
            p.Controls.Add(lblConnState);
        }

        // ── Bot Control Panel ─────────────────────────────────────
        void BuildBotCtrl(Panel p)
        {
            int y = 34;

            // Lot slider
            AddSlider(p, "Lot Size", ref y, 1, 100, (int)(lotSize * 100), out trkLot, out lblLotV, $"{lotSize:F2}");
            trkLot.Scroll += (s, e) => { lotSize = trkLot.Value / 100.0; lblLotV.Text = $"{lotSize:F2}"; };

            // SL slider
            AddSlider(p, "Stop Loss (pips)", ref y, 2, 50, slPips, out trkSL, out lblSLV, $"{slPips} pips");
            trkSL.Scroll += (s, e) => { slPips = trkSL.Value; lblSLV.Text = $"{slPips} pips"; };

            // TP slider
            AddSlider(p, "Take Profit (pips)", ref y, 4, 100, tpPips, out trkTP, out lblTPV, $"{tpPips} pips");
            trkTP.Scroll += (s, e) => { tpPips = trkTP.Value; lblTPV.Text = $"{tpPips} pips"; };

            y += 4;
            // Start / Stop butonları
            btnBotStart = new Button
            {
                Text = "▶  START BOT",
                Location = new Point(10, y),
                Size = new Size(115, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(25, 30, 220, 180),
                ForeColor = MINT,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand,
            };
            btnBotStart.FlatAppearance.BorderColor = Color.FromArgb(70, MINT.R, MINT.G, MINT.B);
            btnBotStart.FlatAppearance.BorderSize = 1;
            btnBotStart.Click += StartBot;

            btnBotStop = new Button
            {
                Text = "⏹  STOP",
                Location = new Point(133, y),
                Size = new Size(115, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(18, 255, 65, 95),
                ForeColor = RED,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Enabled = false,
            };
            btnBotStop.FlatAppearance.BorderColor = Color.FromArgb(60, RED.R, RED.G, RED.B);
            btnBotStop.FlatAppearance.BorderSize = 1;
            btnBotStop.Click += StopBot;

            p.Controls.AddRange(new Control[] { btnBotStart, btnBotStop });
            y += 36;

            lblBotState = new Label { Text = "● Bot Offline", Location = new Point(10, y), Size = new Size(238, 16), ForeColor = TXT2, Font = new Font("Consolas", 8.5f, FontStyle.Bold), BackColor = Color.Transparent, TextAlign = ContentAlignment.MiddleCenter };
            p.Controls.Add(lblBotState);
        }

        void AddSlider(Panel p, string lbl, ref int y, int min, int max, int val, out TrackBar trk, out Label valL, string txt)
        {
            p.Controls.Add(new Label { Text = lbl, Location = new Point(10, y), Size = new Size(160, 15), ForeColor = TXT2, Font = new Font("Segoe UI", 8f), BackColor = Color.Transparent });
            y += 16;
            trk = new TrackBar { Location = new Point(10, y), Size = new Size(178, 26), Minimum = min, Maximum = max, Value = val, TickFrequency = 10, BackColor = CARD };
            valL = new Label { Location = new Point(192, y + 4), Size = new Size(58, 16), ForeColor = MINT, Font = new Font("Consolas", 8.5f, FontStyle.Bold), BackColor = Color.Transparent, Text = txt };
            p.Controls.AddRange(new Control[] { trk, valL });
            y += 30;
        }

        // ── Stats Panel ───────────────────────────────────────────
        void BuildStatsPanel(Panel p)
        {
            // stats sadece Paint ile gösteriliyor
        }

        void Stats_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
            int W = pnlStats.Width, H = pnlStats.Height;
            CardHdr(g, pnlStats, "Session Stats", AMBER);

            // büyük P&L gösterimi
            var pnlRect = new RectangleF(10, 32, W - 20, 54);
            using (var br = new SolidBrush(Color.FromArgb(14, botPnl >= 0 ? MINT.R : RED.R, botPnl >= 0 ? MINT.G : RED.G, botPnl >= 0 ? MINT.B : RED.B)))
                g.FillRectangle(br, pnlRect.X, pnlRect.Y, pnlRect.Width, pnlRect.Height);
            using (var pen = new Pen(Color.FromArgb(35, botPnl >= 0 ? MINT.R : RED.R, botPnl >= 0 ? MINT.G : RED.G, botPnl >= 0 ? MINT.B : RED.B)))
                g.DrawRectangle(pen, pnlRect.X, pnlRect.Y, pnlRect.Width - 1, pnlRect.Height - 1);
            using (var f = new Font("Consolas", 7f)) using (var br = new SolidBrush(TXT3)) g.DrawString("SESSION P&L", f, br, 18, 38);
            Color pc = botPnl >= 0 ? MINT : RED;
            using (var f = new Font("Consolas", 22f, FontStyle.Bold)) using (var br = new SolidBrush(pc))
                g.DrawString((botPnl >= 0 ? "+" : "") + $"${botPnl:N2}", f, br, 12, 50);

            int ry = 96;
            var rows = new (string L, string V, Color C)[]{
                ("Total Trades",  botTrades.ToString(),      TXT),
                ("Wins",          botWins.ToString(),         MINT),
                ("Losses",        (botTrades-botWins).ToString(), RED),
                ("Win Rate",      botTrades>0?$"{(int)((double)botWins/botTrades*100)}%":"—", MINT),
                ("Lot Size",      $"{lotSize:F2}",            TXT),
                ("SL",            $"{slPips} pips",           RED),
                ("TP",            $"{tpPips} pips",           MINT),
                ("Risk/Trade",    $"${balance*riskPct/100:N2}",AMBER),
                ("Balance",       $"${balance:N2}",           TXT),
                ("Bot Status",    botRunning?"RUNNING":"STOPPED",botRunning?MINT:TXT2),
                ("MT Platform",   platform,                   BLUE),
                ("Connection",    connected?"CONNECTED":"OFFLINE",connected?MINT:RED),
            };
            foreach (var (l, v, c) in rows)
            {
                using (var pen = new Pen(Color.FromArgb(7, 255, 255, 255))) g.DrawLine(pen, 10, ry + 16, W - 10, ry + 16);
                using (var f = new Font("Segoe UI", 8.5f)) using (var br = new SolidBrush(TXT2)) g.DrawString(l, f, br, 10, ry);
                using (var f = new Font("Consolas", 9f, FontStyle.Bold)) using (var br = new SolidBrush(c))
                { var sf = new StringFormat { Alignment = StringAlignment.Far }; g.DrawString(v, f, br, new RectangleF(0, ry, W - 10, 16), sf); }
                ry += 20;
            }
        }

        // ── Signals ListView ──────────────────────────────────────
        void BuildSignalsLV(Panel p)
        {
            lvSignals = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = false,
                BackColor = CARD,
                ForeColor = TXT,
                BorderStyle = BorderStyle.None,
                Font = new Font("Consolas", 8.5f),
                OwnerDraw = true
            };
            lvSignals.Columns.Add("Time", 52);
            lvSignals.Columns.Add("Pair", 70);
            lvSignals.Columns.Add("TF", 40);
            lvSignals.Columns.Add("Dir", 46);
            lvSignals.Columns.Add("Entry", 80);
            lvSignals.Columns.Add("SL", 80);
            lvSignals.Columns.Add("TP", 80);
            lvSignals.Columns.Add("Lot", 46);
            lvSignals.Columns.Add("Status", 70);
            lvSignals.Columns.Add("P&L", 72);

            foreach (var sg in signals) AppendSignal(sg);

            lvSignals.DrawColumnHeader += (s, e) => {
                e.Graphics.FillRectangle(new SolidBrush(SURF), e.Bounds);
                using (var pen = new Pen(BDR)) e.Graphics.DrawLine(pen, e.Bounds.Left, e.Bounds.Bottom - 1, e.Bounds.Right, e.Bounds.Bottom - 1);
                using (var f = new Font("Consolas", 7.5f, FontStyle.Bold)) using (var br = new SolidBrush(TXT3))
                    e.Graphics.DrawString(e.Header.Text.ToUpper(), f, br, e.Bounds.Left + 5, e.Bounds.Top + 5);
            };
            lvSignals.DrawItem += (s, e) => e.DrawBackground();
            lvSignals.DrawSubItem += (s, e) => {
                if (!(e.Item.Tag is Signal sg)) return;
                var g = e.Graphics; var rc = e.Bounds;
                if (e.Item.Index % 2 == 0) using (var br = new SolidBrush(Color.FromArgb(8, 255, 255, 255))) g.FillRectangle(br, rc);
                Color fg = TXT;
                if (e.ColumnIndex == 0) fg = TXT2;
                if (e.ColumnIndex == 1) fg = MINT;
                if (e.ColumnIndex == 2) fg = BLUE;
                if (e.ColumnIndex == 3) fg = sg.Dir == "BUY" ? MINT : RED;
                if (e.ColumnIndex == 8) fg = sg.Status == "OPEN" ? AMBER : TXT2;
                if (e.ColumnIndex == 9) fg = sg.IsWin ? MINT : RED;
                using (var f = new Font("Consolas", 8.5f)) using (var br = new SolidBrush(fg))
                    g.DrawString(e.SubItem.Text, f, br, rc.X + 5, rc.Y + 4);
                using (var pen = new Pen(Color.FromArgb(7, 255, 255, 255))) g.DrawLine(pen, rc.Left, rc.Bottom - 1, rc.Right, rc.Bottom - 1);
            };

            var wrap = new Panel { Dock = DockStyle.Fill, BackColor = CARD, Padding = new Padding(0, 28, 0, 0) };
            wrap.Controls.Add(lvSignals);
            p.Controls.Add(wrap);
        }

        void AppendSignal(Signal sg)
        {
            var it = new ListViewItem(sg.Time) { Tag = sg };
            it.SubItems.Add(sg.Pair); it.SubItems.Add(sg.TF); it.SubItems.Add(sg.Dir);
            it.SubItems.Add(sg.Entry); it.SubItems.Add(sg.SL); it.SubItems.Add(sg.TP);
            it.SubItems.Add(sg.Lot); it.SubItems.Add(sg.Status); it.SubItems.Add(sg.PnL);
            lvSignals?.Items.Insert(0, it);
            if (lvSignals?.Items.Count > 100) lvSignals.Items.RemoveAt(lvSignals.Items.Count - 1);
        }

        // ── Chart ─────────────────────────────────────────────────
        void Chart_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
            int W = pnlChart.Width, H = pnlChart.Height;
            if (W < 20 || H < 20) return;

            g.Clear(Color.FromArgb(9, 11, 18));
            CardHdr(g, pnlChart, $"{selPair}  ·  {selTF}  ·  Scalping View", MINT);

            // Grid
            using (var p = new Pen(Color.FromArgb(10, 255, 255, 255), .5f))
            {
                for (int i = 1; i < 10; i++) g.DrawLine(p, i * W / 10, 0, i * W / 10, H);
                for (int i = 1; i < 6; i++) g.DrawLine(p, 0, i * H / 6, W, i * H / 6);
            }

            double pMin = 1.0810, pMax = 1.0860, pRng = pMax - pMin;
            int cL = 8, cR = W - 8, nBar = selTF == "M1" ? 90 : 60;
            float bW = (float)(cR - cL) / nBar;
            int chartH = H - 32;

            float PY(double price) => (float)(chartH - ((price - pMin) / pRng) * chartH + 28);

            // SL / TP bantları
            double sl = livePrice - slPips * 0.0001;
            double tp = livePrice + tpPips * 0.0001;
            float slY = PY(sl), tpY = PY(tp), curY = PY(livePrice);

            using (var br = new SolidBrush(Color.FromArgb(16, RED.R, RED.G, RED.B))) g.FillRectangle(br, cL, slY, cR - cL, curY - slY);
            using (var br = new SolidBrush(Color.FromArgb(16, MINT.R, MINT.G, MINT.B))) g.FillRectangle(br, cL, tpY, cR - cL, curY - tpY);

            using (var pen = new Pen(Color.FromArgb(140, RED.R, RED.G, RED.B), 1f) { DashStyle = DashStyle.Dash }) g.DrawLine(pen, cL, slY, cR, slY);
            using (var pen = new Pen(Color.FromArgb(140, MINT.R, MINT.G, MINT.B), 1f) { DashStyle = DashStyle.Dash }) g.DrawLine(pen, cL, tpY, cR, tpY);
            using (var f = new Font("Consolas", 7.5f))
            {
                using (var br = new SolidBrush(RED)) g.DrawString($"SL  {sl:F5}", f, br, cL + 3, slY + 2);
                using (var br = new SolidBrush(MINT)) g.DrawString($"TP  {tp:F5}", f, br, cL + 3, tpY - 12);
            }

            // Mumlar
            double p2 = 1.0828;
            for (int i = 0; i < nBar; i++)
            {
                double o = p2, c = p2 + (RNG.NextDouble() - .48) * .00035;
                double h = Math.Max(o, c) + RNG.NextDouble() * .00015;
                double l = Math.Min(o, c) - RNG.NextDouble() * .00015;
                p2 = c;
                float cx = cL + i * bW + bW / 2f;
                float yH = PY(h), yL = PY(l), yO = PY(o), yC = PY(c);
                bool bull = c >= o;
                Color col = bull ? MINT : RED;
                using (var pen = new Pen(Color.FromArgb(160, col.R, col.G, col.B), .8f)) g.DrawLine(pen, cx, yH, cx, yL);
                float top = Math.Min(yO, yC), bh = Math.Max(Math.Abs(yO - yC), 1.5f);
                using (var br = new SolidBrush(Color.FromArgb(bull ? 180 : 150, col.R, col.G, col.B)))
                    g.FillRectangle(br, cx - bW * .38f, top, bW * .76f, bh);
            }

            // Sinyal oklarını göster
            if (botRunning)
            {
                float arY = PY(livePrice - 0.0008f);
                using (var br = new SolidBrush(Color.FromArgb(180, MINT.R, MINT.G, MINT.B)))
                    g.FillPolygon(br, new PointF[] { new PointF(cR - 30, arY + 12), new PointF(cR - 22, arY), new PointF(cR - 14, arY + 12) });
                using (var f = new Font("Consolas", 7.5f, FontStyle.Bold)) using (var br = new SolidBrush(MINT))
                    g.DrawString("BUY", f, br, cR - 36, arY + 14);
            }

            // Canlı fiyat
            using (var pen = new Pen(Color.FromArgb(65, 255, 255, 255), .6f) { DashStyle = DashStyle.Dot }) g.DrawLine(pen, cL, curY, cR, curY);
            var tag = new Rectangle(cR - 68, (int)curY - 10, 68, 20);
            using (var br = new SolidBrush(Color.FromArgb(180, MINT.R, MINT.G, MINT.B))) g.FillRectangle(br, tag);
            using (var f = new Font("Consolas", 8f, FontStyle.Bold)) using (var br = new SolidBrush(BG))
            { var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center }; g.DrawString(livePrice.ToString("F5"), f, br, tag, sf); }

            // Bot aktif animasyon noktası
            if (botRunning)
            {
                using (var br = new SolidBrush(MINT)) g.FillEllipse(br, W - 18, 32, 8, 8);
                using (var f = new Font("Consolas", 7f)) using (var br = new SolidBrush(MINT)) g.DrawString("LIVE", f, br, W - 42, 33);
            }
        }

        // ── Card Header ───────────────────────────────────────────
        void CardHdr(Graphics g, Panel p, string title, Color acc)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (var pen = new Pen(Color.FromArgb(22, 255, 255, 255))) g.DrawRectangle(pen, 0, 0, p.Width - 1, p.Height - 1);
            if (p.Width > 2) using (var br = new LinearGradientBrush(new Point(0, 0), new Point(p.Width / 2, 0), Color.FromArgb(55, acc.R, acc.G, acc.B), Color.Transparent)) g.FillRectangle(br, 0, 0, p.Width / 2, 3);
            using (var br = new SolidBrush(Color.FromArgb(14, 255, 255, 255))) g.FillRectangle(br, 0, 3, p.Width, 23);
            using (var pen = new Pen(BDR)) g.DrawLine(pen, 0, 26, p.Width, 26);
            using (var br = new SolidBrush(acc)) g.FillEllipse(br, 8, 11, 4, 4);
            using (var f = new Font("Consolas", 7.5f, FontStyle.Bold)) using (var br = new SolidBrush(TXT3))
                g.DrawString(title.ToUpper(), f, br, 16, 8);
        }

        Panel WinBtn(Color col)
        {
            var p = new Panel { Size = new Size(12, 12), BackColor = col, Cursor = Cursors.Hand };
            p.Paint += (s, e) => { e.Graphics.SmoothingMode = SmoothingMode.AntiAlias; using (var br = new SolidBrush(p.BackColor)) e.Graphics.FillEllipse(br, 0, 0, 11, 11); };
            p.MouseEnter += (s, e) => p.BackColor = ControlPaint.Light(col, .3f);
            p.MouseLeave += (s, e) => p.BackColor = col;
            return p;
        }

        // ═════════════════════════════════════════════════════════
        //  CONNECT / BOT
        // ═════════════════════════════════════════════════════════
        async void ToggleConnect(object s, EventArgs e)
        {
            if (!connected)
            {
                btnConnect.Text = "◌  Connecting..."; btnConnect.Enabled = false;
                await System.Threading.Tasks.Task.Delay(1800);
                connected = true;
                btnConnect.Text = "⏹  DISCONNECT";
                btnConnect.ForeColor = RED; btnConnect.FlatAppearance.BorderColor = Color.FromArgb(70, RED.R, RED.G, RED.B);
                btnConnect.BackColor = Color.FromArgb(20, 255, 65, 95);
                btnConnect.Enabled = true;
                lblConnState.Text = $"● {platform} · {connServer}"; lblConnState.ForeColor = MINT;
                lblStatusTxt.Text = $"● Connected · {platform}"; lblStatusTxt.ForeColor = MINT;
                connServer = cmbServer?.SelectedItem?.ToString() ?? "MetaQuotes-Demo";
            }
            else
            {
                if (botRunning) StopBot(null, null);
                connected = false;
                btnConnect.Text = "▶  CONNECT";
                btnConnect.ForeColor = BLUE; btnConnect.FlatAppearance.BorderColor = Color.FromArgb(70, BLUE.R, BLUE.G, BLUE.B);
                btnConnect.BackColor = Color.FromArgb(22, 70, 155, 255);
                lblConnState.Text = "○  Not connected"; lblConnState.ForeColor = TXT2;
                lblStatusTxt.Text = "○ Disconnected"; lblStatusTxt.ForeColor = TXT2;
            }
            pnlTopBar?.Invalidate(); pnlSidebar?.Invalidate(true); pnlStatus?.Invalidate(); pnlStats?.Invalidate();
        }

        void StartBot(object s, EventArgs e)
        {
            if (!connected) { MessageBox.Show("Please connect to MT4/MT5 first!", "Not Connected", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            botRunning = true;
            btnBotStart.Enabled = false; btnBotStop.Enabled = true;
            lblBotState.Text = "● Bot Running — Scanning signals"; lblBotState.ForeColor = MINT;
            lblStatusTxt.Text = "● Bot Running"; lblStatusTxt.ForeColor = MINT;
            botTimer.Start();
            pnlChart?.Invalidate(); pnlStats?.Invalidate();
        }

        void StopBot(object s, EventArgs e)
        {
            botRunning = false;
            botTimer?.Stop();
            btnBotStart.Enabled = true; btnBotStop.Enabled = false;
            lblBotState.Text = "● Bot Stopped"; lblBotState.ForeColor = AMBER;
            lblStatusTxt.Text = "● Connected — Bot Stopped"; lblStatusTxt.ForeColor = AMBER;
            pnlChart?.Invalidate(); pnlStats?.Invalidate();
        }

        // ═════════════════════════════════════════════════════════
        //  TIMERS
        // ═════════════════════════════════════════════════════════
        void StartTimers()
        {
            ticker = new System.Windows.Forms.Timer { Interval = 1600 };
            ticker.Tick += (s, e) => {
                livePrice += (RNG.NextDouble() - .49) * .00007;
                double chg = livePrice - 1.08291;
                if (lblPrice != null) lblPrice.Text = livePrice.ToString("F5");
                if (lblPriceChg != null) { lblPriceChg.Text = (chg >= 0 ? "▲ +" : "▼ ") + chg.ToString("F5"); lblPriceChg.ForeColor = chg >= 0 ? MINT : RED; }
                if (lblClock != null) lblClock.Text = DateTime.Now.ToString("HH:mm:ss  ·  dd.MM.yyyy");
                pnlChart?.Invalidate();
                pnlSidebar?.Invalidate(true);
                pnlStatus?.Invalidate();
            };
            ticker.Start();

            botTimer = new System.Windows.Forms.Timer { Interval = 4500 };
            botTimer.Tick += (s, e) => {
                if (!botRunning || !connected) return;
                // sinyal üret
                bool isBuy = RNG.NextDouble() > .5;
                bool isWin = RNG.NextDouble() > .38;
                double entry = livePrice;
                double sl = isBuy ? entry - slPips * .0001 : entry + slPips * .0001;
                double tp = isBuy ? entry + tpPips * .0001 : entry - tpPips * .0001;
                double pnl = isWin ? tpPips * lotSize * (selPair == "USDJPY" ? .9 : 1.0) : -(slPips * lotSize * (selPair == "USDJPY" ? .9 : 1.0));

                var sg = new Signal
                {
                    Time = DateTime.Now.ToString("HH:mm:ss"),
                    Pair = selPair,
                    TF = selTF,
                    Dir = isBuy ? "BUY" : "SELL",
                    Entry = entry.ToString("F5"),
                    SL = sl.ToString("F5"),
                    TP = tp.ToString("F5"),
                    Lot = lotSize.ToString("F2"),
                    Status = "CLOSED",
                    PnL = (pnl >= 0 ? "+" : "") + $"${pnl:N2}",
                    IsWin = isWin,
                };
                signals.Insert(0, sg); AppendSignal(sg);
                botTrades++;
                if (isWin) botWins++;
                botPnl += pnl;
                balance += pnl;
                pnlStats?.Invalidate(); pnlSidebar?.Invalidate(true); pnlStatus?.Invalidate();
            };
        }

        protected override void OnFormClosed(FormClosedEventArgs e) { ticker?.Stop(); botTimer?.Stop(); base.OnFormClosed(e); }
    }

    static class NativeMethods
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")] public static extern bool ReleaseCapture();
        [System.Runtime.InteropServices.DllImport("user32.dll")] public static extern IntPtr SendMessage(IntPtr h, int m, IntPtr w, IntPtr l);
    }
}
