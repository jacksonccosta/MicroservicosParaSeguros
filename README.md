# Solução de Microsserviços para Seguros

Este projeto é uma implementação de um desafio técnico para uma plataforma de seguros, utilizando uma arquitetura de microsserviços com .NET 8 e seguindo os princípios da Arquitetura Hexagonal.

## Tecnologias Utilizadas

- **Backend:** .NET 8, C# 12, ASP.NET Core
- **Persistência de Dados:** Entity Framework Core 8, SQL Server
- **Containerização:** Docker
- **Testes:**
  - **Unitários:** xUnit, Moq
  - **Integração:** `Microsoft.AspNetCore.Mvc.Testing` (WebApplicationFactory)
  - **End-to-End (E2E):** Testcontainers, WireMock.Net
  - **Assertions:** FluentAssertions

## Arquitetura e Padrões de Projeto

A solução foi projetada com base em padrões modernos para garantir desacoplamento, testabilidade e manutenibilidade.

### 1. Arquitetura de Microsserviços

O sistema é dividido em dois serviços independentes, cada um com sua própria responsabilidade e banco de dados:
* **`PropostaService`**: Gerencia todo o ciclo de vida das propostas de seguro.
* **`ContratacaoService`**: Efetiva a contratação de propostas aprovadas, comunicando-se com o `PropostaService` para validação.

### 2. Arquitetura Hexagonal (Ports & Adapters)

Este é o padrão central de cada microsserviço, garantindo que a lógica de negócio seja isolada da tecnologia.
* **Core / Hexágono**: O centro da aplicação, contendo as entidades de domínio e os casos de uso (lógica da aplicação). É uma "caixa preta" que não conhece o mundo exterior (APIs, bancos de dados, etc.).
* **Ports (Portas)**: São interfaces que definem o contrato de comunicação.
    * **Driving/Primary Ports**: Interfaces que expõem as funcionalidades do Core para o mundo exterior (ex: `IPropostaUseCaseService`).
    * **Driven/Secondary Ports**: Interfaces que o Core precisa para se comunicar com sistemas externos (ex: `IPropostaRepository`, `IPropostaGateway`).
* **Adapters (Adaptadores)**: São as implementações concretas das portas.
    * **Driving/Primary Adapters**: Adaptadores que "dirigem" a aplicação, como os Controllers da API REST, que recebem requisições e chamam os Ports primários.
    * **Driven/Secondary Adapters**: Adaptadores que são "dirigidos" pela aplicação, como as implementações de repositório com Entity Framework e os Gateways HTTP que implementam os Ports secundários.

### 3. Outros Padrões de Design
* **Domain-Driven Design (DDD)**: As entidades (`Proposta`, `Contratacao`) são ricas, contendo não apenas dados, mas também o comportamento e as regras de negócio associadas a eles.
* **Injeção de Dependência (DI)**: Usada extensivamente para conectar os `Adapters` aos `Ports` em tempo de execução, promovendo o baixo acoplamento.
* **Padrão GWT (Given-When-Then)**: Utilizado para estruturar os testes unitários, tornando-os mais legíveis e focados no comportamento do sistema.

## Estratégia de Testes

O projeto possui uma estratégia de testes em múltiplas camadas para garantir a qualidade:
* **Testes Unitários**: Focados no `Core` de cada serviço, testam a lógica de negócio pura (entidades e casos de uso) de forma isolada, usando `Moq` para simular as dependências (repositórios).
* **Testes de Integração**: Validam cada microsserviço de forma independente, mas completa. Usam `WebApplicationFactory` para hospedar a API em memória e se conectar a um banco de dados de teste real, garantindo que o fluxo desde o Controller até a persistência funcione. Dependências externas (outros microsserviços) são simuladas com `WireMock.Net`.
* **Testes End-to-End (E2E)**: Validam o sistema como um todo. Usam `Testcontainers` para orquestrar contêineres Docker (banco de dados, `PropostaService`, `ContratacaoService`) e testam o fluxo real de comunicação entre os serviços.

## Instruções de Build e Execução em uma Nova Máquina

Siga os passos abaixo para configurar e executar a solução em um novo ambiente de desenvolvimento.

### Pré-requisitos
* **.NET 8 SDK**
* **Visual Studio 2022**
* **SQL Server** (edição Express, Developer ou outra)
* **Docker Desktop** (necessário para os testes E2E e para rodar a aplicação via contêineres)

### Passo 1: Clonar o Repositório
```bash
git clone https://github.com/jacksonccosta/MicroservicosParaSeguros.git
cd <NOME_DA_PASTA>
```

### Passo 2: Configurar o Banco de Dados (Método Recomendado: Migrations)

1.  **Abra a solução** (`Seguros.Solucao.sln`) no Visual Studio 2022.
2.  **Configure as Connection Strings:**
    * No projeto `PropostaService.Api`, abra `appsettings.json` e edite a `DefaultConnection` para apontar para seu SQL Server. Crie um banco de dados vazio para ele (ex: `SegurosPropostaDB`).
    * Faça o mesmo no projeto `ContratacaoService.Api`, apontando para um segundo banco de dados vazio (ex: `SegurosContratacaoDB`).

3.  **Aplique as Migrations via Console do Gerenciador de Pacotes:**
    * Vá em **Ferramentas > Gerenciador de Pacotes do NuGet > Console do Gerenciador de Pacotes**.
    * **Para o PropostaService:**
        * Selecione `PropostaService.Infrastructure` como **Projeto Padrão**.
        * Execute: `Update-Database`
    * **Para o ContratacaoService:**
        * Selecione `ContratacaoService.Infrastructure` como **Projeto Padrão**.
        * Execute: `Update-Database`

    Isso criará todo o schema necessário em ambos os bancos de dados.

### Passo 3: Configurar a Comunicação entre Serviços

1.  Clique com o botão direito na solução e vá em **Propriedades**. Em **Projetos de Inicialização**, selecione **Projeto de Inicialização Único** e escolha `PropostaService.Api`. Rode o projeto.
2.  Anote a URL HTTPS da aplicação (ex: `https://localhost:7201`).
3.  Pare a execução.
4.  No projeto `ContratacaoService.Api`, abra o `appsettings.json` e cole a URL anotada na chave `Services:PropostaServiceUrl`.

### Passo 4: Executar a Solução Completa

1.  Clique com o botão direito na solução e vá em **Propriedades > Projetos de Inicialização**.
2.  Selecione a opção **"Vários projetos de inicialização"**.
3.  Defina a **Ação** como **"Iniciar"** para `PropostaService.Api` e `ContratacaoService.Api`.
4.  Pressione **F5** para iniciar ambos os microsserviços.

### Passo 5: Executar os Testes

Com o **Docker Desktop em execução**, abra o **Gerenciador de Testes** do Visual Studio (**Testar > Gerenciador de Testes**) e clique em "Executar Todos os Testes".