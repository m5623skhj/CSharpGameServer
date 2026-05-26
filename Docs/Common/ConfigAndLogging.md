# Config And Logging

## 관련 코드

- [Config.cs](../../CSharpGameServer/CSharpGameServer/Config/Config.cs)
- [config.json](../../CSharpGameServer/CSharpGameServer/Config/config.json)
- [Logger.cs](../../CSharpGameServer/CSharpGameServer/Logger/Logger.cs)

## Config 구조

`ConfigItem` 필드:

- `LogLevel`
- `DbServerIp`
- `DbSchemaName`
- `DbUserId`
- `DbUserPassword`
- `MigratorFilePath`

JSON 예시는 [config.json](../../CSharpGameServer/CSharpGameServer/Config/config.json) 에 있다.

## 역직렬화 옵션

`Config.ReadConfig()`는 아래 옵션을 사용한다.

- `IncludeFields = true`
- `PropertyNameCaseInsensitive = true`
- `JsonStringEnumConverter`

따라서:

- field 기반 struct 역직렬화 가능
- `DBServerIP` 같은 대문자 키도 매핑 가능
- `LogLevel`은 문자열 enum 허용

## Logging

`LoggerManager`는 Serilog를 사용한다.

출력:

- Console
- `log.txt` rolling file

레벨:

- `LoggingLevelSwitch`
- config의 `LogLevel`로 변경

## 문서화 중 다시 확인된 점

- logger wrapper의 잘못된 레벨 필터는 제거된 상태다.
- `Config.ReadConfig()`는 예외 발생 시 false를 반환하고, DB 문자열과 `MigratorFilePath` 공백도 실패 처리한다.
- 다만 config는 migration과 server initialize에서 각각 다시 읽히므로, 설정 로딩 중복은 남아 있다.

## 관련 문서

- [[GettingStarted]]
- [[DB/Database]]
