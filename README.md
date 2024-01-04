# Projeto Breakeven StoneCo: Resili�ncia em Aplica��es ASP.NET

## Descri��o do Projeto

Este projeto tem como objetivo demonstrar a implementa��o de resili�ncia em aplica��es ASP.NET utilizando a biblioteca Polly em linguagem C#. 
A resili�ncia � uma caracter�stica crucial em sistemas distribu�dos, permitindo que as aplica��es se recuperem de falhas tempor�rias e forne�am uma experi�ncia cont�nua aos usu�rios, mesmo em condi��es adversas.

## Polly - Uma Vis�o Geral

[Polly](https://github.com/App-vNext/Polly) � uma biblioteca de pol�tica de resili�ncia que ajuda os desenvolvedores a lidar com falhas em suas aplica��es, fornecendo mecanismos simples e poderosos para implementar estrat�gias como retry, circuit breaker, fallback e mais. Este projeto utiliza Polly para aprimorar a resili�ncia das aplica��es ASP.NET.

## Recursos Principais

1. **Retry Policy:** Implementa��o de pol�ticas de repeti��o para lidar com falhas tempor�rias e transientes em chamadas de servi�os externos.

2. **Circuit Breaker:** Utiliza��o do circuit breaker para evitar chamadas repetidas a servi�os que est�o falhando, melhorando a efici�ncia e evitando poss�veis sobrecargas.

3. **Fallback Strategy:** Implementa��o de estrat�gias de fallback para fornecer alternativas ou valores padr�o quando uma chamada falha.

4. **Timeout:** 

## Pr�-requisitos

- Visual Studio 2019 ou superior
- .NET 7


## Como Executar (dotnet)

1. Clone este reposit�rio:

```bash
git clone https://github.com/JonathanAmarall/StoneBreakeven

2. Abra a solu��o no Visual Studio.

3. Execute o aplicativo e explore os diferentes cen�rios de falha que foram tratados usando Polly.


## Como Executar (docker)

1. Na raiz do projeto digite:

```bash
docker-compose up -d

2. Abra a solu��o no link: 

3. Teste a solu��o com o auxilio do Swagger OpenAPI