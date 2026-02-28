# Lush Path (Minecraft Custom Launcher)

A custom Minecraft launcher designed for server communities with automated server connection, mod management, and community features.

[한국어](#한국어) | [English](#english)

---

## English

## Project Structure

```
MinecraftLauncher/
├── src/
│   ├── MinecraftLauncher.Core/        # Business logic and core services
│   ├── MinecraftLauncher.Data/        # Data access and persistence
│   └── MinecraftLauncher.UI/          # WinForms user interface
├── tests/
│   └── MinecraftLauncher.Tests/       # Unit and property-based tests
└── MinecraftLauncher.sln              # Solution file
```

## Technology Stack

- **.NET 8.0** - Framework
- **Windows Forms** - UI framework
- **System.Text.Json** - JSON serialization
- **Serilog** - Logging with file rotation
- **Polly** - Retry policies for network requests
- **xUnit** - Unit testing framework
- **FsCheck** - Property-based testing
- **Moq** - Mocking framework

## Features

- **Profile Management** - Manage multiple server profiles with different configurations
- **Automatic Server Connection** - Launch Minecraft and connect to servers automatically
- **Mod Management** - Automatic mod download and updates from server
- **Mod Loader Support** - Forge, Fabric, and Vanilla support
- **Announcements** - Display server news and updates
- **Skin Preview** - 3D player skin rendering
- **Player Statistics** - View gameplay stats and achievements
- **Friend List** - Track online friends
- **UI Customization** - Custom logos and backgrounds per profile

## Directory Structure

The launcher stores data in `%AppData%/MinecraftLauncher/`:

```
MinecraftLauncher/
├── profiles/          # Profile configurations and mods
├── versions/          # Minecraft version installations
├── logs/              # Application logs (rotated at 10MB)
├── cache/             # Cached skins, announcements, statistics
├── config.json        # Main configuration
└── friends.json       # Friend list
```

## Building

```bash
# Restore dependencies
dotnet restore

# Build solution
dotnet build

# Run tests
dotnet test

# Run application
dotnet run --project src/MinecraftLauncher.UI
```

## Requirements

- Windows 10 or later
- .NET 8.0 Runtime
- Java (for Minecraft)

## Development

See the [design document](.kiro/specs/minecraft-custom-launcher/design.md) for detailed architecture and implementation notes.

## License

This project is for educational purposes.


---

## 한국어

Lush Path는 서버 커뮤니티를 위한 커스텀 마인크래프트 런처로, 자동 서버 연결, 모드 관리, 커뮤니티 기능을 제공합니다.

### 프로젝트 구조

```
MinecraftLauncher/
├── src/
│   ├── MinecraftLauncher.Core/        # 비즈니스 로직 및 핵심 서비스
│   ├── MinecraftLauncher.Data/        # 데이터 접근 및 영속성
│   └── MinecraftLauncher.UI/          # WinForms 사용자 인터페이스
├── tests/
│   └── MinecraftLauncher.Tests/       # 단위 테스트 및 속성 기반 테스트
└── MinecraftLauncher.sln              # 솔루션 파일
```

### 기술 스택

- **.NET 8.0** - 프레임워크
- **Windows Forms** - UI 프레임워크
- **System.Text.Json** - JSON 직렬화
- **Serilog** - 파일 로테이션 로깅
- **Polly** - 네트워크 요청 재시도 정책
- **xUnit** - 단위 테스트 프레임워크
- **FsCheck** - 속성 기반 테스트
- **Moq** - 모킹 프레임워크

### 주요 기능

- **프로필 관리** - 다양한 설정으로 여러 서버 프로필 관리
- **자동 서버 연결** - 마인크래프트 실행 및 서버 자동 연결
- **모드 관리** - 서버에서 모드 자동 다운로드 및 업데이트
- **모드 로더 지원** - Forge, Fabric, Paper, Vanilla 지원
- **공지사항** - 서버 뉴스 및 업데이트 표시
- **스킨 미리보기** - 3D 플레이어 스킨 렌더링
- **플레이어 통계** - 게임플레이 통계 및 업적 확인
- **친구 목록** - 온라인 친구 추적
- **UI 커스터마이징** - 프로필별 커스텀 로고 및 배경

### 디렉토리 구조

런처는 `%AppData%/MinecraftLauncher/`에 데이터를 저장합니다:

```
MinecraftLauncher/
├── profiles/          # 프로필 설정 및 모드
├── versions/          # 마인크래프트 버전 설치
├── logs/              # 애플리케이션 로그 (10MB에서 로테이션)
├── cache/             # 캐시된 스킨, 공지사항, 통계
├── config.json        # 메인 설정
└── friends.json       # 친구 목록
```

### 빌드 방법

```bash
# 의존성 복원
dotnet restore

# 솔루션 빌드
dotnet build

# 테스트 실행
dotnet test

# 애플리케이션 실행
dotnet run --project src/MinecraftLauncher.UI
```

### 시스템 요구사항

- Windows 10 이상
- .NET 8.0 런타임
- Java (마인크래프트용)
- 관리자 권한 (일부 기능)

### 설치 방법

1. [Releases](../../releases) 페이지에서 최신 버전 다운로드
2. 설치 프로그램 실행
3. 설치 완료 후 Lush Path 실행
4. 프로필 생성 및 서버 정보 입력

### 개발

자세한 아키텍처 및 구현 노트는 [설계 문서](.kiro/specs/minecraft-custom-launcher/design.md)를 참조하세요.

### 라이선스

이 프로젝트는 교육 목적으로 제작되었습니다.
