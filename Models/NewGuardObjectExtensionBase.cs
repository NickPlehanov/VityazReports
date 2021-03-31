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
    [Table("New_guard_objectExtensionBase")]
    public partial class NewGuardObjectExtensionBase
    {
        [Column("New_address")]
        [StringLength(255)]
        public string NewAddress { get; set; }
        [Column("New_another_secagency")]
        public int? NewAnotherSecagency { get; set; }
        [Column("New_channel_eth")]
        public int? NewChannelEth { get; set; }
        [Column("New_channel_phone")]
        public int? NewChannelPhone { get; set; }
        [Column("New_channel_radio")]
        public int? NewChannelRadio { get; set; }
        [Column("New_dismount_reason")]
        public int? NewDismountReason { get; set; }
        [Column("New_dogovor_type")]
        public int? NewDogovorType { get; set; }
        [Column("New_guard_objectId")]
        public Guid NewGuardObjectId { get; set; }
        [Column("New_guard_on_reopen")]
        public bool? NewGuardOnReopen { get; set; }
        [Column("New_name")]
        [StringLength(100)]
        public string NewName { get; set; }
        [Column("New_office_owner")]
        [StringLength(100)]
        public string NewOfficeOwner { get; set; }
        [Column("New_office_rent_contract")]
        public bool? NewOfficeRentContract { get; set; }
        [Column("New_office_space", TypeName = "decimal(23, 10)")]
        public decimal? NewOfficeSpace { get; set; }
        [Column("New_office_state")]
        public int? NewOfficeState { get; set; }
        [Column("New_protection_os")]
        public bool? NewProtectionOs { get; set; }
        [Column("New_protection_ps")]
        public bool? NewProtectionPs { get; set; }
        [Column("New_protection_trs")]
        public bool? NewProtectionTrs { get; set; }
        [Column("New_siding")]
        public string NewSiding { get; set; }
        [Column("New_type")]
        public int? NewType { get; set; }
        [Column("New_weak_door")]
        public bool? NewWeakDoor { get; set; }
        [Column("New_weak_floor")]
        public bool? NewWeakFloor { get; set; }
        [Column("New_weak_gsm")]
        public bool? NewWeakGsm { get; set; }
        [Column("New_weak_hatch_at_floor")]
        public bool? NewWeakHatchAtFloor { get; set; }
        [Column("New_weak_house")]
        public bool? NewWeakHouse { get; set; }
        [Column("New_weak_limit_access")]
        public bool? NewWeakLimitAccess { get; set; }
        [Column("New_weak_pipe")]
        public bool? NewWeakPipe { get; set; }
        [Column("New_weak_place_akt")]
        public int? NewWeakPlaceAkt { get; set; }
        [Column("New_weak_vent")]
        public bool? NewWeakVent { get; set; }
        [Column("New_weak_wall_entrance")]
        public bool? NewWeakWallEntrance { get; set; }
        [Column("New_weak_wall_rooms")]
        public bool? NewWeakWallRooms { get; set; }
        [Column("New_weak_window")]
        public bool? NewWeakWindow { get; set; }
        [Column("new_curator")]
        public Guid? NewCurator { get; set; }
        [Column("new_account")]
        public Guid? NewAccount { get; set; }
        [Column("new_contact")]
        public Guid? NewContact { get; set; }
        [Column("New_freeroom")]
        public bool? NewFreeroom { get; set; }
        [Column("New_date_planned", TypeName = "datetime")]
        public DateTime? NewDatePlanned { get; set; }
        [Column("New_date_start", TypeName = "datetime")]
        public DateTime? NewDateStart { get; set; }
        [Column("New_rent")]
        public bool? NewRent { get; set; }
        [Column("New_result")]
        public int? NewResult { get; set; }
        [Column("New_inspector")]
        public Guid? NewInspector { get; set; }
        [Column("New_object_number")]
        public int? NewObjectNumber { get; set; }
        [Column("New_dismounted")]
        public bool? NewDismounted { get; set; }
        [Column("New_firstcontact")]
        [StringLength(200)]
        public string NewFirstcontact { get; set; }
        [Column("New_monthlypay")]
        [StringLength(50)]
        public string NewMonthlypay { get; set; }
        [Column("New_competitor_guard_object")]
        public Guid? NewCompetitorGuardObject { get; set; }
        [Column("New_protection_stand")]
        public bool? NewProtectionStand { get; set; }
        [Column("New_channel_gsm")]
        public int? NewChannelGsm { get; set; }
        [Column("New_task_to_meet")]
        public bool? NewTaskToMeet { get; set; }
        [Column("New_weak_longtime")]
        public bool? NewWeakLongtime { get; set; }
        [Column("New_arrive_time")]
        public int? NewArriveTime { get; set; }
        [Column("New_street_guard_object")]
        public Guid? NewStreetGuardObject { get; set; }
        [Column("New_house")]
        [StringLength(30)]
        public string NewHouse { get; set; }
        [Column("New_cost", TypeName = "decimal(23, 10)")]
        public decimal? NewCost { get; set; }
        [Column("New_service_guard_property")]
        public bool? NewServiceGuardProperty { get; set; }
        [Column("New_service_emergency_arrival")]
        public bool? NewServiceEmergencyArrival { get; set; }
        [Column("New_service_maintaince")]
        public bool? NewServiceMaintaince { get; set; }
        [Column("New_service_firealarm")]
        public bool? NewServiceFirealarm { get; set; }
        [Column("New_arrival_time_day")]
        [StringLength(10)]
        public string NewArrivalTimeDay { get; set; }
        [Column("New_arrival_time_night")]
        [StringLength(10)]
        public string NewArrivalTimeNight { get; set; }
        [Column("New_Code")]
        [StringLength(100)]
        public string NewCode { get; set; }
        [Column("New_region")]
        [StringLength(100)]
        public string NewRegion { get; set; }
        [Column("New_city")]
        [StringLength(100)]
        public string NewCity { get; set; }
        [Column("New_house_kv")]
        [StringLength(100)]
        public string NewHouseKv { get; set; }
        [Column("New_number")]
        [StringLength(100)]
        public string NewNumber { get; set; }
        [Column("New_route_number")]
        [StringLength(100)]
        public string NewRouteNumber { get; set; }
        [Column("New_service_comment")]
        public string NewServiceComment { get; set; }
        [Column("New_postmail_index")]
        [StringLength(100)]
        public string NewPostmailIndex { get; set; }
        [Column("New_unit")]
        public Guid? NewUnit { get; set; }
        [Column("New_transactioncurrencyid")]
        public Guid? NewTransactioncurrencyid { get; set; }
        [Column("New_add_object_date_datetime", TypeName = "datetime")]
        public DateTime? NewAddObjectDateDatetime { get; set; }
        [Column("New_remove_date", TypeName = "datetime")]
        public DateTime? NewRemoveDate { get; set; }
        [Column("New_abonent_cost_ps")]
        [StringLength(100)]
        public string NewAbonentCostPs { get; set; }
        [Column("New_CrmCode")]
        [StringLength(100)]
        public string NewCrmCode { get; set; }
        [Column("New_service_ps")]
        public int? NewServicePs { get; set; }
        [Column("New_date_tech_create", TypeName = "datetime")]
        public DateTime? NewDateTechCreate { get; set; }
        [Column("New_agreement_date_valid", TypeName = "datetime")]
        public DateTime? NewAgreementDateValid { get; set; }
        [Column("New_generate_task_ps")]
        public bool? NewGenerateTaskPs { get; set; }
        [Column("New_porch")]
        [StringLength(100)]
        public string NewPorch { get; set; }
        [Column("New_floor")]
        [StringLength(100)]
        public string NewFloor { get; set; }
        [Column("New_document_objectId")]
        public Guid? NewDocumentObjectId { get; set; }
        [Column("New_cert_number")]
        [StringLength(100)]
        public string NewCertNumber { get; set; }
        [Column("New_campain")]
        public Guid? NewCampain { get; set; }
        [Column("New_addr_kladr")]
        [StringLength(250)]
        public string NewAddrKladr { get; set; }
        [Column("New_addr_kladr_button")]
        [StringLength(100)]
        public string NewAddrKladrButton { get; set; }
        [Column("New_alarm_number")]
        [StringLength(15)]
        public string NewAlarmNumber { get; set; }
        [Column("New_mobile_alarm")]
        public bool? NewMobileAlarm { get; set; }
        [Column("New_kladr_addr_uncorrect")]
        public bool? NewKladrAddrUncorrect { get; set; }
        [Column("New_account_agent")]
        public Guid? NewAccountAgent { get; set; }
        [Column("New_retention")]
        public Guid? NewRetention { get; set; }
        [Column("New_retention_date", TypeName = "datetime")]
        public DateTime? NewRetentionDate { get; set; }
        [Column("New_retention_comment")]
        public string NewRetentionComment { get; set; }
        [Column("New_curator_mount")]
        public Guid? NewCuratorMount { get; set; }
        [Column("New_curator_user_mount")]
        public Guid? NewCuratorUserMount { get; set; }
        [Column("New_obj_delete_date", TypeName = "datetime")]
        public DateTime? NewObjDeleteDate { get; set; }
        [Column("New_why_another_corp")]
        public int? NewWhyAnotherCorp { get; set; }
        [Column("New_agreement_add", TypeName = "datetime")]
        public DateTime? NewAgreementAdd { get; set; }
        [Column("New_priost_date", TypeName = "datetime")]
        public DateTime? NewPriostDate { get; set; }
        [Column("New_priost_reason")]
        public int? NewPriostReason { get; set; }
        [Column("New_rast_reason")]
        public int? NewRastReason { get; set; }
        [Column("New_concurent_request")]
        public int? NewConcurentRequest { get; set; }
        [Column("New_start_date_buh", TypeName = "datetime")]
        public DateTime? NewStartDateBuh { get; set; }
        [Column("New_reaction_account")]
        public Guid? NewReactionAccount { get; set; }
        [Column("New_techservice_account")]
        public Guid? NewTechserviceAccount { get; set; }
        [Column("New_to_obl_cost", TypeName = "money")]
        public decimal? NewToOblCost { get; set; }
        [Column("new_to_obl_cost_Base", TypeName = "money")]
        public decimal? NewToOblCostBase { get; set; }
        [Column("New_new_to_obl_cost_decimal", TypeName = "decimal(23, 10)")]
        public decimal? NewNewToOblCostDecimal { get; set; }
        [Column("New_escort")]
        public bool? NewEscort { get; set; }
        [Column("New_video")]
        public bool? NewVideo { get; set; }
        [Column("New_video_service")]
        public bool? NewVideoService { get; set; }
        [Column("New_visit_date", TypeName = "datetime")]
        public DateTime? NewVisitDate { get; set; }
        [Column("New_cost_in_area", TypeName = "money")]
        public decimal? NewCostInArea { get; set; }
        [Column("new_cost_in_area_Base", TypeName = "money")]
        public decimal? NewCostInAreaBase { get; set; }
        [Column("New_name_pult")]
        [StringLength(100)]
        public string NewNamePult { get; set; }
        [Column("New_return_act")]
        public bool? NewReturnAct { get; set; }
        [Column("New_garanty")]
        public bool? NewGaranty { get; set; }
        [Column("New_is_return")]
        public bool? NewIsReturn { get; set; }
        [Column("New_export_to_1c")]
        public bool? NewExportTo1c { get; set; }
        [Column("New_class_a")]
        public bool? NewClassA { get; set; }
        [Column("New_remont_order")]
        public bool? NewRemontOrder { get; set; }
        [Column("New_os_trs_date", TypeName = "datetime")]
        public DateTime? NewOsTrsDate { get; set; }
        [Column("New_date_reorder", TypeName = "datetime")]
        public DateTime? NewDateReorder { get; set; }
        [Column("New_find_fail")]
        public string NewFindFail { get; set; }
        [Column("New_is_go_list")]
        public bool? NewIsGoList { get; set; }
        [Column("New_is_go_take")]
        public bool? NewIsGoTake { get; set; }
        [Column("New_id_go_return")]
        public bool? NewIdGoReturn { get; set; }
        [Column("New_mounter")]
        public Guid? NewMounter { get; set; }
        [Column("New_obh_start", TypeName = "datetime")]
        public DateTime? NewObhStart { get; set; }
        [Column("New_obh_end", TypeName = "datetime")]
        public DateTime? NewObhEnd { get; set; }
        [Column("New_in_pco")]
        public bool? NewInPco { get; set; }
        [Column("New_obhod_datetime", TypeName = "datetime")]
        public DateTime? NewObhodDatetime { get; set; }
        [Column("New_obj_in")]
        public int? NewObjIn { get; set; }
        [Column("New_kv_in")]
        public int? NewKvIn { get; set; }
        [Column("New_keep")]
        public bool? NewKeep { get; set; }
        [Column("New_key_end_date", TypeName = "datetime")]
        public DateTime? NewKeyEndDate { get; set; }
        [Column("New_key_back_date", TypeName = "datetime")]
        public DateTime? NewKeyBackDate { get; set; }
        [Column("New_key_porch")]
        public bool? NewKeyPorch { get; set; }
        [Column("New_porch_end_date", TypeName = "datetime")]
        public DateTime? NewPorchEndDate { get; set; }
        [Column("New_porch_back_date", TypeName = "datetime")]
        public DateTime? NewPorchBackDate { get; set; }
        [Column("New_carman")]
        public bool? NewCarman { get; set; }
        [Column("New_carman_end_date", TypeName = "datetime")]
        public DateTime? NewCarmanEndDate { get; set; }
        [Column("New_carman_back_date", TypeName = "datetime")]
        public DateTime? NewCarmanBackDate { get; set; }
        [Column("New_to_ps_date", TypeName = "datetime")]
        public DateTime? NewToPsDate { get; set; }
        [Column("New_escort_type")]
        public bool? NewEscortType { get; set; }
        [Column("New_abonent_cost_service")]
        [StringLength(100)]
        public string NewAbonentCostService { get; set; }
        [Column("New_to_r_date", TypeName = "datetime")]
        public DateTime? NewToRDate { get; set; }
        [Column("New_vip")]
        public int? NewVip { get; set; }
        [Column("New_status_date", TypeName = "datetime")]
        public DateTime? NewStatusDate { get; set; }
        [Column("New_date_start_report", TypeName = "datetime")]
        public DateTime? NewDateStartReport { get; set; }
        [Column("New_correct_summ", TypeName = "decimal(23, 10)")]
        public decimal? NewCorrectSumm { get; set; }
        [Column("New_abonent_cost_post")]
        [StringLength(100)]
        public string NewAbonentCostPost { get; set; }
        [Column("New_abonent_cost_escort")]
        [StringLength(100)]
        public string NewAbonentCostEscort { get; set; }
        [Column("New_device_is_sell")]
        public bool? NewDeviceIsSell { get; set; }
        [Column("New_call_date", TypeName = "datetime")]
        public DateTime? NewCallDate { get; set; }
        [Column("New_call_comment")]
        [StringLength(500)]
        public string NewCallComment { get; set; }
        [Column("New_object_procent")]
        public int? NewObjectProcent { get; set; }
        [Column("New_object_category")]
        public int? NewObjectCategory { get; set; }
        [Column("New_send_acts")]
        public int? NewSendActs { get; set; }
        [Column("New_poryadok_akt")]
        public int? NewPoryadokAkt { get; set; }
        [Column("New_call_outgone")]
        public bool? NewCallOutgone { get; set; }
        [Column("New_result_call_outgone")]
        public int? NewResultCallOutgone { get; set; }
        [Column("New_comment_call_outgone")]
        public string NewCommentCallOutgone { get; set; }
        [Column("New_date_call_outgone", TypeName = "datetime")]
        public DateTime? NewDateCallOutgone { get; set; }
        [Column("New_100")]
        public bool? New100 { get; set; }
        [Column("New_brand")]
        public Guid? NewBrand { get; set; }
        [Column("New_number_notification")]
        [StringLength(100)]
        public string NewNumberNotification { get; set; }
        [Column("New_date_notification", TypeName = "datetime")]
        public DateTime? NewDateNotification { get; set; }
        [Column("New_last_export_date", TypeName = "datetime")]
        public DateTime? NewLastExportDate { get; set; }
        [Column("New_uvo_unit")]
        public Guid? NewUvoUnit { get; set; }
        [Column("New_history_button")]
        [StringLength(100)]
        public string NewHistoryButton { get; set; }
        [Column("New_rr_ps")]
        public bool? NewRrPs { get; set; }
        [Column("New_rr_video")]
        public bool? NewRrVideo { get; set; }
        [Column("New_rr_skud")]
        public bool? NewRrSkud { get; set; }
        [Column("New_rr_on_off")]
        public bool? NewRrOnOff { get; set; }
        [Column("New_rr_os")]
        public bool? NewRrOs { get; set; }
        [Column("New_copy_guard")]
        [StringLength(100)]
        public string NewCopyGuard { get; set; }
        [Column("New_button_open_photo")]
        [StringLength(100)]
        public string NewButtonOpenPhoto { get; set; }
        [Column("New_button_add_photo")]
        [StringLength(100)]
        public string NewButtonAddPhoto { get; set; }
        [Column("New_is_check")]
        public bool? NewIsCheck { get; set; }
        [Column("New_andromeda_link")]
        [StringLength(150)]
        public string NewAndromedaLink { get; set; }
        [Column("New_check_date", TypeName = "datetime")]
        public DateTime? NewCheckDate { get; set; }
        [Column("New_check_comment")]
        public string NewCheckComment { get; set; }
        [Column("New_is_uvedom")]
        public bool? NewIsUvedom { get; set; }
        [Column("New_check_1c_objnumber")]
        public bool? NewCheck1cObjnumber { get; set; }
        [Column("New_check_1c_abonentcost")]
        public bool? NewCheck1cAbonentcost { get; set; }
        [Column("New_1CCode")]
        [StringLength(100)]
        public string New1ccode { get; set; }
        [Column("New_pravo_doc_pom")]
        public bool? NewPravoDocPom { get; set; }
        [Column("New_uchredit_doc")]
        public bool? NewUchreditDoc { get; set; }
        [Column("New_andromeda_link_button")]
        [StringLength(100)]
        public string NewAndromedaLinkButton { get; set; }
        [Column("New_rr_video_period")]
        public bool? NewRrVideoPeriod { get; set; }
        [Column("New_rr_os_period")]
        public bool? NewRrOsPeriod { get; set; }
        [Column("New_rr_ps_period")]
        public int? NewRrPsPeriod { get; set; }
        [Column("New_rr_skud_period")]
        public int? NewRrSkudPeriod { get; set; }
        [Column("New_rr_video_period_1")]
        public int? NewRrVideoPeriod1 { get; set; }
        [Column("New_rr_os_period_1")]
        public int? NewRrOsPeriod1 { get; set; }
        [Column("New_result_to_ps")]
        public int? NewResultToPs { get; set; }

        [ForeignKey(nameof(NewCuratorMount))]
        public virtual NewGuardObjectBase NewCuratorMountNavigation { get; set; }
        [ForeignKey(nameof(NewCurator))]
        public virtual SystemUserBase NewCuratorNavigation { get; set; }
        [ForeignKey(nameof(NewCuratorUserMount))]
        public virtual SystemUserBase NewCuratorUserMountNavigation { get; set; }
        [ForeignKey(nameof(NewGuardObjectId))]
        public virtual NewGuardObjectBase NewGuardObject { get; set; }
        [ForeignKey(nameof(NewInspector))]
        public virtual SystemUserBase NewInspectorNavigation { get; set; }
        [ForeignKey(nameof(NewRetention))]
        public virtual SystemUserBase NewRetentionNavigation { get; set; }
    }
}