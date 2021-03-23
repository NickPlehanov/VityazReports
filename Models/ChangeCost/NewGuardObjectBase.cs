﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace VityazReports.Models.ChangeCost
{
    [Table("New_guard_objectBase")]
    public partial class NewGuardObjectBase
    {
        public Guid? CreatedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedOn { get; set; }
        public int? DeletionStateCode { get; set; }
        public int? ImportSequenceNumber { get; set; }
        public Guid? ModifiedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ModifiedOn { get; set; }
        [Key]
        [Column("New_guard_objectId")]
        public Guid NewGuardObjectId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? OverriddenCreatedOn { get; set; }
        public Guid? OwningBusinessUnit { get; set; }
        [Column("statecode")]
        public int Statecode { get; set; }
        [Column("statuscode")]
        public int? Statuscode { get; set; }
        public int? TimeZoneRuleVersionNumber { get; set; }
        [Column("UTCConversionTimeZoneCode")]
        public int? UtcconversionTimeZoneCode { get; set; }
        public byte[] VersionNumber { get; set; }
        public Guid? OwningUser { get; set; }
        public Guid? TransactionCurrencyId { get; set; }
        [Column(TypeName = "decimal(23, 10)")]
        public decimal? ExchangeRate { get; set; }

        //[ForeignKey(nameof(OwningUser))]
        //[InverseProperty(nameof(SystemUserBase.NewGuardObjectBase))]
        //public virtual SystemUserBase OwningUserNavigation { get; set; }
    }
}