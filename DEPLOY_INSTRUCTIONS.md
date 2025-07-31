# 🚀 Instruções de Deploy no Render

## 📋 Pré-requisitos

1. **Conta no GitHub**
   - Crie uma conta em [github.com](https://github.com)
   - Crie um novo repositório público

2. **Conta no Render**
   - Acesse [render.com](https://render.com)
   - Crie uma conta gratuita

## 🔧 Passos para Deploy

### 1. Preparar o Repositório

```bash
# No seu projeto local
git init
git add .
git commit -m "Initial commit"
git branch -M main
git remote add origin https://github.com/SEU_USUARIO/SEU_REPOSITORIO.git
git push -u origin main
```

### 2. Configurar o Render

1. **Acesse o Dashboard do Render**
   - Faça login em [render.com](https://render.com)
   - Clique em "New +" no canto superior direito

2. **Criar Web Service**
   - Selecione "Web Service"
   - Conecte sua conta do GitHub
   - Selecione o repositório do projeto

3. **Configurar o Serviço**
   - **Name**: `crossfit-leaderboard` (ou qualquer nome)
   - **Environment**: `.NET`
   - **Region**: Escolha a mais próxima (ex: Oregon)
   - **Branch**: `main`
   - **Build Command**: `dotnet publish -c Release -o out`
   - **Start Command**: `dotnet CrossfitLeaderboard.dll`
   - **Plan**: `Free`

4. **Variáveis de Ambiente**
   - Clique em "Environment"
   - Adicione as seguintes variáveis:
     - `ASPNETCORE_ENVIRONMENT`: `Production`
     - `ASPNETCORE_URLS`: `http://0.0.0.0:$PORT`

5. **Deploy**
   - Clique em "Create Web Service"
   - Aguarde o build e deploy (pode levar 5-10 minutos)

### 3. Acessar a Aplicação

- Após o deploy, você receberá uma URL como:
  `https://crossfit-leaderboard.onrender.com`
- A aplicação estará disponível 24/7 no plano gratuito

## ⚠️ Observações Importantes

### Plano Gratuito do Render
- **750 horas/mês** (suficiente para 24h/dia por ~25 dias)
- **512MB RAM** - adequado para a aplicação
- **Sleep após 15 min** de inatividade
- **Wake automático** na primeira requisição

### Banco de Dados
- **SQLite** será criado automaticamente
- **Dados persistentes** entre reinicializações
- **Sem configuração adicional** necessária

### Limitações do Plano Gratuito
- **Sleep automático** após 15 min sem uso
- **Primeira requisição** pode demorar 30-60 segundos
- **Sem SSL customizado** (mas HTTPS é fornecido)

## 🔍 Troubleshooting

### Se o Deploy Falhar
1. **Verifique os logs** no dashboard do Render
2. **Confirme** que o repositório está público
3. **Verifique** se o `render.yaml` está na raiz do projeto
4. **Confirme** que o .NET 8.0 está sendo usado

### Se a Aplicação Não Carregar
1. **Aguarde** 1-2 minutos após o primeiro acesso
2. **Verifique** se as variáveis de ambiente estão corretas
3. **Confirme** que o banco SQLite foi criado

### Performance
- **Primeira requisição**: 30-60 segundos (wake up)
- **Requisições subsequentes**: 1-5 segundos
- **Ideal para uso temporário** (1 dia)

## 📞 Suporte

- **Render Docs**: [docs.render.com](https://docs.render.com)
- **Render Community**: [community.render.com](https://community.render.com)
- **GitHub Issues**: Para problemas específicos do código

## 🎯 Próximos Passos (Opcional)

Se quiser expandir no futuro:
- **Upgrade para plano pago** ($7/mês) para performance melhor
- **Banco PostgreSQL** para dados mais robustos
- **Custom domain** para URL personalizada
- **SSL customizado** para certificados próprios 