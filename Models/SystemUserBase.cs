// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace VityazReports.Models
{
    [Index(nameof(ActiveDirectoryGuid), Name = "UQ_SystemUserBaseActiveDirectoryGuid", IsUnique = true)]
    public partial class SystemUserBase
    {
        public SystemUserBase()
        {
            AccountBaseOwningUserNavigation = new HashSet<AccountBase>();
            AccountBasePreferredSystemUser = new HashSet<AccountBase>();
            InverseParentSystemUser = new HashSet<SystemUserBase>();
            NewAndromedaBase = new HashSet<NewAndromedaBase>();
            NewGuardObjectBase = new HashSet<NewGuardObjectBase>();
            NewPlacesGbrbase = new HashSet<NewPlacesGbrbase>();
            NewTest2Base = new HashSet<NewTest2Base>();
        }

        [Key]
        public Guid SystemUserId { get; set; }
        public int DeletionStateCode { get; set; }
        public Guid? TerritoryId { get; set; }
        public Guid? OrganizationId { get; set; }
        public Guid BusinessUnitId { get; set; }
        public Guid? ParentSystemUserId { get; set; }
        [StringLength(50)]
        public string FirstName { get; set; }
        [StringLength(20)]
        public string Salutation { get; set; }
        [StringLength(50)]
        public string MiddleName { get; set; }
        [StringLength(50)]
        public string LastName { get; set; }
        [Column("PersonalEMailAddress")]
        [StringLength(100)]
        public string PersonalEmailAddress { get; set; }
        [StringLength(160)]
        public string FullName { get; set; }
        [StringLength(50)]
        public string NickName { get; set; }
        [StringLength(100)]
        public string Title { get; set; }
        [Column("InternalEMailAddress")]
        [StringLength(100)]
        public string InternalEmailAddress { get; set; }
        [StringLength(100)]
        public string JobTitle { get; set; }
        [Column("MobileAlertEMail")]
        [StringLength(100)]
        public string MobileAlertEmail { get; set; }
        public int? PreferredEmailCode { get; set; }
        [StringLength(50)]
        public string HomePhone { get; set; }
        [StringLength(50)]
        public string MobilePhone { get; set; }
        public int? PreferredPhoneCode { get; set; }
        public int? PreferredAddressCode { get; set; }
        [StringLength(200)]
        public string PhotoUrl { get; set; }
        [Required]
        [StringLength(255)]
        public string DomainName { get; set; }
        public int? PassportLo { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedOn { get; set; }
        public int? PassportHi { get; set; }
        [StringLength(500)]
        public string DisabledReason { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ModifiedOn { get; set; }
        public Guid? CreatedBy { get; set; }
        [StringLength(100)]
        public string EmployeeId { get; set; }
        public Guid? ModifiedBy { get; set; }
        public bool? IsDisabled { get; set; }
        [StringLength(100)]
        public string GovernmentId { get; set; }
        public byte[] VersionNumber { get; set; }
        [StringLength(100)]
        public string Skills { get; set; }
        public bool? DisplayInServiceViews { get; set; }
        public Guid? CalendarId { get; set; }
        public Guid? ActiveDirectoryGuid { get; set; }
        public bool SetupUser { get; set; }
        public Guid? SiteId { get; set; }
        [Column("WindowsLiveID")]
        [StringLength(100)]
        public string WindowsLiveId { get; set; }
        public int IncomingEmailDeliveryMethod { get; set; }
        public int OutgoingEmailDeliveryMethod { get; set; }
        public int? ImportSequenceNumber { get; set; }
        public int AccessMode { get; set; }
        public int? InviteStatusCode { get; set; }
        [Required]
        public bool? IsActiveDirectoryUser { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? OverriddenCreatedOn { get; set; }
        [Column("UTCConversionTimeZoneCode")]
        public int? UtcconversionTimeZoneCode { get; set; }
        public int? TimeZoneRuleVersionNumber { get; set; }
        [StringLength(160)]
        public string YomiFullName { get; set; }
        [StringLength(50)]
        public string YomiLastName { get; set; }
        [StringLength(50)]
        public string YomiMiddleName { get; set; }
        [StringLength(50)]
        public string YomiFirstName { get; set; }

        [ForeignKey(nameof(ParentSystemUserId))]
        [InverseProperty(nameof(SystemUserBase.InverseParentSystemUser))]
        public virtual SystemUserBase ParentSystemUser { get; set; }
        [InverseProperty(nameof(AccountBase.OwningUserNavigation))]
        public virtual ICollection<AccountBase> AccountBaseOwningUserNavigation { get; set; }
        [InverseProperty(nameof(AccountBase.PreferredSystemUser))]
        public virtual ICollection<AccountBase> AccountBasePreferredSystemUser { get; set; }
        [InverseProperty(nameof(SystemUserBase.ParentSystemUser))]
        public virtual ICollection<SystemUserBase> InverseParentSystemUser { get; set; }
        [InverseProperty("OwningUserNavigation")]
        public virtual ICollection<NewAndromedaBase> NewAndromedaBase { get; set; }
        [InverseProperty("OwningUserNavigation")]
        public virtual ICollection<NewGuardObjectBase> NewGuardObjectBase { get; set; }
        [InverseProperty("OwningUserNavigation")]
        public virtual ICollection<NewPlacesGbrbase> NewPlacesGbrbase { get; set; }
        [InverseProperty("OwningUserNavigation")]
        public virtual ICollection<NewTest2Base> NewTest2Base { get; set; }
    }
}