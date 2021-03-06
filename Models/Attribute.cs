// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace VityazReports.Models
{
    [Table("Attribute", Schema = "MetadataSchema")]
    [Index(nameof(AttributeTypeId), Name = "XIF12Attribute")]
    [Index(nameof(EntityId), Name = "XIF13Attribute")]
    [Index(nameof(AttributeOf), Name = "XIF54Attribute")]
    [Index(nameof(AggregateOf), Name = "XIF55Attribute")]
    [Index(nameof(EntityId), nameof(Name), Name = "attr_column_name")]
    [Index(nameof(EntityId), nameof(ColumnNumber), Name = "attr_column_number")]
    [Index(nameof(EntityId), nameof(PhysicalName), Name = "ndx_Attribute_PhysicalName")]
    [Index(nameof(LogicalName), Name = "ndx_attr_logicalname")]
    [Index(nameof(Name), Name = "ndx_attr_name")]
    [Index(nameof(PhysicalName), Name = "ndx_attr_physicalname")]
    public partial class Attribute
    {
        [Key]
        public Guid AttributeId { get; set; }
        public Guid? AttributeTypeId { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
        [StringLength(50)]
        public string PhysicalName { get; set; }
        public int? Length { get; set; }
        public bool? IsNullable { get; set; }
        [StringLength(50)]
        public string XmlAbbreviation { get; set; }
        public Guid EntityId { get; set; }
        [StringLength(100)]
        public string DefaultValue { get; set; }
        public int? ColumnNumber { get; set; }
        [Column("ValidForUpdateAPI")]
        public bool? ValidForUpdateApi { get; set; }
        [Required]
        [StringLength(50)]
        public string LogicalName { get; set; }
        [Column("ValidForReadAPI")]
        public bool? ValidForReadApi { get; set; }
        [Column("ValidForCreateAPI")]
        public bool? ValidForCreateApi { get; set; }
        public bool? VisibleToPlatform { get; set; }
        [Column("IsPKAttribute")]
        public bool? IsPkattribute { get; set; }
        public bool? IsCustomField { get; set; }
        public bool IsLogical { get; set; }
        public int? DisplayMask { get; set; }
        public Guid? AttributeOf { get; set; }
        public int ReferencedEntityObjectTypeCode { get; set; }
        public Guid? AggregateOf { get; set; }
        public bool IsSortAttribute { get; set; }
        public byte? PrecisionValue { get; set; }
        public byte? PrecisionSource { get; set; }
        public bool IsIdentity { get; set; }
        [Required]
        public bool? IsReplicated { get; set; }
        [Required]
        public byte[] VersionNumber { get; set; }
        public Guid? YomiOf { get; set; }
        public Guid AttributeRowId { get; set; }
        public int? AppDefaultValue { get; set; }
        [StringLength(50)]
        public string AttributeLogicalTypeId { get; set; }
        public bool Locked { get; set; }
        [StringLength(50)]
        public string AttributeImeModeId { get; set; }
        [StringLength(50)]
        public string AttributeRequiredLevelId { get; set; }
        public int? MaxLength { get; set; }
        public double? MinValue { get; set; }
        public double? MaxValue { get; set; }
        public int? Accuracy { get; set; }
        public int? AccuracySource { get; set; }
        [StringLength(50)]
        public string LookupClass { get; set; }
        [StringLength(50)]
        public string LookupStyle { get; set; }
        public bool? LookupBrowse { get; set; }
        [StringLength(50)]
        public string ImeMode { get; set; }
        [Key]
        public bool? InProduction { get; set; }
        [Key]
        public byte CustomizationLevel { get; set; }
        public bool HasMultipleLabels { get; set; }
        public bool IsRowGuidAttribute { get; set; }
        public bool? IsBaseCurrency { get; set; }
        public Guid? CalculationOf { get; set; }
        public bool? IsDeprecated { get; set; }
    }
}