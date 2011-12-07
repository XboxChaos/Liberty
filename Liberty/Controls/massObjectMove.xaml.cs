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
        private Reach.CampaignSave _saveData = null;
        private float _moveX = 0, _moveY = 0, _moveZ = 0;
        private bool _result = false;
        private Reach.TagGroup _tagGroup;

		public massObjectMove(Reach.CampaignSave saveData, Reach.TagGroup tagGroup, string tagGroupName)
		{
			this.InitializeComponent();

            _saveData = saveData;
            _tagGroup = tagGroup;
            lblSubHeader1.Text = String.Format(lblSubHeader1.Text, tagGroupName);

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

            if ((bool)cBAdvancedAlgo.IsChecked)
            {
                int __x = _saveData.Objects.Count / 2;

                _moveX = _moveX - __x - (float)0.1;
                _moveY = _moveY - __x - (float)0.1;

                bool lastX = false;
                bool lastY = false;
                foreach (Reach.GameObject obj in _saveData.Objects)
                {
                    if (obj != null && !obj.Deleted)
                    {
                        if (lastX)
                            obj.X = _moveX;
                        else
                            obj.X = _moveX + 0.1f;

                        if (lastY)
                            obj.Y = _moveY;
                        else
                            obj.Y = _moveY + 0.1f;

                        obj.Z = _moveZ;

                        Random ran = new Random();
                        lastX = false;
                        lastY = false;

                        if (ran.Next(0, 2) == 1)
                            lastX = true;
                        else
                            lastY = true;
                    }
                }
            }
            else
            {
                foreach (Reach.GameObject obj in _saveData.Objects)
                {
                    if (obj != null && !obj.Deleted)
                    {
                        obj.X = _moveX;
                        obj.Y = _moveY;
                        obj.Z = _moveZ;
                    }
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