Desafio Outser
Este projeto é uma API RESTful desenvolvida para permitir a leitura e análise dos vencedores da categoria "Pior Filme" do Golden Raspberry Awards, utilizando um arquivo CSV como fonte de dados.

Tecnologias Utilizadas
C# com .NET 7.0: Para desenvolvimento da API.
Entity Framework Core: Para manipulação do banco de dados em memória (SQLite).
Injeção de Dependência (IoC): Implementada com Microsoft.Extensions.DependencyInjection.
Swagger: Para documentação e teste da API.
xUnit: Para testes de integração.
Arquitetura do Projeto
O projeto segue uma arquitetura em camadas para separar responsabilidades e facilitar a manutenção.

Estrutura de Pastas

+-- API
¦   +-- Controllers: Contém os endpoints expostos pela API.
¦   +-- Data: Inclui o arquivo CSV usado como fonte de dados.
¦   +-- appsettings.json: Configurações da aplicação.
¦   +-- Program.cs: Configuração inicial e execução da API.
+-- Application
¦   +-- DTOs: Objetos de transferência de dados para resposta da API.
¦   +-- Interfaces: Interfaces para abstrações de serviços e queries.
¦   +-- Services: Implementação da lógica de negócios.
+-- Domain
¦   +-- Entities: Representação das entidades do domínio.
¦   +-- Interfaces: Interfaces para abstração de repositórios.
+-- Infra
¦   +-- Configuration: Configuração das entidades com o EF Core.
¦   +-- Context: Configuração do contexto do banco de dados.
¦   +-- IoC: Configuração de injeção de dependência.
¦   +-- Queries: Implementação das consultas ao banco de dados.
¦   +-- Repositories: Implementação dos repositórios.
+-- Tests
¦   +-- AwardsIntegrationTests.cs: Testes de integração.

Pré-requisitos
SDK do .NET 7.0 ou superior instalado.
Editor de código como Visual Studio 2022 ou Visual Studio Code.
Ferramenta CLI dotnet configurada no PATH.

Como Executar o Projeto
1. Clone o Repositório

git clone https://github.com/wliragomes/Desafio-Outsera.git
cd desafio-outser
2. Configure o Ambiente
O arquivo movielist.csv já está incluído no diretório API/Data. Se for adicionado no local correto, ele será automaticamente carregado ao iniciar a aplicação.
Um endpoint adicional foi criado para facilitar a importação manual do arquivo CSV. Esse endpoint não é necessário caso o arquivo esteja no local correto.
3. Execute a API
Na raiz do projeto, execute os comandos:

dotnet build
dotnet run --project API
A API estará disponível em http://localhost:porta (ou https://localhost:porta para HTTPS).
O Swagger pode ser acessado em http://localhost:porta/swagger.
Endpoints Disponíveis
1. Obter Intervalos dos Produtores
GET /api/awards/producers/intervals

Retorna os produtores com o maior e menor intervalo entre dois prêmios consecutivos.

Exemplo de Resposta:

json
{
"min": [
{
"producer": "Producer 1",
"interval": 1,
"previousWin": 2008,
"followingWin": 2009
},
{
"producer": "Producer 2",
"interval": 1,
"previousWin": 2018,
"followingWin": 2019
}
],
"max": [
{
"producer": "Producer 1",
"interval": 99,
"previousWin": 1900,
"followingWin": 1999
},
{
"producer": "Producer 2",
"interval": 99,
"previousWin": 2000,
"followingWin": 2099
}
]
}

2. Importar Manualmente o CSV
POST /api/awards/import

Esse endpoint permite importar o arquivo CSV manualmente, caso ele não tenha sido carregado automaticamente. Ele espera um arquivo movielist.csv na mesma estrutura e formato fornecidos no repositório. Caso o arquivo já esteja no local correto (API/Data/movielist.csv), esse endpoint não será necessário.

Requisição: Sem corpo necessário, apenas o envio do arquivo no local esperado.

Resposta:

200 OK: Caso o arquivo seja importado com sucesso.
400 Bad Request: Se o arquivo não for encontrado ou tiver um formato inválido.
Como Executar os Testes
1. Configure o Ambiente
Certifique-se de que o arquivo test.runsettings está presente na raiz do projeto e contém o seguinte conteúdo:

<RunSettings>
  <RunConfiguration>
    <DisableAppDomain>true</DisableAppDomain>
    <DisableParallelization>true</DisableParallelization>
  </RunConfiguration>
</RunSettings>
2. Execute os Testes
Na raiz do repositório, rode o comando:

dotnet test --settings test.runsettings
3. Testes de Integração
Os testes de integração estão localizados no arquivo Tests/AwardsIntegrationTests.cs. Eles validam:

O carregamento correto do arquivo CSV no banco de dados.
A resposta do endpoint /api/awards/producers/intervals.
Considerações Técnicas

Arquitetura
Camadas:
API: Camada de apresentação, contendo os controladores da API.
Application: Contém a lógica de negócios e os contratos para serviços e queries.
Domain: Representa as entidades do domínio e abstrações.
Infra: Implementação dos repositórios e configurações de banco de dados.
Injeção de Dependência
Configurada na classe DependencyInjectionApi (Infra/IoC).
Adiciona automaticamente serviços, repositórios, e queries via AddInfrastructureApi.
Banco de Dados
SQLite em memória, configurado no arquivo ApplicationDbContext.cs (camada Infra).
O arquivo CSV é carregado automaticamente ao iniciar a aplicação, mas também pode ser importado manualmente via endpoint.

Autor
Nome: Washington de Lira Gomes
Contato: w.liragomes@yahoo.com.br
GitHub: https://github.com/wliragomes
