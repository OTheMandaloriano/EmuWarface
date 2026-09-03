<div align="center">

<img src="https://api.iconify.design/solar/global-linear.svg?color=%236E7681&width=18" align="top" /> &nbsp;<sub><b>Choose your language</b> &nbsp;·&nbsp; <b>Escolha seu idioma</b> &nbsp;·&nbsp; <b>Выберите язык</b></sub>

<a href="../../VENDORS.md"><img src="https://api.iconify.design/flag/us-4x3.svg?width=21" align="top" alt="English" /> <img src="https://img.shields.io/badge/English-6E7681?style=flat-square" alt="English" /></a>
&nbsp;&nbsp;
<a href="../../docs/languages/VENDORS.pt-BR.md"><img src="https://api.iconify.design/flag/br-4x3.svg?width=21" align="top" alt="Português" /> <img src="https://img.shields.io/badge/Portugu%C3%AAs-6E7681?style=flat-square" alt="Português" /></a>
&nbsp;&nbsp;
<a href="../../docs/languages/VENDORS.ru.md"><img src="https://api.iconify.design/flag/ru-4x3.svg?width=21" align="top" alt="Русский" /> <img src="https://img.shields.io/badge/%D0%A0%D1%83%D1%81%D1%81%D0%BA%D0%B8%D0%B9-1F6FEB?style=flat-square" alt="Русский" /></a>

</div>


<br>

Документ описывает изменение, внесённое поверх [EmuWarface](https://github.com/n1kodim/EmuWarface) — эмулятора серверной части Warface на C# авторства **n1kodim** для сборки **DEV20 1.22400.5519.45100**.

<br>

<h2><img src="https://api.iconify.design/solar/question-circle-bold.svg?color=%233B82F6&width=26" align="top" /> &nbsp;Что такое поставщики</h2>

Вкладка **ПОСТАВЩИКИ** (VENDORS) — это место, где игрок открывает оружие, снаряжение и модули. У каждого поставщика свой список предметов, и открываются они по одному, по мере накопления **очков поставщика** в матчах.

Поставщиков трое, с номерами от `0` до `2`.

<br>

<h2><img src="https://api.iconify.design/solar/refresh-circle-bold.svg?color=%233B82F6&width=26" align="top" /> &nbsp;Что изменилось</h2>

Оригинальный эмулятор отдавал **ноль очков, зашитый в код**, всем игрокам одинаково, не обращаясь к базе данных. Таблица `emu_sponsors` уже существовала и заполнялась при создании профиля, но её никто не читал обратно.

| | Было | Стало |
|---|---|---|
| Источник очков | Число в исходном коде | Таблица `emu_sponsors`, по профилю |
| Настраивается | Нет | Да, через `settings.json` |
| Личный прогресс | Отсутствовал | У каждого профиля свой |
| Предел очков в базе | 255 (`tinyint`) | 4 294 967 295 (`int unsigned`) |

<br>

<h2><img src="https://api.iconify.design/solar/settings-bold.svg?color=%233B82F6&width=26" align="top" /> &nbsp;Настройка поставщиков</h2>

Новый блок в **`EmuWarface/Config/settings.json`**, начиная со строки 13:

```json
"sponsors": {
    "unlock_all": false,
    "unlock_all_points": 999999,
    "starting_points": 0
}
```

| Поле | Назначение | По умолчанию |
|---|---|---|
| `unlock_all` | `true` открывает всё и всем. `false` включает личный прогресс. | `false` |
| `unlock_all_points` | Сколько очков сообщать, пока `unlock_all` включён. | `999999` |
| `starting_points` | С каким количеством очков создаётся новый профиль. | `0` |

> [!NOTE]
> После правки `settings.json` эмулятор нужно перезапустить: файл читается один раз, при старте.

### Как вернуть прежнее поведение

Поменяйте одно слово:

```json
"unlock_all": true
```

Перезапустите эмулятор. У всех игроков снова открыто всё, как было до изменения.

> [!IMPORTANT]
> Возврат к `true` **не стирает** накопленный прогресс. Очки остаются в таблице `emu_sponsors`. Если позже вернуть `false`, каждый игрок найдёт ровно то, что у него было.

<details>
<summary><b>А если я хочу откатить сам код, а не только настройку?</b></summary>

<br>

Откат кода означает удаление изменений, перечисленных ниже. Для возврата прежнего поведения это не нужно: `unlock_all: true` даёт тот же результат без правок и пересборки.

Если исходный код всё же нужен, возьмите его заново:

```bash
git clone https://github.com/n1kodim/EmuWarface
```

</details>

<br>

<h2><img src="https://api.iconify.design/solar/tuning-square-bold.svg?color=%233B82F6&width=26" align="top" /> &nbsp;Что можно настроить</h2>

Файлы лежат в `EmuWarface/Config/`. Все требуют перезапуска эмулятора.

### Поставщики &nbsp;·&nbsp; `settings.json`

**Дать новичкам фору:**

```json
"starting_points": 500
```

Действует только на профили, созданные после изменения. Существующие игроки не затрагиваются.

**Изменить очки действующих игроков** прямо в базе:

```sql
-- один игрок, один поставщик
UPDATE emu_sponsors SET sponsor_points = 1500
WHERE profile_id = 7 AND sponsor_id = 0;

-- один игрок, все три поставщика
UPDATE emu_sponsors SET sponsor_points = 1500 WHERE profile_id = 7;

-- сбросить всех в ноль
UPDATE emu_sponsors SET sponsor_points = 0;
```

**Посмотреть текущий прогресс:**

```sql
SELECT p.nickname, s.sponsor_id, s.sponsor_points
FROM emu_sponsors s
JOIN emu_profiles p ON p.profile_id = s.profile_id
ORDER BY p.nickname, s.sponsor_id;
```

Игрок увидит изменения при следующем входе.

### Сколько игроков нужно для старта матча &nbsp;·&nbsp; `room.json`

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

Сколько игроков должны быть готовы, чтобы матч начался, для каждого типа комнаты. На маленьком сервере значение `1` везде избавляет от ожидания. На людном сервере большие значения дают более ровные матчи.

### Игровые каналы &nbsp;·&nbsp; `masterservers.json`

В комплекте четыре канала: `pve_001`, `pvp_newbie_001`, `pvp_skilled_001` и `pvp_pro_001`.

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

`min_rank` и `max_rank` задают диапазон званий, которым виден канал. Все каналы открыты с 1 по 90, то есть все играют вместе. Чтобы развести новичков и ветеранов, сузьте диапазоны: `pvp_newbie` с 1 по 10, `pvp_skilled` с 11 по 25, `pvp_pro` с 26 по 90.

> [!WARNING]
> Диапазоны, которые не стыкуются, оставляют дыры. Если `newbie` заканчивается на 10, а `skilled` начинается с 15, игрокам с 11 по 14 не достанется ни одного PvP-канала.

### Предметы, которые получает каждый новый игрок &nbsp;·&nbsp; `defaultItems.json`

72 стартовых предмета:

```json
{ "name": "pt05_shop", "type": "Pistol", "classes": 29 }
```

Поле `classes` — это сумма битов, задающая, каким классам доступен предмет: Штурмовик 1, Медик 2, Инженер 4, Снайпер 8, Тяжёлый 16. В примере `29` — это 1+4+8+16, то есть все классы кроме Медика. Для всех классов ставьте `31`.

### Сеть и отладка &nbsp;·&nbsp; `settings.json`

| Поле | Назначение |
|---|---|
| `host` | Адрес, который слушает эмулятор. `127.0.0.1` принимает только эту машину. |
| `port` | Порт XMPP. По умолчанию `5222`. |
| `dedicatedHosts` | Адреса, которым разрешено регистрировать выделенные серверы. |
| `gameVersion` | Принимаемая версия клиента. Должна совпадать с версией игры. |
| `xmpp_debug` | Записывает трафик XMPP. Полезно для диагностики, тяжело в повседневной работе. |
| `xmpp_debug_console` | Дублирует этот трафик в консоль. |
| `use_online_protect` | Включает защиту счётчика игроков онлайн. |

<br>

<h2><img src="https://api.iconify.design/solar/star-bold.svg?color=%2322C55E&width=26" align="top" /> &nbsp;Рекомендуемые конфигурации</h2>

Три рабочих сочетания, в зависимости от задачи.

<details open>
<summary><b>Домашний сервер, вы и несколько друзей</b></summary>

<br>

Цель — зайти и играть, без ожидания и без гринда.

```json
"sponsors": {
    "unlock_all": true,
    "unlock_all_points": 999999,
    "starting_points": 0
}
```

И в `room.json` везде `1`, чтобы матч начинался сразу:

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

**Почему здесь `unlock_all: true`:** это полностью убирает гринд, что подходит для вечера с друзьями. Если хочется прогресса, `false` тоже работает: очки начисляются по итогам каждого матча.

</details>

<details>
<summary><b>Сервер с прогрессом, как в оригинале</b></summary>

<br>

Каждый игрок начинает с нуля и развивается.

```json
"sponsors": {
    "unlock_all": false,
    "unlock_all_points": 999999,
    "starting_points": 0
}
```

Каналы, разведённые по званиям, в `masterservers.json`, чтобы новички не попадали к ветеранам:

| Канал | `min_rank` | `max_rank` |
|---|---|---|
| `pvp_newbie_001` | 1 | 10 |
| `pvp_skilled_001` | 11 | 25 |
| `pvp_pro_001` | 26 | 90 |

> [!NOTE]
> Очки начисляются по итогам каждого матча, по множителю `SponsorPointsMultiplier` из gamedata. Команды SQL выше остаются полезными, чтобы поправить конкретного игрока.

</details>

<details>
<summary><b>Небольшой публичный сервер</b></summary>

<br>

Середина: игрок начинает не с абсолютного нуля, но ему есть чего добиваться.

```json
"sponsors": {
    "unlock_all": false,
    "unlock_all_points": 999999,
    "starting_points": 300
}
```

И отключите отладку, которая нагружает сервер при живых игроках:

```json
"xmpp_debug": false,
"xmpp_debug_console": false
```

**Почему `starting_points: 300`:** новичок сразу видит несколько доступных вариантов и не бросает игру на первом экране, а большая часть каталога остаётся целью.

</details>

<br>

<h2><img src="https://api.iconify.design/solar/shield-warning-bold.svg?color=%23F97316&width=26" align="top" /> &nbsp;Меры предосторожности</h2>

> [!WARNING]
> В файле `Config/sql.json` хранится **пароль от базы данных**. Никогда не публикуйте его заполненным в открытом репозитории и не передавайте вместе с проектом.

> [!CAUTION]
> Порт эмулятора не имеет сильной аутентификации. При выходе в интернет используйте файрвол и открывайте только необходимое. Домашнему серверу место на `127.0.0.1` или за VPN.

<br>

<h2><img src="https://api.iconify.design/solar/file-text-bold.svg?color=%233B82F6&width=26" align="top" /> &nbsp;Изменённые файлы</h2>

Все внутри `EmuWarface/`:

| Файл | Строка | Что сделано |
|---|---|---|
| `Config/settings.json` | 13 — 17 | Добавлен блок `sponsors` |
| `Config.cs` | 113, 123 | Свойство `Sponsors` и класс `SponsorsConfig` |
| `Game/Profile.cs` | 453 | Метод `SponsorsSerialize()`, выбирающий между «открыть всё» и чтением из базы |
| `Game/Profile.cs` | 530 | `AddSponsorPoints()`, начисляет очки, заработанные в матче |
| `Game/Profile.cs` | 618 — 621 | Три `INSERT` для нового профиля используют `starting_points` |
| `Xmpp/Query/JoinChannel.cs` | 53 | Пять зашитых строк заменены одним вызовом метода |
| `Xmpp/Query/CreateProfile.cs` | 70 | Та же замена |
| `Xmpp/Query/SetRewardsInfo.cs` | 324 | Начисляет очки в профиль по итогам матча |
| `Xmpp/Query/SetRewardsInfo.cs` | 344 | Отправляет клиенту вычисленное значение вместо жёсткого нуля |
| `emuwarface.sql` | 289 | `sponsor_points` расширен с `tinyint` до `int unsigned` |

<details>
<summary><b>Код, который был здесь раньше</b></summary>

<br>

В `JoinChannel.cs` и `CreateProfile.cs` заменённый фрагмент выглядел так:

```csharp
//TODO sponsors
XmlElement sponsor_info = Xml.Element("sponsor_info");
sponsor_info.Child(Xml.Element("sponsor").Attr("sponsor_id", "0").Attr("sponsor_points", "0").Attr("next_unlock_item", ""));
sponsor_info.Child(Xml.Element("sponsor").Attr("sponsor_id", "1").Attr("sponsor_points", "0").Attr("next_unlock_item", ""));
sponsor_info.Child(Xml.Element("sponsor").Attr("sponsor_id", "2").Attr("sponsor_points", "0").Attr("next_unlock_item", ""));
character.Child(sponsor_info);
```

Пометка `//TODO sponsors`, оставленная самим автором, показывает, что это была заглушка в ожидании реализации.

</details>

<br>

<h2><img src="https://api.iconify.design/solar/danger-triangle-bold.svg?color=%23F97316&width=26" align="top" /> &nbsp;Известные ограничения</h2>

| Ограничение | Причина |
|---|---|
| `next_unlock_item` остаётся пустым | Зависит от таблицы предметов каждого поставщика, которую эмулятор не загружает. |
| У профилей, созданных до изменения, может не быть строк в `emu_sponsors` | Обрабатывается автоматически: метод создаёт недостающие строки при первом входе, используя `starting_points`. |

<br>

<h2><img src="https://api.iconify.design/solar/checklist-minimalistic-bold.svg?color=%2322C55E&width=26" align="top" /> &nbsp;Проверка</h2>

Проект собран после изменений:

```
26 предупреждений
 0 ошибок
```

Все 26 предупреждений существовали в проекте и раньше, к этим изменениям отношения не имеют.

<br>

<h2><img src="https://api.iconify.design/solar/heart-bold.svg?color=%2322C55E&width=26" align="top" /> &nbsp;Благодарности</h2>

EmuWarface создан **[n1kodim](https://github.com/n1kodim)** при участии **[myrka32](https://github.com/myrka32)** и опубликован **[CryHub](https://github.com/WFCRYHUB)** под лицензией MIT. Вся заслуга за эмулятор принадлежит им.

Это копия проекта с одним точечным изменением в системе поставщиков, описанным выше. Исходная лицензия сохранена в [LICENSE](../../LICENSE).

<br>

<div align="center">
<sub>

Изменение поверх [EmuWarface](https://github.com/n1kodim/EmuWarface) &nbsp;·&nbsp; Лицензия MIT &nbsp;·&nbsp; Сборка DEV20 1.22400.5519.45100

</sub>
</div>
