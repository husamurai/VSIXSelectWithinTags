using System;

namespace SelectLTHashHashGT
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class SelectComment : Selector
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 4133;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("765c9d5b-9b9b-4e24-8310-11270bb6041e");

        public SelectComment(SelectBlocksPackage package) : base(package, StartComment(), EndComment())
        {
            AddCommand(CommandSet, CommandId);
        }
        public static string EndComment() => "*/";
        public static string StartComment() => "/*";
    }
}
