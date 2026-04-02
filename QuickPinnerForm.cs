using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;

namespace QuickPinner
{
    public class QuickPinnerForm : Form
    {
        private const int BAR_HEIGHT = 60;
        private const int BUTTON_HEIGHT = 32;
        private const int LABEL_HEIGHT = 24;
        private const int SPACING = 5;

        // DWM API for dark title bar
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 19;
        private const int DWMWA_COLORIZATION_COLOR = 0x00000010;

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        private readonly string _configPath;
        // Config data
        private string[] _folderPaths = new string[5];
        private Size _windowSize = new Size(220, 325);
        private Point _windowLocation = new Point(0, 100);
        // UI controls
        private TextBox[] _folderLabels;
        private Button[] _selectButtons;
        private Button[] _openButtons;
        private Button[] _clearButtons;
        private int _currentBarCount = 0;

        public QuickPinnerForm()
        {
            _configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
            
            Text = "Atlas QuickPinner";
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            StartPosition = FormStartPosition.Manual;
            TopMost = true;
            BackColor = Color.FromArgb(32, 32, 32);
            using var iconStream = typeof(QuickPinnerForm).Assembly.GetManifestResourceStream("AtlasQuickPinner.pin-icon.ico");
            if (iconStream != null) Icon = new Icon(iconStream);

            LoadConfig();
            CreateControls();
            ApplySize();
            EnableDarkTitleBar();
        }

        private void EnableDarkTitleBar()
        {
            int useDark = 1;
            DwmSetWindowAttribute(Handle, DWMWA_USE_IMMERSIVE_DARK_MODE, ref useDark, sizeof(int));
        }

        private void LoadConfig()
        {
            if (File.Exists(_configPath))
            {
                var content = File.ReadAllText(_configPath);
                
                // Parse the custom format: {"size": {...}, "location": {...}, "paths": [...]}
                var doc = System.Text.Json.JsonDocument.Parse(content);
                var root = doc.RootElement;
                
                // Load folder paths
                if (root.TryGetProperty("paths", out var pathsElement))
                {
                    var paths = System.Text.Json.JsonSerializer.Deserialize<string[]>(pathsElement.GetRawText());
                    if (paths != null)
                    {
                        if (paths.Length > _folderPaths.Length)
                        {
                            Array.Resize(ref _folderPaths, paths.Length);
                        }
                        Array.Copy(paths, _folderPaths, paths.Length);
                    }
                }
                
                // Load window size
                if (root.TryGetProperty("size", out var sizeElement))
                {
                    var size = System.Text.Json.JsonSerializer.Deserialize<Size>(sizeElement.GetRawText());
                    if (size != null)
                    {
                        _windowSize = size;
                    }
                }
                
                // Load window location
                if (root.TryGetProperty("location", out var locElement))
                {
                    var loc = System.Text.Json.JsonSerializer.Deserialize<Point>(locElement.GetRawText());
                    if (loc != null)
                    {
                        _windowLocation = loc;
                    }
                }
            }
        }

        private void SaveConfig()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_configPath));
            
            // Format JSON with size and position on line 2
            var pathsJson = System.Text.Json.JsonSerializer.Serialize(_folderPaths);
            var configJson = $"{{\n  \"size\": {System.Text.Json.JsonSerializer.Serialize(Size)},\n  \"location\": {System.Text.Json.JsonSerializer.Serialize(Location)},\n  \"paths\": {pathsJson}\n}}";
            
            File.WriteAllText(_configPath, configJson);
        }

        private void CreateControls()
        {
            // Initialize arrays with 5 bars
            _folderLabels = new TextBox[5];
            _selectButtons = new Button[5];
            _openButtons = new Button[5];
            _clearButtons = new Button[5];
            _currentBarCount = 5;

            // Calculate button width to fit in window with some padding
            int buttonWidth = (220 - 15) / 2; // Leave room for padding
            
            for (int i = 0; i < 5; i++)
            {
                int y = 5 + i * BAR_HEIGHT;

                // Folder label (shows just the folder name)
                _folderLabels[i] = new TextBox
                {
                    Location = new Point(5, y),
                    Size = new Size(buttonWidth - 10, LABEL_HEIGHT),
                    ReadOnly = true,
                    BorderStyle = BorderStyle.None,
                    Font = new Font("Segoe UI", 8, FontStyle.Bold),
                    BackColor = Color.FromArgb(45, 45, 45),
                    ForeColor = Color.White,
                    Text = GetFolderName(_folderPaths[i]),
                    TextAlign = HorizontalAlignment.Center
                };

                // Clear button (right of folder name, above Open button)
                _clearButtons[i] = new Button
                {
                    Location = new Point(buttonWidth, y + 1),
                    Size = new Size(buttonWidth - 5, LABEL_HEIGHT - 4),
                    Text = "Clear",
                    Font = new Font("Segoe UI", 6),
                    BackColor = Color.FromArgb(180, 50, 50),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand,
                    Tag = i
                };
                _clearButtons[i].Click += ClearButton_Click;

                // Select button (left, below folder name)
                _selectButtons[i] = new Button
                {
                    Location = new Point(5, y + LABEL_HEIGHT + 2),
                    Size = new Size(buttonWidth - 5, BUTTON_HEIGHT),
                    Text = "Select",
                    Font = new Font("Segoe UI", 8),
                    BackColor = Color.FromArgb(50, 100, 200),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand,
                    Tag = i
                };
                _selectButtons[i].Click += SelectButton_Click;

                // Open button (right, below folder name)
                _openButtons[i] = new Button
                {
                    Location = new Point(buttonWidth, y + LABEL_HEIGHT + 2),
                    Size = new Size(buttonWidth - 5, BUTTON_HEIGHT),
                    Text = "Open",
                    Font = new Font("Segoe UI", 8),
                    BackColor = Color.FromArgb(50, 150, 50),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand,
                    Tag = i
                };
                _openButtons[i].Click += OpenButton_Click;

                Controls.Add(_folderLabels[i]);
                Controls.Add(_clearButtons[i]);
                Controls.Add(_selectButtons[i]);
                Controls.Add(_openButtons[i]);
            }
        }

        private string GetFolderName(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath))
                return "---";
            
            // Handle drive root (e.g., C:\)
            if (fullPath.EndsWith("\\") && fullPath.Length == 3 && fullPath[1] == ':')
                return fullPath.TrimEnd('\\');
            
            return Path.GetFileName(fullPath.TrimEnd('\\', '/'));
        }

        private void ClearButton_Click(object? sender, EventArgs e)
        {
            if (sender is Button btn && btn.Tag is int index)
            {
                _folderPaths[index] = null;
                _folderLabels[index].Text = "---";
                SaveConfig();
            }
        }

        private void SelectButton_Click(object? sender, EventArgs e)
        {
            if (sender is Button btn && btn.Tag is int index)
            {
                using var folderDialog = new FolderBrowserDialog();
                folderDialog.Description = "Select a folder";
                folderDialog.ShowNewFolderButton = true;

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    _folderPaths[index] = folderDialog.SelectedPath;
                    _folderLabels[index].Text = GetFolderName(_folderPaths[index]);
                    SaveConfig();
                }
            }
        }

        private void OpenButton_Click(object? sender, EventArgs e)
        {
            if (sender is Button btn && btn.Tag is int index)
            {
                if (!string.IsNullOrEmpty(_folderPaths[index]))
                {
                    try
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = _folderPaths[index],
                            UseShellExecute = true
                        });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            $"Could not open: {_folderPaths[index]}\n\n{ex.Message}",
                            "Quick Pinner",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("No folder selected", "Quick Pinner",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void ApplySize()
        {
            Size = _windowSize;
            Location = _windowLocation;
            
            // Calculate button width to fit in window with some padding
            int buttonWidth = (Size.Width - 15) / 2;
            for (int i = 0; i < _currentBarCount; i++)
            {
                int y = 5 + i * BAR_HEIGHT;
                _folderLabels[i].Size = new Size(buttonWidth - 10, LABEL_HEIGHT);
                _folderLabels[i].Location = new Point(5, y);
                _clearButtons[i].Size = new Size(buttonWidth - 5, LABEL_HEIGHT - 4);
                _clearButtons[i].Location = new Point(buttonWidth, y + 1);
                _selectButtons[i].Size = new Size(buttonWidth - 5, BUTTON_HEIGHT);
                _selectButtons[i].Location = new Point(5, y + LABEL_HEIGHT + 2);
                _openButtons[i].Size = new Size(buttonWidth - 5, BUTTON_HEIGHT);
                _openButtons[i].Location = new Point(buttonWidth, y + LABEL_HEIGHT + 2);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Refresh();
            BringToFront();
            Activate();
        }

        private void AddNewBar()
        {
            int newIndex = _currentBarCount;
            
            // Grow folder paths array if needed
            if (newIndex >= _folderPaths.Length)
            {
                Array.Resize(ref _folderPaths, newIndex + 1);
            }

            // Grow UI arrays
            TextBox[] newLabels = new TextBox[newIndex + 1];
            Array.Copy(_folderLabels, newLabels, _folderLabels.Length);
            _folderLabels = newLabels;

            Button[] newSelect = new Button[newIndex + 1];
            Array.Copy(_selectButtons, newSelect, _selectButtons.Length);
            _selectButtons = newSelect;

            Button[] newOpen = new Button[newIndex + 1];
            Array.Copy(_openButtons, newOpen, _openButtons.Length);
            _openButtons = newOpen;

            Button[] newClear = new Button[newIndex + 1];
            Array.Copy(_clearButtons, newClear, _clearButtons.Length);
            _clearButtons = newClear;

            // Create new controls - use existing folder path from _folderPaths
            int buttonWidth = (Size.Width - 15) / 2;
            int y = newIndex * BAR_HEIGHT;

            _folderLabels[newIndex] = new TextBox
            {
                Location = new Point(5, y),
                Size = new Size(buttonWidth - 10, LABEL_HEIGHT),
                ReadOnly = true,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                BackColor = Color.FromArgb(45, 45, 45),
                ForeColor = Color.White,
                Text = GetFolderName(_folderPaths[newIndex]),
                TextAlign = HorizontalAlignment.Center
            };

            // Clear button (right of folder name, above Open button)
            _clearButtons[newIndex] = new Button
            {
                Location = new Point(buttonWidth, y + 1),
                Size = new Size(buttonWidth - 5, LABEL_HEIGHT - 4),
                Text = "Clear",
                Font = new Font("Segoe UI", 6),
                BackColor = Color.FromArgb(180, 50, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Tag = newIndex
            };
            _clearButtons[newIndex].Click += ClearButton_Click;

            // Select button (left, below folder name)
            _selectButtons[newIndex] = new Button
            {
                Location = new Point(5, y + LABEL_HEIGHT + 2),
                Size = new Size(buttonWidth - 5, BUTTON_HEIGHT),
                Text = "Select",
                Font = new Font("Segoe UI", 8),
                BackColor = Color.FromArgb(50, 100, 200),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Tag = newIndex
            };
            _selectButtons[newIndex].Click += SelectButton_Click;

            // Open button (right, below folder name)
            _openButtons[newIndex] = new Button
            {
                Location = new Point(buttonWidth, y + LABEL_HEIGHT + 2),
                Size = new Size(buttonWidth - 5, BUTTON_HEIGHT),
                Text = "Open",
                Font = new Font("Segoe UI", 8),
                BackColor = Color.FromArgb(50, 150, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Tag = newIndex
            };
            _openButtons[newIndex].Click += OpenButton_Click;

            Controls.Add(_folderLabels[newIndex]);
            Controls.Add(_clearButtons[newIndex]);
            Controls.Add(_selectButtons[newIndex]);
            Controls.Add(_openButtons[newIndex]);
            
            _currentBarCount++;
        }

        private void RemoveLastBar()
        {
            int lastIndex = _currentBarCount - 1;
            
            // Remove from controls
            Controls.Remove(_folderLabels[lastIndex]);
            Controls.Remove(_clearButtons[lastIndex]);
            Controls.Remove(_selectButtons[lastIndex]);
            Controls.Remove(_openButtons[lastIndex]);

            // Shrink UI arrays only - _folderPaths stays intact!
            TextBox[] newLabels = new TextBox[lastIndex];
            Array.Copy(_folderLabels, newLabels, lastIndex);
            _folderLabels = newLabels;

            Button[] newSelect = new Button[lastIndex];
            Array.Copy(_selectButtons, newSelect, lastIndex);
            _selectButtons = newSelect;

            Button[] newOpen = new Button[lastIndex];
            Array.Copy(_openButtons, newOpen, lastIndex);
            _openButtons = newOpen;

            Button[] newClear = new Button[lastIndex];
            Array.Copy(_clearButtons, newClear, lastIndex);
            _clearButtons = newClear;
            
            _currentBarCount--;
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            
            // Skip if buttons haven't been created yet
            if (_currentBarCount == 0 || _selectButtons[0] == null || _clearButtons[0] == null)
                return;
            
            // Calculate how many bars fit in current height
            int availableHeight = Size.Height - 35; // Subtract frame height
            int barsThatFit = Math.Max(1, availableHeight / BAR_HEIGHT);
            
            // Ensure we have enough buttons
            while (_currentBarCount < barsThatFit)
            {
                AddNewBar();
            }
            
            // Trim excess buttons if window is smaller
            while (_currentBarCount > barsThatFit)
            {
                RemoveLastBar();
            }
            
            // Update all button positions
            int buttonWidth = (Size.Width - 15) / 2;
            for (int i = 0; i < _currentBarCount; i++)
            {
                int y = 5 + i * BAR_HEIGHT;
                _folderLabels[i].Size = new Size(buttonWidth - 10, LABEL_HEIGHT);
                _folderLabels[i].Location = new Point(5, y);
                _clearButtons[i].Size = new Size(buttonWidth - 5, LABEL_HEIGHT - 4);
                _clearButtons[i].Location = new Point(buttonWidth, y + 1);
                _selectButtons[i].Size = new Size(buttonWidth - 5, BUTTON_HEIGHT);
                _selectButtons[i].Location = new Point(5, y + LABEL_HEIGHT + 2);
                _openButtons[i].Size = new Size(buttonWidth - 5, BUTTON_HEIGHT);
                _openButtons[i].Location = new Point(buttonWidth, y + LABEL_HEIGHT + 2);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                BeginDrag();
            }
        }

        private bool _isDragging = false;
        private Point _dragOffset;

        private void BeginDrag()
        {
            _isDragging = true;
            _dragOffset = new Point(
                Control.MousePosition.X - Location.X,
                Control.MousePosition.Y - Location.Y);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (_isDragging)
            {
                Location = new Point(
                    Control.MousePosition.X - _dragOffset.X,
                    Control.MousePosition.Y - _dragOffset.Y);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _isDragging = false;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            TrimPaths();
            SaveConfig();
        }

        private void TrimPaths()
        {
            // Find last non-null entry
            int lastNonNull = -1;
            for (int i = _folderPaths.Length - 1; i >= 0; i--)
            {
                if (_folderPaths[i] != null)
                {
                    lastNonNull = i;
                    break;
                }
            }

            // Trim to last valid entry
            if (lastNonNull >= 0 && lastNonNull < _folderPaths.Length - 1)
            {
                Array.Resize(ref _folderPaths, lastNonNull + 1);
            }
        }
    }
}