// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace VityazReports.Models
{
    [Keyless]
    [Table("New_alarmExtensionBase")]
    public partial class NewAlarmExtensionBase
    {
        [Column("New_alarmId")]
        public Guid NewAlarmId { get; set; }
        [Column("New_name")]
        [StringLength(300)]
        public string NewName { get; set; }
        [Column("New_onc")]
        public bool? NewOnc { get; set; }
        [Column("New_TPC")]
        public bool? NewTpc { get; set; }
        [Column("New_group")]
        public int? NewGroup { get; set; }
        [Column("New_arrival", TypeName = "datetime")]
        public DateTime? NewArrival { get; set; }
        [Column("New_cancel", TypeName = "datetime")]
        public DateTime? NewCancel { get; set; }
        [Column("New_departure", TypeName = "datetime")]
        public DateTime? NewDeparture { get; set; }
        [Column("New_andromeda_alarm")]
        public Guid? NewAndromedaAlarm { get; set; }
        [Column("New_owner")]
        public bool? NewOwner { get; set; }
        [Column("New_police")]
        public bool? NewPolice { get; set; }
        [Column("New_order")]
        public bool? NewOrder { get; set; }
        [Column("New_act")]
        public bool? NewAct { get; set; }
        [Column("New_serviceorder_alarm")]
        public Guid? NewServiceorderAlarm { get; set; }
        [Column("New_alarm_dt", TypeName = "datetime")]
        public DateTime? NewAlarmDt { get; set; }
        [Column("New_serviceorder_date", TypeName = "datetime")]
        public DateTime? NewServiceorderDate { get; set; }
        [Column("New_zone")]
        [StringLength(100)]
        public string NewZone { get; set; }
        [Column("New_ps")]
        public bool? NewPs { get; set; }
        [Column("New_taxi_call")]
        public bool? NewTaxiCall { get; set; }
        [Column("New_taxi_time", TypeName = "datetime")]
        public DateTime? NewTaxiTime { get; set; }
        [Column("New_summ")]
        public int? NewSumm { get; set; }
        [Column("New_comment")]
        public string NewComment { get; set; }
        [Column("New_alarm_reporter")]
        [StringLength(100)]
        public string NewAlarmReporter { get; set; }
        [Column("New_alarm_taxi_reporter")]
        [StringLength(100)]
        public string NewAlarmTaxiReporter { get; set; }
        [Column("New_history_button")]
        [StringLength(100)]
        public string NewHistoryButton { get; set; }

        [ForeignKey(nameof(NewAlarmId))]
        public virtual NewAlarmBase NewAlarm { get; set; }
        [ForeignKey(nameof(NewAndromedaAlarm))]
        public virtual NewAndromedaBase NewAndromedaAlarmNavigation { get; set; }
        [ForeignKey(nameof(NewServiceorderAlarm))]
        public virtual NewServiceorderBase NewServiceorderAlarmNavigation { get; set; }
    }
}