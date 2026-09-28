# AGENTS.md

이 문서는 이 프로젝트에서 작업하는 모든 AI 에이전트(Claude Code 등)가 공통으로 따르는 규칙입니다.

## 1. 프로젝트 개요

- 앱 종류: 모바일 세로 화면(portrait) 이미지 검색 앱
- 엔진: Unity 6000.3.10f1
- 렌더 파이프라인: URP (Universal Render Pipeline) 17.3.0

## 2. 아키텍처 규칙

- 레이어 구조: `Presentation → (Service) → Domain ← Data`, `Core`는 모든 레이어가 공용으로 참조한다.
- `Domain`(Model, Repository 인터페이스)은 `UnityEngine`과 DTO에 의존하지 않는다. 순수 C#으로 유지한다.
- DTO는 `Data` 레이어 밖으로 나가지 않는다. `Data → Domain` 경계에서 Mapper가 DTO를 Model로 변환한다.
- 실패는 예외 대신 Result 패턴으로 반환한다. 단, 취소(cancellation)는 예외로 전파한다(`OperationCanceledException` 등).
- 레이어마다 Assembly Definition(asmdef)을 두고, asmdef 참조로 의존 방향을 강제한다.
  (예: `Domain.asmdef`는 `Data`/`Presentation`/`UnityEngine` 어셈블리를 참조하지 않는다)

## 3. 테스트 규칙

- 새 기능을 구현하기 전에 테스트 시나리오를 먼저 합의하고 작성한다.
- Unit Test는 네트워크를 사용하지 않는다. 외부 의존성은 Mock으로 대체한다.
- 테스트를 통과시키기 위해 테스트 코드를 고치지 않는다. 테스트가 틀렸다고 판단되면 근거를 먼저 보고하고, 합의 후에만 수정한다.

## 4. 작업 규칙

- Unity 에디터 작업(씬, GameObject, 프리팹, 에셋 조작)은 `unity-cli` 스킬로 연결된 라이브 에디터를 통해 수행한다.
  씬/프리팹 파일을 직접 텍스트로 편집하지 않는다. 항상 라이브 에디터 연결 여부를 먼저 확인한다.
- 작업 완료를 보고할 때는 컴파일 결과와 테스트 결과 요약을 포함한다.
- 커밋(git commit)은 사용자가 명시적으로 요청했을 때만 수행한다.

## 5. 보안

- API 키, 토큰 등 비밀 값은 절대 커밋하지 않는다. 필요하면 환경 변수나 커밋되지 않는(.gitignore 처리된) 별도 설정 파일로 관리한다.
