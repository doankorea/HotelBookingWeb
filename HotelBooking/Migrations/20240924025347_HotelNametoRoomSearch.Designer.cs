﻿// <auto-generated />
using System;
using HotelBooking.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace HotelBooking.Migrations
{
    [DbContext(typeof(HotelDbContext))]
    [Migration("20240924025347_HotelNametoRoomSearch")]
    partial class HotelNametoRoomSearch
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("HotelBooking.Models.Amenity", b =>
                {
                    b.Property<int>("AmenityId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("AmenityID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AmenityId"));

                    b.Property<string>("Description")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<bool?>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(true);

                    b.Property<string>("Name")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("AmenityId")
                        .HasName("PK__Amenitie__842AF52B16276B79");

                    b.ToTable("Amenities");
                });

            modelBuilder.Entity("HotelBooking.Models.Country", b =>
                {
                    b.Property<int>("CountryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("CountryID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CountryId"));

                    b.Property<string>("CountryCode")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("CountryName")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("CountryId")
                        .HasName("PK__Countrie__10D160BF3F545FFE");

                    b.ToTable("Countries");
                });

            modelBuilder.Entity("HotelBooking.Models.Feedback", b =>
                {
                    b.Property<int>("FeedbackId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("FeedbackID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("FeedbackId"));

                    b.Property<string>("Comment")
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<DateTime?>("FeedbackDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<int?>("Rating")
                        .HasColumnType("int");

                    b.Property<int?>("ReservationId")
                        .HasColumnType("int")
                        .HasColumnName("ReservationID");

                    b.HasKey("FeedbackId")
                        .HasName("PK__Feedback__6A4BEDF6D054479F");

                    b.HasIndex("ReservationId");

                    b.ToTable("Feedbacks");
                });

            modelBuilder.Entity("HotelBooking.Models.Hotel", b =>
                {
                    b.Property<int>("HotelId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("HotelId"));

                    b.Property<string>("HotelName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Rating")
                        .HasColumnType("float");

                    b.HasKey("HotelId");

                    b.ToTable("Hotel");
                });

            modelBuilder.Entity("HotelBooking.Models.Payment", b =>
                {
                    b.Property<int>("PaymentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("PaymentID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PaymentId"));

                    b.Property<decimal?>("Amount")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<DateTime?>("PaymentDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("PaymentMethod")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int?>("ReservationId")
                        .HasColumnType("int")
                        .HasColumnName("ReservationID");

                    b.HasKey("PaymentId")
                        .HasName("PK__Payments__9B556A58F509696D");

                    b.HasIndex("ReservationId");

                    b.ToTable("Payments");
                });

            modelBuilder.Entity("HotelBooking.Models.Reservation", b =>
                {
                    b.Property<int>("ReservationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("ReservationID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ReservationId"));

                    b.Property<DateTime?>("BookingDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<DateTime?>("CheckInDate")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("CheckOutDate")
                        .HasColumnType("datetime");

                    b.Property<int?>("NumberOfGuests")
                        .HasColumnType("int");

                    b.Property<int?>("RoomId")
                        .HasColumnType("int")
                        .HasColumnName("RoomID");

                    b.Property<string>("Status")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int?>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("UserID");

                    b.HasKey("ReservationId")
                        .HasName("PK__Reservat__B7EE5F0436372CF2");

                    b.HasIndex("RoomId");

                    b.HasIndex("UserId");

                    b.ToTable("Reservations");
                });

            modelBuilder.Entity("HotelBooking.Models.Room", b =>
                {
                    b.Property<int>("RoomId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("RoomID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RoomId"));

                    b.Property<int?>("CountryId")
                        .HasColumnType("int")
                        .HasColumnName("CountryID");

                    b.Property<DateTime?>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<int?>("HotelID")
                        .HasColumnType("int");

                    b.Property<string>("Image")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<decimal?>("Price")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<string>("RoomName")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<int?>("RoomTypeId")
                        .HasColumnType("int")
                        .HasColumnName("RoomTypeID");

                    b.Property<string>("Status")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("RoomId")
                        .HasName("PK__Rooms__32863919ED523B01");

                    b.HasIndex("CountryId");

                    b.HasIndex("HotelID");

                    b.HasIndex("RoomTypeId");

                    b.HasIndex(new[] { "RoomName" }, "UQ__Rooms__6B500B55D94A940B")
                        .IsUnique()
                        .HasFilter("[RoomName] IS NOT NULL");

                    b.ToTable("Rooms");
                });

            modelBuilder.Entity("HotelBooking.Models.RoomType", b =>
                {
                    b.Property<int>("RoomTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("RoomTypeID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RoomTypeId"));

                    b.Property<string>("Description")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(true);

                    b.Property<string>("TypeName")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("RoomTypeId")
                        .HasName("PK__RoomType__BCC8961148B57913");

                    b.ToTable("RoomTypes");
                });

            modelBuilder.Entity("HotelBooking.Models.State", b =>
                {
                    b.Property<int>("StateId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("StateID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("StateId"));

                    b.Property<int?>("CountryId")
                        .HasColumnType("int")
                        .HasColumnName("CountryID");

                    b.Property<string>("StateName")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("StateId")
                        .HasName("PK__States__C3BA3B5ABCE18FB0");

                    b.HasIndex("CountryId");

                    b.ToTable("States");
                });

            modelBuilder.Entity("HotelBooking.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("UserID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserId"));

                    b.Property<DateTime?>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(true);

                    b.Property<string>("PasswordHash")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Role")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("UserId")
                        .HasName("PK__Users__1788CCACB90D6ED4");

                    b.HasIndex(new[] { "Email" }, "UQ__Users__A9D105344131D01A")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("RoomAmenity", b =>
                {
                    b.Property<int>("RoomTypeId")
                        .HasColumnType("int")
                        .HasColumnName("RoomTypeID");

                    b.Property<int>("AmenityId")
                        .HasColumnType("int")
                        .HasColumnName("AmenityID");

                    b.HasKey("RoomTypeId", "AmenityId")
                        .HasName("PK__RoomAmen__148A394399AC099C");

                    b.HasIndex("AmenityId");

                    b.ToTable("RoomAmenities", (string)null);
                });

            modelBuilder.Entity("HotelBooking.Models.Feedback", b =>
                {
                    b.HasOne("HotelBooking.Models.Reservation", "Reservation")
                        .WithMany("Feedbacks")
                        .HasForeignKey("ReservationId")
                        .HasConstraintName("FK__Feedbacks__Reser__5535A963");

                    b.Navigation("Reservation");
                });

            modelBuilder.Entity("HotelBooking.Models.Payment", b =>
                {
                    b.HasOne("HotelBooking.Models.Reservation", "Reservation")
                        .WithMany("Payments")
                        .HasForeignKey("ReservationId")
                        .HasConstraintName("FK__Payments__Reserv__5DCAEF64");

                    b.Navigation("Reservation");
                });

            modelBuilder.Entity("HotelBooking.Models.Reservation", b =>
                {
                    b.HasOne("HotelBooking.Models.Room", "Room")
                        .WithMany("Reservations")
                        .HasForeignKey("RoomId")
                        .HasConstraintName("FK__Reservati__RoomI__48CFD27E");

                    b.HasOne("HotelBooking.Models.User", "User")
                        .WithMany("Reservations")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK__Reservati__UserI__47DBAE45");

                    b.Navigation("Room");

                    b.Navigation("User");
                });

            modelBuilder.Entity("HotelBooking.Models.Room", b =>
                {
                    b.HasOne("HotelBooking.Models.Country", "Country")
                        .WithMany("Rooms")
                        .HasForeignKey("CountryId")
                        .HasConstraintName("FK__Rooms__CountryID__693CA210");

                    b.HasOne("HotelBooking.Models.Hotel", "Hotel")
                        .WithMany("Rooms")
                        .HasForeignKey("HotelID");

                    b.HasOne("HotelBooking.Models.RoomType", "RoomType")
                        .WithMany("Rooms")
                        .HasForeignKey("RoomTypeId")
                        .HasConstraintName("FK__Rooms__RoomTypeI__4316F928");

                    b.Navigation("Country");

                    b.Navigation("Hotel");

                    b.Navigation("RoomType");
                });

            modelBuilder.Entity("HotelBooking.Models.State", b =>
                {
                    b.HasOne("HotelBooking.Models.Country", "Country")
                        .WithMany("States")
                        .HasForeignKey("CountryId")
                        .HasConstraintName("FK__States__CountryI__59FA5E80");

                    b.Navigation("Country");
                });

            modelBuilder.Entity("RoomAmenity", b =>
                {
                    b.HasOne("HotelBooking.Models.Amenity", null)
                        .WithMany()
                        .HasForeignKey("AmenityId")
                        .IsRequired()
                        .HasConstraintName("FK__RoomAmeni__Ameni__5070F446");

                    b.HasOne("HotelBooking.Models.RoomType", null)
                        .WithMany()
                        .HasForeignKey("RoomTypeId")
                        .IsRequired()
                        .HasConstraintName("FK__RoomAmeni__RoomT__4F7CD00D");
                });

            modelBuilder.Entity("HotelBooking.Models.Country", b =>
                {
                    b.Navigation("Rooms");

                    b.Navigation("States");
                });

            modelBuilder.Entity("HotelBooking.Models.Hotel", b =>
                {
                    b.Navigation("Rooms");
                });

            modelBuilder.Entity("HotelBooking.Models.Reservation", b =>
                {
                    b.Navigation("Feedbacks");

                    b.Navigation("Payments");
                });

            modelBuilder.Entity("HotelBooking.Models.Room", b =>
                {
                    b.Navigation("Reservations");
                });

            modelBuilder.Entity("HotelBooking.Models.RoomType", b =>
                {
                    b.Navigation("Rooms");
                });

            modelBuilder.Entity("HotelBooking.Models.User", b =>
                {
                    b.Navigation("Reservations");
                });
#pragma warning restore 612, 618
        }
    }
}
