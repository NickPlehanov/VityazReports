using System;
using System.Windows.Media;

namespace VityazReports.Models.GuardObjectsOnMapGBR {
    public class ColorModel {
        public ColorModel(Brush color, bool isfree=true, string objtypeid = null) {
            Color = color ?? throw new ArgumentNullException(nameof(color));
            Isfree = isfree;
            ObjTypeId = objtypeid;
        }

        public Brush Color { get; set; }
        public bool Isfree { get; set; } = true;
        public string ObjTypeId { get; set; } = null;
    }
}
