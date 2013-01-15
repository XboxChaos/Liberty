using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Liberty.IO;

namespace Liberty.SaveManager.Games.Blam
{
    public class HealthInfo
    {
        /// <summary>
        /// Constructs a HealthInfo object, reading strength data from a EndianReader.
        /// </summary>
        /// <param name="reader">The EndianReader to read from.</param>
        /// <param name="defaultHealthModifier">The default health modifier that this object should use</param>
        /// <param name="defaultShieldModifier">The default shield modifier that this object should use</param>
        public HealthInfo(EndianReader reader, float defaultHealthModifier, float defaultShieldModifier, float customInfinity = float.NaN)
        {
            _customInvincibility = customInfinity;
            _healthModifier = reader.ReadFloat();
            _shieldModifier = reader.ReadFloat();
            //System.Diagnostics.Debug.WriteLine("{0:F} {1:F}", _healthModifier, _shieldModifier);

            // Keep backups of the values so that invincibility can be disabled
            // However, if a value is already NaN (invincible), we have to resort to the default value that was passed in.
            if (float.IsNaN(_healthModifier) || float.IsPositiveInfinity(_healthModifier) || _healthModifier == customInfinity)
                _oldHealthModifier = defaultHealthModifier;
            else
                _oldHealthModifier = _healthModifier;

            if (float.IsNaN(_shieldModifier) || float.IsPositiveInfinity(_shieldModifier) || _shieldModifier == customInfinity)
                _oldShieldModifier = defaultShieldModifier;
            else
                _oldShieldModifier = _shieldModifier;
        }

        /// <summary>
        /// Whether or not the object has an infinite amount of health/shields.
        /// </summary>
        public bool IsInfinite
        {
            get
            {
                // If either modifier is NaN or the object has no health and no shields, it's invincible
                return (float.IsNaN(_healthModifier) || float.IsNaN(_shieldModifier) ||
                    float.IsPositiveInfinity(_healthModifier) || float.IsPositiveInfinity(_shieldModifier) ||
                    _healthModifier == _customInvincibility || _shieldModifier == _customInvincibility ||
                    (_healthModifier == 0 && _shieldModifier == 0));
            }
        }

        /// <summary>
        /// Makes the object's health/shield modifiers infinite.
        /// If disabled, then the modifiers will be restored to the values they were last set to.
        /// </summary>
        /// <param name="infinite"></param>
        public void MakeInfinite(bool infinite)
        {
            if (infinite)
            {
                if (_healthModifier != 0)
                    _healthModifier = _customInvincibility;
                if (_shieldModifier != 0)
                    _shieldModifier = _customInvincibility;
            }
            else
            {
                // Revert them to their old values
                if (_healthModifier != 0)
                    _healthModifier = _oldHealthModifier;
                if (_shieldModifier != 0)
                    _shieldModifier = _oldShieldModifier;
            }
        }

        /// <summary>
        /// Writes updated strength information back to a EndianWriter.
        /// </summary>
        /// <param name="writer">The EndianWriter to write to.</param>
        public void WriteTo(EndianWriter writer)
        {
            writer.WriteFloat(_healthModifier);
            writer.WriteFloat(_shieldModifier);
        }

        /// <summary>
        /// Whether or not health data is present.
        /// </summary>
        public bool HasHealth
        {
            get { return (_healthModifier != 0); }
        }

        /// <summary>
        /// The object's health modifier.
        /// </summary>
        public float HealthModifier
        {
            get { return _healthModifier; }
            set { _healthModifier = value; _oldHealthModifier = value; }
        }

        /// <summary>
        /// Whether or not shield data is present.
        /// </summary>
        public bool HasShields
        {
            get { return (_shieldModifier != 0); }
        }

        /// <summary>
        /// The object's shield modifier.
        /// </summary>
        public float ShieldModifier
        {
            get { return _shieldModifier; }
            set { _shieldModifier = value; _oldShieldModifier = value; }
        }

        private float _healthModifier;
        private float _shieldModifier;
        private float _oldHealthModifier;
        private float _oldShieldModifier;
        private float _customInvincibility;
    }
}
