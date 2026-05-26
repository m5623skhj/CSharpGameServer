# TestClient

## 관련 코드

- [Main.cs](../../CSharpGameServer/TestClient/Main/Main.cs)
- [Protocol.cs](../../CSharpGameServer/TestClient/Protocol/Protocol.cs)
- [PacketType.cs](../../CSharpGameServer/TestClient/Protocol/PacketType.cs)

## 목적

서버가 현재 구현한 최소 프로토콜인 `Ping/Pong`을 빠르게 검증하는 콘솔 클라이언트다.

## 동작 흐름

1. `127.0.0.1:10001` 접속
2. `PingPacket` 헤더 생성
3. 구조체를 바이트 배열로 변환
4. 스트림으로 송신
5. `PongPacket` 크기만큼 버퍼 생성
6. `Read()` 1회 호출
7. 응답 구조체를 다시 해석
8. 수신 packet type 출력

## 현재 프로토콜 구조

공유 구조:

- `PacketHeader`
  - `PacketType`
  - `PacketSize`
- `PingPacket`
- `PongPacket`

## 문서화 중 다시 확인된 점

- 이전의 ASCII `"Ping"` 테스트보다 현재 구조가 서버 프로토콜과 맞는다.
- 수신은 이제 지정 길이를 모두 받을 때까지 반복 `Read()` 한다.
- 여전히 최소 기능 검증 도구이며, timeout 제어나 복수 패킷 검증까지 포함한 통합 테스트 도구는 아니다.

## 관련 문서

- [[Packet/Protocol]]
- [[Core/Networking]]
