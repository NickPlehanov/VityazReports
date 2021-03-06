// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace VityazReports.Models
{
    [Table("New_andromedaBase")]
    public partial class NewAndromedaBase
    {
        public NewAndromedaBase()
        {
            NewRentDeviceExtensionBase = new HashSet<NewRentDeviceExtensionBase>();
            NewTest2ExtensionBase = new HashSet<NewTest2ExtensionBase>();
        }

        public Guid? CreatedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedOn { get; set; }
        public int? DeletionStateCode { get; set; }
        public int? ImportSequenceNumber { get; set; }
        public Guid? ModifiedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ModifiedOn { get; set; }
        [Key]
        [Column("New_andromedaId")]
        public Guid NewAndromedaId { get; set; }
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

        [ForeignKey(nameof(OwningUser))]
        [InverseProperty(nameof(SystemUserBase.NewAndromedaBase))]
        public virtual SystemUserBase OwningUserNavigation { get; set; }
        [InverseProperty("NewAndromeda")]
        public virtual NewAndromedaExtensionBase NewAndromedaExtensionBase { get; set; }
        [InverseProperty("NewAndromedaNavigation")]
        public virtual ICollection<NewRentDeviceExtensionBase> NewRentDeviceExtensionBase { get; set; }
        [InverseProperty("NewAndromedaServiceorderNavigation")]
        public virtual ICollection<NewTest2ExtensionBase> NewTest2ExtensionBase { get; set; }
    }
}