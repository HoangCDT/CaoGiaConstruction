# Hướng dẫn Deploy với GitHub Actions

## Bước 1: Tạo GitHub Repository

1. Đăng nhập vào GitHub
2. Click vào **New repository** hoặc truy cập: https://github.com/new
3. Điền thông tin:
   - **Repository name**: `CaoGiaConstruction.WebClient`
   - **Description**: Web application for Cao Gia Construction
   - **Visibility**: Private hoặc Public (tùy chọn)
   - **Initialize**: Không tích vào bất kỳ option nào
4. Click **Create repository**

## Bước 2: Push code lên GitHub

```bash
# Khởi tạo git (nếu chưa có)
git init

# Thêm remote repository
git remote add origin https://github.com/YOUR_USERNAME/CaoGiaConstruction.WebClient.git

# Thêm tất cả files
git add .

# Commit
git commit -m "Initial commit"

# Push lên GitHub
git branch -M main
git push -u origin main
```

## Bước 3: Cấu hình GitHub Secrets

1. Vào repository trên GitHub
2. Click **Settings** → **Secrets and variables** → **Actions**
3. Click **New repository secret** và thêm các secrets sau:

### Docker Hub Secrets

- **Name**: `DOCKERHUB_USERNAME`
  - **Value**: Tên đăng nhập Docker Hub của bạn (ví dụ: `hoangcdt`)

- **Name**: `DOCKERHUB_TOKEN`
  - **Value**: Docker Hub Access Token (khuyến nghị dùng token thay vì password)

### SSH Deployment Secrets cho UAT

- **Name**: `SSH_HOST_UAT`
  - **Value**: Địa chỉ IP hoặc domain của server UAT (ví dụ: `192.168.1.100` hoặc `uat.example.com`)

- **Name**: `SSH_USERNAME_UAT`
  - **Value**: Tên đăng nhập SSH cho UAT (ví dụ: `root` hoặc `deploy`)

- **Name**: `SSH_KEY_UAT`
  - **Value**: Nội dung private key SSH cho UAT (toàn bộ nội dung file `~/.ssh/id_rsa`)

- **Name**: `SSH_PORT_UAT`
  - **Value**: Port SSH cho UAT (mặc định: `22`)

### SSH Deployment Secrets cho Production

- **Name**: `SSH_HOST_PROD`
  - **Value**: Địa chỉ IP hoặc domain của server Production

- **Name**: `SSH_USERNAME_PROD`
  - **Value**: Tên đăng nhập SSH cho Production

- **Name**: `SSH_KEY_PROD`
  - **Value**: Nội dung private key SSH cho Production

- **Name**: `SSH_PORT_PROD`
  - **Value**: Port SSH cho Production (mặc định: `22`)

## Bước 4: Tạo Docker Hub Access Token (Khuyến nghị)

Thay vì dùng mật khẩu, nên tạo Access Token:

1. Đăng nhập Docker Hub
2. Vào **Account Settings** → **Security** → **New Access Token**
3. Đặt tên token (ví dụ: `github-actions`)
4. Copy token và dùng làm `DOCKER_PASSWORD` secret

## Bước 5: Cấu hình Server (Nếu deploy tự động)

### Trên server, tạo SSH key:

```bash
ssh-keygen -t rsa -b 4096 -C "github-actions"
# Không đặt passphrase để tự động deploy
```

### Copy public key vào authorized_keys:

```bash
cat ~/.ssh/id_rsa.pub >> ~/.ssh/authorized_keys
chmod 600 ~/.ssh/authorized_keys
```

### Tạo deployment script trên server:

```bash
mkdir -p /path/to/deployment
cd /path/to/deployment

# Tạo docker-compose.yml (copy từ project)
# Cập nhật đường dẫn trong workflow nếu cần
```

## Bước 6: Test CI/CD

1. Push code lên branch `main` hoặc `develop`
2. Vào **Actions** tab trên GitHub để xem workflow chạy
3. Kiểm tra logs nếu có lỗi

## Workflow Triggers

- **Push to master**: Tự động build và deploy lên UAT
- **Create tag**: Tự động build và deploy lên Production
- **Pull Request**: Chỉ build và test, không deploy

## Flow Deploy

### UAT Environment
- Khi push code lên branch `master`
- Build Docker image với tag `latest`
- Deploy tự động lên server UAT
- Image: `hoangcdt7602119/cao-gia-construction:latest`

### Production Environment
- Khi tạo tag (ví dụ: `v1.0.0`, `v1.2.3`)
- Build Docker image với tag là tên tag
- Deploy tự động lên server Production
- Image: `hoangcdt7602119/cao-gia-construction:v1.0.0`

### Tạo tag để deploy Production

```bash
# Tạo tag
git tag v1.0.0

# Push tag lên GitHub
git push origin v1.0.0
```

## Troubleshooting

### Lỗi Docker login
- Kiểm tra `DOCKERHUB_USERNAME` và `DOCKERHUB_TOKEN` secrets
- Đảm bảo token có quyền push images
- Token phải có quyền "Read & Write" hoặc "Read, Write & Delete"

### Lỗi SSH connection
- Kiểm tra `SSH_HOST_UAT`, `SSH_USERNAME_UAT`, `SSH_KEY_UAT` cho UAT
- Kiểm tra `SSH_HOST_PROD`, `SSH_USERNAME_PROD`, `SSH_KEY_PROD` cho Production
- Test SSH connection thủ công: `ssh -i key_file -p port username@host`
- Đảm bảo SSH key không có passphrase

### Lỗi build
- Kiểm tra logs trong Actions tab
- Đảm bảo .NET SDK version đúng
- Kiểm tra Dockerfile path

## Manual Deployment

Nếu không muốn deploy tự động, có thể deploy thủ công:

```bash
# SSH vào server
ssh user@server

# Pull latest image
docker pull ngocsonit95/cao-gia-construction:latest

# Update docker-compose
cd /path/to/deployment
export IMAGE_TAG=latest
docker-compose pull
docker-compose up -d
```

