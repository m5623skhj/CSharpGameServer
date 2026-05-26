# Server Lifecycle

## 관련 코드

- [Main.cs](../../CSharpGameServer/CSharpGameServer/Main/Main.cs)
- [GameServer.cs](../../CSharpGameServer/CSharpGameServer/GameServer/GameServer.cs)
- [ServerBase.cs](../../CSharpGameServer/CSharpGameServer/Core/ServerBase.cs)
- [GameServerCore.cs](../../CSharpGameServer/CSharpGameServer/GameServer/GameServerCore.cs)
- [ServerCore.cs](../../CSharpGameServer/CSharpGameServer/Core/ServerCore.cs)

## 시작 순서

### 1. 프로세스 진입

`Program.Main()`은 `GameServer.Run()`만 호출한다.

### 2. Migration 단계

`GameServer.Run()`은 먼저 `MigrationRunner.Instance.RunMigration()`을 호출한다.

실패하면:

- 콘솔에 failure 메시지 출력
- `false` 반환
- `Main`에서 `Environment.ExitCode = 1`

### 3. 서버 초기화

`ServerBase.Run()`은 `ServerCore.Initialize()` 성공 여부를 먼저 본다.

초기화 항목:

1. config 읽기
2. logger level 반영
3. DBConnectionManager 초기화
4. Packet register
5. ClientManager에 서버 코어 연결
6. logic worker 생성

### 4. 런타임 유지

초기화 성공 후:

- `ServerCore.Run()`으로 listen 시작
- 콘솔에서 `Esc` 입력 전까지 루프 유지

### 5. 종료

`finally` 블록에서 항상:

- `listenSocket.Close()`
- 모든 세션 종료
- 모든 로직 워커 정지

## GameServerCore 확장점

`GameServerCore`는 `ServerCore`를 상속한다.

- `PcManager.Instance.SetServerCore(this)` 호출
- accept 시 `Client` 대신 `Pc` 생성

즉 기본 네트워크 코어는 공통이고, 세션 구체 타입만 게임 서버용으로 교체한다.

## 문서화 중 다시 확인된 점

- 초기화 실패 시 서버는 더 이상 `Run()`으로 진입하지 않는다.
- `MigrationRunner`가 config를 한 번 읽고, `ServerCore.InitializeByConfig()`가 다시 config를 읽는다. 현재 구조상 동작은 맞지만 설정 로딩이 중복된다.

## 관련 문서

- [[Core/Networking]]
- [[Common/ConfigAndLogging]]
- [[DB/Database]]
