using System;

namespace SelectLTHashHashGT
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class SelectLTHashHashGT : Selector
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 4129; // 9001; instead of 0x0100 does not work;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("765c9d5b-9b9b-4e24-8310-11270bb6041e");

        public SelectLTHashHashGT(SelectBlocksPackage package) : base(package, StartHasher(), EndHasher())
        {
            AddCommand(CommandSet, CommandId);
        }
        public static string EndHasher() => "#>";
        public static string StartHasher() => "<#";
    }
}
