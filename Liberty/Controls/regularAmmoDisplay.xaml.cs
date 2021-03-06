﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Liberty
{
	/// <summary>
	/// Interaction logic for weaponAmmo.xaml
	/// </summary>
    public partial class regularAmmoDisplay : UserControl, Util.IAmmoDisplay
	{
        private Blam.IWeapon _weapon;

		public regularAmmoDisplay(Blam.IWeapon weapon)
		{
			this.InitializeComponent();
            
            _weapon = weapon;
            ClipAmmo = weapon.ClipAmmo;
            UnloadedAmmo = weapon.Ammo;
		}

        private short ClipAmmo
        {
            get { return short.Parse(txtClipAmmo.Text); }
            set { txtClipAmmo.Text = value.ToString(); }
        }

        private short UnloadedAmmo
        {
            get { return short.Parse(txtAmmo.Text); }
            set { txtAmmo.Text = value.ToString(); }
        }

        private void btnMaxClipAmmo_Click(object sender, RoutedEventArgs e)
        {
            txtClipAmmo.Text = "32767";
        }

        private void btnMaxAmmo_Click(object sender, RoutedEventArgs e)
        {
            txtAmmo.Text = "32767";
        }

        public void Save()
        {
            _weapon.ClipAmmo = ClipAmmo;
            _weapon.Ammo = UnloadedAmmo;
        }

        private void ValidateWord(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox == null)
                return;

            textBox.Tag = true; // Mark as changed

            if (textBox.Text != "")
            {
                int value;
                if (int.TryParse(textBox.Text, out value))
                {
                    if (value > 32767)
                        textBox.Text = "32767";
                }
                else
                {
                    int line = textBox.Text.Length - 1;
                    textBox.Text = textBox.Text.Remove(line, 1);
                    textBox.Select(line, 0);
                }
            }
            else
            {
                textBox.Text = "0";
            }
        }
    }
}