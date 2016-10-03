namespace bitkyFlashresUniversal.connClient.model.commtUtil.ConnClient
{
    public abstract class BitkyClient
    {

        public abstract void Send(byte[] bytes);
        public abstract void Send(byte[] bytes, int timeInterval);
        public abstract void GetCallback();
        public abstract void Close();
    }
}