# Cronograma Capilar — API

Back-end do app (Android e iOS) que monta e lembra o **cronograma capilar** da pessoa: qual tratamento fazer em cada lavagem (hidratação, nutrição, reconstrução...) e com qual produto.

**Stack:** .NET 10 · ASP.NET Core (controllers) · EF Core 10 + SQLite (local) · JWT (access + refresh token) · xUnit

## Arquitetura (DDD em camadas)

```
API  ──►  Application  ──►  Domain
 │              ▲
 └──►  Infrastructure ┘
```

| Camada | O que tem | Depende de |
|---|---|---|
| **Domain** | Entidades, objetos de valor, enums, regras de negócio e os contratos dos repositórios | ninguém |
| **Application** | Casos de uso (services), requests/responses (DTOs), `Result`/erros esperados | Domain |
| **Infrastructure** | EF Core (`AppDbContext`, DbSets, configurações das tabelas, migrations), repositórios, JWT, hash de senha e clientes de APIs externas | Application, Domain |
| **API** | Controllers, tratamento global de erros, rate limit, OpenAPI/Swagger | Application, Infrastructure |

### Convenções

- **Nomes no código em inglês**: classes, métodos, propriedades, enums, rotas, tabelas e colunas. **Mensagens para a pessoa em português**: erros e validações. Comentários em português.
- **Uma classe/record/interface/enum por arquivo.**
- Interface em uma pasta `Interfaces/` com o **mesmo nome** da implementação, e as pastas espelhadas entre as camadas:
  `Domain/Repositories/Interfaces/IUserRepository.cs` ↔ `Infrastructure/Repositories/UserRepository.cs`.
- Entidades com **setters privados** e métodos que validam as regras (`User.Create`, `SetHairProfile`...).
- Falha esperada (e-mail em uso, não encontrado) volta como `Result`. Regra de domínio violada lança `DomainException`, que vira **400**. O resto vira **500** sem expor detalhes.
- Enums começam em 1 (o 0 é inválido), trafegam como texto no JSON (`"Curly"`) e são salvos como texto no banco. O app traduz para português na tela.
- Versões de pacotes centralizadas em `Directory.Packages.props`.

## Como rodar

Pré-requisito: SDK do .NET 10.

```bash
dotnet tool restore                      # instala o dotnet-ef deste projeto
cd src/CronogramaCapilar.API
dotnet user-secrets set "Jwt:Key" "<uma chave aleatória com 32+ caracteres>"
dotnet run --launch-profile http
```

- Swagger: http://localhost:5143/swagger
- Banco: SQLite local em `src/CronogramaCapilar.API/cronograma-capilar.db`. Em desenvolvimento ele é criado e atualizado sozinho ao subir a API. Para ver os dados, use o [DB Browser for SQLite](https://sqlitebrowser.org/).

```bash
dotnet test                              # testes (na raiz)

# nova migration depois de mudar entidades/configurações (na raiz)
dotnet ef migrations add NomeDaMudanca --project src/CronogramaCapilar.Infrastructure --startup-project src/CronogramaCapilar.API --output-dir Persistence/Migrations
```

## Endpoints

| Método | Rota | Login? | O que faz |
|---|---|---|---|
| POST | `/api/auth/register` | não | Cria a conta e já devolve os tokens |
| POST | `/api/auth/login` | não | E-mail + senha → tokens |
| POST | `/api/auth/refresh` | não | Refresh token → tokens novos (o antigo deixa de valer) |
| POST | `/api/auth/logout` | não | Encerra a sessão deste aparelho |
| GET | `/api/users/me` | sim | Dados da pessoa + perfil capilar |
| PUT | `/api/users/me` | sim | Atualiza nome, como quer ser chamado(a), nascimento e gênero |
| PUT | `/api/users/me/hair-profile` | sim | Cria/substitui o perfil capilar (tipo, comprimento, lavagens/semana, tratamentos) |
| DELETE | `/api/users/me` | sim | Exclui a conta e tudo ligado a ela |
| GET | `/api/products?query=...&page=1&pageSize=20` | sim | Pesquisa produtos por nome, marca ou tipo |
| GET | `/api/products/{barcode}` | sim | Busca um produto pelo código de barras (embalagem escaneada) |

**Pesquisa de produtos:** os dados vêm do [Open Beauty Facts](https://world.openbeautyfacts.org/), uma base aberta e gratuita, e nada é salvo no nosso banco.
- O Open Beauty Facts permite só 10 pesquisas e 15 consultas de produto por minuto **por IP**, e todas as chamadas saem do nosso servidor. Por isso os resultados ficam em cache: pesquisas por 30 min, produtos por 24 h e "não encontrado" por 1 h.
- Cada usuário pode fazer 30 requisições por minuto nessas rotas.
- Se o Open Beauty Facts estiver fora do ar, a API responde **503**.
- Muitos produtos brasileiros ainda não têm foto na base (`imageUrl: null`), então o app deve mostrar uma imagem padrão.
- O `ProductCatalog:UserAgent` identifica o app para o Open Beauty Facts, que pede o formato `NomeDoApp/Versão (e-mail de contato)`. Como o repositório é público, o valor com e-mail fica nos user-secrets (em produção, na variável de ambiente `ProductCatalog__UserAgent`):
  ```bash
  dotnet user-secrets set "ProductCatalog:UserAgent" "CronogramaCapilar/1.0 (seu@email.com)"
  ```

**Sessão no app:** o access token dura 15 min e vai no header `Authorization: Bearer ...`. Quando expirar, o app chama `/refresh` com o refresh token, que dura 30 dias e deve ficar no armazenamento seguro do celular. Cada refresh token só pode ser usado uma vez. Se um token já trocado for reutilizado, todas as sessões da pessoa são derrubadas (proteção contra token roubado).

**Rate limit:** 20 requisições por minuto, por IP, nas rotas `/api/auth/*`.

## Roteiro

1. ✅ **Base:** arquitetura, banco, cadastro/login com refresh token, perfil capilar, excluir conta
2. ✅ **Produtos:** pesquisa por texto e por código de barras no Open Beauty Facts, com cache e resiliência (retry, timeout, circuit breaker)
3. **Cronograma:** o app recomenda um cronograma pelo perfil, e a pessoa escolhe seguir esse ou montar um do zero. Os dois são editáveis. Também marcar o dia como feito.
4. **Lembretes:** horários/dias dos alarmes, e um endpoint com os próximos lembretes para o app agendar notificações locais
5. **Depois:** notificações pelo servidor, histórico, deploy na nuvem, testes de integração
