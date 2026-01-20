# Cao Gia Construction Web Client

Ứng dụng web quản lý cho Cao Gia Construction được xây dựng bằng ASP.NET Core 8.0 và PostgreSQL.

## 🚀 Tính năng

- Quản lý sản phẩm, dự án, dịch vụ
- Quản lý blog và tin tức
- Quản lý chi nhánh
- Quản lý người dùng và phân quyền
- Hệ thống SEO và meta tags
- Upload và quản lý file

## 📋 Yêu cầu

- .NET 8.0 SDK
- PostgreSQL 12+
- Docker (tùy chọn)

## 🛠️ Cài đặt

### 1. Clone repository

```bash
git clone https://github.com/your-username/CaoGiaConstruction.WebClient.git
cd CaoGiaConstruction.WebClient
```

### 2. Cấu hình database

Cập nhật connection string trong `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "Default": "Host=postgres.hqsolutions.vn;Port=5432;Database=CAOGIACONSTRUCTION_DEV;User ID=hqs;Password=your_password;"
  }
}
```

### 3. Chạy migrations

```bash
cd CaoGiaConstruction.WebClient
dotnet ef database update
```

### 4. Chạy ứng dụng

```bash
dotnet run --project CaoGiaConstruction.WebClient/CaoGiaConstruction.WebClient.csproj
```

Ứng dụng sẽ chạy tại `https://localhost:7244` hoặc `http://localhost:5244`

## 🐳 Docker

### Build image

```bash
docker build -t cao-gia-construction -f CaoGiaConstruction.WebClient/Dockerfile .
```

### Chạy với Docker Compose

```bash
docker-compose up -d
```

## 🔐 Thông tin đăng nhập mặc định

- **Username**: `admin`
- **Password**: `admin@123`

⚠️ **Lưu ý**: Đổi mật khẩu ngay sau lần đăng nhập đầu tiên!

## 🔄 CI/CD với GitHub Actions

Dự án sử dụng GitHub Actions để tự động build và deploy theo flow:

### Workflow Flow

1. **Build Job**: Build và test ứng dụng .NET
2. **Deploy UAT**: Khi push lên branch `master` → Deploy lên UAT
3. **Deploy Production**: Khi tạo tag (ví dụ: `v1.0.0`) → Deploy lên Production

### Workflows

1. **CI/CD Pipeline** (`.github/workflows/ci-cd.yml`)
   - **Build**: Build và test khi push code
   - **Deploy UAT**: Tự động deploy khi push lên `master`
   - **Deploy Production**: Tự động deploy khi tạo tag

2. **Docker Build Only** (`.github/workflows/docker-build.yml`)
   - Build Docker image thủ công với tag tùy chỉnh

### Cấu hình Secrets

Thêm các secrets sau vào GitHub repository settings:

#### Docker Hub
- `DOCKERHUB_USERNAME`: Tên đăng nhập Docker Hub (ví dụ: `hoangcdt`)
- `DOCKERHUB_TOKEN`: Docker Hub Access Token

#### SSH UAT
- `SSH_HOST_UAT`: Địa chỉ server UAT
- `SSH_USERNAME_UAT`: Tên đăng nhập SSH UAT
- `SSH_KEY_UAT`: Private key SSH cho UAT
- `SSH_PORT_UAT`: Port SSH UAT (mặc định: 22)

#### SSH Production
- `SSH_HOST_PROD`: Địa chỉ server Production
- `SSH_USERNAME_PROD`: Tên đăng nhập SSH Production
- `SSH_KEY_PROD`: Private key SSH cho Production
- `SSH_PORT_PROD`: Port SSH Production (mặc định: 22)

### Cách Deploy

#### Deploy UAT
```bash
# Push code lên branch master
git checkout master
git push origin master
```

#### Deploy Production
```bash
# Tạo tag và push
git tag v1.0.0
git push origin v1.0.0
```

Xem chi tiết trong [DEPLOYMENT.md](.github/DEPLOYMENT.md)

## 📁 Cấu trúc dự án

```
CaoGiaConstruction.WebClient/
├── CaoGiaConstruction.WebClient/     # Main web application
│   ├── Areas/                        # Admin area
│   ├── Controllers/                  # MVC Controllers
│   ├── Services/                     # Business logic
│   ├── Context/                      # Database context
│   ├── Migrations/                   # EF Core migrations
│   └── Views/                        # Razor views
├── CaoGiaConstruction.Utilities/    # Shared utilities
└── docker-compose.yml                # Docker Compose config
```

## 🧪 Testing

```bash
dotnet test
```

## 📝 License

Copyright © 2024 Cao Gia Construction. All rights reserved.

## 👥 Contributors

- Development Team

## 📞 Liên hệ

- Website: https://caogiaconstruction.vn
- Email: support@caogiaconstruction.vn

