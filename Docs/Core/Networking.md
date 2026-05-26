# Networking

## 관련 코드

- [ServerCore.cs](../../CSharpGameServer/CSharpGameServer/Core/ServerCore.cs)
- [Client.cs](../../CSharpGameServer/CSharpGameServer/Core/Client.cs)
- [StreamRingBuffer.cs](../../CSharpGameServer/CSharpGameServer/Core/StreamRingBuffer.cs)
- [PacketFactory.cs](../../CSharpGameServer/CSharpGameServer/Packet/PacketFactory.cs)
- [Protocol.cs](../../CSharpGameServer/CSharpGameServer/Packet/Protocol.cs)

## listen 설정

- 포트: `10001`
- backlog: `200`
- receive buffer: `2048`

## accept 흐름

`StartAccept()`는 `SocketAsyncEventArgs`를 생성하고 `AcceptAsync()`를 호출한다.

- 비동기 완료: `AcceptCompleted()`
- 동기 완료: 즉시 `CompleteAccept()`

accept 성공 후:

1. session id 증가
2. `Client` 또는 `Pc` 생성
3. `ThreadPool.QueueUserWorkItem(StartReceive, client)` 호출
4. `ClientManager` 등록

## receive 흐름

`StartReceive()`는 각 세션에 대해 새 `SocketAsyncEventArgs`를 만들어 `ReceiveAsync()`를 건다.

- 비동기 완료면 `ReceiveCompleted()`
- 동기 완료면 즉시 `ProcessReceive()`

`ProcessReceive()`는:

1. `SocketError` 확인
2. `BytesTransferred <= 0` 인지 확인
3. receive buffer의 유효 길이만 새 배열로 복사
4. `ProcessPacket()` 호출

오류면 `CloseClient()`로 연결을 닫는다.

## 누적 스트림 파싱

세션은 [Client.cs](../../CSharpGameServer/CSharpGameServer/Core/Client.cs) 내부에 `StreamRingBuffer`를 가진다.

파싱 순서:

1. `PushStreamData(receivedData)`
2. `PeekAllStreamData()`
3. `PacketFactory.CreatePacket(storedStream, 0)`
4. 성공 시 `RemoveStreamData(packetLength)`
5. 패킷을 로직 워커 큐에 적재
6. 남은 데이터가 있으면 반복

이 방식으로 처리하는 TCP 상황:

- partial header
- fragmented body
- coalesced packet

## PacketFactory의 판단 규칙

헤더 크기:

- `PacketType`: 4 bytes
- `PacketSize`: 2 bytes
- 총 `6` bytes

판단 규칙:

- 남은 길이 `< 6` 이면 `IncompleteReceived`
- `packetLength < 6` 이면 `InvalidReceivedData`
- `packetLength > remainingSize` 이면 `IncompleteReceived`
- `packetLength > StreamRingBuffer.DefaultBufferSize` 이면 `InvalidReceivedData`
- `LoadFromBytes()` 실패 시 `InvalidReceivedData`

## send 흐름

`SendPacket()`은 `ReplyPacket.ToBytes()`만 사용한다.

즉 송신 경로는 현재 다음으로 고정되어 있다.

1. 패킷 객체 생성
2. `ToBytes()`
3. `SendAsync()`
4. 동기 완료면 즉시 `SendCompleted()`

## 문서화 중 다시 확인된 점

- 수신 경로는 correctness 위주로 정리되어 있지만, `ProcessPacket()`이 루프마다 `PeekAllStreamData()` 전체 복사본을 만든다.
- `StartReceive()`의 동기 완료 경로는 `while (true)` 구조라 연속 동기 완료 시 한 스레드를 오래 잡을 수 있다.

## 관련 문서

- [[Packet/Protocol]]
- [[Core/LogicWorker]]
- [[Diagrams/README]]
