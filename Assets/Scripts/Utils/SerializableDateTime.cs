using System;
using System.Globalization;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Utils
{
    [Serializable]
    public struct SerializableDateTime : ISerializationCallbackReceiver
    {
        public const string K_Format = "dd:MM:yyyy HH:mm:ss zzz";
        [SerializeField]
        private string _value;

        public SerializableDateTime(DateTime dateTime)
        {
            DateTime = dateTime;
            _value = null;
        }
        
        public DateTime DateTime { get; private set; }
        
        public static implicit operator DateTime(SerializableDateTime serializableDateTime)
        {
            return serializableDateTime.DateTime;
        }

        public static implicit operator SerializableDateTime(DateTime dateTime)
        {
            return new SerializableDateTime(dateTime);
        }

        public override string ToString()
        {
            return DateTime.ToUniversalTime().ToString(K_Format, CultureInfo.InvariantCulture);
        }
        
        public static DateTime FromString(string value)
        {
            return DateTime.ParseExact(value, K_Format, CultureInfo.InvariantCulture);
        }

        public void OnBeforeSerialize()
        {
            _value = ToString();
        }

        public void OnAfterDeserialize()
        {
            DateTime = FromString(_value);
        }
    }
}