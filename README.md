<div align="center">

# ▦ Volume Profile MT5/MT4

**TradingView-style Volume Profile indicator for MetaTrader 5 and MetaTrader 4 — with POC, Value Area, Buy/Sell split, and session stats. All inside a clean desktop manager.**

<br>

[![Stars](https://img.shields.io/github/stars/torvalds/linux?style=for-the-badge&color=00D4AA&label=Stars)](https://github.com/your-username/volume-profile-mt5/stargazers)
[![Forks](https://img.shields.io/github/forks/torvalds/linux?style=for-the-badge&color=4D9FFF&label=Forks)](https://github.com/your-username/volume-profile-mt5/network)
[![Issues](https://img.shields.io/github/issues/torvalds/linux?style=for-the-badge&color=FF4D6A&label=Issues)](https://github.com/your-username/volume-profile-mt5/issues)
[![Platform](https://img.shields.io/badge/MT5%20%2F%20MT4-Compatible-00D4AA?style=for-the-badge)](https://www.metatrader5.com)
[![License](https://img.shields.io/badge/License-MIT-4D9FFF?style=for-the-badge)](LICENSE)

</div>

---

## 📸 Screenshot

<div align="center">

<p align="center">
  <img src="https://i.ibb.co/vxVgG0kd/auto.png" alt="photo" width="820">
</p>

</div>


---

## 🎬 Demo

<div align="center">

<img src="https://i.imgur.com/crT6bXq.gif" alt="Demo">

</div>

---

## Why Volume Profile?

Most MT5/MT4 traders rely on time-based indicators that tell you *when* something happened. Volume Profile tells you *where* the real trading activity is concentrated — which price levels attracted the most volume, where institutions loaded positions, and where the market is likely to react again.

If you trade with SMC, ICT, or pure price action, Volume Profile is the missing layer.

---

## What It Does

**Volume Profile MT5** renders a TradingView-style horizontal volume histogram directly on your MT5/MT4 chart and exposes all key levels through a dedicated desktop manager:

| Level | Description |
|---|---|
| POC — Point of Control | The single price level with the highest traded volume in the session |
| VAH — Value Area High | Upper boundary of the zone where 70% of volume was traded |
| VAL — Value Area Low | Lower boundary of the same 70% value area |
| Value Area | The full high-volume zone between VAH and VAL — the institutional range |
| Buy / Sell Split | Each bar split into buy-side and sell-side volume for delta analysis |
| Session Stats | High, low, range, and dominant side (buy vs sell) per trading session |

---

## Features

| Feature | Description |
|---|---|
| Live Price Feed | Real-time price ticker with tick-by-tick updates from your connected MT5 account |
| Multiple Profile Types | Session, Fixed Range, Visible Range, and Daily profile modes |
| POC Line | Dashed gold line across the chart marking the Point of Control |
| Value Area Bands | Dotted teal/red lines marking VAH and VAL with price labels |
| Buy / Sell Split Mode | Toggle to split each volume bar into buy-side (teal) and sell-side (red) delta |
| Timeframe Selector | Switch between M1, M5, M15, H1, H4, D1 without leaving the manager |
| Key Levels Panel | Live readout of POC, VAH, VAL, Value Area %, and total session volume |
| Session Stats Panel | Current session (London / New York / Tokyo), high, low, range, dominant side |
| Levels Table | Full sortable table of all VP levels with price, volume, buy %, sell %, and a visual distribution bar |
| MT5 Connection Status | Live connection indicator showing your server name and account login |
| Apply / Reset | Push updated settings to the MT5 terminal instantly with animated feedback |

---

## 🛡️ Designed for Real Trading Conditions

- **No repainting** — all levels are calculated on closed bars only
- **No lag** — volume histogram is rendered natively, not as an overlay on a separate chart
- **Disconnection safe** — the manager detects MT5 disconnections and blocks all operations until reconnected
- **Session-aware** — automatically detects London, New York, and Tokyo sessions

---

## Quick Start

**Requirements:**
- Windows 10 or 11
- [.NET 6 or higher](https://dotnet.microsoft.com/en-us/download)
- MetaTrader 5 or MetaTrader 4 terminal, logged into any account
- Visual Studio 2022 (to build from source)

```bash
# Clone the repository
git clone https://github.com/your-username/volume-profile-mt5.git

# Open in Visual Studio 2022
# File → Open → VolumeProfile.sln

# Build and run — press F5
```

> No external NuGet packages required. Builds and runs out of the box on any Windows machine with .NET 6+.

---

## How to Use

1. Launch the application
2. Confirm the sidebar shows **CONNECTED** with your server and login details
3. Select your **Profile Type** — Session, Fixed Range, Visible Range, or Daily
4. Choose your **Timeframe** from the top bar (M1 → D1)
5. Toggle **Show POC** and **Show Value Area** as needed
6. Enable **Buy / Sell Split** to see delta distribution per level
7. Click **▶ Apply** — settings are pushed to the MT5 terminal
8. Use the **Key Levels** and **Session Stats** panels for a quick reference at a glance
9. The **Levels Table** at the bottom shows the full breakdown with visual distribution bars

---

## Reading the Chart

```
VAH ─────────────────────────────────────  ← Top of 70% volume zone
     ████████████████████████████████████
     ██████████████████████████
POC ══════════════════════════════════════  ← Highest volume level (gold dashed)
     ██████████████████████████████████████████  (widest bar)
     ████████████████████
VAL ─────────────────────────────────────  ← Bottom of 70% volume zone
```

- **Wide bars** = high volume = price acceptance — market spent time here
- **Narrow bars** = low volume = price rejection — market moved through quickly
- **POC** is the strongest magnet level — price tends to return to it
- **VAH / VAL** act as support and resistance — breaks with volume confirm trend

---

## Roadmap

- [x] Volume Profile with POC, VAH, VAL
- [x] Buy / Sell split (delta mode)
- [x] Session stats (London / NY / Tokyo)
- [x] Live price ticker
- [ ] Multi-session overlay — show multiple session profiles on one chart
- [ ] Alert system — get notified when price returns to POC or breaks Value Area
- [ ] Export levels to CSV for backtesting
- [ ] MT4 build — full parity with the MT5 version
- [ ] Telegram integration — send key levels to your phone automatically

> Have a feature request? [Open an issue](https://github.com/your-username/volume-profile-mt5/issues) and describe what you need.

---

## Contributing

Pull requests are welcome. For significant changes, please open an issue first.

```
1. Fork the repository
2. Create a feature branch:  git checkout -b feature/multi-session
3. Commit your changes:      git commit -m "Add: multi-session overlay"
4. Push to your fork:        git push origin feature/multi-session
5. Open a Pull Request
```

---

## License

[MIT](LICENSE) — free to use, modify, and distribute.

---

<div align="center">

**Volume Profile MT5/MT4** &nbsp;·&nbsp; Built for traders, by traders &nbsp;·&nbsp; v2.1.0

</div>