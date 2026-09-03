<div align="center">

<img src="https://api.iconify.design/solar/global-linear.svg?color=%236E7681&width=18" align="top" /> &nbsp;<sub><b>Choose your language</b> &nbsp;·&nbsp; <b>Escolha seu idioma</b> &nbsp;·&nbsp; <b>Выберите язык</b></sub>

<a href="../../README.md"><img src="https://api.iconify.design/flag/us-4x3.svg?width=21" align="top" alt="English" /> <img src="https://img.shields.io/badge/English-6E7681?style=flat-square" alt="English" /></a>
&nbsp;&nbsp;
<a href="../../docs/languages/README.pt-BR.md"><img src="https://api.iconify.design/flag/br-4x3.svg?width=21" align="top" alt="Português" /> <img src="https://img.shields.io/badge/Portugu%C3%AAs-6E7681?style=flat-square" alt="Português" /></a>
&nbsp;&nbsp;
<a href="../../docs/languages/README.ru.md"><img src="https://api.iconify.design/flag/ru-4x3.svg?width=21" align="top" alt="Русский" /> <img src="https://img.shields.io/badge/%D0%A0%D1%83%D1%81%D1%81%D0%BA%D0%B8%D0%B9-1F6FEB?style=flat-square" alt="Русский" /></a>

</div>


<br>

Эмулятор серверной части Warface на C# для сборки **DEV20 1.22400.5519.45100**.

Этот репозиторий — копия [оригинального EmuWarface](https://github.com/n1kodim/EmuWarface) авторства **n1kodim** с сохранённой историей коммитов и одним изменением: **прогресс у поставщиков** больше не зашит в код, его можно настраивать.

<br>

<h2><img src="https://api.iconify.design/solar/checklist-minimalistic-bold.svg?color=%233B82F6&width=26" align="top" /> &nbsp;Требования</h2>

<img src="https://api.iconify.design/devicon/dotnetcore.svg?width=18" align="top" /> &nbsp;[.NET 6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)

<img src="https://api.iconify.design/devicon/mysql-wordmark.svg?width=18" align="top" /> &nbsp;[MySQL](https://dev.mysql.com/downloads/installer/) или MariaDB 10.9.2

База данных создаётся из `EmuWarface/emuwarface.sql`.

Чтобы сразу получить готовые игровые данные, возьмите [релиз оригинального проекта](https://github.com/n1kodim/EmuWarface/releases/latest) — он идёт вместе с gamedata.

<br>

<h2><img src="https://api.iconify.design/solar/star-bold.svg?color=%2322C55E&width=26" align="top" /> &nbsp;Что здесь изменено</h2>

На вкладке **ПОСТАВЩИКИ** (VENDORS) игрок открывает оружие, снаряжение и модули, накапливая очки. Оригинальный эмулятор отдавал **фиксированные очки, зашитые в код**, одинаковые для всех, и не обращался к базе. Таблица `emu_sponsors` уже существовала и заполнялась при создании профиля, но её никто не читал: на этом месте у автора стояла пометка `//TODO sponsors`.

| | Оригинал | Здесь |
|---|---|---|
| Источник очков | Число в исходном коде | Таблица `emu_sponsors`, по профилю |
| Настраивается | Нет | Да, через `settings.json` |
| Личный прогресс | Отсутствовал | У каждого профиля свой |
| Предел очков в базе | 255 (`tinyint`) | 4 294 967 295 (`int unsigned`) |

**[Руководство](MANUAL.ru.md)** описывает установку, аккаунты, 11 команд администратора, 98 команд протокола и то, что пока не работает.

Документация изменения — в **[VENDORS.ru.md](VENDORS.ru.md)**: какой файл и строка изменены, как вернуть прежнее поведение, что можно настроить и три рекомендуемые конфигурации.

<br>

<h2><img src="https://api.iconify.design/solar/settings-bold.svg?color=%233B82F6&width=26" align="top" /> &nbsp;Быстрая настройка</h2>

В файле `EmuWarface/Config/settings.json`:

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
> Чтобы вернуть поведение оригинала, поставьте `"unlock_all": true` и перезапустите эмулятор. Ничего не теряется: очки остаются в базе.

<br>

<h2><img src="https://api.iconify.design/solar/danger-triangle-bold.svg?color=%23F97316&width=26" align="top" /> &nbsp;Известные ограничения</h2>

| Ограничение | Причина |
|---|---|
| `next_unlock_item` остаётся пустым | Зависит от таблицы предметов каждого поставщика, которую эмулятор пока не загружает. |

> [!NOTE]
> При `unlock_all: false` игрок зарабатывает очки поставщиков игрой: величина берётся из `SponsorPointsMultiplier` в gamedata и начисляется по итогам каждого матча.

<br>

<h2><img src="https://api.iconify.design/solar/shield-warning-bold.svg?color=%23F97316&width=26" align="top" /> &nbsp;Безопасность</h2>

> [!CAUTION]
> В `EmuWarface/Config/sql.json` хранится **пароль от базы данных**. Никогда не публикуйте этот файл заполненным и не передавайте его вместе с проектом.

Порт эмулятора не имеет сильной аутентификации. При выходе в интернет используйте файрвол и открывайте только необходимое. Домашнему серверу место на `127.0.0.1` или за VPN.

<br>

<h2><img src="https://api.iconify.design/solar/heart-bold.svg?color=%2322C55E&width=26" align="top" /> &nbsp;Благодарности</h2>

<div align="center">

<table>
<tr>
<td align="center" width="150">
<a href="https://github.com/n1kodim">
<img src="https://github.com/n1kodim.png?size=100" width="80" alt="n1kodim" /><br />
<sub><b>n1kodim</b></sub>
</a><br />
<sub>14 коммитов</sub>
</td>
<td align="center" width="150">
<a href="https://github.com/myrka32">
<img src="https://github.com/myrka32.png?size=100" width="80" alt="myrka32" /><br />
<sub><b>myrka32</b></sub>
</a><br />
<sub>1 коммит</sub>
</td>
</tr>
</table>

</div>

EmuWarface создан **[n1kodim](https://github.com/n1kodim)** при участии **[myrka32](https://github.com/myrka32)** и опубликован **[CryHub](https://github.com/WFCRYHUB)** под лицензией MIT. Вся заслуга за эмулятор принадлежит им.

15 оригинальных коммитов сохранены в истории этого репозитория с исходным авторством, включая [README, написанный автором](https://github.com/OTheMandaloriano/EmuWarface/blob/a9a5638/README.md). Исходная лицензия — в [LICENSE](../../LICENSE).

<br>

<div align="center">
<sub>

На основе [EmuWarface](https://github.com/n1kodim/EmuWarface) от n1kodim &nbsp;·&nbsp; Лицензия MIT &nbsp;·&nbsp; Сборка DEV20 1.22400.5519.45100

</sub>
</div>
