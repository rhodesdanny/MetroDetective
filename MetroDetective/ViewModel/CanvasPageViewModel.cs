using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetroDetective.Common;
using MetroDetective.Model;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Newtonsoft.Json;

namespace MetroDetective.ViewModel
{
    public class CanvasPageViewModel : BindableBase
    {
        public CanvasPageViewModel()
        {
            ResetPage();

            PaintingNumbers = new List<int> { 1, 2, 3};

            CurrentPaintingNumber = PaintingNumbers[0];
            LevelFinished = false;
            GameSpotsList = new GameHotSpots();
            LeftPaintingName = @"Assets/paintings/" + PaintingNumbers[0] + "Left.JPG";
            RightPaintingName = @"Assets/paintings/" + PaintingNumbers[0] + "Right.JPG";

            CurrentPaintingSpots = new PaintingHotSpots();
            CurrentPaintingSpots.Spots = new List<HotSpot>();
            CurrentPaintingSpots.PaintingName = CurrentPaintingNumber + "";

            var bounds = Window.Current.Bounds;
            ScreenHeight = bounds.Height;
            ScreenWidth = bounds.Width;
            ScreenHeightNeg = bounds.Height*-1;
            ScreenWidthNeg = bounds.Width*-1;
            CanvasHeight = ScreenHeight * 80 /100;
            CanvasWidth = ScreenWidth * 48 / 100;
            HintsLeft = 3;
            LoadGameSpotsAsync();
        }

        public void ResetPage()
        {
            TagsLeft = new List<Image>();
            TagsRight = new List<Image>();
            ClickedSpot = null;
            FoundSpotsCount = 0;
        }


        public string LeftPaintingName { get; set; }

        public string RightPaintingName { get; set; }
        
        public double ScreenHeight { get; set; }
        
        public double ScreenWidth { get; set; }

        public double CanvasHeight { get; set; }

        public double CanvasWidth { get; set; }
        
        public double ScreenHeightNeg { get; set; }

        public double ScreenWidthNeg { get; set; }

        public List<int> PaintingNumbers { get; set; }

        public int CurrentPaintingNumber { get; set; }

        public int FoundSpotsCount { get; set; }

        public bool LevelFinished { get; set; }

        public HotSpot ClickedSpot { get; set; }

        //public List<HotSpot> Spots { get; set; }

        public List<Image> TagsLeft { get; set; }

        public List<Image> TagsRight { get; set; }

        public PaintingHotSpots CurrentPaintingSpots { get; set; }

        public GameHotSpots GameSpotsList { get; set; }

        public string ErrorMessage { get; set; }
        
        public int HintsLeft { get; set; }

        public async void LoadGameSpotsAsync()
        {
            try
            {
                var folder = Package.Current.InstalledLocation;
                var file = await folder.GetFileAsync(@"Assets\HotSpots.txt");
                var read = await FileIO.ReadTextAsync(file);
                var text = read;
                GameSpotsList = JsonConvert.DeserializeObject<GameHotSpots>(text);
                CurrentPaintingSpots = GameSpotsList.PaintingSpots[0];
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }

        }
        
        public async void SaveGameSpots(string json)
        {
            var folder = Package.Current.InstalledLocation;
            var file = await folder.GetFileAsync(@"Assets\HotSpots.txt");
            await FileIO.WriteTextAsync(file, json);
        }

        public void LoadCurrentPaintingHotSpots(int index)
        {
            var found = false;

            if (GameSpotsList != null && GameSpotsList.PaintingSpots != null && GameSpotsList.PaintingSpots.Count >= index)
            {
                foreach (var spot in GameSpotsList.PaintingSpots)
                {
                    if (spot.PaintingName == index + "")
                    {
                        CurrentPaintingSpots = GameSpotsList.PaintingSpots[index-1];
                        found = true;
                        break;
                    }
                }
            }

            if (!found)
            {
                CurrentPaintingSpots = new PaintingHotSpots();
                CurrentPaintingSpots.PaintingName = index + "";
            }
        }
    }
}
