using System;

namespace SelectLTHashHashGT
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class ReverseSelectComment : SelectorReverse
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 4138;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("765c9d5b-9b9b-4e24-8310-11270bb6041e");

        public ReverseSelectComment(SelectBlocksPackage package) : base(package, SelectComment.StartComment(), SelectComment.EndComment())
        {
            AddCommand(CommandSet, CommandId);
        }
    }
}
