﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NewPlayer
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// Player and Server 
        /// </summary>
        ServerController Controller = new ServerController("http://96.126.117.25/Music");
        Player player = new Player("http://96.126.117.25/Music");
        /// <summary>
        /// Move form
        /// </summary>
        private bool mouseDown;
        private Point lastLocation;
        /// <summary>
        /// Playlist Information
        /// </summary>
        List<string> AllPlaylists = new List<string>();
        public static List<SongProperties> ShuffledPlaylist = new List<SongProperties>();
        
        /*
            Error Messages
         */
        /// <summary>
        /// Format all Error Messages to look alike
        /// </summary>
        /// <param name="ErrorCode">Error Code, can be looked up at end of Form1.cs</param>
        /// <param name="LineNumber">LineNumber of where the error occured</param>
        public void ErrorMessage(Exception a, string ErrorCode, [CallerLineNumber] int LineNumber = 0)
        {
            try
            {
                MessageBox.Show(a.Message + "\n" +
                                "Error Code: " + ErrorCode + "\n" +
                                "Line: " + LineNumber +
                                a.ToString());
                player.Next();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        public Form1()
        {
            player.Changed += ChangedEventHandler;
            InitializeComponent();
            initialize();
        }
        /*
            Initialize
         */
        private void initialize() {
            try
            {
                // Get Playlists From Web Server

                AllPlaylists = Controller.GetAllPlaylists();
                foreach (string Playlist in AllPlaylists)
                {
                    PlaylistDropDown.Items.Add(Playlist);
                }

                // Set First Playlist
                PlaylistDropDown.SelectedIndex = 0;
                // Start lLog

            }
            catch (Exception err)
            {
                ErrorMessage(err, "1");
            }
        }
        #region Move Form
        private void Main_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                mouseDown = true;// Mouse is pressed down: start moving form
                lastLocation = e.Location; // get location of mouse
            }
            catch (Exception a)
            {
                ErrorMessage(a, "480 \nCould Not Move Window");
            }
        }
        private void Main_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                mouseDown = false;// Mouse is no longer pressed down: stop moving form
            }
            catch (Exception a)
            {
                ErrorMessage(a, "480 \nCould Not Move Window");
            }
        }
        private void Main_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (mouseDown) // Mouse is pressed down: when mouse starts moving
                {

                    this.Location = new Point(
                        (this.Location.X - lastLocation.X) + e.X, (this.Location.Y - lastLocation.Y) + e.Y);// set new location of mouse

                    this.Update();// Moves form locatio to mouse location
                }
            }
            catch (Exception a)
            {
                ErrorMessage(a, "480 \nCould Not Move Window");
            }
        }
        #endregion
        #region FormButtons
        private void btnInfo_Click(object sender, EventArgs e)
        {
            Information info = new Information();
            info.Show();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            try
            {
                player.Previous();
                updateUserInterface(player.getCurrentIndex());
            }
            catch (Exception err) {
                ErrorMessage(err, "1");
            }
        }

        private void btnPlayPause_Click(object sender, EventArgs e)
        {
            try
            {
                if (player.isPlaying())
                {
                    var bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject("play");
                    btnPlayPause.Image = bmp;
                }
                else {
                    var bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject("pause");
                    btnPlayPause.Image = bmp;
                }
                player.PlayPause();
                updateUserInterface(player.getCurrentIndex());
            }
            catch (Exception err) {
                ErrorMessage(err, "1");
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            try
            {

                player.Next();
                updateUserInterface(player.getCurrentIndex());
                
            }
            catch (Exception err) {
                ErrorMessage(err, "1");
            }
        }
        /// <summary>
        /// New Playlist Selected, Get Songs from Selected Playlist
        /// </summary>
        private void PlaylistDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                player.ClearPlaylists();
                // Gets Songs From location and passes to player

                player.Initialize(PlaylistDropDown.SelectedItem.ToString());
                updateUserInterface(player.getCurrentIndex());

            }
            catch (Exception err)
            {
                ErrorMessage(err, "1");
            }
        }
        #endregion
        #region HelperFucntions
        /// <summary>
        /// Update User interface with content from given index
        /// </summary>
        /// <param name="index">index location of Shuffled Playlist</param>
        private void updateUserInterface(int index)
        {
            try
            {
                txtTitle.Text = ShuffledPlaylist[index].Title;
                toolTip1.SetToolTip(txtTitle, ShuffledPlaylist[index].Title);
            }
            catch (ArgumentOutOfRangeException)
            {
            }
            catch (Exception err)
            {
                ErrorMessage(err, "1");
            }
        }
        public void ChangedEventHandler(object sender, EventArgs e)
        {
            try
            {
                //txtTitle.Text = ShuffledPlaylist[player.getCurrentIndex()].Title;
                //txtArtist.Text = "Unknown";
                this.txtTitle.BeginInvoke((MethodInvoker)delegate () { this.txtTitle.Text = ShuffledPlaylist[player.getCurrentIndex()].Title; });
            }
            catch (Exception err)
            {
                ErrorMessage(err, "1");
            }
        }
        #endregion

    }
}
