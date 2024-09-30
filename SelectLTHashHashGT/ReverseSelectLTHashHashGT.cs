using System;

namespace SelectLTHashHashGT
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class ReverseSelectLTHashHashGT : SelectorReverse
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 4134;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("765c9d5b-9b9b-4e24-8310-11270bb6041e");
        public ReverseSelectLTHashHashGT(SelectBlocksPackage package) : base(package, SelectLTHashHashGT.StartHasher(), SelectLTHashHashGT.EndHasher())
        {
            AddCommand(CommandSet, CommandId);
        }
    }
}
