# Getting Started

## 전제 조건

Obsidian에서 문서를 볼 때는 `Docs/` 폴더만 열지 말고 `저장소 루트`를 Vault로 열어야 코드 링크가 같이 동작한다.

- .NET 9 SDK
- MySQL 접근 가능 환경
- migration 실행 파일

## 설정 파일

서버 설정 파일은 [config.json](../CSharpGameServer/CSharpGameServer/Config/config.json) 이다.

현재 사용되는 항목:

- `LogLevel`
- `DBServerIP`
- `DBSchemaName`
- `DBUserId`
- `DBUserPassword`
- `MigratorFilePath`

이 값들은 모두 필수다. 하나라도 비어 있으면 `Config.ReadConfig()` 단계에서 실패한다.

## 가장 중요한 기동 조건

[MigrationRunner.cs](../CSharpGameServer/CSharpGameServer/DB/Migration/MigrationRunner.cs) 기준으로 `MigratorFilePath`는 선택 사항이 아니다.

아래 중 하나라도 만족하면 서버는 시작되지 않는다.

- config 읽기 실패
- `MigratorFilePath` 빈 문자열
- migration 파일 없음
- migration 프로세스 exit code 비정상

## 서버 실행

```powershell
dotnet run --project CSharpGameServer\CSharpGameServer\CSharpGameServer.csproj
```

정상 흐름:

1. migration 실행
2. 설정 로드
3. DB 연결 관리자 초기화
4. 패킷/핸들러 등록
5. 로직 워커 생성
6. `10001` 포트 listen

## 테스트 실행

```powershell
dotnet test CSharpGameServer\CSharpGameServer.sln
```

## 테스트 클라이언트 실행

```powershell
dotnet run --project CSharpGameServer\TestClient\TestClient.csproj
```

테스트 클라이언트는 `127.0.0.1:10001`에 접속해 `PingPacket`을 보내고 `PongPacket`을 받는다.

## 관련 문서

- 전체 구조: [[00_Overview]]
- 설정과 로깅: [[Common/ConfigAndLogging]]
- 네트워크 흐름: [[Core/Networking]]
