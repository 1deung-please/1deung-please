<img width="1600" height="900" alt="image" src="https://github.com/user-attachments/assets/2c146d99-db15-4442-8c20-6d274d47791d" />

# 1deung-please
NPC 11기 미니 프로젝트 1등되게 해주세요

---

## 팀원
1. 김민정 (팀장)
2. 심소연 (부팀장)
3. 황지우
4. 원유경

---

## 사용할 기술 스택
- 개발 툴: Unity Hub - Unity 2022.3.62f3 (LTS)
- 사용 언어: C# (게임 로직), Unity UI/UGUI (또는 UI Toolkit)
- 렌더 파이프라인: 2D (URP)
- 타겟 플랫폼: Mobile
- 작업물 저장 및 공유: GitHub

---

## 사용할 패키지 / 에셋
- **TextMeshPro**: UI 텍스트 렌더링 및 스타일링
- **DOTween**: UI 애니메이션 및 오브젝트 트윈 처리
- **Input System**: 터치 입력 처리
- **Cinemachine**: 씬 전환 및 연출용 카메라 워크
- **Addressables**: 씬/에셋 비동기 로딩

---

## Branch 전략
main branch와 하위 각 팀원별 branch 이용
- **main branch**: 배포 직전 단계의 브랜치. develop branch에서 개발이 끝나면 사용
- **develop branch**: main branch의 하위 브랜치로써, 개발 프로세스를 진행하는 브랜치
- **개인 branch**: develop branch의 하위 브랜치로, 팀원 개개인이 담당한 기능(미니게임 단위)을 개발하는 브랜치
  - 예: `feature/trash-picking-game`, `feature/seat-yield-game`, `feature/word-order-puzzle`

---

## Issue 전략
태그로 구분하며 두 가지 종류의 이슈를 다룸
- **[Request]**: 개선이 필요한 사항에 대한 요청 이슈
- **[Error]**: 오류가 있는 경우에 대한 이슈

---

## Pull Request 전략
태그로 구분하며 개발 및 기능 추가를 다룸
- **[Develop]**: 개발 완료에 대한 태그
- **[Add]**: 개선사항에 따른 기능 추가 태그

---

## Commit 컨벤션
각 태그를 이용하여 어떤 내용이 변경되었는지를 나타내는 규칙
- **[Feat]**: 새로운 기능 추가
- **[Fix]**: 버그 수정
- **[Docs]**: 문서 수정
- **[Design]**: UI/연출 수정
- **[Rename]**: 파일명, 폴더명 변경
- **[Remove]**: 파일 삭제

---

## 코드 컨벤션
코드 작성하면서 이름을 지정해야 하는 것들에 대한 규칙
- 클래스, 인터페이스: PascalCase ex) `TrashSpawnManager`
- 함수, 변수: camelCase ex) `spawnTrashItem()`, `remainingTime`
- Private 필드: camelCase + 언더스코어 접두사(_) ex) `_currentScore`
- 상수: UPPER_CASE ex) `const int MAX_TIME = 10;`
- Scene 파일명: PascalCase ex) `TrashPickingScene`, `SeatYieldScene`, `WordPuzzleScene`

---

## Unity 세팅
- 버전: 2022.3.62f3 (LTS)
- Render Pipeline: 2D (URP)
- Asset Serialization: Force Text
- Version Control Mode: Visible Meta Files
- 테스트: Unity Editor Play Mode 및 빌드 후 실기기 테스트
