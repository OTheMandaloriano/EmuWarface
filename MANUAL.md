<div align="center">

<img src="https://api.iconify.design/solar/global-linear.svg?color=%236E7681&width=18" align="top" /> &nbsp;<sub><b>Choose your language</b> &nbsp;·&nbsp; <b>Escolha seu idioma</b> &nbsp;·&nbsp; <b>Выберите язык</b></sub>

<a href="MANUAL.md"><img src="https://api.iconify.design/flag/us-4x3.svg?width=21" align="top" alt="English" /> <img src="https://img.shields.io/badge/English-1F6FEB?style=flat-square" alt="English" /></a>
&nbsp;&nbsp;
<a href="docs/languages/MANUAL.pt-BR.md"><img src="https://api.iconify.design/flag/br-4x3.svg?width=21" align="top" alt="Português" /> <img src="https://img.shields.io/badge/Portugu%C3%AAs-6E7681?style=flat-square" alt="Português" /></a>
&nbsp;&nbsp;
<a href="docs/languages/MANUAL.ru.md"><img src="https://api.iconify.design/flag/ru-4x3.svg?width=21" align="top" alt="Русский" /> <img src="https://img.shields.io/badge/%D0%A0%D1%83%D1%81%D1%81%D0%BA%D0%B8%D0%B9-6E7681?style=flat-square" alt="Русский" /></a>

</div>


<br>

Reference document for the server. For the project overview see the [README](README.md); for the vendor system, [VENDORS.md](VENDORS.md).

<br>

<h2><img src="https://api.iconify.design/solar/box-bold.svg?color=%233B82F6&width=26" align="top" /> &nbsp;How the server works</h2>

The retail game talks to the developer's servers for everything that is not the shooting itself: login, profile, inventory, rooms, clans, store. EmuWarface puts a server of your own in their place.

Four pieces are involved:

| Piece | Role |
|---|---|
| **EmuWarface** | Answers the client: login, profile, rooms, items |
| **MySQL** | Stores accounts, profiles, inventory, clans |
| **GameData** | Game configuration files (missions, items, rewards) |
| **Game client** | Warface itself, pointed at your server |

They talk over **XMPP**, on port **5222**.

> [!IMPORTANT]
> EmuWarface does **not** host the match itself. It handles the lobby, profiles and rooms. The combat runs on `DedicatedServer.exe`, which ships with the game client.

<br>

<h2><img src="https://api.iconify.design/solar/download-bold.svg?color=%233B82F6&width=26" align="top" /> &nbsp;Installation</h2>

### What you need

<img src="https://api.iconify.design/devicon/dotnetcore.svg?width=18" align="top" /> &nbsp;[.NET 6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)

<img src="https://api.iconify.design/devicon/mysql-wordmark.svg?width=18" align="top" /> &nbsp;[MySQL](https://dev.mysql.com/downloads/installer/) or MariaDB 10.9.2

<img src="https://api.iconify.design/solar/gamepad-bold.svg?color=%236E7681&width=18" align="top" /> &nbsp;A Warface client, build **DEV20 1.22400.5519.45100**

> [!WARNING]
> The source tree **does not include the `GameData` folder**, and the server will not start without it. It ships only in releases. If you cloned the repository, download a [release](https://github.com/OTheMandaloriano/EmuWarface/releases) and copy `GameData` from it next to the executable.

### Step by step

**1. Database.** Create it and import the schema:

```bash
mysql -u root -p -e "CREATE DATABASE emuwarface CHARACTER SET utf8mb4"
mysql -u root -p emuwarface < EmuWarface/emuwarface.sql
```

That creates the 26 tables and inserts test accounts.

**2. Database connection.** In `Config/sql.json`:

```json
{
    "server": "127.0.0.1",
    "user": "root",
    "password": "your_password_here",
    "database": "emuwarface",
    "characterSet": "utf8mb4",
    "port": 3306
}
```

**3. Server.** In `Config/settings.json`, check `host` and `port`. For local use, `127.0.0.1` and `5222` are fine.

**4. Start it:**

```bash
EmuWarface.exe
```

Or, from source:

```bash
dotnet run --project EmuWarface
```

**5. Point the client** at your server, in the game's `online.cfg`:

```
online_host = warface
online_server = 127.0.0.1
```

<br>

<h2><img src="https://api.iconify.design/solar/users-group-rounded-bold.svg?color=%233B82F6&width=26" align="top" /> &nbsp;Accounts and permissions</h2>

The schema ships with accounts ready for testing:

| Login | Password | Permission |
|---|---|---|
| `user1` to `user4` | `12345` | Admin |

> [!CAUTION]
> These accounts are public: anyone who knows the project knows the password. Before opening your server to other people, change the passwords and lower the permissions.

### Creating an account

**There is no in-game sign-up.** Accounts go straight into the database:

```sql
INSERT INTO emu_users (login, password, token, permission, ipaddress)
VALUES ('newplayer', 'password123', '1', 0, '');
```

### The four levels

| Level | Value | What it allows |
|---|---|---|
| `None` | 0 | Playing. Uses `give`, `help` and `online` |
| `Give` | 1 | Reserved for granting items |
| `Moderator` | 2 | Ban, kick, mute, list rooms |
| `Admin` | 3 | Everything |

```sql
UPDATE emu_users SET permission = 2 WHERE login = 'newplayer';
```

<br>

<h2><img src="https://api.iconify.design/solar/command-bold.svg?color=%2322C55E&width=26" align="top" /> &nbsp;Admin commands</h2>

Eleven commands, usable two ways: typed **in the server console** (always with Admin power) or **from inside the game**, respecting the account's permission.

| Command | Alias | Permission | What it does |
|---|---|---|---|
| `give` | `g` | None | Grant items, currency and achievements |
| `help` | | None | List available commands |
| `online` | | None | How many players are connected |
| `ban` | | Moderator | Ban an account |
| `unban` | `ub` | Moderator | Lift a ban |
| `kick` | `k` | Moderator | Disconnect a player |
| `mute` | `m` | Moderator | Silence in chat |
| `unmute` | `um` | Moderator | Give the voice back |
| `broadcast` | `bc` | Moderator | Server-wide announcement |
| `rooms` | `r` | Moderator | List open rooms |
| `setexp` | `exp` | Moderator | Set someone's experience |

### The `give` command in detail

The richest one, accepting several types:

```bash
give user1 p ar29_shop            # permanent weapon
give user1 e ar29_shop 10d        # for 10 days
give user1 s sniper_fbs_01        # skin
give user1 b random_box_10        # random box
give user1 m game 10000           # 10,000 WF$
give user1 m crown 10000          # 10,000 crowns
give user1 m cry 10000            # 10,000 kredits
give user1 a 2231                 # achievement
```

| Type | Alias | What it grants |
|---|---|---|
| `permanent` | `p` | Item forever |
| `expiration` | `e` | Item with a deadline (`10d`, `3h`) |
| `skin` | `s` | Weapon skin |
| `box` | `b` | Random box |
| `money` | `m` | Currency: `game`, `crown` or `cry` |
| `achiev` | `a` | Achievement, by number |

> [!NOTE]
> `give user1 all` grants the entire catalogue, but only works with `use_online_protect` turned off.

### Banning and muting

Both take a duration. Without one, it is permanent:

```bash
ban user1 7d          # seven days
mute user1 2h         # two hours
kick user1
```

<br>

<h2><img src="https://api.iconify.design/solar/checklist-bold.svg?color=%2322C55E&width=26" align="top" /> &nbsp;What the server does</h2>

**98 protocol commands** are served, spread across 91 files. By area:

| Area | Count | What it covers |
|---|---|---|
| **Rooms and matches** | 23 | Create, join, teams, voting, autostart, reconnect, quick play |
| **Profile and account** | 21 | Login, creation, classes, appearance, stats, tutorial |
| **Server** | 11 | Channels, masterservers, anticheat, telemetry, commands |
| **Items and store** | 8 | Buy, consume, extend, cards, expired items |
| **Clans** | 7 | Create, invite, kick, roles, listing |
| **Social** | 7 | Friends, invites, messages, chat, reports |
| **Progression** | 6 | Achievements, contracts, reward multipliers |
| **Other** | 8 | Notifications, authorisation, UI choices |

### Systems ready to use

- **Daily streak bonus** with escalating rewards
- **Random boxes** with cards
- **Clans** with roles, invites and ranking
- **PvP rating**
- **Statistics** per class and per mode
- **Friends** with online status
- **Anticheat** with punish mode
- **Contracts** and achievements

### Game modes

| Mode | Description |
|---|---|
| `PvE_Private` | Co-op, closed room |
| `PvE_Autostart` | Co-op, automatic start |
| `PvP_Public` | Competitive, open room |
| `PvP_Autostart` | Competitive, automatic start |
| `PvP_ClanWar` | Clan war |
| `PvP_Rating` | Ranked match |

### Classes

Rifleman, Medic, Engineer, Recon (sniper) and Heavy.

### Currencies

| Currency | In game |
|---|---|
| `game` | WF$, earned by playing |
| `cry` | Kredits, the paid currency |
| `crown` | Crowns |

<br>

<h2><img src="https://api.iconify.design/solar/settings-bold.svg?color=%233B82F6&width=26" align="top" /> &nbsp;Configuration</h2>

Five files under `Config/`. All require a server restart.

### `settings.json` — the main one

| Field | Purpose |
|---|---|
| `host` | Address it listens on. `127.0.0.1` accepts this machine only |
| `port` | XMPP port. Defaults to `5222` |
| `dedicatedHosts` | Addresses allowed to register dedicated servers |
| `gameVersion` | Client version accepted. Must match the game |
| `certSecret` | TLS certificate password |
| `xmpp_debug` | Records traffic. Useful for diagnosis, heavy day to day |
| `xmpp_debug_console` | Mirrors that traffic to the console |
| `use_online_protect` | Online player count protection |
| `sponsors` | Vendor progression (see [VENDORS.md](VENDORS.md)) |

### `sql.json` — database

Address, user, password, database name and port.

### `room.json` — when a match starts

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

How many players must be ready, per room type. On a small server for friends, `1` everywhere avoids waiting.

### `masterservers.json` — channels

Four channels ship configured: `pve_001`, `pvp_newbie_001`, `pvp_skilled_001` and `pvp_pro_001`. `min_rank` and `max_rank` decide who sees each one.

### `defaultItems.json` — the starter kit

72 items every player receives on account creation:

```json
{ "name": "pt05_shop", "type": "Pistol", "classes": 29 }
```

`classes` is a bit sum: Rifleman 1, Medic 2, Engineer 4, Sniper 8, Heavy 16. The `29` is 1+4+8+16, every class except Medic. Use `31` for all of them.

<br>

<h2><img src="https://api.iconify.design/solar/database-bold.svg?color=%238B5CF6&width=26" align="top" /> &nbsp;What gets stored</h2>

26 tables. The ones you will touch most:

| Table | Holds |
|---|---|
| `emu_users` | Accounts: login, password, permission |
| `emu_profiles` | Profiles: nickname, rank, currency |
| `emu_items` | Each player's inventory |
| `emu_clans` / `emu_clan_members` | Clans and members |
| `emu_friends` | Friend lists |
| `emu_achievements` | Achievements earned |
| `emu_stats` | Stats per class and mode |
| `emu_sponsors` | Vendor points |
| `emu_bans` / `emu_mutes` | Punishments |
| `emu_login_bonus` | Daily streak |
| `emu_pvp_rating` | Competitive rating |

<br>

<h2><img src="https://api.iconify.design/solar/folder-with-files-bold.svg?color=%238B5CF6&width=26" align="top" /> &nbsp;Project structure</h2>

### Repository root

| File or folder | Purpose |
|---|---|
| `README.md` | Front door: what the project is and how to start |
| `MANUAL.md` | This document |
| `VENDORS.md` | The vendor system change |
| `LICENSE` | MIT license of the original project |
| `.gitignore` | What Git ignores (build output, temporary files) |
| `docs/languages/` | Portuguese and Russian translations |
| `EmuWarface/` | All the source code |

### `EmuWarface/` — main files

| File | Purpose |
|---|---|
| `Program.cs` | Starting point. Boots database, commands, gamedata, shop and server, in that order |
| `Server.cs` | Accepts player connections |
| `Config.cs` | Reads the files in `Config/` and turns them into objects |
| `Log.cs` | Logging to screen and file |
| `Utils.cs` | General-purpose helpers |
| `emuwarface.sql` | Database schema: the 26 tables and the test accounts |
| `EmuWarface.csproj` | Project definition and dependencies |

### `Core/` — the engine

Thirteen files holding up everything else. No game rules here: connection, database and routing.

| File | Lines | Purpose |
|---|---|---|
| `Client.cs` | **877** | Each connected player. Handles login, validates the account, keeps the session |
| `SQL.cs` | 162 | All conversation with MySQL |
| `DedicatedServer.cs` | 83 | Registers and tracks the servers that run matches |
| `MasterServer.cs` | 78 | Game channels (PvE, PvP) |
| `CommandHandler.cs` | 69 | Reads commands typed in the console |
| `QueryBinder.cs` | 29 | Discovers every protocol function on its own and registers it |
| `QueryData.cs` | 23 | The data of one request |
| `QueryAttribute.cs` | 21 | The `[Query]` marker tying a game command to a function |
| `QueryException.cs` | 21 | Request errors |
| `ServerException.cs` | 15 | Server errors |
| `ICmd.cs` | 11 | The contract every admin command follows |
| `Permission.cs` | 10 | The four access levels |
| `ConnectionState.cs` | 10 | Where in the connection a player currently is |

### `Game/` — the game rules

| File | Lines | Purpose |
|---|---|---|
| `Profile.cs` | **942** | The profile: rank, currency, class, appearance, vendors |
| `StatsManager.cs` | 486 | Statistics per class, mode and weapon |
| `GameData.cs` | 264 | Loads the game configuration files |
| `PlayerStat.cs` | 201 | A single statistic |
| `QueryCache.cs` | 191 | Keeps prepared answers, to avoid recomputing |
| `GameRestrictionSystem.cs` | 174 | Equipment restrictions per mode |
| `Mission.cs` | 148 | Missions and their parameters |
| `Achievement.cs` | 146 | Achievements |
| `PvpRatingState.cs` | 91 | Competitive rating |
| `Friend.cs` | 77 | Friend list and online status |
| `Invitation.cs` | 71 | Invitations between players |
| `Quickplay.cs` | 62 | Quick match search |
| `RoomPlayerInfo.cs` | 47 | A player's data inside a room |
| `ProfileBan.cs` | 43 | Per-profile bans |
| `Chat.cs` | 34 | Messages |
| `GameDataConfig.cs` | 27 | Where each gamedata file lives |

### `Game/` — subfolders

| Folder | Files | Purpose |
|---|---|---|
| `GameRooms/` | 12 | Rooms: creation, teams, voting, autostart, session. `GameRoom.cs` alone is 936 lines |
| `Enums/` | 16 | Fixed lists: classes, modes, currencies, teams, errors |
| `Notifications/` | 11 | Player notices: item received, achievement, invite, ban |
| `Shops/` | 4 | Store and random boxes. `Shop.cs` is 384 lines |
| `Items/` | 3 | Inventory items. `Item.cs` is 557 lines |
| `Clans/` | 2 | Clans, roles and ranking. `Clan.cs` is 347 lines |
| `GameRoomVotes/` | 2 | In-match voting |

### `Xmpp/` — the protocol

| File | Lines | Purpose |
|---|---|---|
| `Query/` | **91 files** | One file per subject, serving 98 game commands |
| `StreamParser2.cs` | 225 | Reads the XML stream arriving over the network |
| `Iq.cs` | 144 | Question-and-answer messages |
| `Jid.cs` | 117 | Each participant's address |
| `Stanza.cs` | 113 | The basic message unit |
| `StreamParser.cs` | 100 | The earlier reader, kept in the project |
| `Xml.cs` | 97 | Builds the responses |
| `IqType.cs` | 14 | Message types: Get, Set, Result, Error |

### `Config/` — configuration

| File | Purpose |
|---|---|
| `settings.json` | Address, port, game version, debugging, vendors |
| `sql.json` | Database connection. **Holds the password** |
| `room.json` | How many players start a match |
| `masterservers.json` | The four channels and their rank bands |
| `defaultItems.json` | The 72 starter-kit items |
| `cert.pfx` | TLS certificate for the connection |

### `Commands/` — administration

Eleven files, one per command. All follow the `ICmd` interface, which makes adding more straightforward: write the class and `CommandHandler` finds it by itself at startup.

<br>

<h2><img src="https://api.iconify.design/solar/danger-triangle-bold.svg?color=%23F97316&width=26" align="top" /> &nbsp;What does not work</h2>

An honest inventory, taken by reading the code. There are **119 pending markers** left by the authors.

| Limitation | Status |
|---|---|
| **`GameData` is not in the source tree** | Releases only. Without it the server will not start |
| **No in-game sign-up** | Accounts go in through SQL |
| **RCON** | The `rconPort` field exists in the code, but remote administration was never implemented |
| **Rank-up reward** | Marked pending in `Profile.cs` |
| **Rating ban** | Marked pending |

> [!NOTE]
> The 119 pending markers do not mean the server is unstable. Most are fine-tuning and author's notes. The essentials, from login to match, work.

<br>

<h2><img src="https://api.iconify.design/solar/shield-warning-bold.svg?color=%23F97316&width=26" align="top" /> &nbsp;Security</h2>

> [!CAUTION]
> **Plain-text passwords.** The `emu_users` table stores passwords with no protection whatsoever. Anyone with database access reads them all. Never reuse a password you use elsewhere.

> [!WARNING]
> **Test accounts are public.** `user1` through `user4`, password `12345`, all Admin. Change them before opening up to other people.

> [!CAUTION]
> **`sql.json` holds the database password.** Never publish that file filled in, and never ship it with the project.

When exposing to the internet, use a firewall and open only the port you need. A home server belongs on `127.0.0.1` or behind a VPN.

<br>

<h2><img src="https://api.iconify.design/solar/bug-bold.svg?color=%23F97316&width=26" align="top" /> &nbsp;When something goes wrong</h2>

<details>
<summary><b>The server closes right after starting</b></summary>

<br>

Almost always one of these three:

1. **The `GameData` folder is missing.** Check that it sits next to the executable.
2. **MySQL is down or the password is wrong.** Test the `sql.json` connection by hand.
3. **Empty database.** Import `emuwarface.sql`.

</details>

<details>
<summary><b>The game will not connect</b></summary>

<br>

- Does the client's `online.cfg` point at the right address?
- Does `gameVersion` in `settings.json` match the game build?
- Is port 5222 open in the firewall?
- Turn on `xmpp_debug` and watch what reaches the console.

</details>

<details>
<summary><b>I reach the lobby, but matches never start</b></summary>

<br>

EmuWarface does not host matches: `DedicatedServer.exe`, from the client, does. Check that it is running and that its address is listed in `dedicatedHosts`.

</details>

<details>
<summary><b>I forgot an account password</b></summary>

<br>

They sit in plain text in the database:

```sql
SELECT login, password FROM emu_users;
UPDATE emu_users SET password = 'new' WHERE login = 'user1';
```

</details>

<br>

<h2><img src="https://api.iconify.design/solar/heart-bold.svg?color=%2322C55E&width=26" align="top" /> &nbsp;Credits</h2>

EmuWarface is the work of **[n1kodim](https://github.com/n1kodim)**, with a contribution from **[myrka32](https://github.com/myrka32)**, under the MIT license. This manual documents their work.

<br>

<div align="center">
<sub>

Based on [EmuWarface](https://github.com/n1kodim/EmuWarface) by n1kodim &nbsp;·&nbsp; MIT License &nbsp;·&nbsp; DEV20 build 1.22400.5519.45100

</sub>
</div>
