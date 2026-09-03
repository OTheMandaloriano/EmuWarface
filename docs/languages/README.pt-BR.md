<div align="center">

<img src="https://api.iconify.design/solar/global-linear.svg?color=%236E7681&width=18" align="top" /> &nbsp;<sub><b>Choose your language</b> &nbsp;·&nbsp; <b>Escolha seu idioma</b> &nbsp;·&nbsp; <b>Выберите язык</b></sub>

<a href="../../README.md"><img src="https://api.iconify.design/flag/us-4x3.svg?width=21" align="top" alt="English" /> <img src="https://img.shields.io/badge/English-6E7681?style=flat-square" alt="English" /></a>
&nbsp;&nbsp;
<a href="../../docs/languages/README.pt-BR.md"><img src="https://api.iconify.design/flag/br-4x3.svg?width=21" align="top" alt="Português" /> <img src="https://img.shields.io/badge/Portugu%C3%AAs-1F6FEB?style=flat-square" alt="Português" /></a>
&nbsp;&nbsp;
<a href="../../docs/languages/README.ru.md"><img src="https://api.iconify.design/flag/ru-4x3.svg?width=21" align="top" alt="Русский" /> <img src="https://img.shields.io/badge/%D0%A0%D1%83%D1%81%D1%81%D0%BA%D0%B8%D0%B9-6E7681?style=flat-square" alt="Русский" /></a>

</div>


<br>

Emulador de servidor (backend) do Warface escrito em C#, que atende a build **DEV20 1.22400.5519.45100**.

Este repositório é uma cópia do [EmuWarface original](https://github.com/n1kodim/EmuWarface), de **n1kodim**, com o histórico de commits dele preservado, mais uma alteração: a **progressão de fornecedores** deixou de ser fixa no código e passou a ser configurável.

<br>

<h2><img src="https://api.iconify.design/solar/checklist-minimalistic-bold.svg?color=%233B82F6&width=26" align="top" /> &nbsp;Requisitos</h2>

<img src="https://api.iconify.design/devicon/dotnetcore.svg?width=18" align="top" /> &nbsp;[.NET 6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)

<img src="https://api.iconify.design/devicon/mysql-wordmark.svg?width=18" align="top" /> &nbsp;[MySQL](https://dev.mysql.com/downloads/installer/) ou MariaDB 10.9.2

O banco é criado a partir de `EmuWarface/emuwarface.sql`.

Para começar com os dados de jogo já prontos, use a [release do projeto original](https://github.com/n1kodim/EmuWarface/releases/latest), que acompanha o gamedata.

<br>

<h2><img src="https://api.iconify.design/solar/star-bold.svg?color=%2322C55E&width=26" align="top" /> &nbsp;O que muda neste repositório</h2>

Na aba **FORNECEDORES** (VENDORS), o jogador desbloqueia armas, equipamentos e modificações acumulando pontos. O emulador original respondia com **pontos fixos escritos no código**, iguais para todo mundo, ignorando o banco de dados. A tabela `emu_sponsors` já existia e já era preenchida na criação do perfil, mas nada a lia de volta: o próprio autor havia deixado um `//TODO sponsors` no lugar.

| | Original | Aqui |
|---|---|---|
| Origem dos pontos | Número escrito no código | Tabela `emu_sponsors`, por perfil |
| Configurável | Não | Sim, pelo `settings.json` |
| Progresso individual | Não existia | Cada perfil tem o seu |
| Limite de pontos no banco | 255 (`tinyint`) | 4.294.967.295 (`int unsigned`) |

O **[Manual](MANUAL.pt-BR.md)** cobre instalação, contas, os 11 comandos de administração, os 98 comandos de protocolo e o que ainda não funciona.

A documentação da alteração está em **[VENDORS.pt-BR.md](VENDORS.pt-BR.md)**: o que foi alterado em cada arquivo e linha, como voltar ao comportamento anterior, tudo que dá para personalizar e três configurações recomendadas.

<br>

<h2><img src="https://api.iconify.design/solar/settings-bold.svg?color=%233B82F6&width=26" align="top" /> &nbsp;Configuração rápida</h2>

Em `EmuWarface/Config/settings.json`:

```json
"sponsors": {
    "unlock_all": false,
    "unlock_all_points": 999999,
    "starting_points": 0
}
```

| Campo | O que faz | Padrão |
|---|---|---|
| `unlock_all` | `true` libera tudo para todos. `false` usa a progressão individual. | `false` |
| `unlock_all_points` | Quantos pontos anunciar quando `unlock_all` está ligado. | `999999` |
| `starting_points` | Com quantos pontos um perfil novo nasce. | `0` |

> [!NOTE]
> Para voltar ao comportamento do projeto original, use `"unlock_all": true` e reinicie o emulador. Nenhum progresso é apagado: os pontos continuam guardados no banco.

<br>

<h2><img src="https://api.iconify.design/solar/danger-triangle-bold.svg?color=%23F97316&width=26" align="top" /> &nbsp;Limitações conhecidas</h2>

| Limitação | Motivo |
|---|---|
| `next_unlock_item` fica vazio | Depende da tabela de itens de cada fornecedor, que o emulador ainda não carrega. |

> [!NOTE]
> Com `unlock_all: false`, o jogador ganha pontos de fornecedor jogando: o valor sai do `SponsorPointsMultiplier` do gamedata e é creditado ao fim de cada partida.

<br>

<h2><img src="https://api.iconify.design/solar/shield-warning-bold.svg?color=%23F97316&width=26" align="top" /> &nbsp;Segurança</h2>

> [!CAUTION]
> `EmuWarface/Config/sql.json` guarda a **senha do banco**. Nunca publique esse arquivo preenchido, nem o envie junto com o projeto.

A porta do emulador não tem autenticação forte. Ao expor à internet, use firewall e libere só o necessário. Um servidor caseiro deve ficar em `127.0.0.1` ou atrás de VPN.

<br>

<h2><img src="https://api.iconify.design/solar/heart-bold.svg?color=%2322C55E&width=26" align="top" /> &nbsp;Créditos</h2>

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

O EmuWarface é obra de **[n1kodim](https://github.com/n1kodim)**, com contribuição de **[myrka32](https://github.com/myrka32)**, e distribuído pela **[CryHub](https://github.com/WFCRYHUB)** sob licença MIT. Todo o mérito do emulador é deles.

Os 15 commits originais estão preservados no histórico deste repositório, com autoria intacta, incluindo o [README que o autor escreveu](https://github.com/OTheMandaloriano/EmuWarface/blob/a9a5638/README.md). A licença original está em [LICENSE](../../LICENSE).

<br>

<div align="center">
<sub>

Baseado no [EmuWarface](https://github.com/n1kodim/EmuWarface) de n1kodim &nbsp;·&nbsp; Licença MIT &nbsp;·&nbsp; Build DEV20 1.22400.5519.45100

</sub>
</div>
