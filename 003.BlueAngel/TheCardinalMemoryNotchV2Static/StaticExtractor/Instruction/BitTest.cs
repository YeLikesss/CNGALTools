namespace StaticExtractor.Instruction
{
    internal class BitTest
    {
        /// <summary>
        /// bt reg32, reg32
        /// </summary>
        public static bool Bt(uint val, uint pos)
        {
            pos &= 0x1Fu;
            return (val & (1u << (int)pos)) != 0u;
        }

        /// <summary>
        /// bts reg32, reg32
        /// </summary>
        public static uint Bts(uint val, uint pos)
        {
            pos &= 0x1Fu;
            return val | (1u << (int)pos);
        }
    }
}
