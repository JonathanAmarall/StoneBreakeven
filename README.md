# Projeto Breakeven StoneCo: Resiliência em Aplicações ASP.NET

## Descrição do Projeto

Este projeto tem como objetivo demonstrar a implementação de resiliência em aplicações ASP.NET utilizando a biblioteca Polly em linguagem C#. 
A resiliência é uma característica crucial em sistemas distribuídos, permitindo que as aplicações se recuperem de falhas temporárias e forneçam uma experiência contínua aos usuários, mesmo em condições adversas.

## Polly - Uma Visão Geral

[Polly](https://github.com/App-vNext/Polly) É uma biblioteca de política de resiliência que ajuda os desenvolvedores a lidar com falhas em suas aplicações, fornecendo mecanismos simples e poderosos para implementar estratégias como retry, circuit breaker, fallback e mais. Este projeto utiliza Polly para aprimorar a resiliência das aplicações ASP.NET.

## Recursos Principais

1. **Retry Policy:** Implementação de políticas de repetição para lidar com falhas temporárias e transientes em chamadas de serviços externos.

2. **Circuit Breaker:** Utilização do circuit breaker para evitar chamadas repetidas a serviços que estão falhando, melhorando a eficiência e evitando possíveis sobrecargas.

3. **Fallback Strategy:** Implementação de estratégias de fallback para fornecer alternativas ou valores padrão quando uma chamada falha.

4. **Timeout:** É uma que define o que devem acontecer após um determinado estado quando ultrapassa o tempo determinado.

## Pré-requisitos

- Visual Studio 2019 ou superior
- .NET 7


## Como Executar (dotnet)

1. Clone este repositório:

```bash
git clone https://github.com/JonathanAmarall/StoneBreakeven

2. Abra a solução no Visual Studio.

3. Execute o aplicativo e explore os diferentes cenários de falha que foram tratados usando Polly.


## Como Executar (docker)

1. Percorra até o diretório 'docker' e execute o seguinte comando:

docker-compose up -d

2. Abra a solução no link: http://localhost:5034/swagger

3. Teste a solução com o auxilio do Swagger OpenAPI

```

## Contribuindo

Se você encontrar problemas ou desejar contribuir, sinta-se à vontade para abrir uma issue ou enviar um pull request. Todas as contribuições são bem-vindas!