# Database

## 관련 코드

- [MigrationRunner.cs](../../CSharpGameServer/CSharpGameServer/DB/Migration/MigrationRunner.cs)
- [DBConnectionManager.cs](../../CSharpGameServer/CSharpGameServer/DB/DBConnectionManager.cs)
- [DbConnection.cs](../../CSharpGameServer/CSharpGameServer/DB/DbConnection.cs)
- [SPBase.cs](../../CSharpGameServer/CSharpGameServer/DB/SPObjects/SPBase.cs)
- [SPObjectImpl.cs](../../CSharpGameServer/CSharpGameServer/DB/SPObjects/SPObjectImpl.cs)
- [BatchSPObject.cs](../../CSharpGameServer/CSharpGameServer/DB/SPObjects/BatchSPObject.cs)
- [LazyRunner.cs](../../CSharpGameServer/CSharpGameServer/LazyRunner/LazyRunner.cs)

## Migration 정책

현재 migration은 선택 사항이 아니다.

`RunMigration()`의 실패 조건:

- config 읽기 실패
- `MigratorFilePath` 빈 문자열
- migration 파일 없음
- 프로세스 시작 실패
- exit code 비정상

## 연결 풀

`DbConnectionManager`는 `Queue<DbConnection?>` 기반의 단순 풀을 가진다.

- 최대 풀 크기: `10`
- 없으면 새 연결 생성
- 성공한 연결만 풀 반환
- 실패한 연결은 폐기

## DbConnection 책임

`DbConnection`은 아래 실행 함수를 가진다.

- `Execute(SpBase)`
- `ExecuteWithResult(SpBase)`
- `ExecuteBatch(List<SpBase>)`
- `ExecuteBatchWithResult(List<SpBase>)`

공통 구조:

1. `SpBase.CreateCommand()` 또는 batch command 생성
2. SQL 실행
3. 성공 시 `OnCommit()`
4. 실패 시 `OnRollback()`

batch 경로는 transaction을 사용한다.

## SP 모델

`SpBase`는 현재 문자열 포맷 기반 SQL을 쓰지 않는다.

대신:

- `Query`
- `List<QueryParameter>`
- `AddParameter(name, value)`
- `CreateCommand(MySqlConnection)`

형태로 named parameter를 바인딩한다.

예제 구현:

```sql
SELECT * FROM tbl WHERE id = @Id AND name = @Name
```

## LazyRunner와 BatchSpObject

지연 실행 경로에서도 연결 반환 정책은 동일하다.

- 성공 시 `ReleaseConnection(connection, true)`
- 실패 시 `ReleaseConnection(connection, false)` -> 내부에서 close

## 문서화 중 다시 확인된 점

- 실패 연결 폐기 정책은 반영되어 있다.
- 다만 `DbConnectionManager.Initialize()`는 최초 1회만 동작하므로, 잘못된 설정으로 초기화된 뒤 재설정하는 경로는 없다.
- `Config.ReadConfig()`가 DB 필수 문자열 공백을 검증하지 않기 때문에, migration이 성공해도 DB 초기화는 빈 값으로 진행될 수 있다.

## 관련 문서

- [[Common/ConfigAndLogging]]
- [[GettingStarted]]
