<div align="center">

<img src="https://api.iconify.design/solar/global-linear.svg?color=%236E7681&width=18" align="top" /> &nbsp;<sub><b>Choose your language</b> &nbsp;·&nbsp; <b>Escolha seu idioma</b> &nbsp;·&nbsp; <b>Выберите язык</b></sub>

<a href="README.md"><img src="https://api.iconify.design/flag/us-4x3.svg?width=21" align="top" alt="English" /> <img src="https://img.shields.io/badge/English-1F6FEB?style=flat-square" alt="English" /></a>
&nbsp;&nbsp;
<a href="docs/languages/README.pt-BR.md"><img src="https://api.iconify.design/flag/br-4x3.svg?width=21" align="top" alt="Português" /> <img src="https://img.shields.io/badge/Portugu%C3%AAs-6E7681?style=flat-square" alt="Português" /></a>
&nbsp;&nbsp;
<a href="docs/languages/README.ru.md"><img src="https://api.iconify.design/flag/ru-4x3.svg?width=21" align="top" alt="Русский" /> <img src="https://img.shields.io/badge/%D0%A0%D1%83%D1%81%D1%81%D0%BA%D0%B8%D0%B9-6E7681?style=flat-square" alt="Русский" /></a>

</div>


<br>

A backend server emulator for Warface written in C#, targeting the **DEV20 1.22400.5519.45100** build.

This repository is a copy of the [original EmuWarface](https://github.com/n1kodim/EmuWarface) by **n1kodim**, with his commit history preserved, plus one change: **vendor progression** is no longer hardcoded and can now be configured.

<br>

<h2><img src="https://api.iconify.design/solar/checklist-minimalistic-bold.svg?color=%233B82F6&width=26" align="top" /> &nbsp;Requirements</h2>

<img src="https://api.iconify.design/devicon/dotnetcore.svg?width=18" align="top" /> &nbsp;[.NET 6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)

<img src="https://api.iconify.design/devicon/mysql-wordmark.svg?width=18" align="top" /> &nbsp;[MySQL](https://dev.mysql.com/downloads/installer/) or MariaDB 10.9.2

The database is created from `EmuWarface/emuwarface.sql`.

To start with game data ready to go, use the [upstream release](https://github.com/n1kodim/EmuWarface/releases/latest), which ships with gamedata.

<br>

<h2><img src="https://api.iconify.design/solar/star-bold.svg?color=%2322C55E&width=26" align="top" /> &nbsp;What this repository changes</h2>

On the **VENDORS** tab, players unlock weapons, gear and attachments by earning points. The original emulator replied with **hardcoded points**, identical for everyone, ignoring the database. The `emu_sponsors` table already existed and was already filled on profile creation, but nothing read it back: the author had left a `//TODO sponsors` in its place.

| | Upstream | Here |
|---|---|---|
| Point source | Value hardcoded in the source | `emu_sponsors` table, per profile |
| Configurable | No | Yes, through `settings.json` |
| Per-player progress | Did not exist | Each profile keeps its own |
| Database point ceiling | 255 (`tinyint`) | 4,294,967,295 (`int unsigned`) |

The **[Manual](MANUAL.md)** covers installation, accounts, the 11 admin commands, the 98 protocol commands and what does not work yet.

Documentation for the change lives in **[VENDORS.md](VENDORS.md)**: every file and line changed, how to revert, everything you can customise, and three recommended setups.

<br>

<h2><img src="https://api.iconify.design/solar/settings-bold.svg?color=%233B82F6&width=26" align="top" /> &nbsp;Quick configuration</h2>

In `EmuWarface/Config/settings.json`:

```json
"sponsors": {
    "unlock_all": false,
    "unlock_all_points": 999999,
    "starting_points": 0
}
```

| Field | What it does | Default |
|---|---|---|
| `unlock_all` | `true` unlocks everything for everyone. `false` uses per-player progression. | `false` |
| `unlock_all_points` | Point value to report while `unlock_all` is on. | `999999` |
| `starting_points` | Points a brand new profile is created with. | `0` |

> [!NOTE]
> To restore upstream behaviour, set `"unlock_all": true` and restart the emulator. Nothing is erased: points stay stored in the database.

<br>

<h2><img src="https://api.iconify.design/solar/danger-triangle-bold.svg?color=%23F97316&width=26" align="top" /> &nbsp;Known limitations</h2>

| Limitation | Reason |
|---|---|
| `next_unlock_item` stays empty | It depends on each vendor's item table, which the emulator does not load yet. |

> [!NOTE]
> With `unlock_all: false`, players earn vendor points by playing: the amount comes from the `SponsorPointsMultiplier` in the gamedata and is credited at the end of each match.

<br>

<h2><img src="https://api.iconify.design/solar/shield-warning-bold.svg?color=%23F97316&width=26" align="top" /> &nbsp;Security</h2>

> [!CAUTION]
> `EmuWarface/Config/sql.json` holds the **database password**. Never publish that file filled in, and never ship it with the project.

The emulator port has no strong authentication. When exposing it to the internet, use a firewall and open only what is needed. A home server belongs on `127.0.0.1` or behind a VPN.

<br>

<h2><img src="https://api.iconify.design/solar/heart-bold.svg?color=%2322C55E&width=26" align="top" /> &nbsp;Credits</h2>

<div align="center">

<table>
<tr>
<td align="center" width="150">
<a href="https://github.com/n1kodim">
<img src="https://github.com/n1kodim.png?size=100" width="80" alt="n1kodim" /><br />
<sub><b>n1kodim</b></sub>
</a><br />
<sub>14 commits</sub>
</td>
<td align="center" width="150">
<a href="https://github.com/myrka32">
<img src="https://github.com/myrka32.png?size=100" width="80" alt="myrka32" /><br />
<sub><b>myrka32</b></sub>
</a><br />
<sub>1 commit</sub>
</td>
</tr>
</table>

</div>

EmuWarface is the work of **[n1kodim](https://github.com/n1kodim)**, with a contribution from **[myrka32](https://github.com/myrka32)**, released by **[CryHub](https://github.com/WFCRYHUB)** under the MIT license. All credit for the emulator belongs to them.

The 15 original commits are preserved in this repository's history with their authorship intact, including [the README the author wrote](https://github.com/OTheMandaloriano/EmuWarface/blob/a9a5638/README.md). The original license is at [LICENSE](LICENSE).

<br>

<div align="center">
<sub>

Based on [EmuWarface](https://github.com/n1kodim/EmuWarface) by n1kodim &nbsp;·&nbsp; MIT License &nbsp;·&nbsp; DEV20 build 1.22400.5519.45100

</sub>
</div>
