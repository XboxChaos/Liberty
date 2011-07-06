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
using System.Windows.Shapes;

namespace Liberty.Controls
{
	/// <summary>
	/// Interaction logic for massObjectMove.xaml
	/// </summary>
	public partial class massObjectMove : Window
	{
		public massObjectMove()
		{
			this.InitializeComponent();

            lblSubHeader1.Text = lblSubHeader1.Text.Replace("{0}", classInfo.loadPackageData.convertClassToString(classInfo.storage.fileInfoStorage.massCordMoveType));
		}
		
		#region textValidation
        private void txtPlayerXCord_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (txtPlayerXCord.Text == "") { }
            else
            {
                try
                {
                    float validate = float.Parse(txtPlayerXCord.Text);
                }
                catch
                {
                    int line = txtPlayerXCord.Text.Length - 1;
                    txtPlayerXCord.Text = txtPlayerXCord.Text.Remove(line, 1);
                    txtPlayerXCord.Select(line, 0);
                }
            }

            if (txtPlayerXCord.Text == "") { txtPlayerXCord.Text = "0"; }
        }

        private void txtPlayerYCord_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (txtPlayerYCord.Text == "") { }
            else
            {
                try
                {
                    float validate = float.Parse(txtPlayerYCord.Text);
                }
                catch
                {
                    int line = txtPlayerYCord.Text.Length - 1;
                    txtPlayerYCord.Text = txtPlayerYCord.Text.Remove(line, 1);
                    txtPlayerYCord.Select(line, 0);
                }
            }

            if (txtPlayerYCord.Text == "") { txtPlayerYCord.Text = "0"; }
        }

        private void txtPlayerZCord_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (txtPlayerZCord.Text == "") { }
            else
            {
                try
                {
                    float validate = float.Parse(txtPlayerZCord.Text);
                }
                catch
                {
                    int line = txtPlayerZCord.Text.Length - 1;
                    txtPlayerZCord.Text = txtPlayerZCord.Text.Remove(line, 1);
                    txtPlayerZCord.Select(line, 0);
                }
            }

            if (txtPlayerZCord.Text == "") { txtPlayerZCord.Text = "0"; }
        }
        #endregion
		
		#region wpfBullshit
        #region btnOKwpf
        private void btnOK_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                var source = new Uri(@"/Liberty;component/Images/Button-onhover.png", UriKind.Relative);
                btnOK.Source = new BitmapImage(source);
            }
            else
            {
                var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
                btnOK.Source = new BitmapImage(source);
            }
        }

        private void btnOK_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
            btnOK.Source = new BitmapImage(source);
        }

        private void btnOK_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
            btnOK.Source = new BitmapImage(source);

            FormFadeOut.Begin();

            float _x = Convert.ToSingle(txtPlayerXCord.Text);
            float _y = Convert.ToSingle(txtPlayerYCord.Text);
            float _z = Convert.ToSingle(txtPlayerZCord.Text);

            classInfo.storage.fileInfoStorage._massCordX = _x;
            classInfo.storage.fileInfoStorage._massCordY = _y;
            classInfo.storage.fileInfoStorage._massCordZ = _z;

            int x = classInfo.storage.fileInfoStorage.saveData.Objects.Count;
            int[] objectNoNull = new int[x];
            int ___x = 0;
            int ____x = 0;

            if (classInfo.loadPackageData.convertClassToString(classInfo.storage.fileInfoStorage.massCordMoveType) == "bipd")
            {
                foreach (Reach.BipedObject obj in classInfo.storage.fileInfoStorage.saveData.Objects)
                {
                    if (obj != null && !obj.Deleted)
                    {
                        if (obj.TagGroup == classInfo.storage.fileInfoStorage.massCordMoveType)
                        {
                            objectNoNull[___x] = ____x;
                            ___x++;
                        }
                    }
                    ____x++;
                }
            }
            else if (classInfo.loadPackageData.convertClassToString(classInfo.storage.fileInfoStorage.massCordMoveType) == "weap")
            {
                foreach (Reach.WeaponObject obj in classInfo.storage.fileInfoStorage.saveData.Objects)
                {
                    if (obj != null && !obj.Deleted)
                    {
                        if (obj.TagGroup == classInfo.storage.fileInfoStorage.massCordMoveType)
                        {
                            objectNoNull[___x] = ____x;
                            ___x++;
                        }
                    }
                    ____x++;
                }
            }
            else
            {
                foreach (Reach.GameObject obj in classInfo.storage.fileInfoStorage.saveData.Objects)
                {
                    if (obj != null && !obj.Deleted)
                    {
                        if (obj.TagGroup == classInfo.storage.fileInfoStorage.massCordMoveType)
                        {
                            objectNoNull[___x] = ____x;
                            ___x++;
                        }
                    }
                    ____x++;
                }
            }

            if ((bool)cBAdvancedAlgo.IsChecked)
            {
                int __x = ___x / 2;

                _x = _x - __x - (float)0.2;
                _y = _y - __x - (float)0.2;

                int __z = 0;
                for (int k = 0; k < ___x; k++)
                {
                    for (int i = 0; i < __x; i++)
                    {
                        classInfo.storage.fileInfoStorage.saveData.Objects[objectNoNull[__z]].X = _x;
                        classInfo.storage.fileInfoStorage.saveData.Objects[objectNoNull[__z]].Y = _y;
                        classInfo.storage.fileInfoStorage.saveData.Objects[objectNoNull[__z]].Z = _z;
                        _x = _x + (float)0.2;
                        _y = _y + (float)0.2;
                        _y++;

                        __z++;
                        for (int j = 0; j < (__x / 2); j++)
                        {
                            classInfo.storage.fileInfoStorage.saveData.Objects[objectNoNull[__z]].X = _x;
                            classInfo.storage.fileInfoStorage.saveData.Objects[objectNoNull[__z]].Y = _y;
                            classInfo.storage.fileInfoStorage.saveData.Objects[objectNoNull[__z]].Z = _z;
                            _x = _x + (float)0.2;
                            _y = _y + (float)0.2;

                            __z++;
                        }
                    }
                    k++;
                }
            }
            else
            {
                int __z = 0;
                for (int k = 0; k < ___x; k++)
                {
                    classInfo.storage.fileInfoStorage.saveData.Objects[objectNoNull[__z]].X = _x;
                    classInfo.storage.fileInfoStorage.saveData.Objects[objectNoNull[__z]].Y = _y;
                    classInfo.storage.fileInfoStorage.saveData.Objects[objectNoNull[__z]].Z = _z;
                    __z++;
                    k++;
                }
            }
        }
        #endregion

        #region btnUpdatewpf
        private void btnUpdate_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                var source = new Uri(@"/Liberty;component/Images/Button-onhover.png", UriKind.Relative);
                btnUpdate.Source = new BitmapImage(source);
            }
            else
            {
                var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
                btnUpdate.Source = new BitmapImage(source);
            }
        }

        private void btnUpdate_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
            btnUpdate.Source = new BitmapImage(source);
        }

        private void btnUpdate_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
            btnUpdate.Source = new BitmapImage(source);

            FormFadeOut.Begin();
        }
        #endregion

        private void FormFadeOut_Completed(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion
	}
}