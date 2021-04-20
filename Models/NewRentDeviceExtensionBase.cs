﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace VityazReports.Models
{
    [Table("New_rent_deviceExtensionBase")]
    public partial class NewRentDeviceExtensionBase
    {
        [Key]
        [Column("New_rent_deviceId")]
        public Guid NewRentDeviceId { get; set; }
        [Column("New_name")]
        [StringLength(100)]
        public string NewName { get; set; }
        [Column("New_price", TypeName = "money")]
        public decimal? NewPrice { get; set; }
        [Column("new_price_Base", TypeName = "money")]
        public decimal? NewPriceBase { get; set; }
        [Column("New_qty")]
        public int? NewQty { get; set; }
        [Column("New_guard_object_rent_device")]
        public Guid? NewGuardObjectRentDevice { get; set; }
        [Column("New_device_rent_device")]
        public Guid? NewDeviceRentDevice { get; set; }
        [Column("New_andromeda")]
        public Guid? NewAndromeda { get; set; }
        [Column("New_return_guard")]
        public Guid? NewReturnGuard { get; set; }
        [Column("New_is_return")]
        public bool? NewIsReturn { get; set; }

        [ForeignKey(nameof(NewAndromeda))]
        [InverseProperty(nameof(NewAndromedaBase.NewRentDeviceExtensionBase))]
        public virtual NewAndromedaBase NewAndromedaNavigation { get; set; }
        [ForeignKey(nameof(NewDeviceRentDevice))]
        [InverseProperty(nameof(NewDeviceBase.NewRentDeviceExtensionBase))]
        public virtual NewDeviceBase NewDeviceRentDeviceNavigation { get; set; }
        [ForeignKey(nameof(NewGuardObjectRentDevice))]
        [InverseProperty(nameof(NewGuardObjectBase.NewRentDeviceExtensionBaseNewGuardObjectRentDeviceNavigation))]
        public virtual NewGuardObjectBase NewGuardObjectRentDeviceNavigation { get; set; }
        [ForeignKey(nameof(NewRentDeviceId))]
        [InverseProperty(nameof(NewRentDeviceBase.NewRentDeviceExtensionBase))]
        public virtual NewRentDeviceBase NewRentDevice { get; set; }
        [ForeignKey(nameof(NewReturnGuard))]
        [InverseProperty(nameof(NewGuardObjectBase.NewRentDeviceExtensionBaseNewReturnGuardNavigation))]
        public virtual NewGuardObjectBase NewReturnGuardNavigation { get; set; }
    }
}