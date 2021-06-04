using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Doppelkopf.Core.App.Config
{
    public abstract class AbstractSetting
    {
        [JsonIgnore]
        public string DisplayName { get; set; }

        public object ValueObj
        {
            get { return GetValue(); }
            set { SetValue(value); }
        }

        [JsonIgnore]
        public bool Visible { get; set; }

        //[JsonIgnore]
        //public List<(object Value, string DisplayName)> AllowedValues { get; set; }


        public abstract object GetValue();

        public abstract void SetValue(object value);

        public abstract object DisplayNameOfValue(object value);

        public abstract List<object> GetAllowedValues();

        //public string DisplayNameOfValue(object value)
        //{
        //    return AllowedValues.FirstOrDefault(v => v.Value == value).DisplayName ?? "-";
        //}
    }
}
