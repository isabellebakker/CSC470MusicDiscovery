﻿using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace DiscoverSpot
{
    public partial class Form1 : Form
    {
       private static EmbedIOAuthServer _server;
       private static SpotifyClient _spotify;

       public static string _trackName;


       public static async Task InitializeSpotify()
       {
            _server = new EmbedIOAuthServer(new Uri("http://localhost:5543/callback"), 5543);
            await _server.Start();

            _server.AuthorizationCodeReceived += OnAuthorizationCodeReceived;
            _server.ErrorReceived += OnErrorReceived;

            var request = new LoginRequest(_server.BaseUri, "457d687267f447f39d58af721581f1b8", LoginRequest.ResponseType.Code)
            {
                Scope = new List<string> { Scopes.UserTopRead, Scopes.PlaylistModifyPrivate, Scopes.PlaylistModifyPublic }
            };

            BrowserUtil.Open(request.ToUri());
       }

       public static async Task OnAuthorizationCodeReceived(object sender, AuthorizationCodeResponse response)
       {
           await _server.Stop();

           var config = SpotifyClientConfig.CreateDefault();
           var tokenResponse = await new OAuthClient(config).RequestToken(
               new AuthorizationCodeTokenRequest(
                   "457d687267f447f39d58af721581f1b8", "f29c99014e624450b6c3f88a7c67a931", response.Code, new Uri("http://localhost:5543/callback")
               )
           );

           _spotify = new SpotifyClient(tokenResponse.AccessToken);
            // save token in _spotify

        }

       public static async Task OnErrorReceived(object sender, string error, string state)
       {
           await _server.Stop();
       }

        
        public async static Task GetTrack()
        {
            var track = await _spotify.Tracks.Get("1s6ux0lNiTziSrd7iUAADH");

            _trackName = track.Name;
        }
        
        public Form1()
        {
            InitializeComponent();
            button2.Hide();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void label1_Click(object sender, EventArgs e)
        {
            
        }

        private async void Button1_Click(object sender, EventArgs e)
        {
            // Initialize Spotify when the button's clicked
            await InitializeSpotify();
            button1.Hide();
            button2.Show();
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            await GetTrack();
            label1.Text = _trackName;
        }
    }
}
