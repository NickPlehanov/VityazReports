﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace VityazReports.Models
{
    [Keyless]
    [Table("New_serviceorderExtensionBase")]
    [Index(nameof(NewServiceorderId), Name = "IdIndex")]
    [Index(nameof(NewNewServiceman), Name = "NonClusteredIndex-20201110-173747")]
    [Index(nameof(NewAndromedaServiceorder), Name = "NonClusteredIndex-20201110-173852")]
    public partial class NewServiceorderExtensionBase
    {
        [Column("New_serviceorderId")]
        public Guid NewServiceorderId { get; set; }
        [Column("New_name")]
        [StringLength(200)]
        public string NewName { get; set; }
        [Column("New_date", TypeName = "datetime")]
        public DateTime? NewDate { get; set; }
        [Column("New_serviceman_serviceorder")]
        public Guid? NewServicemanServiceorder { get; set; }
        [Column("New_income", TypeName = "datetime")]
        public DateTime? NewIncome { get; set; }
        [Column("New_outgone", TypeName = "datetime")]
        public DateTime? NewOutgone { get; set; }
        [Column("New_list_return")]
        public bool? NewListReturn { get; set; }
        [Column("New_comments")]
        public string NewComments { get; set; }
        [Column("New_new_serviceman")]
        public Guid? NewNewServiceman { get; set; }
        [Column("New_category")]
        public int? NewCategory { get; set; }
        [Column("New_who_init")]
        [StringLength(100)]
        public string NewWhoInit { get; set; }
        [Column("New_address")]
        [StringLength(200)]
        public string NewAddress { get; set; }
        [Column("New_andromeda_serviceorder")]
        public Guid? NewAndromedaServiceorder { get; set; }
        [Column("New_number")]
        public int? NewNumber { get; set; }
        [Column("New_obj_name")]
        [StringLength(200)]
        public string NewObjName { get; set; }
        [Column("New_moved", TypeName = "datetime")]
        public DateTime? NewMoved { get; set; }
        [Column("New_time")]
        [StringLength(30)]
        public string NewTime { get; set; }
        [Column("New_transfer_reason")]
        [StringLength(100)]
        public string NewTransferReason { get; set; }
        [Column("New_result")]
        public int? NewResult { get; set; }
        [Column("New_moved_from")]
        [StringLength(100)]
        public string NewMovedFrom { get; set; }
        [Column("New_jobtime")]
        public int? NewJobtime { get; set; }
        [Column("New_ResultId")]
        public int? NewResultId { get; set; }
        [Column("New_TechConclusion")]
        public string NewTechConclusion { get; set; }
        [Column("New_battery_action")]
        public int? NewBatteryAction { get; set; }
        [Column("New_battery_serviceorder")]
        public Guid? NewBatteryServiceorder { get; set; }
        [Column("New_battery_install_serviceorder")]
        public Guid? NewBatteryInstallServiceorder { get; set; }
        [Column("New_timetransfer")]
        [StringLength(20)]
        public string NewTimetransfer { get; set; }
        [Column("New_AutoCreate")]
        public int? NewAutoCreate { get; set; }
        [Column("New_startdate")]
        [StringLength(100)]
        public string NewStartdate { get; set; }
        [Column("New_SMS")]
        [StringLength(10)]
        public string NewSms { get; set; }
        [Column("New_sms_pnone")]
        [StringLength(15)]
        public string NewSmsPnone { get; set; }
        [Column("New_contact_info")]
        [StringLength(250)]
        public string NewContactInfo { get; set; }
        [Column("New_sended_sms")]
        [StringLength(100)]
        public string NewSendedSms { get; set; }
        [Column("New_is_send_sms")]
        public bool? NewIsSendSms { get; set; }
        [Column("New_comment_new")]
        [StringLength(100)]
        public string NewCommentNew { get; set; }
        [Column("New_duty_count")]
        public int? NewDutyCount { get; set; }
        [Column("New_technique_end")]
        public Guid? NewTechniqueEnd { get; set; }
        [Column("New_autotechstr")]
        [StringLength(100)]
        public string NewAutotechstr { get; set; }
        [Column("New_openOnTablet")]
        public bool? NewOpenOnTablet { get; set; }
        [Column("New_mustRead")]
        public bool? NewMustRead { get; set; }
        [Column("New_go_to_object")]
        [StringLength(100)]
        public string NewGoToObject { get; set; }
        [Column("New_oper_job_naryad")]
        public bool? NewOperJobNaryad { get; set; }
        [Column("New_datetime_cancredirect")]
        [StringLength(100)]
        public string NewDatetimeCancredirect { get; set; }
        [Column("New_ps_reglament")]
        public int? NewPsReglament { get; set; }
        [Column("New_remote_programming")]
        public bool? NewRemoteProgramming { get; set; }
        [Column("New_oper_remove")]
        public bool? NewOperRemove { get; set; }
        [Column("New_not_oper_remove")]
        public bool? NewNotOperRemove { get; set; }
        [Column("New_date_remove", TypeName = "datetime")]
        public DateTime? NewDateRemove { get; set; }
        [Column("New_date_not_remove", TypeName = "datetime")]
        public DateTime? NewDateNotRemove { get; set; }
        [Column("New_mount_return")]
        public bool? NewMountReturn { get; set; }
        [Column("New_mounted_gbr")]
        public bool? NewMountedGbr { get; set; }
        [Column("New_device_is_back")]
        public bool? NewDeviceIsBack { get; set; }
        [Column("New_device_is_back_comment")]
        public string NewDeviceIsBackComment { get; set; }
        [Column("New_perenos_end")]
        public bool? NewPerenosEnd { get; set; }
        [Column("New_start_reglament_creation")]
        public bool? NewStartReglamentCreation { get; set; }
        [Column("New_to_urist")]
        [StringLength(250)]
        public string NewToUrist { get; set; }
        [Column("New_order_from")]
        public int? NewOrderFrom { get; set; }
        [Column("New_moved_kc", TypeName = "datetime")]
        public DateTime? NewMovedKc { get; set; }
        [Column("New_autoset")]
        public bool? NewAutoset { get; set; }
        [Column("New_history_button")]
        [StringLength(100)]
        public string NewHistoryButton { get; set; }
        [Column("New_TimeFrom")]
        public int? NewTimeFrom { get; set; }
        [Column("New_TimeTo")]
        public int? NewTimeTo { get; set; }

        [ForeignKey(nameof(NewAndromedaServiceorder))]
        public virtual NewAndromedaBase NewAndromedaServiceorderNavigation { get; set; }
        [ForeignKey(nameof(NewNewServiceman))]
        public virtual NewServicemanBase NewNewServicemanNavigation { get; set; }
        [ForeignKey(nameof(NewServicemanServiceorder))]
        public virtual NewServicemanBase NewServicemanServiceorderNavigation { get; set; }
        [ForeignKey(nameof(NewServiceorderId))]
        public virtual NewServiceorderBase NewServiceorder { get; set; }
        [ForeignKey(nameof(NewTechniqueEnd))]
        public virtual NewServicemanBase NewTechniqueEndNavigation { get; set; }
    }
}