using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Content.Client.Administration.Systems;
using Content.Client.Administration.UI.CustomControls;
using Content.Client.UserInterface.Controls;
using Content.Client.Verbs.UI;
using Content.Shared.Administration;
using Robust.Client.AutoGenerated;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Input;
using Robust.Shared.Utility;

namespace Content.Client._Starlight.MHelp.UI;
[GenerateTypedNameReferences]
public sealed partial class TicketListControl : BoxContainer
{
    private readonly IUserInterfaceManager _uiManager;
    private Guid? _selectedTicket;
    private readonly Dictionary<Guid, Ticket> _tickets = [];
    public Comparison<Ticket>? Comparison;
    public TicketListControl()
    {
        _uiManager = IoCManager.Resolve<IUserInterfaceManager>();
        RobustXamlLoader.Load(this);
        // Fill the Option data
        TicketListContainer.ItemPressed += PlayerListItemPressed;
        TicketListContainer.GenerateItem += GenerateButton;
        TicketListContainer.NoItemSelected += OnNoItemSelected;
        UpdateList();
        BackgroundPanel.PanelOverride = new StyleBoxFlat { BackgroundColor = new Color(32, 32, 40) };
    }

    public bool TryGetTicket(Guid id, [NotNullWhen(true)] out Ticket? ticket) => _tickets.TryGetValue(id, out ticket);

    public event Action<Guid?> OnSelectionChanged = delegate { };
    public void UpdateList()
    {
        var list = _tickets.Values.ToList();
        if (Comparison != null)
            list.Sort((a, b) => Comparison(a, b));
        TicketListContainer.PopulateList(list);

        if (_selectedTicket.HasValue && _tickets.TryGetValue(_selectedTicket.Value, out var ticket))
            TicketListContainer.Select(ticket);
    }
    private void OnNoItemSelected()
    {
        _selectedTicket = null;
        OnSelectionChanged?.Invoke(null);
    }

    private void PlayerListItemPressed(BaseButton.ButtonEventArgs? args, ListData? data)
    {
        if (args == null || data is not Ticket { Id: Guid ticketId })
            return;

        if (ticketId == _selectedTicket)
            return;

        if (args.Event.Function != EngineKeyFunctions.UIClick)
            return;

        OnSelectionChanged?.Invoke(ticketId);
        _selectedTicket = ticketId;
    }

    private void GenerateButton(ListData data, ListContainerButton button)
    {
        if (data is not Ticket ticket)
            return;

        var entry = new TicketEntry();
        entry.Setup(ticket);
        entry.OnPinStatusChanged += _ => UpdateList();

        button.AddChild(entry);
        button.AddStyleClass(ListContainer.StyleClassListContainerButton);
    }

    internal void EnsureTicket(Guid ticketId, string title, bool ticketClosed)
    {
        if(_tickets.TryGetValue(ticketId, out var ticket))
        {
            ticket.IsClosed = ticketClosed;
            return;
        }
        _tickets.TryAdd(ticketId, new Ticket(ticketId, title) { IsClosed = ticketClosed});
    }
}

public record Ticket(Guid Id, string Title) : ListData
{
    public bool IsPinned { get; set; }
    public bool IsClosed { get; set; }
}
