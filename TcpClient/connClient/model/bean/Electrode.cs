namespace bitkyFlashresUniversal.connClient.model.bean
{
    public class Electrode
    {
        public Electrode(int idOrigin)
        {
            IdOrigin = idOrigin;
        }

        private Electrode(int idOrigin, int idCurrent)
        {
            IdOrigin = idOrigin;
            IdCurrent = idCurrent;
        }

        public Electrode(int idOrigin, int idCurrent, PoleMode mode)
        {
            IdOrigin = idOrigin;
            IdCurrent = idCurrent;
            Mode = mode;
        }

        public Electrode(int idOrigin, PoleMode mode)
        {
            IdOrigin = idOrigin;
            Mode = mode;
        }

        public int IdOrigin { get; }

        public int IdCurrent { get; set; }

        public PoleMode Mode { get; private set; } = PoleMode.N;
        public int Value { get; set; }

        /// <summary>
        ///     克隆当前对象的原始值和当前值，并指定电极的类型
        /// </summary>
        /// <param name="mode">指定电极的类型</param>
        /// <returns>克隆后的电极对象</returns>
        public Electrode Clone(PoleMode mode)
        {
            var pole = new Electrode(IdOrigin, IdCurrent) {Mode = mode};
            return pole;
        }
    }
}