using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace WorkoutTracker2.Model
{
    internal class WorkoutContext : DbContext
    {
        public DbSet<Workout> Workouts { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<RepWeight> RepWeights { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // SQLite is db provider, with db name 'workout'
            optionsBuilder.UseSqlite("Data Source=workout.db");
        }


        /// <summary>
        /// Specifying important aspects of the database structure
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Table names
            modelBuilder.Entity<Workout>()
                .ToTable("Workouts");
            modelBuilder.Entity<Exercise>()
                .ToTable("Exercises");
            modelBuilder.Entity<RepWeight>()
                .ToTable("RepWeights");

            // Table relationships
            modelBuilder.Entity<Workout>()
                .HasMany(w => w.Exercises) // one-to-many
                .WithOne(e => e.Workout) // one-to-one
                .HasForeignKey(e => e.WorkoutId); // foreign key

            modelBuilder.Entity<Exercise>()
                .HasMany(e => e.SetData) // one-to-many
                .WithOne(rw => rw.Exercise) // one-to-one
                .HasForeignKey(rw => rw.ExerciseId); // foreign key

            base.OnModelCreating(modelBuilder);
        }

    }
}
