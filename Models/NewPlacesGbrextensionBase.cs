﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace VityazReports.Models
{
    [Table("New_PlacesGBRExtensionBase")]
    public partial class NewPlacesGbrextensionBase
    {
        [Key]
        [Column("New_PlacesGBRId")]
        public Guid NewPlacesGbrid { get; set; }
        [Column("New_name")]
        [StringLength(100)]
        public string NewName { get; set; }
        [Column("New_place")]
        [StringLength(255)]
        public string NewPlace { get; set; }
        [Column("New_latitude")]
        [StringLength(100)]
        public string NewLatitude { get; set; }
        [Column("New_longitude")]
        [StringLength(100)]
        public string NewLongitude { get; set; }

        [ForeignKey(nameof(NewPlacesGbrid))]
        [InverseProperty(nameof(NewPlacesGbrbase.NewPlacesGbrextensionBase))]
        public virtual NewPlacesGbrbase NewPlacesGbr { get; set; }
    }
}