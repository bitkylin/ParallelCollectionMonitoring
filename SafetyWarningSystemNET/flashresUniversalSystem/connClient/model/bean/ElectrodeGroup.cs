namespace bitkyFlashresUniversal.connClient.model.bean
{
    /// <summary>
    ///     一组电极，这组电极包括类型A、B、M的编号
    /// </summary>
    internal class ElectrodeGroup
    {
        public int TypeA { get; set; } = -1;

        public int TypeB { get; set; } = -1;

        public int TypeM { get; set; } = -1;

        public bool IsReady()
        {
            return (TypeA != -1) && (TypeB != -1) && (TypeM != -1);
        }
    }
}