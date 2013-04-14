using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MetroDetective.Model;
using Newtonsoft.Json;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using MetroDetective.Common;
using Windows.UI.Xaml.Shapes;
using MetroDetective.ViewModel;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MetroDetective
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public CanvasPageViewModel _viewModel;
        private MediaElement Music;
        private bool IsMusicPlaying = true;

        public MainPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Register for the viewstatechanged event
            // Register for the window resize event
            Window.Current.SizeChanged += WindowSizeChanged;     

            _viewModel = new CanvasPageViewModel();
            LoadBackGroundMusic();
            GoToState("MainMenuState");
        }

        private void ShowHideAppBarButtons(bool isHide)
        {
            if (isHide)
            {
                menuButton.Visibility = Visibility.Collapsed;
                hintButton.Visibility = Visibility.Collapsed;
                helpButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                menuButton.Visibility = Visibility.Visible;
                hintButton.Visibility = Visibility.Visible;
                helpButton.Visibility = Visibility.Visible;
            }
        }

            private void WindowSizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
            {
                // Obtain view state by explicitly querying for it
                ApplicationViewState myViewState = ApplicationView.Value;
                if (myViewState == ApplicationViewState.FullScreenLandscape)
                {
                    ShowHideAppBarButtons(false);
                    SplitScreen.Visibility = Visibility.Collapsed;
                }
                else
                {
                    ShowHideAppBarButtons(true);
                    SplitScreen.Visibility = Visibility.Visible;
                }             
            }

        public string ToJson(object obj)
        {

            return JsonConvert.SerializeObject(obj, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        }
        
        private void GoToState(string state)
        {
            switch (state)
            {
                case "MainMenuState":
                    ResetCanvas();
                    _viewModel.HintsLeft = 3;
                    ShowHideAppBarButtons(true);
                    AutomationProperties.SetName(hintButton, "Hints" + " (" + _viewModel.HintsLeft + " Left)");
                    _viewModel = new CanvasPageViewModel();
                    bottomAppBar.IsOpen = false;
                    bottomAppBar.IsSticky = false;
                    ChangePaintings(_viewModel.PaintingNumbers[0]);
                    break;
                case "PlayingState":
                    ShowHideAppBarButtons(false);
                    bottomAppBar.IsOpen = true;
                    bottomAppBar.IsSticky = true;
                    break;
                case "FinishState":
                    ShowHideAppBarButtons(true);
                    bottomAppBar.IsOpen = false;
                    bottomAppBar.IsSticky = false;
                    break;
            }
            VisualStateManager.GoToState(this, state, true);
        }

        private void RenderTags(Canvas canvasOne, Canvas canvasTwo, double X, double Y)
        {
            if(_viewModel.ClickedSpot==null)
            {
                return;
            }

            var random = new Random();
            var index = random.Next(1, 4);
            var uriStr = "ms-appx:///Assets/labels/label_" + index + ".png";
            var tagLeft = new Image { Source = new BitmapImage(new Uri(uriStr, UriKind.RelativeOrAbsolute)) };
            canvasOne.Children.Add(tagLeft);
            tagLeft.RenderTransformOrigin = new Point(0.50, 0.05);
            Canvas.SetLeft(tagLeft, X);
            Canvas.SetTop(tagLeft, Y);
            tagLeft.RenderTransform = new CompositeTransform();
            var anima = tagLeft.AnimateProperty<DoubleAnimationUsingKeyFrames>("(UIElement.RenderTransform).(CompositeTransform.Rotation)").AddEasingKeyFrame(0, 10).AddEasingKeyFrame(1.0, 0, new BackEase());

            var tagRight = new Image { Source = new BitmapImage(new Uri(uriStr, UriKind.RelativeOrAbsolute)) };
            canvasTwo.Children.Add(tagRight);
            tagRight.RenderTransformOrigin = new Point(0.50, 0.05);
            Canvas.SetLeft(tagRight, X);
            Canvas.SetTop(tagRight, Y);
            tagRight.RenderTransform = new CompositeTransform();
            var animaRight = tagRight.AnimateProperty<DoubleAnimationUsingKeyFrames>("(UIElement.RenderTransform).(CompositeTransform.Rotation)").AddEasingKeyFrame(0, 10).AddEasingKeyFrame(1.0, 0, new BackEase());

            var bs = new Storyboard();
            bs.Children.Add(anima);
            bs.Children.Add(animaRight);
            bs.Begin();
            
            _viewModel.FoundSpotsCount++;

            CheckLevelAndGameCompleteStatus();
        }

        private void CheckLevelAndGameCompleteStatus()
        {
            if (_viewModel.FoundSpotsCount >= Constants.HotSpotNumbers)
            {
                if (_viewModel.CurrentPaintingNumber == _viewModel.PaintingNumbers.Count)
                {
                    _viewModel.LevelFinished = true;
                }
                ShowLevelCompleteMsg();
            }
        }

        private void ShowLevelCompleteMsg()
        {
            var msgDialog = new MessageDialog(Constants.LevelCompleteMsg, Constants.LevelCompleteTitle);

            //OK Button
            var okBtn = new UICommand("Next Level");
            if (_viewModel.LevelFinished)
            {
                msgDialog.Title = Constants.GameCompleteTitle;
                msgDialog.Content = Constants.GameCompleteMsg;
                okBtn.Label = "Finish";
                okBtn.Invoked = FinishBtnClick;
            }
            else
            {
                okBtn.Invoked = NextBtnClick;
            }
            msgDialog.Commands.Add(okBtn);
            msgDialog.ShowAsync();
        }
        
        private void NextBtnClick(IUICommand command)
        {
            if (++_viewModel.CurrentPaintingNumber <= _viewModel.PaintingNumbers.Count)
            {
                _viewModel.HintsLeft++;
                AutomationProperties.SetName(hintButton, "Hints" + " (" + _viewModel.HintsLeft + " Left)");
                ResetCanvas();
                Storyboard1.Begin();
                _viewModel.LoadCurrentPaintingHotSpots(_viewModel.CurrentPaintingNumber);
                ChangePaintings(_viewModel.CurrentPaintingNumber);
            }
            else
            {
                _viewModel.LevelFinished = true;
                ShowLevelCompleteMsg();
            }
        }

        private void FinishBtnClick(IUICommand command)
        {
            ResetCanvas();
            GoToState("FinishState");
        }

        private bool ClickValidation(TappedRoutedEventArgs e, Canvas canvas)
        {
            if (_viewModel.CurrentPaintingSpots == null || _viewModel.CurrentPaintingSpots.Spots == null)
                return false;

            var x = (int)e.GetPosition(canvas).X;
            var y = (int)e.GetPosition(canvas).Y;

            foreach (var spot in _viewModel.CurrentPaintingSpots.Spots)
            {
                var X_value = spot.X * _viewModel.CanvasWidth ;
                var Y_value = spot.Y * _viewModel.CanvasHeight;

                if (!spot.Checked
                    &&
                    Constants.HotSpotToleranceRadius >= Math.Abs(X_value - x)
                    &&
                    Constants.HotSpotToleranceRadius >= Math.Abs(Y_value - y)
                    )
                {
                    spot.Checked = true;
                    spot.X = X_value;
                    spot.Y = Y_value;
                    _viewModel.ClickedSpot = spot;
                    return true;
                }
            }
            return false;
        }

        private void ResetCanvas()
        {
            canvasLeft.Children.Clear();
            canvasRight.Children.Clear();
            tb_XY.Text = "";
            _viewModel.ResetPage();
        }


        private void ChangePaintings(int index)
        {
            try
            {
                var uriStr = "ms-appx:///Assets/paintings/" + index;
                LeftPaintingBrush.ImageSource = new BitmapImage(new Uri(uriStr + "Left.jpg", UriKind.RelativeOrAbsolute));
                RightPaintingBrush.ImageSource = new BitmapImage(new Uri(uriStr + "Right.jpg", UriKind.RelativeOrAbsolute));
            }
            catch (Exception e)
            {
                var errorDialog = new MessageDialog("Sorry we crashed :-(", "Oops...");

                errorDialog.ShowAsync();
            }
        }


        private void AddSpotToCurrentPainting(int x, int y)
        {
            if (!(bool)ckbox_AddSpots.IsChecked) 
                return; 

            if (_viewModel.CurrentPaintingSpots == null)
                _viewModel.CurrentPaintingSpots = new PaintingHotSpots();
            if (_viewModel.CurrentPaintingSpots.PaintingName == null)
                _viewModel.CurrentPaintingSpots.PaintingName = _viewModel.CurrentPaintingNumber + "";
            if (_viewModel.CurrentPaintingSpots.Spots == null)
                _viewModel.CurrentPaintingSpots.Spots = new List<HotSpot>();
            

            if (_viewModel.CurrentPaintingSpots.Spots.Count >= Constants.HotSpotNumbers)
            {
                return;
            }

            foreach (var spot in _viewModel.CurrentPaintingSpots.Spots)
            {
                if (Math.Abs(x - spot.X) < Constants.HotSpotToleranceRadius &&
                    Math.Abs(y - spot.Y) < Constants.HotSpotToleranceRadius)
                {
                    return;
                }
            }
            _viewModel.CurrentPaintingSpots.Spots.Add(new HotSpot() {Checked = false, X = x/_viewModel.CanvasWidth, Y = y/_viewModel.CanvasHeight});
        }


        private void AddCurrentPaintingToGameSpotsList(PaintingHotSpots currentSpots)
        {
            if (ckbox_AddSpots.IsChecked != null && !(bool)ckbox_AddSpots.IsChecked) { return; }

            if (_viewModel.GameSpotsList == null)
            {
                _viewModel.GameSpotsList = new GameHotSpots();

            }

            if (_viewModel.GameSpotsList.PaintingSpots == null)
            {
                _viewModel.GameSpotsList.PaintingSpots = new List<PaintingHotSpots>();
            }
            
            foreach (var painting in _viewModel.GameSpotsList.PaintingSpots)
            {
                if (!string.IsNullOrEmpty(painting.PaintingName) && painting.PaintingName.Equals(currentSpots.PaintingName))
                {
                    return;
                }
            }

            _viewModel.GameSpotsList.PaintingSpots.Add(currentSpots);
        }


        public async void LoadBackGroundMusic()
        {
            var package = Windows.ApplicationModel.Package.Current;
            var installedLocation = package.InstalledLocation;
            var storageFile = await installedLocation.GetFileAsync(@"Assets\Sounds\music.mp3");
            if (storageFile != null)
            {
                var stream = await storageFile.OpenAsync(Windows.Storage.FileAccessMode.Read);
                Music = new MediaElement();
                Music.SetSource(stream, storageFile.ContentType);
                Music.IsLooping = true;
            }
        }

        private void CanvasLeftTapped(object sender, TappedRoutedEventArgs e)
        {

            if (ckbox_AddSpots.IsChecked != null && (bool) ckbox_AddSpots.IsChecked)
            {
                int x = (int) e.GetPosition(canvasLeft).X;
                int y = (int) e.GetPosition(canvasLeft).Y;
                tb_XY.Text = x + "/" + y;
                AddSpotToCurrentPainting(x, y);
            }
            else
            {
                if (ClickValidation(e, canvasLeft))
                {
                    var X = _viewModel.ClickedSpot.X - 16;
                    var Y = _viewModel.ClickedSpot.Y - 2;
                    RenderTags(canvasLeft, canvasRight, X, Y);
                }
            }
        }


        private void CanvasRightTapped(object sender, TappedRoutedEventArgs e)
        {


            if (ckbox_AddSpots.IsChecked != null && (bool) ckbox_AddSpots.IsChecked)
            {
                int x = (int) e.GetPosition(canvasRight).X;
                int y = (int) e.GetPosition(canvasRight).Y;
                tb_XY.Text = x + "/" + y;
                AddSpotToCurrentPainting(x, y);
            }
            else
            {

             
                if (ClickValidation(e, canvasRight))
                {
                    var X = _viewModel.ClickedSpot.X - 16;
                    var Y = _viewModel.ClickedSpot.Y - 2;
                    RenderTags(canvasLeft, canvasRight, X, Y);
                }
            }
        }

        private void btn_reset_Click(object sender, RoutedEventArgs e)
        {
            ResetCanvas();
        }

        private void btn_addSpot_Click(object sender, RoutedEventArgs e)
        {
            AddCurrentPaintingToGameSpotsList(_viewModel.CurrentPaintingSpots);
        }

        private void MainMenuGrid_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            GoToState("PlayingState");
        }

        private void FinishedGrid_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            GoToState("MainMenuState");
        }
        
        private void btn_Help_Click(object sender, RoutedEventArgs e)
        {
            var helpDialog = new MessageDialog(Constants.HelpMsg, "Help");
            helpDialog.ShowAsync();
        }

        private void btn_back_Click(object sender, RoutedEventArgs e)
        {
            GoToState("MainMenuState");
        }

        private void save_allSpot_Click(object sender, RoutedEventArgs e)
        {
            var json = ToJson(_viewModel.GameSpotsList);
            var helpDialog = new MessageDialog(json, "Json");
            helpDialog.ShowAsync();
            //_viewModel.SaveGameSpots(ToJson(_viewModel.GameSpotsList));
        }

        private void btn_next_page_Click(object sender, RoutedEventArgs e)
        {
            NextBtnClick(new UICommand());
        }

        private void btn_pause_Click(object sender, RoutedEventArgs e)
        {

            if (Music != null && IsMusicPlaying)
            {
                var playStyle = (Style)App.Current.Resources["PlayAppBarButtonStyle"];

                musicPause.Style = playStyle;
                AutomationProperties.SetName(musicPause, "Play Music");

                Music.Pause();
                IsMusicPlaying = false;
                return;
            }

            if (Music != null && !IsMusicPlaying)
            {
                
                var playStyle = (Style)App.Current.Resources["PauseAppBarButtonStyle"];
                musicPause.Style = playStyle;
                AutomationProperties.SetName(musicPause, "Pause Music");
                Music.Play();
                IsMusicPlaying = true;
                return;
            }

        }

        private void btn_hint_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.HintsLeft <= 0)
            {
                var helpDialog = new MessageDialog("Sorry, you have used up all the hints. You will earn more hints by finishing this level", "No hints left");
                helpDialog.ShowAsync();
                return;
            }
            if (_viewModel.CurrentPaintingSpots == null || _viewModel.CurrentPaintingSpots.Spots == null)
                return;
            
            foreach (var spot in _viewModel.CurrentPaintingSpots.Spots)
            {
                 var X_value = spot.X * _viewModel.CanvasWidth;
                 var Y_value = spot.Y * _viewModel.CanvasHeight;

                if (!spot.Checked)
                {
                    spot.Checked = true;
                    spot.X = X_value;
                    spot.Y = Y_value;
                    _viewModel.ClickedSpot = spot;

                    var X = _viewModel.ClickedSpot.X - 16;
                    var Y = _viewModel.ClickedSpot.Y - 2;
                    RenderTags(canvasLeft, canvasRight, X, Y);
                    _viewModel.HintsLeft--;
                    AutomationProperties.SetName(hintButton,"Hints"+" ("+_viewModel.HintsLeft+" Left)");
                    break;
                }
            }
        }

    }

}
