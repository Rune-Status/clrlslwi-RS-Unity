namespace RS
{
    /// <summary>
    /// A menu action that handles a click on a widget.
    /// </summary>
    public class WidgetAction : AbstractMenuAction
    {
        private Widget widget;

        public WidgetAction(Widget widget, string caption, bool hasPriority)
        {
            this.widget = widget;
            base.Caption = caption;
            base.HasPriority = hasPriority;
        }

        public override void Callback(ActionMenu menu)
        {
            switch (widget.Config.OptionType)
            {
                case 2:
                    var prefix = widget.Config.OptionPrefix;
                    if (prefix.IndexOf(' ') != -1)
                    {
                        prefix = prefix.Substring(0, prefix.IndexOf(' '));
                    }

                    var suffix = widget.Config.OptionPrefix;
                    if (suffix.IndexOf(' ') != -1)
                    {
                        suffix = suffix.Substring(suffix.IndexOf(' ') + 1);
                    }

                    var tooltip = prefix + ' ' + widget.Config.OptionSuffix + ' ' + suffix;
                    GameContext.SetSelectedWidget(widget.Config.Index, -1, tooltip);
                    break;
                case 3:
                    GameContext.CloseWidgets();
                    break;
                case 1:
                case 4:
                case 5:
                    {
                        var @out = new Packet(185);
                        @out.WriteShort(widget.Config.Index);
                        GameContext.NetworkHandler.Write(@out);
                        break;
                    }
                case 6:
                    {
                        var @out = new Packet(40);
                        @out.WriteShort(widget.Config.Index);
                        GameContext.NetworkHandler.Write(@out);
                        break;
                    }
            }
        }
    }
}
