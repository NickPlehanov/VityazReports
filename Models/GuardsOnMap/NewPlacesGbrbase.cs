﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace VityazReports.Models.GuardsOnMap
{
    [Table("New_PlacesGBRBase")]
    public partial class NewPlacesGbrbase
    {
        [Key]
        [Column("New_PlacesGBRId")]
        public Guid NewPlacesGbrid { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedOn { get; set; }
        public Guid? CreatedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ModifiedOn { get; set; }
        public Guid? ModifiedBy { get; set; }
        public Guid? OwningUser { get; set; }
        public Guid? OwningBusinessUnit { get; set; }
        [Column("statecode")]
        public int Statecode { get; set; }
        [Column("statuscode")]
        public int? Statuscode { get; set; }
        public int? DeletionStateCode { get; set; }
        public byte[] VersionNumber { get; set; }
        public int? ImportSequenceNumber { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? OverriddenCreatedOn { get; set; }
        public int? TimeZoneRuleVersionNumber { get; set; }
        [Column("UTCConversionTimeZoneCode")]
        public int? UtcconversionTimeZoneCode { get; set; }

        //[ForeignKey(nameof(OwningUser))]
        //[
        //(nameof(SystemUserBase.NewPlacesGbrbase))]
        //public virtual SystemUserBase OwningUserNavigation { get; set; }
        //[InverseProperty("NewPlacesGbr")]
        //public virtual NewPlacesGbrextensionBase NewPlacesGbrextensionBase { get; set; }
    }
}