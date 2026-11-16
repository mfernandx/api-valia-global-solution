
using Microsoft.EntityFrameworkCore;
using ValiaApiWeb.Domain.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ValiaApiWeb.Data
{
    public class ValiaDbContext : DbContext
    {
        public ValiaDbContext(DbContextOptions<ValiaDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Employment> Employments { get; set; }
        public DbSet<WorkShift> WorkShifts { get; set; }
        public DbSet<WorkOccurrence> WorkOccurrences { get; set; }
        public DbSet<WorkConditionQuestion> WorkConditionQuestions { get; set; }
        public DbSet<WorkConditionAssessment> WorkConditionAssessments { get; set; }
        public DbSet<WorkConditionAnswer> WorkConditionAnswers { get; set; }
        public DbSet<Evidence> Evidences { get; private set; }
        public DbSet<CommunityPost> CommunityPosts { get; private set; }
        public DbSet<CommunityComment> CommunityComments { get; private set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            var dateOnlyConverter = new ValueConverter<DateOnly, DateTime>(
                date => date.ToDateTime(TimeOnly.MinValue),
                dateTime => DateOnly.FromDateTime(dateTime)
            );


            modelBuilder.Entity<User>(entity =>
            {

                entity.ToTable("USERS");
                entity.HasKey(u => u.UserId);
                entity.Property(u => u.UserId).HasColumnName("USER_ID").ValueGeneratedNever();
                entity.Property(u => u.FullName).HasColumnName("FULL_NAME").HasMaxLength(150).IsRequired();
                entity.Property(u => u.Email).HasColumnName("EMAIL").HasMaxLength(200).IsRequired();
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.PhoneNumber).HasColumnName("PHONE_NUMBER").HasMaxLength(20).IsRequired(false);
                entity.Property(u => u.PasswordHash).HasColumnName("PASSWORD_HASH").HasMaxLength(500).IsRequired();
                entity.Property(u => u.IsActive).HasColumnName("IS_ACTIVE").HasColumnType("NUMBER(1)").IsRequired();
                entity.Property(u => u.CreatedAt).HasColumnName("CREATED_AT").IsRequired();
                entity.Property(u => u.UpdatedAt).HasColumnName("UPDATED_AT").IsRequired(false);
                entity.HasMany(u => u.Employments).WithOne(e => e.User).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Restrict);
                entity.HasMany(u => u.Evidences).WithOne(e => e.User).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Restrict);
                entity.HasMany(u => u.CommunityPosts).WithOne(p => p.User).HasForeignKey(p => p.UserId).OnDelete(DeleteBehavior.Restrict);
                entity.HasMany(u => u.CommunityComments).WithOne(c => c.User).HasForeignKey(c => c.UserId).OnDelete(DeleteBehavior.Restrict);

            });


            modelBuilder.Entity<Employment>(entity =>
            {
                
                entity.ToTable("EMPLOYMENTS");
                entity.HasKey(e => e.EmploymentId);
                entity.Property(e => e.EmploymentId).HasColumnName("EMPLOYMENT_ID").ValueGeneratedNever();
                entity.Property(e => e.UserId).HasColumnName("USER_ID").IsRequired();
                entity.Property(e => e.JobTitle).HasColumnName("JOB_TITLE").HasMaxLength(150).IsRequired();
                entity.Property(e => e.EmploymentType).HasColumnName("EMPLOYMENT_TYPE").HasConversion<string>().HasMaxLength(30).IsRequired();
                entity.Property(e => e.AdmissionDate).HasColumnName("ADMISSION_DATE").HasColumnType("DATE").HasConversion(dateOnlyConverter).IsRequired();
                entity.Property(e => e.TerminationDate).HasColumnName("TERMINATION_DATE").HasColumnType("DATE").HasConversion(dateOnlyConverter).IsRequired(false);
                entity.Property(e => e.BaseSalary).HasColumnName("BASE_SALARY").HasColumnType("NUMBER(12,2)").IsRequired(false);
                entity.Property(e => e.WeeklyWorkload).HasColumnName("WEEKLY_WORKLOAD").HasColumnType("NUMBER(5,2)").IsRequired(false);
                entity.Property(e => e.ProfessionalCategory).HasColumnName("PROFESSIONAL_CATEGORY").HasMaxLength(150).IsRequired(false);
                entity.Property(e => e.CreatedAt).HasColumnName("CREATED_AT").IsRequired();
                entity.Property(e => e.UpdatedAt).HasColumnName("UPDATED_AT").IsRequired(false);
                entity.Property(e => e.Version).HasColumnName("VERSION").IsRequired();
                entity.HasOne(e => e.User).WithMany(u => u.Employments).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Restrict);
                entity.HasMany(e => e.WorkShifts).WithOne(w => w.Employment).HasForeignKey(w => w.EmploymentId).OnDelete(DeleteBehavior.Restrict);
                entity.HasMany(e => e.WorkOccurrences).WithOne(o => o.Employment).HasForeignKey(o => o.EmploymentId).OnDelete(DeleteBehavior.Restrict);
                entity.HasMany(e => e.WorkConditionAssessments).WithOne(a => a.Employment).HasForeignKey(a => a.EmploymentId).OnDelete(DeleteBehavior.Restrict);
                entity.HasMany(e => e.Evidences).WithOne(ev => ev.Employment).HasForeignKey(ev => ev.EmploymentId).OnDelete(DeleteBehavior.Restrict);
            });



            modelBuilder.Entity<WorkShift>(entity =>
            {
                entity.ToTable("WORK_SHIFTS");
                entity.HasKey(w => w.WorkShiftId);
                entity.Property(w => w.WorkShiftId).HasColumnName("WORK_SHIFT_ID").ValueGeneratedNever();
                entity.Property(w => w.EmploymentId).HasColumnName("EMPLOYMENT_ID").IsRequired();
                entity.Property(w => w.StartTime).HasColumnName("START_TIME").IsRequired();
                entity.Property(w => w.EndTime).HasColumnName("END_TIME").IsRequired();
                entity.Property(w => w.BreakStart).HasColumnName("BREAK_START").IsRequired(false);
                entity.Property(w => w.BreakEnd).HasColumnName("BREAK_END").IsRequired(false);
                entity.Property(w => w.Notes).HasColumnName("NOTES").HasMaxLength(1000).IsRequired(false);
                entity.Property(w => w.CreatedAt).HasColumnName("CREATED_AT").IsRequired();
                entity.Property(w => w.UpdatedAt).HasColumnName("UPDATED_AT").IsRequired(false);
                entity.Property(w => w.Version).HasColumnName("VERSION").IsRequired();
                entity.HasIndex(w => new { w.EmploymentId, w.StartTime });
                entity.HasOne(w => w.Employment).WithMany(e => e.WorkShifts).HasForeignKey(w => w.EmploymentId).OnDelete(DeleteBehavior.Restrict);
            });


            modelBuilder.Entity<WorkOccurrence>(entity =>
            {
                entity.ToTable("WORK_OCCURRENCES");
                entity.HasKey(o => o.WorkOccurrenceId);
                entity.Property(o => o.WorkOccurrenceId).HasColumnName("WORK_OCCURRENCE_ID").ValueGeneratedNever();
                entity.Property(o => o.EmploymentId).HasColumnName("EMPLOYMENT_ID").IsRequired();
                entity.Property(o => o.Category).HasColumnName("CATEGORY").HasConversion<string>().HasMaxLength(50).IsRequired();
                entity.Property(o => o.Description).HasColumnName("DESCRIPTION").HasMaxLength(3000).IsRequired();
                entity.Property(o => o.OccurredAt).HasColumnName("OCCURRED_AT").IsRequired();
                entity.Property(o => o.CreatedAt).HasColumnName("CREATED_AT").IsRequired();
                entity.Property(o => o.UpdatedAt).HasColumnName("UPDATED_AT").IsRequired(false);
                entity.Property(o => o.Version).HasColumnName("VERSION").IsRequired();
                entity.HasIndex(o => new { o.EmploymentId, o.OccurredAt });
                entity.HasOne(o => o.Employment).WithMany(e => e.WorkOccurrences).HasForeignKey(o => o.EmploymentId).OnDelete(DeleteBehavior.Restrict);
            });


            modelBuilder.Entity<Evidence>(entity =>
            {
                entity.ToTable("EVIDENCES");
                entity.HasKey(e => e.EvidenceId);
                entity.Property(e => e.EvidenceId).HasColumnName("EVIDENCE_ID").ValueGeneratedNever();
                entity.Property(e => e.UserId).HasColumnName("USER_ID").IsRequired();
                entity.Property(e => e.EmploymentId).HasColumnName("EMPLOYMENT_ID").IsRequired(false);
                entity.Property(e => e.Title).HasColumnName("TITLE").HasMaxLength(150).IsRequired();
                entity.Property(e => e.Category).HasColumnName("CATEGORY").HasConversion<string>().HasMaxLength(50).IsRequired();
                entity.Property(e => e.FileName).HasColumnName("FILE_NAME").HasMaxLength(255).IsRequired();
                entity.Property(e => e.StorageKey).HasColumnName("STORAGE_KEY").HasMaxLength(100).IsRequired();
                entity.Property(e => e.ContentType).HasColumnName("CONTENT_TYPE").HasMaxLength(100).IsRequired();
                entity.Property(e => e.FileSize).HasColumnName("FILE_SIZE").IsRequired();
                entity.Property(e => e.CreatedAt).HasColumnName("CREATED_AT").IsRequired();
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.EmploymentId);
                entity.HasIndex(e => e.StorageKey).IsUnique();
                entity.HasOne(e => e.User).WithMany(u => u.Evidences).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Employment).WithMany(emp => emp.Evidences).HasForeignKey(e => e.EmploymentId).OnDelete(DeleteBehavior.Restrict);

            });


            modelBuilder.Entity<WorkConditionQuestion>(entity =>
            {
                entity.ToTable("WORK_CONDITION_QUESTIONS");
                entity.HasKey(q => q.WorkConditionQuestionId);
                entity.Property(q => q.WorkConditionQuestionId).HasColumnName("WORK_CONDITION_QUESTION_ID").ValueGeneratedNever();
                entity.Property(q => q.QuestionText).HasColumnName("QUESTION_TEXT").HasMaxLength(1000).IsRequired();
                entity.Property(q => q.Category).HasColumnName("CATEGORY").HasConversion<string>().HasMaxLength(50).IsRequired();
                entity.Property(q => q.DisplayOrder).HasColumnName("DISPLAY_ORDER").IsRequired();
                entity.Property(q => q.QuestionnaireVersion).HasColumnName("QUESTIONNAIRE_VERSION").IsRequired();
                entity.Property(q => q.IsActive).HasColumnName("IS_ACTIVE").HasColumnType("NUMBER(1)").IsRequired();
                entity.Property(q => q.CreatedAt).HasColumnName("CREATED_AT").IsRequired();
                entity.HasIndex(q => new { q.QuestionnaireVersion, q.DisplayOrder }).IsUnique();
            });


            modelBuilder.Entity<WorkConditionAssessment>(entity =>
            {
                entity.ToTable("WORK_CONDITION_ASSESSMENTS");
                entity.HasKey(a => a.WorkConditionAssessmentId);
                entity.Property(a => a.WorkConditionAssessmentId).HasColumnName("WORK_CONDITION_ASSESSMENT_ID").ValueGeneratedNever();
                entity.Property(a => a.EmploymentId).HasColumnName("EMPLOYMENT_ID").IsRequired();
                entity.Property(a => a.QuestionnaireVersion).HasColumnName("QUESTIONNAIRE_VERSION").IsRequired();
                entity.Property(a => a.AssessmentDate).HasColumnName("ASSESSMENT_DATE").HasColumnType("DATE").HasConversion(dateOnlyConverter).IsRequired();
                entity.Property(a => a.CreatedAt).HasColumnName("CREATED_AT").IsRequired();
                entity.HasIndex(a => new { a.EmploymentId, a.AssessmentDate });
                entity.HasOne(a => a.Employment).WithMany(e => e.WorkConditionAssessments).HasForeignKey(a => a.EmploymentId).OnDelete(DeleteBehavior.Restrict);
            });



            modelBuilder.Entity<WorkConditionAnswer>(entity =>
            {
                entity.ToTable("WORK_CONDITION_ANSWERS");
                entity.HasKey(a => a.WorkConditionAnswerId);
                entity.Property(a => a.WorkConditionAnswerId).HasColumnName("WORK_CONDITION_ANSWER_ID").ValueGeneratedNever();
                entity.Property(a => a.WorkConditionAssessmentId).HasColumnName("WORK_CONDITION_ASSESSMENT_ID").IsRequired();
                entity.Property(a => a.WorkConditionQuestionId).HasColumnName("WORK_CONDITION_QUESTION_ID").IsRequired();
                entity.Property(a => a.Score).HasColumnName("SCORE").IsRequired();
                entity.Property(a => a.Observation).HasColumnName("OBSERVATION").HasMaxLength(1000).IsRequired(false);
                entity.HasIndex(a => new { a.WorkConditionAssessmentId, a.WorkConditionQuestionId }).IsUnique();
                entity.HasOne(a => a.Assessment).WithMany(assessment => assessment.Answers).HasForeignKey(a => a.WorkConditionAssessmentId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(a => a.Question)
                    .WithMany(question => question.Answers)
                    .HasForeignKey(a => a.WorkConditionQuestionId)
                    .OnDelete(DeleteBehavior.Restrict);
            });



            modelBuilder.Entity<CommunityPost>(entity =>
            {
                entity.ToTable("COMMUNITY_POSTS");
                entity.HasKey(p => p.CommunityPostId);
                entity.Property(p => p.CommunityPostId).HasColumnName("COMMUNITY_POST_ID").ValueGeneratedNever();
                entity.Property(p => p.UserId).HasColumnName("USER_ID").IsRequired();
                entity.Property(p => p.Category).HasColumnName("CATEGORY").HasConversion<string>().HasMaxLength(30).IsRequired();
                entity.Property(p => p.Title).HasColumnName("TITLE").HasMaxLength(150).IsRequired();
                entity.Property(p => p.Content).HasColumnName("CONTENT").HasMaxLength(5000).IsRequired();
                entity.Property(p => p.IsAnonymous).HasColumnName("IS_ANONYMOUS").HasColumnType("NUMBER(1)").IsRequired();
                entity.Property(p => p.IsActive).HasColumnName("IS_ACTIVE").HasColumnType("NUMBER(1)").IsRequired();
                entity.Property(p => p.CreatedAt).HasColumnName("CREATED_AT").IsRequired();
                entity.Property(p => p.UpdatedAt).HasColumnName("UPDATED_AT").IsRequired(false);
                entity.Property(p => p.Version).HasColumnName("VERSION").IsRequired();
                entity.HasIndex(p => new { p.IsActive, p.CreatedAt });
                entity.HasOne(p => p.User).WithMany(u => u.CommunityPosts).HasForeignKey(p => p.UserId).OnDelete(DeleteBehavior.Restrict);
            });



            modelBuilder.Entity<CommunityComment>(entity =>
            {
                entity.ToTable("COMMUNITY_COMMENTS");
                entity.HasKey(c => c.CommunityCommentId);
                entity.Property(c => c.CommunityCommentId).HasColumnName("COMMUNITY_COMMENT_ID").ValueGeneratedNever();
                entity.Property(c => c.CommunityPostId).HasColumnName("COMMUNITY_POST_ID").IsRequired();
                entity.Property(c => c.UserId).HasColumnName("USER_ID").IsRequired();
                entity.Property(c => c.Content).HasColumnName("CONTENT").HasMaxLength(2000).IsRequired();
                entity.Property(c => c.IsAnonymous).HasColumnName("IS_ANONYMOUS").HasColumnType("NUMBER(1)").IsRequired();
                entity.Property(c => c.IsActive).HasColumnName("IS_ACTIVE").HasColumnType("NUMBER(1)").IsRequired();
                entity.Property(c => c.CreatedAt).HasColumnName("CREATED_AT").IsRequired();
                entity.Property(c => c.UpdatedAt).HasColumnName("UPDATED_AT").IsRequired(false);
                entity.Property(c => c.Version).HasColumnName("VERSION").IsRequired();
                entity.HasIndex(c => new { c.CommunityPostId, c.IsActive, c.CreatedAt });
                entity.HasOne(c => c.CommunityPost).WithMany(p => p.Comments).HasForeignKey(c => c.CommunityPostId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(c => c.User).WithMany(u => u.CommunityComments).HasForeignKey(c => c.UserId).OnDelete(DeleteBehavior.Restrict);
            });

        }

    }
}

