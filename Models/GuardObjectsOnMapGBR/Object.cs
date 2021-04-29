﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace VityazReports.Models.GuardObjectsOnMapGBR
{
    [Index(nameof(CtuseCommon), nameof(RecordDeleted), nameof(ControlTime), Name = "IX_Object_CTUseCommon_RecordDeleted_ControlTime")]
    [Index(nameof(Contract), nameof(ObjectId), Name = "IX_Object_Contract_ObjectID")]
    [Index(nameof(ControlTime), nameof(CtuseCommon), nameof(RecordDeleted), Name = "IX_Object_ControlTime")]
    [Index(nameof(EventTemplateId), Name = "IX_Object_EventTemplateID")]
    [Index(nameof(ObjectId), Name = "IX_Object_GUID", IsUnique = true)]
    [Index(nameof(ObjectNumber), nameof(RecordDeleted), Name = "IX_Object_ObjectNumber_RecordDeleted")]
    [Index(nameof(RecordDeleted), Name = "IX_Object_RecordDeleted")]
    public partial class Object
    {
        [Key]
        [Column("ObjectID")]
        public int ObjectId { get; set; }
        public int ObjectNumber { get; set; }
        [Column("EventTemplateID")]
        public int EventTemplateId { get; set; }
        [Column("ObjTypeID")]
        public int ObjTypeId { get; set; }
        [Required]
        [StringLength(128)]
        public string Name { get; set; }
        [Required]
        [StringLength(25)]
        public string Contract { get; set; }
        [Required]
        [StringLength(128)]
        public string ObjectPassword { get; set; }
        [Required]
        [StringLength(128)]
        public string Phone1 { get; set; }
        [Required]
        [StringLength(128)]
        public string Phone2 { get; set; }
        [Required]
        [StringLength(128)]
        public string Phone3 { get; set; }
        [Required]
        [StringLength(255)]
        public string Address { get; set; }
        [Column("ArmSchedule_EarlyArm")]
        public bool ArmScheduleEarlyArm { get; set; }
        [Column("ArmSchedule_ControlArm")]
        public bool ArmScheduleControlArm { get; set; }
        [Column("ArmSchedule_LaterArm")]
        public bool ArmScheduleLaterArm { get; set; }
        [Column("ArmSchedule_EarlyDisarm")]
        public bool ArmScheduleEarlyDisarm { get; set; }
        [Column("ArmSchedule_ControlDisarm")]
        public bool ArmScheduleControlDisarm { get; set; }
        [Column("ArmSchedule_LaterDisarm")]
        public bool ArmScheduleLaterDisarm { get; set; }
        [Column("ArmSchedule_Deviation")]
        public int ArmScheduleDeviation { get; set; }
        [Column("LSchedEnable")]
        public bool LschedEnable { get; set; }
        [Column("LSchedStart", TypeName = "datetime")]
        public DateTime LschedStart { get; set; }
        [Column("LSchedStop", TypeName = "datetime")]
        public DateTime LschedStop { get; set; }
        public bool Disable { get; set; }
        [Column("DisableDT", TypeName = "datetime")]
        public DateTime DisableDt { get; set; }
        public bool AutoEnable { get; set; }
        [Column("AutoEnableDT", TypeName = "datetime")]
        public DateTime AutoEnableDt { get; set; }
        public int ControlTime { get; set; }
        public bool ManualDisarm { get; set; }
        [StringLength(255)]
        public string MapFileName { get; set; }
        [StringLength(255)]
        public string WebLink { get; set; }
        public bool IsArm { get; set; }
        public bool IsFire { get; set; }
        public bool IsPanic { get; set; }
        [Required]
        [Column("CTUseCommon")]
        public bool? CtuseCommon { get; set; }
        [Required]
        [Column("CTIgnoreSystemEvent")]
        public bool? CtignoreSystemEvent { get; set; }
        [Required]
        public bool? IsTestFilter { get; set; }
        [Required]
        public bool? IsDoubleFilter { get; set; }
        [Column("IsUseEPAF")]
        public bool IsUseEpaf { get; set; }
        [Column("DeviceTypeID")]
        public int? DeviceTypeId { get; set; }
        public bool? IsRadioChannel { get; set; }
        public bool? IsPhoneChannel { get; set; }
        public bool? IsEthernetChannel { get; set; }
        [Column("IsGsmDTMFChannel")]
        public bool? IsGsmDtmfchannel { get; set; }
        [Column("IsGsmGPRSChannel")]
        public bool? IsGsmGprschannel { get; set; }
        [Column("IsGsmCSDChannel")]
        public bool? IsGsmCsdchannel { get; set; }
        [Column("RadioTransID")]
        public int? RadioTransId { get; set; }
        [StringLength(25)]
        public string ActionNumber { get; set; }
        [StringLength(128)]
        public string ChannelPhonePhone { get; set; }
        [Column("ChannelEthernetProviderID")]
        public int? ChannelEthernetProviderId { get; set; }
        [Column("ChannelGsmOperator1ID")]
        public int? ChannelGsmOperator1Id { get; set; }
        [Column("ChannelGsmOperator2ID")]
        public int? ChannelGsmOperator2Id { get; set; }
        [StringLength(128)]
        public string ChannelGsmPhone1 { get; set; }
        [StringLength(128)]
        public string ChannelGsmPhone2 { get; set; }
        [Column("DeviceSIM1")]
        [StringLength(128)]
        public string DeviceSim1 { get; set; }
        [Column("DeviceSIM2")]
        [StringLength(128)]
        public string DeviceSim2 { get; set; }
        public int? WarnSignalLevel { get; set; }
        public int? AlarmSignalLevel { get; set; }
        [Column("XCoord")]
        public int? Xcoord { get; set; }
        [Column("YCoord")]
        public int? Ycoord { get; set; }
        public bool? CoordVisible { get; set; }
        [Column("MountingCompanyID")]
        public int? MountingCompanyId { get; set; }
        [Column("SecurityCompanyID")]
        public int? SecurityCompanyId { get; set; }
        [Column("ServiceCompanyID")]
        public int? ServiceCompanyId { get; set; }
        [Column("MonitoringStateID")]
        public int? MonitoringStateId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? StartMonitoringDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? StopMonitoringDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? StateChangeDate { get; set; }
        [Column("SecurityDogovorID")]
        public int? SecurityDogovorId { get; set; }
        [Column("ReactionDogovorID")]
        public int? ReactionDogovorId { get; set; }
        [Column("PSecurityCompanyID")]
        public int? PsecurityCompanyId { get; set; }
        [Column("ReactionCompanyID")]
        public int? ReactionCompanyId { get; set; }
        [Column("MountingPersonID")]
        public int? MountingPersonId { get; set; }
        [Column("PServiceCompanyID")]
        public int? PserviceCompanyId { get; set; }
        [Column("TariffID")]
        public int? TariffId { get; set; }
        [Column(TypeName = "money")]
        public decimal? Price { get; set; }
        [Column("SecurityStateID")]
        public int? SecurityStateId { get; set; }
        [Column("GUID")]
        public Guid Guid { get; set; }
        public bool? IsManCaused { get; set; }
        public bool? IsAccessControl { get; set; }
        public bool? IsServiceAvailable { get; set; }
        public bool? IsVitalFunction { get; set; }
        [StringLength(4000)]
        public string CommentForOperator { get; set; }
        [StringLength(4000)]
        public string CommentForGuard { get; set; }
        [StringLength(4000)]
        public string CustomersComment { get; set; }
        public bool RecordDeleted { get; set; }
        [Column("latitude")]
        public double? Latitude { get; set; }
        [Column("longitude")]
        public double? Longitude { get; set; }
        [Column("TransmitterID")]
        [StringLength(24)]
        public string TransmitterId { get; set; }
        [Column("JupiterDeviceID")]
        [StringLength(25)]
        public string JupiterDeviceId { get; set; }
        [StringLength(128)]
        public string JupiterCypherKey { get; set; }
        [StringLength(4000)]
        public string JupiterParams { get; set; }
        [Column("ObjAdminID")]
        public int? ObjAdminId { get; set; }
        [StringLength(25)]
        public string JupiterArmDisarmPassword { get; set; }
        [Column("DeviceCSN")]
        public int? DeviceCsn { get; set; }
        public int? DeviceArmAllowed { get; set; }
        [Column("DeviceHWVersion")]
        public int? DeviceHwversion { get; set; }
        [Column("DeviceSWVersion")]
        public int? DeviceSwversion { get; set; }
        [Column(TypeName = "money")]
        public decimal? MoneyBalance { get; set; }
        public int? DisableReason { get; set; }
        [Column(TypeName = "money")]
        public decimal? ContractPrice { get; set; }
        [Column("RegionID")]
        public int RegionId { get; set; }
        [Column("DeviceIMEI")]
        [StringLength(25)]
        public string DeviceImei { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? PaymentDate { get; set; }
        [Column("IsSecureATM")]
        public bool? IsSecureAtm { get; set; }
        [NotMapped]
        public string ObjTypeName { get; set; }
    }
}