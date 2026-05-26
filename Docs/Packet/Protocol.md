# Protocol

## 관련 코드

- [Packet.cs](../../CSharpGameServer/CSharpGameServer/PacketBase/Packet.cs)
- [PacketFactory.cs](../../CSharpGameServer/CSharpGameServer/Packet/PacketFactory.cs)
- [Protocol.cs](../../CSharpGameServer/CSharpGameServer/Packet/Protocol.cs)
- [PacketRegisterList.cs](../../CSharpGameServer/CSharpGameServer/Packet/PacketRegisterList.cs)
- [PacketHandlerManager.cs](../../CSharpGameServer/CSharpGameServer/Packet/PacketHandlerManager.cs)

## 기본 구조

현재 패킷은 고정 길이 바이너리 구조다.

헤더:

- `PacketType`: `int`
- `PacketSize`: `ushort`

총 헤더 크기:

- `6 bytes`

## 패킷 타입

현재 코드에서 실제 등록되는 요청 패킷은 `PingPacket` 하나다.

- `PingPacket`
- `PongPacket`

## 패킷 클래스 계층

- `PacketBase`
  - `Type`
  - `SetPacketType()`
  - `LoadFromBytes()`
  - `ToBytes()`
- `RequestPacket`
  - 패킷 등록과 핸들러 연결 담당
- `ReplyPacket`
  - 응답 패킷 기반형

## 등록 방식

`RequestPacket.RegisterPacket()`이 아래 두 작업을 같이 한다.

1. `PacketFactory.RegisterPacket(Type, GetType())`
2. `PacketHandlerManager.RegisterPacketHandler(Type, GetHandler())`

현재 [PacketRegisterList.cs](../../CSharpGameServer/CSharpGameServer/Packet/PacketRegisterList.cs) 에서는 `PingPacket`만 등록된다.

## 직렬화 / 역직렬화

`PingPacket`과 `PongPacket`은 모두:

- `StructLayout(LayoutKind.Sequential, Pack = 1)`
- `Marshal.Copy()`
- `Marshal.PtrToStructure<T>()`

방식으로 직렬화/역직렬화된다.

수신 경로는 반드시 `LoadFromBytes()`를 통한다.
송신 경로는 반드시 `ToBytes()`를 통한다.

## 현재 서버 기능

[PCPacketHandler.cs](../../CSharpGameServer/CSharpGameServer/PC/PCPacketHandler.cs) 기준 구현된 동작은 하나다.

- `PingPacket` 수신
- `PongPacket` 송신

## 문서화 중 다시 확인된 점

- 요청 패킷 등록은 singleton dictionary 기반이다.
- 같은 타입과 같은 핸들러의 재등록은 idempotent 하게 허용한다.
- 다른 타입 또는 다른 핸들러를 같은 키에 다시 등록하려고 하면 여전히 실패한다.
- 현재는 요청/응답 모두 payload가 없는 헤더형 패킷이라 구조가 단순하다.

## 관련 문서

- [[Core/Networking]]
- [[TestClient/TestClient]]
