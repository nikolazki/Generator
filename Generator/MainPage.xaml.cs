﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.UI.Popups;


namespace Generator
{
    
    /// <summary>
    /// Main interface
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Random stat = new Random();
        
        public MainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
        }
        
        //methods for main page
        /// <summary>
        /// Saves each object into a text file 
        /// in the form of "str,dex,int,name"
        /// </summary>
        private async void saveToFile()
        {
            FileSavePicker savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            savePicker.FileTypeChoices.Add("NPC List", new List<string>() { ".npc" });
            savePicker.SuggestedFileName = "NPCs";
            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                await FileIO.WriteTextAsync(file, string.Empty);
                foreach (NPC NPC in npcList.Items)
                {
                    await FileIO.AppendTextAsync(file, NPC.npcToText());
                    await FileIO.AppendTextAsync(file, Environment.NewLine);
                }
            }
        }
        /// <summary>
        /// Reads From a Text File in the form of
        /// "str,dex,int,name"
        /// </summary>
        private async void loadFromFile()
        {
            npcList.Items.Clear();
            string[] splitLine;
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.FileTypeFilter.Add(".npc");
            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                var readFile = await Windows.Storage.FileIO.ReadLinesAsync(file);
                foreach (var line in readFile)
                {
                    splitLine = line.Split(',');
                    if (splitLine.Length == 8)
                        npcFromFile(splitLine);
                }
                //SELECT FIRST ITEM IN LIST
                npcList.SelectedIndex = 0;
                clearGenStats();
            }
        }
        /// <summary>
        /// helper function for loading from files
        /// </summary>
        /// <param name="splitLine"></param>
        private void npcFromFile(string[] splitLine)
        {
            int str, dex, intell, cont, wis, charisma;
            string name;
            NPC npc;
            int.TryParse(splitLine[0], out str);
            int.TryParse(splitLine[1], out dex);
            int.TryParse(splitLine[2], out cont);
            int.TryParse(splitLine[3], out intell);
            int.TryParse(splitLine[4], out wis);
            int.TryParse(splitLine[5], out charisma);
            string type = splitLine[6];
            name = splitLine[7];
            npc = new NPC(str, dex, cont, intell, wis, charisma, name);
            npc.Type = type;
            npcList.Items.Add(npc);
        }
        private async void displayAlert()
        {
            var dialog = new MessageDialog("Please Enter A Name");
            await dialog.ShowAsync();
        }
        /// <summary>
        /// Adds NPC to the npcList
        /// </summary>
        private void addToList()
        {
          
            npcList.Items.Add(createNPC());
            npcList.SelectedIndex = (npcList.Items.Count) - 1;
            clearGenStats();
        }
        /// <summary>
        /// Creates an NPC from the stats generated on page
        /// </summary>
        /// <returns>NPC</returns>
        private NPC createNPC()
        {
            int str, dex, cont, intell, wis, charisma;
            NPC npcToReturn;
            int.TryParse(strBox.Text, out str);
            int.TryParse(dexBox.Text, out dex);
            int.TryParse(contBox.Text, out cont);
            int.TryParse(intBox.Text, out intell);
            int.TryParse(wisBox.Text, out wis);
            int.TryParse(charBox.Text, out charisma);
            //create npc with basic stats first
            npcToReturn = new NPC(str, dex, cont, intell, wis, charisma, nameBox.Text);
            //set additional stats for npc
            setType(npcToReturn);
            return npcToReturn;
        }
        /// <summary>
        /// Sets the Type for the NPC from the selected radio button in the combo box
        /// </summary>
        /// <param name="npc"></param>
        private void setType(NPC npc)
        {
            TextBlock type = (TextBlock)comboBox.SelectedItem;
            npc.Type = type.Text;
        }
        /// <summary>
        /// Clears all Stats for the Generator
        /// </summary>
        private void clearGenStats()
        {
            nameBox.Text = "";
            strBox.Text = "";
            dexBox.Text = "";
            intBox.Text = "";
            contBox.Text = "";
            wisBox.Text = "";
            charBox.Text = "";
            comboBox.SelectedIndex = -1;
        }
        /// <summary>
        /// Generates the values to place into the text boxes
        /// </summary>
        private void generateStats()
        {
            strBox.Text = stat.Next(Range.min, Range.max).ToString();
            dexBox.Text = stat.Next(Range.min, Range.max).ToString();
            contBox.Text = stat.Next(Range.min, Range.max).ToString();
            intBox.Text = stat.Next(Range.min, Range.max).ToString();
            wisBox.Text = stat.Next(Range.min, Range.max).ToString();
            charBox.Text = stat.Next(Range.min, Range.max).ToString();
        }
        

        //EVENTS
        private void loadText_Click(object sender, RoutedEventArgs e)
        {
            loadFromFile();
        }
        private void saveText_Click(object sender, RoutedEventArgs e)
        {
           saveToFile();
        }
        private void genButton_Click(object sender, RoutedEventArgs e)
        {
            generateStats();
        }
        private void addToList_Click(object sender, RoutedEventArgs e)
        {
            if (nameBox.Text == "")
            {
                displayAlert();
            }
            else
            {
                addToList();
                clearGenStats();
            }
        }
        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            npcList.Items.Remove(npcList.SelectedItem);
            if(npcList.Items.Count > 0)
                npcList.SelectedIndex = 0;
        }
        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            saveToFile();
        }
        private void loadButton_Click(object sender, RoutedEventArgs e)
        {
            loadFromFile();
        }
        private void clearALL_Click(object sender, RoutedEventArgs e)
        {
            npcList.Items.Clear();
        }
        private void viewStatsButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(NPCStatPage),npcList.SelectedItem);
        }
        private void radioButtonType_Click(object sender, RoutedEventArgs e)
        {
            comboBox.IsDropDownOpen = false;
            comboBox.SelectedItem = sender;
        }
    }
}
