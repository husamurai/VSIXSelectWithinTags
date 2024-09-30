using System;

namespace SelectLTHashHashGT
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class ReverseSelectParanthesisedBlock : SelectorReverse
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 4136;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("765c9d5b-9b9b-4e24-8310-11270bb6041e");

        public ReverseSelectParanthesisedBlock(SelectBlocksPackage package) : base(package, SelectParanthesisedBlock.StartParanthesis(), SelectParanthesisedBlock.EndParanthesis())
        {
            AddCommand(CommandSet, CommandId);
        }
    }
}
