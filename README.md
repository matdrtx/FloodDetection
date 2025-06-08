# 🌊 Flood Detection API

Esta é a **API RESTful** do projeto **Flood Detection System**, responsável por centralizar os dados coletados por sensores ambientais, gerar previsões de alagamento com Machine Learning (ML.NET), armazenar leituras e distribuir alertas através de uma arquitetura escalável com microsserviços.

---

## 📚 Sumário

- [📦 Sobre o Projeto](#-sobre-o-projeto)
- [🧱 Arquitetura](#-arquitetura)
- [🛠 Tecnologias Utilizadas](#-tecnologias-utilizadas)
- [🚀 Como Executar o Projeto](#-como-executar-o-projeto)
- [📡 Endpoints da API](#-endpoints-da-api)
- [🧠 Machine Learning](#-machine-learning)
- [📨 RabbitMQ](#-rabbitmq)
- [🧪 Testes](#-testes)
- [📌 Rate Limiting](#-rate-limiting)

---

## 📦 Sobre o Projeto

Este sistema detecta alagamentos com base em leituras de sensores (nível do rio e intensidade da chuva) e gera alertas automaticamente com suporte a ML.NET. A API centraliza leituras, sensores e alertas, sendo o “cérebro” do sistema.

---

## 🧱 Arquitetura

```
[ ESP32 / Sensores ] --> [ API .NET ] --> [ RabbitMQ ] --> [ AlertWorker + ML.NET ]
                                            ↓
                                    [ Swagger + Logging ]
```

---

## 🛠 Tecnologias Utilizadas

- ASP.NET Core 8.0
- ML.NET (previsão de alagamentos)
- RabbitMQ (mensageria)
- Swagger (documentação interativa)
- Rate Limiting via `AspNetCoreRateLimit`
- API Versioning
- Xunit (testes automatizados)

---

## 🚀 Como Executar o Projeto

### ✅ Pré-requisitos

- [.NET SDK 8.0+](https://dotnet.microsoft.com/en-us/download)
- [RabbitMQ](https://www.rabbitmq.com/download.html) rodando localmente
- Docker (opcional, para RabbitMQ):

```bash
docker run -d --hostname my-rabbit --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management
```

Acesse o painel RabbitMQ: http://localhost:15672 (usuário: guest / senha: guest)

---

### 📂 Estrutura dos Projetos

- `FloodDetection.API` - API principal
- `FloodDetection.AlertWorker` - Worker com ML.NET que consome mensagens
- `FloodDetection.Domain` - Entidades de domínio
- `FloodDetection.ML` - Modelo de previsão
- `FloodDetection.Tests` - Testes automatizados

---

### ▶️ Executando a API

```bash
cd FloodDetection/FloodDetection.API
dotnet build
dotnet run
```

Acesse a documentação Swagger:

```
http://localhost:5019/swagger
```

---

### ▶️ Executando o AlertWorker

Em outro terminal:

```bash
cd FloodDetection/FloodDetection.AlertWorker
dotnet build
dotnet run
```

Este worker consumirá mensagens da fila `leituras`, preverá o risco de alagamento com ML.NET e gerará alertas no console.

---

## 📡 Endpoints da API

| Método | Rota                   | Descrição                                  |
|--------|------------------------|---------------------------------------------|
| GET    | `/api/v1/sensores`     | Lista todos os sensores                     |
| POST   | `/api/v1/sensores`     | Cria um novo sensor                         |
| GET    | `/api/v1/sensores/{id}`| Retorna um sensor por ID                    |
| DELETE | `/api/v1/sensores/{id}`| Remove um sensor                            |
| GET    | `/api/v1/leituras`     | Lista todas as leituras                     |
| POST   | `/api/v1/leituras`     | Envia uma leitura (e publica no RabbitMQ)   |
| GET    | `/api/v1/leituras/{id}`| Retorna uma leitura por ID                  |
| GET    | `/api/v1/alertas`      | Lista todos os alertas simulados            |
| POST   | `/api/v1/alertas`      | Simula a criação de um alerta               |

---

## 🧠 Machine Learning

O modelo `AlagamentoPredictor` usa valores de:

- `nívelAgua` (float)
- `velocidadeChuva` (float)

e prevê se haverá alagamento nas próximas horas, retornando:

- `VaiAlagar` (bool)
- `Probability` (float)

Com base nisso, o AlertWorker gera uma das seguintes ações:

- `Crítico`: evacuação imediata
- `Médio`: monitorar a região
- `Baixo`: nenhuma ação necessária

---

## 📨 RabbitMQ

A API envia mensagens para a fila `leituras`, que são consumidas pelo AlertWorker.

- Mensagens são publicadas no formato JSON
- O consumidor está sempre ativo enquanto o AlertWorker estiver rodando

---

## 🧪 Testes

Há cobertura inicial de testes com `Xunit` em:

- `SensorControllerTests.cs`

Execute os testes com:

```bash
cd FloodDetection/FloodDetection.Tests
dotnet test
```

---

## 📌 Rate Limiting

A API limita o número de requisições por IP:

- `x-rate-limit-limit`: limite total (ex: 30)
- `x-rate-limit-remaining`: quantas requisições ainda restam
- `x-rate-limit-reset`: quando o limite será resetado
