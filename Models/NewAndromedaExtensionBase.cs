﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace VityazReports.Models
{
    [Table("New_andromedaExtensionBase")]
    public partial class NewAndromedaExtensionBase
    {
        [Column("New_address")]
        [StringLength(200)]
        public string NewAddress { get; set; }
        [Key]
        [Column("New_andromedaId")]
        public Guid NewAndromedaId { get; set; }
        [Column("New_corp")]
        [StringLength(100)]
        public string NewCorp { get; set; }
        [Column("New_name")]
        [StringLength(255)]
        public string NewName { get; set; }
        [Column("New_number")]
        public int? NewNumber { get; set; }
        [Column("New_objtype")]
        [StringLength(200)]
        public string NewObjtype { get; set; }
        [Column("New_phone")]
        [StringLength(100)]
        public string NewPhone { get; set; }
        [Column("New_square")]
        [StringLength(150)]
        public string NewSquare { get; set; }
        [Column("New_startdate")]
        [StringLength(100)]
        public string NewStartdate { get; set; }
        [Column("New_type")]
        [StringLength(100)]
        public string NewType { get; set; }
        [Column("New_phone2")]
        [StringLength(100)]
        public string NewPhone2 { get; set; }
        [Column("New_enddate")]
        [StringLength(50)]
        public string NewEnddate { get; set; }
        [Column("New_post_")]
        public Guid? NewPost { get; set; }
        [Column("New_sms_phone")]
        [StringLength(15)]
        public string NewSmsPhone { get; set; }
        [Column("New_contact_andromeda")]
        public Guid? NewContactAndromeda { get; set; }
        [Column("New_contact_info")]
        [StringLength(250)]
        public string NewContactInfo { get; set; }
        [Column("New_remote_programming")]
        public bool? NewRemoteProgramming { get; set; }
        [Column("New_is_to_ps")]
        public bool? NewIsToPs { get; set; }
        [Column("New_route")]
        public int? NewRoute { get; set; }
        [Column("New_termination")]
        public bool? NewTermination { get; set; }

        [ForeignKey(nameof(NewAndromedaId))]
        [InverseProperty(nameof(NewAndromedaBase.NewAndromedaExtensionBase))]
        public virtual NewAndromedaBase NewAndromeda { get; set; }
        [ForeignKey(nameof(NewPost))]
        [InverseProperty(nameof(NewAlarmBase.NewAndromedaExtensionBase))]
        public virtual NewAlarmBase NewPostNavigation { get; set; }
    }
}