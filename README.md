# 🔥 Guns-Survival 🔥

![Game Screenshot](https://via.placeholder.com/800x450.png?text=Your+Game+Screenshot+Here)

## 🎮 프로젝트 소개

**Guns-Survival**은 다양한 몬스터로부터 살아남는 것을 목표로 하는 1인칭 슈팅(FPS) 서바이벌 게임입니다. 좀비, 악어, 악마, 드래곤 등 다양한 적들이 당신을 위협합니다. 강력한 무기로 적들을 물리치고 끝까지 생존하세요!

## ✨ 주요 기능

*   **FPS 게임 플레이**: 실감 나는 1인칭 시점의 전투를 경험하세요.
*   **다양한 적**: 좀비, 악어, 악마, 드래곤 등 개성 넘치는 몬스터들이 등장합니다.
*   **무기 시스템**: 다양한 총기를 사용하여 적들을 전략적으로 상대할 수 있습니다.
*   **스포너 시스템**: 설정된 위치에서 지속적으로 적들이 생성되어 긴장감을 유지합니다.
*   **레벨업 시스템**: 플레이어의 성장을 통해 더욱 강력해질 수 있습니다.

## 🛠️ 기술 스택

*   **게임 엔진**: [Unity](https://unity.com/)
*   **주요 에셋**:
    *   [FPS Builder](https://assetstore.unity.com/packages/tools/game-toolkits/fps-builder-224206)
    *   Monster Packs (Zombies, Crocodiles, Demons, Dragons)
    *   Modern Weapons Pack
*   **입력 시스템**: Unity New Input System

## 📂 프로젝트 구조

```
Guns-Survival/
├── Assets/              # 모든 게임 리소스 (스크립트, 프리팹, 씬 등)
│   ├── Scenes/          # 게임 씬 파일
│   ├── Scripts/         # C# 스크립트
│   │   ├── Player/      # 플레이어 관련 로직
│   │   ├── Enemy/       # 적 관련 로직
│   │   ├── Spawner/     # 적 생성 관련 로직
│   │   └── Game/        # 게임 관리 로직
│   ├── Prefabs/         # 재사용 가능한 게임 오브젝트
│   └── Animations/      # 애니메이션 파일
├── ProjectSettings/     # Unity 프로젝트 설정
└── Packages/            # 프로젝트 종속성 패키지
```

## 🚀 시작하기

1.  이 저장소를 클론하거나 다운로드합니다. (Git LFS 를 먼저 초기화 하세요)
2.  Unity Hub를 통해 프로젝트를 엽니다. (권장 Unity 버전: 6000.0.42f1)
3.  `Assets/Scenes` 폴더에 있는 메인 씬(예: `SampleScene.unity` 또는 `PinePlayground.unity`)을 엽니다.
4.  Unity 에디터에서 플레이 버튼을 눌러 게임을 실행합니다.

---
