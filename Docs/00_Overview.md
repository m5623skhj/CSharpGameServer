# CSharpGameServer 개요

`CSharpGameServer`는 `net9.0` 기반 TCP 서버 예제다. 현재 구현은 서버 골격, 패킷 파싱, 로직 워커, DB 실행 모델, 테스트 클라이언트, 단위 테스트까지 포함하지만 실제 게임 기능은 최소 수준이다.

## 프로젝트 구성

| 프로젝트 | 역할 |
| --- | --- |
| `CSharpGameServer` | 실제 서버 실행 프로젝트 |
| `TestClient` | `Ping/Pong` 바이너리 패킷 검증용 콘솔 클라이언트 |
| `ServerUnitTest` | 버퍼, 패킷, 설정, DB 명령 생성 테스트 |
| `DBMigrator` | 별도 migration 실행 도구 |

## 현재 구현된 핵심 기능

- TCP listen / accept / receive / send 루프
- 세션별 누적 스트림 버퍼
- 고정 길이 바이너리 패킷 파싱
- `PingPacket` 수신 후 `PongPacket` 응답
- 로직 워커 스레드 분배
- MySQL 연결 풀 골격
- named parameter 기반 쿼리 생성
- migration 필수 실행 정책

## 코드 기준 핵심 흐름

1. [Main.cs](../CSharpGameServer/CSharpGameServer/Main/Main.cs) 에서 `GameServer.Run()` 호출
2. [GameServer.cs](../CSharpGameServer/CSharpGameServer/GameServer/GameServer.cs) 에서 migration 실행
3. [ServerBase.cs](../CSharpGameServer/CSharpGameServer/Core/ServerBase.cs) 에서 `Initialize()` 성공 시 서버 실행
4. [ServerCore.cs](../CSharpGameServer/CSharpGameServer/Core/ServerCore.cs) 에서 accept/receive 시작
5. [PacketFactory.cs](../CSharpGameServer/CSharpGameServer/Packet/PacketFactory.cs) 로 누적 스트림의 헤더 파싱
6. [LogicWorkerThread.cs](../CSharpGameServer/CSharpGameServer/Core/LogicWorkerThread/LogicWorkerThread.cs) 로 패킷 처리 위임
7. [PCPacketHandler.cs](../CSharpGameServer/CSharpGameServer/PC/PCPacketHandler.cs) 의 `HandlePing()` 에서 `Pong` 응답

## 주요 문서

- 서버 시작과 종료: [[Core/ServerLifecycle]]
- 수신 버퍼와 패킷 파싱: [[Core/Networking]]
- 워커 스레드: [[Core/LogicWorker]]
- 패킷 구조: [[Packet/Protocol]]
- DB 계층: [[DB/Database]]
- 설정과 로깅: [[Common/ConfigAndLogging]]
- 테스트 클라이언트: [[TestClient/TestClient]]

## 현재 제약 사항

- `Config/config.json` 의 `MigratorFilePath`가 비어 있으면 서버 시작 실패
- `Config.ReadConfig()`는 필수 문자열 공백을 검증하지만, 설정 중복 로딩과 같은 구조적 문제는 남아 있음
- `ProcessPacket()`은 반복 파싱마다 누적 버퍼 전체 복사본을 생성
- `TestClient`는 `Read()` 1회로 전체 응답이 온다고 가정
- `RedisHelper`는 운영용 helper로 보기 어려운 상태
