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

namespace Liberty.Controls
{
    /// <summary>
    /// Interaction logic for step2.xaml
    /// </summary>
    public partial class step2 : UserControl
    {
        public event EventHandler ExecuteMethod;
        private int originalBipdItem = -1;

        public step2()
        {
            InitializeComponent();
        }

        private class mapIdentComparer : IComparer<Reach.BipedObject>
        {
            public int Compare(Reach.BipedObject x, Reach.BipedObject y)
            {
                return x.ResourceID.CompareTo(y.ResourceID);
            }
        }

        public void loadData()
        {
            checkInvincible.IsChecked = classInfo.loadPackageData.isPlayerInvincible();

            float[] playerCords = classInfo.loadPackageData.getPlayerCords();

            txtPlayerXCord.Text = Convert.ToString(playerCords[0]);
            txtPlayerYCord.Text = Convert.ToString(playerCords[1]);
            txtPlayerZCord.Text = Convert.ToString(playerCords[2]);

            SortedSet<Reach.BipedObject> availableBipeds = new SortedSet<Reach.BipedObject>(new mapIdentComparer());
            Reach.BipedObject currentBiped = classInfo.storage.fileInfoStorage.saveData.Player.Biped;
            foreach (Reach.GameObject obj in classInfo.storage.fileInfoStorage.saveData.Objects)
            {
                if (obj != null && !obj.Deleted && obj.TagGroup == Reach.TagGroup.Bipd && obj.Zone == currentBiped.Zone)
                    availableBipeds.Add((Reach.BipedObject)obj);
            }

            originalBipdItem = -1;
            cBNoWeapTransfer.IsEnabled = false;
            cBBipeds.Items.Clear();
            foreach (Reach.BipedObject obj in availableBipeds)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = classInfo.nameLookup.translate(obj.ResourceID);
                item.Tag = obj;
                cBBipeds.Items.Add(item);

                if (obj.ResourceID == currentBiped.ResourceID)
                {
                    originalBipdItem = cBBipeds.Items.Count - 1;
                    cBBipeds.SelectedItem = item;
                }
            }
        }

        public void saveData()
        {
            classInfo.storage.fileInfoStorage.saveData.Player.ChangeBiped((Reach.BipedObject)((ComboBoxItem)cBBipeds.SelectedItem).Tag, !(bool)cBNoWeapTransfer.IsChecked);
            classInfo.savePackageData.setPlayerInvincibility((bool)checkInvincible.IsChecked);

            float[] playerCords = new float[3];
            playerCords[0] = Convert.ToSingle(txtPlayerXCord.Text);
            playerCords[1] = Convert.ToSingle(txtPlayerYCord.Text);
            playerCords[2] = Convert.ToSingle(txtPlayerZCord.Text);

            classInfo.savePackageData.setPlayerCords(playerCords);
        }

        protected virtual void OnExecuteMethod()
        {
            if (ExecuteMethod != null)
                ExecuteMethod(this, EventArgs.Empty);
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

		#region btnOpenwpf
        private void btnOpen_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
            btnOpen.Source = new BitmapImage(source);

            classInfo.storage.fileInfoStorage.leavingStep2 = true;

            OnExecuteMethod();
        }

        private void btnOpen_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                var source = new Uri(@"/Liberty;component/Images/SecondaryButton.png", UriKind.Relative);
                btnOpen.Source = new BitmapImage(source);
            }
            else
            {
                var source = new Uri(@"/Liberty;component/Images/Button-onhover.png", UriKind.Relative);
                btnOpen.Source = new BitmapImage(source);
            }
        }

        private void btnOpen_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var source = new Uri(@"/Liberty;component/Images/SecondaryButton", UriKind.Relative);
            btnOpen.Source = new BitmapImage(source);
        }
        #endregion
		
        private void cBBipeds_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (originalBipdItem != cBBipeds.SelectedIndex)
            {
                OnExecuteMethod();
                if (classInfo.storage.fileInfoStorage.messageOpt) { cBNoWeapTransfer.IsEnabled = true; }
                else { cBBipeds.SelectedIndex = originalBipdItem; }
            }
            originalBipdItem = cBBipeds.SelectedIndex;
        }
    }
}