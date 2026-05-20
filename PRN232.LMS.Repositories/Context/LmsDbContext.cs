using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.Repositories.Context;

public class LmsDbContext : DbContext
{
    public LmsDbContext(DbContextOptions<LmsDbContext> options) : base(options) { }

    public DbSet<Semester> Semesters { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Semester>(entity =>
        {
            entity.HasKey(e => e.SemesterId);
            entity.Property(e => e.SemesterName).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.CourseId);
            entity.Property(e => e.CourseName).HasMaxLength(100).IsRequired();
            entity.HasOne(e => e.Semester)
                  .WithMany(s => s.Courses)
                  .HasForeignKey(e => e.SemesterId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.SubjectId);
            entity.Property(e => e.SubjectCode).HasMaxLength(20).IsRequired();
            entity.Property(e => e.SubjectName).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId);
            entity.Property(e => e.FullName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(100).IsRequired();
            entity.HasIndex(e => e.Email).IsUnique();
        });

        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.HasKey(e => e.EnrollmentId);
            entity.Property(e => e.Status).HasMaxLength(20).IsRequired();
            entity.HasOne(e => e.Student)
                  .WithMany(s => s.Enrollments)
                  .HasForeignKey(e => e.StudentId)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Course)
                  .WithMany(c => c.Enrollments)
                  .HasForeignKey(e => e.CourseId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        // ── Semesters ──────────────────────────────────────────────────
        modelBuilder.Entity<Semester>().HasData(
            new Semester { SemesterId = 1, SemesterName = "Fall 2023",   StartDate = new DateTime(2023,9,1),  EndDate = new DateTime(2024,1,15) },
            new Semester { SemesterId = 2, SemesterName = "Spring 2024", StartDate = new DateTime(2024,2,1),  EndDate = new DateTime(2024,6,15) },
            new Semester { SemesterId = 3, SemesterName = "Summer 2024", StartDate = new DateTime(2024,6,20), EndDate = new DateTime(2024,9,15) },
            new Semester { SemesterId = 4, SemesterName = "Fall 2024",   StartDate = new DateTime(2024,9,1),  EndDate = new DateTime(2025,1,15) },
            new Semester { SemesterId = 5, SemesterName = "Spring 2025", StartDate = new DateTime(2025,2,1),  EndDate = new DateTime(2025,6,15) }
        );

        // ── Subjects ───────────────────────────────────────────────────
        modelBuilder.Entity<Subject>().HasData(
            new Subject { SubjectId = 1,  SubjectCode = "PRN211", SubjectName = "Basic Cross-Platform Application Programming",     Credit = 3 },
            new Subject { SubjectId = 2,  SubjectCode = "PRN212", SubjectName = "C# Programming and .NET Framework",               Credit = 3 },
            new Subject { SubjectId = 3,  SubjectCode = "PRN231", SubjectName = "Apply Object-Oriented Programming",               Credit = 3 },
            new Subject { SubjectId = 4,  SubjectCode = "PRN232", SubjectName = "Advanced Cross-Platform Application Programming", Credit = 3 },
            new Subject { SubjectId = 5,  SubjectCode = "PRJ301", SubjectName = "Java Web Application Development",               Credit = 3 },
            new Subject { SubjectId = 6,  SubjectCode = "DBI202", SubjectName = "Introduction to Database",                       Credit = 3 },
            new Subject { SubjectId = 7,  SubjectCode = "MAE101", SubjectName = "Mathematics for Engineering",                    Credit = 3 },
            new Subject { SubjectId = 8,  SubjectCode = "CEA201", SubjectName = "Computer Organization and Architecture",         Credit = 3 },
            new Subject { SubjectId = 9,  SubjectCode = "SSG104", SubjectName = "Communication and In-Group Working Skills",      Credit = 3 },
            new Subject { SubjectId = 10, SubjectCode = "ENW492", SubjectName = "English for IT Professionals",                   Credit = 3 }
        );

        // ── Courses (20 courses, 4 per semester) ───────────────────────
        modelBuilder.Entity<Course>().HasData(
            new Course { CourseId = 1,  CourseName = "PRN211 - Fall 2023",   SemesterId = 1 },
            new Course { CourseId = 2,  CourseName = "PRN212 - Fall 2023",   SemesterId = 1 },
            new Course { CourseId = 3,  CourseName = "DBI202 - Fall 2023",   SemesterId = 1 },
            new Course { CourseId = 4,  CourseName = "MAE101 - Fall 2023",   SemesterId = 1 },
            new Course { CourseId = 5,  CourseName = "PRN231 - Spring 2024", SemesterId = 2 },
            new Course { CourseId = 6,  CourseName = "PRN212 - Spring 2024", SemesterId = 2 },
            new Course { CourseId = 7,  CourseName = "DBI202 - Spring 2024", SemesterId = 2 },
            new Course { CourseId = 8,  CourseName = "SSG104 - Spring 2024", SemesterId = 2 },
            new Course { CourseId = 9,  CourseName = "PRN232 - Summer 2024", SemesterId = 3 },
            new Course { CourseId = 10, CourseName = "PRJ301 - Summer 2024", SemesterId = 3 },
            new Course { CourseId = 11, CourseName = "ENW492 - Summer 2024", SemesterId = 3 },
            new Course { CourseId = 12, CourseName = "CEA201 - Summer 2024", SemesterId = 3 },
            new Course { CourseId = 13, CourseName = "PRN232 - Fall 2024",   SemesterId = 4 },
            new Course { CourseId = 14, CourseName = "PRJ301 - Fall 2024",   SemesterId = 4 },
            new Course { CourseId = 15, CourseName = "MAE101 - Fall 2024",   SemesterId = 4 },
            new Course { CourseId = 16, CourseName = "SSG104 - Fall 2024",   SemesterId = 4 },
            new Course { CourseId = 17, CourseName = "PRN232 - Spring 2025", SemesterId = 5 },
            new Course { CourseId = 18, CourseName = "ENW492 - Spring 2025", SemesterId = 5 },
            new Course { CourseId = 19, CourseName = "DBI202 - Spring 2025", SemesterId = 5 },
            new Course { CourseId = 20, CourseName = "CEA201 - Spring 2025", SemesterId = 5 }
        );

        // ── Students (50 students) ─────────────────────────────────────
        modelBuilder.Entity<Student>().HasData(
            new Student { StudentId =  1, FullName = "Nguyen An",        Email = "SE181901@fpt.edu.vn", DateOfBirth = new DateTime(2001,3,15)  },
            new Student { StudentId =  2, FullName = "Tran Binh",        Email = "SE181902@fpt.edu.vn", DateOfBirth = new DateTime(2001,7,22)  },
            new Student { StudentId =  3, FullName = "Le Cuong",         Email = "SE181903@fpt.edu.vn", DateOfBirth = new DateTime(2002,1,10)  },
            new Student { StudentId =  4, FullName = "Pham Dung",        Email = "SE181904@fpt.edu.vn", DateOfBirth = new DateTime(2001,5,18)  },
            new Student { StudentId =  5, FullName = "Hoang Em",         Email = "SE181905@fpt.edu.vn", DateOfBirth = new DateTime(2002,9,4)   },
            new Student { StudentId =  6, FullName = "Ngo Hai",          Email = "SE181906@fpt.edu.vn", DateOfBirth = new DateTime(2001,11,27) },
            new Student { StudentId =  7, FullName = "Do Khanh",         Email = "SE181907@fpt.edu.vn", DateOfBirth = new DateTime(2002,6,3)   },
            new Student { StudentId =  8, FullName = "Ly Lan",           Email = "SE181908@fpt.edu.vn", DateOfBirth = new DateTime(2001,2,14)  },
            new Student { StudentId =  9, FullName = "Dang Mai",         Email = "SE181909@fpt.edu.vn", DateOfBirth = new DateTime(2002,8,20)  },
            new Student { StudentId = 10, FullName = "Bui Nam",          Email = "SE181910@fpt.edu.vn", DateOfBirth = new DateTime(2001,4,9)   },
            new Student { StudentId = 11, FullName = "Nguyen Oanh",      Email = "SE181911@fpt.edu.vn", DateOfBirth = new DateTime(2002,12,5)  },
            new Student { StudentId = 12, FullName = "Tran Phuong",      Email = "SE181912@fpt.edu.vn", DateOfBirth = new DateTime(2001,6,30)  },
            new Student { StudentId = 13, FullName = "Le Quang",         Email = "SE181913@fpt.edu.vn", DateOfBirth = new DateTime(2002,3,17)  },
            new Student { StudentId = 14, FullName = "Pham Son",         Email = "SE181914@fpt.edu.vn", DateOfBirth = new DateTime(2001,10,8)  },
            new Student { StudentId = 15, FullName = "Hoang Tam",        Email = "SE181915@fpt.edu.vn", DateOfBirth = new DateTime(2002,5,25)  },
            new Student { StudentId = 16, FullName = "Ngo Uyen",         Email = "SE181916@fpt.edu.vn", DateOfBirth = new DateTime(2001,1,12)  },
            new Student { StudentId = 17, FullName = "Do Van",           Email = "SE181917@fpt.edu.vn", DateOfBirth = new DateTime(2002,7,11)  },
            new Student { StudentId = 18, FullName = "Ly Xuan",          Email = "SE181918@fpt.edu.vn", DateOfBirth = new DateTime(2001,9,23)  },
            new Student { StudentId = 19, FullName = "Dang Yen",         Email = "SE181919@fpt.edu.vn", DateOfBirth = new DateTime(2002,2,7)   },
            new Student { StudentId = 20, FullName = "Bui Zung",         Email = "SE181920@fpt.edu.vn", DateOfBirth = new DateTime(2001,8,16)  },
            new Student { StudentId = 21, FullName = "Nguyen Anh",       Email = "SE181921@fpt.edu.vn", DateOfBirth = new DateTime(2003,4,1)   },
            new Student { StudentId = 22, FullName = "Tran Bach",        Email = "SE181922@fpt.edu.vn", DateOfBirth = new DateTime(2002,11,19) },
            new Student { StudentId = 23, FullName = "Le Chi",           Email = "SE181923@fpt.edu.vn", DateOfBirth = new DateTime(2003,1,28)  },
            new Student { StudentId = 24, FullName = "Pham Dat",         Email = "SE181924@fpt.edu.vn", DateOfBirth = new DateTime(2002,6,14)  },
            new Student { StudentId = 25, FullName = "Hoang Duc",        Email = "SE181925@fpt.edu.vn", DateOfBirth = new DateTime(2003,9,6)   },
            new Student { StudentId = 26, FullName = "Ngo Giang",        Email = "SE181926@fpt.edu.vn", DateOfBirth = new DateTime(2002,3,21)  },
            new Student { StudentId = 27, FullName = "Do Hoa",           Email = "SE181927@fpt.edu.vn", DateOfBirth = new DateTime(2003,7,13)  },
            new Student { StudentId = 28, FullName = "Ly Hung",          Email = "SE181928@fpt.edu.vn", DateOfBirth = new DateTime(2002,10,2)  },
            new Student { StudentId = 29, FullName = "Dang Long",        Email = "SE181929@fpt.edu.vn", DateOfBirth = new DateTime(2003,5,17)  },
            new Student { StudentId = 30, FullName = "Bui Minh",         Email = "SE181930@fpt.edu.vn", DateOfBirth = new DateTime(2002,12,26) },
            new Student { StudentId = 31, FullName = "Nguyen Nga",       Email = "SE181931@fpt.edu.vn", DateOfBirth = new DateTime(2003,2,9)   },
            new Student { StudentId = 32, FullName = "Tran Nhat",        Email = "SE181932@fpt.edu.vn", DateOfBirth = new DateTime(2002,8,31)  },
            new Student { StudentId = 33, FullName = "Le Phuc",          Email = "SE181933@fpt.edu.vn", DateOfBirth = new DateTime(2003,6,22)  },
            new Student { StudentId = 34, FullName = "Pham Quan",        Email = "SE181934@fpt.edu.vn", DateOfBirth = new DateTime(2002,4,15)  },
            new Student { StudentId = 35, FullName = "Hoang Sang",       Email = "SE181935@fpt.edu.vn", DateOfBirth = new DateTime(2003,10,3)  },
            new Student { StudentId = 36, FullName = "Ngo Thanh",        Email = "SE181936@fpt.edu.vn", DateOfBirth = new DateTime(2002,1,24)  },
            new Student { StudentId = 37, FullName = "Do Thi",           Email = "SE181937@fpt.edu.vn", DateOfBirth = new DateTime(2003,11,11) },
            new Student { StudentId = 38, FullName = "Ly Thu",           Email = "SE181938@fpt.edu.vn", DateOfBirth = new DateTime(2002,7,4)   },
            new Student { StudentId = 39, FullName = "Dang Toan",        Email = "SE181939@fpt.edu.vn", DateOfBirth = new DateTime(2003,3,27)  },
            new Student { StudentId = 40, FullName = "Bui Trung",        Email = "SE181940@fpt.edu.vn", DateOfBirth = new DateTime(2002,9,18)  },
            new Student { StudentId = 41, FullName = "Nguyen Tu Anh",    Email = "SE181941@fpt.edu.vn", DateOfBirth = new DateTime(2003,8,8)   },
            new Student { StudentId = 42, FullName = "Tran Tuyet Nhi",   Email = "SE181942@fpt.edu.vn", DateOfBirth = new DateTime(2002,5,29)  },
            new Student { StudentId = 43, FullName = "Le Quoc Anh",      Email = "SE181943@fpt.edu.vn", DateOfBirth = new DateTime(2003,12,16) },
            new Student { StudentId = 44, FullName = "Pham Hoang Viet",  Email = "SE181944@fpt.edu.vn", DateOfBirth = new DateTime(2002,2,5)   },
            new Student { StudentId = 45, FullName = "Hoang Kim Chi",    Email = "SE181945@fpt.edu.vn", DateOfBirth = new DateTime(2003,6,19)  },
            new Student { StudentId = 46, FullName = "Ngo Thanh Nhan",   Email = "SE181946@fpt.edu.vn", DateOfBirth = new DateTime(2002,11,7)  },
            new Student { StudentId = 47, FullName = "Do Bao Dan",       Email = "SE181947@fpt.edu.vn", DateOfBirth = new DateTime(2003,4,23)  },
            new Student { StudentId = 48, FullName = "Ly Thanh Tam",     Email = "SE181948@fpt.edu.vn", DateOfBirth = new DateTime(2002,10,14) },
            new Student { StudentId = 49, FullName = "Dang Van Hieu",    Email = "SE181949@fpt.edu.vn", DateOfBirth = new DateTime(2003,1,6)   },
            new Student { StudentId = 50, FullName = "Bui Khanh Linh",   Email = "SE181950@fpt.edu.vn", DateOfBirth = new DateTime(2002,7,20)  }
        );

        // ── Enrollments (500 = 50 students × 10 enrollments each) ─────
        // Deterministic: each student enrolls in 10 different courses (rotated by offset)
        var statusCycle = new[] { "Active", "Completed", "Active", "Dropped", "Pending",
                                   "Active", "Completed", "Active", "Active",  "Completed" };
        var enrollments = new List<Enrollment>();
        int eid = 1;
        for (int s = 1; s <= 50; s++)
        {
            for (int j = 0; j < 10; j++)
            {
                int courseId = ((s - 1 + j * 3) % 20) + 1;
                var enrollDate = new DateTime(2023, 8, 1).AddDays((s * 7 + j * 13) % 600);
                enrollments.Add(new Enrollment
                {
                    EnrollmentId = eid++,
                    StudentId    = s,
                    CourseId     = courseId,
                    EnrollDate   = enrollDate,
                    Status       = statusCycle[(s + j) % statusCycle.Length]
                });
            }
        }
        modelBuilder.Entity<Enrollment>().HasData(enrollments);
    }
}
