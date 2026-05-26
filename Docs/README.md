# CSharpGameServer Docs

이 폴더는 `CSharpGameServer` 저장소를 위한 Obsidian 문서 모음이다.

## 문서 시작점

- [[00_Overview]]
- [[GettingStarted]]
- [[Core/ServerLifecycle]]
- [[Core/Networking]]
- [[Core/LogicWorker]]
- [[Packet/Protocol]]
- [[DB/Database]]
- [[Common/ConfigAndLogging]]
- [[TestClient/TestClient]]
- [[Diagrams/README]]

## 문서 사용 방식

- 문서 간 이동은 `[[WikiLink]]` 기준이다.
- 코드 참조는 저장소 내부 상대 경로 Markdown 링크로 연결했다.
- 문서 내용은 현재 코드 기준으로 작성했다.
- 코드 링크까지 정상적으로 사용하려면 Obsidian에서 `Docs/`만 열지 말고 `저장소 루트`를 Vault로 열어야 한다.

## 빠른 메모

- 실제 서버 기능은 현재 `Ping -> Pong` 응답이 중심이다.
- `MigratorFilePath`가 비어 있으면 서버는 시작되지 않는다.
- DB 실행은 named parameter 기반으로 바뀌었고, 실패한 연결은 풀에 반환하지 않는다.
