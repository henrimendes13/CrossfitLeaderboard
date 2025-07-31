# Crossfit Leaderboard

Aplicação ASP.NET Core para gerenciamento de leaderboard de Crossfit.

## Deploy na AWS

### Pré-requisitos
- Conta no GitHub
- Conta na AWS (com $100 créditos)

### Passos para Deploy

1. **Criar repositório no GitHub**
   - Faça push deste projeto para um repositório no GitHub

2. **Criar conta na AWS**
   - Acesse [aws.amazon.com](https://aws.amazon.com)
   - Crie uma conta com $100 créditos

3. **Criar aplicação no Elastic Beanstalk**
   - No AWS Console, vá para Elastic Beanstalk
   - Clique em "Create Application"
   - Selecione seu repositório GitHub

4. **Configurações do Deploy**
   - **Platform**: .NET Core on Linux
   - **Instance Type**: t2.micro
   - **Environment**: Single instance
   - **Branch**: main

5. **Variáveis de Ambiente**
   - Configuradas automaticamente via .ebextensions
   - `ASPNETCORE_ENVIRONMENT`: Production
   - `ASPNETCORE_URLS`: http://0.0.0.0:8080

6. **Deploy**
   - Clique em "Create Application"
   - AWS irá automaticamente fazer o build e deploy

### Acesso à Aplicação
Após o deploy, você receberá uma URL como: `https://crossfit-leaderboard.elasticbeanstalk.com`

### Observações
- O banco SQLite será criado automaticamente
- A aplicação usa t2.micro (gratuito com $100 créditos)
- Sem sleep automático - sempre ativo
- Dados persistentes entre reinicializações

### Estrutura do Projeto
- **Controllers**: Controladores MVC
- **Entities**: Modelos de dados
- **Services**: Lógica de negócio
- **Views**: Interface do usuário
- **Data**: Contexto do Entity Framework 