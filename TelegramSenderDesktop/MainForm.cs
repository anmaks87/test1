using System.ComponentModel;

namespace TelegramSenderDesktop;

public sealed class MainForm : Form
{
    private readonly AppSettingsStore _settingsStore;
    private readonly TelegramService _telegramService;
    private readonly BindingList<Recipient> _recipients;

    private AppSettings _settings;

    private readonly TextBox _botTokenTextBox = new();
    private readonly TextBox _messageTextBox = new();
    private readonly TextBox _testMessageTextBox = new();
    private readonly CheckedListBox _recipientCheckedList = new();
    private readonly ComboBox _singleRecipientComboBox = new();
    private readonly TextBox _newRecipientNameTextBox = new();
    private readonly TextBox _newRecipientChatIdTextBox = new();
    private readonly Label _statusLabel = new();
    private readonly Button _sendSelectedButton = new();
    private readonly Button _sendTestButton = new();
    private readonly Button _addRecipientButton = new();
    private readonly Button _removeRecipientButton = new();
    private readonly Button _saveSettingsButton = new();

    public MainForm(AppSettingsStore settingsStore, TelegramService telegramService)
    {
        _settingsStore = settingsStore;
        _telegramService = telegramService;
        _settings = _settingsStore.Load();
        _recipients = new BindingList<Recipient>(_settings.Recipients);

        InitializeComponent();
        LoadSettingsIntoControls();
        RefreshRecipientViews();
    }

    private void InitializeComponent()
    {
        SuspendLayout();

        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(980, 720);
        MinimumSize = new Size(900, 680);
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Telegram Sender Desktop";

        var rootLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4,
            Padding = new Padding(16)
        };
        rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        var credentialsGroup = CreateCredentialsGroup();
        var contentLayout = CreateContentLayout();
        var actionsPanel = CreateActionsPanel();

        _statusLabel.AutoSize = true;
        _statusLabel.Margin = new Padding(0, 12, 0, 0);
        _statusLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        _statusLabel.Text = "Готово до роботи";

        rootLayout.Controls.Add(credentialsGroup, 0, 0);
        rootLayout.Controls.Add(contentLayout, 0, 1);
        rootLayout.Controls.Add(actionsPanel, 0, 2);
        rootLayout.Controls.Add(_statusLabel, 0, 3);

        Controls.Add(rootLayout);
        ResumeLayout(false);
    }

    private GroupBox CreateCredentialsGroup()
    {
        var group = new GroupBox
        {
            Dock = DockStyle.Top,
            Text = "Налаштування бота",
            Padding = new Padding(12),
            AutoSize = true
        };

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            ColumnCount = 3,
            RowCount = 2,
            AutoSize = true
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

        var tokenLabel = new Label
        {
            Text = "Токен бота",
            AutoSize = true,
            Anchor = AnchorStyles.Left,
            Margin = new Padding(0, 8, 12, 0)
        };

        _botTokenTextBox.Dock = DockStyle.Fill;
        _botTokenTextBox.UseSystemPasswordChar = true;
        _botTokenTextBox.Margin = new Padding(0, 4, 12, 0);

        var helpLabel = new Label
        {
            AutoSize = true,
            ForeColor = Color.DimGray,
            Text = "Токен зберігається локально у файлі settings.json",
            Margin = new Padding(0, 8, 0, 0)
        };

        _saveSettingsButton.Text = "Зберегти";
        _saveSettingsButton.AutoSize = true;
        _saveSettingsButton.Click += SaveSettingsButton_Click;

        layout.Controls.Add(tokenLabel, 0, 0);
        layout.Controls.Add(_botTokenTextBox, 1, 0);
        layout.Controls.Add(_saveSettingsButton, 2, 0);
        layout.Controls.Add(helpLabel, 1, 1);

        group.Controls.Add(layout);
        return group;
    }

    private TableLayoutPanel CreateContentLayout()
    {
        var contentLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Margin = new Padding(0, 16, 0, 0)
        };
        contentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55F));
        contentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45F));

        contentLayout.Controls.Add(CreateRecipientsGroup(), 0, 0);
        contentLayout.Controls.Add(CreateMessagesGroup(), 1, 0);

        return contentLayout;
    }

    private GroupBox CreateRecipientsGroup()
    {
        var group = new GroupBox
        {
            Dock = DockStyle.Fill,
            Text = "Отримувачі",
            Padding = new Padding(12)
        };

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        _recipientCheckedList.Dock = DockStyle.Fill;
        _recipientCheckedList.CheckOnClick = true;
        _recipientCheckedList.DisplayMember = nameof(Recipient.Name);
        _recipientCheckedList.Font = new Font("Segoe UI", 11F);

        var singleRecipientPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            AutoSize = true,
            WrapContents = false,
            Margin = new Padding(0, 12, 0, 0)
        };

        var singleRecipientLabel = new Label
        {
            Text = "Отримувач для тесту",
            AutoSize = true,
            Margin = new Padding(0, 8, 12, 0)
        };

        _singleRecipientComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        _singleRecipientComboBox.Width = 240;

        singleRecipientPanel.Controls.Add(singleRecipientLabel);
        singleRecipientPanel.Controls.Add(_singleRecipientComboBox);

        var addPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            AutoSize = true,
            Margin = new Padding(0, 16, 0, 0)
        };
        addPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45F));
        addPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45F));
        addPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

        _newRecipientNameTextBox.PlaceholderText = "Ім'я";
        _newRecipientNameTextBox.Dock = DockStyle.Fill;

        _newRecipientChatIdTextBox.PlaceholderText = "Chat ID";
        _newRecipientChatIdTextBox.Dock = DockStyle.Fill;

        _addRecipientButton.Text = "Додати";
        _addRecipientButton.AutoSize = true;
        _addRecipientButton.Click += AddRecipientButton_Click;

        addPanel.Controls.Add(_newRecipientNameTextBox, 0, 0);
        addPanel.Controls.Add(_newRecipientChatIdTextBox, 1, 0);
        addPanel.Controls.Add(_addRecipientButton, 2, 0);

        _removeRecipientButton.Text = "Видалити вибраних";
        _removeRecipientButton.AutoSize = true;
        _removeRecipientButton.Margin = new Padding(0, 16, 0, 0);
        _removeRecipientButton.Click += RemoveRecipientButton_Click;

        layout.Controls.Add(_recipientCheckedList, 0, 0);
        layout.Controls.Add(singleRecipientPanel, 0, 1);
        layout.Controls.Add(addPanel, 0, 2);
        layout.Controls.Add(_removeRecipientButton, 0, 3);

        group.Controls.Add(layout);
        return group;
    }

    private GroupBox CreateMessagesGroup()
    {
        var group = new GroupBox
        {
            Dock = DockStyle.Fill,
            Text = "Повідомлення",
            Padding = new Padding(12)
        };

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4
        };
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

        var selectedLabel = new Label
        {
            Text = "Текст для кнопки \"Відправити\"",
            AutoSize = true
        };

        _messageTextBox.Dock = DockStyle.Fill;
        _messageTextBox.Multiline = true;
        _messageTextBox.ScrollBars = ScrollBars.Vertical;
        _messageTextBox.Font = new Font("Segoe UI", 11F);

        var testLabel = new Label
        {
            Text = "Текст для кнопки \"Відправити тест\"",
            AutoSize = true,
            Margin = new Padding(0, 12, 0, 0)
        };

        _testMessageTextBox.Dock = DockStyle.Fill;
        _testMessageTextBox.Multiline = true;
        _testMessageTextBox.ScrollBars = ScrollBars.Vertical;
        _testMessageTextBox.Font = new Font("Segoe UI", 11F);

        layout.Controls.Add(selectedLabel, 0, 0);
        layout.Controls.Add(_messageTextBox, 0, 1);
        layout.Controls.Add(testLabel, 0, 2);
        layout.Controls.Add(_testMessageTextBox, 0, 3);

        group.Controls.Add(layout);
        return group;
    }

    private FlowLayoutPanel CreateActionsPanel()
    {
        var panel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            AutoSize = true,
            WrapContents = false,
            Margin = new Padding(0, 16, 0, 0)
        };

        _sendSelectedButton.Text = "Відправити";
        _sendSelectedButton.AutoSize = true;
        _sendSelectedButton.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        _sendSelectedButton.Click += SendSelectedButton_Click;

        _sendTestButton.Text = "Відправити тест";
        _sendTestButton.AutoSize = true;
        _sendTestButton.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        _sendTestButton.Click += SendTestButton_Click;

        panel.Controls.Add(_sendSelectedButton);
        panel.Controls.Add(_sendTestButton);

        return panel;
    }

    private void LoadSettingsIntoControls()
    {
        _botTokenTextBox.Text = _settings.BotToken;
        _messageTextBox.Text = _settings.DefaultMessage;
        _testMessageTextBox.Text = _settings.TestMessage;
    }

    private void RefreshRecipientViews()
    {
        _recipientCheckedList.Items.Clear();

        foreach (var recipient in _recipients)
        {
            _recipientCheckedList.Items.Add(recipient, false);
        }

        _singleRecipientComboBox.DataSource = null;
        _singleRecipientComboBox.DataSource = _recipients.ToList();
        _singleRecipientComboBox.DisplayMember = nameof(Recipient.Name);

        if (_singleRecipientComboBox.Items.Count > 0)
        {
            _singleRecipientComboBox.SelectedIndex = 0;
        }
    }

    private void SaveSettings()
    {
        _settings.BotToken = _botTokenTextBox.Text.Trim();
        _settings.DefaultMessage = _messageTextBox.Text.Trim();
        _settings.TestMessage = _testMessageTextBox.Text.Trim();
        _settings.Recipients = _recipients.ToList();
        _settingsStore.Save(_settings);
    }

    private bool ValidateToken()
    {
        if (!string.IsNullOrWhiteSpace(_botTokenTextBox.Text))
        {
            return true;
        }

        SetStatus("Заповни токен бота");
        return false;
    }

    private void SetStatus(string text)
    {
        _statusLabel.Text = text;
    }

    private async Task SendMessagesAsync(IEnumerable<Recipient> recipients, string messageText)
    {
        var recipientList = recipients.ToList();

        if (!ValidateToken())
        {
            return;
        }

        if (recipientList.Count == 0)
        {
            SetStatus("Немає вибраних отримувачів");
            return;
        }

        if (string.IsNullOrWhiteSpace(messageText))
        {
            SetStatus("Введи текст повідомлення");
            return;
        }

        if (recipientList.Any(r => string.IsNullOrWhiteSpace(r.ChatId)))
        {
            SetStatus("У одного з отримувачів не заповнений Chat ID");
            return;
        }

        ToggleBusyState(true);
        SetStatus("Відправляю повідомлення...");

        try
        {
            SaveSettings();

            foreach (var recipient in recipientList)
            {
                await _telegramService.SendMessageAsync(
                    _settings.BotToken,
                    recipient.ChatId,
                    messageText,
                    CancellationToken.None);
            }

            SetStatus($"Успішно відправлено: {recipientList.Count} отримувач.");
        }
        catch (Exception ex)
        {
            SetStatus($"Помилка: {ex.Message}");
        }
        finally
        {
            ToggleBusyState(false);
        }
    }

    private void ToggleBusyState(bool isBusy)
    {
        _sendSelectedButton.Enabled = !isBusy;
        _sendTestButton.Enabled = !isBusy;
        _addRecipientButton.Enabled = !isBusy;
        _removeRecipientButton.Enabled = !isBusy;
        _saveSettingsButton.Enabled = !isBusy;
    }

    private async void SendSelectedButton_Click(object? sender, EventArgs e)
    {
        var recipients = _recipientCheckedList.CheckedItems.Cast<Recipient>().ToList();
        await SendMessagesAsync(recipients, _messageTextBox.Text);
    }

    private async void SendTestButton_Click(object? sender, EventArgs e)
    {
        if (_singleRecipientComboBox.SelectedItem is not Recipient recipient)
        {
            SetStatus("Оберіть отримувача для тесту");
            return;
        }

        await SendMessagesAsync([recipient], _testMessageTextBox.Text);
    }

    private void AddRecipientButton_Click(object? sender, EventArgs e)
    {
        var name = _newRecipientNameTextBox.Text.Trim();
        var chatId = _newRecipientChatIdTextBox.Text.Trim();

        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(chatId))
        {
            SetStatus("Заповни ім'я та Chat ID");
            return;
        }

        if (_recipients.Any(r => string.Equals(r.Name, name, StringComparison.OrdinalIgnoreCase)))
        {
            SetStatus("Отримувач з таким ім'ям вже існує");
            return;
        }

        _recipients.Add(new Recipient
        {
            Name = name,
            ChatId = chatId
        });

        _newRecipientNameTextBox.Clear();
        _newRecipientChatIdTextBox.Clear();
        RefreshRecipientViews();
        SaveSettings();
        SetStatus($"Додано отримувача: {name}");
    }

    private void RemoveRecipientButton_Click(object? sender, EventArgs e)
    {
        var recipientsToRemove = _recipientCheckedList.CheckedItems.Cast<Recipient>().ToList();

        if (recipientsToRemove.Count == 0)
        {
            SetStatus("Познач отримувачів, яких потрібно видалити");
            return;
        }

        foreach (var recipient in recipientsToRemove)
        {
            _recipients.Remove(recipient);
        }

        RefreshRecipientViews();
        SaveSettings();
        SetStatus($"Видалено отримувачів: {recipientsToRemove.Count}");
    }

    private void SaveSettingsButton_Click(object? sender, EventArgs e)
    {
        SaveSettings();
        SetStatus("Налаштування збережено");
    }
}
