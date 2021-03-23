using System;
using System.Collections.Generic;
using System.Text;
using VityazReports.Models;

namespace VityazReports.Helpers {
    public class Compare {
        public List<Comparator> CompareObject(NewGuardObjectHistory _old, NewGuardObjectHistory _new) {
            List<Comparator> comparator = new List<Comparator>();
            if (_old == null || _new == null)
                return null;
            else {
                foreach (var item in _old.GetType().GetProperties()) {
                    object oldValue = _old.GetType().GetProperty(item.Name).GetValue(_old);
                    object newValue = _new.GetType().GetProperty(item.Name).GetValue(_new);
                    if (oldValue != null)
                        if (oldValue.Equals(newValue))
                            continue;
                        else
                                //foreach(var property in _old.GetType().GetProperties()) {
                                if (_old.GetType().GetProperty(item.Name).GetValue(_old) != null && _new.GetType().GetProperty(item.Name).GetValue(_new) != null)
                            if (!_old.GetType().GetProperty(item.Name).GetValue(_old).Equals(_new.GetType().GetProperty(item.Name).GetValue(_new))) {
                                comparator.Add(new Comparator() {
                                    FieldName = item.Name,
                                    OldValue = (_old.GetType().GetProperty(item.Name).GetValue(_old) ?? ""),
                                    NewValue = (_new.GetType().GetProperty(item.Name).GetValue(_new) ?? ""),

                                });
                                //}
                            }
                            //if(item.Name.ToString().Equals("NewMonthlypay"))
                            //	comparator.Add(new Comparator() {
                            //		FieldName = item.Name,
                            //		OldValue = _old.GetType().GetProperty(item.Name).GetValue(_old) == null ? null : _old.GetType().GetProperty(item.Name).GetValue(_old).ToString(),
                            //		NewValue = _new.GetType().GetProperty(item.Name).GetValue(_new) == null ? null : _new.GetType().GetProperty(item.Name).GetValue(_new).ToString()
                            //	});
                            else
                                continue;
                }
                return comparator;
            }
        }
    }
    public class Comparator {
        public string FieldName { get; set; }
        public object OldValue { get; set; }
        public object NewValue { get; set; }

        public string _FiledName {
            get {
                switch (FieldName) {
                    case "New_rr_on_off": return "Ежемес. рег. работы";
                    case "New_rr_os": return "ОС рег. работы";
                    case "New_rr_ps": return "ПС рег. работы";
                    case "New_rr_video": return "Видео рег. работы";
                    case "New_rr_skud": return "СКУД рег. работы";
                    default: return "";
                }
            }
        }
    }
}
