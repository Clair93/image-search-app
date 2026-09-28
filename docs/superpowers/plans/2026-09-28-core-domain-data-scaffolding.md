# Core/Domain/Data 레이어 스캐폴딩 계획

**Goal:** Core/Domain/Data 3개 레이어의 폴더 구조, Assembly Definition 참조 관계, 필요 패키지, EditMode 테스트 어셈블리 구성을 확정한다 (코드 작성은 이 계획의 범위 밖 — 승인 후 별도 TDD 구현 계획에서 진행).

**Architecture:** `Presentation → (Service) → Domain ← Data`, `Core`는 공용. Domain은 UnityEngine에 의존하지 않는 순수 C#을 목표로 하되, 이번 설계 변경으로 UniTask(Unity 생태계 전용 패키지)에는 의존한다 — 이는 "Unity 밖에서도 재사용 가능"이라는 이전 목표를 일부 포기하는 절충이다.

**Tech Stack:** Unity 6000.3.10f1, URP 17.3.0, C# / .NET, `com.cysharp.unitask` (비동기), `com.unity.nuget.newtonsoft-json` (JSON), `System.Net.Http.HttpClient` (HTTP)

**Spec:** [`AGENTS.md`](../../../AGENTS.md), [`diagram.puml`](../../../diagram.puml)

## Global Constraints

- Domain(Model, Repository 인터페이스)은 UnityEngine과 DTO에 의존하지 않는다 (AGENTS.md 2항)
- DTO는 Data 레이어 밖으로 나가지 않는다. Mapper가 DTO→Model 변환 (AGENTS.md 2항)
- 실패는 예외 대신 Result 패턴, 취소는 예외로 전파 (AGENTS.md 2항)
- 레이어마다 asmdef로 의존 방향 강제 (AGENTS.md 2항)
- Unit Test는 네트워크를 쓰지 않고 Mock만 사용 (AGENTS.md 3항)
- Unity 에디터 작업은 unity-cli 스킬로 (AGENTS.md 4항)

## 변경 이력

- **2026-09-28**: 비동기 방식을 `Task`/BCL → **UniTask**(`com.cysharp.unitask`, git 참조)로 변경 (설계 변경, 팀 결정). `com.unity.nuget.newtonsoft-json`과 함께 이미 Package Manager API로 설치 완료, `Packages/manifest.json`에 반영 확인됨.

---

## 1. 폴더/파일 목록

```
Assets/
  Scripts/
    Core/
      Result.cs                          # Result<TData,TError> + nested Success/Error records
      NetworkError.cs                    # enum (diagram.puml의 ErrorCode와 네이밍 재확인 필요 — 아래 미해결 사항 참고)
      Core.asmdef
    Domain/
      Models/
        ImageItem.cs
      ValueObjects/
        ImageSearchQuery.cs
      Repositories/
        IImageSearchRepository.cs
      Domain.asmdef
    Data/
      Dtos/
        PixabayImageDto.cs
        PixabaySearchResponseDto.cs
      Mapping/
        PixabayResponseMapper.cs
      Sources/
        IPixabayDataSource.cs
        PixabayImageDataSource.cs
      Repositories/
        PixabayImageRepository.cs
      Data.asmdef

  Tests/
    EditMode/
      Core/
        ResultTests.cs
        Core.Tests.asmdef
      Domain/
        ImageSearchQueryTests.cs         # 필요 시
        Domain.Tests.asmdef
      Data/
        PixabayResponseMapperTests.cs
        PixabayImageRepositoryTests.cs
        PixabayImageDataSourceTests.cs
        Fixtures/
          PixabayJsonFixtures.cs         # Mock JSON 문자열 상수 모음
        Mocks/
          MockPixabayDataSource.cs       # IPixabayDataSource 수동 구현 fake
          FakeHttpMessageHandler.cs      # HttpClient용 fake transport
        Data.Tests.asmdef
```

## 2. asmdef 및 참조 관계

```
Core.asmdef        (references: UniTask)                         (noEngineReferences: true)
   ↑
Domain.asmdef       (references: Core, UniTask)                   (noEngineReferences: true)
   ↑
Data.asmdef         (references: Core, Domain, UniTask)
```

- `Core`/`Domain`은 `SearchAsync(...) : UniTask<Result<...>>` 시그니처 때문에 **UniTask 어셈블리(`com.cysharp.unitask` 패키지의 `UniTask.asmdef`, 어셈블리명 `UniTask`)를 명시적으로 참조**해야 한다.
- `noEngineReferences: true`는 그대로 유지 — Core/Domain 코드가 직접 `UnityEngine.*` API를 호출하는 건 여전히 막는다. 다만 UniTask 어셈블리 자체는 내부적으로 `UnityEngine`을 참조하므로(`noEngineReferences: false`), Domain이 "Unity 밖에서도 재사용 가능한 순수 C#"이라는 목표는 사실상 포기된 상태다 (diagram.puml에 노트로 남겨둠).
- `Data.asmdef`는 `Core`, `Domain`, `UniTask` 참조. HTTP는 `HttpClient`(BCL), JSON은 Newtonsoft.Json이라 UnityEngine 직접 참조는 필요 없음 — 원한다면 Data도 `noEngineReferences: true`로 둘 수 있음 (선택 사항).
- (참고, 범위 밖) 나중에 Presentation을 붙일 때: `Presentation.asmdef`는 `Core`+`Domain`(+`UniTask`)만 참조하고 `Data`는 직접 참조하지 않는다 (DIP). 구현체 연결은 별도 컴포지션 루트가 담당.

테스트 asmdef:
```
Core.Tests.asmdef    references: Core, UniTask                        (Test Assembly, Editor)
Domain.Tests.asmdef  references: Core, Domain, UniTask                (Test Assembly, Editor)
Data.Tests.asmdef    references: Core, Domain, Data, UniTask          (Test Assembly, Editor)
```

## 3. 필요 패키지 및 설치 방법

| 용도 | 선택 | 상태 |
|---|---|---|
| 비동기 | `com.cysharp.unitask` (UniTask 2.5.11, git 참조) | **설치 완료** — `unity command package_add`로 Package Manager API를 통해 설치, `Packages/manifest.json`에서 확인됨 |
| JSON 역직렬화 (DTO) | `com.unity.nuget.newtonsoft-json` (3.2.2) | **설치 완료** — 동일한 방식으로 설치 확인됨 |
| 테스트 프레임워크 | `com.unity.test-framework` (1.6.0) | 이미 설치돼 있던 패키지 |
| HTTP 호출 | `System.Net.Http.HttpClient` (BCL) | 설치 불필요 |
| Mock/Fake | 수동 작성 (프레임워크 없음) | 설치 불필요 — `IPixabayDataSource` 메서드가 하나뿐이라 손으로 만든 Fake로 충분 (YAGNI) |

## 4. 테스트 어셈블리 구성 & Mock 위치

- 레이어당 테스트 asmdef 하나씩, EditMode 전용(Editor 플랫폼).
- **NUnit `[Test]` 메서드 자체는 `async Task`로 선언**하고 내부에서 `UniTask` 반환값을 그냥 `await`하면 된다 — NUnit이 `UniTask`를 테스트 반환 타입으로 직접 인식하지 못하므로 별도 변환(`.AsTask()`) 없이 이 패턴을 쓴다.
- Mock/Fake 클래스는 프로덕션 코드가 아니라 해당 테스트 asmdef 안에만 존재 (`Assets/Tests/EditMode/Data/Mocks/`).
  - `MockPixabayDataSource` : `IPixabayDataSource` 수동 구현. `PixabayImageRepository` 테스트에서 사용 (UniTask 반환).
  - `FakeHttpMessageHandler` : `HttpClient`용 fake transport. `PixabayImageDataSource` 테스트에서 사용.
  - `PixabayJsonFixtures` : Mock JSON 문자열 상수 모음.
- `Core.Tests`/`Domain.Tests`는 대상이 순수 값 타입/enum이라 Mock 불필요.

## 미해결 사항 (승인 전 확인 필요)

1. **`ErrorCode` vs `NetworkError` 네이밍** — `diagram.puml`은 `ErrorCode { Network, Server, InvalidRequest, Unknown }`인데, 앞서 합의한 테스트 시나리오 문서에서는 `NetworkError { NoInternet, ServerError, NotFound, Unknown }`를 가정했다. 이번 UniTask 반영 작업 범위 밖이라 그대로 남겨뒀지만, 실제 코드 작성 전에 반드시 하나로 통일해야 한다.
2. **HttpClient vs UnityWebRequest** — 모바일 타겟에서 `HttpClient` 동작 검증 필요 (이전 계획에서도 언급).
3. **asmdef 이름 프리픽스** — `Core`/`Domain`/`Data`를 그대로 쓸지, 회사/앱명 프리픽스(`ImageSearch.Core` 등)를 붙일지.

---

이 계획이 승인되면, 다음 단계로 파일별 코드/테스트 스텝을 담은 세부 TDD 구현 계획을 별도 문서로 작성합니다.
