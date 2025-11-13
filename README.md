# Post Management - Full Stack Application

## Tổng quan dự án
Ứng dụng quản lý bài viết (Post Management) với:
- **Backend**: ASP.NET Core Web API (.NET 9) + PostgreSQL + Cloudinary
- **Frontend**: React 18 + Vite + TailwindCSS + Axios

## Cấu trúc dự án
```
PE_PRN_Test/
├── backend/                    # ASP.NET Core Web API
│   └── PostManagementAPI/
│       ├── Controllers/        # REST API endpoints
│       ├── Models/            # Entity models (Post)
│       ├── DTOs/              # Data transfer objects
│       ├── Data/              # DbContext
│       ├── Services/          # Image upload, seeding
│       ├── Configuration/     # Cloudinary, Swagger
│       └── Migrations/        # EF Core migrations
└── frontend/                  # React application
    ├── src/
    │   ├── components/        # PostCard, PostForm, ConfirmModal
    │   ├── pages/            # ListPage, CreatePage, EditPage
    │   └── services/         # API service (Axios)
    ├── package.json
    ├── vite.config.js
    └── tailwind.config.js
```

## Chức năng chính

### Backend API
- ✅ CRUD operations (Create, Read, Update, Delete)
- ✅ Search posts: `GET /api/posts?q=keyword`
- ✅ Sort posts: `GET /api/posts?sort=name_asc|name_desc`
- ✅ Pagination: `GET /api/posts?page=1&pageSize=10`
- ✅ Image upload (Cloudinary + multipart/form-data)
- ✅ Database seeding
- ✅ Swagger documentation

### Frontend
- ✅ Danh sách bài viết với search & sort
- ✅ Tạo bài viết mới (upload ảnh hoặc URL)
- ✅ Sửa bài viết
- ✅ Xóa bài viết (có confirm modal)
- ✅ Responsive design (Tailwind)

## Cài đặt và chạy

### 1. Backend

```powershell
# Di chuyển vào thư mục backend
cd backend\PostManagementAPI

# Cài đặt dependencies (nếu cần)
dotnet restore

# Cấu hình database (appsettings.json)
# Thay đổi connection string PostgreSQL của bạn

# Chạy migrations
dotnet ef database update

# (Tùy chọn) Seed dữ liệu mẫu
dotnet run --seed

# Chạy API
dotnet run
```

Backend sẽ chạy tại: `https://localhost:7XXX` (port tự động)
Swagger: `https://localhost:7XXX/swagger`

### 2. Frontend

```powershell
# Di chuyển vào thư mục frontend
cd frontend

# Cài đặt dependencies (đã cài rồi)
npm install

# Tạo file .env
# Copy từ .env.example và cấu hình
cp .env.example .env

# Sửa VITE_API_URL trong .env thành URL backend của bạn
# Ví dụ: VITE_API_URL=https://localhost:7001

# Chạy dev server
npm run dev
```

Frontend sẽ chạy tại: `http://localhost:5173`

## Cấu hình

### Backend (appsettings.json)
```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Database=postmanagement;Username=postgres;Password=yourpassword"
  },
  "CloudinarySettings": {
    "CloudName": "your-cloud-name",
    "ApiKey": "your-api-key",
    "ApiSecret": "your-api-secret"
  }
}
```

### Frontend (.env)
```
VITE_API_URL=https://localhost:7001
```

## Testing

### Backend Tests
```powershell
cd backend\PostManagementAPI.Tests
dotnet test
```

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /api/posts | Lấy danh sách posts |
| GET | /api/posts/{id} | Lấy post theo ID |
| POST | /api/posts | Tạo post mới |
| PUT | /api/posts/{id} | Cập nhật post |
| DELETE | /api/posts/{id} | Xóa post |

### Query Parameters
- `q` - Search keyword
- `sort` - Sort order (name_asc, name_desc)
- `page` - Page number (default: 1)
- `pageSize` - Items per page (default: 10)

## Technologies

### Backend
- .NET 9.0
- ASP.NET Core Web API
- Entity Framework Core 9.0.10
- PostgreSQL (Npgsql 9.0.4)
- Cloudinary SDK 1.27.8
- Swashbuckle (Swagger) 9.0.6
- xUnit 2.9.2

### Frontend
- React 18.3.1
- Vite 5.3.1
- TailwindCSS 3.4.4
- Axios 1.7.2
- React Router DOM 6.26.0

## Ghi chú
- Backend hỗ trợ CORS cho `http://localhost:5173` và `http://localhost:3000`
- Cloudinary không bắt buộc - nếu không cấu hình sẽ dùng placeholder URLs
- File upload tối đa 5MB, hỗ trợ: JPEG, PNG, GIF, WebP
