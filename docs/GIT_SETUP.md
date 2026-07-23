\## Git 설정 (필수, 1회만) — Unity 씬/프리팹 충돌 방지



Unity 프로젝트는 씬(`.unity`), 프리팹(`.prefab`) 파일을 일반 텍스트 merge로 병합하면 깨질 수 있습니다.

\*\*아래 설정을 각자 로컬에서 한 번씩 실행\*\*해주세요. (`.gitattributes`는 이미 레포에 포함되어 있어 pull만 받으면 되지만, 아래 명령어는 개인 로컬 설정이라 자동으로 안 받아집니다.)



\### 1. Unity 설치 경로에서 UnityYAMLMerge 위치 확인



\- \*\*Windows\*\*: `C:\\Program Files\\Unity\\Hub\\Editor\\\[내 Unity 버전]\\Editor\\Data\\Tools\\UnityYAMLMerge.exe`

\- \*\*Mac\*\*: `/Applications/Unity/Hub/Editor/\[내 Unity 버전]/Unity.app/Contents/Tools/UnityYAMLMerge`



`\[내 Unity 버전]` 부분은 본인이 설치한 버전 폴더명으로 바꿔주세요. ex) 2022.3.62f3

* 확인하는 방법
1. Unity Hub 열기
2. 왼쪽 사이드바에서 "설치(Installs)" 탭 클릭
3. 거기 보이는 카드/목록에 2022.3.XXf1 또는 2023.X.XXfX 같은 형식의 숫자가 버전



\### 2. 터미널에서 아래 명령어 실행



\*\*Windows (PowerShell/cmd):\*\*

```bash

git config merge.tool unityyamlmerge

git config mergetool.unityyamlmerge.trustExitCode false

git config mergetool.unityyamlmerge.cmd "\\"C:/Program Files/Unity/Hub/Editor/\[내 Unity 버전]/Editor/Data/Tools/UnityYAMLMerge.exe\\" merge -p \\"$BASE\\" \\"$REMOTE\\" \\"$LOCAL\\" \\"$MERGED\\""

```



\*\*Mac:\*\*

```bash

git config merge.tool unityyamlmerge

git config mergetool.unityyamlmerge.trustExitCode false

git config mergetool.unityyamlmerge.cmd '/Applications/Unity/Hub/Editor/\[내 Unity 버전]/Unity.app/Contents/Tools/UnityYAMLMerge merge -p "$BASE" "$REMOTE" "$LOCAL" "$MERGED"'

```



\### 3. 충돌 발생 시 사용법



```bash

git mergetool

```

자동 병합이 안 되는 부분은 Unity Editor에서 직접 열어 확인 후 저장하면 됩니다.



\### 참고

\- 씬/프리팹을 최대한 \*\*같은 시간에 같은 파일을 건드리지 않는 것\*\*이 가장 확실한 예방책입니다. (미니게임별 담당 씬만 작업)

\- Asset Serialization이 `Force Text`로 설정되어 있어야 위 방식이 작동합니다. (Edit → Project Settings → Editor에서 확인)

