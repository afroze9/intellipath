﻿// <auto-generated />
using System;
using IntelliPath.Orchestrator.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace IntelliPath.Orchestrator.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.0");

            modelBuilder.Entity("IntelliPath.Orchestrator.Entities.ChatMessage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("ConversationId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("ModifiedAt")
                        .HasColumnType("TEXT");

                    b.Property<int>("Role")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ConversationId");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("IntelliPath.Orchestrator.Entities.Conversation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("ModifiedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Conversations");
                });

            modelBuilder.Entity("IntelliPath.Orchestrator.Entities.MemoryTag", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("MemoryTags");

                    b.HasData(
                        new
                        {
                            Id = new Guid("4a425f94-7d0b-48a9-9e99-7ceb42cf6f1d"),
                            Name = "Personal Info"
                        },
                        new
                        {
                            Id = new Guid("73095da3-2a2e-4310-9501-fbcce3bad261"),
                            Name = "Work Info"
                        },
                        new
                        {
                            Id = new Guid("11fa6aa8-6781-4fa9-8ca3-ec5f1de998c4"),
                            Name = "Hobbies"
                        },
                        new
                        {
                            Id = new Guid("1d3b28a9-7ead-485c-9ed8-807e5c6d4ddb"),
                            Name = "Skills"
                        },
                        new
                        {
                            Id = new Guid("54a563df-7731-47e6-8808-72bca4bae316"),
                            Name = "Education"
                        },
                        new
                        {
                            Id = new Guid("e808f592-8c8f-461d-9be0-6f9d0f1d4f6f"),
                            Name = "Certifications"
                        },
                        new
                        {
                            Id = new Guid("305e4feb-3c13-499e-b4d8-483be516725a"),
                            Name = "Projects"
                        });
                });

            modelBuilder.Entity("IntelliPath.Orchestrator.Entities.ChatMessage", b =>
                {
                    b.HasOne("IntelliPath.Orchestrator.Entities.Conversation", "Conversation")
                        .WithMany("Messages")
                        .HasForeignKey("ConversationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Conversation");
                });

            modelBuilder.Entity("IntelliPath.Orchestrator.Entities.Conversation", b =>
                {
                    b.Navigation("Messages");
                });
#pragma warning restore 612, 618
        }
    }
}
