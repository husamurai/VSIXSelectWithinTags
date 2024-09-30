using System;

namespace SelectLTHashHashGT
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class SelectParanthesisedBlock : Selector
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 4130;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("765c9d5b-9b9b-4e24-8310-11270bb6041e");
        public SelectParanthesisedBlock(SelectBlocksPackage package) : base(package, StartParanthesis(), EndParanthesis())
        {
            AddCommand(CommandSet, CommandId);
        }

        public static string EndParanthesis() => "}";
        public static string StartParanthesis() => "{";

    }
}
