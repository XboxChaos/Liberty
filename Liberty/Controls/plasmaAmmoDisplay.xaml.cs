using System;
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
	/// Interaction logic for plasmaAmmoDisplay.xaml
	/// </summary>
	public partial class plasmaAmmoDisplay : UserControl, Util.IAmmoDisplay
	{
        private Blam.IWeapon _weapon;

		public plasmaAmmoDisplay(Blam.IWeapon weapon)
		{
			this.InitializeComponent();

            _weapon = weapon;
            Energy = _weapon.Energy;
		}

        private float Energy
        {
            get
            {
                if ((bool)cBInfinite.IsChecked)
                    return float.NegativeInfinity;
                else
                    return float.Parse(txtPlasma.Text);
            }
            set
            {
                if (float.IsNegativeInfinity(value) || float.IsPositiveInfinity(value) || float.IsNaN(value) || float.IsInfinity(value))
                {
                    txtPlasma.Text = "100";
                    cBInfinite.IsChecked = true;
                }
                else
                {
                    txtPlasma.Text = value.ToString();
                    cBInfinite.IsChecked = false;
                }
            }
        }

        private void cBInfinite_Checked(object sender, RoutedEventArgs e)
        {
            txtPlasma.IsEnabled = false;
        }

        private void cBInfinite_Unchecked(object sender, RoutedEventArgs e)
        {
            txtPlasma.IsEnabled = true;
        }

        public void Save()
        {
            _weapon.Energy = Energy;
        }
    }
}