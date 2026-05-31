# 🎯 Project TDH (3D Multiplay Defense Tech Prototype)

본 프로젝트는 Photon PUN2 아키텍처를 기반으로 구현된 **3인 동시 접속 실시간 멀티플레이어 디펜스 게임**입니다. 실시간 동기화 환경에서 발생하는 **네트워크 패킷 과부하 해소**와 **클라이언트 런타임 프레임 최적화**를 목표로 설계 및 구현했습니다.

## 🛠 사용 기술 (Tech Stack)
- **Engine:** Unity 2022.3.14f1
- **Language:** C#[cite: 3]
- **Network:** Photon Unity Networking 2 (PUN2)[cite: 3]
- **Architecture:** Controller-Model 분리 구조 (MVC 패턴 응용)[cite: 3]

## 🚀 핵심 기술 구현 및 문제 해결 (Technical Challenges)

### 1. 데드 레코닝(Dead Reckoning) 기반 네트워크 패킷 최적화
- **문제 상황:** 실시간 멀티플레이어 환경에서 수많은 몬스터와 플레이어의 위치 데이터를 매 프레임(`Update`) 네트워크로 전송 시, 급격한 대역폭 과부하 및 프레임 드랍 발생.
- **해결 방안:** 이전 프레임의 이동 방향 벡터와 속도 데이터를 기반으로 다음 위치를 예측 연산하는 **데드 레코닝(Dead Reckoning) 알고리즘** 구현.
- **성과:** 동기화 전송 주기를 효율적으로 늘림으로써 데이터 패킷 전송량을 획기적으로 절감하고, 불연속적인 위치 변이를 선형 보간하여 끊김 없는 부드러운 동기화 구현.

### 2. 네트워크 및 로컬 오브젝트 풀링(Object Pooling) 이원화
- **설계 의도:** 가상 세계의 동기화 중요도에 따라 에셋 관리 레이어를 이원화하여 연산 부담 최소화.
- **네트워크 풀 (`NetworkPoolManager`):** 게임의 승패 및 상호작용에 직결되는 몬스터 객체는 `Master Client` 소유의 네트워크 풀에서 일괄 관리. 객체 재사용 시 **Pun RPC**를 통해 모든 클라이언트의 풀링 상태를 무결성 있게 제어[cite: 3].
- **로컬 풀 (`LocalPoolManager`):** 대량으로 생성되지만 동기화 중요도가 낮은 투사체(Projectile) 및 파티클 이펙트는 각 클라이언트의 개별 로컬 풀에서 관리하여 불필요한 네트워크 레이턴시 차단[cite: 3].

### 3. 유연한 확장을 위한 Controller-Model 아키텍처
- **설계 의도:** 입력 계층과 도메인 비즈니스 로직 계층의 분리를 통한 유지보수성 극대화[cite: 3].
- **구현:** `PlayerController`는 순수 유저의 조작 입력만을 처리하고 명령을 전달하며, `CharacterModel`은 이동, 공격, 스킬 등 캐릭터 고유의 행위 메커니즘을 캡슐화[cite: 3].
- **성과:** 캐릭터 모델이 입력 체계에 종속되지 않아, 추후 멀티플레이어 환경에서의 원격(Remote) 입력 조종 및 AI 봇(Bot) 제어 시스템으로 손쉽게 확장할 수 있는 아키텍처 확보[cite: 3].

### 4. ScriptableObject 기반 데이터 구동형 스킬 에디터 시스템
- **설계 의도:** 하드 코딩을 최소화하고 기획 데이터 변경에 유연하게 대응하는 시스템 구축[cite: 3].
- **구현:** 스킬의 고유 스탯, 타격 판정(`AttackEffect`), 버프 및 디버프 상태이상(`BuffEffect`) 로직을 `System.Serializable`로 직렬화하여 **ScriptableObject** 에셋 형태로 모듈화[cite: 3]. Inspector 상에서 데이터 조립만으로 새로운 스킬을 즉시 저작할 수 있는 높은 생산성 확보[cite: 3].

## 📂 프로젝트 구조 (Architecture)
- `Scripts/Controllers`: 입력 및 네트워크 동기화 제어 계층[cite: 3]
- `Scripts/Models`: 캐릭터 비즈니스 로직 및 컴포넌트 제어 계층[cite: 3]
- `Scripts/Managers`: 로컬/네트워크 오브젝트 풀 및 시스템 매니저 계층[cite: 3]
- `Scripts/Data`: ScriptableObject 기반 스킬/캐릭터 데이터 에셋[cite: 3]
