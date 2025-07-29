# Configuração do PostgreSQL

## Pré-requisitos

1. **Instalar PostgreSQL**:
   - Baixe e instale o PostgreSQL: https://www.postgresql.org/download/
   - Durante a instalação, anote a senha do usuário `postgres`

2. **Verificar instalação**:
   ```bash
   psql --version
   ```

## Configuração do Banco de Dados

### 1. Conectar ao PostgreSQL
```bash
psql -U postgres -h localhost
```

### 2. Criar o banco de dados
```sql
CREATE DATABASE crossfit_leaderboard;
```

### 3. Verificar se foi criado
```sql
\l
```

### 4. Sair do psql
```sql
\q
```

## Configuração da Aplicação

### 1. Atualizar a string de conexão
No arquivo `appsettings.json`, atualize a string de conexão com suas credenciais:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=crossfit_leaderboard;Username=postgres;Password=SUA_SENHA_AQUI"
  }
}
```

### 2. Executar a aplicação
```bash
dotnet run
```

A aplicação irá:
- ✅ Conectar ao PostgreSQL
- ✅ Criar as tabelas automaticamente
- ✅ Inserir dados de exemplo (teams e workouts)

## Verificação

### 1. Verificar tabelas criadas
```bash
psql -U postgres -d crossfit_leaderboard -c "\dt"
```

### 2. Verificar dados inseridos
```bash
psql -U postgres -d crossfit_leaderboard -c "SELECT * FROM \"Teams\";"
psql -U postgres -d crossfit_leaderboard -c "SELECT * FROM \"Workouts\";"
```

## Troubleshooting

### Erro de conexão
- Verifique se o PostgreSQL está rodando
- Verifique se a senha está correta
- Verifique se o banco `crossfit_leaderboard` foi criado

### Erro de permissão
```sql
GRANT ALL PRIVILEGES ON DATABASE crossfit_leaderboard TO postgres;
```

### Reset do banco
```sql
DROP DATABASE crossfit_leaderboard;
CREATE DATABASE crossfit_leaderboard;
```

## Estrutura das Tabelas

### Teams
- `Id` (int, PK)
- `Name` (varchar(100))
- `TotalPoints` (int)

### Workouts
- `Id` (int, PK)
- `Name` (varchar(100))
- `Description` (varchar(500))
- `Type` (enum: Repetitions, Time, Weight)
- `Unit` (varchar(20))

### WorkoutResults
- `Id` (int, PK)
- `TeamId` (int, FK)
- `WorkoutId` (int, FK)
- `Result` (decimal(10,2))
- `Position` (int)
- `Points` (int) 