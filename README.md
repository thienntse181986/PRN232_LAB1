# PRN232 LMS - Learning Management System API

**Student:** Nguyen Trong Thien | **ID:** SE181986  
**Course:** PRN232 - Advanced Cross-Platform Application Programming

## 📦 Technology Stack
- ASP.NET Core 8 Web API
- Entity Framework Core 8
- SQL Server 2022
- Swagger / OpenAPI (Swashbuckle)
- Docker + Docker Compose

## 🏗 Project Structure

```
LAB1_NguyenTrongThien_SE181986/
├── PRN232.LMS.sln
├── Dockerfile
├── docker-compose.yml
├── PRN232.LMS.API/                    ← Controller Layer
│   ├── Controllers/
│   │   ├── StudentsController.cs
│   │   ├── CoursesController.cs
│   │   ├── SubjectsController.cs
│   │   ├── SemestersController.cs
│   │   └── EnrollmentsController.cs
│   ├── Program.cs
│   ├── appsettings.json
│   └── appsettings.Development.json
├── PRN232.LMS.Services/               ← Service Layer
│   ├── Interfaces/
│   │   └── IServiceInterfaces.cs
│   ├── Implementations/
│   │   ├── StudentService.cs
│   │   ├── CourseService.cs
│   │   ├── SubjectService.cs
│   │   ├── SemesterService.cs
│   │   └── EnrollmentService.cs
│   ├── Helpers/
│   │   └── QueryHelper.cs
│   └── Models/
│       ├── Request/
│       │   ├── EntityRequests.cs
│       │   └── QueryParameters.cs
│       └── Response/
│           ├── ApiResponse.cs
│           └── EntityResponses.cs
└── PRN232.LMS.Repositories/           ← Repository Layer
    ├── Entities/
    │   ├── Semester.cs
    │   ├── Course.cs
    │   ├── Subject.cs
    │   ├── Student.cs
    │   └── Enrollment.cs
    ├── Context/
    │   ├── LmsDbContext.cs
    │   └── LmsDbContextFactory.cs
    ├── Interfaces/
    │   ├── IGenericRepository.cs
    │   ├── IStudentRepository.cs
    │   ├── ICourseRepository.cs
    │   ├── ISubjectRepository.cs
    │   ├── ISemesterRepository.cs
    │   └── IEnrollmentRepository.cs
    ├── Implementations/
    │   ├── GenericRepository.cs
    │   ├── StudentRepository.cs
    │   ├── CourseRepository.cs
    │   ├── SubjectRepository.cs
    │   ├── SemesterRepository.cs
    │   └── EnrollmentRepository.cs
    └── Migrations/
```

## 🚀 Cách chạy project

### Option 1: Docker Compose (Chạy API qua Docker, dùng SQL Server của máy thật)

> [!NOTE]
> Cấu hình Docker đã được tùy biến để tránh xung đột cổng `1433` với SQL Server local trên máy của bạn. API container sẽ kết nối trực tiếp đến SQL Server trên Windows thông qua `host.docker.internal`.

```bash
# Chạy API container
docker compose up -d

# Truy cập Swagger UI
http://localhost:8080
```

### Option 2: Local Development (Chạy trực tiếp trên Windows)

**Bước 1:** Đảm bảo SQL Server cục bộ đang chạy với cấu hình:
- Server name: `LAPTOP-IOT1D9JU`
- User: `sa`  
- Password: `1234567890`

**Bước 2:** Chạy migrations (tự động khi khởi động ứng dụng hoặc chạy thủ công):
```bash
# Chạy thủ công nếu cần
dotnet ef database update --project PRN232.LMS.Repositories --startup-project PRN232.LMS.API
```

**Bước 3:** Khởi chạy API trực tiếp:
```bash
cd PRN232.LMS.API
dotnet run
```

**Bước 4:** Mở Swagger UI:
```
http://localhost:5000  (hoặc cổng được hiển thị trong terminal)
```

## 📡 API Endpoints

### Students
| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | `/api/students` | Lấy danh sách (search, sort, page, expand) |
| GET | `/api/students/{id}` | Lấy theo ID + enrollments |
| POST | `/api/students` | Tạo mới |
| PUT | `/api/students/{id}` | Cập nhật |
| DELETE | `/api/students/{id}` | Xóa |

### Courses, Subjects, Semesters, Enrollments
Tương tự với route `/api/courses`, `/api/subjects`, `/api/semesters`, `/api/enrollments`

## 🔍 Query Parameters

| Parameter | Ví dụ | Mô tả |
|-----------|-------|-------|
| `search` | `?search=nguyen` | Tìm kiếm theo keyword |
| `sort` | `?sort=fullName,-dateOfBirth` | Sắp xếp (prefix `-` = descending) |
| `page` | `?page=2` | Số trang (mặc định: 1) |
| `size` | `?size=10` | Kích thước trang (max: 100) |
| `expand` | `?expand=student,course` | Load dữ liệu quan hệ |

## 📋 Response Format

```json
{
  "success": true,
  "message": "Request processed successfully",
  "data": { },
  "pagination": {
    "page": 1,
    "pageSize": 10,
    "totalItems": 500,
    "totalPages": 50
  },
  "errors": null
}
```

## 🗄 Database

- **5** Semesters
- **10** Subjects  
- **20** Courses (4 per semester)
- **50** Students
- **500** Enrollments (10 per student)

## Docker Commands

```bash
# Xem logs
docker compose logs -f api

# Dừng
docker compose down

# Dừng và xóa volumes
docker compose down -v

# Rebuild
docker compose up --build
```

## EF Migrations

```bash
# Tạo migration mới
dotnet ef migrations add <MigrationName> --project PRN232.LMS.Repositories --startup-project PRN232.LMS.API

# Apply migration
dotnet ef database update --project PRN232.LMS.Repositories --startup-project PRN232.LMS.API

# Rollback
dotnet ef database update <PreviousMigrationName> --project PRN232.LMS.Repositories --startup-project PRN232.LMS.API
```
