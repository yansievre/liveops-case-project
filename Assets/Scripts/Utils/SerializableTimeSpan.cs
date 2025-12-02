using System;
using System.Globalization;
using UnityEngine;

namespace Utils
{
    [Serializable]
    public struct SerializableTimeSpan
    {
        public const string K_Format = @"d\:hh\:mm\:ss";
        [SerializeField]
        private string _value;

        public SerializableTimeSpan(TimeSpan timeSpan)
        {
            TimeSpan = timeSpan;
            _value = null;
        }
        
        public TimeSpan TimeSpan { get; private set; }
        
        public static implicit operator TimeSpan(SerializableTimeSpan serializableDateTime)
        {
            return serializableDateTime.TimeSpan;
        }

        public static implicit operator SerializableTimeSpan(TimeSpan dateTime)
        {
            return new SerializableTimeSpan(dateTime);
        }

        public override string ToString()
        {
            return TimeSpan.ToString(K_Format, CultureInfo.InvariantCulture);
        }
        
        public static TimeSpan FromString(string value)
        {
            return TimeSpan.ParseExact(value, K_Format, CultureInfo.InvariantCulture);
        }

        public void OnBeforeSerialize()
        {
            _value = ToString();
        }

        public void OnAfterDeserialize()
        {
            TimeSpan = FromString(_value);
        }
    }
}