# 🐳 Docker Deployment Guide

## Prerequisites
- Docker Desktop installed
- Docker Compose installed

## Quick Start

### 1. Build and Run All Services
```bash
docker-compose up -d
```

### 2. Access the Application
- **Frontend**: http://localhost
- **Backend API**: http://localhost:5201
- **Swagger**: http://localhost:5201/swagger
- **PostgreSQL**: localhost:5432

### 3. Stop All Services
```bash
docker-compose down
```

### 4. Stop and Remove Volumes (Clean Reset)
```bash
docker-compose down -v
```

## Individual Service Commands

### Backend Only
```bash
cd backend/PostManagementAPI
docker build -t postmanagement-api .
docker run -p 5201:5201 postmanagement-api
```

### Frontend Only
```bash
cd frontend
docker build -t postmanagement-frontend .
docker run -p 80:80 postmanagement-frontend
```

## Database Migration

Run migrations after starting containers:
```bash
docker-compose exec backend dotnet ef database update
```

## View Logs
```bash
# All services
docker-compose logs -f

# Specific service
docker-compose logs -f backend
docker-compose logs -f frontend
docker-compose logs -f postgres
```

## Environment Variables

Edit `docker-compose.yml` to change:
- Database credentials
- API URLs
- Port mappings

## Production Deployment

For production, update:
1. Change PostgreSQL password in `docker-compose.yml`
2. Set `ASPNETCORE_ENVIRONMENT=Production`
3. Update frontend API URL in `.env.production`
4. Add SSL certificates for HTTPS

## Troubleshooting

### Port Already in Use
```bash
# Check what's using the port
netstat -ano | findstr :5201
netstat -ano | findstr :80

# Stop the process or change port in docker-compose.yml
```

### Database Connection Failed
```bash
# Check if PostgreSQL is running
docker-compose ps postgres

# Check logs
docker-compose logs postgres
```

### Frontend Can't Connect to Backend
- Ensure backend is running: `docker-compose ps backend`
- Check API URL in frontend `.env.production`
- Verify CORS settings in backend
