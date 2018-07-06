namespace RS
{
    /// <summary>
    /// A menu action that handles game frame tab changes.
    /// </summary>
    public class ChangeTabAction : AbstractMenuAction
    {
        private int tabIndex;

        public ChangeTabAction(int tabIndex, string caption, bool hasPriority)
        {
            this.tabIndex = tabIndex;
            base.Caption = caption;
            base.HasPriority = hasPriority;
        }

        public override void Callback(ActionMenu menu)
        {
            GameContext.TabArea.SelectedTabIndex = tabIndex;
        }
    }
}