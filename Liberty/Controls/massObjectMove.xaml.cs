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
        private Util.SaveEditor _saveEditor = null;
        private float _moveX = 0, _moveY = 0, _moveZ = 0;
        private bool _result = false;
        private Reach.TagGroup _tagGroup;

		public massObjectMove(Util.SaveEditor saveEditor, Reach.TagGroup tagGroup, string tagGroupName)
		{
			this.InitializeComponent();

            _saveEditor = saveEditor;
            _tagGroup = tagGroup;
            lblSubHeader1.Text = lblSubHeader1.Text.Replace("{0}", tagGroupName);

            if (classInfo.storage.settings.applicationSettings.noWarnings)
            {
                Thickness newMargin = coordBoxes.Margin;
                newMargin.Top -= Math.Round(lblWarning.Height / 2);
                coordBoxes.Margin = newMargin;

                lblWarning.Visibility = Visibility.Hidden;
            }
		}

        public float moveX
        {
            get { return _moveX; }
        }

        public float moveY
        {
            get { return _moveY; }
        }

        public float moveZ
        {
            get { return _moveZ; }
        }

        public bool result
        {
            get { return _result; }
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
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            float _x = Convert.ToSingle(txtPlayerXCord.Text);
            float _y = Convert.ToSingle(txtPlayerYCord.Text);
            float _z = Convert.ToSingle(txtPlayerZCord.Text);

            _moveX = _x;
            _moveY = _y;
            _moveZ = _z;

            int x = _saveEditor.Objects.Count;
            int[] objectNoNull = new int[x];
            int ___x = 0;
            int ____x = 0;

            // Xerax: I don't understand the point of these if statements; they all look the same.
            // -- AMD
            /*if (classInfo.loadPackageData.convertClassToString(classInfo.storage.fileInfoStorage.massCordMoveType) == "bipd")
            {
                foreach (Reach.GameObject obj in _saveData.Objects)
                {
                    Reach.BipedObject biped = obj as Reach.BipedObject;
                    if (biped != null && !biped.Deleted)
                    {
                        if (biped.TagGroup == classInfo.storage.fileInfoStorage.massCordMoveType)
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
                foreach (Reach.GameObject obj in _saveData.Objects)
                {
                    Reach.WeaponObject weap = obj as Reach.WeaponObject;
                    if (weap != null && !weap.Deleted)
                    {
                        if (weap.TagGroup == classInfo.storage.fileInfoStorage.massCordMoveType)
                        {
                            objectNoNull[___x] = ____x;
                            ___x++;
                        }
                    }
                    ____x++;
                }
            }
            else if (classInfo.loadPackageData.convertClassToString(classInfo.storage.fileInfoStorage.massCordMoveType) == "vehi")
            {
                foreach (Reach.GameObject obj in _saveData.Objects)
                {
                    Reach.VehicleObject vehi = obj as Reach.VehicleObject;
                    if (vehi != null && !vehi.Deleted)
                    {
                        if (vehi.TagGroup == classInfo.storage.fileInfoStorage.massCordMoveType)
                        {
                            objectNoNull[___x] = ____x;
                            ___x++;
                        }
                    }
                    ____x++;
                }
            }
            else*/
            {
                foreach (Reach.GameObject obj in _saveEditor.Objects)
                {
                    if (obj != null && !obj.Deleted)
                    {
                        if (obj.TagGroup == _tagGroup)
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
                        _saveEditor.Objects[objectNoNull[__z]].X = _x;
                        _saveEditor.Objects[objectNoNull[__z]].Y = _y;
                        _saveEditor.Objects[objectNoNull[__z]].Z = _z;
                        _x = _x + (float)0.2;
                        _y = _y + (float)0.2;
                        _y++;

                        __z++;
                        for (int j = 0; j < (__x / 2); j++)
                        {
                            _saveEditor.Objects[objectNoNull[__z]].X = _x;
                            _saveEditor.Objects[objectNoNull[__z]].Y = _y;
                            _saveEditor.Objects[objectNoNull[__z]].Z = _z;
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
                    _saveEditor.Objects[objectNoNull[__z]].X = _x;
                    _saveEditor.Objects[objectNoNull[__z]].Y = _y;
                    _saveEditor.Objects[objectNoNull[__z]].Z = _z;
                    __z++;
                    k++;
                }
            }

            _result = true;
            FormFadeOut.Begin();
            classInfo.applicationExtra.disableInput(this);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            _result = false;
            FormFadeOut.Begin();
            classInfo.applicationExtra.disableInput(this);
        }

        private void FormFadeOut_Completed(object sender, EventArgs e)
        {
            this.Close();
        }

        #region textValidation
        private void ValidateFloat(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox == null)
                return;

            if (textBox.Text != "")
            {
                float value;
                if (!float.TryParse(textBox.Text, out value))
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
        #endregion
        #endregion
    }
}