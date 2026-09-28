# image-search

모바일 세로 이미지 검색 앱. Unity 6000.3.10f1, URP.

## 로컬 개발 환경 설정

### Pixabay API 키

1. https://pixabay.com/api/docs/ 에서 무료 API 키를 발급받습니다.
2. `Assets/Secrets~/pixabay-api-key.txt.example` 파일을 같은 폴더에 `pixabay-api-key.txt`라는 이름으로 복사합니다.
3. 복사한 파일을 열어 발급받은 키로 내용을 교체합니다.

키 파일이 없어도 앱은 정상적으로 실행됩니다 — 이 경우 `MockImageDataSource`(더미 데이터)로 자동 전환되며, 콘솔에 경고 로그가 표시됩니다.

CI 등 파일을 두기 어려운 환경에서는 환경변수 `PIXABAY_API_KEY`를 설정해도 됩니다 (환경변수가 파일보다 우선 적용됩니다).

`Assets/Secrets~/pixabay-api-key.txt`는 `.gitignore`에 등록되어 있어 Git에 커밋되지 않습니다.

자세한 아키텍처 규칙은 [AGENTS.md](./AGENTS.md)를 참고하세요.
