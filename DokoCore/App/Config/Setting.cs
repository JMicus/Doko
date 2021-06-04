using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Doppelkopf.Core.App.Config
{
    public class Setting<T> : AbstractSetting
    {


        public T Value { get; set; }

        [JsonIgnore]
        public List<(T Value, string DisplayName)> AllowedValues { get; set; }

        public Setting(T value, bool visible = false, string displayName = null, List<(T Value, string DisplayValue)> allowedValues = null)
        {
            DisplayName = displayName;
            Value = value;
            AllowedValues = allowedValues; //.Select(av => ((object)av.Value, av.DisplayValue)).ToList();
            Visible = visible;
        }

        public override object GetValue()
        {
            return Value;
        }

        public override void SetValue(object value)
        {
            Value = (T)Convert.ChangeType(value, typeof(T));
        }

        public override List<object> GetAllowedValues()
        {
             return AllowedValues.Select(av => (object)av.Value).ToList();
        }

        public override object DisplayNameOfValue(object value)
        {
            return AllowedValues.FirstOrDefault(av => av.Value.ToString() == ((T)Convert.ChangeType(value, typeof(T))).ToString()).DisplayName ?? "-";
        }
    }
}
