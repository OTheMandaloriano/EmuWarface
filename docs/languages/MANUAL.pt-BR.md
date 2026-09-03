<div align="center">

<img src="https://api.iconify.design/solar/global-linear.svg?color=%236E7681&width=18" align="top" /> &nbsp;<sub><b>Choose your language</b> &nbsp;·&nbsp; <b>Escolha seu idioma</b> &nbsp;·&nbsp; <b>Выберите язык</b></sub>

<a href="../../MANUAL.md"><img src="https://api.iconify.design/flag/us-4x3.svg?width=21" align="top" alt="English" /> <img src="https://img.shields.io/badge/English-6E7681?style=flat-square" alt="English" /></a>
&nbsp;&nbsp;
<a href="../../docs/languages/MANUAL.pt-BR.md"><img src="https://api.iconify.design/flag/br-4x3.svg?width=21" align="top" alt="Português" /> <img src="https://img.shields.io/badge/Portugu%C3%AAs-1F6FEB?style=flat-square" alt="Português" /></a>
&nbsp;&nbsp;
<a href="../../docs/languages/MANUAL.ru.md"><img src="https://api.iconify.design/flag/ru-4x3.svg?width=21" align="top" alt="Русский" /> <img src="https://img.shields.io/badge/%D0%A0%D1%83%D1%81%D1%81%D0%BA%D0%B8%D0%B9-6E7681?style=flat-square" alt="Русский" /></a>

</div>


<br>

Documento de referência do servidor. Para a visão geral do projeto, veja o [README](README.pt-BR.md); para o sistema de fornecedores, [VENDORS.pt-BR.md](VENDORS.pt-BR.md).

<br>

<h2><img src="https://api.iconify.design/solar/box-bold.svg?color=%233B82F6&width=26" align="top" /> &nbsp;Como o servidor funciona</h2>

O jogo original conversa com os servidores da desenvolvedora para tudo que não é o tiro em si: login, perfil, inventário, salas, clãs, loja. O EmuWarface põe um servidor no lugar desses, na sua máquina.

Quatro peças participam:

| Peça | Papel |
|---|---|
| **EmuWarface** | Responde ao cliente: login, perfil, salas, itens |
| **MySQL** | Guarda contas, perfis, inventário, clãs |
| **GameData** | Arquivos de configuração do jogo (missões, itens, recompensas) |
| **Cliente do jogo** | O Warface em si, apontado para o seu servidor |

A conversa acontece por **XMPP**, na porta **5222**.

> [!IMPORTANT]
> O EmuWarface **não** hospeda a partida em si. Ele cuida do lobby, do perfil e das salas. Quem roda o combate é o `DedicatedServer.exe`, que vem com o cliente do jogo.

<br>

<h2><img src="https://api.iconify.design/solar/download-bold.svg?color=%233B82F6&width=26" align="top" /> &nbsp;Instalação</h2>

### O que você precisa

<img src="https://api.iconify.design/devicon/dotnetcore.svg?width=18" align="top" /> &nbsp;[.NET 6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)

<img src="https://api.iconify.design/devicon/mysql-wordmark.svg?width=18" align="top" /> &nbsp;[MySQL](https://dev.mysql.com/downloads/installer/) ou MariaDB 10.9.2

<img src="https://api.iconify.design/solar/gamepad-bold.svg?color=%236E7681&width=18" align="top" /> &nbsp;Cliente do Warface, build **DEV20 1.22400.5519.45100**

> [!WARNING]
> O código-fonte **não inclui a pasta `GameData`**, e sem ela o servidor não sobe. Ela só vem nas releases. Se você clonou o repositório, baixe uma [release](https://github.com/OTheMandaloriano/EmuWarface/releases) e copie a `GameData` de lá para junto do executável.

### Passo a passo

**1. Banco de dados.** Crie um banco e importe o esquema:

```bash
mysql -u root -p -e "CREATE DATABASE emuwarface CHARACTER SET utf8mb4"
mysql -u root -p emuwarface < EmuWarface/emuwarface.sql
```

Isso cria as 26 tabelas e já insere contas de teste.

**2. Conexão com o banco.** Em `Config/sql.json`:

```json
{
    "server": "127.0.0.1",
    "user": "root",
    "password": "sua_senha_aqui",
    "database": "emuwarface",
    "characterSet": "utf8mb4",
    "port": 3306
}
```

**3. Servidor.** Em `Config/settings.json`, confira `host` e `port`. Para uso local, `127.0.0.1` e `5222` servem.

**4. Suba o servidor:**

```bash
EmuWarface.exe
```

Ou, do código-fonte:

```bash
dotnet run --project EmuWarface
```

**5. Aponte o cliente** para o seu servidor, no `online.cfg` do jogo:

```
online_host = warface
online_server = 127.0.0.1
```

<br>

<h2><img src="https://api.iconify.design/solar/users-group-rounded-bold.svg?color=%233B82F6&width=26" align="top" /> &nbsp;Contas e permissões</h2>

O esquema já traz contas prontas para teste:

| Login | Senha | Permissão |
|---|---|---|
| `user1` a `user4` | `12345` | Admin |

> [!CAUTION]
> Essas contas são públicas: qualquer pessoa que conheça o projeto sabe a senha. Antes de abrir o servidor para outras pessoas, troque as senhas e rebaixe as permissões.

### Criar uma conta

**Não existe cadastro pelo jogo.** As contas entram direto no banco:

```sql
INSERT INTO emu_users (login, password, token, permission, ipaddress)
VALUES ('novojogador', 'senha123', '1', 0, '');
```

### Os quatro níveis

| Nível | Valor | O que pode |
|---|---|---|
| `None` | 0 | Jogar. Usa `give`, `help` e `online` |
| `Give` | 1 | Reservado para dar itens |
| `Moderator` | 2 | Banir, expulsar, silenciar, ver salas |
| `Admin` | 3 | Tudo |

```sql
UPDATE emu_users SET permission = 2 WHERE login = 'novojogador';
```

<br>

<h2><img src="https://api.iconify.design/solar/command-bold.svg?color=%2322C55E&width=26" align="top" /> &nbsp;Comandos de administração</h2>

São 11 comandos, usados de dois jeitos: digitados **no console do servidor** (sempre com poder de Admin) ou **de dentro do jogo**, respeitando a permissão da conta.

| Comando | Atalho | Permissão | Para que serve |
|---|---|---|---|
| `give` | `g` | None | Dar itens, moedas e conquistas |
| `help` | | None | Lista os comandos disponíveis |
| `online` | | None | Quantos jogadores conectados |
| `ban` | | Moderator | Banir uma conta |
| `unban` | `ub` | Moderator | Remover banimento |
| `kick` | `k` | Moderator | Expulsar do servidor |
| `mute` | `m` | Moderator | Silenciar no chat |
| `unmute` | `um` | Moderator | Devolver a voz |
| `broadcast` | `bc` | Moderator | Aviso para todo o servidor |
| `rooms` | `r` | Moderator | Listar salas abertas |
| `setexp` | `exp` | Moderator | Definir a experiência de alguém |

### O comando `give` em detalhe

É o mais completo, e aceita vários tipos:

```bash
give user1 p ar29_shop            # arma permanente
give user1 e ar29_shop 10d        # por 10 dias
give user1 s sniper_fbs_01        # visual (skin)
give user1 b random_box_10        # caixa aleatória
give user1 m game 10000           # 10.000 WF$
give user1 m crown 10000          # 10.000 coroas
give user1 m cry 10000            # 10.000 kredits
give user1 a 2231                 # conquista
```

| Tipo | Atalho | O que dá |
|---|---|---|
| `permanent` | `p` | Item para sempre |
| `expiration` | `e` | Item com prazo (`10d`, `3h`) |
| `skin` | `s` | Visual de arma |
| `box` | `b` | Caixa aleatória |
| `money` | `m` | Moeda: `game`, `crown` ou `cry` |
| `achiev` | `a` | Conquista, por número |

> [!NOTE]
> `give user1 all` entrega o catálogo inteiro, mas só funciona com `use_online_protect` desligado.

### Banir e silenciar

Aceitam prazo. Sem prazo, é permanente:

```bash
ban user1 7d          # sete dias
mute user1 2h         # duas horas
kick user1
```

<br>

<h2><img src="https://api.iconify.design/solar/checklist-bold.svg?color=%2322C55E&width=26" align="top" /> &nbsp;O que o servidor faz</h2>

São **98 comandos de protocolo** atendidos, distribuídos em 91 arquivos. Por área:

| Área | Quantas | O que cobre |
|---|---|---|
| **Salas e partidas** | 23 | Criar, entrar, times, votação, início automático, reconexão, busca rápida |
| **Perfil e conta** | 21 | Login, criação, classes, aparência, estatísticas, tutorial |
| **Servidor** | 11 | Canais, masterservers, anticheat, telemetria, comandos |
| **Itens e loja** | 8 | Comprar, consumir, prolongar, cartas, itens vencidos |
| **Clãs** | 7 | Criar, convidar, expulsar, cargos, listagem |
| **Social** | 7 | Amigos, convites, mensagens, chat, denúncias |
| **Progresso** | 6 | Conquistas, contratos, multiplicadores de recompensa |
| **Outros** | 8 | Notificações, autorização, escolhas de interface |

### Sistemas prontos para usar

- **Bônus diário por dias seguidos**, com prêmios crescentes
- **Caixas aleatórias** com cartas
- **Clãs** com cargos, convites e ranking
- **Rating de PvP**
- **Estatísticas** por classe e por modo
- **Amigos** com status online
- **Anticheat** com modo de punição
- **Contratos** e conquistas

### Modos de jogo

| Modo | Descrição |
|---|---|
| `PvE_Private` | Cooperativo, sala fechada |
| `PvE_Autostart` | Cooperativo, início automático |
| `PvP_Public` | Competitivo, sala aberta |
| `PvP_Autostart` | Competitivo, início automático |
| `PvP_ClanWar` | Guerra de clãs |
| `PvP_Rating` | Partida ranqueada |

### Classes

Rifleman (Fuzileiro), Medic (Médico), Engineer (Engenheiro), Recon (Sniper) e Heavy (Pesado).

### Moedas

| Moeda | No jogo |
|---|---|
| `game` | WF$, ganho jogando |
| `cry` | Kredits, moeda paga |
| `crown` | Coroas |

<br>

<h2><img src="https://api.iconify.design/solar/settings-bold.svg?color=%233B82F6&width=26" align="top" /> &nbsp;Configuração</h2>

Cinco arquivos em `Config/`. Todos exigem reinício do servidor.

### `settings.json` — o principal

| Campo | Para que serve |
|---|---|
| `host` | Endereço em que escuta. `127.0.0.1` aceita só esta máquina |
| `port` | Porta do XMPP. Padrão `5222` |
| `dedicatedHosts` | Endereços que podem registrar servidores dedicados |
| `gameVersion` | Versão do cliente aceita. Precisa bater com o jogo |
| `certSecret` | Senha do certificado TLS |
| `xmpp_debug` | Grava o tráfego. Útil para diagnosticar, pesado no dia a dia |
| `xmpp_debug_console` | Espelha esse tráfego no console |
| `use_online_protect` | Proteção da contagem de jogadores |
| `sponsors` | Progressão de fornecedores (ver [VENDORS.pt-BR.md](VENDORS.pt-BR.md)) |

### `sql.json` — banco de dados

Endereço, usuário, senha, nome do banco e porta.

### `room.json` — quando a partida começa

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

Quantos jogadores precisam estar prontos, por tipo de sala. Em servidor de poucos amigos, `1` em tudo evita espera.

### `masterservers.json` — canais

Quatro canais vêm configurados: `pve_001`, `pvp_newbie_001`, `pvp_skilled_001` e `pvp_pro_001`. O `min_rank` e o `max_rank` definem quem enxerga cada um.

### `defaultItems.json` — o kit inicial

72 itens que todo jogador recebe ao criar a conta:

```json
{ "name": "pt05_shop", "type": "Pistol", "classes": 29 }
```

`classes` é uma soma: Fuzileiro 1, Médico 2, Engenheiro 4, Sniper 8, Pesado 16. O `29` é 1+4+8+16, todas menos Médico. Para todas, use `31`.

<br>

<h2><img src="https://api.iconify.design/solar/database-bold.svg?color=%238B5CF6&width=26" align="top" /> &nbsp;O que fica guardado</h2>

26 tabelas no banco. As que você mais vai mexer:

| Tabela | Guarda |
|---|---|
| `emu_users` | Contas: login, senha, permissão |
| `emu_profiles` | Perfis: apelido, patente, moedas |
| `emu_items` | Inventário de cada jogador |
| `emu_clans` / `emu_clan_members` | Clãs e integrantes |
| `emu_friends` | Lista de amigos |
| `emu_achievements` | Conquistas obtidas |
| `emu_stats` | Estatísticas por classe e modo |
| `emu_sponsors` | Pontos de fornecedor |
| `emu_bans` / `emu_mutes` | Punições |
| `emu_login_bonus` | Sequência de dias seguidos |
| `emu_pvp_rating` | Rating competitivo |

<br>

<h2><img src="https://api.iconify.design/solar/folder-with-files-bold.svg?color=%238B5CF6&width=26" align="top" /> &nbsp;Estrutura do projeto</h2>

### Raiz do repositório

| Arquivo ou pasta | Para que serve |
|---|---|
| `README.md` | Porta de entrada: o que o projeto é e como começar |
| `MANUAL.md` | Este documento |
| `VENDORS.md` | A alteração no sistema de fornecedores |
| `LICENSE` | Licença MIT do projeto original |
| `.gitignore` | O que o Git ignora (resultado de build, temporários) |
| `docs/languages/` | Traduções em português e russo |
| `EmuWarface/` | Todo o código-fonte |

### `EmuWarface/` — arquivos principais

| Arquivo | Para que serve |
|---|---|
| `Program.cs` | Ponto de partida. Inicia banco, comandos, gamedata, loja e servidor, nesta ordem |
| `Server.cs` | Aceita as conexões dos jogadores |
| `Config.cs` | Lê os arquivos de `Config/` e os transforma em objetos |
| `Log.cs` | Registro em tela e em arquivo |
| `Utils.cs` | Funções auxiliares de uso geral |
| `emuwarface.sql` | Esquema do banco: as 26 tabelas e as contas de teste |
| `EmuWarface.csproj` | Definição do projeto e das dependências |

### `Core/` — o motor

Treze arquivos que sustentam o resto. Não têm regra de jogo: cuidam de conexão, banco e roteamento.

| Arquivo | Linhas | Para que serve |
|---|---|---|
| `Client.cs` | **877** | Cada jogador conectado. Faz login, valida a conta e mantém a sessão |
| `SQL.cs` | 162 | Toda a conversa com o MySQL |
| `DedicatedServer.cs` | 83 | Registra e acompanha os servidores que rodam as partidas |
| `MasterServer.cs` | 78 | Os canais de jogo (PvE, PvP) |
| `CommandHandler.cs` | 69 | Lê os comandos digitados no console |
| `QueryBinder.cs` | 29 | Descobre sozinho todas as funções de protocolo e as registra |
| `QueryData.cs` | 23 | Os dados de uma requisição |
| `QueryAttribute.cs` | 21 | A marcação `[Query]` que liga um comando do jogo a uma função |
| `QueryException.cs` | 21 | Erros de requisição |
| `ServerException.cs` | 15 | Erros do servidor |
| `ICmd.cs` | 11 | O contrato que todo comando de administração segue |
| `Permission.cs` | 10 | Os quatro níveis de acesso |
| `ConnectionState.cs` | 10 | Em que ponto da conexão o jogador está |

### `Game/` — as regras do jogo

| Arquivo | Linhas | Para que serve |
|---|---|---|
| `Profile.cs` | **942** | O perfil: patente, moedas, classe, aparência, fornecedores |
| `StatsManager.cs` | 486 | Estatísticas por classe, modo e arma |
| `GameData.cs` | 264 | Carrega os arquivos de configuração do jogo |
| `PlayerStat.cs` | 201 | Uma estatística isolada |
| `QueryCache.cs` | 191 | Guarda respostas prontas, para não recalcular |
| `GameRestrictionSystem.cs` | 174 | Restrições de equipamento por modo |
| `Mission.cs` | 148 | As missões e seus parâmetros |
| `Achievement.cs` | 146 | Conquistas |
| `PvpRatingState.cs` | 91 | Rating competitivo |
| `Friend.cs` | 77 | Lista de amigos e status online |
| `Invitation.cs` | 71 | Convites entre jogadores |
| `Quickplay.cs` | 62 | Busca rápida de partida |
| `RoomPlayerInfo.cs` | 47 | Dados de um jogador dentro da sala |
| `ProfileBan.cs` | 43 | Banimentos por perfil |
| `Chat.cs` | 34 | Mensagens |
| `GameDataConfig.cs` | 27 | Onde cada arquivo de gamedata fica |

### `Game/` — subpastas

| Pasta | Arquivos | Para que serve |
|---|---|---|
| `GameRooms/` | 12 | As salas: criação, times, votação, início automático, sessão. O `GameRoom.cs` sozinho tem 936 linhas |
| `Enums/` | 16 | Listas fixas: classes, modos, moedas, times, erros |
| `Notifications/` | 11 | Avisos ao jogador: item recebido, conquista, convite, banimento |
| `Shops/` | 4 | Loja e caixas aleatórias. O `Shop.cs` tem 384 linhas |
| `Items/` | 3 | Itens do inventário. O `Item.cs` tem 557 linhas |
| `Clans/` | 2 | Clãs, cargos e ranking. O `Clan.cs` tem 347 linhas |
| `GameRoomVotes/` | 2 | Votações dentro da partida |

### `Xmpp/` — o protocolo

| Arquivo | Linhas | Para que serve |
|---|---|---|
| `Query/` | **91 arquivos** | Um arquivo por assunto, atendendo 98 comandos do jogo |
| `StreamParser2.cs` | 225 | Lê o fluxo XML que chega pela rede |
| `Iq.cs` | 144 | As mensagens de pergunta e resposta |
| `Jid.cs` | 117 | O endereço de cada participante |
| `Stanza.cs` | 113 | A unidade básica de mensagem |
| `StreamParser.cs` | 100 | Leitor anterior, mantido no projeto |
| `Xml.cs` | 97 | Monta as respostas |
| `IqType.cs` | 14 | Tipos de mensagem: Get, Set, Result, Error |

### `Config/` — configuração

| Arquivo | Para que serve |
|---|---|
| `settings.json` | Endereço, porta, versão do jogo, depuração, fornecedores |
| `sql.json` | Conexão com o banco. **Guarda a senha** |
| `room.json` | Quantos jogadores para a partida começar |
| `masterservers.json` | Os quatro canais e suas faixas de patente |
| `defaultItems.json` | Os 72 itens do kit inicial |
| `cert.pfx` | Certificado TLS da conexão |

### `Commands/` — administração

Onze arquivos, um por comando. Todos seguem a interface `ICmd`, o que torna simples acrescentar outros: basta criar a classe, que o `CommandHandler` a encontra sozinho ao iniciar.

<br>

<h2><img src="https://api.iconify.design/solar/danger-triangle-bold.svg?color=%23F97316&width=26" align="top" /> &nbsp;O que não funciona</h2>

Levantamento honesto, feito lendo o código. Há **119 marcações de pendência** deixadas pelos autores.

| Limitação | Situação |
|---|---|
| **`GameData` não vem no código-fonte** | Só nas releases. Sem ela o servidor não sobe |
| **Sem cadastro pelo jogo** | Contas entram por SQL |
| **RCON** | O campo `rconPort` existe no código, mas a administração remota não foi implementada |
| **Recompensa por subir de patente** | Marcado como pendente em `Profile.cs` |
| **Banimento por rating** | Marcado como pendente |

> [!NOTE]
> As 119 pendências não significam que o servidor seja instável. A maioria é ajuste fino e observação dos autores. O essencial, do login à partida, funciona.

<br>

<h2><img src="https://api.iconify.design/solar/shield-warning-bold.svg?color=%23F97316&width=26" align="top" /> &nbsp;Segurança</h2>

> [!CAUTION]
> **Senhas em texto puro.** A tabela `emu_users` guarda a senha sem qualquer proteção. Quem acessar o banco lê todas. Nunca reaproveite uma senha que você use em outro lugar.

> [!WARNING]
> **Contas de teste são públicas.** `user1` a `user4`, senha `12345`, todas Admin. Troque antes de abrir para outras pessoas.

> [!CAUTION]
> **`sql.json` guarda a senha do banco.** Nunca publique esse arquivo preenchido nem o envie junto com o projeto.

Ao expor à internet, use firewall e libere só a porta necessária. Servidor caseiro deve ficar em `127.0.0.1` ou atrás de VPN.

<br>

<h2><img src="https://api.iconify.design/solar/bug-bold.svg?color=%23F97316&width=26" align="top" /> &nbsp;Quando algo dá errado</h2>

<details>
<summary><b>O servidor fecha assim que abre</b></summary>

<br>

Quase sempre é uma destas três:

1. **Falta a pasta `GameData`.** Confira se ela está junto do executável.
2. **MySQL fora do ar ou senha errada.** Teste a conexão do `sql.json` à mão.
3. **Banco vazio.** Importe o `emuwarface.sql`.

</details>

<details>
<summary><b>O jogo não conecta</b></summary>

<br>

- O `online.cfg` do cliente aponta para o endereço certo?
- O `gameVersion` do `settings.json` bate com a build do jogo?
- A porta 5222 está liberada no firewall?
- Ligue `xmpp_debug` e veja o que chega ao console.

</details>

<details>
<summary><b>Entro no lobby, mas a partida não começa</b></summary>

<br>

O EmuWarface não hospeda partidas: quem faz isso é o `DedicatedServer.exe`, do cliente. Confira se ele está rodando e se o endereço dele consta em `dedicatedHosts`.

</details>

<details>
<summary><b>Esqueci a senha de uma conta</b></summary>

<br>

Estão em texto puro no banco:

```sql
SELECT login, password FROM emu_users;
UPDATE emu_users SET password = 'nova' WHERE login = 'user1';
```

</details>

<br>

<h2><img src="https://api.iconify.design/solar/heart-bold.svg?color=%2322C55E&width=26" align="top" /> &nbsp;Créditos</h2>

O EmuWarface é obra de **[n1kodim](https://github.com/n1kodim)**, com contribuição de **[myrka32](https://github.com/myrka32)**, sob licença MIT. Este manual documenta o trabalho deles.

<br>

<div align="center">
<sub>

Baseado no [EmuWarface](https://github.com/n1kodim/EmuWarface) de n1kodim &nbsp;·&nbsp; Licença MIT &nbsp;·&nbsp; Build DEV20 1.22400.5519.45100

</sub>
</div>
