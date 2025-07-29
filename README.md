# Crossfit Leaderboard

Sistema de leaderboard para competições de CrossFit com atualização dinâmica em tempo real e persistência em PostgreSQL.

## 🎯 **Objetivo**

Uma única tela onde você pode:
- ✅ Inserir resultados de cada equipe em cada workout
- ✅ Ver atualização automática das posições e pontuações
- ✅ Visualizar o ranking em tempo real
- ✅ Resetar o leaderboard quando necessário
- ✅ **Criar quantas equipes quiser**
- ✅ **Criar quantos workouts quiser**
- ✅ **Persistência em banco de dados PostgreSQL**

## 🏗️ **Arquitetura**

### Estrutura de Pastas
```
CrossfitLeaderboard/
├── Controllers/          # Controladores MVC
├── Data/                # DbContext e configurações
├── Entities/            # Entidades do domínio
├── Models/              # ViewModels
├── Services/            # Lógica de negócio
│   ├── Interfaces/      # Interfaces dos serviços
│   └── Repositories/    # Repositórios (EF Core)
├── Views/               # Views Razor
└── wwwroot/js/         # JavaScript
```

### Componentes Principais

#### **1. Entity Framework Core**
- ✅ **ApplicationDbContext** - Configuração do banco PostgreSQL
- ✅ **Migrations automáticas** - Criação de tabelas
- ✅ **Seed data** - Dados iniciais

#### **2. Repositórios Entity Framework**
- ✅ **EntityFrameworkTeamRepository** - CRUD Teams
- ✅ **EntityFrameworkWorkoutRepository** - CRUD Workouts
- ✅ **EntityFrameworkWorkoutResultRepository** - CRUD Results

#### **3. Serviços**
- ✅ **TeamService** - Gerencia equipes
- ✅ **WorkoutService** - Gerencia workouts
- ✅ **LeaderboardService** - Orquestra tudo

## 🚀 **Funcionalidades**

### ✅ **Sistema de Pontuação**
- **1º Lugar** = 1 ponto
- **2º Lugar** = 2 pontos
- **3º Lugar** = 3 pontos
- **Sem resultado** = 0 ponto
- **Vencedor**: Equipe com menor total de pontos

### ✅ **Tipos de Workout**
- **Repetições/Peso**: Maior valor = melhor posição
- **Tempo**: Menor valor = melhor posição

### ✅ **Atualização Dinâmica**
- Posições recalculadas automaticamente
- Pontuações atualizadas em tempo real
- Ranking reordenado instantaneamente

### ✅ **Persistência de Dados**
- **PostgreSQL** como banco de dados
- **Entity Framework Core** para ORM
- **Migrations automáticas**
- **Dados persistentes** entre sessões

## 🎮 **Como Usar**

### 1. **Configurar PostgreSQL**
```bash
# Instalar PostgreSQL
# Criar banco de dados
CREATE DATABASE crossfit_leaderboard;
```

### 2. **Configurar Aplicação**
```json
// appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=crossfit_leaderboard;Username=postgres;Password=SUA_SENHA"
  }
}
```

### 3. **Executar Aplicação**
```bash
dotnet run
```

A aplicação irá:
- ✅ Conectar ao PostgreSQL
- ✅ Criar tabelas automaticamente
- ✅ Inserir dados de exemplo

### 4. **Usar o Sistema**
- **Acesse `/`** - Leaderboard principal
- **Acesse `/Teams`** - Gerenciar equipes
- **Acesse `/Workouts`** - Gerenciar workouts

## 📱 **Interface**

- **Tabela Responsiva**: Funciona em desktop e mobile
- **Badges de Posição**: Visualização clara das posições
- **Cores Dinâmicas**: Destaque para equipes com pontos
- **Instruções**: Guia visual do sistema de pontuação

## 🔧 **Tecnologias**

- **.NET 8.0** - Backend
- **ASP.NET Core MVC** - Framework web
- **Entity Framework Core** - ORM
- **PostgreSQL** - Banco de dados
- **Bootstrap 5** - Interface responsiva
- **jQuery** - Manipulação DOM
- **AJAX** - Comunicação assíncrona

## 📊 **Estrutura do Banco**

### Tabelas
- **Teams** - Equipes participantes
- **Workouts** - Exercícios/competições
- **WorkoutResults** - Resultados das equipes
- **Leaderboards** - Agregações (futuro)

### Relacionamentos
- **Team** ↔ **WorkoutResult** (1:N)
- **Workout** ↔ **WorkoutResult** (1:N)

## 🚀 **Executar o Projeto**

### 1. **Instalar PostgreSQL**
```bash
# Windows: https://www.postgresql.org/download/windows/
# Linux: sudo apt-get install postgresql
# macOS: brew install postgresql
```

### 2. **Configurar Banco**
```bash
psql -U postgres -h localhost
CREATE DATABASE crossfit_leaderboard;
\q
```

### 3. **Configurar Aplicação**
Editar `appsettings.json` com suas credenciais

### 4. **Executar**
```bash
dotnet run
```

Acesse: `https://localhost:7000`

## 📋 **Instruções Detalhadas**

Veja o arquivo `POSTGRESQL_SETUP.md` para instruções completas de configuração do PostgreSQL.

## 🎯 **Próximos Passos (Opcionais)**

Se quiser expandir no futuro:
- ✅ Múltiplas categorias (Masculino/Feminino)
- ✅ Autenticação de usuários
- ✅ API REST para integração
- ✅ Real-time com SignalR
- ✅ Relatórios e estatísticas 