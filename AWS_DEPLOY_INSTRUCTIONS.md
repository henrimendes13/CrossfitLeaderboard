# 🚀 Deploy na AWS Elastic Beanstalk

## 📋 Pré-requisitos

1. **Conta AWS**
   - Conta AWS criada com $100 créditos
   - Acesso ao AWS Console

2. **Projeto no GitHub**
   - Repositório público
   - Código atualizado

## 🔧 Passo a Passo

### 1. Preparar o Projeto

```bash
# Commit e push das alterações
git add .
git commit -m "Configurado para AWS Elastic Beanstalk"
git push
```

### 2. Acessar AWS Console

1. **Faça login** em [aws.amazon.com](https://aws.amazon.com)
2. **Vá para Elastic Beanstalk**:
   - Console AWS → Elastic Beanstalk
   - Clique em "Create Application"

### 3. Configurar Application

1. **Application information**:
   - **Application name**: `crossfit-leaderboard`
   - **Description**: `Crossfit Leaderboard Application`

2. **Platform**:
   - **Platform**: `.NET Core on Linux`
   - **Platform branch**: `.NET Core on Linux 2`
   - **Platform version**: `2.2.0 (Recommended)`

3. **Application code**:
   - **Source**: `Public sample application`
   - **Repository**: Selecione seu repositório GitHub
   - **Branch**: `main`

### 4. Configurar Environment

1. **Environment information**:
   - **Environment name**: `crossfit-leaderboard-prod`
   - **Domain**: Deixe vazio (AWS gerará)

2. **Additional configuration**:
   - **Instance type**: `t2.micro` (gratuito)
   - **Single instance** (para economizar)

### 5. Deploy

1. **Clique em "Create application"**
2. **Aguarde** o deploy (5-10 minutos)
3. **URL será fornecida** automaticamente

## ⚙️ Configurações Importantes

### Arquivos Criados:

1. **`.ebextensions/01_environment.config`**
   - Configura variáveis de ambiente
   - Define porta 8080

2. **`.ebextensions/02_healthcheck.config`**
   - Configura health checks
   - Define t2.micro

3. **`.ebextensions/03_nginx.config`**
   - Configura proxy reverso
   - Roteia para aplicação .NET

4. **`Procfile`**
   - Define comando de inicialização

### Variáveis de Ambiente:
- `ASPNETCORE_ENVIRONMENT`: `Production`
- `ASPNETCORE_URLS`: `http://0.0.0.0:8080`

## 💰 Custos Estimados

### Com $100 créditos:
- **t2.micro**: ~$8-12/mês
- **Elastic IP**: $3-5/mês
- **Data transfer**: ~$1-2/mês
- **Total**: ~$15-20/mês

**Duração**: ~5-6 meses com $100 créditos

## ✅ Vantagens da AWS

- ✅ **Sem sleep** - sempre ativo
- ✅ **Dados persistentes** - SQLite mantido
- ✅ **Performance melhor** - t2.micro é mais rápido
- ✅ **SSL gratuito** - via Application Load Balancer
- ✅ **Custom domain** - possível
- ✅ **Backup automático** - opcional

## 🔍 Troubleshooting

### Se o Deploy Falhar:
1. **Verifique os logs** no Elastic Beanstalk
2. **Confirme** que o repositório está público
3. **Verifique** se os arquivos `.ebextensions` estão corretos
4. **Confirme** que o .NET 8.0 está sendo usado

### Se a Aplicação Não Carregar:
1. **Aguarde** 5-10 minutos após o deploy
2. **Verifique** se as variáveis de ambiente estão corretas
3. **Confirme** que o banco SQLite foi criado

### Performance:
- **Primeira requisição**: 1-5 segundos
- **Requisições subsequentes**: <1 segundo
- **Ideal para uso contínuo**

## 📞 Suporte

- **AWS Docs**: [docs.aws.amazon.com](https://docs.aws.amazon.com)
- **Elastic Beanstalk**: [docs.aws.amazon.com/elasticbeanstalk](https://docs.aws.amazon.com/elasticbeanstalk)
- **GitHub Issues**: Para problemas específicos do código

## 🎯 Próximos Passos (Opcional)

Se quiser expandir no futuro:
- **Upgrade para t2.small** para mais performance
- **Banco RDS** para dados mais robustos
- **Custom domain** com Route 53
- **SSL customizado** com Certificate Manager
- **Auto Scaling** para múltiplas instâncias 