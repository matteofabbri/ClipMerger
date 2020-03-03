using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Media.Editing;
using Windows.Storage.Pickers;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace FileMerger
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private MediaComposition composition;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void Select_OnClick(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker {SuggestedStartLocation = PickerLocationId.Desktop};
            picker.FileTypeFilter.Add(".mp4");

            var pickedFiles = await picker.PickMultipleFilesAsync();

            if (pickedFiles == null) return;

            composition = new MediaComposition();
            var files = pickedFiles.OrderBy(x => x.Name).ToArray();

            foreach (var file in files)
            {
                var clip = await MediaClip.CreateFromFileAsync(file);
                composition.Clips.Add(clip);
            }
        }

        private async void Merge_OnClick(object sender, RoutedEventArgs e)
        {
            var pickerSave = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.VideosLibrary
            };
            pickerSave.FileTypeChoices.Add("MP4 files", new List<string>() { ".mp4" });
            pickerSave.SuggestedFileName = "merged clips.mp4";

            var saveFile = await pickerSave.PickSaveFileAsync();
            if (saveFile == null) return;

            var saveOperation = composition.RenderToFileAsync(saveFile, MediaTrimmingPreference.Precise);
            saveOperation.Progress = async (info, progress) =>
            {
                await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(() =>
                {
                    TextBlock.Text = $"Rendering... Progress: {progress:F0}%";
                }));
            };
        }
    }
}
