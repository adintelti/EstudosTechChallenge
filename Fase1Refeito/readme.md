FCG.Users.API

API REST desenvolvida em .NET 8 para gerenciamento de usuários, autenticação JWT e biblioteca de jogos da plataforma FIAP Cloud Games (FCG).

Este projeto foi desenvolvido como parte do Tech Challenge – Fase 1, utilizando arquitetura em camadas, autenticação JWT, PostgreSQL com Docker, testes unitários, logging estruturado e boas práticas de desenvolvimento backend.

📚 Funcionalidades
👤 Usuários
Cadastro de usuários
Login com JWT
Senha criptografada com BCrypt
Validação de senha forte
Validação de e-mail
🔐 Autenticação e autorização
JWT Bearer Authentication
Roles:
User
Admin
Controle de acesso por perfil
Proteção de rotas com [Authorize]
🎮 Games
Cadastro de jogos
Atualização de jogos
Exclusão de jogos
Consulta de jogos
Listagem de jogos
📚 Biblioteca do usuário
Adquirir jogos
Consultar biblioteca de jogos adquiridos
Proteção contra aquisição duplicada
Segurança horizontal:
usuário comum só acessa a própria biblioteca
administradores acessam qualquer biblioteca
🏗️ Arquitetura

O projeto foi desenvolvido utilizando arquitetura em camadas:

FCG.Users.API
│
├── FCG.Users.API
├── FCG.Users.Application
├── FCG.Users.Domain
├── FCG.Users.Infrastructure
└── FCG.Users.Tests
🧱 Tecnologias utilizadas
.NET 8
ASP.NET Core MVC
Entity Framework Core
PostgreSQL
Docker
JWT Authentication
BCrypt
FluentValidation
Serilog
Swagger
xUnit
Moq
🐳 Banco de dados com Docker
📦 Subir PostgreSQL

Execute:

docker compose up -d
⚙️ Connection String

Arquivo:

appsettings.json

Exemplo:

"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=fcg_users;Username=postgres;Password=postgres"
}
🚀 Como executar o projeto
1. Clonar repositório
git clone https://github.com/SEU-USUARIO/SEU-REPOSITORIO.git
2. Abrir no Visual Studio 2022

Abrir:

FCG.Users.API.sln
3. Restaurar pacotes
dotnet restore
4. Executar PostgreSQL no Docker
docker compose up -d
5. Executar migrations

No Package Manager Console:

Update-Database
6. Executar aplicação

Pressione:

F5

ou:

dotnet run
📄 Swagger

Ao executar a aplicação:

https://localhost:7181/swagger
🔐 Autenticação JWT
Login
POST /auth/login

Exemplo:

{
  "email": "admin@fcg.com",
  "password": "Admin@123"
}
Utilizar token
Realize login
Copie o token JWT
Clique em:
Authorize

no Swagger.

Informe:
Bearer SEU_TOKEN
👑 Usuário administrador seed

A aplicação cria automaticamente um usuário administrador ao iniciar.

Credenciais padrão
Email: admin@fcg.com
Senha: Admin@123
Role: Admin
📚 Principais endpoints
Auth
Método	Endpoint
POST	/auth/register
POST	/auth/login
Games
Método	Endpoint
GET	/games
GET	/games/{id}
POST	/games
PUT	/games/{id}
DELETE	/games/{id}
Biblioteca
Método	Endpoint
POST	/users/{userId}/games/{gameId}
GET	/users/{userId}/games
✅ Validações

Implementadas com FluentValidation.

Exemplos:

título obrigatório
preço maior que zero
senha forte
e-mail válido
🧪 Testes unitários

O projeto possui testes unitários utilizando:

xUnit
Moq
EF Core InMemory
Cobertura atual
AuthService
Cadastro com sucesso
Login com sucesso
Usuário já existente
Usuário inexistente
Senha inválida
GameService
Create
Update
Delete
GetById
Game inexistente
UserGameService
Adquirir jogo
Biblioteca do usuário
Usuário inexistente
Game inexistente
Jogo duplicado
📄 Logging

Implementado com Serilog.

Recursos
Logs em console
Logs em arquivo
CorrelationId
Logs estruturados
Logs automáticos HTTP
Logs de exceptions
📁 Logs

Os arquivos de log são gerados em:

Logs/
🔒 Segurança
JWT Authentication
Password Hashing com BCrypt
Controle de Roles
Proteção contra acesso indevido à biblioteca
Middleware global de exceptions
CorrelationId para rastreamento de requests

👨‍💻 Autor

Adriano Gomes Pimentel

Pós-graduando em Arquitetura de Sistemas .NET — Fiap Faculdade de Adminstração Paulista
Pós-graduação em Full Stack Development — PUC Minas
Tecnólogo em Análise e Desenvolvimento de Sistemas — FATEC Carapicuíba
Desenvolvedor .NET com experiência em backend, arquitetura e APIs REST
📄 Licença

Projeto desenvolvido para fins acadêmicos — Tech Challenge FIAP.