\# 화면 사이즈 / 카메라 / Canvas 세팅 가이드



가로 고정 안드로이드 게임 기준 (기획 문서 참고: 기준 해상도 1920×1080, 전체 캔버스 2400×1080, 좌우 240px 기기별로 잘릴 수 있음)



이미 세팅을 마친 팀장(MainMenu 씬 기준)이 정리한 문서이며, \*\*다른 미니게임 씬(02, 03) 및 Narrative/Tutorial 등을 담당하는 팀원은 아래 "팀원이 해야 할 일"만 따라 하면 됩니다.\*\*



\---



\## 1. 기준값 (프로젝트 공통 컨벤션)



| 항목 | 값 |

|---|---|

| 기준 해상도 (Reference Resolution) | 1920 x 1080 |

| 전체 캔버스 (여백 포함 최대) | 2400 x 1080 (좌우 최대 240px씩 잘릴 수 있음) |

| Pixels Per Unit (PPU) | 100 |

| Orthographic Size | 5.4 (= 1080 ÷ 2 ÷ 100) |

| Canvas Scaler - UI Scale Mode | Scale With Screen Size |

| Canvas Scaler - Screen Match Mode | Match Width Or Height |

| Canvas Scaler - Match 값 | 1 (Height 기준) |

| Unity 버전 | 2022.3 LTS (2022.3.62f3) |



> 세로(1080)는 기기와 무관하게 항상 고정, 가로만 기기 비율에 따라 늘어나거나 줄어듭니다. 그래서 Camera/Canvas 모두 \*\*세로(Height) 기준으로 스케일링\*\*하도록 세팅합니다.



\---



\## 2. 세팅 완료 항목 (MainMenu 씬 기준)



\### 카메라 (Main Camera)

\- Projection: Orthographic

\- Size: \*\*5.4\*\*



\### Canvas

\- Render Mode: Screen Space - Camera

\- Render Camera: Main Camera 연결

\- Canvas Scaler

&#x20; - UI Scale Mode: Scale With Screen Size

&#x20; - Reference Resolution: 1920 x 1080

&#x20; - Screen Match Mode: Match Width Or Height

&#x20; - Match: 1 (Height)



\### 안전 영역 가이드 프레임

\- Canvas 하위에 Image 오브젝트 생성, Width/Height = 1920 x 1080, Anchor = 중앙(0.5, 0.5), Pos = (0,0,0)

\- 작업 중 눈으로 안전 영역을 확인하는 용도 (반투명 색상 추천, 최종 빌드 전 비활성화/삭제 가능)



\---



\## 3. 프리팹 (팀 공용 템플릿)



`Assets/Prefabs/UI/` 폴더에 아래 3개 프리팹으로 저장되어 있습니다.



\- `MainCanvas.prefab` — Canvas + Canvas Scaler 세팅 + 안전 영역 가이드 프레임 포함

\- `MainCamera.prefab` — Orthographic Size 세팅 포함

\- `EventSystem.prefab` — UI 입력 처리용



> 씬에 Canvas를 새로 만들면 Unity가 EventSystem을 자동으로 하나 생성해주는데, 이미 씬에 EventSystem이 있다면 그걸 그대로 사용하면 됩니다 (프리팹으로 교체할 필요 없음). 다만 씬에 EventSystem이 2개 이상 되지 않도록만 주의해주세요.



\---



\## 4. 팀원이 해야 할 일 (본인 담당 씬에 적용하기)



각자 담당 미니게임 씬(예: MiniGame\_02, MiniGame\_03) 또는 Narrative/Tutorial 씬에서 아래 순서대로 진행해주세요.



1\. 씬에 기본으로 있는 Main Camera, Canvas(및 그에 딸린 EventSystem)가 있다면 \*\*삭제\*\* (이미 설정한 부분이 있다면 옮기기)

2\. `Assets/Prefabs/UI/MainCanvas.prefab`을 Hierarchy로 드래그해서 배치

3\. `Assets/Prefabs/UI/MainCamera.prefab`을 Hierarchy로 드래그해서 배치

4\. 씬에 이미 EventSystem이 있으면 그대로 사용하고, 없으면 `Assets/Prefabs/UI/EventSystem.prefab`을 드래그해서 배치 (씬에 EventSystem이 정확히 1개만 있는지 확인)

5\. 실제 UI 요소(점수, 타이머, 버튼 등) 배치 시:

&#x20;  - 핵심 UI/오브젝트는 \*\*1920×1080 안전 영역 안쪽\*\*에 배치 (좌우 240px 여백 영역에는 배경 요소만)

&#x20;  - 좌측 상단에 오는 UI는 Rect Transform의 Anchor Presets에서 \*\*Top-Left\*\*로 설정 (Shift+클릭하면 Pivot도 같이 맞춰짐)

&#x20;  - 화면 가장자리에 딱 붙이지 말고 60\~100px 정도 여백(패딩)을 주고 배치



\## 5. 프리팹 값 수정이 필요할 때



Orthographic Size나 Canvas Scaler 값 등을 바꿔야 하는 상황이 생기면, 아무 씬에서나 값을 바꾼 뒤 Inspector 상단의 \*\*Overrides → Apply All\*\*을 눌러야 원본 프리팹 및 이를 쓰는 다른 모든 씬에 반영됩니다. Apply 하지 않으면 그 씬에서만 바뀌고 나머지는 그대로 남습니다. 값을 바꾸기 전에 담당자(팀장)에게 먼저 공유해주세요.

