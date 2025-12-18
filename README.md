# CSharpGameServer

## 제작 기간 : 2024.03.23 ~ 진행중

1. 개요

---

1. 개요

C#으로 간단한 게임 서버 프레임워크를 제작해보고자 시작한 프로젝트 입니다.


---

대략적인 구조는 아래와 같습니다.

![image](https://github.com/m5623skhj/CSharpGameServer/assets/42509418/5d9f33c7-e295-4a6f-a225-3a9cd5ace605)

---

2. PacketGenerator

2.1 사용자 편의를 위해 PacketGenerate.bat으로 제공됩니다.

위 파일을 실행하면, PacketDefine.yml 파일에 기술 되어 있는 패킷 타입들로 아래 파일들을 새로 생성합니다.

* Server
  * PacketType.cs
  * Protocol.cs
  * PacketHandler.cs
  * ClientPacketHandler.cs
* Client
  * PacketType.cs
  * Protocol.cs

위의 실행을 완료하면, yml 파일에 정의했던 PacketName으로 PCPacketHandler.cs에 아래의 형식으로 정의해야 합니다(RequestPacket만 해당).

public override void HandlePacketName(PacketName inParameter) { 함수 정의 ... }

![image](https://github.com/user-attachments/assets/e41fdc78-cf09-47ac-9332-12a41b802a33)

위를 정의하면, 자동적으로 해당 패킷 타입이 왔을 경우 해당 함수를 호출합니다.

---

2.2  yml 파일 정의

* Unique Type
  * 이 yml 파일 내에서 무조건 하나만 존재해야하는 타입입니다.
  * 패킷은 생성되지 않고, PacketType에만 해당 타입이 생성됩니다.
 
* RequestPacket Type
  * 클라이언트에서 서버로 패킷을 보내는 경우의 타입입니다.
  * 이 타입의 경우, PCPacketHandler.cs에 패킷 핸들러를 정의해줘야 합니다(2.1 참조).
 
* ReplyPacket Type
  * 서버에서 클라이언트로 패킷을 보내는 경우의 타입입니다.
 
* 추가 유의 사항
  * PacketName이 중복될 경우, 에러를 출력하게 됩니다.
  * Unique 타입이 두 개 이상 정의될 경우, 에러를 출력하게 됩니다.
 
---

3. Migrator

프로그램 실행시, [마이그레이터](https://github.com/m5623skhj/DBMigrator)가 실행되며, 마이그레이션이 실패할 경우, 프로그램은 자동으로 종료하게 됩니다.

4 를 참조하여 DB 설정들을 자신의 상황에 맞게 수정해야 합니다.

---

4. config

config.json에 정의된 사용자 정의 옵션들을 읽어오게 하고 싶으면, Config.cs에 있는 ConfigItem 구조체에 변수를 추가해야 합니다.

단, 구조체의 변수 이름과 config.json 내의 변수 이름이 같아야 정상적으로 파싱이 됩니다.

![image](https://github.com/user-attachments/assets/60ad74f1-eacb-4280-a33f-565dce58cb16) ![image](https://github.com/user-attachments/assets/0959ed2c-1fed-478b-ac53-0bb070a00ec2)

---
