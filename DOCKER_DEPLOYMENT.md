# Docker 部署指南

## 📦 项目简介

Music Web GD 是一个基于 Vue 3 的在线音乐播放器项目，本文档介绍如何使用 Docker 部署该项目。

## 🚀 快速开始

### 方式一：从 GitHub Container Registry 拉取镜像

```bash
# 拉取最新版本
docker pull ghcr.io/fistbaozi/music-web-gd:latest

# 运行容器
docker run -d \
  --name music-web-gd \
  -p 80:80 \
  ghcr.io/fistbaozi/music-web-gd:latest
```

### 方式二：本地构建镜像

```bash
# 克隆项目
git clone https://github.com/FistBaoZi/music-web-gd.git
cd music-web-gd

# 构建镜像
docker build -t music-web-gd:latest .

# 运行容器
docker run -d \
  --name music-web-gd \
  -p 80:80 \
  music-web-gd:latest
```

### 方式三：使用 Docker Compose

```bash
# 启动服务
docker-compose up -d

# 查看日志
docker-compose logs -f

# 停止服务
docker-compose down
```

## 🔧 高级配置

### 自定义端口

```bash
# 使用 8080 端口
docker run -d \
  --name music-web-gd \
  -p 8080:80 \
  ghcr.io/fistbaozi/music-web-gd:latest
```

### 设置时区

```bash
docker run -d \
  --name music-web-gd \
  -p 80:80 \
  -e TZ=Asia/Shanghai \
  ghcr.io/fistbaozi/music-web-gd:latest
```

### 持久化配置（如需要）

```bash
docker run -d \
  --name music-web-gd \
  -p 80:80 \
  -v /path/to/config:/etc/nginx/conf.d \
  ghcr.io/fistbaozi/music-web-gd:latest
```

## 📝 Docker Compose 完整配置

创建 `docker-compose.yml` 文件：

```yaml
version: '3.8'

services:
  music-web-gd:
    image: ghcr.io/fistbaozi/music-web-gd:latest
    container_name: music-web-gd
    ports:
      - "80:80"
    restart: unless-stopped
    environment:
      - TZ=Asia/Shanghai
    healthcheck:
      test: ["CMD", "wget", "--quiet", "--tries=1", "--spider", "http://localhost/"]
      interval: 30s
      timeout: 3s
      retries: 3
      start_period: 5s
```

## 🔨 本地构建

### 构建单架构镜像

```bash
# AMD64 架构
docker build --platform linux/amd64 -t music-web-gd:amd64 .

# ARM64 架构
docker build --platform linux/arm64 -t music-web-gd:arm64 .
```

### 构建多架构镜像

```bash
# 创建 buildx 构建器
docker buildx create --name mybuilder --use
docker buildx inspect --bootstrap

# 构建并推送多架构镜像
docker buildx build \
  --platform linux/amd64,linux/arm64 \
  -t ghcr.io/fistbaozi/music-web-gd:latest \
  --push .
```

## 🎯 GitHub Actions 工作流

项目已配置自动化构建工作流，位于 `.github/workflows/docker-publish.yml`。

### 触发构建

1. 前往 GitHub 仓库的 Actions 页面
2. 选择 "Build and Push Docker Image" 工作流
3. 点击 "Run workflow"
4. 输入版本号（例如：v1.0.0）
5. 点击 "Run workflow" 按钮

### 工作流功能

- ✅ 自动安装依赖并构建项目
- ✅ 构建多架构 Docker 镜像（amd64/arm64）
- ✅ 推送到 GitHub Container Registry
- ✅ 生成版本标签和 latest 标签
- ✅ 生成部署说明文档

## 📊 容器管理

### 查看容器状态

```bash
# 查看运行中的容器
docker ps

# 查看容器日志
docker logs music-web-gd

# 实时查看日志
docker logs -f music-web-gd
```

### 容器操作

```bash
# 停止容器
docker stop music-web-gd

# 启动容器
docker start music-web-gd

# 重启容器
docker restart music-web-gd

# 删除容器
docker rm -f music-web-gd
```

### 镜像管理

```bash
# 查看镜像
docker images

# 删除镜像
docker rmi ghcr.io/fistbaozi/music-web-gd:latest

# 清理未使用的镜像
docker image prune -a
```

## 🔍 健康检查

容器内置健康检查功能，每 30 秒检查一次服务状态：

```bash
# 查看健康状态
docker inspect --format='{{.State.Health.Status}}' music-web-gd
```

## 🌐 访问应用

部署成功后，通过浏览器访问：

- 本地访问：http://localhost
- 局域网访问：http://你的IP地址
- 自定义端口：http://localhost:8080

## 🛡️ 安全建议

1. **使用 HTTPS**：在生产环境中建议使用反向代理（如 Nginx、Caddy）配置 SSL 证书
2. **限制访问**：配置防火墙规则限制访问
3. **定期更新**：及时更新镜像到最新版本
4. **监控日志**：定期查看容器日志

## 🔧 反向代理配置（可选）

### Nginx 反向代理

```nginx
server {
    listen 80;
    server_name your-domain.com;
    
    location / {
        proxy_pass http://localhost:80;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

### Caddy 反向代理

```
your-domain.com {
    reverse_proxy localhost:80
}
```

## 📈 性能优化

镜像已包含以下优化：

- ✅ 多阶段构建，减小镜像体积
- ✅ 基于 Alpine Linux，体积更小
- ✅ 启用 Gzip 压缩
- ✅ 静态资源缓存策略
- ✅ 健康检查机制

## 🐛 故障排查

### 容器无法启动

```bash
# 查看容器日志
docker logs music-web-gd

# 查看详细信息
docker inspect music-web-gd
```

### 端口冲突

```bash
# 检查端口占用
netstat -ano | findstr :80

# 使用其他端口
docker run -d --name music-web-gd -p 8080:80 ghcr.io/fistbaozi/music-web-gd:latest
```

### 网络问题

```bash
# 检查容器网络
docker network inspect bridge

# 进入容器调试
docker exec -it music-web-gd sh
```

## 📚 相关文件说明

- `Dockerfile` - Docker 镜像构建文件
- `docker-compose.yml` - Docker Compose 配置文件
- `.dockerignore` - Docker 构建忽略文件
- `nginx.conf` - Nginx 配置文件
- `.github/workflows/docker-publish.yml` - GitHub Actions 工作流

## 🤝 贡献

欢迎提交 Issue 和 Pull Request！

## 📄 许可证

本项目遵循相应的开源许可证。
