using System;

namespace SelectLTHashHashGT
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class BlockSelection : Selector
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 4126;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("765c9d5b-9b9b-4e24-8310-11270bb6041e");
        public BlockSelection(SelectBlocksPackage package) : base(package)
        {
            AddCommand(CommandSet, CommandId);
        }
        public override void DoTheJob()
        {
            this.MenuCommand.Visible = false;
        } // do nothing
    }
}
