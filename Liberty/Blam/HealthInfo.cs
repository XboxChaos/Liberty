using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Liberty.SaveIO;

namespace Liberty.Blam
{
    // TODO: Refactor the Reach backend to use this
    public class HealthInfo
    {
        /// <summary>
        /// Constructs a HealthInfo object, reading strength data from a SaveReader.
        /// </summary>
        /// <param name="reader">The SaveReader to read from.</param>
        /// <param name="defaultHealthModifier">
        /// The default health modifier that this object should use
        /// (in case the object is already invincible and needs to be made vulnerable)
        /// </param>
        /// <param name="defaultShieldModifier">
        /// The default shield modifier that this object should use
        /// (in case the object is already invincible and needs to be made vulnerable)
        /// </param>
        public HealthInfo(SaveReader reader, float defaultHealthModifier, float defaultShieldModifier)
        {
            _healthModifier = reader.ReadFloat();
            _shieldModifier = reader.ReadFloat();
            //System.Diagnostics.Debug.WriteLine("{0:F} {1:F}", _healthModifier, _shieldModifier);

            // Keep backups of the values so that invincibility can be disabled
            // However, if a value is already NaN (invincible), we have to resort to the default value that was passed in.
            if (float.IsNaN(_healthModifier))
                _oldHealthModifier = defaultHealthModifier;
            else
                _oldHealthModifier = _healthModifier;

            if (float.IsNaN(_shieldModifier))
                _oldShieldModifier = defaultShieldModifier;
            else
                _oldShieldModifier = _shieldModifier;
        }

        /// <summary>
        /// Whether or not the object is invincible.
        /// </summary>
        public bool IsInvincible
        {
            get
            {
                // If either modifier is NaN or the object has no health and no shields, it's invincible
                return (float.IsNaN(_healthModifier) || float.IsNaN(_shieldModifier) || (_healthModifier == 0 && _shieldModifier == 0));
            }
        }

        /// <summary>
        /// Changes the object's invincibility status.
        /// If invincibility is disabled, then the modifiers will be restored to the values they were last set to.
        /// </summary>
        /// <param name="invincible"></param>
        public void MakeInvincible(bool invincible)
        {
            if (invincible)
            {
                // Set the modifiers to NaN :P
                if (_healthModifier != 0)
                    _healthModifier = float.NaN;
                if (_shieldModifier != 0)
                    _shieldModifier = float.NaN;
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
        /// Writes updated strength information back to a SaveWriter.
        /// </summary>
        /// <param name="writer">The SaveWriter to write to.</param>
        public void WriteTo(SaveWriter writer)
        {
            writer.WriteFloat(_healthModifier);
            writer.WriteFloat(_shieldModifier);
        }

        /// <summary>
        /// Whether or not health data is present.
        /// Avoid setting this to NaN if possible and use Invincible if invincibility is needed.
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
        /// Avoid setting this to NaN if possible and use Invincible if invincibility is needed.
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
    }
}
