<div align="center">

<img src="https://api.iconify.design/solar/global-linear.svg?color=%236E7681&width=18" align="top" /> &nbsp;<sub><b>Choose your language</b> &nbsp;·&nbsp; <b>Escolha seu idioma</b> &nbsp;·&nbsp; <b>Выберите язык</b></sub>

<a href="../../VENDORS.md"><img src="https://api.iconify.design/flag/us-4x3.svg?width=21" align="top" alt="English" /> <img src="https://img.shields.io/badge/English-6E7681?style=flat-square" alt="English" /></a>
&nbsp;&nbsp;
<a href="../../docs/languages/VENDORS.pt-BR.md"><img src="https://api.iconify.design/flag/br-4x3.svg?width=21" align="top" alt="Português" /> <img src="https://img.shields.io/badge/Portugu%C3%AAs-1F6FEB?style=flat-square" alt="Português" /></a>
&nbsp;&nbsp;
<a href="../../docs/languages/VENDORS.ru.md"><img src="https://api.iconify.design/flag/ru-4x3.svg?width=21" align="top" alt="Русский" /> <img src="https://img.shields.io/badge/%D0%A0%D1%83%D1%81%D1%81%D0%BA%D0%B8%D0%B9-6E7681?style=flat-square" alt="Русский" /></a>

</div>


<br>

Este documento acompanha uma alteração feita sobre o [EmuWarface](https://github.com/n1kodim/EmuWarface), o emulador de servidor do Warface escrito em C# por **n1kodim** que atende a build **DEV20 1.22400.5519.45100**.

<br>

<h2><img src="https://api.iconify.design/solar/question-circle-bold.svg?color=%233B82F6&width=26" align="top" /> &nbsp;O que são os Fornecedores</h2>

No jogo, a aba **FORNECEDORES** (VENDORS no original em inglês) é onde o jogador desbloqueia armas, equipamentos e modificações. Cada fornecedor tem uma lista de itens, e o jogador libera um de cada vez conforme acumula **pontos de fornecedor** jogando partidas.

São três fornecedores, numerados de `0` a `2`.

<br>

<h2><img src="https://api.iconify.design/solar/refresh-circle-bold.svg?color=%233B82F6&width=26" align="top" /> &nbsp;O que mudou</h2>

O emulador original respondia com **zero pontos fixos** para todo jogador, ignorando o banco de dados. A tabela `emu_sponsors` já existia e já era preenchida na criação do perfil, mas nada a lia de volta.

| | Antes | Depois |
|---|---|---|
| Origem dos pontos | Número escrito no código | Tabela `emu_sponsors`, por perfil |
| Configurável | Não | Sim, pelo `settings.json` |
| Progresso individual | Não existia | Cada perfil tem o seu |
| Limite de pontos no banco | 255 (`tinyint`) | 4.294.967.295 (`int unsigned`) |

<br>

<h2><img src="https://api.iconify.design/solar/settings-bold.svg?color=%233B82F6&width=26" align="top" /> &nbsp;A configuração dos Fornecedores</h2>

Bloco novo em **`EmuWarface/Config/settings.json`**, a partir da linha 13:

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
> Alterar o `settings.json` exige reiniciar o emulador. O arquivo é lido uma única vez, quando o servidor sobe.

### Voltar ao comportamento anterior

Troque uma palavra:

```json
"unlock_all": true
```

Reinicie o emulador. Todos voltam a ver tudo liberado, como antes da alteração.

> [!IMPORTANT]
> Voltar para `true` **não apaga** o progresso guardado. Os pontos continuam na tabela `emu_sponsors`. Se um dia você voltar para `false`, cada jogador reencontra exatamente o que tinha.

<details>
<summary><b>E se eu quiser desfazer a alteração no código, e não só na configuração?</b></summary>

<br>

Reverter o código significa apagar as alterações listadas mais abaixo. Não é necessário para voltar ao comportamento antigo: `unlock_all: true` já entrega o mesmo resultado, sem editar nem recompilar nada.

Se ainda assim quiser o código original, baixe-o de novo da origem:

```bash
git clone https://github.com/n1kodim/EmuWarface
```

</details>

<br>

<h2><img src="https://api.iconify.design/solar/tuning-square-bold.svg?color=%233B82F6&width=26" align="top" /> &nbsp;Tudo que dá para personalizar</h2>

Os arquivos ficam em `EmuWarface/Config/`. Todos exigem reinício do emulador.

### Fornecedores &nbsp;·&nbsp; `settings.json`

**Dar vantagem inicial a quem chega:**

```json
"starting_points": 500
```

Vale só para perfis criados depois da mudança. Quem já existe permanece como está.

**Ajustar quem já joga**, direto no banco:

```sql
-- um jogador, um fornecedor
UPDATE emu_sponsors SET sponsor_points = 1500
WHERE profile_id = 7 AND sponsor_id = 0;

-- um jogador, os três fornecedores
UPDATE emu_sponsors SET sponsor_points = 1500 WHERE profile_id = 7;

-- todo mundo recomeça do zero
UPDATE emu_sponsors SET sponsor_points = 0;
```

**Consultar o progresso atual:**

```sql
SELECT p.nickname, s.sponsor_id, s.sponsor_points
FROM emu_sponsors s
JOIN emu_profiles p ON p.profile_id = s.profile_id
ORDER BY p.nickname, s.sponsor_id;
```

O jogador vê a mudança no próximo login.

### Quantos jogadores para começar a partida &nbsp;·&nbsp; `room.json`

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

Quantos jogadores precisam estar prontos para a partida iniciar, por tipo de sala. Num servidor de poucos amigos, deixar tudo em `1` evita ficar esperando sala encher. Num servidor com movimento, valores maiores dão partidas mais equilibradas.

### Canais de jogo &nbsp;·&nbsp; `masterservers.json`

Vêm quatro canais configurados: `pve_001`, `pvp_newbie_001`, `pvp_skilled_001` e `pvp_pro_001`.

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

`min_rank` e `max_rank` definem a faixa de patente que enxerga o canal. Todos vêm abertos de 1 a 90, o que junta todo mundo no mesmo lugar. Para separar iniciantes de veteranos, estreite as faixas: `pvp_newbie` de 1 a 10, `pvp_skilled` de 11 a 25, `pvp_pro` de 26 a 90.

> [!WARNING]
> Faixas que não se encostam deixam buracos. Se `newbie` vai até 10 e `skilled` começa em 15, quem está entre 11 e 14 fica sem canal de PvP.

### Itens que todo jogador ganha ao criar a conta &nbsp;·&nbsp; `defaultItems.json`

São 72 itens de partida:

```json
{ "name": "pt05_shop", "type": "Pistol", "classes": 29 }
```

O campo `classes` é uma soma de bits que define quais classes usam o item: Fuzileiro 1, Médico 2, Engenheiro 4, Sniper 8, Pesado 16. O `29` do exemplo é 1+4+8+16, ou seja, todas menos Médico. Para todas as classes, use `31`.

### Rede e depuração &nbsp;·&nbsp; `settings.json`

| Campo | Para que serve |
|---|---|
| `host` | Endereço em que o emulador escuta. `127.0.0.1` aceita só esta máquina. |
| `port` | Porta do XMPP. Padrão `5222`. |
| `dedicatedHosts` | Endereços autorizados a registrar servidores dedicados. |
| `gameVersion` | Versão do cliente aceita. Precisa bater com a do jogo. |
| `xmpp_debug` | Grava o tráfego XMPP. Útil para diagnosticar, pesado no dia a dia. |
| `xmpp_debug_console` | Espelha esse tráfego no console. |
| `use_online_protect` | Liga a proteção de contagem de jogadores online. |

<br>

<h2><img src="https://api.iconify.design/solar/star-bold.svg?color=%2322C55E&width=26" align="top" /> &nbsp;Configurações recomendadas</h2>

Três combinações que funcionam, conforme o uso.

<details open>
<summary><b>Servidor caseiro, você e alguns amigos</b></summary>

<br>

O objetivo é entrar e jogar, sem espera e sem grind.

```json
"sponsors": {
    "unlock_all": true,
    "unlock_all_points": 999999,
    "starting_points": 0
}
```

E no `room.json`, tudo em `1`, para a partida começar assim que houver gente:

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

**Por que `unlock_all: true` aqui:** tira o grind por completo, o que combina com uma tarde entre amigos. Se preferir progressão, `false` também funciona: os pontos entram ao fim de cada partida.

</details>

<details>
<summary><b>Servidor com progressão, do jeito do jogo original</b></summary>

<br>

Cada jogador começa do zero e evolui.

```json
"sponsors": {
    "unlock_all": false,
    "unlock_all_points": 999999,
    "starting_points": 0
}
```

Canais separados por patente, no `masterservers.json`, para que iniciante não caia com veterano:

| Canal | `min_rank` | `max_rank` |
|---|---|---|
| `pvp_newbie_001` | 1 | 10 |
| `pvp_skilled_001` | 11 | 25 |
| `pvp_pro_001` | 26 | 90 |

> [!NOTE]
> Os pontos entram ao fim de cada partida, calculados pelo `SponsorPointsMultiplier` do gamedata. Os comandos SQL acima seguem úteis para ajustar um jogador específico.

</details>

<details>
<summary><b>Servidor público pequeno</b></summary>

<br>

Um meio-termo: o jogador não começa do zero absoluto, mas ainda tem o que conquistar.

```json
"sponsors": {
    "unlock_all": false,
    "unlock_all_points": 999999,
    "starting_points": 300
}
```

E desligue a depuração, que pesa com gente conectada:

```json
"xmpp_debug": false,
"xmpp_debug_console": false
```

**Por que `starting_points: 300`:** quem chega já encontra algumas opções abertas e não desiste na primeira tela, mas a maior parte do catálogo continua sendo conquista.

</details>

<br>

<h2><img src="https://api.iconify.design/solar/shield-warning-bold.svg?color=%23F97316&width=26" align="top" /> &nbsp;Cuidados</h2>

> [!WARNING]
> O arquivo `Config/sql.json` guarda a **senha do banco**. Nunca publique esse arquivo preenchido num repositório público, nem envie a alguém junto com o projeto.

> [!CAUTION]
> A porta do emulador não tem autenticação forte. Ao expor à internet, use firewall e libere só o necessário. Um servidor caseiro deve ficar em `127.0.0.1` ou atrás de VPN.

<br>

<h2><img src="https://api.iconify.design/solar/file-text-bold.svg?color=%233B82F6&width=26" align="top" /> &nbsp;Arquivos alterados</h2>

Todos dentro de `EmuWarface/`:

| Arquivo | Linha | O que foi feito |
|---|---|---|
| `Config/settings.json` | 13 a 17 | Bloco `sponsors` acrescentado |
| `Config.cs` | 113, 123 | Propriedade `Sponsors` e classe `SponsorsConfig` |
| `Game/Profile.cs` | 453 | Método `SponsorsSerialize()`, que decide entre liberar tudo ou ler o banco |
| `Game/Profile.cs` | 530 | `AddSponsorPoints()`, que credita os pontos ganhos na partida |
| `Game/Profile.cs` | 618 a 621 | Os três `INSERT` de perfil novo passam a usar `starting_points` |
| `Xmpp/Query/JoinChannel.cs` | 53 | Cinco linhas fixas trocadas por uma chamada ao método |
| `Xmpp/Query/CreateProfile.cs` | 70 | Mesma troca |
| `Xmpp/Query/SetRewardsInfo.cs` | 324 | Credita os pontos no perfil ao fim da partida |
| `Xmpp/Query/SetRewardsInfo.cs` | 344 | Envia ao cliente o valor calculado, no lugar do zero fixo |
| `emuwarface.sql` | 289 | `sponsor_points` passou de `tinyint` para `int unsigned` |

<details>
<summary><b>O código que estava lá antes</b></summary>

<br>

Em `JoinChannel.cs` e `CreateProfile.cs`, o trecho substituído era este:

```csharp
//TODO sponsors
XmlElement sponsor_info = Xml.Element("sponsor_info");
sponsor_info.Child(Xml.Element("sponsor").Attr("sponsor_id", "0").Attr("sponsor_points", "0").Attr("next_unlock_item", ""));
sponsor_info.Child(Xml.Element("sponsor").Attr("sponsor_id", "1").Attr("sponsor_points", "0").Attr("next_unlock_item", ""));
sponsor_info.Child(Xml.Element("sponsor").Attr("sponsor_id", "2").Attr("sponsor_points", "0").Attr("next_unlock_item", ""));
character.Child(sponsor_info);
```

O `//TODO sponsors` deixado pelo autor mostra que era um provisório à espera de implementação.

</details>

<br>

<h2><img src="https://api.iconify.design/solar/danger-triangle-bold.svg?color=%23F97316&width=26" align="top" /> &nbsp;Limitações conhecidas</h2>

| Limitação | Motivo |
|---|---|
| `next_unlock_item` fica vazio | Depende da tabela de itens de cada fornecedor, que o emulador ainda não carrega. |
| Perfis criados antes da alteração podem não ter linhas na `emu_sponsors` | Tratado automaticamente: o método cria as linhas faltantes no primeiro login, usando `starting_points`. |

<br>

<h2><img src="https://api.iconify.design/solar/checklist-minimalistic-bold.svg?color=%2322C55E&width=26" align="top" /> &nbsp;Verificação</h2>

O projeto foi compilado após as alterações:

```
26 Aviso(s)
 0 Erro(s)
```

Os 26 avisos já existiam no projeto original e não têm relação com estas mudanças.

<br>

<h2><img src="https://api.iconify.design/solar/heart-bold.svg?color=%2322C55E&width=26" align="top" /> &nbsp;Créditos</h2>

O EmuWarface é obra de **[n1kodim](https://github.com/n1kodim)** e da **[CryHub](https://github.com/WFCRYHUB)**, distribuído sob licença MIT. Todo o mérito do emulador é deles.

Este repositório é uma cópia com uma alteração pontual no sistema de fornecedores, documentada acima. A licença original está preservada em [LICENSE](../../LICENSE), e o README do projeto continua em [README.pt-BR.md](README.pt-BR.md).

<br>

<div align="center">
<sub>

Alteração sobre o [EmuWarface](https://github.com/n1kodim/EmuWarface) &nbsp;·&nbsp; Licença MIT &nbsp;·&nbsp; Build DEV20 1.22400.5519.45100

</sub>
</div>
