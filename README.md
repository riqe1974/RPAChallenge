# RPA Challenge - Sistema de Captura e Disponibilização de Dados

## Visão Geral da Solução

Este projeto consiste em um sistema automatizado composto por dois serviços independentes que se comunicam através de um banco de dados SQLite compartilhado:

1. **RPA (Worker Service)**: Um robô implementado como um serviço em background que, em intervalos regulares, acessa uma página web pública (`http://quotes.toscrape.com`), realiza a raspagem (scraping) de citações e autores, e persiste os dados no banco SQLite.

2. **Web API (ASP.NET Core)**: Uma aplicação RESTful que expõe os dados coletados através de endpoints, permitindo consultas e operações CRUD básicas sobre as citações.

Ambos os serviços são conteinerizados com Docker e orquestrados via Docker Compose, garantindo portabilidade, isolamento e facilidade de implantação.

### Tecnologias Utilizadas
- .NET 8 (C#)
- Entity Framework Core (opcional, não utilizado; preferiu-se ADO.NET via Microsoft.Data.Sqlite)
- HTML Agility Pack (para parsing HTML)
- Polly (para resiliência com políticas de retry)
- Serilog (logging estruturado)
- SQLite (banco de dados embutido)
- Docker / Docker Compose
- Git / GitHub

---

## Instruções Exatas de Como Rodar o Projeto

### Pré-requisitos
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) instalado e em execução.
- Git (para clonar o repositório).
- (Opcional) Visual Studio 2022 para desenvolvimento, mas a execução é totalmente via Docker.

### Passos para Execução

1. **Clone o repositório**
 ```bash
   git clone https://github.com/seu-usuario/rpachallenge.git
   cd rpachallenge
2. docker-compose up --build
   ```bash
   git clone https://github.com/seu-usuario/rpachallenge.git
   cd rpachallenge
