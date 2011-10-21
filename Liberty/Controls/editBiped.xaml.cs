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
    public partial class editBiped : UserControl, StepUI.IStep
    {
        private MainWindow mainWindow = null;
        private int originalBipdItem = -1;
        private bool loading = false;

        public editBiped()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(step2_Loaded);
        }

        void step2_Loaded(object sender, RoutedEventArgs e)
        {
            mainWindow = Window.GetWindow(this) as MainWindow;
        }

        public void Load(Util.SaveEditor saveEditor)
        {
            loading = true;
            checkInvincible.IsChecked = saveEditor.Invincible;
            checkNoPhysics.IsEnabled = (saveEditor.Vehicle != null);
            checkNoPhysics.IsChecked = saveEditor.Noclip;

            txtPlayerXCord.Text = saveEditor.X.ToString();
            txtPlayerYCord.Text = saveEditor.Y.ToString();
            txtPlayerZCord.Text = saveEditor.Z.ToString();

            originalBipdItem = -1;
            cBWeapTransfer.IsEnabled = false;
            cBBipeds.Items.Clear();

            Reach.BipedObject currentBiped = saveEditor.Biped;
            HashSet<Reach.BipedObject> availableBipeds = saveEditor.FindSwappableBipeds();
            SortedDictionary<string, Reach.BipedObject> sortedBipeds = new SortedDictionary<string, Reach.BipedObject>();
            foreach (Reach.BipedObject obj in availableBipeds)
                sortedBipeds[saveEditor.IdentifyObject(obj, false)] = obj;

            foreach (KeyValuePair<string, Reach.BipedObject> obj in sortedBipeds)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = obj.Key;
                item.Tag = obj.Value;
                cBBipeds.Items.Add(item);

                if (obj.Value == currentBiped)
                {
                    originalBipdItem = cBBipeds.Items.Count - 1;
                    cBBipeds.SelectedIndex = originalBipdItem;
                }
            }
            cBBipeds.IsEnabled = true;

            loading = false;
        }

        public bool Save(Util.SaveEditor saveEditor)
        {
            saveEditor.SwapBiped((Reach.BipedObject)((ComboBoxItem)cBBipeds.SelectedItem).Tag, (bool)cBWeapTransfer.IsChecked);

            saveEditor.Invincible = (bool)checkInvincible.IsChecked;
            saveEditor.Noclip = (bool)checkNoPhysics.IsChecked;

            saveEditor.X = Convert.ToSingle(txtPlayerXCord.Text);
            saveEditor.Y = Convert.ToSingle(txtPlayerYCord.Text);
            saveEditor.Z = Convert.ToSingle(txtPlayerZCord.Text);

            return true;
        }

        public void Show()
        {
            Visibility = Visibility.Visible;
        }

        public void Hide()
        {
            Visibility = Visibility.Hidden;
        }

        #region wpf
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

        private void btnNameIdent_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.showLeavingDialog("http://liberty.codeplex.com/discussions/264198", "CodePlex");
        }
		
        private void cBBipeds_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (originalBipdItem != cBBipeds.SelectedIndex && !loading)
            {
                bool swap = mainWindow.showWarning("Swapping your biped may cause the game to freeze or behave unexpectedly. Your old biped will also be deleted. Continue?", "Biped Swap");

                if (swap)
                    cBWeapTransfer.IsEnabled = true;
                else
                    cBBipeds.SelectedIndex = originalBipdItem;
            }
            originalBipdItem = cBBipeds.SelectedIndex;
        }

        private void checkNoPhysics_Checked(object sender, RoutedEventArgs e)
        {
            // hax
            if (!loading)
            {
                if (!mainWindow.showWarning("Enabling noclip mode will allow you to float and pass through any object, but it will make the mission impossible to complete. You will also be unable to move vertically due to limitations in the game. Continue?", "Noclip Mode"))
                    checkNoPhysics.IsChecked = false;
            }
        }
        #endregion
    }
}