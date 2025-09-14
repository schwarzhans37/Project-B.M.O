# 동양미래대학교 컴퓨터컴퓨터소프트웨어공학과
### [졸업작품 출품] 게임개발 프로젝트

***

> #### 작품명 : In To The Dark

> #### 개발자
<img width="736" height="184" alt="image" src="https://github.com/user-attachments/assets/23c24922-654a-4fee-b20b-104141961876" />

***

> #### 작품 소개
- __개요__ : 넓은 숲을 가로질러 던전을 찾아간 보물 사냥꾼들이 적대적 존재들을 피해 무사히 보물을 회수해 귀환해야하는 3D 멀티플레이 협동 공포게임

- __모티브__ : Lethal Company(2023)
<img width="616" height="353" alt="image" src="https://github.com/user-attachments/assets/ee44d3c0-9ba4-41db-97ab-265fb3df044b" />

  - Lethal Company : 우주선을 타고 행성에 착륙하여 자원을 회수하기 위해 필드를 돌아다니고, 적대적 개채들을 피해 살아남고 귀환해야하는 협동 게임

- __개발 환경__
  - 게임 엔진 : Unity 2022.3.23f1
  - 멀티플레이 서버 : Mirror API

- __멀티플레이 구조__
  - 개발 언어 : C#, Mirror API
  - 핵심 기능 : 리슨 서버 방식, 실시간 동기화
  - 기능 설명
     1) 리슨 서버 : 특정 호스트에 서버와 로컬 클라이언트를 생성하고 외부 리모트 클라이언트와 통신
     2) 실시간 동기화 : 서버에 플레이어를 생성하고 각각의 클라이언트에 공유, 클라이언트에서 변경 사항 발생 시 서버에 변경 사항 전달 및 공유의 형태로 실시간 동기화 구현

  - 호스트 서버
  <img width="802" height="342" alt="image" src="https://github.com/user-attachments/assets/955ca9f7-fef3-41a1-a469-29922dbe0044" />

  - 네트워크 동기화
  <img width="785" height="563" alt="image" src="https://github.com/user-attachments/assets/f825927d-49b5-4b2d-b606-8807dde247cd" />

- __몬스터__
  - 행동 패턴 분류 : 배회(Patrolling), 감지(Detecting), 추적(Chasing), 공격(Attacking)
  - 행동 패턴 설명
    1) 배회 : 터레인(지형) 위에서 장애물 및 이동불가 지역을 제외한 전역을 임의로 이동하거나 대기
    2) 감지 : 몬스터 각자의 감지범위를 기반으로, 시야, 소리 등을 통해 플레이어가 주변에 있음일 감지
    3) 추적 : 감지된 플레이어를 추적해 가까이 접근
    4) 공격 : 공격 사거리에 접근한 플레이어를 향해 공격
  - 몬스터 종류
  <img width="1149" height="257" alt="image" src="https://github.com/user-attachments/assets/54ceebbd-dcf4-41de-84c7-f29027fa2f52" />


