<div align="center">

<img src="https://api.iconify.design/solar/global-linear.svg?color=%236E7681&width=18" align="top" /> &nbsp;<sub><b>Choose your language</b> &nbsp;·&nbsp; <b>Escolha seu idioma</b> &nbsp;·&nbsp; <b>Выберите язык</b></sub>

<a href="VENDORS.md"><img src="https://api.iconify.design/flag/us-4x3.svg?width=21" align="top" alt="English" /> <img src="https://img.shields.io/badge/English-1F6FEB?style=flat-square" alt="English" /></a>
&nbsp;&nbsp;
<a href="docs/languages/VENDORS.pt-BR.md"><img src="https://api.iconify.design/flag/br-4x3.svg?width=21" align="top" alt="Português" /> <img src="https://img.shields.io/badge/Portugu%C3%AAs-6E7681?style=flat-square" alt="Português" /></a>
&nbsp;&nbsp;
<a href="docs/languages/VENDORS.ru.md"><img src="https://api.iconify.design/flag/ru-4x3.svg?width=21" align="top" alt="Русский" /> <img src="https://img.shields.io/badge/%D0%A0%D1%83%D1%81%D1%81%D0%BA%D0%B8%D0%B9-6E7681?style=flat-square" alt="Русский" /></a>

</div>


<br>

This document covers a change made on top of [EmuWarface](https://github.com/n1kodim/EmuWarface), the C# Warface server emulator by **n1kodim** targeting the **DEV20 1.22400.5519.45100** build.

<br>

<h2><img src="https://api.iconify.design/solar/question-circle-bold.svg?color=%233B82F6&width=26" align="top" /> &nbsp;What vendors are</h2>

In game, the **VENDORS** tab is where a player unlocks weapons, gear and attachments. Each vendor holds a list of items, and the player unlocks them one at a time by earning **vendor points** through matches.

There are three vendors, numbered `0` through `2`.

<br>

<h2><img src="https://api.iconify.design/solar/refresh-circle-bold.svg?color=%233B82F6&width=26" align="top" /> &nbsp;What changed</h2>

Upstream, the emulator replied with **zero hardcoded points** for every player, ignoring the database. The `emu_sponsors` table already existed and was already filled on profile creation, but nothing ever read it back.

| | Before | After |
|---|---|---|
| Point source | Value hardcoded in the source | `emu_sponsors` table, per profile |
| Configurable | No | Yes, through `settings.json` |
| Per-player progress | Did not exist | Each profile keeps its own |
| Database point ceiling | 255 (`tinyint`) | 4,294,967,295 (`int unsigned`) |

<br>

<h2><img src="https://api.iconify.design/solar/settings-bold.svg?color=%233B82F6&width=26" align="top" /> &nbsp;Vendor configuration</h2>

New block in **`EmuWarface/Config/settings.json`**, starting at line 13:

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
> Editing `settings.json` requires restarting the emulator. The file is read once, at startup.

### Reverting to the previous behaviour

Change one word:

```json
"unlock_all": true
```

Restart the emulator. Everyone sees everything unlocked again, exactly as before the change.

> [!IMPORTANT]
> Switching back to `true` **does not erase** stored progress. Points stay in `emu_sponsors`. Flip it back to `false` later and every player finds precisely what they had.

<details>
<summary><b>What if I want to undo the code change, not just the setting?</b></summary>

<br>

Reverting the code means removing the changes listed further down. It is not required to restore the old behaviour: `unlock_all: true` produces the same result with no editing and no rebuild.

If you want the pristine source anyway, pull it again from upstream:

```bash
git clone https://github.com/n1kodim/EmuWarface
```

</details>

<br>

<h2><img src="https://api.iconify.design/solar/tuning-square-bold.svg?color=%233B82F6&width=26" align="top" /> &nbsp;Everything you can customise</h2>

Files live under `EmuWarface/Config/`. All of them require an emulator restart.

### Vendors &nbsp;·&nbsp; `settings.json`

**Give newcomers a head start:**

```json
"starting_points": 500
```

Applies only to profiles created after the change. Existing players are untouched.

**Adjust current players** straight in the database:

```sql
-- one player, one vendor
UPDATE emu_sponsors SET sponsor_points = 1500
WHERE profile_id = 7 AND sponsor_id = 0;

-- one player, all three vendors
UPDATE emu_sponsors SET sponsor_points = 1500 WHERE profile_id = 7;

-- reset everyone to zero
UPDATE emu_sponsors SET sponsor_points = 0;
```

**Inspect current progress:**

```sql
SELECT p.nickname, s.sponsor_id, s.sponsor_points
FROM emu_sponsors s
JOIN emu_profiles p ON p.profile_id = s.profile_id
ORDER BY p.nickname, s.sponsor_id;
```

Players see the change at their next login.

### How many players start a match &nbsp;·&nbsp; `room.json`

```json
{
    "min_players_ready_pvp_public": 1,
    "min_players_ready_pvp_autostart": 2,
    "min_players_ready_pve_private": 1,
    "min_players_ready_pve_autostart": 1,
    "min_players_ready_pvp_clanwar": 1,
    "min_players_ready_pvp_rating": 4
}
```

How many players must be ready before a match begins, per room type. On a small server for friends, setting everything to `1` avoids waiting for a room to fill. On a busier server, higher values give more balanced matches.

### Game channels &nbsp;·&nbsp; `masterservers.json`

Four channels ship configured: `pve_001`, `pvp_newbie_001`, `pvp_skilled_001` and `pvp_pro_001`.

```json
{
    "server_id": 1,
    "resource": "pve_001",
    "channel": "pve",
    "rank_group": "all",
    "min_rank": 1,
    "max_rank": 90,
    "bootstrap": ""
}
```

`min_rank` and `max_rank` set which ranks can see the channel. All ship open from 1 to 90, which puts everyone in the same place. To keep beginners away from veterans, narrow the bands: `pvp_newbie` 1 to 10, `pvp_skilled` 11 to 25, `pvp_pro` 26 to 90.

> [!WARNING]
> Bands that do not touch leave gaps. If `newbie` ends at 10 and `skilled` starts at 15, players between 11 and 14 get no PvP channel at all.

### Items every player receives on account creation &nbsp;·&nbsp; `defaultItems.json`

72 starting items:

```json
{ "name": "pt05_shop", "type": "Pistol", "classes": 29 }
```

`classes` is a bit sum choosing which classes may use the item: Rifleman 1, Medic 2, Engineer 4, Sniper 8, Heavy 16. The `29` above is 1+4+8+16, so every class except Medic. Use `31` for all of them.

### Network and debugging &nbsp;·&nbsp; `settings.json`

| Field | Purpose |
|---|---|
| `host` | Address the emulator listens on. `127.0.0.1` accepts this machine only. |
| `port` | XMPP port. Defaults to `5222`. |
| `dedicatedHosts` | Addresses allowed to register dedicated servers. |
| `gameVersion` | Client version accepted. Must match the game. |
| `xmpp_debug` | Records XMPP traffic. Useful for diagnosis, heavy for daily use. |
| `xmpp_debug_console` | Mirrors that traffic to the console. |
| `use_online_protect` | Enables online player count protection. |

<br>

<h2><img src="https://api.iconify.design/solar/star-bold.svg?color=%2322C55E&width=26" align="top" /> &nbsp;Recommended setups</h2>

Three combinations that work, depending on your use.

<details open>
<summary><b>Home server, you and a few friends</b></summary>

<br>

The goal is to jump in and play, with no waiting and no grind.

```json
"sponsors": {
    "unlock_all": true,
    "unlock_all_points": 999999,
    "starting_points": 0
}
```

And in `room.json`, everything at `1`, so matches start as soon as anyone is ready:

```json
{
    "min_players_ready_pvp_public": 1,
    "min_players_ready_pvp_autostart": 1,
    "min_players_ready_pve_private": 1,
    "min_players_ready_pve_autostart": 1,
    "min_players_ready_pvp_clanwar": 1,
    "min_players_ready_pvp_rating": 1
}
```

**Why `unlock_all: true` here:** it removes the grind entirely, which suits an afternoon with friends. If you would rather have progression, `false` works too: points are credited at the end of each match.

</details>

<details>
<summary><b>Progression server, the way the original plays</b></summary>

<br>

Every player starts from zero and works up.

```json
"sponsors": {
    "unlock_all": false,
    "unlock_all_points": 999999,
    "starting_points": 0
}
```

Rank-separated channels in `masterservers.json`, so beginners do not land against veterans:

| Channel | `min_rank` | `max_rank` |
|---|---|---|
| `pvp_newbie_001` | 1 | 10 |
| `pvp_skilled_001` | 11 | 25 |
| `pvp_pro_001` | 26 | 90 |

> [!NOTE]
> Points are credited at the end of each match, taken from the `SponsorPointsMultiplier` in the gamedata. The SQL commands above stay useful for adjusting a specific player.

</details>

<details>
<summary><b>Small public server</b></summary>

<br>

A middle ground: players do not start from absolute zero, but still have something to earn.

```json
"sponsors": {
    "unlock_all": false,
    "unlock_all_points": 999999,
    "starting_points": 300
}
```

And turn debugging off, since it costs performance once people connect:

```json
"xmpp_debug": false,
"xmpp_debug_console": false
```

**Why `starting_points: 300`:** newcomers find a few options already open and do not quit at the first screen, while most of the catalogue stays something to earn.

</details>

<br>

<h2><img src="https://api.iconify.design/solar/shield-warning-bold.svg?color=%23F97316&width=26" align="top" /> &nbsp;Precautions</h2>

> [!WARNING]
> `Config/sql.json` holds the **database password**. Never publish that file filled in to a public repository, and never send it along with the project.

> [!CAUTION]
> The emulator port has no strong authentication. When exposing it to the internet, use a firewall and open only what is needed. A home server belongs on `127.0.0.1` or behind a VPN.

<br>

<h2><img src="https://api.iconify.design/solar/file-text-bold.svg?color=%233B82F6&width=26" align="top" /> &nbsp;Files changed</h2>

All under `EmuWarface/`:

| File | Line | What was done |
|---|---|---|
| `Config/settings.json` | 13 to 17 | `sponsors` block added |
| `Config.cs` | 113, 123 | `Sponsors` property and `SponsorsConfig` class |
| `Game/Profile.cs` | 453 | `SponsorsSerialize()` method, choosing between unlock-all and reading the database |
| `Game/Profile.cs` | 530 | `AddSponsorPoints()`, which credits the points earned in a match |
| `Game/Profile.cs` | 618 to 621 | The three new-profile `INSERT` statements now use `starting_points` |
| `Xmpp/Query/JoinChannel.cs` | 53 | Five hardcoded lines replaced by one method call |
| `Xmpp/Query/CreateProfile.cs` | 70 | Same replacement |
| `Xmpp/Query/SetRewardsInfo.cs` | 324 | Credits the points to the profile at the end of the match |
| `Xmpp/Query/SetRewardsInfo.cs` | 344 | Sends the calculated value to the client, instead of the hardcoded zero |
| `emuwarface.sql` | 289 | `sponsor_points` widened from `tinyint` to `int unsigned` |

<details>
<summary><b>The code that was there before</b></summary>

<br>

In `JoinChannel.cs` and `CreateProfile.cs`, the replaced block read:

```csharp
//TODO sponsors
XmlElement sponsor_info = Xml.Element("sponsor_info");
sponsor_info.Child(Xml.Element("sponsor").Attr("sponsor_id", "0").Attr("sponsor_points", "0").Attr("next_unlock_item", ""));
sponsor_info.Child(Xml.Element("sponsor").Attr("sponsor_id", "1").Attr("sponsor_points", "0").Attr("next_unlock_item", ""));
sponsor_info.Child(Xml.Element("sponsor").Attr("sponsor_id", "2").Attr("sponsor_points", "0").Attr("next_unlock_item", ""));
character.Child(sponsor_info);
```

The author's own `//TODO sponsors` marks it as a placeholder awaiting implementation.

</details>

<br>

<h2><img src="https://api.iconify.design/solar/danger-triangle-bold.svg?color=%23F97316&width=26" align="top" /> &nbsp;Known limitations</h2>

| Limitation | Reason |
|---|---|
| `next_unlock_item` stays empty | It depends on each vendor's item table, which the emulator does not load yet. |
| Profiles created before the change may have no `emu_sponsors` rows | Handled automatically: the method creates the missing rows on first login, using `starting_points`. |

<br>

<h2><img src="https://api.iconify.design/solar/checklist-minimalistic-bold.svg?color=%2322C55E&width=26" align="top" /> &nbsp;Verification</h2>

The project was compiled after the changes:

```
26 Warning(s)
 0 Error(s)
```

All 26 warnings predate this work and are unrelated to these changes.

<br>

<h2><img src="https://api.iconify.design/solar/heart-bold.svg?color=%2322C55E&width=26" align="top" /> &nbsp;Credits</h2>

EmuWarface is the work of **[n1kodim](https://github.com/n1kodim)** and **[CryHub](https://github.com/WFCRYHUB)**, released under the MIT license. All credit for the emulator belongs to them.

This repository is a copy carrying one focused change to the vendor system, documented above. The original license is preserved in [LICENSE](LICENSE), and the project README remains at [README.md](README.md).

<br>

<div align="center">
<sub>

A change on top of [EmuWarface](https://github.com/n1kodim/EmuWarface) &nbsp;·&nbsp; MIT License &nbsp;·&nbsp; DEV20 build 1.22400.5519.45100

</sub>
</div>
