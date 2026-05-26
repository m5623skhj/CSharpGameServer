# Logic Worker

## 관련 코드

- [LogicWorkerThread.cs](../../CSharpGameServer/CSharpGameServer/Core/LogicWorkerThread/LogicWorkerThread.cs)
- [ServerCore.cs](../../CSharpGameServer/CSharpGameServer/Core/ServerCore.cs)

## 목적

수신 스레드에서 곧바로 게임 로직을 실행하지 않고, 워커 스레드에 패킷 처리를 위임한다.

## 구성

- 워커 수: `16`
- 각 워커는 `Queue<Tuple<Client, RequestPacket>>` 보유
- wake-up 방식: `AutoResetEvent`
- stop 방식: `ManualResetEvent`

## 분배 규칙

워커 인덱스 계산:

```text
threadId = ownerId % threadSize
```

코드 구현은 [LogicWorkerThread.cs](../../CSharpGameServer/CSharpGameServer/Core/LogicWorkerThread/LogicWorkerThread.cs)의 `GetThreadId()`에 들어 있다.

## 처리 순서

1. `ServerCore.ProcessPacket()`이 `PushPacket(client, packet)` 호출
2. 세션 ID로 대상 워커 결정
3. 큐에 `Tuple<Client, RequestPacket>` 적재
4. `DoWork()`로 대상 워커 이벤트 set
5. 워커가 큐를 비우며 `PacketHandlerManager.CallHandler()` 호출

## 현재 구조의 의미

- 동일 세션은 항상 같은 워커로 흐른다.
- 수신 스레드와 로직 스레드가 분리된다.
- 구현은 단순하며, 정교한 backpressure나 actor scheduler는 없다.

## 문서화 중 다시 확인된 점

- 이전의 busy loop 문제는 `AutoResetEvent`로 바뀌면서 해소됐다.
- `PushPacket()`은 워커가 running 상태일 때만 큐에 넣는다.
- 큐 자체는 lock 보호를 받지만, `Client` 객체가 워커와 I/O 경로에서 동시에 참조되는 구조다. 현재는 단순 처리라 괜찮지만 상태가 늘어나면 thread-safety 검토가 더 필요하다.

## 관련 문서

- [[Core/Networking]]
- [[Packet/Protocol]]
