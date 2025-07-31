# Crossfit Leaderboard

Aplicação ASP.NET Core para gerenciamento de leaderboard de Crossfit.

## Deploy no Render

### Pré-requisitos
- Conta no GitHub
- Conta no Render (gratuita)

### Passos para Deploy

1. **Criar repositório no GitHub**
   - Faça push deste projeto para um repositório no GitHub

2. **Criar conta no Render**
   - Acesse [render.com](https://render.com)
   - Crie uma conta gratuita

3. **Criar novo Web Service**
   - No dashboard do Render, clique em "New +"
   - Selecione "Web Service"
   - Conecte com sua conta do GitHub
   - Selecione o repositório do projeto

4. **Configurações do Deploy**
   - **Name**: crossfit-leaderboard (ou qualquer nome)
   - **Environment**: .NET
   - **Build Command**: `dotnet publish -c Release -o out`
   - **Start Command**: `dotnet CrossfitLeaderboard.dll`
   - **Plan**: Free

5. **Variáveis de Ambiente**
   - `ASPNETCORE_ENVIRONMENT`: Production
   - `ASPNETCORE_URLS`: http://0.0.0.0:$PORT

6. **Deploy**
   - Clique em "Create Web Service"
   - O Render irá automaticamente fazer o build e deploy

### Acesso à Aplicação
Após o deploy, você receberá uma URL como: `https://crossfit-leaderboard.onrender.com`

### Observações
- O banco SQLite será criado automaticamente
- A aplicação usa o plano gratuito do Render (750 horas/mês)
- Para uso temporário (1 dia), o plano gratuito é suficiente

### Estrutura do Projeto
- **Controllers**: Controladores MVC
- **Entities**: Modelos de dados
- **Services**: Lógica de negócio
- **Views**: Interface do usuário
- **Data**: Contexto do Entity Framework 