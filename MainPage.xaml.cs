using System.Collections.ObjectModel;
using System.Text.Json;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Maui.Media;

namespace MauiApp2
{
    public partial class MainPage : ContentPage
    {
        private ObservableCollection<SongModel> _songs = new();
        private readonly string _songsFilePath = Path.Combine(FileSystem.AppDataDirectory, "songs.json");
        private int _currentIndex = -1;

        public MainPage()
        {
            InitializeComponent();
            ListaPiosenek.ItemsSource = _songs;
            _ = WczytajPiosenkiAsync();
        }

        private async void Add_Clicked(object sender, EventArgs e)
        {
            try
            {
                var result = await FilePicker.PickMultipleAsync(new PickOptions
                {
                    PickerTitle = "Wybierz pliki muzyczne",
                    FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                    {
                        { DevicePlatform.WinUI, new[] { ".mp3", ".wav", ".flac" } },
                        { DevicePlatform.Android, new[] { "audio/*" } },
                        { DevicePlatform.iOS, new[] { "public.audio" } }
                    })
                });

                if (result != null)
                {
                    foreach (var file in result)
                    {
                        _songs.Add(new SongModel
                        {
                            Title = Path.GetFileNameWithoutExtension(file.FileName),
                            Path = file.FullPath
                        });
                    }
                }
                await ZapiszPiosenkiAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Błąd", $"Nie udało się wybrać plików: {ex.Message}", "OK");
            }
        }

        private async Task ZapiszPiosenkiAsync()
        {
            try
            {
                var json = JsonSerializer.Serialize(_songs);
                await File.WriteAllTextAsync(_songsFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd zapisu piosenek: {ex.Message}");
            }
        }

        private async Task WczytajPiosenkiAsync()
        {
            try
            {
                if (File.Exists(_songsFilePath))
                {
                    var json = await File.ReadAllTextAsync(_songsFilePath);
                    var loadedSongs = JsonSerializer.Deserialize<ObservableCollection<SongModel>>(json);

                    if (loadedSongs != null)
                    {
                        _songs.Clear();
                        foreach (var song in loadedSongs)
                            _songs.Add(song);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd odczytu piosenek: {ex.Message}");
            }
        }

       /* private void dalej_Clicked(object sender, EventArgs e)
        {
            if (_songs.Count == 0) return;
            if (_currentIndex < _songs.Count - 1) _currentIndex++;
            else _currentIndex = 0;

            Odtwarzacz.Source = _songs[_currentIndex].Path;
            Odtwarzacz.Play();
            AktualnaPiosenka.Text = $"▶ {_songs[_currentIndex].Title}";
        }

        private void cofnij_Clicked(object sender, EventArgs e)
        {
            if (_songs.Count == 0) return;
            if (_currentIndex > 0) _currentIndex--;
            else _currentIndex = _songs.Count - 1;

            Odtwarzacz.Source = _songs[_currentIndex].Path;
            Odtwarzacz.Play();
            AktualnaPiosenka.Text = $"▶ {_songs[_currentIndex].Title}";
        }

        private void pauza_Clicked(object sender, EventArgs e)
        {
            if (Odtwarzacz.CurrentState == MediaElementState.Playing)
            {
                Odtwarzacz.Pause();
                AktualnaPiosenka.Text = $"⏸ {_songs[_currentIndex].Title}";
            }
            else if (Odtwarzacz.CurrentState == MediaElementState.Paused)
            {
                Odtwarzacz.Play();
                AktualnaPiosenka.Text = $"▶ {_songs[_currentIndex].Title}";
            }
            else
            {
                DisplayAlert("Info", "Brak odtwarzania do wznowienia.", "OK");
            }
        }

        private void shuffle_Clicked(object sender, EventArgs e)
        {
            if (_songs.Count == 0) return;
            var random = new Random();
            _currentIndex = random.Next(_songs.Count);

            Odtwarzacz.Source = _songs[_currentIndex].Path;
            Odtwarzacz.Play();
            AktualnaPiosenka.Text = $"▶ {_songs[_currentIndex].Title}";
        }*/
    }
}
