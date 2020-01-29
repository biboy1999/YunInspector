using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace YunInspector.CIIP
{
    public partial class CIIPContext : DbContext
    {
        public CIIPContext()
        {
        }

        public CIIPContext(DbContextOptions<CIIPContext> options)
            : base(options)
        {
        }

        public virtual DbSet<College> College { get; set; }
        public virtual DbSet<CollegeDepartment> CollegeDepartment { get; set; }
        public virtual DbSet<Comment> Comment { get; set; }
        public virtual DbSet<Course> Course { get; set; }
        public virtual DbSet<Department> Department { get; set; }
        public virtual DbSet<Teacher> Teacher { get; set; }
        public virtual DbSet<TeacherCourse> TeacherCourse { get; set; }
        public virtual DbSet<User> User { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {

            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<College>(entity =>
            {
                entity.Property(e => e.CollegeId)
                    .HasColumnName("CollegeID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Name).HasMaxLength(255);
            });

            modelBuilder.Entity<CollegeDepartment>(entity =>
            {
                // entity.HasNoKey();
                entity.HasKey(x => new { x.CollegeId, x.DepartmentId });

                entity.Property(e => e.CollegeId).HasColumnName("CollegeID");

                entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");

                entity.HasOne(d => d.College)
                    .WithMany(p => p.CollegeDepartment)
                    .HasForeignKey(d => d.CollegeId)
                    .HasConstraintName("FK_CollegeDepartment_College");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.CollegeDepartment)
                    .HasForeignKey(d => d.DepartmentId)
                    .HasConstraintName("FK_CollegeDepartment_Department");
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.Property(e => e.CommentId).HasColumnName("CommentID");

                entity.Property(e => e.CommentContent).HasColumnType("ntext");

                entity.Property(e => e.CourseId).HasColumnName("CourseID");

                entity.Property(e => e.PostDate).HasColumnType("datetime");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.Comment)
                    .HasForeignKey(d => d.CourseId)
                    .HasConstraintName("FK_Comment_Course");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Comment)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Comment_User");
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.Property(e => e.CourseId).HasColumnName("CourseID");

                entity.Property(e => e.Class).HasMaxLength(255);

                entity.Property(e => e.ClassNo).HasMaxLength(255);

                entity.Property(e => e.CollegeId).HasColumnName("CollegeID");

                entity.Property(e => e.CollegeNo).HasMaxLength(255);

                entity.Property(e => e.Credit).HasMaxLength(255);

                entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");

                entity.Property(e => e.Detail).HasMaxLength(255);

                entity.Property(e => e.Max).HasMaxLength(255);

                entity.Property(e => e.Name).HasMaxLength(255);

                entity.Property(e => e.Teacher).HasMaxLength(255);

                entity.Property(e => e.TimetableClassroom)
                    .HasColumnName("Timetable_classroom")
                    .HasMaxLength(255);

                entity.HasOne(d => d.College)
                    .WithMany(p => p.Course)
                    .HasForeignKey(d => d.CollegeId)
                    .HasConstraintName("FK_Course_College");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.Course)
                    .HasForeignKey(d => d.DepartmentId)
                    .HasConstraintName("FK_Course_Department");
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.Property(e => e.DepartmentId)
                    .HasColumnName("DepartmentID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Ename)
                    .HasColumnName("EName")
                    .HasMaxLength(255);

                entity.Property(e => e.Name).HasMaxLength(255);
            });

            modelBuilder.Entity<Teacher>(entity =>
            {
                entity.Property(e => e.TeacherId)
                    .HasColumnName("TeacherID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Name).HasMaxLength(255);
            });

            modelBuilder.Entity<TeacherCourse>(entity =>
            {
                // entity.HasNoKey();
                entity.HasKey(x => new { x.CourseId, x.TeacherId });

                entity.ToTable("Teacher_Course");

                entity.Property(e => e.CourseId).HasColumnName("CourseID");

                entity.Property(e => e.TeacherId).HasColumnName("TeacherID");

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.TeacherCourse)
                    .HasForeignKey(d => d.CourseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Teacher_Course_Course");

                entity.HasOne(d => d.Teacher)
                    .WithMany(p => p.TeacherCourse)
                    .HasForeignKey(d => d.TeacherId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Teacher_Course_Teacher");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.GoogleId)
                    .HasColumnName("GoogleID")
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
